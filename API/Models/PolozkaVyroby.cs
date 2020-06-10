using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class PolozkaVyroby
    {
        public int KodOperace { get; set; }
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }
        public int CisloOperace { get; set; }
        public string Popis { get; set; }
        public int Parametr { get; set; }
        public string MaterialKod { get; set; }
        public string MaterialNazev { get; set; }
        public int MaterialSarze { get; set; }
        public double VahaPozadovana { get; set; }
        public double VahaNaplneno { get; set; }
        public int StavOperace { get; set; }
        public string Operator { get; set; }
        public bool End { get; set; }
        public bool IsStarted { get; set; }
        public DateTime Start1 { get; set; }
        public DateTime Stop1 { get; set; }
        public int DelkaTrvani { get; set; }
        public string Sklad { get; set; }
    }
}
