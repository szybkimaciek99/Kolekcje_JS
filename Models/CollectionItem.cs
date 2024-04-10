namespace CollectionManagmentSystem.Models
{
    public class CollectionItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Rating { get; set; }
        public string Status { get; set; }
        public int CollectionId { get; set; }
    }
}
