using ApplicationCore.Entities;
using ApplicationCore.Enum;
using ApplicationService.Models.ReservationModels;
using ApplicationService.Models.TableModels;
using ApplicationService.Models.UserModels;
using ApplicationService.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator;

namespace ApplicationService.Services.Implementation
{
    public class BookTableService : IBookTableService
    {
        private readonly IUnitOfWork _unitOfWork;
        public BookTableService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;            
        }
        /// <summary>
        /// Cancel reservation before DEADLINE_HOURS
        /// <para>Throw MemberAccessException: Unauthorized requester</para>
        /// <para>Throw InvalidOperationException: Exceed deadline to cancel</para>
        /// <para>Throw KeyNotFoundException: Reservation or Cancel status does not exist</para>
        /// </summary>
        /// <param name="reservationId"></param>
        /// <param name="requester"></param>
        /// <returns></returns>
        /// <exception cref="MemberAccessException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task CancelReservation(int reservationId, AuthorizedModel requester)
        {
            var find = await _unitOfWork.ReservationRepository.Get(filter: r => r.Id == reservationId, includeProperties: "User,Status");
            var found = find.FirstOrDefault();
            if (found != null)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                //authorizing
                if (found.User.Email != requester.Email)
                {
                    throw new MemberAccessException("Unauthorized");
                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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
        /// <summary>
        /// Get vacant tables based on
        /// <para>Number of tables in desired type and seat</para>
        /// <para>Reservations made at desired date</para>
        /// </summary>
        /// <param name="desired"></param>
        /// <returns>The number of vacants in working hours</returns>
        public async Task<IEnumerable<VacantTables>> GetVacantTables(DesiredReservationModel desired)
        {
            var task_desired_table_inday = _unitOfWork.TableRepository.Get(filter: table =>
                table.IsDeleted == false &&
                table.Status == IEnum.TableStatus.Available &&
                table.Type.Private == desired.Private &&
                table.Type.Seat >= desired.Seat &&
                Math.Abs(table.Type.Seat - desired.Seat) <= GlobalValidation.BOUNDARY_SEAT
                , orderBy: null, includeProperties: "Type");
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var current_time = DateTimeOffset.Now;
            var task_reservations_inday = _unitOfWork.ReservationRepository.Get(filter: r => 
                r.ReservedTime.Date.Equals(desired.DesiredDate) &&
                (current_time.AddHours(2).AddMinutes(50).DateTime <= r.ReservedTime) &&
                r.GuestAmount >= desired.Seat &&
                Math.Abs(r.GuestAmount - desired.Seat) <= GlobalValidation.BOUNDARY_SEAT && 
                r.Status != IEnum.ReservationStatus.Cancel,
                orderBy: null, includeProperties: null
                );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            await Task.WhenAll(task_reservations_inday,task_desired_table_inday);
            var count_desired_table_inday = task_desired_table_inday.Result.Count();
            var reservations_inday = task_reservations_inday.Result;

            var list = new List<VacantTables>();
            for (int i = GlobalValidation.START_TIME; i <= GlobalValidation.END_TIME; i++)
            {
                int count_reservations_atTime = reservations_inday.Count(r => r.ReservedTime.TimeOfDay >= TimeSpan.FromHours(i + GlobalValidation.BOUNDARY_HOURS * -1) && r.ReservedTime.TimeOfDay <= TimeSpan.FromHours(i + GlobalValidation.BOUNDARY_HOURS));
                int count_vacant = count_desired_table_inday - count_reservations_atTime;
                var item = new VacantTables(count_vacant, TimeOnly.FromTimeSpan(TimeSpan.FromHours(i)));
                list.Add(item);
            }
            var result = (IEnumerable<VacantTables>)list;
            return result;
        }
        /// <summary>
        /// Mofidy reservation through ValidateReservation
        /// <para>Throw MemberAccessException: Unauthorized requester</para>
        /// <para>Throw InvalidOperationException: No vacant at current time</para>
        /// <para>Throw KeyNotFoundException: No reservation has such Id</para>
        /// </summary>
        /// <param name="reservation"></param>
        /// <param name="requester"></param>
        /// <returns></returns>
        /// <exception cref="MemberAccessException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task ModifiedReservation(CustomerModifiedReservationModel reservation, AuthorizedModel requester)
        {
            var find = await _unitOfWork.ReservationRepository.Get(filter: r => r.Id == reservation.Id, includeProperties: "User,Status");
            var found = find.FirstOrDefault();
            if (found != null)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                //authorizing
                if (found.User.Email != requester.Email)
                {
                    throw new MemberAccessException("Unauthorized");
                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                //modifiying
                if(!(found.ReservedTime > DateTimeOffset.Now.AddHours(GlobalValidation.DEADLINE_HOURS)))
                {
                    throw new InvalidOperationException("Exceed deadline");
                }
                var validateModel = reservation.ToNewReservationModel();
                validateModel.Email = found.User.Email;
                if(await ValidateReservation(validateModel))
                {
                    //is valid change
                    //update
                    found.GuestAmount = validateModel.Seat;
                    found.Note = validateModel.Note;
                    found.ReservedTime = validateModel.DesiredDate.ToDateTime(validateModel.DesiredTime);
                    found.Private = validateModel.Private;
                    found.TableId = null;
                    await _unitOfWork.ReservationRepository.Update(found, found.Id);
                    _unitOfWork.Commit();
                } else
                {
                    //throw
                    throw new InvalidOperationException("The last vacant has been occupied!");
                }
            } else
            {
                throw new KeyNotFoundException();
            }
        }
        /// <summary>
        /// Validate new reservation data
        /// </summary>
        /// <param name="reservation"></param>
        /// <param name="requester"></param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        public Task<bool> ValidateReservation(NewReservationModel reservation)
        {
            if (reservation.DesiredDate.ToDateTime(reservation.DesiredTime) < DateTime.Now.AddHours(2).AddMinutes(30))
            {
                throw new InvalidDataException();
            }
            var task_getVacantTablesOnDate = GetVacantTables(reservation.ToDesiredModel());
            //var task_reservations = _unitOfWork.ReservationRepository.Get(filter: r =>
            //        (r.ReservedTime.Add(TimeSpan.FromHours(GlobalValidation.BOUNDARY_HOURS)).TimeOfDay == reservation.DesiredTime.ToTimeSpan() ||
            //        r.ReservedTime.TimeOfDay == reservation.DesiredTime.ToTimeSpan()) &&
            //        Math.Abs(reservation.Seat - reservation.Seat) <= GlobalValidation.BOUNDARY_SEAT
            //        );
            var task_reservations = _unitOfWork.ReservationRepository.Get(filter: r =>          
                    r.Status == IEnum.ReservationStatus.Pending &&
                    Math.Abs(r.GuestAmount - reservation.Seat) <= GlobalValidation.BOUNDARY_SEAT
                    );
            Task.WaitAll(task_reservations, task_getVacantTablesOnDate);
            //int checkVacantOnDate = getVacantTablesOnDate.Where(v => v.Time == reservation.DesiredTime).Count();
            var getVacantTableOnTime = task_getVacantTablesOnDate.Result.First(t => t.Time == reservation.DesiredTime);
            var reservations = task_reservations.Result.ToList();
            reservations = reservations.Where(r => r.ReservedTime.Day == reservation.DesiredDate.Day && (r.ReservedTime.Add(TimeSpan.FromHours(GlobalValidation.BOUNDARY_HOURS)).TimeOfDay == reservation.DesiredTime.ToTimeSpan() ||
                    r.ReservedTime.TimeOfDay == reservation.DesiredTime.ToTimeSpan())).ToList();
            var amountReservations = reservations.Count();
            var count_available = getVacantTableOnTime.Amount - amountReservations;
            if(count_available > 0)
            {
                //is valid
                return Task.FromResult(true);                
            } else
            {
                //is not valid
                return Task.FromResult(false);
                //throw new InvalidOperationException("The last vacant has been occupied!");
            }

        }
        /// <summary>
        /// Add reservation through ValidateReservation
        /// <para>Throw InvalidOperationException: No vacant at current time</para>
        /// <para>Throw MemberAccessException: Unauthorized requester</para>
        /// </summary>
        /// <param name="reservation"></param>
        /// <param name="requester"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="MemberAccessException"></exception>
        public virtual async Task AddReservation(NewReservationModel reservation, AuthorizedModel requester)
        {      
            try
            {
                if (await ValidateReservation(reservation))
                {
                    var task_user = await _unitOfWork.UserRepository.Get(filter: u => u.Email == requester.Email);
                    var user = task_user.FirstOrDefault();
                    if (user != null)
                    {
                        var newReservation = reservation.ToReservation();
                        newReservation.UserId = user.Id;
                        await _unitOfWork.ReservationRepository.Create(newReservation);
                        _unitOfWork.Commit();
                    }
                    else
                    {
                        throw new MemberAccessException("Unauthorized");
                    }
                }
                else
                {
                    throw new InvalidOperationException("The last vacant has been occupied!");
                }
            } catch (InvalidDataException)
            {
                throw new InvalidOperationException("Invalid time to make a reservation!");
            }                  
        }
        /// <summary>
        /// Get pending reservation
        /// Throw KeyNotFoundException: No pending reservation found for this customer request
        /// </summary>
        /// <param name="requester"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task<ReservationModel> ViewCurrentReservation(AuthorizedModel requester)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var find = await _unitOfWork.ReservationRepository.Get(filter: r => r.User.Email == requester.Email && r.Status == IEnum.ReservationStatus.Pending && r.ReservedTime > DateTimeOffset.Now, orderBy: null, includeProperties:"User,Table");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            
            var reservation = find.FirstOrDefault();
            if(reservation != null)
            {
                var model = ReservationModel.FromReservation(reservation);
                model.AssignedTableId = null;
                return model;
            } else
            {
                throw new KeyNotFoundException();
            }
            
        }

        public async Task<IEnumerable<ReservationModel>> ViewHistoryReservations(AuthorizedModel requester)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var reservations = await _unitOfWork.ReservationRepository.Get(filter: r => r.User.Email == requester.Email && (r.Status == IEnum.ReservationStatus.Complete || r.Status == IEnum.ReservationStatus.Cancel), orderBy: null, includeProperties: "User");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            var result = new List<ReservationModel>();
            foreach (var item in reservations)
            {
                var add = ReservationModel.FromReservation(item);
                add.AssignedTableId = null;
                result.Add(add);
            }
            return result;
        }
    }
}
