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
    public class ParksController : ApiController
    {
        private AnimalContext db = new AnimalContext();

        // GET: /Parks
        public IEnumerable<Park> GetParks()
        {
            var parksInDatabase = db.Parks.ToList();
            List<Park> parks = new List<Park>();
            foreach (var park in parksInDatabase)
            {
                List<AnimalGenus> animalGenuses = new List<AnimalGenus>();
                foreach (var animalGenus in park.AnimalGenuses)
                {
                    animalGenuses.Add(new AnimalGenus() { Name = animalGenus.Name, Id = animalGenus.Id });
                }
                List<RingStation> ringStations = new List<RingStation>();
                foreach (var ringStation in park.RingStations)
                {
                    ringStations.Add(new RingStation() { Name = ringStation.Name, Id = ringStation.Id });
                }
                parks.Add(new Park() { Name = park.Name, Id = park.Id, AnimalGenuses = animalGenuses, RingStations = ringStations });
            }
            return parks;
        }

        // GET: /Parks/5
        [ResponseType(typeof(Park))]
        public IHttpActionResult GetPark(int id)
        {
            var park = dbGetPark(id);
            if (park != null)
                return Ok(park);
            else
                return NotFound();
        }

        // PUT: /Parks/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPark(int id, [FromBody]Park park)
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
        [ResponseType(typeof(Park))]
        public IHttpActionResult PostPark([FromBody]Park park)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var parkInDb = dbPostPark(park);
            if (parkInDb != null)
                return CreatedAtRoute("DefaultApi", new { id = parkInDb.Id }, parkInDb);
            else
                return BadRequest();            
        }

        // DELETE: /Parks/5
        [ResponseType(typeof(Park))]
        public IHttpActionResult DeletePark(int id)
        {
            var park = dbDeletePark(id);
            if (park != null)
                return Ok(park);
            else
                return NotFound();            
        }

        private Park dbGetPark(int id)
        {
            Park park = db.Parks.Find(id);
            if (park == null)
            {
                return null;
            }
            List<AnimalGenus> animalGenuses = new List<AnimalGenus>();
            foreach (var animalGenus in park.AnimalGenuses)
            {
                animalGenuses.Add(new AnimalGenus() { Name = animalGenus.Name, Id = animalGenus.Id });
            }
            List<RingStation> ringStations = new List<RingStation>();
            foreach (var ringStation in park.RingStations)
            {
                ringStations.Add(new RingStation() { Name = ringStation.Name, Id = ringStation.Id });
            }
            return new Park() { Name = park.Name, Id = park.Id, RingStations = ringStations, AnimalGenuses = animalGenuses };
        }

        private bool dbPutPark(int id, Park park)
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

        private Park dbPostPark(Park park)
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
            park.Id = parkInDb.Id;
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