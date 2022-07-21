using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Resteurant_API.Authentication;
using Resteurant_API.Claims;
using Resteurant_API.DataContext;
using Resteurant_API.Dtos;
using Resteurant_API.Entities;
using Resteurant_API.Exceptions;
using Resteurant_API.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Resteurant_API.Services
{
    public class AccountService : IAccountService
    {
        private readonly ResteurantDbContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private const string databaseErrorMessage = "Database operation error occured";

        public AccountService(ResteurantDbContext dbContext, IPasswordHasher<User> passwordHasher,
            AuthenticationSettings authenticationSettings, AuthenticationStateProvider authenticationStateProvider, 
            IHttpContextAccessor httpContextAccessor, HttpClient httpClient)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
            _authenticationStateProvider = authenticationStateProvider;
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
        }

        #region Register
        public async Task RegisterUser(RegisterUserDto dto)
        {
            var newUser = new User()
            {
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                Nationality = dto.Nationality,
                RoleId = dto.RoleId
            };

            var hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);
            newUser.PasswordHash = hashedPassword;

            try
            {
                await _dbContext.Users.AddAsync(newUser);
                await _dbContext.SaveChangesAsync();
            }
            catch (DatabaseOperationException)
            {
                throw new DatabaseOperationException(databaseErrorMessage);
            }
        }
        #endregion

        #region Login
        public async Task<string> Login_UseJSONWebToken(LoginUserDto dto)
        {
            var user = await Authenticate(dto);
            var token = await GenerateJSONWebToken(user);
            try
            {
                await ((CustomAuthStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(token);
                _httpContextAccessor.HttpContext.Session.SetString("authToken", token);

                return await GenerateJSONWebToken(user);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task Login_UseCookies(LoginUserDto dto)
        {
            var user = await Authenticate(dto);

            if (user != null)
            {
                var claims = UserClaims.CreateClaims(user);
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await _httpContextAccessor.HttpContext.SignInAsync(claimsPrincipal);
            }
            
        }

        private async Task<User> Authenticate(LoginUserDto dto)
        {
            var user = await _dbContext.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user is null)
                throw new NotUniqueItemException("Invalid username or password");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
                throw new NotUniqueItemException("Invalid username or password");

            return user;
        }

        private Task<string> GenerateJSONWebToken(User user)
        {
            var claims = UserClaims.CreateClaims(user);
            var pk = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var credentials = new SigningCredentials(pk, SecurityAlgorithms.HmacSha256);
            var expireDate = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

            var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer, _authenticationSettings.JwtIssuer, 
                claims, expires: expireDate, signingCredentials: credentials);
            var jwt =  new JwtSecurityTokenHandler().WriteToken(token);

            return Task.FromResult(jwt);
        }
        #endregion

        public async Task Logout()
        {
            await _httpContextAccessor.HttpContext
                .SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
