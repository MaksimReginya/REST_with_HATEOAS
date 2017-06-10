using System;
using WebApi.Hal;

namespace REST_with_HATEOAS.Resources.AnimalTypes
{
    public class AnimalTypeListRepresentation : SimpleListRepresentation<AnimalTypeRepresentation>
    {
        public AnimalTypeListRepresentation()
        {
            Rel = LinkTemplates.AnimalTypes.GetAnimalTypes.Rel;
        }
        protected override void CreateHypermedia()
        {
            Href = LinkTemplates.AnimalTypes.GetAnimalTypes.CreateLink().Href;
            Links.Add(LinkTemplates.AnimalTypes.GetAnimalTypes.CreateLink());
        }
    }
}