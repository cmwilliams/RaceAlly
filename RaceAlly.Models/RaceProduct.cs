namespace RaceAlly.Models
{
    public class RaceProduct
    {
        public int RaceProductId { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public bool? Active { get; set; }
        public virtual RaceCategory RaceCategory { get; set; }
    }
}
