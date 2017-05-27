using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    public struct CriticPixel
    {
        public int Row;
        public int Col;
        public int RaysNumber;

        public CriticPixel(int row, int col, int rn)
        {
            Row = row;
            Col = col;
            RaysNumber = rn;
        }
    }
}
