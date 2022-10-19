using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RBAC_CoreMVC.DTOs
{
    public class RoleDto
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
}
