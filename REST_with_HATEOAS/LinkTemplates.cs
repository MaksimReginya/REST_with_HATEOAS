using System;
using WebApi.Hal;

namespace REST_with_HATEOAS
{
    public class LinkTemplates
    {
        public static Link Entities { get { return new Link("Entities", "~/Entities"); } }

        public static class AnimalTypes
        {
            public static Link AnimalType { get { return new Link("AnimalType", "~/AnimalTypes/{id}"); } }
            public static Link CreateAnimalType { get { return new Link("CreateAnimalType", "~/AnimalTypes"); } }
            public static Link GetAnimalTypes { get { return new Link("AnimalType", "~/AnimalTypes"); } }
            public static Link UpdateAnimalType { get { return new Link("UpdateAnimalType", "~/AnimalTypes/{id}"); } }
            public static Link DeleteAnimalType { get { return new Link("DeleteAnimalType", "~/AnimalTypes/{id}"); } }
        }
        public static class AnimalGenuses
        {
            public static Link AnimalGenus { get { return new Link("AnimalGenus", "~/AnimalGenuses/{id}"); } }
            public static Link CreateAnimalGenus { get { return new Link("CreateAnimalGenus", "~/AnimalGenuses"); } }
            public static Link GetAnimalGenuses { get { return new Link("AnimalGenuses", "~/AnimalGenuses"); } }
            public static Link UpdateAnimalGenus { get { return new Link("UpdateAnimalGenus", "~/AnimalGenuses/{id}"); } }
            public static Link DeleteAnimalGenus { get { return new Link("DeleteAnimalGenus", "~/AnimalGenuses/{id}"); } }
        }
        public static class Parks
        {
            public static Link Park { get { return new Link("Park", "~/Parks/{id}"); } }
            public static Link CreatePark { get { return new Link("CreatePark", "~/Parks"); } }
            public static Link GetParks { get { return new Link("Parks", "~/Parks"); } }
            public static Link UpdatePark { get { return new Link("UpdatePark", "~/Parks/{id}"); } }
            public static Link DeletePark { get { return new Link("DeletePark", "~/Parks/{id}"); } }
        }
        public static class RingStations
        {
            public static Link RingStation { get { return new Link("RingStation", "~/RingStations/{id}"); } }
            public static Link CreateRingStation { get { return new Link("CreateRingStation", "~/RingStations"); } }
            public static Link GetRingStations { get { return new Link("RingStations", "~/RingStations"); } }
            public static Link UpdateRingStation { get { return new Link("UpdateRingStation", "~/RingStations/{id}"); } }
            public static Link DeleteRingStation { get { return new Link("DeleteRingStation", "~/RingStations/{id}"); } }
        }
    }
}