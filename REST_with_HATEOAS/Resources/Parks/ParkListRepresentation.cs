using System;
using WebApi.Hal;

namespace REST_with_HATEOAS.Resources.Parks
{
    public class ParkListRepresentation : SimpleListRepresentation<ParkRepresentation>
    {
        public ParkListRepresentation()
        {
            Rel = LinkTemplates.Parks.GetParks.Rel;
        }
        protected override void CreateHypermedia()
        {
            Href = LinkTemplates.Parks.GetParks.CreateLink().Href;
            Links.Add(LinkTemplates.Parks.GetParks.CreateLink());
        }
    }
}