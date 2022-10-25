using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using RBAC_CoreMVC.Models;

namespace RBAC_CoreMVC.Data
{
    public static class DbContextService
    {
        /// <summary>
        /// 配置服务
        /// </summary>
        /// <returns></returns>
        public static IServiceProvider ServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddDbContext<RBACContext>(options => 
                options.UseNpgsql(AppSettingsHelper.Configuration.GetConnectionString("RBACContext")));
            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }
        /// <summary>
        /// 获取上下文
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static RBACContext GetContext(IServiceProvider services)
        {
            var context = services.GetService<RBACContext>();
            return context;
        }
        /// <summary>
        /// 获取上下文
        /// </summary>
        public static RBACContext GetContext()
        {
            var services = ServiceProvider();
            var context = services.GetService<RBACContext>();
            return context;
        }
    }

    public class AppSettingsHelper
    {
        public static IConfiguration Configuration { get; set; }
        static AppSettingsHelper()
        {
            //ReloadOnChange = true 当appsettings.json被修改时重新加载            
            Configuration = new ConfigurationBuilder()
                .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
                .Build();
        }

    }
}
