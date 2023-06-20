using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Models.TableModels
{
    public class NewTableModel
    {
        public string TableDescription { get; set; } = null!;
        public string TableStatus { get; set; } = "Vacant";
        public int Seat { get; set; } = 0;
        public bool Private { get; set; } = false;
    }
}
