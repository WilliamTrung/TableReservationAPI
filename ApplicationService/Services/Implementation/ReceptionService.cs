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
        public async Task AssignTable(int tableId, AssignTableReservationModel reservation)
        {
            var _table = (await _unitOfWork.TableRepository.Get(filter: t => t.Id == tableId, includeProperties: "Type")).FirstOrDefault();            
            //find reservation
            var _reservation = (await _unitOfWork.ReservationRepository.Get(filter: r => r.Id == reservation.Id, includeProperties: "User")).FirstOrDefault();
            
            if (_table == null || _reservation == null) 
            {
                throw new KeyNotFoundException();
            }
            //check reservation with email or phone
            if(reservation.Email != null || reservation.Phone != null)
            {
                if(reservation.Email != null)
                {
                    if (_reservation.User != null)
                    {
                        //check reservation email
                        if (!(_reservation.User.Email == reservation.Email))
                        {
                            throw new InvalidDataException("Reservation's email mismatch!");
                        }
                    }
                    else
                    {
                        throw new InvalidDataException("This reservation does not have a specified user!");
                    }
                } else if (reservation.Phone != null)
                {
                    if (_reservation.Note != null)
                    {
                        //check reservation phone
                        if (!_reservation.Note.Contains(reservation.Phone))
                        {
                            throw new InvalidDataException("Reservation's phone mismatch!");
                        }
                    }
                    else
                    {
                        throw new InvalidDataException("This reservation does not have a related phone number!");
                    }
                }
            } else
            {
                throw new InvalidDataException("No email or phone provided!");
            }
            
            if (_reservation.Modified != reservation.ModifiedDate)
            {
                throw new InvalidOperationException();
            }
            var vacants = await GetVacantTablesInformation(reservation.Id);
            if (!(vacants.Any(table => table.Id == tableId)))
            {
                throw new InvalidDataException("Not a valid table for this reservation!");
            }
            _reservation.TableId = tableId;
            _reservation.Status = IEnum.ReservationStatus.Assigned;
            await _unitOfWork.ReservationRepository.Update(_reservation, _reservation.Id);
            _unitOfWork.Commit();
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
        public async Task CheckinCustomer(int reservationId)
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
        public async Task CheckoutCustomer(int reservationId)
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
            var pending_reservations = await _unitOfWork.ReservationRepository.Get(filter: r => r.Status == IEnum.ReservationStatus.Pending, orderBy: r => r.OrderBy(c => c.ReservedTime), includeProperties: "User");
            var result = new List<ReservationModel>();
            foreach (var item in pending_reservations)
            {
                result.Add(ReservationModel.FromReservation(item));
            }
            return result;

        }
        /// <summary>
        /// Get vacant table(s) based on provided reservationId
        /// <para>Throw KeyNotFoundException: No reservation found!</para>
        /// </summary>
        /// <param name="reservationId"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task<IEnumerable<TableModel>> GetVacantTablesInformation(int reservationId)
        {
            var reservation = (await _unitOfWork.ReservationRepository.Get(filter: r => r.Id == reservationId)).FirstOrDefault();
            if (reservation == null)
            {
                throw new KeyNotFoundException("Reservation not found!");
            }
            var task_desired_table_inday = (await _unitOfWork.TableRepository.Get(filter: table =>
                            table.IsDeleted == false &&
                            table.Status == IEnum.TableStatus.Available &&
                            table.Type.Private == reservation.Private &&
                            table.Type.Seat >= reservation.GuestAmount &&
                            Math.Abs(table.Type.Seat - reservation.GuestAmount) <= GlobalValidation.BOUNDARY_SEAT
                            , orderBy: null, includeProperties: "Type")).ToList();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var current_time = DateTime.Now;
            var task_reservations_inday = (await _unitOfWork.ReservationRepository.Get(
                filter: r =>
                r.ReservedTime.Date == reservation.ReservedTime.Date &&
                //(current_time.AddHours(2).AddMinutes(50) <= r.ReservedTime) &&
                r.GuestAmount >= reservation.GuestAmount &&
                Math.Abs(r.GuestAmount - reservation.GuestAmount) <= GlobalValidation.BOUNDARY_SEAT &&
                r.Status != IEnum.ReservationStatus.Cancel &&
                r.Private == reservation.Private,
                orderBy: null, includeProperties: null
                )).ToList();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            var reservations = task_reservations_inday;
            var vacant_tables = task_desired_table_inday.Where(t => !reservations.Any(r => (r.ReservedTime.TimeOfDay >= TimeSpan.FromHours(reservation.ReservedTime.Hour + GlobalValidation.BOUNDARY_HOURS * -1) && r.ReservedTime.TimeOfDay <= TimeSpan.FromHours(reservation.ReservedTime.Hour + GlobalValidation.BOUNDARY_HOURS)) && r.TableId == t.Id));
            var result = new List<TableModel>();
            foreach (var item in vacant_tables)
            {
                result.Add(TableModel.Converter(item));
            }
            return result;
        }

        public async Task<IEnumerable<ReservationModel>> GetAssignedReservation()
        {
            var assigned_reservations = await _unitOfWork.ReservationRepository.Get(filter: r => r.Status == IEnum.ReservationStatus.Assigned, orderBy: r => r.OrderBy(c => c.ReservedTime), includeProperties: "User");
            var result = new List<ReservationModel>();
            foreach (var item in assigned_reservations)
            {
                result.Add(ReservationModel.FromReservation(item));
            }
            return result;
        }
    }
}
