using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorGeometry
{
    public class Vector
    {

        //Atributos
        public double[] Components;

        public int Dimensions
        {
            get
            {
                return Components.Length;
            }
        }

        public double X
        {
            get
            {
                return Components[0];
            }
            set
            {
                Components[0] = value;
            }
        }

        public double Y
        {
            get
            {
                return Components[1];
            }
            set
            {
                Components[1] = value;
            }
        }

        public double Z
        {
            get
            {
                return Components[2];
            }
            set
            {
                Components[2] = value;
            }
        }

        public double Magnitud
        {
            get
            {
                return Math.Sqrt((ProductoPunto(this, this)));
            }
        }

        public double this[int index]
        {
            get
            {
                return Components[index];
            }
            set
            {
                Components[index] = value;
            }
        }

        public Vector(params double[] param)
        {
            Components = new double[param.Length];
            for (int i = 0; i < Dimensions; i++)
            {
                Components[i] = param[i];
            }
        }

        public override string ToString()
        {
            string resp = "(";
            for (int i = 0; i < Dimensions - 1; i++)
            {
                resp += Components[i] + ", ";
            }
            resp += Components[Dimensions - 1] + ")";
            return resp;
        }

        public void Sumar(Vector otro)
        {
            if (otro.Dimensions != otro.Dimensions)
            {
                throw new InvalidOperationException("EL tamano de los vectores es distinto");
            }

            for (int i = 0; i < otro.Dimensions; i++)
            {
                Components[i] += otro[i];
            }
        }

        public static Vector operator +(Vector v1, Vector v2)
        {
            if (v1.Dimensions != v2.Dimensions)
            {
                throw new InvalidOperationException("EL tamano de los vectores es distinto");
            }

            double[] ResultComponents = new double[v1.Dimensions];

            for (int i = 0; i < v1.Dimensions; i++)
            {
                ResultComponents[i] = v1[i] + v2[i];
            }
            Vector Result = new Vector(ResultComponents);
            return Result;
        }

        public void Restar(Vector otro)
        {
            if (otro.Dimensions != otro.Dimensions)
            {
                throw new InvalidOperationException("EL tamano de los vectores es distinto");
            }

            for (int i = 0; i < otro.Dimensions; i++)
            {
                Components[i] -= otro[i];
            }
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            if (v1.Dimensions != v2.Dimensions)
            {
                throw new InvalidOperationException("EL tamano de los vectores es distinto");
            }

            double[] ResultComponents = new double[v1.Dimensions];

            for (int i = 0; i < v1.Dimensions; i++)
            {
                ResultComponents[i] = v1[i] - v2[i];
            }
            Vector Result = new Vector(ResultComponents);
            return Result;
        }

        public void Ponderar(double ponderador)
        {

            for (int i = 0; i < Dimensions; i++)
            {
                Components[i] *= ponderador;
            }
        }

        public static Vector operator *(Vector v1, double ponderador)
        {

            double[] ResultComponents = new double[v1.Dimensions];

            for (int i = 0; i < v1.Dimensions; i++)
            {
                ResultComponents[i] = v1[i] * ponderador;
            }
            Vector Result = new Vector(ResultComponents);
            return Result;
        }

        public void Dividir(double divisor)
        {
            if (divisor == 0)
            {
                throw new DivideByZeroException("No puedes dividir un vector por 0");
            }

            for (int i = 0; i < Dimensions; i++)
            {
                Components[i] /= divisor;
            }
        }

        public static Vector operator /(Vector v1, double divisor)
        {

            if (divisor == 0)
            {
                throw new DivideByZeroException("No puedes dividir un vector por 0");
            }

            double[] ResultComponents = new double[v1.Dimensions];

            for (int i = 0; i < v1.Dimensions; i++)
            {
                ResultComponents[i] = v1[i] / divisor;
            }
            Vector Result = new Vector(ResultComponents);
            return Result;
        }

        public void Multiplicar(Vector otro)
        {
            if (otro.Dimensions != otro.Dimensions)
            {
                throw new InvalidOperationException("EL tamano de los vectores es distinto");
            }

            for (int i = 0; i < otro.Dimensions; i++)
            {
                Components[i] *= otro[i];
            }
        }

        public static Vector operator *(Vector v1, Vector v2)
        {
            if (v1.Dimensions != v2.Dimensions)
            {
                throw new InvalidOperationException("EL tamano de los vectores es distinto");
            }

            double[] ResultComponents = new double[v1.Dimensions];

            for (int i = 0; i < v1.Dimensions; i++)
            {
                ResultComponents[i] = v1[i] * v2[i];
            }
            Vector Result = new Vector(ResultComponents);
            return Result;
        }

        public static double ProductoPunto(Vector v1, Vector v2)
        {
            double resp = 0;
            for (int i = 0; i < v1.Dimensions; i++)
            {
                resp += v1[i] * v2[i];
            }

            return resp;
        }

        public void Normalizar()
        {
            Dividir(Magnitud);
        }

        public Vector Normalizado()
        {
            double[] ResultComponents = new double[Dimensions];
            double mag = Magnitud;
            for (int i = 0; i < Dimensions; i++)
            {
                ResultComponents[i] = Components[i] / mag;
            }
            Vector Result = new Vector(ResultComponents);
            return Result;
        }

        public static Vector ProductoCruz(Vector v1, Vector v2)
        {

            double[] ResultComponents = new double[3];

            ResultComponents[0] = v1.Y * v2.Z - v2.Y * v1.Z;
            ResultComponents[1] = v2.X * v1.Z - v1.X * v2.Z;
            ResultComponents[2] = v1.X * v2.Y - v2.X * v1.Y;

            Vector Result = new Vector(ResultComponents);
            return Result;
        }

        public double Get(string dimention)
        {
            if (dimention.Equals("x")) return X;
            else if (dimention.Equals("y")) return Y;
            else if (dimention.Equals("z")) return Z;
            else throw new Exception("No es un parametro valido");
        }
    }
}
