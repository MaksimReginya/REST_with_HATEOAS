using System;
using System.Collections.Generic;
using WebApi.Hal;

using REST_with_HATEOAS.Resources.AnimalGenuses;
using REST_with_HATEOAS.Resources.RingStations;

namespace REST_with_HATEOAS.Resources.Parks
{    
    public class ParkRepresentation : Representation
    {
        public ParkRepresentation()
        {
            Rel = LinkTemplates.Parks.Park.Rel;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public List<AnimalGenusRepresentation> AnimalGenuses { get; set; }

        public List<RingStationRepresentation> RingStations { get; set; }

        protected override void CreateHypermedia()
        {
            Href = LinkTemplates.Parks.Park.CreateLink(new { id = Id }).Href;
        }
    }
}