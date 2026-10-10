namespace CivicConnect.Controllers
{
    /// <summary>OWNER: Aidan Smith. Login / logout / registration (NFR-002).</summary>
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
            // TODO: return View(new LoginViewModel { ReturnUrl = returnUrl });
            throw new NotImplementedException();
        }

        // POST /Account/Login
        [HttpPost, ValidateAntiForgeryToken]
        public Task<IActionResult> Login(LoginViewModel model)
        {
            // TODO: ModelState invalid -> View(model). _auth.ValidateCredentialsAsync; failure -> add generic ModelState error and View(model).
            // Success -> HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, ClaimsPrincipalFactory.Create(user));
            // redirect to model.ReturnUrl ONLY if Url.IsLocalUrl(returnUrl) (open-redirect protection), else to the role's home
            // (Requester -> ServiceRequests/MyRequests, Staff -> StaffRequests/Index, Management -> Management/Dashboard).
            throw new NotImplementedException();
        }

        // POST /Account/Logout
        [HttpPost, ValidateAntiForgeryToken, Authorize]
        public Task<IActionResult> Logout()
        {
            // TODO: HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); RedirectToAction("Index", "Home").
            throw new NotImplementedException();
        }

        // GET /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            // TODO: return View(new RegisterViewModel());
            throw new NotImplementedException();
        }

        // POST /Account/Register
        [HttpPost, ValidateAntiForgeryToken]
        public Task<IActionResult> Register(RegisterViewModel model)
        {
            // TODO: _auth.RegisterRequesterAsync(new RegisterCommand{...}); DuplicateEmail/ValidationFailed -> ModelState error + View(model);
            // success -> sign in (same as Login) and redirect to ServiceRequests/Create.
            throw new NotImplementedException();
        }

        // GET /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            // TODO: return View();
            throw new NotImplementedException();
        }
    }
}
