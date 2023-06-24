using ApplicationCore.Enum;
using ApplicationService.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Tasks.Implementation
{
    public class Tasks : ITasks
    {
        private readonly IUnitOfWork _unitOfWork;
        public Tasks(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;   
        }
        public Task LateCheckInReservation(int reservationId, DateTime current_modified)
        {
            var reservation = _unitOfWork.ReservationRepository.Get(filter: r => r.Id == reservationId).Result.FirstOrDefault();
            if(reservation == null)
            {
                return Task.FromException(new KeyNotFoundException());
            }
            if(reservation.Status == IEnum.ReservationStatus.Pending && reservation.Modified == current_modified)
            {
                reservation.Status = IEnum.ReservationStatus.Cancel;
                _unitOfWork.ReservationRepository.Update(reservation);
                _unitOfWork.Commit();
                return Task.CompletedTask;
            } else
            {
                return Task.FromException(new InvalidOperationException());
            }
        }

        public Task LateCheckOutReservation(int reservationId, DateTime current_modified)
        {
            var reservation = _unitOfWork.ReservationRepository.Get(filter: r => r.Id == reservationId).Result.FirstOrDefault();
            if (reservation == null)
            {
                return Task.FromException(new KeyNotFoundException());
            }
            if (reservation.Status == IEnum.ReservationStatus.Active && reservation.Modified == current_modified)
            {
                reservation.Status = IEnum.ReservationStatus.Complete;
                _unitOfWork.ReservationRepository.Update(reservation);
                _unitOfWork.Commit();
                return Task.CompletedTask;
            }
            else
            {
                return Task.FromException(new InvalidOperationException());
            }
        }
    }
}
