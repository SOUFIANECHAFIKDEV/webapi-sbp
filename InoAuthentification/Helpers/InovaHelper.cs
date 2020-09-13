using InoAuthentification.DbContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace InoAuthentification.Helpers
{
    public class InovaHelper
    {
        private static IHttpContextAccessor _httpContextAccessor;
        private static DbContextOptionsBuilder _dbContextOptionsBuilder;
        public static void Configure(IHttpContextAccessor httpContextAccessor, string connectionString)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContextOptionsBuilder = new DbContextOptionsBuilder<InoAuthentificationDbContext>();
            _dbContextOptionsBuilder.UseMySQL(connectionString);
        }
        public static HttpContext HttpCurrent => _httpContextAccessor.HttpContext;
        public static DbContextOptions _dbContextOptions => _dbContextOptionsBuilder.Options;
    }
}
