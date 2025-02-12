using Application.Abstractions.Interfaces;
using Application.DTOs.UserDtos;
using Domain.Common;
using Identity.Models;
//using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;

namespace Identity.Services
{
    public class AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtService jwtService) : IAuthService
    {
        public async Task<Result> CreateUserAsync(
            string email, 
            string password, 
            string confirmPassword)
        {
            //check if email already exists
            var existingUser = await userManager.FindByEmailAsync(email);
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
            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return Result<string>.Failure(new Error("Failed to create user"));
            }

            // create user entity
            var userDto = new UserDto
            {
                UserId = user.Id, 
                Email = user.Email,
                Roles = await userManager.GetRolesAsync(user)
            };

            //generate jwt token
            var token = jwtService.GenerateJwtToken(userDto);
            return Result<string>.Success(token);
        }

        public async Task<Result<string>> LoginAsync(string email, string password)
        {
            //find user by email
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Result<string>.Failure(new Error("Invalid username or password"));
            }

            //check password
            var result = await signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
            {
                return Result<string>.Failure(new Error("Invalid username or password"));
            }

            var userDto = new UserDto
            {
                UserId = user.Id,
                Email = user.Email,
                Roles = await userManager.GetRolesAsync(user)
            };

            var token = jwtService.GenerateJwtToken(userDto);
            return Result<string>.Success(token);
        }

        public Task<bool> IsEmailUniqueAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<string>> AddUserAsync(
            string email, 
            string password, 
            string role)
        {
            //allowed roles
            var allowedRoles = new List<string> { 
                "Admin", 
                "Employee",
                "Accountant",
                "Hostel" };

            if (!allowedRoles.Contains(role))
            {
                return Result<string>.Failure(new Error("Invalid role"));
            }

            //check if user already exists
            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return Result<string>.Failure(new Error("User already exists"));
            }

            //create ApplicationUser
            var user = new ApplicationUser
            {
                Email = email,
                UserName = email
            };

            //create user in the database (commit changes)
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return Result<string>.Failure(new Error("Failed to create user"));
            }

            // assign role to the user
            await userManager.AddToRoleAsync(user, role);

            return Result<string>.Success("User created successfully");
        }
    }
}
