using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  public class AccountController : BaseApiController
  {
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper)
    {
      _userManager = userManager;
      _mapper = mapper;
      _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
      if (await UserExists(registerDto.UserName)) return BadRequest("Username is taken");

      var user = _mapper.Map<AppUser>(registerDto);

      // using var hmac = new HMACSHA512();
      
        user.UserName = registerDto.UserName.ToLower();
        // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
        // user.PasswordSalt = hmac.Key;
      

      var result = await _userManager.CreateAsync(user, registerDto.Password);

      if(!result.Succeeded) return BadRequest(result.Errors);

      var roleResult = await _userManager.AddToRoleAsync(user, "Member");

      if(!roleResult.Succeeded) return BadRequest(result.Errors);

      return new UserDto
      {
        UserName = user.UserName,
        Token = await _tokenService.CreateToken(user),
        KnownAs = user.KnownAs,
        Gender = user.Gender
      };

    }
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
      var user = await _userManager.Users
        .Include(p => p.Photos)
        .SingleOrDefaultAsync(x => x.UserName == loginDto.UserName);

      if (user == null) return Unauthorized("Invalid Username");

      var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

      if(!result) return Unauthorized("Invalid password");

      // using var hmac = new HMACSHA512(user.PasswordSalt);

      // var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

      // for (int i = 0; i < computedHash.Length; i++)
      // {
      //   if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
      // }
      return new UserDto
      {
        UserName = user.UserName,
        Token = await _tokenService.CreateToken(user),
        PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
        KnownAs = user.KnownAs,
        Gender = user.Gender
      };



    }

    private async Task<bool> UserExists(string username)
    {
      return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
  }
}