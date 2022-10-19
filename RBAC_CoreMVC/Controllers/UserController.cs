using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RBAC_CoreMVC.Data;
using RBAC_CoreMVC.DTOs;
using RBAC_CoreMVC.Models;

namespace RBAC_CoreMVC.Controllers
{
    public class UserController : RBACControllerBase
    {
        private readonly RBACContext _context;

        public UserController(RBACContext context)
        {
            _context = context;
        }

        // GET: User
        public async Task<IActionResult> Index(string userDepartment,
           string userName, string currentName, int? page)
        {
            if (!string.IsNullOrEmpty(userDepartment) 
                || !string.IsNullOrEmpty(userName))
            {
                page = 1;
            }
            else
            {
                userName = currentName;
            }

            ViewData["CurrentName"] = currentName;

            string userId = SessionHelper.GetSession(HttpContext.Session, "CurrentUserId");
            var users = _context.Users.Where(u => u.IsDeleted == 0);

            if (!string.IsNullOrEmpty(userDepartment))
            {
                users = users.Where(u => u.DepartmentId.Equals(userDepartment));
            }

            if (!string.IsNullOrEmpty(userName))
            {
                users = users.Where(u => u.Name.Contains(userName));
            }

            users = users.Include(d => d.Department).Include(u => u.UserRoles).OrderBy(u => u.Code);

            DepartmentDropDownListFirstNull(userDepartment);

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(await PagedList<User>
                .CreateAsync(users.AsNoTracking(), pageNumber, pageSize));
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            var user = new User();
            user.UserRoles = new List<UserRole>();
            GetRoleList(user);
            DepartmentDropDownList();
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,Name,Email,Phone,DepartmentId,Remarks")] User user, string[] selectedRole)
        {
            user.CreateUserId = SessionHelper.GetSession(HttpContext.Session, "CurrentUserId");

            if (selectedRole != null)
            {
                user.UserRoles = new List<UserRole>();
                foreach (var role in selectedRole)
                {
                    var userRoleToAdd = new UserRole();
                    userRoleToAdd.UserId = user.Id;
                    userRoleToAdd.RoleId = role;
                    user.UserRoles.Add(userRoleToAdd);
                }
            }

            if (ModelState.IsValid)
            {
                if (UserCodeExists(user.Code))
                {
                    ModelState.AddModelError("", "用户编码已存在");
                    GetRoleList(user);
                    DepartmentDropDownList(user.DepartmentId);
                    return View(user);
                }

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            GetRoleList(user);
            DepartmentDropDownList(user.DepartmentId);
            return View(user);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            GetRoleList(user);
            DepartmentDropDownList(user.DepartmentId);
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string[] selectedRole)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var userToUpdate = await _context.Users.Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (await TryUpdateModelAsync(userToUpdate, "",
                u => u.Id,
                u => u.Code,
                u => u.Name,
                u => u.Email,
                u => u.Phone,
                u => u.DepartmentId,
                u => u.Remarks))
            {
                try
                {
                    UpdataUserRoles(selectedRole, userToUpdate);

                    userToUpdate.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    _context.Update(userToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(userToUpdate.Id))
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

            GetRoleList(userToUpdate);
            DepartmentDropDownList(userToUpdate.DepartmentId);
            return View(userToUpdate);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            //_context.Users.Remove(user);
            user.IsDeleted = 0;
            user.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _context.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: User/ResetPassword/5
        public async Task<IActionResult> ResetPassword(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("ResetPassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPasswordConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            user.Password = "123456";
            user.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _context.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: User/ChangeMyUserInfo/5
        public async Task<IActionResult> ChangeMyUserInfo(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            UserDto userDto = new UserDto();
            userDto.User = user;
            return View(userDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeMyUserInfo(string id, [Bind("Email,Phone,Password,RePassword")] UserDto userDto)
        {
            var user = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            userDto.User = user;

            if (ModelState.IsValid)
            {
                if(string.IsNullOrEmpty(userDto.Email)&& string.IsNullOrEmpty(userDto.Phone)
                    && string.IsNullOrEmpty(userDto.Password) && string.IsNullOrEmpty(userDto.RePassword))
                {
                    return View(userDto);
                }

                if (!string.IsNullOrEmpty(userDto.Email))
                {
                    user.Email = userDto.Email;
                }

                if (!string.IsNullOrEmpty(userDto.Phone))
                {
                    user.Phone = userDto.Phone;
                }

                if (!string.IsNullOrEmpty(userDto.Password) || !string.IsNullOrEmpty(userDto.RePassword))
                {
                    if (!userDto.Password.Equals(userDto.RePassword))
                    {
                        ModelState.AddModelError("", "两次密码不一致");
                        return View(userDto);
                    }
                    user.Password = userDto.Password;
                }

                user.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                _context.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return View(userDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRange(string[] selectedUser)
        {
            List<User> users = new List<User>();
            foreach (var item in selectedUser)
            {
                var user = await _context.Users.FindAsync(item);
                user.IsDeleted = 1;
                user.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                users.Add(user);
            }
            _context.UpdateRange(users);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPasswordRange(string[] selectedUser)
        {
            List<User> users = new List<User>();
            foreach (var item in selectedUser)
            {
                var user = await _context.Users.FindAsync(item);
                user.Password = "123456";
                user.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                users.Add(user);
            }
            _context.UpdateRange(users);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private bool UserCodeExists(string Code)
        {
            return _context.Users.Any(e => e.Code == Code);
        }

        protected void DepartmentDropDownList(object selectedDepartment = null)
        {
            var deparmentQuery = _context.Departments.Where(d => d.IsDeleted == 0);

            ViewData["DepartmentId"] = new SelectList(deparmentQuery, "Id", "Name", selectedDepartment);
        }

        protected void DepartmentDropDownListFirstNull(object selectedDepartment = null)
        {
            List<SelectListItem> list = _context.Departments.Where(m => m.IsDeleted == 0)
                .Select(s => new SelectListItem()
                {
                    Value = s.Id,
                    Text = s.Name
                }).ToList();
            list.Insert(0, new SelectListItem { Value = null, Text = "---请选择---" });

            ViewData["DepartmentId"] = new SelectList(list, "Value", "Text", selectedDepartment);
        }

        private void GetRoleList(User user)
        {
            var roles = _context.Roles.Where(m => m.IsDeleted == 0);
            var selectedRole = new HashSet<string>(user.UserRoles.Select(r => r.RoleId));
            var roleList = new List<RoleDto>();

            foreach (var r in roles)
            {
                roleList.Add(new RoleDto
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    IsSelected = selectedRole.Contains(r.Id)
                });
            }

            ViewData["RoleList"] = roleList;
        }

        private void UpdataUserRoles(string[] selectedRole, User userToUpdate)
        {
            if (selectedRole == null || selectedRole.Length == 0)
            {
                userToUpdate.UserRoles = new List<UserRole>();
                return;
            }

            var selectedRoleHs = new HashSet<string>(selectedRole);
            var userRoles = new HashSet<string>(userToUpdate.UserRoles.Select(u => u.RoleId));

            var roles = _context.Roles.Where(m => m.IsDeleted == 0);

            foreach (var role in roles)
            {
                if (selectedRoleHs.Contains(role.Id))
                {
                    if (!userRoles.Contains(role.Id))
                    {
                        UserRole temp = new UserRole();
                        temp.RoleId = role.Id;
                        temp.UserId = userToUpdate.Id;
                        userToUpdate.UserRoles.Add(temp);
                    }
                }
                else
                {
                    if (userRoles.Contains(role.Id))
                    {
                        var temp = userToUpdate.UserRoles.Where(u => u.RoleId == role.Id).FirstOrDefault();
                        userToUpdate.UserRoles.Remove(temp);
                    }
                }
            }
        }
    }
}
