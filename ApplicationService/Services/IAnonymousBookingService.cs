﻿using ApplicationService.Models.ReservationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Services
{
    public interface IAnonymousBookingService : IReceptionService
    {
        /// <summary>
        /// Add reservation through ValidateReservation
        /// <para>Throw InvalidOperationException: No vacant at current time</para>
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        Task AddAnonymousReservation(NewAnonymousModel reservation);
        Task<IEnumerable<ReservationModel>> GetPendingAnonymousReservations();
        /// <summary>
        /// Cancel an anonymous reservation
        /// <para>Throw InvalidOperationException: Exceed deadline to be canceled!</para>
        /// <para>Throw KeyNotFoundException: Reservation not found!</para>
        /// </summary>
        /// <param name="reservationId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        Task CancelAnonymousReservation(int reservationId);        
    }
}
