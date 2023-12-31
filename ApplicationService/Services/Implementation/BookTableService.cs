﻿using ApplicationCore.Entities;
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
                if (!(found.ReservedTime > DateTime.Now.AddHours(GlobalValidation.DEADLINE_HOURS)))
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
            var desired_table_inday = (await _unitOfWork.TableRepository.Get(filter: table =>
                table.IsDeleted == false &&
                table.Status == IEnum.TableStatus.Available &&
                table.Type.Private == desired.Private &&
                table.Type.Seat >= desired.Seat &&
                Math.Abs(table.Type.Seat - desired.Seat) <= GlobalValidation.BOUNDARY_SEAT
                , orderBy: null, includeProperties: "Type")).ToList();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var current_time = DateTime.Now;
            var reservations_inday = (await _unitOfWork.ReservationRepository.Get(
                filter: r =>
                r.ReservedTime.Date == desired.DesiredDate.ToDateTime(TimeOnly.MinValue) &&
                //(current_time.AddHours(2).AddMinutes(50) <= r.ReservedTime) &&
                r.GuestAmount >= desired.Seat &&
                Math.Abs(r.GuestAmount - desired.Seat) <= GlobalValidation.BOUNDARY_SEAT &&
                r.Status != IEnum.ReservationStatus.Cancel &&
                r.Private == desired.Private,
                orderBy: null, includeProperties: null
                )).ToList();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            var count_desired_table_inday = desired_table_inday.Count();

            var list = new List<VacantTables>();
            double i = GlobalValidation.START_TIME;
            var current_hour = TimeOnly.FromDateTime(current_time);        
            //check if current time is today and in working hour --> limit the returned result
            if(desired.DesiredDate == DateOnly.FromDateTime(current_time) && current_hour >= TimeOnly.FromTimeSpan(TimeSpan.FromHours(GlobalValidation.START_TIME)) && current_hour <= TimeOnly.FromTimeSpan(TimeSpan.FromHours(GlobalValidation.END_TIME)))
            {
                // Find the nearest half-hour
                int minutes = current_hour.Minute;
                int nearestHalfHour = ((minutes + 29) / 30) * 30; // Round up to nearest half-hour

                // If the minutes are 60 after rounding, increment the hour
                int nearestHour = current_hour.Hour + (nearestHalfHour / 60);
                nearestHalfHour %= 60;

                // Create the nearest round time
                TimeOnly nearestRoundTime = new TimeOnly(nearestHour, nearestHalfHour, 0);
                nearestRoundTime = nearestRoundTime.AddHours(GlobalValidation.BOUNDARY_HOURS);
                i = nearestRoundTime.Hour + (nearestRoundTime.Minute/60);
            } else if(desired.DesiredDate == DateOnly.FromDateTime(current_time) && current_hour >= TimeOnly.FromTimeSpan(TimeSpan.FromHours(GlobalValidation.END_TIME)))
            {
                i = GlobalValidation.END_TIME + 1;
            }
            for (; i <= GlobalValidation.END_TIME; i+=0.5)
            {
                int count_reservations_atTime = reservations_inday.Count(r => r.ReservedTime.TimeOfDay >= TimeSpan.FromHours(i + GlobalValidation.BOUNDARY_HOURS * -1) && r.ReservedTime.TimeOfDay <= TimeSpan.FromHours(i + GlobalValidation.BOUNDARY_HOURS+1));
                int count_vacant = count_desired_table_inday - count_reservations_atTime;
                if(count_vacant < 0)
                {
                    count_vacant = 0;
                }
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
            var find = await _unitOfWork.ReservationRepository.Get(filter: r => r.Id == reservation.Id, includeProperties: "User");
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
                if(!(found.ReservedTime > DateTime.Now.AddHours(GlobalValidation.DEADLINE_HOURS)))
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
                    found.Status = IEnum.ReservationStatus.Pending;
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
            if (reservation.DesiredDate.ToDateTime(reservation.DesiredTime) < DateTime.Now.AddHours(GlobalValidation.BOUNDARY_HOURS))
            {
                throw new InvalidDataException();
            }
            var task_getVacantTablesOnDate = GetVacantTables(reservation.ToDesiredModel());
            //var task_reservations = _unitOfWork.ReservationRepository.Get(filter: r =>
            //        (r.ReservedTime.Add(TimeSpan.FromHours(GlobalValidation.BOUNDARY_HOURS)).TimeOfDay == reservation.DesiredTime.ToTimeSpan() ||
            //        r.ReservedTime.TimeOfDay == reservation.DesiredTime.ToTimeSpan()) &&
            //        Math.Abs(reservation.Seat - reservation.Seat) <= GlobalValidation.BOUNDARY_SEAT
            //        );
            //var task_reservations = _unitOfWork.ReservationRepository.Get(filter: r =>          
            //        r.Status == IEnum.ReservationStatus.Pending &&
            //        Math.Abs(r.GuestAmount - reservation.Seat) <= GlobalValidation.BOUNDARY_SEAT
            //        );
            //Task.WaitAll(task_reservations, task_getVacantTablesOnDate);
            Task.WaitAll(task_getVacantTablesOnDate);
            //int checkVacantOnDate = getVacantTablesOnDate.Where(v => v.Time == reservation.DesiredTime).Count();
            var getVacantTableOnTime = task_getVacantTablesOnDate.Result.First(t => t.Time == reservation.DesiredTime);
            //var reservations = task_reservations.Result.ToList();
            //reservations = reservations.Where(r => r.ReservedTime.Day == reservation.DesiredDate.Day && (r.ReservedTime.Add(TimeSpan.FromHours(GlobalValidation.BOUNDARY_HOURS)).TimeOfDay == reservation.DesiredTime.ToTimeSpan() ||
            //        r.ReservedTime.TimeOfDay == reservation.DesiredTime.ToTimeSpan())).ToList();
            //var amountReservations = reservations.Count();
            var count_available = getVacantTableOnTime.Amount;//- amountReservations;
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
        public async Task<IEnumerable<ReservationModel>> ViewCurrentReservation(AuthorizedModel requester)
        {
            Console.WriteLine(DateTime.UtcNow);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var find = await _unitOfWork.ReservationRepository.Get(filter: r => r.User.Email == requester.Email && (r.Status == IEnum.ReservationStatus.Pending || r.Status == IEnum.ReservationStatus.Assigned) && r.ReservedTime > DateTime.UtcNow, orderBy: null, includeProperties:"User,Table");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                        
            if(find.Count() > 0)
            {
                var result = new List<ReservationModel>();
                foreach (var reservation in find)
                {
                    var model = ReservationModel.FromReservation(reservation);
                    model.AssignedTableId = null;
                    result.Add(model);
                }                                
                return result;
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
