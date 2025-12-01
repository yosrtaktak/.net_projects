using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Backend.Core.Entities;
using Backend.Application.DTOs;
using Backend.Application.Services;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtService _jwtService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
    {
        // Vérifier si l'utilisateur existe déjà
        var existingUser = await _userManager.FindByNameAsync(dto.Username);
        if (existingUser != null)
        {
            return BadRequest(new { message = "Username already exists" });
        }

        var existingEmail = await _userManager.FindByEmailAsync(dto.Email);
        if (existingEmail != null)
        {
            return BadRequest(new { message = "Email already exists" });
        }

        // Créer le nouvel utilisateur
        var user = new ApplicationUser
        {
            UserName = dto.Username,
            Email = dto.Email,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new { message = "User creation failed", errors = result.Errors });
        }

        // Assigner le rôle (par défaut Customer)
        var roleToAssign = string.IsNullOrEmpty(dto.Role) ? "Customer" : dto.Role;
        await _userManager.AddToRoleAsync(user, roleToAssign);

        // Générer le token
        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateToken(user, roles);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Username = user.UserName!,
            Email = user.Email!,
            Role = roles.FirstOrDefault() ?? "Customer"
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        // Trouver l'utilisateur
        var user = await _userManager.FindByNameAsync(dto.Username);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        // Vérifier le mot de passe
        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        // Mettre à jour la date de dernière connexion
        user.LastLogin = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        // Générer le token
        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateToken(user, roles);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Username = user.UserName!,
            Email = user.Email!,
            Role = roles.FirstOrDefault() ?? "Customer"
        });
    }
}
