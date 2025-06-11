using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proyecto_web_api.Domain.Models;

namespace Proyecto_web_api.Infrastructure.Repositories.Interfaces
{
    public interface IStripeRepository
    {
        /// <summary>
        /// Agrega una nueva suscripción a la base de datos.
        /// <summary>
        /// <param name="subscription">La suscripción a agregar.</param>
        Task AddSubscriptionAsync(Subscription subscription);

        /// <summary>
        /// Obtiene una suscripción por su ID de Stripe.
        /// </summary>
        /// <param name="userId">El ID del usuario al que pertenece la suscripción.</param>
        /// <param name="stripeSubscriptionId">El ID de la suscripción en Stripe.</param>
        /// <returns>La suscripción correspondiente al ID de Stripe.</returns>
        /// 
        Task<Subscription?> GetSubscriptionByStripeIdAsync(string userId, string stripeSubscriptionId);
        
        /// <summary>
        /// Actualiza una suscripción en la base de datos.
        /// </summary>
        /// <param name="subscription">La suscripción a actualizar.</param>
        Task UpdateSubscriptionAsync(Subscription subscription);
    }
}