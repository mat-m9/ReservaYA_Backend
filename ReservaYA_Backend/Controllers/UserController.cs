using Microsoft.AspNetCore.Mvc;
using ReservaYA_Backend.ResponseModels;
using ReservaYA_Backend.Services;

namespace ReservaYA_Backend.Controllers
{
    public class UserController : Controller
    {
        private readonly IIdentityService _identityService;

        public UserController(IIdentityService identityService)
        {
            _identityService = identityService;
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
    }
}
