using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Models.TableModels
{
    public class TableStatusModel
    {
        [Required]
        public string Description = null!;
    }
}
