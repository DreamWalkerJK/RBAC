using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RBAC_CoreMVC.Models
{
    /// <summary>
    /// 部门实体
    /// </summary>
    public class Department : Entity
    {
        /// <summary>
        /// 部门负责人
        /// </summary>
        [Display(Name = "部门负责人")]
        public string ManagerId { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [Display(Name = "联系电话")]
        public string ContactNumber { get; set; }

        /// <summary>
        /// 父级部门
        /// </summary>
        [Display(Name = "上级部门")]
        public string ParentId { get; set; }


        /// <summary>
        /// 用户合集
        /// </summary>
        public virtual ICollection<User> Users { get; set; }
    }
}
