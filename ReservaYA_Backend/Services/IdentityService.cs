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

        public async Task<string> RegisterAsync(string userName)
        {
            var existingUser = await _userManager.FindByNameAsync(userName);
            if (existingUser != null)
            {
                return null;
            }
            var newUser = new UserModel
            {
                UserName = userName

            };

            string password = await passwordGenerator.GeneratePassword();

            var createdUser = await _userManager.CreateAsync(newUser, password);

            if (!createdUser.Succeeded)
            {
                return null;
            }
            var getUser = await _userManager.FindByNameAsync(userName);
            var setRole = await _userManager.AddToRoleAsync(getUser, "USUARIO");

            return password;
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
                    new Claim(type: "id", value: newUser.Id)

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

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);
            if (validatedToken == null)
                return new AuthenticationResult { Errors = new[] { "Invalid Token" } };

            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
                return new AuthenticationResult { Errors = new[] { "This token has't expired yet" } };

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await databaseContext.RefreshTokens.Where(rT => rT.JwtId.Equals(refreshToken)).FirstOrDefaultAsync();

            if (storedRefreshToken == null)
                return new AuthenticationResult { Errors = new[] { "This refresh token does not exist" } };

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                return new AuthenticationResult { Errors = new[] { "This refresh token has expired" } };

            if (storedRefreshToken.Ivalidated)
                return new AuthenticationResult { Errors = new[] { "This refresh token has been invalidated" } };

            if (storedRefreshToken.Used)
                return new AuthenticationResult { Errors = new[] { "This refresh token has been used" } };

            if (storedRefreshToken.JwtId != jti)
                return new AuthenticationResult { Errors = new[] { "This refresh token does not match this JWT" } };

            storedRefreshToken.Used = true;
            databaseContext.RefreshTokens.Update(storedRefreshToken);
            await databaseContext.SaveChangesAsync();

            var UserModel = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);
            //var UserModel = await _userManager.FindByNameAsync(validatedToken.Claims.Single(x => x.Type == "Sub").Value);
            return await GenerateAthenticationResultForUserAsync(UserModel);
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

        public async Task<bool> ChangePassword(string userId, string oldPassword, string newPassWord)
        {
            UserModel tempUser = await _userManager.FindByIdAsync(userId);
            databaseContext.Users.Update(tempUser);
            await databaseContext.SaveChangesAsync();
            var resp = await _userManager.ChangePasswordAsync(tempUser, oldPassword, newPassWord);
            if (resp.Succeeded)
                return true;
            return false;
        }

    }
}
