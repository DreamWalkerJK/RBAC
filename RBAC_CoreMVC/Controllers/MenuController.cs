using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RBAC_CoreMVC.Data;
using RBAC_CoreMVC.Models;

namespace RBAC_CoreMVC.Controllers
{
    public class MenuController : RBACControllerBase
    {
        private readonly RBACContext _context;

        public MenuController(RBACContext context)
        {
            _context = context;
        }

        // GET: Menu
        public async Task<IActionResult> Index(string menuParent, string menuName, 
            string currentName, int? page)
        {
            if(!string.IsNullOrEmpty(menuParent) || !string.IsNullOrEmpty(menuName))
            {
                page = 1;
            }
            else
            {
                menuName = currentName;
            }

            ViewData["CurrentName"] = menuName;

            var menus = from m in _context.Menus
                        where m.IsDeleted == 0
                        select m;

            if (!string.IsNullOrEmpty(menuParent))
            {
                menus = menus.Where(m => m.ParentId.Equals(menuParent));
            }

            if (!string.IsNullOrEmpty(menuName))
            {
                menus = menus.Where(m => m.Name.Contains(menuName));
            }

            menus = menus.OrderBy(m => m.Code);

            MenuParentDropDownListFirstNull(string.Empty, menuParent);

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(await PagedList<Menu>
                .CreateAsync(menus.AsNoTracking(), pageNumber, pageSize));
        }

        // GET: Menu/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // GET: Menu/Create
        public IActionResult Create()
        {
            MenuParentDropDownListFirstNull(string.Empty);
            MenuTypeDropDownList();
            return View();
        }

        // POST: Menu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,Name,Url,Type,ParentId,Remarks")] Menu menu)
        {
            menu.CreateUserId = SessionHelper.GetSession(HttpContext.Session, "CurrentUserId");

            if (ModelState.IsValid)
            {
                if (MenuCodeExists(menu.Code))
                {
                    ModelState.AddModelError("", "功能编码已存在");
                    MenuParentDropDownListFirstNull(menu.Id);
                    MenuTypeDropDownList(menu.Type);
                    return View(menu);
                }

                _context.Add(menu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            MenuParentDropDownListFirstNull(menu.Id);
            MenuTypeDropDownList(menu.Type);
            return View(menu);
        }

        // GET: Menu/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound();
            }
            MenuParentDropDownListFirstNull(menu.Id);
            MenuTypeDropDownList(menu.Type);
            return View(menu);
        }

        // POST: Menu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Code,Name,Url,Type,ParentId,Remarks")] Menu menu)
        {
            if (id != menu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    menu.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    _context.Update(menu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuExists(menu.Id))
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
            MenuParentDropDownListFirstNull(menu.Id);
            MenuTypeDropDownList(menu.Type);
            return View(menu);
        }

        // GET: Menu/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // POST: Menu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var menu = await _context.Menus.FindAsync(id);
            //_context.Menus.Remove(menu);
            menu.IsDeleted = 1;
            menu.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _context.Update(menu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRange(string[] selectedMenu)
        {
            List<Menu> menus = new List<Menu>();
            foreach(var item in selectedMenu)
            {
                var menu = await _context.Menus.FindAsync(item);
                menu.IsDeleted = 1;
                menu.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                menus.Add(menu);
            }
            _context.UpdateRange(menus);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MenuExists(string id)
        {
            return _context.Menus.Any(e => e.Id == id);
        }

        /// <summary>
        /// 检查Menu编码是否存在
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        private bool MenuCodeExists(string Code)
        {
            return _context.Menus.Any(e => e.Code == Code);
        }

        /// <summary>
        /// 上级功能的下拉列表，首行值为null
        /// </summary>
        /// <param name="id"></param>
        /// <param name="selectedParent"></param>
        protected void MenuParentDropDownListFirstNull(string id, object selectedParent = null)
        {
            List<SelectListItem> list = _context.Menus.Where(m => m.Id != id && m.Type == 0)
                .Select(s => new SelectListItem()
                    {
                        Value = s.Id,
                        Text = s.Name
                    }).ToList();
            list.Insert(0, new SelectListItem { Value = null, Text = "---请选择---" });

            ViewData["ParentId"] = new SelectList(list, "Value", "Text", selectedParent);
        }

        /// <summary>
        /// Menu类型列表
        /// </summary>
        /// <param name="selectedType"></param>
        protected void MenuTypeDropDownList(object selectedType = null)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            SelectListItem[] items = {
                new SelectListItem { Value = 0.ToString(), Text = "导航" },
                new SelectListItem { Value = 1.ToString(), Text = "功能" }
            };

            list.AddRange(items);

            ViewData["MenuType"] = new SelectList(list, "Value", "Text", selectedType);
        }
    }
}
