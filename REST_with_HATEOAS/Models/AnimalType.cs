using System;
using System.Collections.Generic;

namespace REST_with_HATEOAS.Models
{
    public class AnimalType : IEquatable<AnimalType>
    {        
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public virtual AnimalGenus AnimalGenus { get; set; }
        
        public virtual List<RingStation> RingStations { get; set; }

        public AnimalType()
        {
            RingStations = new List<RingStation>();
        }

        public bool Equals(AnimalType other)
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