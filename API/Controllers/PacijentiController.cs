using API.Data;
using API.DTOs;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Geolocation;
using System.Threading.Tasks;
using System.Linq;

namespace API.Controllers
{
    [Route("api/pacijenti")]
    [ApiController]
    public class PacijentiController : ControllerBase
    {
        private readonly IPacijentRepo _repository;
        private readonly IMapper _mapper;
        private readonly ILokacijaRepo _lokacijaRepo;
        private readonly IStanjeRepo _stanjeRepo;

        public PacijentiController(IPacijentRepo repository, IMapper mapper, ILokacijaRepo lokacijaRepo, IStanjeRepo stanjeRepo)
        {
            _repository = repository;
            _mapper = mapper;
            _lokacijaRepo = lokacijaRepo;
            _stanjeRepo = stanjeRepo;
        }

        [AllowAnonymous]
        [HttpGet("{status}", Name = "GetPacijentiByStatus")]
        public ActionResult<List<PacijentReadDTO>> GetPacijentiByStatus(int status)
        {
            List<PacijentReadDTO> pacijentiDTO = new List<PacijentReadDTO>();
            var pacijenti = _repository.GetPacijentiByStatus(status);
            foreach (var pacijent in pacijenti)
            {
                var zadnjaLokacija = _lokacijaRepo.GetLastLokacijeByID(pacijent.Id);
                var ZadnjeStanje = _stanjeRepo.GetLastStanjeByID(pacijent.Id);
                List<StanjePacijenta> povijestStanja = _stanjeRepo.GetStanjaByID(pacijent.Id).ToList();

                PacijentReadDTO pacijentReadDTO = new PacijentReadDTO() { 
                    Id = pacijent.Id,
                    Oib = pacijent.Oib,
                    Ime = pacijent.Ime,
                    Prezime = pacijent.Prezime,
                    AdresaSi = pacijent.AdresaSi,
                    Lat = pacijent.Lat,
                    Long = pacijent.Long,
                    ZadnjaLokacija = zadnjaLokacija,
                    ZadnjeStanje = ZadnjeStanje,
                    PovijestStanja = povijestStanja
                };

                pacijentiDTO.Add(pacijentReadDTO);
            }

            return Ok(pacijentiDTO);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult<IEnumerable<PacijentReadDTO>> GetAllPacijenti()
        {
            var pacijenti = _repository.GetAllPacijenti();
            foreach (var pacijent in pacijenti)
            {
                var stanje = _stanjeRepo.GetLastStanjeByID(pacijent.Id);
                var udaljenost = 0d;
                var trenutnaLokacija = _lokacijaRepo.GetLastLokacijeByID(pacijent.Id);
                if(trenutnaLokacija?.Id != null && trenutnaLokacija?.Id != 0)
                {
                    var TL = new Coordinate(Convert.ToDouble(trenutnaLokacija.Lat), Convert.ToDouble(trenutnaLokacija.Long));
                    var SI = new Coordinate(Convert.ToDouble(pacijent.Lat), Convert.ToDouble(pacijent.Long));
                    udaljenost = GeoCalculator.GetDistance(TL, SI, 5) / 0.62137;
                }
                
                if (stanje?.Temperatura > 37)
                {
                    pacijent.Stanje = "Visoka temp.";
                }
                else if(udaljenost > 1)
                {
                    pacijent.Stanje = "Udaljen više od 1km";
                }
                else 
                {
                    pacijent.Stanje = "Ok";
                }
            }
            return Ok(_mapper.Map<IEnumerable<PacijentReadDTO>>(pacijenti));
        }

        [AllowAnonymous]
        [HttpGet("GetPacijentiForKarta", Name = "GetPacijentiForKarta")]
        public ActionResult<IEnumerable<PacijentiForKartaReadDTO>> GetPacijentiForKarta()
        {
            List<PacijentiForKartaReadDTO> pacijentiForKarta = new List<PacijentiForKartaReadDTO>();
            var pacijenti = _repository.GetAllPacijenti();
            foreach (var pacijent in pacijenti)
            {
                var stanje = _stanjeRepo.GetLastStanjeByID(pacijent.Id);
                var udaljenost = 0d;
                var trenutnaLokacija = _lokacijaRepo.GetLastLokacijeByID(pacijent.Id);
                if (trenutnaLokacija?.Id != null && trenutnaLokacija?.Id != 0)
                {
                    var TL = new Coordinate(Convert.ToDouble(trenutnaLokacija.Lat), Convert.ToDouble(trenutnaLokacija.Long));
                    var SI = new Coordinate(Convert.ToDouble(pacijent.Lat), Convert.ToDouble(pacijent.Long));
                    udaljenost = GeoCalculator.GetDistance(TL, SI, 5) / 0.62137;
                }

                if (stanje?.Temperatura > 37)
                {
                    pacijent.Stanje = "Visoka temp.";
                }
                else if (udaljenost > 1)
                {
                    pacijent.Stanje = "Udaljen više od 1km";
                }
                else
                {
                    pacijent.Stanje = "Ok";
                }
                PacijentiForKartaReadDTO pacijentForKarta = new PacijentiForKartaReadDTO();
                _mapper.Map<Pacijent, PacijentiForKartaReadDTO>(pacijent, pacijentForKarta);
                _mapper.Map<LokacijaPacijenta, PacijentiForKartaReadDTO>(trenutnaLokacija, pacijentForKarta);
                _mapper.Map<StanjePacijenta, PacijentiForKartaReadDTO>(stanje, pacijentForKarta);
                pacijentiForKarta.Add(pacijentForKarta);
            }
            return Ok(pacijentiForKarta);
        }

        [AllowAnonymous]
        [HttpGet("GetPacijentByID/{ID}", Name = "GetPacijentByID")]
        public ActionResult<PacijentReadDTO> GetPacijentByID(long ID)
        {
            var pacijent = _repository.GetPacijentByID(ID);
            if (pacijent != null)
            {
                return Ok(_mapper.Map<PacijentReadDTO>(pacijent));
            }
            return NotFound();
        }

        [AllowAnonymous]
        [HttpGet("{OIB}", Name = "GetPacijentByOIB")]
        public ActionResult<PacijentReadDTO> GetPacijentByOIB(long OIB)
        {
            var pacijent = _repository.GetPacijentByOIB(OIB);
            if (pacijent != null)
            {
                return Ok(_mapper.Map<PacijentReadDTO>(pacijent));
            }
            return NotFound();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<PacijentReadDTO> CreatePacijent(PacijentCreateDTO pacijent)
        {
            var pacijentModel = _mapper.Map<Pacijent>(pacijent);
            _repository.CreatePacijent(pacijentModel);
            _repository.SaveChanges();

            var pacijentReadDTO = _mapper.Map<PacijentReadDTO>(pacijentModel);
            return CreatedAtRoute(nameof(GetPacijentByOIB), new { Oib = pacijentReadDTO.Oib }, pacijentReadDTO);
        }

        [AllowAnonymous]
        [HttpPut("{OIB}")]
        public async Task<ActionResult> UpdatePacijent(long OIB, PacijentUpdateDTO pacijent)
        {
            var pacijentModelFromRepo = _repository.GetPacijentByOIB(OIB);
            if (pacijentModelFromRepo == null)
            {
                return NotFound();
            }
            _mapper.Map(pacijent, pacijentModelFromRepo);
            await _repository.UpdatePacijent(pacijentModelFromRepo);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpPatch("{OIB}")]
        public ActionResult PartialPacijentUpdate(int OIB, JsonPatchDocument<PacijentUpdateDTO> patchDoc)
        {
            var pacijentModelFromRepo = _repository.GetPacijentByOIB(OIB);
            if (pacijentModelFromRepo == null)
            {
                return NotFound();
            }
            var pacijentToPatch = _mapper.Map<PacijentUpdateDTO>(pacijentModelFromRepo);
            patchDoc.ApplyTo(pacijentToPatch, ModelState);
            if (!TryValidateModel(pacijentToPatch))
            {
                return ValidationProblem(ModelState);
            }
            _mapper.Map(pacijentToPatch, pacijentModelFromRepo);
            _repository.UpdatePacijent(pacijentModelFromRepo);
            _repository.SaveChanges();
            return NoContent();
        }

        [AllowAnonymous]
        [HttpDelete("{OIB}")]
        public ActionResult DeletePacijent(long OIB)
        {
            var pacijentModelFromRepo = _repository.GetPacijentByOIB(OIB);
            if (pacijentModelFromRepo == null)
            {
                return NotFound();
            }
            _repository.DeletePacijent(pacijentModelFromRepo);
            _repository.SaveChanges();
            return NoContent();
        }

    }
}