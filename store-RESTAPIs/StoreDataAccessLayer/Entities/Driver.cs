namespace StoreDataAccessLayer.Entities
{
    public class Driver
    {
        public int DriverId { get; set; }
        public string DriverName { get; set; } = null!;
        public int UserId { get; set; }
        
        public User User { get; set; } = null!;
    }
}

