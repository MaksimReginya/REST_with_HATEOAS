using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

using REST_with_HATEOAS.Models;

namespace REST_with_HATEOAS.Controllers
{
    public class RingStationsController : ApiController
    {
        private AnimalContext db = new AnimalContext();

        // GET: /RingStations
        public IEnumerable<RingStation> GetRingStations()
        {
            var ringStationsInDatabase = db.RingStations.ToList();
            List<RingStation> ringStations = new List<RingStation>();
            foreach (var ringStation in ringStationsInDatabase)
            {
                List<AnimalType> animalTypes = new List<AnimalType>();
                foreach (var animalType in ringStation.AnimalTypes)
                {
                    animalTypes.Add(new AnimalType() { Name = animalType.Name, Id = animalType.Id });
                }
                var park = ringStation.Park == null ? null : new Park() { Name = ringStation.Park.Name, Id = ringStation.Park.Id };
                ringStations.Add(new RingStation() { Name = ringStation.Name, Id = ringStation.Id, AnimalTypes = animalTypes, Park = park });
            }
            return ringStations;
        }

        // GET: /RingStations/5
        [ResponseType(typeof(RingStation))]
        public IHttpActionResult GetRingStation(int id)
        {
            var ringStation = dbGetRingStation(id);
            if (ringStation != null)
                return Ok(ringStation);
            else
                return NotFound();
        }

        // PUT: /RingStations/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRingStation(int id, [FromBody]RingStation ringStation)
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
        [ResponseType(typeof(RingStation))]
        public IHttpActionResult PostRingStation([FromBody]RingStation ringStation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ringStationInDatabase = dbPostRingStation(ringStation);
            if (ringStationInDatabase != null)
                return CreatedAtRoute("DefaultApi", new { id = ringStationInDatabase.Id }, ringStationInDatabase);
            else
                return BadRequest();            
        }

        // DELETE: /RingStations/5
        [ResponseType(typeof(RingStation))]
        public IHttpActionResult DeleteRingStation(int id)
        {
            var ringStation = dbDeleteRingStation(id);
            if (ringStation != null)
                return Ok(ringStation);
            else
                return NotFound();
        }

        private RingStation dbGetRingStation(int id)
        {
            RingStation ringStation = db.RingStations.Find(id);
            if (ringStation == null)
            {
                return null;
            }
            List<AnimalType> animalTypes = new List<AnimalType>();
            foreach (var animalType in ringStation.AnimalTypes)
            {
                animalTypes.Add(new AnimalType() { Name = animalType.Name, Id = animalType.Id });
            }
            var park = ringStation.Park == null ? null : new Park() { Name = ringStation.Park.Name, Id = ringStation.Park.Id };
            return new RingStation() { Name = ringStation.Name, Id = ringStation.Id, AnimalTypes = animalTypes, Park = park };
        }

        private bool dbPutRingStation(int id, RingStation ringStation)
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

        private RingStation dbPostRingStation(RingStation ringStation)
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
            ringStation.Id = ringStationInDatabase.Id;
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