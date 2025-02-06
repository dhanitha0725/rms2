using Application.Abstractions.Interfaces;
using Application.DTOs;
using Domain.Common;
using Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtService _jwtService;

        public AuthService(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        public async Task<Result> CreateUserAsync(
            string email, 
            string password, 
            string confirmPassword)
        {
            //check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return Result<string>.Failure(new Error("Email already exists"));
            }

            //check password is matching
            if (password != confirmPassword)
            {
                return Result<string>.Failure(new Error("Password is not matched."));
            }

            //create ApplicationUser
            var user = new ApplicationUser
            {
                Email = email,
                UserName = email
            };

            //create user in the database (commit changes)
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return Result<string>.Failure(new Error("Failed to create user"));
            }

            // create user entity
            var userDto = new UserDto
            {
                UserId = user.Id, 
                Email = user.Email,
                Roles = await _userManager.GetRolesAsync(user)
            };

            //generate jwt token
            var token = _jwtService.GenerateJwtToken(userDto);
            return Result<string>.Success(token);
        }

        public async Task<Result<string>> LoginAsync(string email, string password)
        {
            //find user by email
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Result<string>.Failure(new Error("User not found"));
            }

            //check password
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
            {
                return Result<string>.Failure(new Error("Invalid username or password"));
            }

            var userDto = new UserDto
            {
                UserId = user.Id, // Fix: Keep UserId as string
                Email = user.Email,
                Roles = await _userManager.GetRolesAsync(user)
            };

            var token = _jwtService.GenerateJwtToken(userDto);
            return Result<string>.Success(token);
        }

        public Task<bool> IsEmailUniqueAsync(string email)
        {
            throw new NotImplementedException();
        }
    }
}
