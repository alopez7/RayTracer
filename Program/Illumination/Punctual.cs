using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorGeometry;
using Materials;

namespace Illumination
{
    public class Punctual: Light
    {
        public Vector Position;

        public Punctual(Dictionary<string, dynamic> dict)
        {
            Double[] numbers = ParseVect(dict, "color");
            Color = new Color(numbers[0], numbers[1], numbers[2]);
            Position = new Vector(ParseVect(dict, "position"));
        }

        public override double GetDistance(Vector point, int ID)
        {
            return (Position - point).Magnitud;
        }

        public override Vector GetDirection(Vector point, int ID)
        {
            return (Position - point).Normalizado();
        }
    }
}
