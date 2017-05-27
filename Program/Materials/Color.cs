using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorGeometry;

namespace Materials
{
    public class Color: Vector
    {
        public double R
        {
            get
            {
                return this[0];
            }
            set
            {
                this[0] = value;
            }
        }
        public double G
        {
            get
            {
                return this[1];
            }
            set
            {
                this[1] = value;
            }
        }
        public double B
        {
            get
            {
                return this[2];
            }
            set
            {
                this[2] = value;
            }
        }

        public Color(double R, double G, double B): base(R, G, B)
        {

        }

        public void Truncate()
        {
            for (int i = 0; i < 3; i++)
            {
                if (Components[i] < 0)
                {
                    Components[i] = 0;
                }
                else if (Components[i] > 1)
                {
                    Components[i] = 1;
                }
            }
        }
        
        public Color Copy()
        {
            return new Color(R, G, B);
        }

        public static Color operator +(Color c1, Color c2)
        {

            double[] ResultComponents = new double[c1.Dimensions];

            for (int i = 0; i < c1.Dimensions; i++)
            {
                ResultComponents[i] = c1[i] + c2[i];
            }
            Color Result = new Color(ResultComponents[0], ResultComponents[1], ResultComponents[2]);
            return Result;
        }

        public static Color operator -(Color c1, Color c2)
        {

            double[] ResultComponents = new double[c1.Dimensions];

            for (int i = 0; i < c1.Dimensions; i++)
            {
                ResultComponents[i] = c1[i] - c2[i];
            }
            Color Result = new Color(ResultComponents[0], ResultComponents[1], ResultComponents[2]);
            return Result;
        }

        public static Color operator *(Color c1, double num)
        {

            double[] ResultComponents = new double[c1.Dimensions];

            for (int i = 0; i < c1.Dimensions; i++)
            {
                ResultComponents[i] = c1[i] * num;
            }
            Color Result = new Color(ResultComponents[0], ResultComponents[1], ResultComponents[2]);
            return Result;
        }

        public static Color operator *(Color c1, Color c2)
        {

            double[] ResultComponents = new double[c1.Dimensions];

            for (int i = 0; i < c1.Dimensions; i++)
            {
                ResultComponents[i] = c1[i] * c2[i];
            }
            Color Result = new Color(ResultComponents[0], ResultComponents[1], ResultComponents[2]);
            return Result;
        }
    }
}
