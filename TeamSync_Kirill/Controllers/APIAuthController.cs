using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TeamSync_Kirill.Models;

namespace TeamSync_Kirill.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class APIAuthController : ControllerBase
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly RoleManager<UserRole> _roleManager;
		private readonly ILogger<APIAuthController> _logger;
		private readonly IConfiguration _configuration;

		public APIAuthController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<UserRole> roleManager, IConfiguration configuration, ILogger<APIAuthController> logger)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
			_configuration = configuration;
			_logger = logger;
		}

		// Register
		[HttpPost("register")]
		public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
		{
			if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
			{
				_logger.LogWarning("Register attempt with missing email or password.");
				return BadRequest("Email and password are required.");
			}

			var user = new User { UserName = model.Email, Email = model.Email, EmailConfirmed = true };
			var result = await _userManager.CreateAsync(user, model.Password);

			if (result.Succeeded)
			{
				_logger.LogInformation("User {Email} created successfully.");
				return Ok("User created successfully.");
			}

			foreach (var error in result.Errors)
			{
				_logger.LogError("Error creating user {Email}: {Error}", model.Email, error.Description);
			}

			return BadRequest(result.Errors);
		}





		//Login
		[HttpPost("login")]
		public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
		{
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null)
			{
				_logger.LogWarning("User {Email} not found.", model.Email);
				return Unauthorized("Invalid email or password.");
			}

			var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
			if (result.Succeeded)
			{
				var token = GenerateJwtToken(user);
				_logger.LogInformation("User {Email} logged in successfully.", model.Email);
				return Ok(new { Token = token });
			}

			return Unauthorized("Invalid login attempt.");
		}

		private string GenerateJwtToken(User user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

			// Обновление SecurityTokenDescriptor
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
					new Claim(ClaimTypes.NameIdentifier, user.Id),
					new Claim(ClaimTypes.Name, user.UserName),
					new Claim(ClaimTypes.Email, user.Email)
				}),
				Expires = DateTime.UtcNow.AddMinutes(30),  // Токен истекает через 30 минут
				NotBefore = DateTime.UtcNow, // Активация токена сразу
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
				Issuer = _configuration["Jwt:Issuer"],
				Audience = _configuration["Jwt:Audience"]
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}
