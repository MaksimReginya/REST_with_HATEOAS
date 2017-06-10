using System;
using System.Collections.Generic;

namespace REST_with_HATEOAS.Models
{    
    public class RingStation : IEquatable<RingStation>
    {        
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public virtual Park Park { get; set; }
        
        public virtual List<AnimalType> AnimalTypes { get; set; }

        public RingStation()
        {
            AnimalTypes = new List<AnimalType>();
        }

        public bool Equals(RingStation other)
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