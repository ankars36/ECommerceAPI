using ECommerceAPI.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.Features.Commands.AppUser.FacebookLogin
{
    public class FacebookLoginCommandResponse
    {
    }
    public class FacebookLoginSuccessCommandResponse : FacebookLoginCommandResponse
    {
        public Token Token { get; set; }
    }
    public class FacebookLoginErrorCommandResponse : FacebookLoginCommandResponse
    {
        public string Message { get; set; }
    }
}
