using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.ModelsApi
{
    public class HlavickaZakazky
    {
        public string VyrobniZakazka { get; set; }
        public string KodPolozky { get; set; }
        public string NazevPolozky { get; set; }
        public double Mnozstvi { get; set; }
        public string Sarze { get; set; }
        public DateTime Datum { get; set; }
        public DateTime Cas { get; set; }
        public DateTime ZPStart { get; set; }
        public DateTime ZPStop { get; set; }
    }
}
