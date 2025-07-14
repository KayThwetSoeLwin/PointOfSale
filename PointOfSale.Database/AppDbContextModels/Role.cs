using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PointOfSale.Database.AppDbContextModels
{
    public partial class Role
    {
        [Column("RoleId")]
        public int RoleId { get; set; }

        [Column("RoleName")]
        public string RoleName { get; set; } = null!;
    }
}
