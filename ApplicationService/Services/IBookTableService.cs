using ApplicationCore.Entities;
using ApplicationService.Models.ReservationModels;
using ApplicationService.Models.TableModels;
using ApplicationService.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Services
{
    public interface IBookTableService
    {
        Task<IEnumerable<ReservationModel>> ViewHistoryReservations(AuthorizedModel requester);
        /// <summary>
        /// Get pending reservation
        /// Throw KeyNotFoundException: No pending reservation found for this customer request
        /// </summary>
        /// <param name="requester"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        Task<ReservationModel> ViewCurrentReservation(AuthorizedModel requester);
        /// <summary>
        /// Get vacant tables based on
        /// <para>Number of tables in desired type and seat</para>
        /// <para>Reservations made at desired date</para>
        /// </summary>
        /// <param name="desired"></param>
        /// <returns>The number of vacants in working hours</returns>
        Task<IEnumerable<VacantTables>> GetVacantTables(DesiredReservationModel desired);
        /// <summary>
        /// Validate new reservation data
        /// </summary>
        /// <param name="reservation"></param>
        /// <param name="requester"></param>
        /// <returns></returns>
        Task<bool> ValidateReservation(NewReservationModel reservation);
        /// <summary>
        /// Add reservation through ValidateReservation
        /// <para>Throw InvalidOperationException: No vacant at current time</para>
        /// <para>Throw MemberAccessException: Unauthorized requester</para>
        /// </summary>
        /// <param name="reservation"></param>
        /// <param name="requester"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="MemberAccessException"></exception>
        Task AddReservation(NewReservationModel reservation, AuthorizedModel requester);
        /// <summary>
        /// Mofidy reservation through ValidateReservation
        /// <para>Throw MemberAccessException: Unauthorized requester</para>
        /// <para>Throw InvalidOperationException: No vacant at current time</para>
        /// <para>Throw KeyNotFoundException: No reservation has such Id</para>
        /// </summary>
        /// <param name="reservation"></param>
        /// <param name="requester"></param>
        /// <returns></returns>
        /// <exception cref="MemberAccessException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        Task ModifiedReservation(CustomerModifiedReservationModel reservation, AuthorizedModel requester);
        /// <summary>
        /// Cancel reservation before DEADLINE_HOURS
        /// <para>Throw MemberAccessException: Unauthorized requester</para>
        /// <para>Throw InvalidOperationException: Exceed deadline to cancel</para>
        /// <para>Throw KeyNotFoundException: Reservation or Cancel status does not exist</para>
        /// </summary>
        /// <param name="reservationId"></param>
        /// <param name="requester"></param>
        /// <returns></returns>
        /// <exception cref="MemberAccessException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        Task CancelReservation(int reservationId, AuthorizedModel requester);      
    }
}
