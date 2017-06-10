using System;
using WebApi.Hal;

namespace REST_with_HATEOAS.Resources.AnimalGenuses
{
    public class AnimalGenusListRepresentation : SimpleListRepresentation<AnimalGenusRepresentation>
    {
        public AnimalGenusListRepresentation()
        {
            Rel = LinkTemplates.AnimalGenuses.GetAnimalGenuses.Rel;
        }
        protected override void CreateHypermedia()
        {
            Href = LinkTemplates.AnimalGenuses.GetAnimalGenuses.CreateLink().Href;
            Links.Add(LinkTemplates.AnimalGenuses.GetAnimalGenuses.CreateLink());
        }
    }
}