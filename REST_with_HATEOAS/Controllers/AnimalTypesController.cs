using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

using REST_with_HATEOAS.Models;
using REST_with_HATEOAS.Resources.AnimalTypes;
using REST_with_HATEOAS.Resources.AnimalGenuses;
using REST_with_HATEOAS.Resources.RingStations;

namespace REST_with_HATEOAS.Controllers
{
    public class AnimalTypesController : ApiController
    {
        private AnimalContext db = new AnimalContext();

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
        private AnimalTypeRepresentation GetAnimalType(AnimalType animalType)
        {
            var representation = new AnimalTypeRepresentation()
            {
                Id = animalType.Id,
                Name = animalType.Name
            };
            representation.Links.Add(LinkTemplates.AnimalTypes.DeleteAnimalType.CreateLink(new { id = animalType.Id }));
            representation.Links.Add(LinkTemplates.AnimalTypes.UpdateAnimalType.CreateLink(new { id = animalType.Id }));
            representation.AnimalGenus = animalType.AnimalGenus == null ? null : GetAnimalGenus(animalType.AnimalGenus);
            representation.RingStations = animalType.RingStations.Select(ringStation => GetRingStation(ringStation)).ToList();
            return representation;
        }

        // GET: /AnimalTypes
        public AnimalTypeListRepresentation GetAnimalTypes()
        {
            var response = new AnimalTypeListRepresentation();

            var animalTypesInDatabase = db.AnimalTypes.ToList();

            foreach (var animalType in animalTypesInDatabase)
            {
                response.ResourceList.Add(GetAnimalType(animalType));
            }

            return response;
        }

        // GET: /AnimalTypes/5
        [ResponseType(typeof(AnimalTypeRepresentation))]
        public IHttpActionResult GetAnimalType(int id)
        {
            var animalType = db.AnimalTypes.Find(id);
            if (animalType != null)
                return Ok(GetAnimalType(animalType));
            else
                return NotFound();
        }

        // PUT: /AnimalTypes/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAnimalType(int id, AnimalTypeRepresentation animalType)
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
        [ResponseType(typeof(AnimalTypeRepresentation))]
        public IHttpActionResult PostAnimalType(AnimalTypeRepresentation animalType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var animalTypeInDatabase = dbPostAnimalType(animalType);
            if (animalTypeInDatabase != null)
                return CreatedAtRoute("DefaultApi", new { id = animalTypeInDatabase.Id }, GetAnimalType(animalTypeInDatabase));
            else
                return BadRequest();            
        }

        // DELETE: /AnimalTypes/5
        [ResponseType(typeof(AnimalTypeRepresentation))]
        public IHttpActionResult DeleteAnimalType(int id)
        {
            var animalType = dbDeleteAnimalType(id);
            if (animalType != null)
                return Ok(GetAnimalType(animalType));
            else
                return NotFound();            
        }              
        
        private bool dbPutAnimalType(int id, AnimalTypeRepresentation animalType)
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

        private AnimalType dbPostAnimalType(AnimalTypeRepresentation animalType)
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