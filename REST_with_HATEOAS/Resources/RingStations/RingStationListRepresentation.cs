using System;
using WebApi.Hal;

namespace REST_with_HATEOAS.Resources.RingStations
{
    public class RingStationListRepresentation : SimpleListRepresentation<RingStationRepresentation>
    {
        public RingStationListRepresentation()
        {
            Rel = LinkTemplates.RingStations.GetRingStations.Rel;
        }
        protected override void CreateHypermedia()
        {
            Href = LinkTemplates.RingStations.GetRingStations.CreateLink().Href;
            Links.Add(LinkTemplates.RingStations.GetRingStations.CreateLink());
        }
    }
}