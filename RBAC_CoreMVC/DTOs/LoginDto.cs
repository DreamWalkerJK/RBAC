using System.ComponentModel.DataAnnotations;

namespace RBAC_CoreMVC.DTOs
{
    public class LoginDto
    {
        [Display(Name = "用户名")]
        [Required(ErrorMessage = "用户名不可为空")]
        public string UserName { get; set; }

        [Display(Name = "密码")]
        [Required(ErrorMessage = "密码不可为空")]
        [DataType(DataType.Password)]
        [StringLength(18, MinimumLength = 6, ErrorMessage = "密码长度需在6-18位之间")]
        public string Password { get; set; }
    }
}
