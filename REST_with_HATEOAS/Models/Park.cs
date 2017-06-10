using System;
using System.Collections.Generic;

namespace REST_with_HATEOAS.Models
{    
    public class Park : IEquatable<Park>
    {        
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public virtual List<AnimalGenus> AnimalGenuses { get; set; }
        
        public virtual List<RingStation> RingStations { get; set; }

        public Park()
        {
            AnimalGenuses = new List<AnimalGenus>();
            RingStations = new List<RingStation>();
        }

        public bool Equals(Park other)
        {
            if (this.Name.Equals(other.Name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}