using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS
{
    public class AdditionalPlayerData
    {
        public AdditionalPlayerData()
        {
            SecondaryPositions = new List<string>();
        }

        public string PrefferedFoot { get; set; }
        public List<string> SecondaryPositions { get; set; }
    }
}
