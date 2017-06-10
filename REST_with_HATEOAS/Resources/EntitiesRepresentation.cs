using System;
using WebApi.Hal;

namespace REST_with_HATEOAS.Resources
{
    public class EntitiesRepresentation : Representation
    {
        public EntitiesRepresentation()
        {
            Rel = LinkTemplates.Entities.Rel;
        }
        protected override void CreateHypermedia()
        {
            Href = LinkTemplates.Entities.CreateLink().Href;

            var curiesAnimalType = new CuriesLink("animalTypes", "doc/animalTypes/{#rel}");

            Links.Add(curiesAnimalType.CreateLink("create", LinkTemplates.AnimalTypes.CreateAnimalType.Href));
            Links.Add(curiesAnimalType.CreateLink("update", LinkTemplates.AnimalTypes.UpdateAnimalType.Href));
            Links.Add(curiesAnimalType.CreateLink("retreive", LinkTemplates.AnimalTypes.AnimalType.Href));
            Links.Add(curiesAnimalType.CreateLink("delete", LinkTemplates.AnimalTypes.DeleteAnimalType.Href));
            Links.Add(curiesAnimalType.CreateLink("list", LinkTemplates.AnimalTypes.GetAnimalTypes.Href));


            var curiesAnimalGenus = new CuriesLink("animalGenuses", "/doc/animalGenuses/{#rel}");

            Links.Add(curiesAnimalGenus.CreateLink("create", LinkTemplates.AnimalGenuses.CreateAnimalGenus.Href));
            Links.Add(curiesAnimalGenus.CreateLink("update", LinkTemplates.AnimalGenuses.UpdateAnimalGenus.Href));
            Links.Add(curiesAnimalGenus.CreateLink("retreive", LinkTemplates.AnimalGenuses.AnimalGenus.Href));
            Links.Add(curiesAnimalGenus.CreateLink("delete", LinkTemplates.AnimalGenuses.DeleteAnimalGenus.Href));
            Links.Add(curiesAnimalGenus.CreateLink("list", LinkTemplates.AnimalGenuses.GetAnimalGenuses.Href));

            var curiesPark = new CuriesLink("parks", "/doc/parks/{#rel}");

            Links.Add(curiesPark.CreateLink("create", LinkTemplates.Parks.CreatePark.Href));
            Links.Add(curiesPark.CreateLink("update", LinkTemplates.Parks.UpdatePark.Href));
            Links.Add(curiesPark.CreateLink("retreive", LinkTemplates.Parks.Park.Href));
            Links.Add(curiesPark.CreateLink("delete", LinkTemplates.Parks.DeletePark.Href));
            Links.Add(curiesPark.CreateLink("list", LinkTemplates.Parks.GetParks.Href));

            var curiesRingStation = new CuriesLink("ringStations", "/doc/ringStations/{#rel}");

            Links.Add(curiesRingStation.CreateLink("create", LinkTemplates.RingStations.CreateRingStation.Href));
            Links.Add(curiesRingStation.CreateLink("update", LinkTemplates.RingStations.UpdateRingStation.Href));
            Links.Add(curiesRingStation.CreateLink("retreive", LinkTemplates.RingStations.RingStation.Href));
            Links.Add(curiesRingStation.CreateLink("delete", LinkTemplates.RingStations.DeleteRingStation.Href));
            Links.Add(curiesRingStation.CreateLink("list", LinkTemplates.RingStations.GetRingStations.Href));
        }
    }
}