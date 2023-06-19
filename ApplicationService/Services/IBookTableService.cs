using ApplicationCore.Entities;
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
        Task<IEnumerable<Reservation>> ViewHistoryReservations(CustomerModel requester);
        Task MakeReservation(Reservation reservation, CustomerModel requester);
        Task ModifiedReservation(Reservation reservation, CustomerModel requester);
        Task CancelReservation(int reservationId, CustomerModel reu);      
    }
}
