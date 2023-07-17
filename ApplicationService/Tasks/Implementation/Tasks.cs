using ApplicationCore.Enum;
using ApplicationService.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator;

namespace ApplicationService.Tasks.Implementation
{
    public class Tasks : ITasks
    {
        private readonly IUnitOfWork _unitOfWork;
        private bool disposedValue;

        public Tasks(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;   
        }
        public async Task LateCheckInReservation()
        {            
            var time_compare = DateTime.Now.AddMinutes(GlobalValidation.CHECKIN_BOUNDARY * -1);
            Console.WriteLine("Start latecheckinreservation: " + DateTime.Now);
            var reservations = await _unitOfWork.ReservationRepository.Get(filter: r => r.ReservedTime <= time_compare && (r.Status == IEnum.ReservationStatus.Assigned || r.Status == IEnum.ReservationStatus.Pending));
            foreach (var reservation in reservations)
            {
                reservation.Status = IEnum.ReservationStatus.Cancel;
                //await _unitOfWork.ReservationRepository.Update(reservation, reservation.Id);
            }
            if (reservations.Any())
            {
                _unitOfWork.Commit();
            }            
        }

        public async Task LateCheckOutReservation()
        {
            Console.WriteLine("Start latecheckoutreservation: " + DateTime.Now);
            var time_compare = DateTime.Now.AddMinutes(GlobalValidation.CHECKOUT_MAX * -1);
            var reservations = await _unitOfWork.ReservationRepository.Get(filter: r => r.ReservedTime <= time_compare && r.Status == IEnum.ReservationStatus.Active);
            foreach (var reservation in reservations)
            {
                reservation.Status = IEnum.ReservationStatus.Complete;
                await _unitOfWork.ReservationRepository.Update(reservation, reservation.Id);                
            }
            if (reservations.Any())
            {
                _unitOfWork.Commit();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _unitOfWork.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
