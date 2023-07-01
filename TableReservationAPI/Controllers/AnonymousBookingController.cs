using ApplicationService.Models.ReservationModels;
using ApplicationService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TableReservationAPI.CustomMiddleware;

namespace TableReservationAPI.Controllers
{
    [Route("api/anonymouse-booking")]
    [GoogleAuthorized("Reception")]
    [ApiController]
    public class AnonymousBookingController : ControllerBase
    {
        private readonly IAnonymousBookingService _anonymousBookingService;        
        public AnonymousBookingController(IAnonymousBookingService anonymousBookingService)
        {
            _anonymousBookingService = anonymousBookingService;
        }
        [HttpGet("vacant-amount")]
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
                return Ok(StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message));
            }
            catch (Exception)
            {
                return Ok(StatusCode(StatusCodes.Status401Unauthorized));
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
                return Ok(StatusCode(StatusCodes.Status503ServiceUnavailable, "Exceed deadline - must be 3 hours soon to cancel!"));
            }
            catch (KeyNotFoundException)
            {
                return Ok(StatusCode(StatusCodes.Status404NotFound, "No reservation found!"));
            }
        }
        [HttpGet("pending-reservations")]
        public async Task<IActionResult> GetCurrentPendingReservationsAsync()
        {
            var pending = await _anonymousBookingService.GetPendingAnonymousReservations();
            return Ok(pending);
        }
        [HttpPost("check-in")]
        public async Task<IActionResult> CheckInAnonymousReservation(int reservationId)
        {
            try
            {
                await _anonymousBookingService.CheckinAnonymousReservation(reservationId);
                return Ok(StatusCode(StatusCodes.Status202Accepted));
            }
            catch (KeyNotFoundException ex)
            {
                return Ok(StatusCode(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Ok(StatusCode(StatusCodes.Status400BadRequest, ex.Message));
            }
        }
        [HttpPost("check-out")]
        public async Task<IActionResult> CheckOutAnonymousReservation(int reservationId)
        {
            try
            {
                await _anonymousBookingService.CheckoutAnonymousReservation(reservationId);
                return Ok(StatusCode(StatusCodes.Status202Accepted));
            }
            catch (KeyNotFoundException ex)
            {
                return Ok(StatusCode(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Ok(StatusCode(StatusCodes.Status400BadRequest, ex.Message));
            }
        }
    }
}
