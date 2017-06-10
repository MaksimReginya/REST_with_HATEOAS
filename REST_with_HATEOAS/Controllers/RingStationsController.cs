using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

using REST_with_HATEOAS.Models;
using REST_with_HATEOAS.Resources.RingStations;
using REST_with_HATEOAS.Resources.Parks;
using REST_with_HATEOAS.Resources.AnimalTypes;

namespace REST_with_HATEOAS.Controllers
{
    public class RingStationsController : ApiController
    {
        private AnimalContext db = new AnimalContext();

        [NonAction]
        private ParkRepresentation GetPark(Park park)
        {
            var representation = new ParkRepresentation()
            {
                Id = park.Id,
                Name = park.Name
            };
            representation.Links.Add(LinkTemplates.Parks.DeletePark.CreateLink(new { id = park.Id }));
            representation.Links.Add(LinkTemplates.Parks.UpdatePark.CreateLink(new { id = park.Id }));
            return representation;
        }

        [NonAction]
        private AnimalTypeRepresentation GetAnimalType(AnimalType animalType)
        {
            var representation = new AnimalTypeRepresentation()
            {
                Id = animalType.Id,
                Name = animalType.Name
            };
            representation.Links.Add(LinkTemplates.AnimalTypes.DeleteAnimalType.CreateLink(new { id = animalType.Id }));
            representation.Links.Add(LinkTemplates.AnimalTypes.UpdateAnimalType.CreateLink(new { id = animalType.Id }));
            return representation;
        }

        [NonAction]
        private RingStationRepresentation GetRingStation(RingStation ringStation)
        {
            var representation = new RingStationRepresentation()
            {
                Id = ringStation.Id,
                Name = ringStation.Name
            };
            representation.Links.Add(LinkTemplates.RingStations.DeleteRingStation.CreateLink(new { id = ringStation.Id }));
            representation.Links.Add(LinkTemplates.RingStations.UpdateRingStation.CreateLink(new { id = ringStation.Id }));
            representation.Park = ringStation.Park == null ? null : GetPark(ringStation.Park);
            representation.AnimalTypes = ringStation.AnimalTypes.Select(animalType => GetAnimalType(animalType)).ToList();
            return representation;
        }

        // GET: /RingStations
        public RingStationListRepresentation GetRingStations()
        {
            var response = new RingStationListRepresentation();

            var ringStationsInDatabase = db.RingStations.ToList();

            foreach (var ringStation in ringStationsInDatabase)
            {
                response.ResourceList.Add(GetRingStation(ringStation));
            }

            return response;
        }

        // GET: /RingStations/5
        [ResponseType(typeof(RingStationRepresentation))]
        public IHttpActionResult GetRingStation(int id)
        {
            var ringStation = db.RingStations.Find(id);
            if (ringStation != null)
                return Ok(GetRingStation(ringStation));
            else
                return NotFound();
        }

        // PUT: /RingStations/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRingStation(int id, RingStationRepresentation ringStation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (dbPutRingStation(id, ringStation))
                return StatusCode(HttpStatusCode.NoContent);
            else
                return NotFound();            
        }

        // POST: /RingStations
        [ResponseType(typeof(RingStationRepresentation))]
        public IHttpActionResult PostRingStation(RingStationRepresentation ringStation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ringStationInDatabase = dbPostRingStation(ringStation);
            if (ringStationInDatabase != null)
                return CreatedAtRoute("DefaultApi", new { id = ringStationInDatabase.Id }, GetRingStation(ringStationInDatabase));
            else
                return BadRequest();            
        }

        // DELETE: /RingStations/5
        [ResponseType(typeof(RingStationRepresentation))]
        public IHttpActionResult DeleteRingStation(int id)
        {
            var ringStation = dbDeleteRingStation(id);
            if (ringStation != null)
                return Ok(GetRingStation(ringStation));
            else
                return NotFound();
        }
        
        private bool dbPutRingStation(int id, RingStationRepresentation ringStation)
        {
            var ringStationInDatabase = db.RingStations.Find(id);
            if (ringStationInDatabase == null)
            {
                return false;
            }

            foreach (var animalType in ringStationInDatabase.AnimalTypes)
            {
                animalType.RingStations.Remove(ringStationInDatabase);
            }

            ringStationInDatabase.AnimalTypes = new List<AnimalType>();

            foreach (var animalType in ringStation.AnimalTypes)
            {
                var animalTypeInDatabase = db.AnimalTypes.FirstOrDefault(elem => elem.Name == animalType.Name);
                if (animalTypeInDatabase == null)
                {
                    animalTypeInDatabase = new AnimalType() { Name = animalType.Name };
                }

                ringStationInDatabase.AnimalTypes.Add(animalTypeInDatabase);
                animalTypeInDatabase.RingStations.Add(ringStationInDatabase);
            }

            if (ringStationInDatabase.Park != null)
            {
                ringStationInDatabase.Park.RingStations.Remove(ringStationInDatabase);
                ringStationInDatabase.Park = null;
            }

            if (ringStation.Park != null)
            {
                var parkInDb = db.Parks.FirstOrDefault(elem => elem.Name == ringStation.Park.Name);
                if (parkInDb == null)
                {
                    parkInDb = new Park() { Name = ringStation.Park.Name };
                }
                ringStationInDatabase.Park = parkInDb;
                parkInDb.RingStations.Add(ringStationInDatabase);
            }

            ringStationInDatabase.Name = ringStation.Name;
                        
            db.SaveChanges();

            return true;
        }

        private RingStation dbPostRingStation(RingStationRepresentation ringStation)
        {
            var ringStationInDatabase = db.RingStations.FirstOrDefault(elem => elem.Name == ringStation.Name);
            if (ringStationInDatabase != null)
            {
                return null;
            }
            ringStationInDatabase = new RingStation() { Name = ringStation.Name };
            db.RingStations.Add(ringStationInDatabase);

            foreach (var animalType in ringStation.AnimalTypes)
            {
                var animalTypeInDatabase = db.AnimalTypes.FirstOrDefault(elem => elem.Name == animalType.Name);
                if (animalTypeInDatabase == null)
                {
                    animalTypeInDatabase = new AnimalType() { Name = animalType.Name };
                }
                animalTypeInDatabase.RingStations.Add(ringStationInDatabase);
                ringStationInDatabase.AnimalTypes.Add(animalTypeInDatabase);
            }

            if (ringStation.Park != null)
            {
                var parkInDb = db.Parks.FirstOrDefault(elem => elem.Name == ringStation.Park.Name);
                if (parkInDb == null)
                {
                    parkInDb = new Park() { Name = ringStation.Park.Name };
                }
                ringStationInDatabase.Park = parkInDb;
                parkInDb.RingStations.Add(ringStationInDatabase);
            }

            db.SaveChanges();
            
            return ringStationInDatabase;
        }

        private RingStation dbDeleteRingStation(int id)
        {
            RingStation ringStation = db.RingStations.Find(id);
            if (ringStation == null)
            {
                return null;
            }

            foreach (var animalType in ringStation.AnimalTypes)
            {
                animalType.RingStations.Remove(ringStation);
            }

            if (ringStation.Park != null)
            {
                ringStation.Park.RingStations.Remove(ringStation);
            }

            db.RingStations.Remove(ringStation);
            db.SaveChanges();

            return ringStation;
        }
    }
}