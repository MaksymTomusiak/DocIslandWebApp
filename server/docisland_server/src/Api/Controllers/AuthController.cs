using Api.Dtos;
using Api.Modules.Errors;
using Application.Users.Commands;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("auth")]
[ApiController]
public class AuthController(
    UserManager<User> userManager,
    ISender sender) : Controller
{
    private const string SuccessVerificationViewName = "VerifyEmailSuccess";
    
    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] UserLoginDto request)
    {
        var command = new LoginUserCommand
        {
            Email = request.Email,
            Password = request.Password
        };
        var result = await sender.Send(command);
        return result.Match(Ok, e => e.ToObjectResult());
    }
    
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] UserRegisterDto request)
    {
        var command = new RegisterUserCommand
        {
            Email = request.Email,
            Password = request.Password,
            UserName = request.UserName
        };
        var result = await sender.Send(command);
        return result.Match(Ok, e => e.ToObjectResult());
    }
    
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] Guid userId, [FromQuery] string token)
    {
        var command = new VerifyEmailCommand { UserId = userId, Token = token };
        var result = await sender.Send(command);
        return result.Match<IActionResult>(
            success => View(SuccessVerificationViewName, success.UserName),
            exception => exception.ToObjectResult()
        );
    }

    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerificationEmail([FromBody] string email)
    {
        var command = new ResendVerificationEmailCommand { Email = email };
        var result = await sender.Send(command);
        return result.Match<IActionResult>(
            _ => Ok(new { message = "Verification email resent successfully" }),
            exception => exception.ToObjectResult()
        );
    }
}