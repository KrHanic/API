using API.DTOs;
using API.EFModels;
using API.Models;
using AutoMapper;
using Geolocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class PacijentRepo : IPacijentRepo
    {
        private readonly KV_TESTContext _context;
        private readonly IMapper _mapper;

        public PacijentRepo(KV_TESTContext ctx, IMapper mapper)
        {
            _context = ctx;
            _mapper = mapper;
        }
        public List<PacijentReadDTO> GetPacijenteKojiImajuSimptome(PacijentFilterDTO filter)
        {
            List<PacijentReadDTO> pacijentiDTO = new List<PacijentReadDTO>();

            var FiltriraniPacijentiUIzolaciji = _context._02Pacijent.Where(
                    p => p._02LokacijaPacijenta.Where(
                        l => l.Vrijeme >= filter.VrijemeOd &&
                        l.Vrijeme <= filter.VrijemeDo)
                    .Any(l => l.KorisnikId == p.Id)
                ).ToList();

            foreach (var pacijent in FiltriraniPacijentiUIzolaciji)
            {
                bool imaSimptome = false;
                var satanja = _context._02StanjePacijenta.Where(
                        l => l.KorisnikId == pacijent.Id &&
                        l.Vrijeme >= filter.VrijemeOd &&
                        l.Vrijeme <= filter.VrijemeDo
                    ).ToList();

                foreach (var stanje in satanja)
                {
                    if (stanje.BolUMisicima == 1 || stanje.Kasalj == 1 ||
                        stanje.Umor == 1 || stanje.Temperatura > 37)
                        imaSimptome = true;
                }
                if (imaSimptome)
                    pacijentiDTO.Add(new PacijentReadDTO() {
                        Id = pacijent.Id,
                        Oib = pacijent.Oib,
                        Ime = pacijent.Ime,
                        Prezime = pacijent.Prezime,
                        AdresaSi = pacijent.AdresaSi
                    });
            }

            return pacijentiDTO;
        }
        public List<PacijentReadDTO> GetPacijenteKojiSuPrekrsiliIzolaciju(PacijentFilterDTO filter)
        {
            List<PacijentReadDTO> pacijentiDTO = new List<PacijentReadDTO>();

            var FiltriraniPacijentiUIzolaciji = _context._02Pacijent.Where(
                    p => p._02LokacijaPacijenta.Where(
                        l => l.Vrijeme >= filter.VrijemeOd &&
                        l.Vrijeme <= filter.VrijemeDo)
                    .Any(l => l.KorisnikId == p.Id)
                ).ToList();

            foreach (var pacijent in FiltriraniPacijentiUIzolaciji)
            {
                bool udaljenosPrekrsena = false;
                var lokacije = _context._02LokacijaPacijenta.Where(
                        l => l.KorisnikId == pacijent.Id &&
                        l.Vrijeme >= filter.VrijemeOd &&
                        l.Vrijeme <= filter.VrijemeDo
                    ).ToList();

                foreach (var lokacija in lokacije)
                {
                    var TL = new Coordinate(Convert.ToDouble(lokacija.Lat), Convert.ToDouble(lokacija.Long));
                    var SI = new Coordinate(Convert.ToDouble(pacijent.Lat), Convert.ToDouble(pacijent.Long));
                    var udaljenost = GeoCalculator.GetDistance(TL, SI, 5) / 0.62137;
                    if (udaljenost > 1)
                        udaljenosPrekrsena = true;
                }
                if (udaljenosPrekrsena)
                    pacijentiDTO.Add(new PacijentReadDTO() { 
                        Id = pacijent.Id,
                        Oib = pacijent.Oib,
                        Ime = pacijent.Ime,
                        Prezime = pacijent.Prezime,
                        AdresaSi = pacijent.AdresaSi
                    });
            }

            return pacijentiDTO;
        }
        public int? GetBrojPacijenataSaSimptomima(PacijentFilterDTO filter)
        {
            int broj = 0;
            var FiltriraniPacijentiUIzolaciji = _context._02Pacijent.Where(
                    p => p._02LokacijaPacijenta.Where(
                        l => l.Vrijeme >= filter.VrijemeOd &&
                        l.Vrijeme <= filter.VrijemeDo)
                    .Any(l => l.KorisnikId == p.Id)
                ).ToList();

            foreach (var pacijent in FiltriraniPacijentiUIzolaciji)
            {
                bool imaSimptome = false;
                var satanja = _context._02StanjePacijenta.Where(
                        l => l.KorisnikId == pacijent.Id &&
                        l.Vrijeme >= filter.VrijemeOd &&
                        l.Vrijeme <= filter.VrijemeDo
                    ).ToList();

                foreach (var stanje in satanja)
                {
                    if (stanje.BolUMisicima == 1 || stanje.Kasalj == 1 || 
                        stanje.Umor == 1 || stanje.Temperatura > 37)
                        imaSimptome = true;
                }
                if (imaSimptome)
                    broj++;
            }

            return broj;
        }
        public int? GetBrojPacijenataKojiSuPrekrsiliIzolaciju(PacijentFilterDTO filter)
        {
            int broj = 0;
            var FiltriraniPacijentiUIzolaciji = _context._02Pacijent.Where(
                    p => p._02LokacijaPacijenta.Where(
                        l => l.Vrijeme >= filter.VrijemeOd &&
                        l.Vrijeme <= filter.VrijemeDo)
                    .Any(l => l.KorisnikId == p.Id)
                ).ToList();

            foreach (var pacijent in FiltriraniPacijentiUIzolaciji)
            {
                bool udaljenosPrekrsena = false;
                var lokacije = _context._02LokacijaPacijenta.Where(
                        l => l.KorisnikId == pacijent.Id &&
                        l.Vrijeme >= filter.VrijemeOd &&
                        l.Vrijeme <= filter.VrijemeDo
                    ).ToList();

                foreach (var lokacija in lokacije)
                {
                    var TL = new Coordinate(Convert.ToDouble(lokacija.Lat), Convert.ToDouble(lokacija.Long));
                    var SI = new Coordinate(Convert.ToDouble(pacijent.Lat), Convert.ToDouble(pacijent.Long));
                    var udaljenost = GeoCalculator.GetDistance(TL, SI, 5) / 0.62137;
                    if (udaljenost > 1)
                        udaljenosPrekrsena = true;
                }
                if (udaljenosPrekrsena)
                    broj++;
            }

            return broj;
        }
        public int? GetBrojPacijenataUIzolaciji(PacijentFilterDTO filter)
        {
            return _context._02Pacijent.Where(p => p._02LokacijaPacijenta.Where(
                    l => l.Vrijeme >= filter.VrijemeOd && 
                    l.Vrijeme <= filter.VrijemeDo)
                    .Any(l => l.KorisnikId == p.Id)
                ).Count();
        }
        public int? GetBrojPacijenataVanIzolacije(PacijentFilterDTO filter)
        {
            return _context._02Pacijent.Where(p => p.Status == 3 && 
            (p._02LokacijaPacijenta.Where(
                    l => l.KorisnikId == p.Id &&
                    l.Vrijeme >= filter.VrijemeOd)
                    .Max(l => l.Vrijeme) <= filter.VrijemeDo)
                ).Count();
        }
        public long CreatePacijent(Pacijent pacijent)
        {
            if (pacijent == null)
            {
                throw new ArgumentNullException(nameof(pacijent));
            }

            var pacijentDB = _mapper.Map<_02Pacijent>(pacijent);
            _context._02Pacijent.Add(pacijentDB);
            _context.SaveChanges();
            return pacijentDB.Id;
        }

        public async Task RemovePacijentFromIsolation(Pacijent pacijent)
        {
            if (pacijent == null)
            {
                throw new ArgumentNullException(nameof(pacijent));
            }

            var entity = _context._02Pacijent.First(g => g.Id == pacijent.Id);
            _context.Entry(entity).CurrentValues.SetValues(pacijent);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<Pacijent> GetAllPacijenti()
        {
            return _mapper.Map<List<Pacijent>>(_context._02Pacijent.ToList());
        }

        public Pacijent GetPacijentByOIB(long OIB)
        {
            return _mapper.Map<Pacijent>(_context._02Pacijent.Where(x => x.Oib == OIB).FirstOrDefault());
        }

        public Pacijent GetPacijentByID(long ID)
        {
            return _mapper.Map<Pacijent>(_context._02Pacijent.Where(x => x.Id == ID).FirstOrDefault());
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public async Task UpdatePacijent(Pacijent pacijent)
        {
            var entity = _context._02Pacijent.First(g => g.Id == pacijent.Id);
            _context.Entry(entity).CurrentValues.SetValues(pacijent);
            await _context.SaveChangesAsync();
        }

        public List<Pacijent> GetPacijentiByStatus(int status)
        {
            return _mapper.Map<List<Pacijent>>(_context._02Pacijent.Where(x => x.Status == status).ToList());
        }
    }
}
