namespace CivicConnect.Controllers
{
    //Login / logout / registration (NFR-002)
    [AllowAnonymous]
    public class AccountController : AppControllerBase
    {
        private readonly IAuthService _auth;

        public AccountController(IAuthService auth)
        {
            _auth = auth;
        }

        // GET /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        // POST /Account/Login
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _auth.ValidateCredentialsAsync(model.Email, model.Password);
            if (result.Failed)
            {
                // One generic message for every failure
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Invalid email or password.");
                model.Password = string.Empty;
                return View(model);
            }

            var user = result.Value!;
            await SignInUserAsync(user);

            // Open-redirect protection
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return LocalRedirect(model.ReturnUrl);

            return RedirectToRoleHome(user.Role);
        }

        // POST /Account/Logout
        [HttpPost, ValidateAntiForgeryToken, Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // GET /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // POST /Account/Register
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _auth.RegisterRequesterAsync(new RegisterCommand
            {
                Email = model.Email,
                DisplayName = model.DisplayName,
                Password = model.Password
            });

            if (result.Failed)
            {
                var field = result.ErrorCode == ErrorCodes.DuplicateEmail ? nameof(RegisterViewModel.Email) : string.Empty;
                ModelState.AddModelError(field, result.ErrorMessage ?? "Registration failed.");
                model.Password = string.Empty;
                model.ConfirmPassword = string.Empty;
                return View(model);
            }

            await SignInUserAsync(result.Value!);
            return RedirectToAction("Create", "ServiceRequests");
        }

        // GET /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            return View();
        }

        private Task SignInUserAsync(User user)
        {
            return HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimsPrincipalFactory.Create(user),
                new AuthenticationProperties { IsPersistent = false });
        }
    }
}
