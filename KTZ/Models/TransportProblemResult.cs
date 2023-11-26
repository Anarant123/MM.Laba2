using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KTZ.Models
{
    public class TransportProblemResult
    {
        public int[] U { get; set; }
        public int[] V { get; set; }
        public int[,] Basis { get; set; }
    }
}
