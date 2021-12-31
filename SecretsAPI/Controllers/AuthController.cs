using Microsoft.AspNetCore.Mvc;
using SecretsAPI.Repos;
using SecretsAPI.Models;
using System.Threading.Tasks;

namespace SecretsAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{

    private readonly ILogger<AuthController> _logger;
    private readonly IUsersRepo _usersRepo;

    public AuthController(
        ILogger<AuthController> logger,
        IUsersRepo usersRepo)
    {
        _logger = logger;
        _usersRepo = usersRepo;
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
    
}
