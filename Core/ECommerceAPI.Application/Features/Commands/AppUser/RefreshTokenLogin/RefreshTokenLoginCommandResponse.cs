using ECommerceAPI.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.Features.Commands.AppUser.RefreshTokenLogin
{
    public class RefreshTokenLoginCommandResponse
    {
    }
    public class RefreshTokenLoginSuccessCommandResponse : RefreshTokenLoginCommandResponse
    {
        public Token Token { get; set; }
    }
    public class RefreshTokenLoginErrorCommandResponse : RefreshTokenLoginCommandResponse
    {
        public string Message { get; set; }
    }
}
