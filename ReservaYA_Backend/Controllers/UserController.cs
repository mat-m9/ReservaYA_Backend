using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReservaYA_Backend.Models;
using ReservaYA_Backend.ResponseModels;
using ReservaYA_Backend.Services;

namespace ReservaYA_Backend.Controllers
{
    public class UserController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly EmailService _emailService;
        private readonly UserManager<UserModel> _userManager;

        public UserController(IIdentityService identityService, EmailService emailService, UserManager<UserModel> userManager)
        {
            _identityService = identityService;
            _emailService = emailService;
            _userManager = userManager;
        }

        [HttpPost(template: ApiRoutes.User.Register)]
        public async Task<IActionResult> Register( UserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var authResponse = await _identityService.RegisterAsync(request.mail, request.password);


            if (authResponse == null)
            {
                return BadRequest();
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token
            });
        }

        [HttpPost(template: ApiRoutes.User.Login)]
        public async Task<IActionResult> Login([FromBody] UserRequest request)
        {
            var authResponse = await _identityService.LoginAsync(request.mail, request.password);

            if (!authResponse.Success)
            {
                return BadRequest();
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token
            });
        }

        [HttpPut(ApiRoutes.User.Change)]
        public async Task<IActionResult> ChangePassWord([FromBody] ChangePasswordRequest changePasswordRequest)
        {
            bool response = await _identityService.ChangePassword(changePasswordRequest.Mail, changePasswordRequest.OldPassword, changePasswordRequest.NewPassword);
            if (response)
                return Ok();
            return BadRequest();
        }

        [HttpPost(ApiRoutes.User.Recover)]
        public async Task<IActionResult> RecoverPassword(string email)
        {
            var user = await _userManager.FindByNameAsync(email);
            if (user == null /*|| !(await _userManager.IsEmailConfirmedAsync(user))*/)
                return BadRequest("Usuario no encontrado");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, token = token }, Request.Scheme);

            var subject = "Recuperación de contraseña";
            var message = $"Para restablecer tu contraseña, haz clic <a href='{callbackUrl}'>aquí</a>.";
            await _emailService.SendEmailAsync(email, subject, message);

            return Ok("Se ha enviado un correo de recuperación");
        }
    }
}
