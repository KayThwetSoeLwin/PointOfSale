using Microsoft.AspNetCore.Mvc.Rendering;
using PointOfSale.Shared.DTOs;

namespace PointOfSale.MVCUI.ViewModels
{
    public class StaffUpdateViewModel : StaffUpdateRequestModel
    {
       
        public List<SelectListItem>? Roles { get; set; }

    }
}
