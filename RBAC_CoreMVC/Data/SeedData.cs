using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RBAC_CoreMVC.Models;

namespace RBAC_CoreMVC.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new RBACContext(
                serviceProvider.GetRequiredService<DbContextOptions<RBACContext>>()))
            {
                if (context.Users.Any())
                {
                    return;
                }

                string departmentId = Guid.NewGuid().ToString();
                string userId = Guid.NewGuid().ToString();
                string roleId = Guid.NewGuid().ToString();

                string menuId = Guid.NewGuid().ToString();
                string menuId1 = Guid.NewGuid().ToString();
                string menuId2 = Guid.NewGuid().ToString();
                string menuId3 = Guid.NewGuid().ToString();

                context.Departments.Add(
                    new Department
                    {
                        Id = departmentId,
                        Code = "C0000",
                        Name = "RBAC",
                        CreateUserId = userId
                    });

                context.Roles.Add(
                    new Role
                    {
                        Id = roleId,
                        Code = "R0000",
                        Name = "管理员",
                        CreateUserId = userId
                    });

                context.Menus.AddRange(
                    new Menu
                    {
                        Id = menuId,
                        Code = "M0000",
                        Name = "功能管理",
                        Url = "/Menu/Index",
                        Type = 0,
                        CreateUserId = userId
                    },
                    new Menu
                    {
                        Id = menuId1,
                        Code = "M0001",
                        Name = "角色管理",
                        Url = "/Role/Index",
                        Type = 0,
                        CreateUserId = userId
                    },
                    new Menu
                    {
                        Id = menuId2,
                        Code = "M0002",
                        Name = "部门管理",
                        Url = "/Department/Index",
                        Type = 0,
                        CreateUserId = userId
                    },
                    new Menu
                    {
                        Id = menuId3,
                        Code = "M0003",
                        Name = "用户管理",
                        Url = "/User/Index",
                        Type = 0,
                        CreateUserId = userId
                    });

                context.RoleMenus.AddRange(
                    new RoleMenu
                    {
                        RoleId = roleId,
                        MenuId = menuId
                    },
                    new RoleMenu
                    {
                        RoleId = roleId,
                        MenuId = menuId1
                    },
                    new RoleMenu
                    {
                        RoleId = roleId,
                        MenuId = menuId2
                    },
                    new RoleMenu
                    {
                        RoleId = roleId,
                        MenuId = menuId3
                    });

                context.Users.Add(
                    new User
                    {
                        Id = userId,
                        Code = "admin",
                        Name = "超级管理员",
                        Password = "123456",
                        DepartmentId = departmentId
                    });

                context.UserRoles.Add(
                    new UserRole
                    {
                        UserId = userId,
                        RoleId = roleId
                    });

                context.SaveChanges();
            }
        }
    }
}
