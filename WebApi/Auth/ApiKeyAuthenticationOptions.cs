using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication;

namespace WebApi.Auth
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public static readonly string Scheme = "X-API-Key";
    }
}