using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class TechnologickyRozpis
    {
        public DateTime ZStart { get; set; }
        public DateTime ZStop { get; set; }
        public HlavickaZakazky Hlavicka { get; set; }
        public double PlanovaneMnozstvi { get; set; }
        public double SkutecneMnozstvi { get; set; }
        public int Stav { get; set; }
        public bool JeVBehu { get; set; }
        public int AktualniPolozka { get; set; }

        public List<PolozkaVyroby> Polozky { get; set; }
    }
}
