using System;
using System.ComponentModel.DataAnnotations;

namespace RBAC_CoreMVC.Models
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Required]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 编码
        /// </summary>
        [Required(ErrorMessage = "编码不可为空")]
        [Display(Name = "编码")]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "编码只允许大小写字母和数字")]
        [StringLength(15, ErrorMessage = "编码不得超过15个字符")]
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "名称不可为空")]
        [Display(Name = "名称")]
        [StringLength(30, ErrorMessage = "名称不得超过30个字符")]
        public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        [StringLength(60, ErrorMessage = "备注不得超过60个字符")]
        public string Remarks { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Display(Name = "创建人")]
        public string CreateUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public string CreateTime { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// 更新时间
        /// </summary>
        [Display(Name = "更新时间")]
        public string UpdateTime { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [Display(Name = "是否删除")]
        public int IsDeleted { get; set; } = 0;
    }
}
