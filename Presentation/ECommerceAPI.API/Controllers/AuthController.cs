﻿using ECommerceAPI.Application.Features.Commands.AppUser.FacebookLogin;
using ECommerceAPI.Application.Features.Commands.AppUser.GoogleLogin;
using ECommerceAPI.Application.Features.Commands.AppUser.LoginUser;
using ECommerceAPI.Application.Features.Commands.AppUser.PasswordReset;
using ECommerceAPI.Application.Features.Commands.AppUser.RefreshTokenLogin;
using ECommerceAPI.Application.Features.Commands.AppUser.VerifyResetToken;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> LoginUser(LoginUserCommandRequest loginUserCommandRequest)
        => Ok(await _mediator.Send(loginUserCommandRequest));

        [HttpPost("[action]")]
        public async Task<IActionResult> RefreshTokenLogin([FromBody] RefreshTokenLoginCommandRequest refreshTokenLoginCommandRequest)
        => Ok(await _mediator.Send(refreshTokenLoginCommandRequest));

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin(GoogleLoginCommandRequest googleLoginCommandRequest)
        => Ok(await _mediator.Send(googleLoginCommandRequest));

        [HttpPost("facebook-login")]
        public async Task<IActionResult> FacebookLogin(FacebookLoginCommandRequest facebookLoginCommandRequest)
        => Ok(await _mediator.Send(facebookLoginCommandRequest));

        [HttpPost("password-reset")]
        public async Task<IActionResult> PasswordReset([FromBody] PasswordResetCommandRequest passwordResetCommandRequest)
        => Ok(await _mediator.Send(passwordResetCommandRequest));

        [HttpPost("verify-reset-token")]
        public async Task<IActionResult> VerifyResetToken([FromBody] VerifyResetTokenCommandRequest verifyResetTokenCommandRequest)
        => Ok(await _mediator.Send(verifyResetTokenCommandRequest));
    }
}
