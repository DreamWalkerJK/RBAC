using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RBAC_CoreMVC.Data;
using RBAC_CoreMVC.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RBAC_CoreMVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly RBACContext _context;

        public LoginController(RBACContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var user = SessionHelper.GetSession(HttpContext.Session, "CurrentUser");
            var userId = SessionHelper.GetSession(HttpContext.Session, "CurrentUserId");
            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(userId))
            {
                ViewBag.CurrentUser = user;
                ViewBag.CurrentUserId = userId;
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Index(LoginDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.Include(u => u.UserRoles).FirstOrDefaultAsync(u => 
                    (u.Code == model.UserName || u.Email == model.UserName || u.Phone == model.UserName) 
                    && u.Password == model.Password && u.IsDeleted == 0);

                if(user != null)
                {
                    SessionHelper.SetSession(HttpContext.Session, "CurrentUserId", user.Id);
                    SessionHelper.SetSession(HttpContext.Session, "CurrentUser", string.Format("{0}({1})",user.Name, user.Code));

                    ViewBag.CurrentUser = string.Format("{0}({1})", user.Name, user.Code);
                    ViewBag.CurrentUserId = user.Id;

                    var menuDtos = new List<MenuDto>();
                    var roleIds = user.UserRoles.Select(u => u.RoleId).ToList();

                    var menuIds = new List<string>();
                    foreach (var roleId in roleIds)
                    {
                        var menuId = _context.RoleMenus.Where(r => r.RoleId == roleId).Select(r => r.MenuId).ToList();
                        menuIds.AddRange(menuId);
                    }

                    var otherMenuIds = menuIds.Distinct();

                    foreach (var menuId in otherMenuIds)
                    {
                        var menu = _context.Menus.Where(m => m.Id == menuId).Select(m => new MenuDto
                        {
                            MenuId = m.Id,
                            MenuName = m.Name,
                            MenuUrl = m.Url.Split(new char[] { '/' }).ToString()
                        }).FirstOrDefault();
                        menuDtos.Add(menu);
                    }

                    ViewBag.CurrentMenu = menuDtos;

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "用户名或密码错误");
                return View();
            }

            return View(model);
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Logout()
        {
            var userId = SessionHelper.GetSession(HttpContext.Session, "CurrentUserId");

            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _context.Users.FindAsync(userId);

                if(user != null)
                {
                    user.LastLoginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
            }

            SessionHelper.RemoveSession(HttpContext.Session, "CurrentUserId");
            SessionHelper.RemoveSession(HttpContext.Session, "CurrentUser");

            ViewBag.CurrentUser = null;
            ViewBag.CurrentUserId = null;

            return RedirectToAction(nameof(Index));
        }
    }
}
