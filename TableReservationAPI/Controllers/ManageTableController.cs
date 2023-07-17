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
        public async Task<IActionResult> AddTable(NewTableModel table)
        {
            try
            {
                await _tableManagementService.AddTable(table);
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (MissingMemberException ex)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable, ex.Message);
            }            
        }
        [HttpPut]
        public async Task<IActionResult> UpdateTableAsync(ModifiedTableModel table)
        {
            try
            {
                await _tableManagementService.UpdateTable(table);
                return Ok(StatusCode(StatusCodes.Status202Accepted));
            }
            catch (MissingMemberException ex)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable, ex.Message);
            }
            
        }
    }
}
