using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorGeometry;

namespace Materials
{
    public class Lambert: brdf
    {
        string brdf;

        public Lambert(Dictionary<string, dynamic> dict)
        {
            Name = dict["name"];
            double[] numbers = ParseVect(dict, "color");
            color = new Color(numbers[0], numbers[1], numbers[2]);
            Type = dict["__type__"];
            brdf = dict["brdf"];
            UseForAmbient = dict["use_for_ambient"];
        }

        public override double FuncionReflectancia(Vector camDir, Vector normal, Vector lightDir)
        {
            lightDir.Normalizar();
            camDir.Normalizar();
            return Math.Max(0, Vector.ProductoPunto(normal, lightDir));
        }
    }
}
