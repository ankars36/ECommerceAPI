using ECommerceAPI.Application.Consts;
using ECommerceAPI.Application.CustomAttributes;
using ECommerceAPI.Application.Enums;
using ECommerceAPI.Application.Features.Commands.AppUser.AssignRoleToUser;
using ECommerceAPI.Application.Features.Commands.AppUser.CreateUser;
using ECommerceAPI.Application.Features.Commands.AppUser.FacebookLogin;
using ECommerceAPI.Application.Features.Commands.AppUser.GoogleLogin;
using ECommerceAPI.Application.Features.Commands.AppUser.LoginUser;
using ECommerceAPI.Application.Features.Commands.AppUser.UpdatePassword;
using ECommerceAPI.Application.Features.Queries.AppUser.GetAllUsers;
using ECommerceAPI.Application.Features.Queries.AppUser.GetRolesToUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserCommandRequest createUserCommandRequest)
        => Ok(await _mediator.Send(createUserCommandRequest));

        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordCommandRequest updatePasswordCommandRequest)
        => Ok(await _mediator.Send(updatePasswordCommandRequest));

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Users, ActionType = ActionType.Reading, Definition = "Get All Users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetAllUsersQueryRequest getAllUsersQueryRequest)
        => Ok(await _mediator.Send(getAllUsersQueryRequest));

        [HttpGet("get-roles-to-user/{UserId}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Users, ActionType = ActionType.Reading, Definition = "Get Roles To Users")]
        public async Task<IActionResult> GetRolesToUser([FromRoute] GetRolesToUserQueryRequest getRolesToUserQueryRequest)
        => Ok(await _mediator.Send(getRolesToUserQueryRequest));

        [HttpPost("assign-role-to-user")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Users, ActionType = ActionType.Writing, Definition = "Assign Role To User")]
        public async Task<IActionResult> AssignRoleToUser(AssignRoleToUserCommandRequest assignRoleToUserCommandRequest)
        => Ok(await _mediator.Send(assignRoleToUserCommandRequest));
    }
}
