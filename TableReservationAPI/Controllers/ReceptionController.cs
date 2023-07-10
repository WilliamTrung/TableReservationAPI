using ApplicationService.Models.ReservationModels;
using ApplicationService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using TableReservationAPI.CustomMiddleware;

namespace TableReservationAPI.Controllers
{
    [GoogleAuthorized(roles: "Reception,Administrator")]
    [Route("api/reception")]
    [ApiController]
    public class ReceptionController : ODataController
    {
        private readonly IReceptionService _receptionService;
        public ReceptionController(IReceptionService receptionService)
        {
            _receptionService = receptionService;
        }
        [HttpGet("pending-reservation")]
        [EnableQuery]
        public IActionResult GetPendingReservations() { 
            var result = _receptionService.GetPendingReservations().Result;
            return Ok(result);
        }
        [HttpGet("get-vacants")]
        public async Task<IActionResult> GetVacantTablesAsync(int reservationId)
        {
            try
            {
                var result = await _receptionService.GetVacantTablesInformation(reservationId);
                return Ok(result);
            } catch (KeyNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            
            
        }

        [HttpPut("assign-table")]
        public async Task<IActionResult> AssignTableAsync(int tableId, ReservationModel reservation)
        {
            try
            {
                await _receptionService.AssignTable(tableId, reservation);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return Ok(StatusCode(StatusCodes.Status404NotFound, "No table or reservation found!"));
            }
            catch (InvalidOperationException)
            {
                return Ok(StatusCode(StatusCodes.Status409Conflict, "This reservation has been modified!"));
            }
            catch (InvalidDataException ex)
            {
                return Ok(StatusCode(StatusCodes.Status406NotAcceptable, ex.Message));
            }
        }
        [HttpPost("check-in")]
        public async Task<IActionResult> CheckinReservation(int reservationId)
        {
            try
            {
                await _receptionService.CheckinCustomer(reservationId);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return Ok(StatusCode(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Ok(StatusCode(StatusCodes.Status409Conflict, ex.Message));
            }
            catch (ArgumentNullException ex)
            {
                return Ok(StatusCode(StatusCodes.Status406NotAcceptable, ex.Message));
            }
        }
        [HttpPost("check-out")]
        public async Task<IActionResult> CheckoutReservation(int reservationId)
        {
            try
            {
                await _receptionService.CheckoutCustomer(reservationId);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return Ok(StatusCode(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Ok(StatusCode(StatusCodes.Status409Conflict, ex.Message));
            }
            catch (ArgumentNullException ex)
            {
                return Ok(StatusCode(StatusCodes.Status406NotAcceptable, ex.Message));
            }
        }
    }
}
