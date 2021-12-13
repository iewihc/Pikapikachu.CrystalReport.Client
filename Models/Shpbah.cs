using System.Collections.Generic;

namespace Pikapikachu.CrystalReport.Client.Models
{
    public class Shpbah
    {
        public string nbr { get; set; }
        public string acrmon { get; set; }
        public int amt { get; set; }
        public List<Shpbat> Shpbats { get; set; } = new List<Shpbat>();
    }

    public class Shpbat
    {
        public string seq { get; set; }
        public string name { get; set; }
    }
}