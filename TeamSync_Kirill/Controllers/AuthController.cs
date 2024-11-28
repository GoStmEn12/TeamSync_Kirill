using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TeamSync_Kirill.Models;

namespace TeamSync_Kirill.Controllers
{
	public class AuthController : Controller
	{
		/*private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly RoleManager<UserRole> _roleManager;

		public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<UserRole> roleManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
		}

		[HttpGet]
		public ViewResult Register() => View();
		[HttpPost]
		public async Task<IActionResult> RegisterAsync(string email, string password)
		{
			if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
			{
				ModelState.AddModelError(string.Empty, "Email and password are required.");
				return View();
			}

			var user = new User { UserName = email, Email = email, EmailConfirmed = true };
			var result = await _userManager.CreateAsync(user, password);

			if (result.Succeeded)
				return RedirectToAction("Index", "Home");

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return View();
		}

		[HttpGet]
		public ViewResult Login() => View();
		[HttpPost]
		public async Task<IActionResult> LoginAsync(string email, string password)
		{
			if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
			{
				ModelState.AddModelError(string.Empty, "Email and password are required.");
				return View();
			}

			var result = await _signInManager.PasswordSignInAsync(email, password, false, false);

			if (result.Succeeded) return RedirectToAction("Index", "Home");
			else
			{
				if (result.IsLockedOut)
					ModelState.AddModelError(string.Empty, "This account has been locked out, please try again later.");
				else if (result.IsNotAllowed)
					ModelState.AddModelError(string.Empty, "You are not allowed to log in.");
				else if (result.RequiresTwoFactor)
					ModelState.AddModelError(string.Empty, "Two-factor authentication required");
				else
					ModelState.AddModelError(string.Empty, "Invalid login attempt.");

				return View();
			}
		}
		[HttpGet]
		public ViewResult Logout() => View();
		[HttpPost]
		public async Task<IActionResult> LogoutConfirmedAsync()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}*/

		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		public AuthController(
			UserManager<IdentityUser> userManager,
			SignInManager<IdentityUser> signInManager,
			RoleManager<IdentityRole> roleManager
			)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
		}

		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Register(string email, string password)
		{
			if ( string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
			{
				return BadRequest("Login or Email or password are important ...");
			}
			var user = new IdentityUser
			{
				UserName = email,
				Email = email,
				EmailConfirmed = true
			};
			var result = await _userManager.CreateAsync(user, password);
			if (result.Succeeded)
			{
				return RedirectToAction("Index", "Home");
				//return Ok("User is registered ...");
			}
			foreach (var item in result.Errors)
			{
				Console.WriteLine(item);
			}
			return BadRequest(Json(result.Errors));
		}
		[HttpGet]
		public IActionResult Auth()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Auth(string email, string password)
		{

			if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
			{
				return BadRequest("Email or password are important ...");
			}
			var result = await _signInManager.PasswordSignInAsync(
					email,
					password,
					isPersistent: false,
					lockoutOnFailure: false
				);
			if (result.Succeeded)
			{
				return RedirectToAction("Index", "Home");
				//return Ok("Auth OK");
			}
			return BadRequest("Email or password are error ...");
		}
		[HttpPost]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}
		[HttpGet]
		public ViewResult CreateRole()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> CreateRole(string roleName)
		{
			if (string.IsNullOrEmpty(roleName))
			{
				return BadRequest("The role name is important ...");
			}
			var roleExists = await _roleManager.RoleExistsAsync(roleName);
			if (roleExists)
			{
				return BadRequest($"The role {roleName} is already exists ...");
			}
			var role = new IdentityRole { Name = roleName };
			var result = await _roleManager.CreateAsync(role);
			if (result.Succeeded)
			{
				return RedirectToAction("Index", "Home");
				//return Ok("Auth OK");
			}
			return BadRequest(Json(result.Errors));
		}
		[HttpGet]
		public ViewResult AssignRole()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> AssignRole(string userId, string roleName)
		{
			if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName))
			{
				return BadRequest("userId or roleName are important ...");
			}
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return NotFound("The User not found ...");
			}
			var roleExists = await _roleManager.RoleExistsAsync(roleName);
			if (!roleExists)
			{
				return BadRequest($"The role {roleName} is already exists ...");
			}
			var result = await _userManager.AddToRoleAsync(user, roleName);
			if (result.Succeeded)
			{
				return RedirectToAction("Index", "Home");
			}
			return BadRequest(Json(result.Errors));
		}
	}
}
