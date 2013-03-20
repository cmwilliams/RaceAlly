using System.Collections.Generic;

namespace RaceAlly.Models
{
    public class RaceCategory
    {
        public int RaceCategoryId { get; set; }
        public string Name { get; set; }
        public bool? Active { get; set; }
        public virtual Race Race { get; set; }
        public virtual ICollection<RaceProduct> RaceProducts { get; set; }
    }
}
