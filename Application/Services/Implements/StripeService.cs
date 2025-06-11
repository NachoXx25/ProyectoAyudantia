using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotenv.net.Utilities;
using Microsoft.AspNetCore.Identity;
using Proyecto_web_api.Application.DTOs.StripeDTOs;
using Proyecto_web_api.Application.Services.Interfaces;
using Proyecto_web_api.Domain.Models;
using Proyecto_web_api.Infrastructure.Repositories.Interfaces;
using Serilog;
using Stripe;
using Subscription = Proyecto_web_api.Domain.Models.Subscription;
namespace Proyecto_web_api.Application.Services.Implements
{
    public class StripeService : IStripeService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IStripeRepository _stripeRepository;
        public StripeService(UserManager<User> userManager, RoleManager<Role> roleManager, IStripeRepository stripeRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _stripeRepository = stripeRepository;
            StripeConfiguration.ApiKey = EnvReader.GetStringValue("STRIPE_SECRET_KEY");
        }

        /// <summary>
        /// Confirma un pago utilizando el PaymentIntent de Stripe.
        /// </summary>
        /// <param name="userId">Id del usuario que realiza el pago.</param>
        /// <param name="request">Objeto que contiene el PaymentIntentId</param>
        /// <returns>True si el pago se confirma correctamente, false si no.</returns>
        public async Task<bool> ConfirmPaymentAsync(int userId, ConfirmPaymentRequestDto request)
        {
            try
            {
                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = await paymentIntentService.GetAsync(request.PaymentIntentId);

                if (paymentIntent.Status == "succeeded")
                {
                    await ActivatePremiumAsync(userId, paymentIntent.Id);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ha ocurrido un error al confirmar el pago para el usuario {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Crea un PaymentIntent para procesar un pago.
        /// </summary>
        /// <param name="userId">Id del usuario que realiza el pago.</param>
        /// <param name="request">Objeto que contiene la información del pago.</param>
        /// <returns>Un objeto PaymentResponseDto que contiene el ClientSecret y el PaymentIntentId.</returns>
        public async Task<PaymentResponseDto> CreatePaymentIntentAsync(int userId, CreatePaymentRequestDto request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new Exception("Usuario no encontrado.");
                if (await IsUserPremiumAsync(userId)) throw new Exception("El usuario ya es premium.");

                if (string.IsNullOrEmpty(user.StripeCustomerId))
                {
                    Log.Information("El usuario {UserId} no tiene un Customer ID de Stripe, creando uno nuevo.", userId);
                    var customerService = new CustomerService();
                    var customer = await customerService.CreateAsync(new CustomerCreateOptions
                    {
                        Email = user.Email,
                        Metadata = new Dictionary<string, string>
                        {
                            { "UserId", userId.ToString() }
                        }
                    });
                    user.StripeCustomerId = customer.Id;
                    await _userManager.UpdateAsync(user);
                }

                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
                {
                    Amount = (long)request.Amount,
                    Currency = request.Currency.ToLower(),
                    Customer = user.StripeCustomerId,
                    Metadata = new Dictionary<string, string>
                    {
                        { "UserId", userId.ToString() },
                        { "ProductType", "Premium" }
                    },
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true,
                        AllowRedirects = "never"
                    },
                    
                });
                var subscription = new Subscription
                {
                    UserId = userId,
                    StripeSubscriptionId = paymentIntent.Id,
                    Amount = request.Amount,
                    StripePaymentIntentId = paymentIntent.Id
                };

                await _stripeRepository.AddSubscriptionAsync(subscription);

                return new PaymentResponseDto
                {
                    ClientSecret = paymentIntent.ClientSecret,
                    PaymentIntentId = paymentIntent.Id,
                    SubscriptionId = subscription.Id,
                    PaymentStatus = paymentIntent.Status
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ha ocurrido un error al crear el PaymentIntent para el usuario {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Maneja el webhook de Stripe para eventos relacionados con pagos y suscripciones.
        /// </summary>
        /// <param name="json">El cuerpo del webhook en formato JSON.</param>
        /// <param name="stripeSignature">La firma del webhook de Stripe.</param>
        public async Task HandleWebhookAsync(string json, string stripeSignature)
        {
            try
            {
                var webhookSecret = EnvReader.GetStringValue("STRIPE_WEBHOOK_SECRET");
                var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, webhookSecret);

                switch (stripeEvent.Type)
                {
                    case "payment_intent.succeeded":
                        await HandlePaymentSucceeded(stripeEvent);
                        break;
                    case "payment_intent.payment_failed":
                        await HandlePaymentFailed(stripeEvent);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ha ocurrido un error al manejar el webhook de Stripe.");
                throw;
            }
        }

        /// <summary>
        /// Chequea si un usuario es premium.
        /// </summary>
        /// <param name="userId">Id del usuario</param>
        /// <returns>True si es premium, false si no</returns> 
        public async Task<bool> IsUserPremiumAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var premiumRole = await _roleManager.FindByNameAsync("Premium");
            if (user == null || premiumRole == null)
            {
                Log.Error("Usuario o rol Premium no encontrado. UserId: {UserId}, PremiumRole: {PremiumRole}", userId, premiumRole?.Name);
                return false;
            }

            return user.RoleId == premiumRole.Id;
        }
        private async Task ActivatePremiumAsync(int userId, string stripeSubscriptionId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return;

            if (await IsUserPremiumAsync(userId))
            {
                Log.Information("El usuario {UserId} ya es premium, no se requiere activación.", userId);
                return;
            }

            var premiumRole = await _roleManager.FindByNameAsync("Premium");
            if (premiumRole == null)
            {
                Log.Error("El rol Premium no existe, no se puede activar la suscripción para el usuario {UserId}.", userId);
                return;
            }
            user.RoleId = premiumRole.Id;
            await _userManager.UpdateAsync(user);

            var subscription = await _stripeRepository.GetSubscriptionByStripeIdAsync(userId.ToString(), stripeSubscriptionId);
            if (subscription != null)
            {
                subscription.PaymentStatus = "completed";
                subscription.UpdatedAt = DateTime.UtcNow;
                await _stripeRepository.UpdateSubscriptionAsync(subscription);
            }
            Log.Information("El usuario {UserId} ha sido activado como Premium.", userId);
        }

        /// <summary>
        /// Maneja el evento de pago exitoso.
        /// </summary>
        /// <param name="stripeEvent">El evento de Stripe que contiene la información del pago.</param>
        private async Task HandlePaymentSucceeded(Event stripeEvent)
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent?.Metadata.TryGetValue("UserId", out var userIdStr) == true)
            {
                if (int.TryParse(userIdStr, out int userId))
                {
                    await ActivatePremiumAsync(userId, paymentIntent.Id);
                }
            }
        }
        
        /// <summary>
        /// Maneja el evento de pago fallido.
        /// </summary>
        /// <param name="stripeEvent">El evento de Stripe que contiene la información del pago.</param>
        private async Task HandlePaymentFailed(Event stripeEvent)
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent?.Metadata.TryGetValue("UserId", out var userIdStr) == true)
            {
                if (int.TryParse(userIdStr, out int userId))
                {
                    Log.Warning("El pago del usuario {UserId} ha fallado. PaymentIntent ID: {PaymentIntentId}", userId, paymentIntent.Id);
                    var subscription = await _stripeRepository.GetSubscriptionByStripeIdAsync(userId.ToString(), paymentIntent.Id);
                    if (subscription != null)
                    {
                        subscription.PaymentStatus = "failed";
                        subscription.UpdatedAt = DateTime.UtcNow;
                        await _stripeRepository.UpdateSubscriptionAsync(subscription);
                    }
                    Log.Information("La suscripción del usuario {UserId} ha sido actualizada a estado 'failed'.", userId);
                }
            }
        }
    }
}