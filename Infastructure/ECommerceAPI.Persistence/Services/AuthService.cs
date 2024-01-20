using ECommerceAPI.Application.Abstractions.Services;
using ECommerceAPI.Application.Abstractions.Token;
using ECommerceAPI.Application.DTOs;
using ECommerceAPI.Application.DTOs.Facebook;
using ECommerceAPI.Application.Exceptions;
using ECommerceAPI.Application.Features.Commands.AppUser.FacebookLogin;
using ECommerceAPI.Application.Features.Commands.AppUser.GoogleLogin;
using ECommerceAPI.Application.Features.Commands.AppUser.LoginUser;
using ECommerceAPI.Domain.Entities.Identity;
using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace ECommerceAPI.Persistence.Services
{
    public class AuthService : IAuthService
    {
        readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        readonly SignInManager<Domain.Entities.Identity.AppUser> _signInManager;
        readonly ITokenHandler _tokenHandler;
        readonly IConfiguration _configuration;
        readonly HttpClient _httpClient;

        public AuthService(UserManager<AppUser> userManager, ITokenHandler tokenHandler, IConfiguration configuration, HttpClient httpClient, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _configuration = configuration;
            _httpClient = httpClient;
            _signInManager = signInManager;
        }

        async Task<Token> CreateUserExternalAsync(AppUser user, string email, string name, UserLoginInfo userLoginInfo, int accessTokenLifeTime)
        {
            bool result = user != null;
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = email,
                        UserName = email,
                        NameSurname = name
                    };
                    IdentityResult createResult = await _userManager.CreateAsync(user);
                    result = createResult.Succeeded;
                }

                if (result)
                    await _userManager.AddLoginAsync(user, userLoginInfo);
                else
                    throw new Exception("Invalid external authentication.");
            }
            return _tokenHandler.CreateAccessToken(accessTokenLifeTime);
        }

        public async Task<Token> FacebookLoginAsync(string authToken, int accessTokenLifeTime)
        {
            string accessTokenResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={_configuration["ExternalLogin:Facebook:Client-Id"]}&client_secret={_configuration["ExternalLogin:Facebook:Client-Secret"]}&grant_type=client_credentials");
            FacebookAccessTokenResponse? facebookAccessTokenResponse = JsonSerializer.Deserialize<FacebookAccessTokenResponse>(accessTokenResponse);


            string userAccessTokenValidation = await _httpClient.GetStringAsync($"https://graph.facebook.com/{authToken}/accounts?access_token={facebookAccessTokenResponse?.AccessToken}");
            FacebookUserAccessTokenValidation? validation = JsonSerializer.Deserialize<FacebookUserAccessTokenValidation>(userAccessTokenValidation);

            if (validation?.Data.IsValid != null)
            {
                string userInfoResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/me?fields=email,name&access_token={authToken}");
                FacebookUserInfoResponse? userInfo = JsonSerializer.Deserialize<FacebookUserInfoResponse>(userInfoResponse);

                UserLoginInfo userLoginInfo = new("FACEBOOK", validation.Data.UserId, "FACEBOOK");
                Domain.Entities.Identity.AppUser user = await _userManager.FindByLoginAsync(userLoginInfo.LoginProvider, userLoginInfo.ProviderKey);

                return await CreateUserExternalAsync(user, userInfo.Email, userInfo.Name, userLoginInfo, accessTokenLifeTime);
            }
            throw new Exception("Invalid external authentication.");
        }

        public async Task<Token> GoogleLoginAsync(string idToken, int accessTokenLifeTime)
        {
            ValidationSettings? settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>()
                { _configuration["ExternalLogin:Google-Client-Id"] }
            };
            Payload payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            UserLoginInfo userLoginInfo = new("GOOGLE", payload.Subject, "GOOGLE");
            Domain.Entities.Identity.AppUser user = await _userManager.FindByLoginAsync(userLoginInfo.LoginProvider, userLoginInfo.ProviderKey);

            return await CreateUserExternalAsync(user, payload.Email, payload.Name, userLoginInfo, accessTokenLifeTime);
        }

        public async Task<Token> LoginAsync(string usernameOrEmail, string password, int accessTokenLifeTime)
        {
            Domain.Entities.Identity.AppUser user = await _userManager.FindByNameAsync(usernameOrEmail);
            if (user == null)
                user = await _userManager.FindByEmailAsync(usernameOrEmail);

            if (user == null)
                throw new NotFoundUserException();

            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (result.Succeeded)
            {
                return _tokenHandler.CreateAccessToken(accessTokenLifeTime);
            }

            throw new AuthenticationErrorException();
        }
    }
}
