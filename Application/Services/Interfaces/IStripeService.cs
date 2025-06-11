using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proyecto_web_api.Application.DTOs.StripeDTOs;

namespace Proyecto_web_api.Application.Services.Interfaces
{
    public interface IStripeService
    {

        /// <summary>
        /// Crea un PaymentIntent para procesar un pago.
        /// </summary>
        /// <param name="userId">Id del usuario que realiza el pago.</param>
        /// <param name="request">Objeto que contiene la información del pago.</param>
        /// <returns>Un objeto PaymentResponseDto que contiene el ClientSecret y el PaymentIntentId.</returns>
        Task<PaymentResponseDto> CreatePaymentIntentAsync(int userId, CreatePaymentRequestDto request);

        /// <summary>
        /// Confirma un pago utilizando el PaymentIntent de Stripe.
        /// </summary>
        /// <param name="userId">Id del usuario que realiza el pago.</param>
        /// <param name="request">Objeto que contiene el PaymentIntentId</param>
        /// <returns>True si el pago se confirma correctamente, false si no.</returns>
        Task<bool> ConfirmPaymentAsync(int userId, ConfirmPaymentRequestDto request);

        /// <summary>
        /// Maneja el webhook de Stripe para eventos relacionados con pagos y suscripciones.
        /// </summary>
        /// <param name="json">El cuerpo del webhook en formato JSON.</param>
        /// <param name="stripeSignature">La firma del webhook de Stripe.</param>
        Task HandleWebhookAsync(string json, string stripeSignature);

        /// <summary>
        /// Chequea si un usuario es premium.
        /// </summary>
        /// <param name="userId">Id del usuario</param>
        /// <returns>True si es premium, false si no</returns> 
        Task<bool> IsUserPremiumAsync(int userId);
    }
}