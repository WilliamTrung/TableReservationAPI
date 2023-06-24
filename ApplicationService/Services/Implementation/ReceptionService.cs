using ApplicationCore.Enum;
using ApplicationService.Models.ReservationModels;
using ApplicationService.Models.TableModels;
using ApplicationService.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator;

namespace ApplicationService.Services.Implementation
{
    public class ReceptionService : BookTableService, IReceptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountService _accountService;
        private readonly IQueueService _queueService;
        public ReceptionService(IUnitOfWork unitOfWork, IAccountService accountService, IQueueService queueService) : base(unitOfWork, queueService)
        {
            _unitOfWork = unitOfWork;
            _accountService = accountService;
            _queueService = queueService;
        }
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
        public Task AssignTable(int tableId, ReservationModel reservation)
        {
            var task_table = _unitOfWork.TableRepository.Get(filter: t => t.Id == reservation.Id, includeProperties: "Type");
            var task_reservation = _unitOfWork.ReservationRepository.Get(filter: r => r.Id == reservation.Id);
            Task.WaitAll(task_table, task_reservation);
            var table = task_table.Result.FirstOrDefault();
            var _reservation = task_reservation.Result.FirstOrDefault();
            if(table == null || _reservation == null) 
            {
                throw new KeyNotFoundException();
            }
            if(_reservation.Modified != reservation.ModifiedDate)
            {
                throw new InvalidOperationException();
            }
            if(table.Type.Private != reservation.Private || table.Status != IEnum.TableStatus.Available || table.IsDeleted == true)
            {
                throw new InvalidDataException();
            }
            _reservation.TableId = tableId;
            _unitOfWork.ReservationRepository.Update(_reservation, _reservation.Id);
            _unitOfWork.Commit();
            return Task.CompletedTask;
        }
        /// <summary>
        /// Check in an arrived customer
        /// <para>Throw KeyNotFoundException: No user with such account</para>
        /// <para>Throw ArgumentNullException: No pending reservation for this customer</para>
        /// <para>Throw InvalidOperationException: Not a valid time to check in</para>
        /// </summary>
        /// <param name="customerEmail"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task CheckinCustomer(string customerEmail)
        {
            var customer = await _accountService.ValidateLoginAsync(customerEmail);
            if(customer == null)
            {
                throw new KeyNotFoundException(customerEmail);   
            }
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var reservation = _unitOfWork.ReservationRepository.Get(filter: r => r.User.Email == customer.Email && r.Status == IEnum.ReservationStatus.Pending).Result.FirstOrDefault();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if(reservation == null)
            {
                throw new ArgumentNullException("No pending reservation!");
            }
            if(!(DateTimeOffset.UtcNow >= reservation.ReservedTime && DateTimeOffset.UtcNow <= reservation.ReservedTime.AddMinutes(GlobalValidation.CHECKIN_BOUNDARY)))
            {
                throw new InvalidOperationException("Must only be checked in within " + reservation.ReservedTime + " - " + reservation.ReservedTime.AddMinutes(GlobalValidation.CHECKIN_BOUNDARY));
            }
            reservation.Status = IEnum.ReservationStatus.Active;
            await _unitOfWork.ReservationRepository.Update(reservation, reservation.Id);
            _unitOfWork.Commit();            
        }
        /// <summary>
        /// Check out an arrived customer
        /// <para>Throw KeyNotFoundException: No user with such account</para>
        /// <para>Throw ArgumentNullException: No active reservation for this customer</para>
        /// <para>Throw InvalidOperationException: Not a valid time to check out</para>
        /// </summary>
        /// <param name="customerEmail"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task CheckoutCustomer(string customerEmail)
        {
            var customer = await _accountService.ValidateLoginAsync(customerEmail);
            if (customer == null)
            {
                throw new KeyNotFoundException(customerEmail);
            }
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var reservation = _unitOfWork.ReservationRepository.Get(filter: r => r.User.Email == customer.Email && r.Status == IEnum.ReservationStatus.Active).Result.FirstOrDefault();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (reservation == null)
            {
                throw new ArgumentNullException("No active reservation!");
            }
            if (!(DateTimeOffset.UtcNow <= reservation.Modified.AddMinutes(GlobalValidation.CHECKOUT_MAX)))
            {
                throw new InvalidOperationException("Must only be checked out within " + GlobalValidation.CHECKOUT_MAX + " minutes after check-in!");
            }
            reservation.Status = IEnum.ReservationStatus.Complete;
            await _unitOfWork.ReservationRepository.Update(reservation, reservation.Id);
            _unitOfWork.Commit();

            await _queueService.ScheduleReservationCheckout(reservation.Id, reservation.Modified.DateTime, reservation.Modified.DateTime.AddMinutes(150));
        }

        public async Task<IEnumerable<ReservationModel>> GetPendingReservations()
        {
            var pending_reservations = await _unitOfWork.ReservationRepository.Get(filter: r => r.Status == IEnum.ReservationStatus.Pending, orderBy: r => r.OrderBy(c => c.ReservedTime), includeProperties: null);
            var result = new List<ReservationModel>();
            foreach (var item in pending_reservations)
            {
                result.Add(ReservationModel.FromReservation(item));
            }
            return result;

        }

        public async Task<IEnumerable<TableModel>> GetVacantTables(ReservationModel reservation)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var task_reservations_atTime = _unitOfWork.ReservationRepository.Get(filter: r =>
                r.ReservedTime.Date.Equals(reservation.Date) &&
                (DateTimeOffset.UtcNow.AddHours(GlobalValidation.DEADLINE_HOURS).CompareTo(r.ReservedTime) >= 0) &&
                r.GuestAmount >= reservation.Seat &&
                Math.Abs(r.GuestAmount - reservation.Seat) <= GlobalValidation.BOUNDARY_SEAT &&
                r.Status != IEnum.ReservationStatus.Cancel &&
                r.TableId != null,
                orderBy: null, includeProperties: "Status"
                );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            var task_desired_table_inday = _unitOfWork.TableRepository.Get(filter: table =>
                table.IsDeleted == false &&
                table.Status == IEnum.TableStatus.Available &&
                table.Type.Private == reservation.Private &&
                table.Type.Seat >= reservation.Seat &&
                Math.Abs(table.Type.Seat - reservation.Seat) <= GlobalValidation.BOUNDARY_SEAT
                , orderBy: null, includeProperties: "Status,Type");
            await Task.WhenAll(task_reservations_atTime, task_desired_table_inday);
            var reservations = task_reservations_atTime.Result;
            var vacant_tables = task_desired_table_inday.Result.Where(t => !reservations.Any(r => r.TableId == t.Id));
            var result = new List<TableModel>();
            foreach (var item in vacant_tables)
            {
                result.Add(TableModel.Converter(item));
            }
            return result;
        }
    }
}
