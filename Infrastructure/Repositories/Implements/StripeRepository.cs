using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Proyecto_web_api.Domain.Models;
using Proyecto_web_api.Infrastructure.Data;
using Proyecto_web_api.Infrastructure.Repositories.Interfaces;

namespace Proyecto_web_api.Infrastructure.Repositories.Implements
{
    public class StripeRepository : IStripeRepository
    {
        private readonly DataContext _context;
        public StripeRepository(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Agrega una nueva suscripción a la base de datos.
        /// <summary>
        /// <param name="subscription">La suscripción a agregar.</param>
        public async Task AddSubscriptionAsync(Subscription subscription)
        {
            await _context.Subscriptions.AddAsync(subscription);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtiene una suscripción por su ID de Stripe.
        /// </summary>
        /// <param name="userId">El ID del usuario al que pertenece la suscripción.</param>
        /// <param name="stripeSubscriptionId">El ID de la suscripción en Stripe.</param>
        /// <returns>La suscripción correspondiente al ID de Stripe.</returns>
        /// 
        public async Task<Subscription?> GetSubscriptionByStripeIdAsync(string userId, string stripeSubscriptionId)
        {
            return await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.UserId.ToString() == userId && s.StripeSubscriptionId == stripeSubscriptionId);
        }

        /// <summary>
        /// Actualiza una suscripción en la base de datos.
        /// </summary>
        /// <param name="subscription">La suscripción a actualizar.</param>
        public async Task UpdateSubscriptionAsync(Subscription subscription)
        {
            _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync();
        }
    }
}