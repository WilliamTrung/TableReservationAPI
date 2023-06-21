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
        Task<IEnumerable<TableModel>> GetVacantTables(ReservationModel reservation);
    }
}
