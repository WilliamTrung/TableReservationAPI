using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Models.TableModels
{
    public class TableTypeModel
    {
        [Range(1,int.MaxValue)]
        public int Seat { get; set; }
        public bool Private { get; set; } = false;
    }
}
