using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RBAC_CoreMVC.Data;
using RBAC_CoreMVC.DTOs;
using RBAC_CoreMVC.Models;

namespace RBAC_CoreMVC.Controllers
{
    public abstract class RBACControllerBase : Controller
    {
        /// <summary>
        /// 检验是否登录以及是否有权限
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sqlContext = DbContextService.GetContext();

            var controllerName = ControllerContext.HttpContext.Request.RouteValues["controller"].ToString();

            var user = SessionHelper.GetSession(HttpContext.Session, "CurrentUser");
            var userId = SessionHelper.GetSession(HttpContext.Session, "CurrentUserId");

            ViewBag.CurrentUser = user;
            ViewBag.CurrentUserId = userId;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(userId))
            {
                context.Result = new RedirectResult("/Login/Index");
                return;
            }

            if (!string.IsNullOrEmpty(userId) && !controllerName.Equals("Login"))
            {
                var menuDtos = new List<MenuDto>();
                var tempUser = sqlContext.Users.Include(u => u.UserRoles).FirstOrDefault(u => u.Id == userId);
                var roleIds = tempUser.UserRoles.Select(u => u.RoleId).ToList();

                var menuIds = new List<string>();
                foreach (var roleId in roleIds)
                {
                    var menuId = sqlContext.RoleMenus.Where(r => r.RoleId == roleId).Select(r => r.MenuId).ToList();
                    menuIds.AddRange(menuId);
                }

                var otherMenuIds = menuIds.Distinct();

                foreach (var menuId in otherMenuIds)
                {
                    var menu = sqlContext.Menus.Where(m => m.Id == menuId).Select(m => new MenuDto
                    {
                        MenuId = m.Id,
                        MenuName = m.Name,
                        MenuUrl = m.Url
                    }).FirstOrDefault();
                    menuDtos.Add(menu);
                }

                bool isRight = false;
                if (controllerName.Equals("Home"))
                {
                    isRight = true;
                }
                else
                {
                    foreach (var menuDto in menuDtos)
                    {
                        if (menuDto.MenuUrl.Split('/')[1].Equals(controllerName))
                        {
                            isRight = true;
                            break;
                        }
                    }
                }

                if (!isRight)
                {
                    context.Result = new RedirectResult("/Home/NotRight");
                    return;
                }

                ViewBag.CurrentMenu = menuDtos;
            }

            base.OnActionExecuting(context);
        }
    }
}
