using System.Collections.Generic;

namespace RBAC_CoreMVC.Models
{
    /// <summary>
    /// 角色
    /// </summary>
    public class Role : Entity
    {
        /// <summary>
        /// 角色集合
        /// </summary>
        public virtual ICollection<UserRole> UserRoles { get; set; }

        /// <summary>
        /// 菜单合集
        /// </summary>
        public virtual ICollection<RoleMenu> RoleMenus { get; set; }
    }
}
