﻿using ECommerceAPI.Application.Abstractions.Services.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.Abstractions.Services
{
    public interface IAuthService: IInternalAuthentication, IExternalAuthentication
    {
        Task PasswordResetAsnyc(string email);
        Task<bool> VerifyResetTokenAsync(string resetToken, string userId);
    }
}
