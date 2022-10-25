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
    public class DepartmentController : RBACControllerBase
    {
        private readonly RBACContext _context;

        public DepartmentController(RBACContext context)
        {
            _context = context;
        }

        // GET: Department
        public async Task<IActionResult> Index(string departmentName,
            string currentName, int? page)
        {
            if (!string.IsNullOrEmpty(departmentName))
            {
                page = 1;
            }
            else
            {
                departmentName = currentName;
            }

            ViewData["CurrentName"] = currentName;

            var departments = _context.Departments.Where(u => u.IsDeleted == 0);

            if (!string.IsNullOrEmpty(departmentName))
            {
                departments = departments.Where(d => d.Name.Contains(departmentName));
            }

            departments = departments.OrderBy(d => d.Code);

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(await PagedList<Department>
                .CreateAsync(departments.AsNoTracking(), pageNumber, pageSize));
        }

        // GET: Department/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // GET: Department/Create
        public IActionResult Create()
        {
            DepartmentsParentDropDownListFirstNull(string.Empty);
            DepartmentsManagerDropDownListFirstNull();
            return View();
        }

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,Name,ManagerId,ContactNumber,ParentId,Remarks")] Department department)
        {
            department.CreateUserId = SessionHelper.GetSession(HttpContext.Session, "CurrentUserId");

            if (ModelState.IsValid)
            {
                if (DepartmentCodeExists(department.Code))
                {
                    ModelState.AddModelError("", "部门编码已存在");
                    DepartmentsParentDropDownListFirstNull(department.Id);
                    DepartmentsManagerDropDownListFirstNull();
                    return View(department);
                }

                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            DepartmentsParentDropDownListFirstNull(department.Id);
            DepartmentsManagerDropDownListFirstNull();
            return View(department);
        }

        // GET: Department/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            DepartmentsParentDropDownListFirstNull(department.Id);
            DepartmentsManagerDropDownListFirstNull();
            return View(department);
        }

        // POST: Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Code,Name,ManagerId,ContactNumber,ParentId,Remarks")] Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    department.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.Id))
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

            DepartmentsParentDropDownListFirstNull(department.Id);
            DepartmentsManagerDropDownListFirstNull();
            return View(department);
        }

        // GET: Department/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var department = await _context.Departments.FindAsync(id);
            //_context.Departments.Remove(department);
            department.IsDeleted = 1;
            department.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _context.Update(department);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(string id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }

        /// <summary>
        /// 查询部门编码是否存在
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        private bool DepartmentCodeExists(string Code)
        {
            return _context.Departments.Any(e => e.Code == Code);
        }

        /// <summary>
        /// 上级部门下拉列表，首行值为null
        /// </summary>
        /// <param name="id"></param>
        /// <param name="selectedParent"></param>
        protected void DepartmentsParentDropDownListFirstNull(string id, object selectedParent = null)
        {
            List<SelectListItem> list = _context.Departments.Where(m => m.Id != id && m.IsDeleted == 0)
                .Select(s => new SelectListItem()
                {
                    Value = s.Id,
                    Text = s.Name
                }).ToList();
            list.Insert(0, new SelectListItem { Value = null, Text = "---请选择---" });

            ViewData["ParentId"] = new SelectList(list, "Value", "Text", selectedParent);
        }

        /// <summary>
        /// 部门管理人下拉列表，首行值为null
        /// </summary>
        /// <param name="selectedUser"></param>
        protected void DepartmentsManagerDropDownListFirstNull(object selectedUser = null)
        {
            List<SelectListItem> list = _context.Users.Where(u => u.IsDeleted == 0)
                .Select(s => new SelectListItem()
                {
                    Value = s.Id,
                    Text = s.Name
                }).ToList();
            list.Insert(0, new SelectListItem { Value = null, Text = "---请选择---" });

            ViewData["ManagerId"] = new SelectList(list, "Value", "Text", selectedUser);
        }
    }
}
