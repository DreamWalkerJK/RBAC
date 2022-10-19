using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RBAC_CoreMVC.Data;
using RBAC_CoreMVC.DTOs;
using RBAC_CoreMVC.Models;

namespace RBAC_CoreMVC.Controllers
{
    public class RoleController : RBACControllerBase
    {
        private readonly RBACContext _context;

        public RoleController(RBACContext context)
        {
            _context = context;
        }

        // GET: Role
        public async Task<IActionResult> Index(string roleName,
            string currentName, int? page)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                page = 1;
            }
            else
            {
                roleName = currentName;
            }

            ViewData["CurrentName"] = currentName;

            var roles = _context.Roles.Where(u => u.IsDeleted == 0);

            if (!string.IsNullOrEmpty(roleName))
            {
                roles = roles.Where(u => u.Name.Contains(roleName));
            }

            roles = roles.OrderBy(r => r.Code);

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(await PagedList<Role>
                .CreateAsync(roles.AsNoTracking(), pageNumber, pageSize));
        }

        // GET: Role/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // GET: Role/Create
        public IActionResult Create()
        {
            var role = new Role();
            role.RoleMenus = new List<RoleMenu>();
            GetTypeMenuList(role);
            return View();
        }

        // POST: Role/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,Name,Remarks")] Role role, string[] selectedMenu)
        {
            role.CreateUserId = SessionHelper.GetSession(HttpContext.Session, "CurrentUserId");

            if(selectedMenu != null)
            {
                role.RoleMenus = new List<RoleMenu>();
                foreach(var menu in selectedMenu)
                {
                    var roleMenuToAdd = new RoleMenu();
                    roleMenuToAdd.RoleId = role.Id;
                    roleMenuToAdd.MenuId = menu;
                    role.RoleMenus.Add(roleMenuToAdd);
                }
            }

            if (ModelState.IsValid)
            {
                if (RoleCodeExists(role.Code))
                {
                    ModelState.AddModelError("", "角色编码已存在");
                    GetTypeMenuList(role);
                    return View(role);
                }

                _context.Add(role);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            GetTypeMenuList(role);
            return View(role);
        }

        // GET: Role/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles.Include(r => r.RoleMenus)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (role == null)
            {
                return NotFound();
            }

            GetTypeMenuList(role);
            return View(role);
        }

        // POST: Role/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string[] selectedMenu)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var roleToUpdate = await _context.Roles.Include(r => r.RoleMenus)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (await TryUpdateModelAsync(roleToUpdate, "",
                r => r.Id,
                r => r.Code,
                r => r.Name,
                r => r.Remarks))
            {
                try
                {
                    UpdataRoleMenus(selectedMenu, roleToUpdate);

                    roleToUpdate.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    _context.Update(roleToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleExists(roleToUpdate.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            GetTypeMenuList(roleToUpdate);
            return View(roleToUpdate);
        }

        // GET: Role/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: Role/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var role = await _context.Roles.FindAsync(id);
            //_context.Roles.Remove(role);
            role.IsDeleted = 1;
            role.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _context.Update(role);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoleExists(string id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }

        private bool RoleCodeExists(string code)
        {
            return _context.Roles.Any(r => r.Code == code);
        }

        private void GetTypeMenuList(Role role)
        {
            var menus = _context.Menus.Where(m => m.IsDeleted == 0
            && m.Type == 0
            && string.IsNullOrEmpty(m.ParentId));
            var selectedMenu = new HashSet<string>(role.RoleMenus.Select(r => r.MenuId));
            var menuList = new List<MenuDto>();

            foreach(var menu in menus)
            {
                menuList.Add(new MenuDto
                {
                    MenuId = menu.Id,
                    MenuName = menu.Name,
                    IsSelected = selectedMenu.Contains(menu.Id)
                });
            }

            ViewData["MenuList"] = menuList;
        }

        private void UpdataRoleMenus(string[] selectedMenu, Role roleToUpdate)
        {
            if (selectedMenu == null || selectedMenu.Length == 0)
            {
                roleToUpdate.RoleMenus = new List<RoleMenu>();
                return;
            }

            var selectedMenuHs = new HashSet<string>(selectedMenu);
            var roleMenus = new HashSet<string>(roleToUpdate.RoleMenus.Select(m => m.MenuId));

            var menus = _context.Menus.Where(m => m.IsDeleted == 0 && m.Type == 0 && string.IsNullOrEmpty(m.ParentId));

            foreach (var menu in menus)
            {
                if (selectedMenuHs.Contains(menu.Id))
                {
                    if (!roleMenus.Contains(menu.Id))
                    {
                        RoleMenu temp = new RoleMenu();
                        temp.RoleId = roleToUpdate.Id;
                        temp.MenuId = menu.Id;
                        roleToUpdate.RoleMenus.Add(temp);
                    }
                }
                else
                {
                    if (roleMenus.Contains(menu.Id))
                    {
                        var temp = roleToUpdate.RoleMenus.Where(m => m.MenuId == menu.Id).FirstOrDefault();
                        roleToUpdate.RoleMenus.Remove(temp);
                    }
                }
            }
        }
    }
}
