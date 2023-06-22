using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Models
{
    public class OAuthConfiguration
    {
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set;} = null!;
    }
}
