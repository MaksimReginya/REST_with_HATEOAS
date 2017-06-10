using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

using REST_with_HATEOAS.Models;
using REST_with_HATEOAS.Resources.AnimalGenuses;
using REST_with_HATEOAS.Resources.Parks;
using REST_with_HATEOAS.Resources.AnimalTypes;

namespace REST_with_HATEOAS.Controllers
{
    public class AnimalGenusesController : ApiController
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
        private AnimalGenusRepresentation GetAnimalGenus(AnimalGenus animalGenus)
        {
            var representation = new AnimalGenusRepresentation()
            {
                Id = animalGenus.Id,
                Name = animalGenus.Name
            };
            representation.Links.Add(LinkTemplates.AnimalGenuses.DeleteAnimalGenus.CreateLink(new { id = animalGenus.Id }));
            representation.Links.Add(LinkTemplates.AnimalGenuses.UpdateAnimalGenus.CreateLink(new { id = animalGenus.Id }));
            representation.AnimalTypes = animalGenus.AnimalTypes.Select(animalType => GetAnimalType(animalType)).ToList();
            representation.Parks = animalGenus.Parks.Select(park => GetPark(park)).ToList();
            return representation;
        }

        // GET: /AnimalGenuses
        public AnimalGenusListRepresentation GetAnimalGenuses()
        {
            var response = new AnimalGenusListRepresentation();

            var animalGenusesInDatabase = db.AnimalGenuses.ToList();

            foreach (var animalGenus in animalGenusesInDatabase)
            {
                response.ResourceList.Add(GetAnimalGenus(animalGenus));
            }

            return response;
        }

        // GET: /AnimalGenuses/5   
        [ResponseType(typeof(AnimalGenusRepresentation))]
        public IHttpActionResult GetAnimalGenus(int id)
        {
            var animalGenus = db.AnimalGenuses.Find(id);
            if (animalGenus != null)
                return Ok(GetAnimalGenus(animalGenus));
            else
                return NotFound();
        }

        // PUT: /AnimalGenuses/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAnimalGenus(int id, AnimalGenusRepresentation animalGenus)
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
        [ResponseType(typeof(AnimalGenusRepresentation))]
        public IHttpActionResult PostAnimalGenus(AnimalGenusRepresentation animalGenus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var animalGenusInDb = dbPostAnimalGenus(animalGenus);
            if (animalGenusInDb != null)
                return CreatedAtRoute("DefaultApi", new { id = animalGenusInDb.Id }, GetAnimalGenus(animalGenusInDb));
            else
                return BadRequest();            
        }

        // DELETE: /AnimalGenuses/5
        [ResponseType(typeof(AnimalGenusRepresentation))]
        public IHttpActionResult DeleteAnimalGenus(int id)
        {
            var animalGenus = dbDeleteAnimalGenus(id);
            if (animalGenus != null)
                return Ok(GetAnimalGenus(animalGenus));
            else
                return NotFound();
        }
        
        private bool dbPutAnimalGenus(int id, AnimalGenusRepresentation animalGenus)
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

        private AnimalGenus dbPostAnimalGenus(AnimalGenusRepresentation animalGenus)
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
            
            return animalGenusInDb;
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