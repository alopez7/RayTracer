using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorGeometry;
using Materials;

namespace Illumination
{
    public abstract class Light
    {
        public Color Color;

        public abstract Vector GetDirection(Vector point, int ID);

        public abstract double GetDistance(Vector point, int ID);

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
