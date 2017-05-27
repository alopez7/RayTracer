using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorGeometry;

namespace Materials
{
    public class BlinnPhong: brdf
    {
        string brdf;
        double Shininess;

        public BlinnPhong(Dictionary<string, dynamic> dict)
        {
            Name = dict["name"];
            Double[] numbers = ParseVect(dict, "color");
            color = new Color(numbers[0], numbers[1], numbers[2]);
            Type = dict["__type__"];
            brdf = dict["brdf"];
            Shininess = dict["brdfParams"]["shininess"];
            UseForAmbient = false;
        }

        public override double FuncionReflectancia(Vector camDir, Vector normal, Vector lightDir)
        {
            lightDir.Normalizar();
            camDir.Normalizar();
            Vector h = ((lightDir + camDir) / 2).Normalizado();
            return Math.Max(0, Math.Pow(Vector.ProductoPunto(normal, h), Shininess));
        }
    }
}
