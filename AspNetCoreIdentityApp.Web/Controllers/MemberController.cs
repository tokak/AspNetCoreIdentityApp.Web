using AspNetCoreIdentityApp.Core.Enums;
using AspNetCoreIdentityApp.Core.Models;
using AspNetCoreIdentityApp.Core.ViewModels;
using AspNetCoreIdentityApp.Repository.Models;
using AspNetCoreIdentityApp.Service.Services;
using AspNetCoreIdentityApp.Service.TwoFactorService;
using AspNetCoreIdentityApp.Web.Extansions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileProvider _fileProvider;
        private readonly IMemberService _memberService;
        private readonly TwoFactorService _twoFactorService;
        private string userName => User.Identity.Name;
        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider, IMemberService memberService, TwoFactorService twoFactorService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
            _memberService = memberService;
            _twoFactorService = twoFactorService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _memberService.GetUserViewModelByUserNameAsync(userName));
        }
        public async Task LogoutAsync()
        {
            _memberService.LogoutAsync();
        }

        public IActionResult PasswordChange()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var checkOldPassword = await _memberService.CheckPasswordAsync(userName, request.PasswordNew);

            if (!checkOldPassword)
            {
                ModelState.AddModelError(string.Empty, "Eski şifreniz yanlış");
            }

            var (isSuccess, errors) = await _memberService.ChangePasswordAsync(userName, request.PasswordOld, request.PasswordNew);

            if (!isSuccess)
            {
                //ModelState.AddModelErrorList(resultChangePassword.Errors.Select(x => x.Description).ToList());
                ModelState.AddModelErrorList(errors);
                return View();
            }
            TempData["SuccessMessage"] = "Şifreniz başarılı bir şekilde değiştirilmiştir.";
            return View();
        }


        public async Task<IActionResult> UserEdit()
        {
            ViewBag.genderList = _memberService.GetGenderSelectList();

            return View(await _memberService.GetUserEditViewModelAsync(userName));
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViewModel request)
        {
            // Model geçerli değilse, mevcut sayfayı tekrar göster.
            if (!ModelState.IsValid)
            {
                return View();
            }
            var (isSuccess, errors) = await _memberService.EditUserAsync(request, userName);

            if (!isSuccess)
            {
                ModelState.AddModelErrorList(errors);
                return View();
            }
            TempData["SuccessMessage"] = "Üye bilgileri başarıyla değiştirilmiştir.";

            return View(await _memberService.GetUserEditViewModelAsync(userName));
        }


        public IActionResult AccessDenied(string ReturnUrl)
        {
            string message = string.Empty;

            ViewBag.message = "Bu sayfayı görmeye yetkiniz yoktur. Yetki almak için yöneticiniz ile görüşünüz.";
            return View();
        }

        [HttpGet]
        public IActionResult Claims()
        {
            return View(_memberService.GetClaims(User));
        }

        [Authorize(Policy = "AnkaraPolicy")]
        [HttpGet]
        public IActionResult AnkaraPage()
        {
            return View();
        }

        [Authorize(Policy = "ExchangePolicy")]
        [HttpGet]
        public IActionResult ExchangePage()
        {
            return View();
        }

        [Authorize(Policy = "ViolencePolicy")]
        [HttpGet]
        public IActionResult ViolencePage()
        {
            return View();
        }


        public async Task<IActionResult> TwoFactorAuth()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return View(new AuthenticationViewModel() { TwoFactorType = (TwoFactor)currentUser.TwoFactor });
        }
        [HttpPost]
        public async Task<IActionResult> TwoFactorAuth(AuthenticationViewModel authenticationVM)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            switch (authenticationVM.TwoFactorType)
            {
                case TwoFactor.None:
                    currentUser.TwoFactorEnabled = false;
                    currentUser.TwoFactor = (sbyte)TwoFactor.None;
                    TempData["message"] = "iki adımlı doğrulama tipiniz hiçbiri olarak belirlenmiştir.";
                    break;
                case TwoFactor.Phone:
                    currentUser.TwoFactorEnabled = false;
                    currentUser.TwoFactor = (sbyte)TwoFactor.Phone;
                    TempData["message"] = "iki adımlı doğrulama tipiniz sms olarak belirlenmiştir.";
                    break;
                case TwoFactor.Email:
                    currentUser.TwoFactorEnabled = true;
                    currentUser.TwoFactor = (sbyte)(TwoFactor.Email);
                    TempData["message"] = "iki adımlı doğrulama tipiniz email olarak belirlenmiştir.";
                    break;
                case TwoFactor.MicrosoftGoogle:
                    //currentUser.TwoFactorEnabled = false;
                    //currentUser.TwoFactor = (sbyte)TwoFactor.MicrosoftGoogle;
                    //TempData["message"] = "iki adımlı doğrulama tipiniz google olarak belirlenmiştir.";
                    return RedirectToAction("TwoFactorWithAuthentication");

                default:
                    break;
            }

            await _userManager.UpdateAsync(currentUser);
            return View(authenticationVM);
        }

        public async Task<IActionResult> TwoFactorWithAuthentication()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            string unformattedKey = await _userManager.GetAuthenticatorKeyAsync(currentUser);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(currentUser);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(currentUser);
            }
            AuthenticationViewModel authenticationView = new()
            {
                Sharedkey = unformattedKey,
                AuthenticationUri = _twoFactorService.GenerateQrCodeUri(currentUser.Email, unformattedKey),
            };
            return View(authenticationView);
        }
        [HttpPost]
        public async Task<IActionResult> TwoFactorWithAuthentication(AuthenticationViewModel authenticationViewModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var verificationCode = authenticationViewModel.VerificationCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var isTwoFactorValid = await _userManager.VerifyTwoFactorTokenAsync(currentUser, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);
            if (isTwoFactorValid)
            {
                currentUser.TwoFactorEnabled = true;
                currentUser.TwoFactor = (sbyte)TwoFactor.MicrosoftGoogle;
                var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(currentUser, 5);
                TempData["recoveryCodes"] = recoveryCodes;
                TempData["message"] = "İki adımlı kimlik doğrulama tipiniz olarak Microsoft/Google Authenticator olarak belirlenmiştir.";
                return RedirectToAction("TwoFactorAuth");
            }

            ModelState.AddModelError("", "Girdiğiniz doğrulama kodu yanlıştır");            
            return View(authenticationViewModel);
        }


      
    }
}