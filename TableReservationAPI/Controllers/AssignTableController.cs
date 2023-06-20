using ApplicationService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace TableReservationAPI.Controllers
{
    [Route("api/assign-table")]
    [ApiController]
    public class AssignTableController : ODataController
    {
        private readonly IReceptionService _receptionService;
        public AssignTableController(IReceptionService receptionService)
        {
            _receptionService = receptionService;
        }
        [HttpGet]
        [EnableQuery]
        public IActionResult GetPendingReservations() { 
            var result = _receptionService.GetPendingReservations().Result;
            return Ok(result);
        }
    }
}
