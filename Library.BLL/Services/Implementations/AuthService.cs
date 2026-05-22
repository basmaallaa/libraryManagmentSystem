using Library.BLL.Common;
using Library.BLL.Helper;
using Library.BLL.ModelVM.Account;
using Library.BLL.Services.Interfaces;
using Library.DAL.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.BLL.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<Response<string>> RegisterAsync(RegisterVM vm)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(vm.Email);
                if (existingUser != null)
                {
                    return new Response<string>
                    (
                        null,
                        "Email already exists.",
                        false
                    );
                }
                var user = new ApplicationUser
                {
                    FullName = vm.FullName,
                    Email = vm.Email,
                    UserName = vm.Email,
                    Address = vm.Address,
                };
                if (vm.Image != null)
                {
                    user.ProfileImage =
                        await FileHelper.UploadFileAsync(vm.Image, "users");
                }
                var result = await _userManager.CreateAsync(user, vm.Password);
                if (!result.Succeeded)
                {
                    return new Response<string>
                    (
                        null,
                        string.Join(", ", result.Errors.Select(e => e.Description)),
                        false
                    );
                }
                await _userManager.AddToRoleAsync(user, "User");
                return new Response<string>
                (
                    null,
                    "Registration successful.",
                    true
                );
            }
            catch (Exception ex)
            {
                return new Response<string>
                (
                    null,
                    $"An error occurred: {ex.Message}",
                    false
                );
            }
        }
        public async Task<Response<string>> LoginAsync(LoginVM vm)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(vm.Email);
                if (user == null)
                {
                    return new Response<string>
                    (
                        null,
                        "Invalid email or password.",
                        false
                    );
                }
                if (!user.IsActive)
                {
                    return new Response<string>
                    (
                        null,
                        "Account is inactive. Please contact support.",
                        false
                    );
                }
                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName,
                    vm.Password,
                    vm.RememberMe,
                   false);

                if (!result.Succeeded)
                {
                    return new Response<string>
                    (
                        null,
                        "Invalid login attempt.",
                        false
                    );
                }

                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "User";

                return new Response<string>
                    (
                        role,
                        "Login successful.",
                        true
                    );
            }
            catch (Exception ex)
            {
                return new Response<string>
                (
                    null,
                    $"An error occurred: {ex.Message}",
                    false
                );
            }
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        
    }
}
