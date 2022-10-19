using RBAC_CoreMVC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RBAC_CoreMVC.DTOs
{
    public class UserDto
    {
        public User User { get; set; }

        [Display(Name = "邮箱")]
        [DataType(DataType.EmailAddress, ErrorMessage = "请输入正确格式的邮箱地址")]
        public string Email { get; set; }

        [Display(Name = "手机号")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "请输入正确格式的手机号")]
        public string Phone { get; set; }

        [Display(Name = "密码")]
        [DataType(DataType.Password)]
        [StringLength(18, MinimumLength = 6, ErrorMessage = "密码长度需在6-18位之间")]
        public string Password { get; set; }

        [Display(Name = "重复密码")]
        [DataType(DataType.Password)]
        [StringLength(18, MinimumLength = 6, ErrorMessage = "密码长度需在6-18位之间")]
        public string RePassword { get; set; }

    }
}
