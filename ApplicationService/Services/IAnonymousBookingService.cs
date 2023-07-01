using ApplicationService.Models.ReservationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Services
{
    public interface IAnonymousBookingService : IReceptionService
    {
        /// <summary>
        /// Check in an arrived customer
        /// <para>Throw KeyNotFoundException: Reservation not found with status Pending!</para>
        /// <para>Throw InvalidOperationException: Not an anonymous reservation</para>
        /// <para>Throw InvalidOperationException: Not a valid time to check in</para>
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        Task CheckinAnonymousReservation(int reservationId);
        /// <summary>
        /// Check out an arrived customer
        /// <para>Throw KeyNotFoundException: Reservation not found with status Active!</para>
        /// <para>Throw InvalidOperationException: Not an anonymous reservation</para>
        /// <para>Throw InvalidOperationException: Not a valid time to check out</para>
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        Task CheckoutAnonymousReservation(int reservationId);
        /// <summary>
        /// Add reservation through ValidateReservation
        /// <para>Throw InvalidOperationException: No vacant at current time</para>
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        Task AddAnonymousReservation(NewAnonymousModel reservation);
        Task<IEnumerable<ReservationModel>> GetPendingAnonymousReservations();
        /// <summary>
        /// Cancel an anonymous reservation
        /// <para>Throw InvalidOperationException: Exceed deadline to be canceled!</para>
        /// <para>Throw KeyNotFoundException: Reservation not found!</para>
        /// </summary>
        /// <param name="reservationId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        Task CancelAnonymousReservation(int reservationId);        
    }
}
