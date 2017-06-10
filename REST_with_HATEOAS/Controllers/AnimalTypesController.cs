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
    public class AnimalTypesController : ApiController
    {
        private AnimalContext db = new AnimalContext();

        // GET: /AnimalTypes
        public IEnumerable<AnimalType> GetAnimalTypes()
        {
            var animalTypesInDatabase = db.AnimalTypes.ToList();
            List<AnimalType> animalTypes = new List<AnimalType>();
            foreach (var animalType in animalTypesInDatabase)
            {
                List<RingStation> ringStations = new List<RingStation>();
                foreach (var ringStation in animalType.RingStations)
                {
                    ringStations.Add(new RingStation() { Name = ringStation.Name, Id = ringStation.Id });
                }
                var animalGenus = animalType.AnimalGenus == null ? null : new AnimalGenus() { Name = animalType.AnimalGenus.Name, Id = animalType.AnimalGenus.Id };
                animalTypes.Add(new AnimalType() { Name = animalType.Name, Id = animalType.Id, RingStations = ringStations, AnimalGenus = animalGenus });
            }
            return animalTypes;
        }

        // GET: /AnimalTypes/5
        [ResponseType(typeof(AnimalType))]
        public IHttpActionResult GetAnimalType(int id)
        {
            var animalType = dbGetAnimalType(id);
            if (animalType != null)
                return Ok(animalType);
            else
                return NotFound();
        }

        // PUT: /AnimalTypes/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAnimalType(int id, [FromBody]AnimalType animalType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (dbPutAnimalType(id, animalType))
                return StatusCode(HttpStatusCode.NoContent);
            else
                return NotFound();
        }

        // POST: /AnimalTypes
        [ResponseType(typeof(AnimalType))]
        public IHttpActionResult PostAnimalType([FromBody]AnimalType animalType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var animalTypeInDatabase = dbPostAnimalType(animalType);
            if (animalTypeInDatabase != null)
                return CreatedAtRoute("DefaultApi", new { id = animalTypeInDatabase.Id }, animalTypeInDatabase);
            else
                return BadRequest();            
        }

        // DELETE: /AnimalTypes/5
        [ResponseType(typeof(AnimalType))]
        public IHttpActionResult DeleteAnimalType(int id)
        {
            var animalType = dbDeleteAnimalType(id);
            if (animalType != null)
                return Ok(animalType);
            else
                return NotFound();            
        }              

        private AnimalType dbGetAnimalType(int id)
        {
            AnimalType animalType = db.AnimalTypes.Find(id);
            if (animalType == null)
            {
                return null;
            }
            List<RingStation> ringStations = new List<RingStation>();
            foreach (var ringStation in animalType.RingStations)
            {
                ringStations.Add(new RingStation() { Name = ringStation.Name, Id = ringStation.Id });
            }
            var animalGenus = animalType.AnimalGenus == null ? null : new AnimalGenus() { Name = animalType.AnimalGenus.Name, Id = animalType.AnimalGenus.Id };
            return new AnimalType() { Name = animalType.Name, Id = animalType.Id, RingStations = ringStations, AnimalGenus = animalGenus };
        }

        private bool dbPutAnimalType(int id, AnimalType animalType)
        {
            var animalTypeInDatabase = db.AnimalTypes.Find(id);
            if (animalTypeInDatabase == null)
            {
                return false;
            }

            foreach (var ringStations in animalTypeInDatabase.RingStations)
            {
                ringStations.AnimalTypes.Remove(animalTypeInDatabase);
            }

            animalTypeInDatabase.RingStations = new List<RingStation>();

            foreach (var ringStation in animalType.RingStations)
            {
                var ringStationInDatabase = db.RingStations.FirstOrDefault(elem => elem.Name == ringStation.Name);
                if (ringStationInDatabase == null)
                {
                    ringStationInDatabase = new RingStation() { Name = ringStation.Name };
                }

                animalTypeInDatabase.RingStations.Add(ringStationInDatabase);
                ringStationInDatabase.AnimalTypes.Add(animalTypeInDatabase);
            }

            if (animalTypeInDatabase.AnimalGenus != null)
            {
                animalTypeInDatabase.AnimalGenus.AnimalTypes.Remove(animalTypeInDatabase);
                animalTypeInDatabase.AnimalGenus = null;
            }

            if (animalType.AnimalGenus != null)
            {
                var animalGenusInDb = db.AnimalGenuses.FirstOrDefault(elem => elem.Name == animalType.AnimalGenus.Name);
                if (animalGenusInDb == null)
                {
                    animalGenusInDb = new AnimalGenus() { Name = animalType.AnimalGenus.Name };
                }
                animalTypeInDatabase.AnimalGenus = animalGenusInDb;
                animalGenusInDb.AnimalTypes.Add(animalTypeInDatabase);
            }

            animalTypeInDatabase.Name = animalType.Name;
                        
            db.SaveChanges();

            return true;
        }

        private AnimalType dbPostAnimalType(AnimalType animalType)
        {
            var animalTypeInDatabase = db.AnimalTypes.FirstOrDefault(elem => elem.Name == animalType.Name);
            if (animalTypeInDatabase != null)
            {
                return null;
            }
            animalTypeInDatabase = new AnimalType() { Name = animalType.Name };
            db.AnimalTypes.Add(animalTypeInDatabase);

            foreach (var ringStation in animalType.RingStations)
            {
                var ringStationInDatabase = db.RingStations.FirstOrDefault(elem => elem.Name == ringStation.Name);
                if (ringStationInDatabase == null)
                {
                    ringStationInDatabase = new RingStation() { Name = ringStation.Name };
                }
                animalTypeInDatabase.RingStations.Add(ringStationInDatabase);
                ringStationInDatabase.AnimalTypes.Add(animalTypeInDatabase);
            }

            if (animalType.AnimalGenus != null)
            {
                var animalGenusInDb = db.AnimalGenuses.FirstOrDefault(elem => elem.Name == animalType.AnimalGenus.Name);
                if (animalGenusInDb == null)
                {
                    animalGenusInDb = new AnimalGenus() { Name = animalType.AnimalGenus.Name };
                }
                animalTypeInDatabase.AnimalGenus = animalGenusInDb;
                animalGenusInDb.AnimalTypes.Add(animalTypeInDatabase);
            }

            db.SaveChanges();
            animalType.Id = animalTypeInDatabase.Id;
            return animalTypeInDatabase;
        }

        private AnimalType dbDeleteAnimalType(int id)
        {
            AnimalType animalType = db.AnimalTypes.Find(id);
            if (animalType == null)
            {
                return null;
            }

            foreach (var ringStation in animalType.RingStations)
            {
                ringStation.AnimalTypes.Remove(animalType);
            }

            if (animalType.AnimalGenus != null)
            {
                animalType.AnimalGenus.AnimalTypes.Remove(animalType);
            }

            db.AnimalTypes.Remove(animalType);
            db.SaveChanges();

            return animalType;
        }
    }
}