using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_web_api.Application.DTOs.StripeDTOs;
using Proyecto_web_api.Application.Services.Interfaces;
using Serilog;
using Stripe;

namespace Proyecto_web_api.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StripeController : ControllerBase
    {
        private readonly IStripeService _stripeService;
        public StripeController(IStripeService stripeService)
        {
            _stripeService = stripeService;
        }


        /// <summary>
        /// Endpoint que recibe webhooks de Stripe
        /// </summary>
        [HttpPost("webhook")]
        [AllowAnonymous] 
        public async Task<IActionResult> HandleWebhook()
        {
            try
            {
                var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                var stripeSignature = Request.Headers["Stripe-Signature"].ToString();
                
                if (string.IsNullOrEmpty(stripeSignature))
                {
                    Log.Warning("Webhook recibido sin Stripe-Signature header");
                    return BadRequest("Missing Stripe-Signature header");
                }

                await _stripeService.HandleWebhookAsync(json, stripeSignature);
                
                Log.Information("Webhook procesado correctamente");
                return Ok(); 
            }
            catch (StripeException stripeEx)
            {
                Log.Error(stripeEx, "Error de Stripe procesando webhook: {StripeError}", stripeEx.Message);
                return StatusCode(400); 
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error procesando webhook de Stripe");
                return StatusCode(500); 
            }
        }
        
        /// <summary>
        /// Confirma un pago utilizando un PaymentIntent.
        /// </summary>
        /// <param name="request">Detalles del pago a confirmar.</param>
        /// <returns>Resultado de la confirmación del pago.</returns>
        [HttpPost("create-payment")]
        [Authorize]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequestDto request)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                int.TryParse(userId, out int parsedUserId);
                var result = await _stripeService.CreatePaymentIntentAsync(parsedUserId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al crear PaymentIntent");
                return BadRequest(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Simula un pago sin interactuar con Stripe.
        /// Esta función es útil para pruebas y desarrollo.
        /// </summary>
        /// <param name="request">Detalles del pago a simular.</param>
        /// <returns>Resultado de la simulación del pago.</returns>
        [HttpPost("simulate-payment")]
        [Authorize]
        public async Task<IActionResult> SimulatePayment([FromBody] ConfirmPaymentRequestDto request)
        {
            try
            {
                var paymentIntentService = new PaymentIntentService();

                var confirmedPayment = await paymentIntentService.ConfirmAsync(request.PaymentIntentId, new PaymentIntentConfirmOptions
                {
                    PaymentMethod = "pm_card_visa", 
                });

                return Ok(new { 
                    paymentIntentId = confirmedPayment.Id,
                    status = confirmedPayment.Status,
                    amount = confirmedPayment.Amount / 100m,
                    message = $"Pago simulado - Status: {confirmedPayment.Status}"
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error simulando pago");
                return BadRequest(new { 
                    success = false, 
                    error = ex.Message 
                });
            }
        }
        
        /// <summary>
        /// Confirma si un pago fue exitoso y activa premium.
        /// </summary>
        /// <param name="request">PaymentIntent ID para verificar.</param>
        /// <returns>Resultado de la activación de premium.</returns>
        [HttpPost("confirm-payment")]
        [Authorize]
        public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentRequestDto request)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                int.TryParse(userId, out int parsedUserId);
                
                var result = await _stripeService.ConfirmPaymentAsync(parsedUserId, request);
                
                if (result)
                {
                    return Ok(new { 
                        success = true, 
                        message = "Premium activado exitosamente" 
                    });
                }
                
                return BadRequest(new { 
                    success = false, 
                    message = "El pago no se completó correctamente" 
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error confirmando pago");
                return BadRequest(new { 
                    success = false, 
                    error = ex.Message 
                });
            }
        }
    }
}