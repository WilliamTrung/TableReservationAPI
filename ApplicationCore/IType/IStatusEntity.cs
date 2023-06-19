using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.IType
{
    public interface IStatusEntity
    {
        public int Id { get; set;  }
        public string Description { get; set; }
    }
}
