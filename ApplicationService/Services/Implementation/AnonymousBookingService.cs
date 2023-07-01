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
        /// Check out an arrived customer
        /// <para>Throw KeyNotFoundException: Reservation not found with status Active!</para>
        /// <para>Throw InvalidOperationException: Not an anonymous reservation</para>
        /// <para>Throw InvalidOperationException: Not a valid time to check out</para>
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task CheckoutAnonymousReservation(int reservationId)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var reservation = _unitOfWork.ReservationRepository.Get(filter: r => r.Id == reservationId && r.Status == IEnum.ReservationStatus.Active).Result.FirstOrDefault();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (reservation == null)
            {
                throw new KeyNotFoundException("Reservation not found with status Active!");
            }
            if (reservation.UserId != null)
            {
                throw new InvalidOperationException("Not an anonymous reservation!");
            }
            if (!(DateTimeOffset.Now <= reservation.Modified.AddMinutes(GlobalValidation.CHECKOUT_MAX)))
            {
                throw new InvalidOperationException("Must only be checked out within " + GlobalValidation.CHECKOUT_MAX + " minutes after check-in!");
            }
            reservation.Status = IEnum.ReservationStatus.Complete;
            //await _unitOfWork.ReservationRepository.Update(reservation, reservation.Id);
            //_unitOfWork.Commit();
        }
        /// <summary>
        /// Check in an arrived customer
        /// <para>Throw KeyNotFoundException: Reservation not found with status Pending!</para>
        /// <para>Throw InvalidOperationException: Not an anonymous reservation</para>
        /// <para>Throw InvalidOperationException: Not a valid time to check in</para>
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task CheckinAnonymousReservation(int reservationId)
{
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var reservation = _unitOfWork.ReservationRepository.Get(filter: r => r.Id == reservationId && r.Status == IEnum.ReservationStatus.Pending).Result.FirstOrDefault();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (reservation == null)
            {
                throw new KeyNotFoundException("Reservation not found with status Pending!");
            }
            if(reservation.UserId != null)
            {
                throw new InvalidOperationException("Not an anonymous reservation!");
            }
            if (!(DateTimeOffset.Now >= reservation.ReservedTime && DateTimeOffset.Now <= reservation.ReservedTime.AddMinutes(GlobalValidation.CHECKIN_BOUNDARY)))
            {
                throw new InvalidOperationException("Must only be checked in within " + reservation.ReservedTime + " - " + reservation.ReservedTime.AddMinutes(GlobalValidation.CHECKIN_BOUNDARY));
            }
            reservation.Status = IEnum.ReservationStatus.Active;
            //await _unitOfWork.ReservationRepository.Update(reservation, reservation.Id);
            //_unitOfWork.Commit();
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
                    //await _unitOfWork.ReservationRepository.Create(newReservation);
                    //_unitOfWork.Commit();
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

        public async Task<IEnumerable<ReservationModel>> GetPendingAnonymousReservations()
        {
            var query = await _unitOfWork.ReservationRepository.Get(filter: r => r.UserId == null && r.Status == IEnum.ReservationStatus.Pending);
            var result = new List<ReservationModel>();
            result = query.Select(r => ReservationModel.FromReservation(r)).ToList();
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
    }
}
