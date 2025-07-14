using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PointOfSale.Database.AppDbContextModels
{
    public class Menu
    {
        public int MenuId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(200)]
        public string Endpoint { get; set; }

        // Navigation
        public ICollection<MenuPermission> MenuPermissions { get; set; }
    }
}
