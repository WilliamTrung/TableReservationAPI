using ApplicationService.Models.ReservationModels;
using ApplicationService.Models.TableModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Services
{
    public interface IReceptionService : IBookTableService
    {
        /// <summary>
        /// Assign table to reservation
        /// <para>Throw KeyNotFoundException: No table or reservation with such id found</para>
        /// <para>Throw InvalidOperationException: This reservation is old -- has been modified by another user</para>
        /// <para>Throw InvalidDataException: Selected table data is invalid!</para>
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="reservation"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        Task AssignTable(int tableId, ReservationModel reservation);
        Task<IEnumerable<ReservationModel>> GetPendingReservations();
        /// <summary>
        /// Get vacant table(s) based on provided reservationId
        /// <para>Throw KeyNotFoundException: No reservation found!</para>
        /// </summary>
        /// <param name="reservationId"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        Task<IEnumerable<TableModel>> GetVacantTablesInformation(int reservationId);
        /// <summary>
        /// Check in an arrived customer
        /// <para>Throw KeyNotFoundException: Reservation not found!</para>
        /// <para>Throw ArgumentNullException: No pending reservation for this customer</para>
        /// <para>Throw InvalidOperationException: Not a valid time to check in</para>
        /// </summary>
        /// <param name="customerEmail"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        Task CheckinCustomer(int reservationId);
        /// <summary>
        /// Check out an arrived customer
        /// <para>Throw KeyNotFoundException: Reservation not found</para>
        /// <para>Throw ArgumentNullException: No active reservation for this customer</para>
        /// <para>Throw InvalidOperationException: Not a valid time to check out</para>
        /// </summary>
        /// <param name="customerEmail"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        Task CheckoutCustomer(int reservationId);
        
    }
}
