using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_web_api.Application.DTOs.StripeDTOs
{
    public class CreatePaymentRequestDto
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "clp";
        public string PaymentMethodId { get; set; } = string.Empty;
    }
}