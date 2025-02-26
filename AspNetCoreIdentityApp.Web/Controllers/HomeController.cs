using AspNetCoreIdentityApp.Web.Extansions;
using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Action("Index", "Home");
            var hasUser = await _userManager.FindByEmailAsync(model.Email);
            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Email veya þifre yanlýþ");
                return View();
            }
            var signInresult = await _signInManager.PasswordSignInAsync(hasUser, model.Password, model.RemembeMe, true);

            if (signInresult.Succeeded)
            {
                return RedirectToAction(returnUrl);
            }

            if (signInresult.IsLockedOut)
            {
                ModelState.AddModelErrorList(new List<string>() { "3 Dakika boyunca giriþ yapamazsýnýz." });
                return View();
            }
            var accessFailedCount = _userManager.GetAccessFailedCountAsync(hasUser);
            ModelState.AddModelErrorList(new List<string>() { $"Email veya þifreniz yanlýþ", $"Baþarýsýz giriþ sayýsý = {accessFailedCount}" });

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            AppUser appUser = new AppUser()
            {
                Email = request.Email,
                UserName = request.UserName,
                PhoneNumber = request.Phone,
            };
            var identityResult = await _userManager.CreateAsync(appUser, request.PasswordConfirm);

            if (identityResult.Succeeded)
            {
                ViewBag.Message = "Üyelik kayýt iþlemi baþarýyla gerçekleþmiþtir.";
                return View();
            }

            ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());
            //foreach (IdentityError item in identityResult.Errors)
            //{
            //    ModelState.AddModelError(string.Empty, item.Description);
            //}
            return View();
        }


        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPassordViewModel request)
        {
            var hasUser = await _userManager.FindByEmailAsync(request.Email);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Bu email adresine sahip kullanýcý bulunamadý.");
                return View();
            }

            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser);

            var passwordResetLink = Url.Action("ResetPassword", "Home", new { userId = hasUser.Id, Token = passwordResetToken });

            TempData["SuccessMessage"] = "Þifre yenileme linki e-posta adresinize gönderilmiþtir.";

            return RedirectToAction(nameof(ForgetPassword));
        }


    }
}
