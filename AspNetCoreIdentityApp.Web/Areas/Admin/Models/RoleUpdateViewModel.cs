using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Models
{
    public class RoleUpdateViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Rol adı boş bırakılamaz")]
        [Display(Name = "Rol Adı :")]
        public string Name { get; set; }
    }
}
