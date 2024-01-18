using ECommerceAPI.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.Features.Commands.AppUser.GoogleLogin
{
    public class GoogleLoginCommandResponse
    {
    }
    public class GoogleLoginSuccessCommandResponse : GoogleLoginCommandResponse
    {
        public Token Token { get; set; }
    }
    public class GoogleLoginErrorCommandResponse : GoogleLoginCommandResponse
    {
        public string Message { get; set; }
    }
}
