namespace StoreDataAccessLayer.Entities
{
    public class Company
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public byte RoleId { get; set; }
        
        public Roles Role { get; set; } = null!;
    }
}

