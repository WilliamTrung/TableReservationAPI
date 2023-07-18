using ApplicationCore.Enum;
using ApplicationService.Models.ReservationModels;
using ApplicationService.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator;

namespace ApplicationService.Services.Implementation
{
    public class AnonymousBookingService : ReceptionService, IAnonymousBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountService _accountService;
        public AnonymousBookingService(IUnitOfWork unitOfWork, IAccountService accountService) : base(unitOfWork, accountService)
        {
            _unitOfWork = unitOfWork;
            _accountService = accountService;
        }
        /// <summary>
        /// Add reservation through ValidateReservation
        /// <para>Throw InvalidOperationException: No vacant at current time</para>
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task AddAnonymousReservation(NewAnonymousModel reservation)
        {
            try
            {
                if (await ValidateReservation(reservation.ToNewReservation()))
                {
                    var newReservation = reservation.ToReservation();
                    await _unitOfWork.ReservationRepository.Create(newReservation);
                    _unitOfWork.Commit();
                }
                else
                {
                    throw new InvalidOperationException("The last vacant has been occupied!");
                }
            }
            catch (InvalidDataException)
            {
                throw new InvalidOperationException("Invalid time to make a reservation!");
            }
        }

        public async Task<IEnumerable<UpdateAnonymousModel>> GetPendingAnonymousReservations()
        {
            var query = await _unitOfWork.ReservationRepository.Get(filter: r => r.UserId == null && r.Status == IEnum.ReservationStatus.Pending);
            var result = new List<UpdateAnonymousModel>();
            
            result = query.Select(r => UpdateAnonymousModel.FromReservation(r)).ToList();
            return result;
        }
        /// <summary>
        /// Cancel an anonymous reservation
        /// <para>Throw InvalidOperationException: Exceed deadline to be canceled!</para>
        /// <para>Throw KeyNotFoundException: Reservation not found!</para>
        /// </summary>
        /// <param name="reservationId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task CancelAnonymousReservation(int reservationId)
        {
            var find = await _unitOfWork.ReservationRepository.Get(filter: r => r.Id == reservationId && (r.Status == IEnum.ReservationStatus.Pending || r.Status == IEnum.ReservationStatus.Assigned));
            var found = find.FirstOrDefault();
            if (found != null)
            {
                //modifiying
                if (!(found.ReservedTime > DateTimeOffset.Now.AddHours(GlobalValidation.DEADLINE_HOURS)))
                {
                    throw new InvalidOperationException("Exceed deadline");
                }
                found.Status = IEnum.ReservationStatus.Cancel;
                await _unitOfWork.ReservationRepository.Update(found, found.Id);
                _unitOfWork.Commit();
            }
            else
            {
                throw new KeyNotFoundException("The reservation does not exist!");
            }
        }

        public async Task<IEnumerable<UpdateAnonymousModel>> GetAssignedAnonymousReservations()
        {
            var query = await _unitOfWork.ReservationRepository.Get(filter: r => r.UserId == null && r.Status == IEnum.ReservationStatus.Assigned);
            var result = new List<UpdateAnonymousModel>();

            result = query.Select(r => UpdateAnonymousModel.FromReservation(r)).ToList();
            return result;
        }

        public async Task<IEnumerable<UpdateAnonymousModel>> GetActiveAnonymousReservations()
        {
            var query = await _unitOfWork.ReservationRepository.Get(filter: r => r.UserId == null && r.Status == IEnum.ReservationStatus.Active);
            var result = new List<UpdateAnonymousModel>();

            result = query.Select(r => UpdateAnonymousModel.FromReservation(r)).ToList();
            return result;
        }
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
        public async Task ModifiedReservation(UpdateAnonymousModel reservation)
        {
            var find = await _unitOfWork.ReservationRepository.Get(filter: r => r.Id == reservation.Id);
            var found = find.FirstOrDefault();
            if (found != null)
            {
                //modifiying
                if (!(found.ReservedTime > DateTime.Now.AddHours(GlobalValidation.DEADLINE_HOURS)))
                {
                    throw new InvalidOperationException("Exceed deadline");
                }
                var validateModel = reservation.ToNewReservation();                
                if (await ValidateReservation(validateModel))
                {
                    //is valid change
                    //update
                    found.GuestAmount = validateModel.Seat;
                    found.Note = validateModel.Note;
                    found.ReservedTime = validateModel.DesiredDate.ToDateTime(validateModel.DesiredTime);
                    found.Private = validateModel.Private;
                    found.Status = IEnum.ReservationStatus.Pending;
                    found.TableId = null;
                    await _unitOfWork.ReservationRepository.Update(found, found.Id);
                    _unitOfWork.Commit();
                }
                else
                {
                    //throw
                    throw new InvalidOperationException("The last vacant has been occupied!");
                }
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }
    }
}
