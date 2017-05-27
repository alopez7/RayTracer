using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorGeometry;

namespace Materials
{
    public abstract class Material
    {
        public Color color;
        public string Type;
        public string Name;
        public bool UseForAmbient;

        public double[] ParseVect(Dictionary<string, dynamic> dic, string key)
        {
            double[] arr = new double[3];
            for (int i = 0; i < 3; i++)
            {
                arr[i] = dic[key][i];
            }
            return arr;
        }
    }
}
