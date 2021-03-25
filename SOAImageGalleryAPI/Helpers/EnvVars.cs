using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOAImageGalleryAPI.Helpers
{
    public static class EnvVars
    {
        public static string[] GetEnvVar(string enviroment, IConfiguration config)
        {
            string[] returnedArr = new string[3];
            if (enviroment == "Development")
            {
                returnedArr[0] = config["MinIOHost"];
                returnedArr[1] = config["MinIOAccessKey"];
                returnedArr[2] = config["MinIOHostSecretKey"];
                return returnedArr;
            }
            else if (enviroment == "Production")
            {
                returnedArr[0] = Environment.GetEnvironmentVariable("IMAGE_GALLERY_MINIO_HOST");
                returnedArr[1] = Environment.GetEnvironmentVariable("IMAGE_GALLERY_MINIO_ACCESS_KEY");
                returnedArr[2] = Environment.GetEnvironmentVariable("IMAGE_GALLERY_MINIO_SECRET_KEY");
                return returnedArr;
            }
            else
            {
                returnedArr[0] = Environment.GetEnvironmentVariable("IMAGE_GALLERY_MINIO_HOST");
                returnedArr[1] = Environment.GetEnvironmentVariable("IMAGE_GALLERY_MINIO_ACCESS_KEY");
                returnedArr[2] = Environment.GetEnvironmentVariable("IMAGE_GALLERY_MINIO_SECRET_KEY");
                return returnedArr;
            }
        }
    }
}
