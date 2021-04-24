using ConsoleApp.PostgreSQL;
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
        public static string DecodeUid(string authorization)
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

        public static bool Validate(string authorization, DataContext _context)
        {
            try
            {
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:

                    var scheme = headerValue.Scheme;
                    var parameter = headerValue.Parameter;

                    var result = _context.Blacklist.FirstOrDefault(i => i.Token == parameter);

                    if (result == null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }


                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public static DateTime? DecodeExpTime(string authorization)
        {
            try
            {
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:

                    //var scheme = headerValue.Scheme;
                    var parameter = headerValue.Parameter;

                    var token = new JwtSecurityToken(parameter);
                    var timestamp = token.Claims.First(c => c.Type == "exp").Value;

                    if (Int32.TryParse(timestamp, out int expTimestamp))
                    {
                        return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(expTimestamp).ToLocalTime();
                    }

                    return null;

                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                else
                {
                    return null;
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
