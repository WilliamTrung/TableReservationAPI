using ApplicationService.Models.ReservationModels;
using ApplicationService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using TableReservationAPI.CustomMiddleware;

namespace TableReservationAPI.Controllers
{
    [Route("api/anonymous-booking")]
    [GoogleAuthorized(roles: "Reception,Administrator")]
    [ApiController]
    public class AnonymousBookingController : ODataController
    {
        private readonly IAnonymousBookingService _anonymousBookingService;        
        public AnonymousBookingController(IAnonymousBookingService anonymousBookingService)
        {
            _anonymousBookingService = anonymousBookingService;
        }
        [HttpPost("vacant-amount")]
        public IActionResult GetVacantsAmount(DesiredReservationModel desired)
        {
            var vacants = _anonymousBookingService.GetVacantTables(desired).Result;
            return Ok(vacants);
        }
        [HttpPost("new-reservation")]
        public async Task<IActionResult> AddReservationAsync(NewAnonymousModel newReservation)
        {

            try
            {
                await _anonymousBookingService.AddAnonymousReservation(newReservation);
                return Ok(StatusCode(StatusCodes.Status201Created));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
        }
        [HttpPut("modify")]
        public async Task<IActionResult> ModifyReservationAsync(UpdateAnonymousModel reservation)
        {
            try
            {
                await _anonymousBookingService.ModifiedReservation(reservation);
                return Ok(StatusCode(StatusCodes.Status202Accepted));
            }
            catch (InvalidDataException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Invalid time for " + reservation.ReservedTime);
            }
            catch (InvalidOperationException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "No vacant table found!");
            }
            catch (KeyNotFoundException)
            {
                return StatusCode(StatusCodes.Status404NotFound, "No reservation found!");
            }
        }
        [HttpPut("cancel")]
        public async Task<IActionResult> CancelReservationAsync(int reservationId)
        {
            try
            {
                await _anonymousBookingService.CancelAnonymousReservation(reservationId);
                return Ok(StatusCode(StatusCodes.Status202Accepted));
            }
            catch (InvalidOperationException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Exceed deadline - must be 3 hours soon to cancel!");
            }
            catch (KeyNotFoundException)
            {
                return StatusCode(StatusCodes.Status404NotFound, "No reservation found!");
            }
        }
        [HttpGet("current-reservations")]
        [EnableQuery]
        public async Task<IActionResult> GetCurrentReservationsAsync()
        {
            var pending = await _anonymousBookingService.GetPendingAnonymousReservations();
            var assigned = await _anonymousBookingService.GetAssignedAnonymousReservations();
            var mix = pending.Concat(assigned);
            return Ok(mix);
        }
        [HttpGet("assigned-reservations")]
        [EnableQuery]
        public async Task<IActionResult> GetAssignedReservationsAsync()
        {
            var assigned = await _anonymousBookingService.GetAssignedAnonymousReservations();
            return Ok(assigned);
        }
        [HttpGet("active-reservations")]
        [EnableQuery]
        public async Task<IActionResult> GetActiveReservations()
        {
            var pending = await _anonymousBookingService.GetActiveAnonymousReservations();
            return Ok(pending);
        }
        [HttpPost("check-in")]
        public async Task<IActionResult> CheckInAnonymousReservation(int reservationId)
        {
            try
            {
                await _anonymousBookingService.CheckinCustomer(reservationId);
                return Ok(StatusCode(StatusCodes.Status202Accepted));
            }
            catch (KeyNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }
        [HttpPost("check-out")]
        public async Task<IActionResult> CheckOutAnonymousReservation(int reservationId)
        {
            try
            {
                await _anonymousBookingService.CheckoutCustomer(reservationId);
                return Ok(StatusCode(StatusCodes.Status202Accepted));
            }
            catch (KeyNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }
    }
}
