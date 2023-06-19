using ApplicationService.Models.TableModels;
using ApplicationService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace TableReservationAPI.Controllers
{
    [Route("api/manage-table")]
    [ApiController]
    public class ManageTableController : ODataController
    {
        private readonly ITableManagementService _tableManagementService;
        public ManageTableController(ITableManagementService tableManagementService)
        {
            _tableManagementService = tableManagementService;
        }
        [HttpGet("status")]
        [EnableQuery]
        public IActionResult GetTableStatuses()
        {
            return Ok(_tableManagementService.GetTableStatuses());
        }
        [HttpGet("type")]
        [EnableQuery]
        public IActionResult GetTableTypes()
        {
            return Ok(_tableManagementService.GetTableTypes());
        }
        [HttpGet]
        [EnableQuery]
        public IActionResult GetTables()
        {
            var result = _tableManagementService.GetTables();
            return Ok(result);
        }
        [HttpPost]
        public IActionResult AddTable(ModifiedTableModel table)
        {
            try
            {
                if (!_tableManagementService.AddTable(table).IsCompletedSuccessfully)
                {
                    return Ok(StatusCode(StatusCodes.Status500InternalServerError));
                }
            }
            catch (MissingMemberException ex)
            {
                return Ok(StatusCode(StatusCodes.Status406NotAcceptable, ex.Message));
            }
            return Ok(StatusCode(StatusCodes.Status201Created));
        }
        [HttpPut]
        public IActionResult UpdateTable(ModifiedTableModel table)
        {
            try
            {
                if (!_tableManagementService.UpdateTable(table).IsCompletedSuccessfully)
                {
                    return Ok(StatusCode(StatusCodes.Status500InternalServerError));
                }
            }
            catch (MissingMemberException ex)
            {
                return Ok(StatusCode(StatusCodes.Status406NotAcceptable, ex.Message));
            }
            return Ok(StatusCode(StatusCodes.Status202Accepted));
        }
    }
}
