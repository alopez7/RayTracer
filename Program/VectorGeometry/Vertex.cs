using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorGeometry
{
    public class Vertex
    {
        private Vector Position;
        public Vector Normal;
        public List<int> Triangle_IDs;
        public Vector pos
        {
            get
            {
                return Position;
            }
        }
        public Vector TexPosition;

        public Vertex(Vector posit)
        {
            Position = posit;
            Triangle_IDs = new List<int>();
        }
    }
}
