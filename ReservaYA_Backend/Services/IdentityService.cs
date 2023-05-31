using Microsoft.AspNetCore.Identity;
using ReservaYA_Backend.Options;
using ReservaYA_Backend.ResponseModels;
using ReservaYA_Backend.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;

namespace ReservaYA_Backend.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly DatabaseContext databaseContext;
        private readonly PasswordGeneratorService passwordGenerator;

        public IdentityService(UserManager<UserModel> userName, JwtSettings jwtSettings, TokenValidationParameters tokenValidationParameters, DatabaseContext databaseContext, PasswordGeneratorService passwordGeneratorService)
        {
            _userManager = userName;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            this.databaseContext = databaseContext;
            this.passwordGenerator = passwordGeneratorService;
        }

        public async Task<AuthenticationResult> RegisterAsync(string mail, string password)
        {
            var existingUser = await _userManager.FindByNameAsync(mail);
            if (existingUser != null)
            {
                return null;
            }
            var newUser = new UserModel
            {
                UserName = mail

            };

            var createdUser = await _userManager.CreateAsync(newUser, password);

            if (!createdUser.Succeeded)
            {
                return null;
            }
            var getUser = await _userManager.FindByNameAsync(mail);
            var setRole = await _userManager.AddToRoleAsync(getUser, "USUARIO");

            return await GenerateAthenticationResultForUserAsync(newUser);
        }

        public async Task<AuthenticationResult> LoginAsync(string userName, string password)
        {
            var UserModel = await _userManager.FindByNameAsync(userName);
            if (UserModel == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "El usuario no existe" }
                };
            }
            var userHasValidPassword = await _userManager.CheckPasswordAsync(UserModel, password);
            if (!userHasValidPassword)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "El usuario o la contraseña son incorrectos" }
                };
            }
            return await GenerateAthenticationResultForUserAsync(UserModel);
        }



        private async Task<AuthenticationResult> GenerateAthenticationResultForUserAsync(UserModel newUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new List<Claim>();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(claims: new[]
                {
                    new Claim(type: JwtRegisteredClaimNames.Sub, value: newUser.UserName),
                    new Claim(type: JwtRegisteredClaimNames.Jti, value: Guid.NewGuid().ToString()),
                    new Claim(type: "id", value: newUser.Id),
                    new Claim(type: "numReservas", value: newUser.NumReservas.ToString())

                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenLifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = newUser.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenLifeTime)
            };

            await databaseContext.RefreshTokens.AddAsync(refreshToken);
            await databaseContext.SaveChangesAsync();

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.JwtId
            };
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                    return null;
                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase);
        }

        public async Task<bool> ChangePassword(string mail, string oldPassword, string newPassWord)
        {
            UserModel tempUser = await _userManager.FindByNameAsync(mail);
            databaseContext.Users.Update(tempUser);
            await databaseContext.SaveChangesAsync();
            var resp = await _userManager.ChangePasswordAsync(tempUser, oldPassword, newPassWord);
            if (resp.Succeeded)
                return true;
            return false;
        }

    }
}
