using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_web_api.Application.DTOs.StripeDTOs
{
    public class PaymentResponseDto
    {
        public string ClientSecret { get; set; } = string.Empty;
        public string PaymentIntentId { get; set; } = string.Empty;
        public int SubscriptionId { get; set; }
        public string  PaymentStatus { get; set; } = string.Empty;
    }
}