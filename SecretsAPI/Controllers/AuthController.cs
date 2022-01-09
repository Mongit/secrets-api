using Microsoft.AspNetCore.Mvc;
using SecretsAPI.Repos;
using SecretsAPI.Models;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace SecretsAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{

    private readonly ILogger<AuthController> _logger;
    private readonly IUsersRepo _usersRepo;
    private readonly IConfiguration _configuration;
    public AuthController(
        ILogger<AuthController> logger,
        IUsersRepo usersRepo,
	    IConfiguration configuration)
    {
        _logger = logger;
        _usersRepo = usersRepo;
        _configuration = configuration;
    }

    [HttpPost("Register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(Login login)
    {
        if(login.IsValid() == false)
        {
            return BadRequest("Username or password too long or too short.");
        }
        try
        {
            await _usersRepo.Create(login);
        }
        catch 
        {
            throw;
            //return BadRequest("Username already exists.");
        }
        return Ok("CREATED. User not active. API administrator may or may not activate your account.");
    }

    [HttpPost("Login")]
    public async Task<ActionResult<string>> Login(Login model)
    {
		if (!await _usersRepo.HasAccess(model))
		{
			return BadRequest("User not found.");
		}

		var user = await _usersRepo.GetOne(model.Username);

		string token = CreateToken(user);
		return Ok(token);
    }

    [NonAction]
    private string CreateToken(User user)
    {
		List<Claim> claims = new List<Claim>
		{
			new Claim(ClaimTypes.Name, user.Username),
			new Claim(ClaimTypes.Role, "Admin"),
			new Claim("Id", user.Id.ToString())
		};

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
   			   _configuration.GetSection("JWT_TOKEN_KEY").Value));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
			claims: claims,
			expires: DateTime.Now.AddSeconds(60),
			signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
    
}
