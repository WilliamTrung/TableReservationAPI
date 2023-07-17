using ApplicationService.Models.ReservationModels;
using ApplicationService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using TableReservationAPI.CustomMiddleware;

namespace TableReservationAPI.Controllers
{
    [Route("api/booking-reservation")]
    [GoogleAuthorized(roles: "Customer")]    
    [ApiController]
    public class BookReservationController : ODataController
    {
        private readonly IBookTableService _bookTableService;
        private readonly IAccountService _loginService;
        public BookReservationController(IBookTableService bookTableService, IAccountService loginService)
        {
            _loginService = loginService;
            _bookTableService = bookTableService;
        }
        [HttpPost("vacant-amount")]
        public IActionResult GetVacantsAmount(DesiredReservationModel desired) 
        {
            var vacants = _bookTableService.GetVacantTables(desired).Result;
            return Ok(vacants);             
        }
        [HttpPost]
        public async Task<IActionResult> AddReservationAsync(NewReservationModel newReservation)
        {
            
            try
            {
                var customer = await _loginService.ValidateLoginAsync(Request.Headers["Authorization"]);
                await _bookTableService.AddReservation(newReservation, customer);                
                return Ok(StatusCode(StatusCodes.Status201Created));
            }
            catch (InvalidOperationException ex)
            {
                return Ok(StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message));
            }
            catch (MemberAccessException)
            {
                return Ok(StatusCode(StatusCodes.Status401Unauthorized, "Invalid executioner!"));
            }
            catch (Exception)
            {
                return Ok(StatusCode(StatusCodes.Status401Unauthorized));
            }
        }
        [HttpPut("update")]
        public async Task<IActionResult> ModifiedReservationAsync(CustomerModifiedReservationModel modifiedReservation)
        {
            try
            {
                var customer = await _loginService.ValidateLoginAsync(Request.Headers["Authorization"]);
                await _bookTableService.ModifiedReservation(modifiedReservation, customer);
                return Ok(StatusCode(StatusCodes.Status202Accepted));
            }
            catch (InvalidOperationException)
            {
                return Ok(StatusCode(StatusCodes.Status503ServiceUnavailable, "No vacant table found!"));
            }
            catch (KeyNotFoundException)
            {
                return Ok(StatusCode(StatusCodes.Status404NotFound, "No reservation found!"));
            }
            catch (MemberAccessException)
            {
                return Ok(StatusCode(StatusCodes.Status401Unauthorized, "Invalid executioner!"));
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
                var customer = await _loginService.ValidateLoginAsync(Request.Headers["Authorization"]);
                await _bookTableService.CancelReservation(reservationId, customer);
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
            catch (MemberAccessException)
            {
                return Ok(StatusCode(StatusCodes.Status401Unauthorized, "Invalid executioner!"));
            }
            catch (Exception)
            {
                return Ok(StatusCode(StatusCodes.Status401Unauthorized));
            }
        }
        [HttpGet("current-pending")]
        public async Task<IActionResult> GetCurrentPendingReservationAsync()
        {
            try
            {
                var customer = await _loginService.ValidateLoginAsync(Request.Headers["Authorization"]);
                var found = await _bookTableService.ViewCurrentReservation(customer);
                return Ok(found);
            }
            catch (KeyNotFoundException)
            {
                return Ok(StatusCode(StatusCodes.Status404NotFound, "No reservation found!"));
            }
            catch (Exception ex)
            {
                return Ok(StatusCode(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
        [HttpGet("history")]
        public async Task<IActionResult> GetHistoryReservationsAsync()
        {
            try
            {
                var customer = await _loginService.ValidateLoginAsync(Request.Headers["Authorization"]);
                var list = await _bookTableService.ViewHistoryReservations(customer);
                return Ok(list);
            }
            catch (Exception)
            {
                return Ok(StatusCode(StatusCodes.Status401Unauthorized));
            }
        }
    }
}
