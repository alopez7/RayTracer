using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorGeometry;

namespace Materials
{
    public class Dielectric: Material
    {
        public double IR;
        public Vector Attenuation;

        public Dielectric(Dictionary<string, dynamic> dict)
        {
            Name = dict["name"];
            double[] numbers = ParseVect(dict, "color");
            color = new Color(numbers[0], numbers[1], numbers[2]);
            Type = dict["__type__"];
            Attenuation = new Vector(ParseVect(dict, "attenuation"));
            UseForAmbient = false;
            IR = dict["refraction_index"];
        }

        public Color Attenuated(double distance)
        {
            return new Color(Math.Exp(-Attenuation[0] * distance), Math.Exp(-Attenuation[1] * distance), Math.Exp(-Attenuation[2] * distance));
        }
    }
}
