using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SOAImageGalleryAPI.Helpers
{
    public static class TokenDecoder
    {
        public static string Decode(string authorization)
        {
            try
            {
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:

                    var scheme = headerValue.Scheme;
                    var parameter = headerValue.Parameter;

                    var token = new JwtSecurityToken(parameter);
                    return token.Claims.First(c => c.Type == "Id").Value;

                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                else
                {
                    return "Parse error";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
