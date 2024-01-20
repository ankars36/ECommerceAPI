using ECommerceAPI.Application.Abstractions.Services.Authentication;
using ECommerceAPI.Application.DTOs;
using MediatR;

namespace ECommerceAPI.Application.Features.Commands.AppUser.FacebookLogin
{
    public class FacebookLoginCommandHandler : IRequestHandler<FacebookLoginCommandRequest, FacebookLoginCommandResponse>
    {
        readonly IExternalAuthentication _authService;

        public FacebookLoginCommandHandler(IExternalAuthentication authService)
        {
            _authService = authService;
        }

        public async Task<FacebookLoginCommandResponse> Handle(FacebookLoginCommandRequest request, CancellationToken cancellationToken)
        {
            Token token = await _authService.FacebookLoginAsync(request.AuthToken, 15);
            return new FacebookLoginSuccessCommandResponse() { Token = token };
        }
    }
}
