using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_web_api.Application.DTOs.StripeDTOs
{
    public class ConfirmPaymentRequestDto
    {
        public string PaymentIntentId { get; set; } = string.Empty;
    }
}