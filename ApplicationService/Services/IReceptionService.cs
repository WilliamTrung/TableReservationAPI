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
        Task AssignTable(int tableId, ReservationModel reservation);
        Task<IEnumerable<ReservationModel>> GetPendingReservations();
        Task<IEnumerable<TableModel>> GetVacantTables(ReservationModel reservation);
    }
}
