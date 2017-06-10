using System;
using System.Collections.Generic;
using WebApi.Hal;

using REST_with_HATEOAS.Resources.Parks;
using REST_with_HATEOAS.Resources.AnimalTypes;

namespace REST_with_HATEOAS.Resources.AnimalGenuses
{
    public class AnimalGenusRepresentation : Representation
    {
        public AnimalGenusRepresentation()
        {
            Rel = LinkTemplates.AnimalGenuses.AnimalGenus.Rel;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public List<ParkRepresentation> Parks{ get; set; }

        public List<AnimalTypeRepresentation> AnimalTypes{ get; set; }

        protected override void CreateHypermedia()
        {
            Href = LinkTemplates.AnimalGenuses.AnimalGenus.CreateLink(new { id = Id }).Href;
        }
    }
}