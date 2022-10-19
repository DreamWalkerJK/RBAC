using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RBAC_CoreMVC.Models
{
    /// <summary>
    /// 用户
    /// </summary>
    public class User : Entity
    {
        /// <summary>
        /// 密码
        /// </summary>
        [Display(Name = "密码")]
        [Required(ErrorMessage = "密码不可为空")]
        [DataType(DataType.Password)]
        [StringLength(18, MinimumLength = 6, ErrorMessage = "密码长度需在6-18位之间")]
        public string Password { get; set; } = "123456";

        /// <summary>
        /// 邮箱
        /// </summary>
        [Display(Name = "邮箱")]
        [DataType(DataType.EmailAddress, ErrorMessage = "请输入正确格式的邮箱地址")]
        public string Email { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [Display(Name = "手机号")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "请输入正确格式的手机号")]
        public string Phone { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        [Display(Name = "所属部门")]
        public string DepartmentId { get; set; }

        /// <summary>
        /// 上次登录时间
        /// </summary>
        [Display(Name = "上次登录时间")]
        public string LastLoginTime { get; set; }


        /// <summary>
        /// 所属部门
        /// </summary>
        public virtual Department Department { get; set; }

        /// <summary>
        /// 角色集合
        /// </summary>
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
