using ApplicationCore.Entities;
using ApplicationService.Models.TableModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Services
{
    public interface ITableManagementService
    {
        /// <summary>
        /// Add table
        /// <para>Throw MissingMemberException: Status does not exist</para>
        /// <para>Throw MissingMemberException: Private and Seat set does not exist</para>        
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        /// <exception cref="MissingMemberException"></exception>
        Task AddTable(ModifiedTableModel table);
        /// <summary>
        /// Update table info
        /// <para>Throw MissingMemberException: Status does not exist</para>
        /// <para>Throw MissingMemberException: Private and Seat set does not exist</para>        
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        /// <exception cref="MissingMemberException"></exception>
        Task UpdateTable(ModifiedTableModel table);
        Task DisableTable(int id);
        Task<IEnumerable<ModifiedTableModel>> GetTables();
        Task<IEnumerable<TableStatusModel>> GetTableStatuses();
        Task<IEnumerable<TableTypeModel>> GetTableTypes();
    }
}
