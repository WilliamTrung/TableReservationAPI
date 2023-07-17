using ApplicationCore.Entities;
using ApplicationCore.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApplicationService.Models.TableModels
{
    public class ModifiedTableModel
    {
        public int Id { get; set; }
        public string TableDescription { get; set; } = null!;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public IEnum.TableStatus Status { get; set; } = IEnum.TableStatus.Available;
        public int Seat { get; set; }
        public bool Private { get; set; }
        public bool IsDeleted { get; set; }


        public static ModifiedTableModel Converter(Table table)
        {
            return new ModifiedTableModel
            {
                Id = table.Id,
                Private = table.Type.Private,
                Seat = table.Type.Seat,
                TableDescription = table.Description,
                Status = table.Status,
                IsDeleted = table.IsDeleted
            };
        }
    }
}
