using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class PacijentiForKartaReadDTO
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public decimal Temperatura { get; set; }
        public int Kasalj { get; set; }
        public int Umor { get; set; }
        public int BolUMisicima { get; set; }
        public long Vrijeme { get; set; }
        public string Stanje { get; set; } // ok, povisena temp ili udaljen vise od 1km
        public decimal Lat { get; set; }
        public decimal Long { get; set; }
    }
}
