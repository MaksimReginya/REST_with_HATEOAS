using System;
using System.Collections.Generic;
using WebApi.Hal;

using REST_with_HATEOAS.Resources.Parks;
using REST_with_HATEOAS.Resources.AnimalTypes;

namespace REST_with_HATEOAS.Resources.RingStations
{
    public class RingStationRepresentation : Representation
    {
        public RingStationRepresentation()
        {
            Rel = LinkTemplates.RingStations.RingStation.Rel;

        }
        public int Id { get; set; }

        public string Name { get; set; }

        public ParkRepresentation Park { get; set; }

        public List<AnimalTypeRepresentation> AnimalTypes{ get; set; }

        protected override void CreateHypermedia()
        {
            Href = LinkTemplates.RingStations.RingStation.CreateLink(new { id = Id }).Href;
        }
    }
}