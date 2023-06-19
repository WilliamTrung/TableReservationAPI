using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.IType
{
    public interface IAuditEntity
    {
        DateTimeOffset Created { get; set; }
        DateTimeOffset Modified { get; set; }

    }
}
