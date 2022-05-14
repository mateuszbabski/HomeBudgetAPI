using HomeBudget.Authentication;
using HomeBudget.Entities;
using HomeBudget.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HomeBudget.Services
{
    public interface IAccountService
    {
        string GenerateJwt(LoginUserModel dto);
        void RegisterUser(RegisterUserModel dto);
    }

    public class AccountService : IAccountService
    {
        private readonly BudgetDbContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AuthenticationSettings _authenticationSettings;

        public AccountService(BudgetDbContext dbContext, 
            IPasswordHasher<User> passwordHasher, 
            AuthenticationSettings authenticationSettings)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
        }

        public void RegisterUser(RegisterUserModel dto)
        {
           
            var newUser = new User()
            {
                FirstName = dto.FirstName,
                Email = dto.Email,
                Password = dto.Password

            };

            var hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);


            newUser.PasswordHash = hashedPassword;
            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();
        }

        public string GenerateJwt(LoginUserModel dto)
        {
            var user = _dbContext.Users.FirstOrDefault(m => m.Email == dto.Email);

            if (user is null)
            {
                throw new BadHttpRequestException("Email not found in database");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadHttpRequestException("Invalid username or password");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName}"),
                new Claim(ClaimTypes.Email, $"{user.Email}")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

            var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);


        }


    }
}
