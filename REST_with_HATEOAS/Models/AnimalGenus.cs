using System;
using System.Collections.Generic;

namespace REST_with_HATEOAS.Models
{    
    public class AnimalGenus : IEquatable<AnimalGenus>
    {        
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public virtual List<Park> Parks { get; set; }
        
        public virtual List<AnimalType> AnimalTypes { get; set; }

        public AnimalGenus()
        {
            Parks = new List<Park>();
            AnimalTypes = new List<AnimalType>();
        }

        public bool Equals(AnimalGenus other)
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