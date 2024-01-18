using ECommerceAPI.Application.Abstractions.Token;
using ECommerceAPI.Application.DTOs;
using ECommerceAPI.Application.Exceptions;
using ECommerceAPI.Application.Features.Commands.AppUser.LoginUser;
using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace ECommerceAPI.Application.Features.Commands.AppUser.GoogleLogin
{
    public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommandRequest, GoogleLoginCommandResponse>
    {
        readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        readonly ITokenHandler _tokenHandler;
        readonly IConfiguration _configuration;

        public GoogleLoginCommandHandler(UserManager<Domain.Entities.Identity.AppUser> userManager, ITokenHandler tokenHandler, IConfiguration configuration)
        {
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _configuration = configuration;
        }

        public async Task<GoogleLoginCommandResponse> Handle(GoogleLoginCommandRequest request, CancellationToken cancellationToken)
        {
            ValidationSettings? settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>()
                { _configuration["ExternalLogin:Google-Client-Id"] }
            };
            Payload payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);

            UserLoginInfo userLoginInfo = new(request.Provider, payload.Subject, request.Provider);
            Domain.Entities.Identity.AppUser user = await _userManager.FindByLoginAsync(userLoginInfo.LoginProvider, userLoginInfo.ProviderKey);
            bool result = user != null;
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    user = new() { Id = Guid.NewGuid().ToString(), Email = payload.Email, UserName = payload.Email, NameSurname = payload.Email };
                    IdentityResult createResult = await _userManager.CreateAsync(user);
                    result = createResult.Succeeded;
                }
            }

            if (result)
                await _userManager.AddLoginAsync(user, userLoginInfo);
            else
                throw new Exception("Invalid external authentication.");

            Token token = _tokenHandler.CreateAccessToken(5);
            return new GoogleLoginSuccessCommandResponse() { Token = token };
        }
    }
}
