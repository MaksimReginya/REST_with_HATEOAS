using System;
using System.Collections.Generic;
using WebApi.Hal;

using REST_with_HATEOAS.Resources.AnimalGenuses;
using REST_with_HATEOAS.Resources.RingStations;

namespace REST_with_HATEOAS.Resources.AnimalTypes
{    
    public class AnimalTypeRepresentation : Representation
    {
        public AnimalTypeRepresentation()
        {
            Rel = LinkTemplates.AnimalTypes.AnimalType.Rel;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public AnimalGenusRepresentation AnimalGenus{ get; set; }

        public List<RingStationRepresentation> RingStations{ get; set; }

        protected override void CreateHypermedia()
        {
            Href = LinkTemplates.AnimalTypes.AnimalType.CreateLink(new { id = Id }).Href;
        }
    }
}