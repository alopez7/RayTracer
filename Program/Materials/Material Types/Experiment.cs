using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorGeometry;

namespace Materials
{
    public class Experiment: brdf
    {
        private string brdf;
        private double Multiplicador; 

        public Experiment(Dictionary<string, dynamic> dict)
        {
            Name = dict["name"];
            Double[] numbers = ParseVect(dict, "color");
            color = new Color(numbers[0], numbers[1], numbers[2]);
            Type = dict["__type__"];
            brdf = dict["brdf"];
            UseForAmbient = dict["use_for_ambient"];
            Multiplicador = dict["brdfParams"]["multiplicador"];
        }

        public override double FuncionReflectancia(Vector camDir, Vector normal, Vector lightDir)
        {
            lightDir.Normalizar();
            camDir.Normalizar();
            double a = Vector.ProductoPunto(normal, lightDir) * Multiplicador;
            return Math.Pow((Math.Sin(a)), 2);
        }
    }
}
