using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RBAC_CoreMVC.Models
{
    /// <summary>
    /// 功能
    /// </summary>
    public class Menu : Entity
    {
        /// <summary>
        /// 地址
        /// </summary>
        [Display(Name = "地址")]
        public string Url { get; set; }

        /// <summary>
        /// 类型 0导航 1功能
        /// </summary>
        [Display(Name = "类型")]
        public int Type { get; set; } = 0;

        /// <summary>
        /// 父级ID
        /// </summary>
        [Display(Name = "上级菜单")]
        public string ParentId { get; set; }

        /// <summary>
        /// 角色合集
        /// </summary>
        public virtual ICollection<RoleMenu> RoleMenus { get; set; }

    }
}
