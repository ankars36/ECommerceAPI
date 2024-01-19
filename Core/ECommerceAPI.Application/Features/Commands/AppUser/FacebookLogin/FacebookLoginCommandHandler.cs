using ECommerceAPI.Application.Abstractions.Token;
using ECommerceAPI.Application.DTOs;
using ECommerceAPI.Application.DTOs.Facebook;
using ECommerceAPI.Application.Features.Commands.AppUser.GoogleLogin;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.Features.Commands.AppUser.FacebookLogin
{
    public class FacebookLoginCommandHandler : IRequestHandler<FacebookLoginCommandRequest, FacebookLoginCommandResponse>
    {
        readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        readonly ITokenHandler _tokenHandler;
        readonly IConfiguration _configuration;
        readonly HttpClient _httpClient;

        public FacebookLoginCommandHandler(UserManager<Domain.Entities.Identity.AppUser> userManager, ITokenHandler tokenHandler, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<FacebookLoginCommandResponse> Handle(FacebookLoginCommandRequest request, CancellationToken cancellationToken)
        {
            string accessTokenResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={_configuration["ExternalLogin:Facebook:Client-Id"]}&client_secret={_configuration["ExternalLogin:Facebook:Client-Secret"]}&grant_type=client_credentials");
            FacebookAccessTokenResponse facebookAccessTokenResponse = JsonSerializer.Deserialize<FacebookAccessTokenResponse>(accessTokenResponse);


            string userAccessTokenValidation = await _httpClient.GetStringAsync($"https://graph.facebook.com/{request.AuthToken}/accounts?access_token={facebookAccessTokenResponse.AccessToken}");
            FacebookUserAccessTokenValidation validation = JsonSerializer.Deserialize<FacebookUserAccessTokenValidation>(userAccessTokenValidation);

            if (validation.Data.IsValid)
            {
                string userInfoResponse= await _httpClient.GetStringAsync($"https://graph.facebook.com/me?fields=email,name&access_token={request.AuthToken}");
                FacebookUserInfoResponse userInfo= JsonSerializer.Deserialize<FacebookUserInfoResponse>(userInfoResponse);

                UserLoginInfo userLoginInfo = new("FACEBOOK", validation.Data.UserId, "FACEBOOK");
                Domain.Entities.Identity.AppUser user = await _userManager.FindByLoginAsync(userLoginInfo.LoginProvider, userLoginInfo.ProviderKey);
                bool result = user != null;
                if (user == null)
                {
                    user = await _userManager.FindByEmailAsync(userInfo.Email);
                    if (user == null)
                    {
                        user = new() { Id = Guid.NewGuid().ToString(), Email = userInfo.Email, UserName = userInfo.Email, NameSurname = userInfo.Name };
                        IdentityResult createResult = await _userManager.CreateAsync(user);
                        result = createResult.Succeeded;
                    }

                    if (result)
                        await _userManager.AddLoginAsync(user, userLoginInfo);
                    else
                        throw new Exception("Invalid external authentication.");
                }
            }

            Token token = _tokenHandler.CreateAccessToken(5);
            return new FacebookLoginSuccessCommandResponse() { Token = token };
        }
    }
}
