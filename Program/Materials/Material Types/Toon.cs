using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorGeometry;

namespace Materials
{
    public class Toon: Lambert
    {
        private int Intervals;

        public Toon(Dictionary<string, dynamic> dict): base(dict)
        {
            Intervals = dict["brdfParams"]["intervals"];
        }

        public override double FuncionReflectancia(Vector camDir, Vector normal, Vector lightDir)
        {
            double val = base.FuncionReflectancia(camDir, normal, lightDir);
            int cant = (int)(val * Intervals);
            double a = ((double)cant) / ((double)Intervals);
            return a;
        }
    }
}
