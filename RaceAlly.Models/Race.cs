using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RaceAlly.Models
{
    public class Race
    {
        public int RaceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? RegistrationOpenDate { get; set; }
        public DateTime? RegistrationCloseDate { get; set; }
        public string Address { get; set; }
        public string AddressCont { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public string Website { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<RaceCategory> RaceCategories { get; set; }
    }
}
