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
        /// Add reservation through ValidateReservation
        /// <para>Throw InvalidOperationException: No vacant at current time</para>
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        Task AddAnonymousReservation(NewAnonymousModel reservation);
        Task<IEnumerable<UpdateAnonymousModel>> GetPendingAnonymousReservations();
        Task<IEnumerable<UpdateAnonymousModel>> GetAssignedAnonymousReservations();
        Task<IEnumerable<UpdateAnonymousModel>> GetActiveAnonymousReservations();
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
        /// <summary>
        /// Mofidy reservation through ValidateReservation
        /// <para>Throw InvalidOperationException: No vacant at current time</para>
        /// <para>Throw KeyNotFoundException: No reservation has such Id</para>
        /// </summary>
        /// <param name="reservation"></param>
        /// <param name="requester"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        Task ModifiedReservation(UpdateAnonymousModel reservation);
    }
}
