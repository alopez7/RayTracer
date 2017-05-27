using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorGeometry;
using Materials;

namespace Illumination
{
    public class Directional: Light
    {
        private Vector DirectionVector;

        public Directional(Dictionary<string,dynamic> dict)
        {
            Double[] numbers = ParseVect(dict, "color");
            Color = new Color(numbers[0], numbers[1], numbers[2]);
            DirectionVector = (new Vector(ParseVect(dict, "direction"))).Normalizado();
        }

        public override double GetDistance(Vector point, int ID)
        {
            return double.PositiveInfinity;
        }

        public override Vector GetDirection(Vector point, int ID)
        {
            return (DirectionVector);
        }
    }
}
