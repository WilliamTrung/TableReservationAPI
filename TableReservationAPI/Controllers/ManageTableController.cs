using ApplicationService.Models;
using ApplicationService.Models.TableModels;
using ApplicationService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using TableReservationAPI.CustomMiddleware;

namespace TableReservationAPI.Controllers
{
    [Route("api/manage-table")]
    [GoogleAuthorized(roles: "Reception,Administrator")]
    [ApiController]
    public class ManageTableController : ODataController
    {
        private readonly ITableManagementService _tableManagementService;
        public ManageTableController(ITableManagementService tableManagementService)
        {
            _tableManagementService = tableManagementService;
        }
        [HttpGet("status")]
        public IActionResult GetTableStatuses()
        {
            var result = _tableManagementService.GetTableStatuses().Result;
            return Ok(result);
        }
        [HttpGet("type")]
        [EnableQuery]
        public IActionResult GetTableTypes()
        {
            var result = _tableManagementService.GetTableTypes().Result;
            return Ok(result);
        }
        [HttpGet]
        [EnableQuery]
        public IActionResult GetTables()
        {
            var result = _tableManagementService.GetTables().Result;
            return Ok(result);
        }
        [HttpPost]
        public IActionResult AddTable(NewTableModel table)
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
