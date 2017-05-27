using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorGeometry;

namespace Materials
{
    public class Mirror: Material
    {
        double GlossyFactor;

        public Mirror(Dictionary<string, dynamic> dict)
        {
            Name = dict["name"];
            double[] numbers = ParseVect(dict, "color");
            color = new Color(numbers[0], numbers[1], numbers[2]);
            Type = dict["__type__"];
            try
            {
                GlossyFactor = dict["glossyFactor"];
            }
            catch
            {
                GlossyFactor = 0;
            }
            UseForAmbient = false;
        }
    }
}
