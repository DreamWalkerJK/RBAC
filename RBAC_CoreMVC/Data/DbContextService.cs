using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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
                options.UseNpgsql("User ID=Test;Password=DreamWalker_JK;Host=localhost;Port=5432;Database=RBAC;Pooling=true;"));
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
}
