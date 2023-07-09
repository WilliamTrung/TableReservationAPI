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
        public ReceptionService(IUnitOfWork unitOfWork, IAccountService accountService) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _accountService = accountService;
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
        Task IReceptionService.AssignTable(int tableId, ReservationModel reservation)
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
        /// <para>Throw KeyNotFoundException: Reservation not found!</para>
        /// <para>Throw ArgumentNullException: No pending reservation for this customer</para>
        /// <para>Throw InvalidOperationException: Not a valid time to check in</para>
        /// </summary>
        /// <param name="customerEmail"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        async Task IReceptionService.CheckinCustomer(int reservationId)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var reservation = (await _unitOfWork.ReservationRepository.Get(filter: r => r.Id == reservationId)).FirstOrDefault();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if(reservation == null)
            {
                throw new KeyNotFoundException("Reservation not found!");                
            }
            if(reservation.Status != IEnum.ReservationStatus.Pending)
            {
                throw new ArgumentNullException("No pending reservation!");
            }
            if(!(DateTimeOffset.Now >= reservation.ReservedTime && DateTimeOffset.Now <= reservation.ReservedTime.AddMinutes(GlobalValidation.CHECKIN_BOUNDARY)))
            {
                throw new InvalidOperationException("Must only be checked in within " + reservation.ReservedTime + " - " + reservation.ReservedTime.AddMinutes(GlobalValidation.CHECKIN_BOUNDARY));
            }
            reservation.Status = IEnum.ReservationStatus.Active;
            await _unitOfWork.ReservationRepository.Update(reservation, reservation.Id);
            _unitOfWork.Commit();            
        }

        

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
        async Task IReceptionService.CheckoutCustomer(int reservationId)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var reservation = (await _unitOfWork.ReservationRepository.Get(filter: r => r.Id == reservationId)).FirstOrDefault();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (reservation == null)
            {
                throw new KeyNotFoundException("Reservation not found!");
            }
            if (reservation.Status != IEnum.ReservationStatus.Active)
            {
                throw new ArgumentNullException("No active reservation!");
            }
            if (!(DateTimeOffset.Now <= reservation.Modified.AddMinutes(GlobalValidation.CHECKOUT_MAX)))
            {
                throw new InvalidOperationException("Must only be checked out within " + GlobalValidation.CHECKOUT_MAX + " minutes after check-in!");
            }
            reservation.Status = IEnum.ReservationStatus.Complete;
            await _unitOfWork.ReservationRepository.Update(reservation, reservation.Id);
            _unitOfWork.Commit();
        }

        

        async Task<IEnumerable<ReservationModel>> IReceptionService.GetPendingReservations()
        {
            var pending_reservations = await _unitOfWork.ReservationRepository.Get(filter: r => r.Status == IEnum.ReservationStatus.Pending, orderBy: r => r.OrderBy(c => c.ReservedTime), includeProperties: null);
            var result = new List<ReservationModel>();
            foreach (var item in pending_reservations)
            {
                result.Add(ReservationModel.FromReservation(item));
            }
            return result;

        }

        public async Task<IEnumerable<TableModel>> GetVacantTablesInformation(DesiredReservationModel desired)
        {
            var task_desired_table_inday = (await _unitOfWork.TableRepository.Get(filter: table =>
                            table.IsDeleted == false &&
                            table.Status == IEnum.TableStatus.Available &&
                            table.Type.Private == desired.Private &&
                            table.Type.Seat >= desired.Seat &&
                            Math.Abs(table.Type.Seat - desired.Seat) <= GlobalValidation.BOUNDARY_SEAT
                            , orderBy: null, includeProperties: "Type")).ToList();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var current_time = DateTimeOffset.Now;
            var task_reservations_inday = await _unitOfWork.ReservationRepository.Get(filter: r =>
                r.ReservedTime.Date.Equals(desired.DesiredDate) &&
                (current_time.AddHours(2).AddMinutes(50).DateTime <= r.ReservedTime) &&
                r.GuestAmount >= desired.Seat &&
                Math.Abs(r.GuestAmount - desired.Seat) <= GlobalValidation.BOUNDARY_SEAT &&
                r.Status != IEnum.ReservationStatus.Cancel,
                orderBy: null, includeProperties: null
                );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            var reservations = task_reservations_inday;
            var vacant_tables = task_desired_table_inday.Where(t => !reservations.Any(r => r.TableId == t.Id));
            var result = new List<TableModel>();
            foreach (var item in vacant_tables)
            {
                result.Add(TableModel.Converter(item));
            }
            return result;
        }
    }
}
