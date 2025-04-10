using AspNetCoreIdentityApp.Web.Extansions;
using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.Services;
using AspNetCoreIdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
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

            if (!ModelState.IsValid)
            {
                return View();
            }
            var hasUser = await _userManager.FindByEmailAsync(model.Email);
            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Email veya �ifre yanl��");
                return View();
            }

            var signInresult = await _signInManager.PasswordSignInAsync(hasUser, model.Password, model.RemembeMe, true);
            if (signInresult.Succeeded)
            {
                if (hasUser.BirthDate.HasValue)
                {
                    await _signInManager.SignInWithClaimsAsync(hasUser, model.RemembeMe, new[] {new Claim("birthdate",hasUser.BirthDate.Value.ToString())});
                }
                if (returnUrl == null)
                    return RedirectToAction("Index");

                return RedirectToAction(returnUrl);
            }

            if (signInresult.IsLockedOut)
            {
                ModelState.AddModelErrorList(new List<string>() { "3 Dakika boyunca giri� yapamazs�n�z." });
                return View();
            }
            var accessFailedCount = await _userManager.GetAccessFailedCountAsync(hasUser);
            ModelState.AddModelErrorList(new List<string>() { $"Email veya �ifreniz yanl��", $"Ba�ar�s�z giri� say�s� = {accessFailedCount}" });

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
            if (!identityResult.Succeeded)
            {
                ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());
                //foreach (IdentityError item in identityResult.Errors)
                //{
                //    ModelState.AddModelError(string.Empty, item.Description);
                //}
                return View();
            }
            var user = await _userManager.FindByEmailAsync(request.Email);
            var exchangeClaim = new Claim("ExchangeExpireDate", DateTime.Now.AddDays(10).ToString());
            var claimResult = await _userManager.AddClaimAsync(user, exchangeClaim);
            if (!claimResult.Succeeded)
            {
                ModelState.AddModelErrorList(claimResult.Errors.Select(x => x.Description).ToList());
                //foreach (IdentityError item in identityResult.Errors)
                //{
                //    ModelState.AddModelError(string.Empty, item.Description);
                //}
                return View();
            }
            TempData["SuccessMessage"] = "�yelik kay�t i�lemi ba�ar�yla ger�ekle�mi�tir.";
            return RedirectToAction(nameof(HomeController.SignUp));
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
                ModelState.AddModelError(string.Empty, "Bu email adresine sahip kullan�c� bulunamad�.");
                return View();
            }

            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser);

            var passwordResetLink = Url.Action("ResetPassword", "Home", new { userId = hasUser.Id, Token = passwordResetToken }, HttpContext.Request.Scheme);

            await _emailService.SendResetPasswordEmail(passwordResetLink, hasUser.Email);

            TempData["SuccessMessage"] = "�ifre yenileme linki e-posta adresinize g�nderilmi�tir.";

            return RedirectToAction(nameof(ForgetPassword));
        }

        public async Task<IActionResult> ResetPassword(string userId, string token)
        {
            TempData["token"] = token;
            TempData["userId"] = userId;


            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel request)
        {
            var userId = TempData["userId"];
            var token = TempData["token"];

            if (userId == null && token == null)
            {
                throw new Exception("Bir hata meydana geldi");
            }

            var hasUser = await _userManager.FindByIdAsync(userId.ToString());
            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Kullan�c� bulunamam��t�r.");
                return View();
            }

            var result = await _userManager.ResetPasswordAsync(hasUser, token.ToString(), request.Password);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "�ifreniz ba�ar�l� bir �ekilde yenilenmi�tir.";

            }
            else
            {
                ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());

            }
            return View();

        }


    }
}
