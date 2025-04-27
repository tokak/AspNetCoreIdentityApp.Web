using AspNetCoreIdentityApp.Core.ViewModels;
using AspNetCoreIdentityApp.Repository.Models;
using AspNetCoreIdentityApp.Web.Extansions;
using AspNetCoreIdentityApp.Service.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
                ModelState.AddModelError(string.Empty, "Email veya þifre yanlýþ");
                return View();
            }

            var signInresult = await _signInManager.PasswordSignInAsync(hasUser, model.Password, model.RemembeMe, true);
            if (signInresult.Succeeded)
            {
                if (hasUser.BirthDate.HasValue)
                {
                    await _signInManager.SignInWithClaimsAsync(hasUser, model.RemembeMe, new[] { new Claim("birthdate", hasUser.BirthDate.Value.ToString()) });
                }
                if (returnUrl == null)
                    return RedirectToAction("Index");

                return RedirectToAction(returnUrl);
            }

            if (signInresult.IsLockedOut)
            {
                ModelState.AddModelErrorList(new List<string>() { "3 Dakika boyunca giriþ yapamazsýnýz." });
                return View();
            }
            var accessFailedCount = await _userManager.GetAccessFailedCountAsync(hasUser);
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
            TempData["SuccessMessage"] = "Üyelik kayýt iþlemi baþarýyla gerçekleþmiþtir.";
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
                ModelState.AddModelError(string.Empty, "Bu email adresine sahip kullanýcý bulunamadý.");
                return View();
            }

            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser);

            var passwordResetLink = Url.Action("ResetPassword", "Home", new { userId = hasUser.Id, Token = passwordResetToken }, HttpContext.Request.Scheme);

            await _emailService.SendResetPasswordEmail(passwordResetLink, hasUser.Email);

            TempData["SuccessMessage"] = "Þifre yenileme linki e-posta adresinize gönderilmiþtir.";

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
                ModelState.AddModelError(string.Empty, "Kullanýcý bulunamamýþtýr.");
                return View();
            }

            var result = await _userManager.ResetPasswordAsync(hasUser, token.ToString(), request.Password);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Þifreniz baþarýlý bir þekilde yenilenmiþtir.";

            }
            else
            {
                ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());

            }
            return View();

        }

        // Kullanýcýyý Facebook ile kimlik doðrulama için yönlendirir.
        public IActionResult FacebookLogin(string ReturnUrl)
        {
            string RedirectUrl = Url.Action("ExternalResponse", "Home", new { ReturnUrl = ReturnUrl });
            var property = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", RedirectUrl);
            return new ChallengeResult("Facebook", property);
        }


        public IActionResult GoogleLogin(string ReturnUrl)
        {
            string RedirectUrl = Url.Action("ExternalResponse", "Home", new { ReturnUrl = ReturnUrl });
            var property = _signInManager.ConfigureExternalAuthenticationProperties("Google", RedirectUrl);
            return new ChallengeResult("Google", property);
        }



        // Facebook kimlik doðrulama iþleminden dönen kullanýcý bilgilerini iþler.
        // Kullanýcý daha önce giriþ yaptýysa giriþ yapar, ilk defa giriþ yapýyorsa kullanýcý oluþturur ve baðlar.
        public async Task<IActionResult> ExternalResponse(string ReturnUrl = "/")
        {
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("LogIn");
            }
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);

            if (result.Succeeded)
            {
                return Redirect(ReturnUrl);
            }
            else
            {
                AppUser user = new AppUser();
                user.Email = info.Principal.FindFirst(ClaimTypes.Email).Value;
                string ExternalUserId = info.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;

                if (info.Principal.HasClaim(x => x.Type == ClaimTypes.Name))
                {
                    string userName = info.Principal.FindFirst(ClaimTypes.Name).Value;
                    userName = userName.Replace(" ", "-").ToLower() + ExternalUserId.Substring(0, 5).ToString();
                    user.UserName = userName;
                }
                else
                {
                    user.UserName = info.Principal.FindFirst(ClaimTypes.Email).Value;
                }

                IdentityResult createResult = await _userManager.CreateAsync(user);
                if (createResult.Succeeded)
                {
                    IdentityResult loginResult = await _userManager.AddLoginAsync(user, info);
                    if (loginResult.Succeeded)
                    {
                        //await _signInManager.SignInAsync(user, true);
                        await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        foreach (var error in loginResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                        var errors = ModelState.Values
                                                .SelectMany(v => v.Errors)
                                                .Select(e => e.ErrorMessage)
                                                .ToList();

                        return View("ErrorPage", errors); // List<string> olarak ErrorPage'e gönderiyoruz
                    }
                }
                else
                {
                    foreach (var error in createResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    var errors = ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage)
                                            .ToList();

                    return View("ErrorPage", errors); // List<string> olarak ErrorPage'e gönderiyoruz
                }
            }
        }



        public IActionResult ErrorPage()
        {
            return View();
        }

    }
}
