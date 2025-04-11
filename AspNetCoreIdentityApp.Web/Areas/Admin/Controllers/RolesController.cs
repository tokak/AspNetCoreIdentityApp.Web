using AspNetCoreIdentityApp.Web.Areas.Admin.Models;
using AspNetCoreIdentityApp.Web.Extansions;
using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,role-action")]
    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        public RolesController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
     
        public async Task<IActionResult> Index()
        {
            var result = await _roleManager.Roles.Select(x => new RoleViewModel()
            {
                Id = x.Id,
                Name = x.Name!
            }).ToListAsync();
            return View(result);
        }

        [HttpGet]
        public IActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleCreateViewModel request)
        {
            AppRole role = new AppRole() { Name = request.Name };
            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors);
                return View();
            }
            TempData["SuccessMessage"] = "Rol oluşturuldu.";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> RoleUpdate(string id)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(id);
            if (roleToUpdate == null)
            {
                throw new Exception("Güncellenecek rol adı bulunamadı");
            }
            RoleUpdateViewModel model = new RoleUpdateViewModel()
            {
                Id = roleToUpdate.Id,
                Name = roleToUpdate.Name!
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> RoleUpdate(RoleUpdateViewModel request)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(request.Id);

            if (roleToUpdate == null)
            {
                throw new Exception("Güncellenecek rol adı bulunamadı");
            }

            roleToUpdate.Name = request.Name;
            await _roleManager.UpdateAsync(roleToUpdate);
            TempData["SuccessMessage"] = "Güncelleme işlemi gerçekleştirildi.";
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> RoleDelete(string id)
        {
            var roleToDelete = await _roleManager.FindByIdAsync(id);
            if (roleToDelete == null)
            {
                throw new Exception("Silinecek rol adı bulunamadı");
            }
            var result = await _roleManager.DeleteAsync(roleToDelete);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Silme işlemi gerçekleştirildi.";
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }
                return View();
            }
        }

        public async Task<IActionResult> AssignRoleToUser(string id)
        {
            var currentUser = await _userManager.FindByIdAsync(id);
            ViewBag.userId = id;
            var roles = await _roleManager.Roles.ToListAsync();

            var userRoles = await _userManager.GetRolesAsync(currentUser);

            var roleViewModelList = new List<AssignRoleToUserViewModel>();

            foreach (var role in roles)
            {
                var assignRoleToUserViewModel = new AssignRoleToUserViewModel()
                {
                    Id = role.Id,
                    Name = role.Name,
                };
                if (userRoles.Contains(role.Name))
                {
                    assignRoleToUserViewModel.Exist = true;
                }
                roleViewModelList.Add(assignRoleToUserViewModel);
            }
            return View(roleViewModelList);
        }
        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser(string userId, List<AssignRoleToUserViewModel> requestList)
        {
            var userToAssignRoles = await _userManager.FindByIdAsync(userId);

            foreach (var role in requestList)
            {
                if (role.Exist)
                {
                    await _userManager.AddToRoleAsync(userToAssignRoles, role.Name);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(userToAssignRoles, role.Name);
                }
            }

            TempData["SuccessMessage"] = "Rol atama işlemi gerçekleştirildi.";
            return RedirectToAction("UserList", "Home");
        }
    }
}
