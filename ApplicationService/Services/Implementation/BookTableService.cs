using ApplicationCore.Entities;
using ApplicationService.Models.UserModels;
using ApplicationService.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Services.Implementation
{
    public class BookTableService : IBookTableService
    {
        private readonly IUnitOfWork _unitOfWork;
        public BookTableService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Task CancelReservation(int reservationId, CustomerModel reu)
        {
            throw new NotImplementedException();
        }

        public Task MakeReservation(Reservation reservation, CustomerModel requester)
        {
            throw new NotImplementedException();
        }

        public Task ModifiedReservation(Reservation reservation, CustomerModel requester)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Reservation>> ViewHistoryReservations(CustomerModel requester)
        {
            throw new NotImplementedException();
        }
    }
}
