namespace RBAC_CoreMVC.DTOs
{
    public class MenuDto
    {
        public string MenuId { get; set; }

        public string MenuName { get; set; }

        public string MenuUrl { get; set; }

        public string MenuUrlFull { get; set; }

        public bool IsSelected { get; set; }
    }
}
