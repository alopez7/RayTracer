using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Materials;
using VectorGeometry;

namespace Illumination
{
    public class AreaLight: Light
    {

        private Random random;
        private Vector Position;
        private double SizeA;
        private double SizeB;
        private Vector DirectionA;
        private Vector DirectionB;
        private Dictionary<int, Vector> Positions;


        public AreaLight(Dictionary<string, dynamic> dict)
        {
            random = new Random();
            double[] numbers = ParseVect(dict, "color");
            Color = new Color(numbers[0], numbers[1], numbers[2]);
            Position = new Vector(ParseVect(dict, "position"));
            DirectionA = new Vector(ParseVect(dict, "directionA"));
            DirectionB = new Vector(ParseVect(dict, "directionB"));
            SizeA = dict["sizeA"];
            SizeB = dict["sizeB"];
            Positions = new Dictionary<int, Vector>();
        }

        public Vector RandomPosition(int ID)
        {
            Vector pos;
            Vector DespA = (DirectionA * (GetRandom() * SizeA - SizeA / 2));
            Vector DespB = (DirectionB * (GetRandom() * SizeB - SizeB / 2));
            pos = (Position + DespA + DespB);
            lock (Positions)
            {
                if (Positions.ContainsKey(ID)) Positions[ID] = pos;
                else Positions.Add(ID, pos);
            }
            return pos;
        }

        public override Vector GetDirection(Vector point, int ID)
        {
            
            return (RandomPosition(ID) - point).Normalizado();
        }

        public override double GetDistance(Vector point, int ID)
        {
            return (Positions[ID] - point).Magnitud;
        }

        public double GetRandom()
        {
            double value;
            lock (random)
            {
                value = random.NextDouble();
            }
            return value;
        }
    }
}
