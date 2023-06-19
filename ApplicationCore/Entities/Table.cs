using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.IType;

namespace ApplicationCore.Entities
{
    public class Table : ISoftDeleteEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TypeId { get; set; }
        public int StatusId { get; set; }
        public string Description { get; set; } = null!;
        public bool IsDeleted { get; set; } = false;

        public virtual TableStatus Status { get; set; } = null!;
        public virtual TableType Type { get; set; } = null!;
    }
}
