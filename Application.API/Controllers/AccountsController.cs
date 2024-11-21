using Application.API.jwtFeatures;
using Application.Domain.Madels;
using AutoMapper;
using Infrastructure.DTO;
using Infrastructure.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Application.API.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;
        private readonly JwtHandler _jwtHandler;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(UserManager<User> userManager, ILogger<AccountsController> logger,
            IMapper mapper, JwtHandler jwtHandler)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this._logger = logger;
            _jwtHandler = jwtHandler;
        }
        /*        [HttpPost("register")]
                public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistrationDto)
                {
                    if (userForRegistrationDto is null)
                        return BadRequest();

                    var user = mapper.Map<User>(userForRegistrationDto);
                    var result = await userManager.CreateAsync(user, userForRegistrationDto.Password);

                    if (!result.Succeeded)
                    {
                        var errors = result.Errors.Select(e => e.Description);

                        return BadRequest(new RegistrationResponseDto { Errors = errors });
                    }

                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var param = new Dictionary<string, string?>
                    {
                        { "token" , token  },
                        {"email" , user.Email }
                    };
                    var callback = QueryHelpers.AddQueryString(userForRegistrationDto.ClientUri!, param);

                    var confirmationLink = Url.Action("ConfirmEmail", "Email", new { token, email = user.Email }, Request.Scheme);

                    EmailHelper emailHelper = new EmailHelper();
                    bool emailResponse = emailHelper.SendEmail(user.Email, confirmationLink);

                    return StatusCode(201);
                }
           */

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistrationDto)
        {
            if (userForRegistrationDto is null)
                return BadRequest(new { message = "User registration data is required." });

            var user = mapper.Map<User>(userForRegistrationDto);
            var result = await userManager.CreateAsync(user, userForRegistrationDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                _logger.LogWarning("Failed to register user: {Errors}", string.Join(", ", errors));
                return BadRequest(new RegistrationResponseDto { Errors = errors });
            }

            // توليد رمز تأكيد البريد الإلكتروني
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            // تأكيد المستخدم تلقائيًا
            var confirmResult = await userManager.ConfirmEmailAsync(user, token);
            if (!confirmResult.Succeeded)
            {
                var errors = confirmResult.Errors.Select(e => e.Description);
                _logger.LogError("Failed to confirm user email: {Errors}", string.Join(", ", errors));
                return StatusCode(500, "An error occurred while confirming the user.");
            }

            return StatusCode(201, new { message = "User registered and confirmed successfully." });
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto userForAuthenticationDto)
        {
            var user = await userManager.FindByNameAsync(userForAuthenticationDto.Email!);

            if (user is null)
                return BadRequest("Invalid Request");

            if (!await userManager.IsEmailConfirmedAsync(user))
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Email is not confirmed" });

            if (!await userManager.CheckPasswordAsync(user, userForAuthenticationDto.Password!))
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Invalid Authentication " });

            var roles = await userManager.GetRolesAsync(user);
            var token = _jwtHandler.CreateToken(user, roles);

            return Ok(new AuthResponseDto { IsAuthSuccessful = true, Token = token });
        }

        [HttpGet("emailconfirmation")]
        public async Task<IActionResult> EmailConfirmation([FromBody] string email, [FromQuery] string token)
        {
            var user = await userManager.FindByIdAsync(email);
            if (user is null)
                return BadRequest("Invalid Email Confirmation Request ");
            var confirmResult = await userManager.ConfirmEmailAsync(user, token);
            if (!confirmResult.Succeeded)
                return BadRequest("Invalid Email Confirmation Request ");
            return Ok();
        }

    }
}