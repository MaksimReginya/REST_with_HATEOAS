using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

using REST_with_HATEOAS.Models;
using REST_with_HATEOAS.Resources.Parks;
using REST_with_HATEOAS.Resources.AnimalGenuses;
using REST_with_HATEOAS.Resources.RingStations;

namespace REST_with_HATEOAS.Controllers
{
    public class ParksController : ApiController
    {
        private AnimalContext db = new AnimalContext();

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
            return representation;
        }

        [NonAction]
        private AnimalGenusRepresentation GetAnimalGenus(AnimalGenus animalGenus)
        {
            var representation = new AnimalGenusRepresentation()
            {
                Id = animalGenus.Id,
                Name = animalGenus.Name
            };
            representation.Links.Add(LinkTemplates.AnimalGenuses.DeleteAnimalGenus.CreateLink(new { id = animalGenus.Id }));
            representation.Links.Add(LinkTemplates.AnimalGenuses.UpdateAnimalGenus.CreateLink(new { id = animalGenus.Id }));
            return representation;
        }

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
            representation.RingStations = park.RingStations.Select(station => GetRingStation(station)).ToList();
            representation.AnimalGenuses = park.AnimalGenuses.Select(genus => GetAnimalGenus(genus)).ToList();
            return representation;
        }

        // GET: /Parks
        public ParkListRepresentation GetParks()
        {
            var response = new ParkListRepresentation();

            var parksInDatabase = db.Parks.ToList();

            foreach (var park in parksInDatabase)
            {
                response.ResourceList.Add(GetPark(park));
            }
            return response;
        }

        // GET: /Parks/5        
        public ParkRepresentation GetPark(int id)
        {
            Park park = db.Parks.Find(id);

            if (park == null)
            {
                return null;
            }

            return GetPark(park);
        }

        // PUT: /Parks/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPark(int id, ParkRepresentation park)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
                        
            if (dbPutPark(id, park))
                return StatusCode(HttpStatusCode.NoContent);
            else
                return NotFound();            
        }

        // POST: /Parks
        [ResponseType(typeof(ParkRepresentation))]
        public IHttpActionResult PostPark(ParkRepresentation park)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var parkInDb = dbPostPark(park);
            if (parkInDb != null)
                return CreatedAtRoute("DefaultApi", new { id = parkInDb.Id }, GetPark(parkInDb));
            else
                return BadRequest();            
        }

        // DELETE: /Parks/5
        [ResponseType(typeof(ParkRepresentation))]
        public IHttpActionResult DeletePark(int id)
        {
            var park = dbDeletePark(id);
            if (park != null)
                return Ok(GetPark(park));
            else
                return NotFound();            
        }
       
        private bool dbPutPark(int id, ParkRepresentation park)
        {
            var parkInDb = db.Parks.Find(id);
            if (parkInDb == null)
            {
                return false;
            }

            foreach (var animalGenus in parkInDb.AnimalGenuses)
            {
                animalGenus.Parks.Remove(parkInDb);
            }

            parkInDb.AnimalGenuses = new List<AnimalGenus>();

            foreach (var animalGenus in park.AnimalGenuses)
            {
                var animalGenusInDatabase = db.AnimalGenuses.FirstOrDefault(elem => elem.Name == animalGenus.Name);
                if (animalGenusInDatabase == null)
                {
                    animalGenusInDatabase = new AnimalGenus() { Name = animalGenus.Name };
                }

                animalGenusInDatabase.Parks.Add(parkInDb);
                parkInDb.AnimalGenuses.Add(animalGenusInDatabase);
            }

            foreach (var ringStation in parkInDb.RingStations)
            {
                ringStation.Park = null;
            }

            parkInDb.RingStations = new List<RingStation>();

            foreach (var ringStation in park.RingStations)
            {
                var ringStationInDatabase = db.RingStations.FirstOrDefault(elem => elem.Name == ringStation.Name);
                if (ringStationInDatabase == null || (ringStationInDatabase != null && ringStationInDatabase.Park != null && !ringStationInDatabase.Park.Equals(parkInDb)))
                {
                    ringStationInDatabase = new RingStation() { Name = ringStation.Name };
                }

                ringStationInDatabase.Park = parkInDb;
                parkInDb.RingStations.Add(ringStationInDatabase);
            }

            parkInDb.Name = park.Name;

            db.SaveChanges();

            return true;
        }

        private Park dbPostPark(ParkRepresentation park)
        {
            if (db.Parks.FirstOrDefault(elem => elem.Name == park.Name) != null)
            {
                return null;
            }

            var parkInDb = new Park() { Name = park.Name };
            db.Parks.Add(parkInDb);

            foreach (var animalGenus in park.AnimalGenuses)
            {
                var animalGenusInDb = db.AnimalGenuses.FirstOrDefault(elem => elem.Name == animalGenus.Name);
                if (animalGenusInDb == null)
                {
                    animalGenusInDb = new AnimalGenus() { Name = animalGenus.Name };
                }
                parkInDb.AnimalGenuses.Add(animalGenusInDb);
                animalGenusInDb.Parks.Add(parkInDb);
            }

            foreach (var ringStation in park.RingStations)
            {
                var newStation = new RingStation { Name = ringStation.Name };
                db.RingStations.Add(newStation);
                newStation.Park = parkInDb;
                parkInDb.RingStations.Add(newStation);
            }

            db.SaveChanges();            
            return parkInDb;
        }

        private Park dbDeletePark(int id)
        {
            Park park = db.Parks.Find(id);
            if (park == null)
            {
                return null;
            }

            foreach (var animalGenus in park.AnimalGenuses)
            {
                animalGenus.Parks.Remove(park);
            }

            foreach (var ringStation in park.RingStations)
            {
                ringStation.Park = null;
            }

            db.Parks.Remove(park);
            db.SaveChanges();

            return park;
        }
    }
}