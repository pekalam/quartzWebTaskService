using System.Net.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.Primitives;

namespace WebApi.Auth
{
    public class ApiKeysStrings
    {
        public string ClientKey { get; set; }
        public string ManagmentAppKey { get; set; }
    }

    public class ApiKey
    {
        public Claim Claim { get; }
        public string Key { get; }

        public ApiKey(Claim claim, string key)
        {
            Claim = claim;
            Key = key;
        }

        public static ApiKey From(string str, bool clientApp)
        {
            if (clientApp)
            {
                return new ApiKey(new Claim(ClaimTypes.Role, "Client"), str);
            }
            else
            {
                return new ApiKey(new Claim(ClaimTypes.Role, "ManagmentApp"), str);
            }
        }
    }
}