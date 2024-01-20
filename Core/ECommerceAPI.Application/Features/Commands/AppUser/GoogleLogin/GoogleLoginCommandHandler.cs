using ECommerceAPI.Application.Abstractions.Services.Authentication;
using ECommerceAPI.Application.DTOs;
using MediatR;

namespace ECommerceAPI.Application.Features.Commands.AppUser.GoogleLogin
{
    public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommandRequest, GoogleLoginCommandResponse>
    {
        readonly IExternalAuthentication _authService;

        public GoogleLoginCommandHandler(IExternalAuthentication authService)
        {
            _authService = authService;
        }

        public async Task<GoogleLoginCommandResponse> Handle(GoogleLoginCommandRequest request, CancellationToken cancellationToken)
        {
            Token token = await _authService.GoogleLoginAsync(request.IdToken, 15);

            return new GoogleLoginSuccessCommandResponse() { Token = token };
        }
    }
}
