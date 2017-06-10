using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace REST_with_HATEOAS.Models
{
    class AnimalContext : DbContext
    {
        public AnimalContext() : base("DbConnection") { }

        public DbSet<AnimalGenus> AnimalGenuses { get; set; }
        public DbSet<AnimalType> AnimalTypes { get; set; }
        public DbSet<Park> Parks { get; set; }
        public DbSet<RingStation> RingStations { get; set; }
    }
}