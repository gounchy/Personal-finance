using CNPM_Nhom12.Models;
using CNPM_Nhom12.Services;
using CNPM_Nhom12.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CNPM_Nhom12.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly EmailService _emailService;

        /// <summary>Chuyển về trang landing và mở modal tương ứng (query giữ returnUrl nếu có).</summary>
        private RedirectResult RedirectToLandingAuth(string mode, string? returnUrl = null)
        {
            var q = mode switch
            {
                "register" => "openRegister=1",
                "forgot" => "openForgot=1",
                _ => "openLogin=1"
            };
            if (!string.IsNullOrWhiteSpace(returnUrl))
                q += "&returnUrl=" + Uri.EscapeDataString(returnUrl);
            return Redirect("/?" + q);
        }

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            EmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        // ── ĐĂNG KÝ ────────────────────────────────────────────────────────

        [HttpGet]
        public IActionResult Register() => RedirectToLandingAuth("register");

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["RegisterError"] = string.Join(" ", ModelState.Values
                    .SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Redirect("/?openRegister=1");
            }

            var existing = await _userManager.FindByEmailAsync(vm.Email);
            if (existing != null)
            {
                TempData["RegisterError"] = "Email này đã được sử dụng.";
                return Redirect("/?openRegister=1");
            }

            var otp = new Random().Next(100000, 999999).ToString();

            TempData["Reg_FullName"] = vm.FullName;
            TempData["Reg_Email"] = vm.Email;
            TempData["Reg_Phone"] = vm.PhoneNumber;
            TempData["Reg_Address"] = vm.Address;
            TempData["Reg_Password"] = vm.Password;
            TempData["Reg_Otp"] = otp;
            TempData["Reg_OtpExpiry"] = DateTime.Now.AddMinutes(5).ToString("o");

            var sent = await _emailService.SendOtpAsync(vm.Email, otp);
            if (!sent)
            {
                TempData["RegisterError"] = "Không thể gửi email. Vui lòng kiểm tra địa chỉ và thử lại.";
                return RedirectToLandingAuth("register");
            }

            return RedirectToAction(nameof(VerifyOtp), new { email = vm.Email });
        }



        [HttpGet]
        public IActionResult VerifyOtp(string email)
            => View(new VerifyOtpViewModel { Email = email });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var savedOtp = TempData["Reg_Otp"]?.ToString();
            var expiryStr = TempData["Reg_OtpExpiry"]?.ToString();
            var fullName = TempData["Reg_FullName"]?.ToString() ?? "";
            var phone = TempData["Reg_Phone"]?.ToString() ?? "";
            var address = TempData["Reg_Address"]?.ToString() ?? "";
            var password = TempData["Reg_Password"]?.ToString() ?? "";

            TempData.Keep();

            if (savedOtp == null || expiryStr == null)
            {
                ModelState.AddModelError("", "Phiên xác thực đã hết hạn. Vui lòng đăng ký lại.");
                return View(vm);
            }

            if (DateTime.Parse(expiryStr) < DateTime.Now)
            {
                ModelState.AddModelError("", "Mã OTP đã hết hạn. Vui lòng đăng ký lại.");
                return View(vm);
            }

            if (vm.Otp != savedOtp)
            {
                ModelState.AddModelError("Otp", "Mã OTP không đúng.");
                return View(vm);
            }

            var user = new ApplicationUser
            {
                UserName = vm.Email,
                Email = vm.Email,
                FullName = fullName,
                PhoneNumber = phone,
                Address = address,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError("", e.Description);
                return View(vm);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            TempData["Success"] = $"Chào mừng {fullName} đã đến với SpendWise!";
            return RedirectToAction("Index", "Home");
        }



        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // Nếu đã đăng nhập rồi thì về trang chủ
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return RedirectToLandingAuth("login", returnUrl);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                TempData["LoginError"] = "Vui lòng nhập đầy đủ email và mật khẩu.";
                return RedirectToLandingAuth("login", returnUrl);
            }

            var result = await _signInManager.PasswordSignInAsync(
                vm.Email, vm.Password, vm.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                TempData["LoginError"] = "Tài khoản bị khóa do đăng nhập sai quá nhiều lần. Thử lại sau 5 phút.";
                return RedirectToLandingAuth("login", returnUrl);
            }

            TempData["LoginError"] = "Email hoặc mật khẩu không đúng.";
            return RedirectToLandingAuth("login", returnUrl);
        }



        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }



        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null)
            {
                TempData["ForgotPasswordNotice"] =
                    "Nếu email đã đăng ký trong hệ thống, bạn sẽ nhận mã OTP. Vui lòng kiểm tra hộp thư (kể cả mục spam).";
                return RedirectToLandingAuth("forgot");
            }

            var otp = new Random().Next(100000, 999999).ToString();
            user.OtpCode = otp;
            user.OtpExpiry = DateTime.Now.AddMinutes(5);
            await _userManager.UpdateAsync(user);

            await _emailService.SendForgotPasswordOtpAsync(vm.Email, otp);

            TempData["Info"] = "Mã OTP đã được gửi về email của bạn.";
            return RedirectToAction(nameof(ResetPassword), new { email = vm.Email });
        }



        [HttpGet]
        public IActionResult ResetPassword(string email)
            => View(new ResetPasswordViewModel { Email = email });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null || user.OtpCode == null || user.OtpExpiry == null)
            {
                ModelState.AddModelError("", "Yêu cầu không hợp lệ.");
                return View(vm);
            }

            if (user.OtpExpiry < DateTime.Now)
            {
                ModelState.AddModelError("Otp", "Mã OTP đã hết hạn. Vui lòng yêu cầu lại.");
                return View(vm);
            }

            if (user.OtpCode != vm.Otp)
            {
                ModelState.AddModelError("Otp", "Mã OTP không đúng.");
                return View(vm);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, vm.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError("", e.Description);
                return View(vm);
            }

            user.OtpCode = null;
            user.OtpExpiry = null;
            await _userManager.UpdateAsync(user);

            TempData["Success"] = "Đặt lại mật khẩu thành công! Vui lòng đăng nhập lại.";
            return RedirectToAction(nameof(Login));
        }
    }
}