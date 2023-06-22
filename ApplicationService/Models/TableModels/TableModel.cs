using ApplicationCore.Entities;
using ApplicationCore.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Models.TableModels
{
    public class TableModel
    {
        public int Id { get; set; }
        public string TableDescription { get; set; } = null!;
        public IEnum.TableStatus Status { get; set; } 
        public int Seat { get; set; }
        public bool Private { get; set; }

        public static TableModel Converter(Table table)
        {
            return new TableModel
            {
                Id = table.Id,
                Private = table.Type.Private,
                Seat = table.Type.Seat,
                TableDescription = table.Description,
                Status = table.Status
            };
        }
    }
}
