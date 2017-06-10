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
    public class AnimalGenusesController : ApiController
    {
        private AnimalContext db = new AnimalContext();

        // GET: /AnimalGenuses
        public IEnumerable<AnimalGenus> GetAnimalGenuses()
        {
            var animalGenusesInDatabase = db.AnimalGenuses.ToList();
            List<AnimalGenus> animalGenuses = new List<AnimalGenus>();
            foreach (var animalGenus in animalGenusesInDatabase)
            {
                List<AnimalType> animalTypes = new List<AnimalType>();
                foreach (var animalType in animalGenus.AnimalTypes)
                {
                    animalTypes.Add(new AnimalType() { Name = animalType.Name, Id = animalType.Id });
                }
                List<Park> parks = new List<Park>();
                foreach (var park in animalGenus.Parks)
                {
                    parks.Add(new Park() { Name = park.Name, Id = park.Id });
                }
                animalGenuses.Add(new AnimalGenus() { Name = animalGenus.Name, Id = animalGenus.Id, AnimalTypes = animalTypes, Parks = parks });
            }
            return animalGenuses;
        }

        // GET: /AnimalGenuses/5
        [ResponseType(typeof(AnimalGenus))]
        public IHttpActionResult GetAnimalGenus(int id)
        {
            var animalGenus = dbGetAnimalGenus(id);
            if (animalGenus != null)
                return Ok(animalGenus);
            else
                return NotFound();
        }

        // PUT: /AnimalGenuses/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAnimalGenus(int id, [FromBody]AnimalGenus animalGenus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (dbPutAnimalGenus(id, animalGenus))
                return StatusCode(HttpStatusCode.NoContent);
            else
                return NotFound();
        }

        // POST: /AnimalGenuses
        [ResponseType(typeof(AnimalGenus))]
        public IHttpActionResult PostAnimalGenus([FromBody]AnimalGenus animalGenus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var animalGenusInDb = dbPostAnimalGenus(animalGenus);
            if (animalGenusInDb != null)
                return CreatedAtRoute("DefaultApi", new { id = animalGenusInDb.Id }, animalGenusInDb);
            else
                return BadRequest();            
        }

        // DELETE: /AnimalGenuses/5
        [ResponseType(typeof(AnimalGenus))]
        public IHttpActionResult DeleteAnimalGenus(int id)
        {
            var animalGenus = dbDeleteAnimalGenus(id);
            if (animalGenus != null)
                return Ok(animalGenus);
            else
                return NotFound();
        }

        private AnimalGenus dbGetAnimalGenus(int id)
        {
            AnimalGenus animalGenus = db.AnimalGenuses.Find(id);
            if (animalGenus == null)
            {
                return null;
            }
            List<AnimalType> animalTypes = new List<AnimalType>();
            foreach (var animalType in animalGenus.AnimalTypes)
            {
                animalTypes.Add(new AnimalType() { Name = animalType.Name, Id = animalType.Id });
            }
            List<Park> parks = new List<Park>();
            foreach (var park in animalGenus.Parks)
            {
                parks.Add(new Park() { Name = park.Name, Id = park.Id });
            }
            return new AnimalGenus() { Name = animalGenus.Name, Id = animalGenus.Id, AnimalTypes = animalTypes, Parks = parks };
        }

        private bool dbPutAnimalGenus(int id, AnimalGenus animalGenus)
        {
            var animalGenusInDb = db.AnimalGenuses.Find(id);
            if (animalGenusInDb == null)
            {
                return false;
            }

            foreach (var animalType in animalGenusInDb.AnimalTypes)
            {
                animalType.AnimalGenus = null;
            }

            animalGenusInDb.AnimalTypes = new List<AnimalType>();

            foreach (var animalType in animalGenus.AnimalTypes)
            {
                var newAnimalType = new AnimalType { Name = animalType.Name };
                db.AnimalTypes.Add(newAnimalType);
                newAnimalType.AnimalGenus = animalGenusInDb;
                animalGenusInDb.AnimalTypes.Add(newAnimalType);
            }

            foreach (var park in animalGenusInDb.Parks)
            {
                park.AnimalGenuses.Remove(animalGenusInDb);
            }

            animalGenusInDb.Parks = new List<Park>();

            foreach (var park in animalGenus.Parks)
            {
                var parkInDatabase = db.Parks.FirstOrDefault(elem => elem.Name == park.Name);
                if (parkInDatabase == null)
                {
                    parkInDatabase = new Park() { Name = park.Name };
                }

                parkInDatabase.AnimalGenuses.Add(animalGenusInDb);
                animalGenusInDb.Parks.Add(parkInDatabase);
            }

            animalGenusInDb.Name = animalGenus.Name;
                        
            db.SaveChanges();

            return true;       
        }

        private AnimalGenus dbPostAnimalGenus(AnimalGenus animalGenus)
        {
            if (db.AnimalGenuses.FirstOrDefault(elem => elem.Name == animalGenus.Name) != null)
            {
                return null;
            }

            var animalGenusInDb = new AnimalGenus() { Name = animalGenus.Name };
            db.AnimalGenuses.Add(animalGenusInDb);

            foreach (var park in animalGenus.Parks)
            {
                var parkInDb = db.Parks.FirstOrDefault(elem => elem.Name == park.Name);
                if (parkInDb == null)
                {
                    parkInDb = new Park() { Name = park.Name };
                }
                parkInDb.AnimalGenuses.Add(animalGenusInDb);
                animalGenusInDb.Parks.Add(parkInDb);
            }

            foreach (var animalType in animalGenus.AnimalTypes)
            {
                var newAnimalType = new AnimalType { Name = animalType.Name };
                db.AnimalTypes.Add(newAnimalType);
                newAnimalType.AnimalGenus = animalGenusInDb;
                animalGenusInDb.AnimalTypes.Add(newAnimalType);
            }

            db.SaveChanges();
            animalGenus.Id = animalGenusInDb.Id;
            return animalGenus;
        }

        private AnimalGenus dbDeleteAnimalGenus(int id)
        {
            AnimalGenus animalGenus = db.AnimalGenuses.Find(id);
            if (animalGenus == null)
            {
                return null;
            }

            foreach (var animalType in animalGenus.AnimalTypes)
            {
                animalType.AnimalGenus = null;
            }

            foreach (var park in animalGenus.Parks)
            {
                park.AnimalGenuses.Remove(animalGenus);
            }

            db.AnimalGenuses.Remove(animalGenus);
            db.SaveChanges();

            return animalGenus;
        }
    }
}