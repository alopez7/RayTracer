using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorGeometry;

namespace Geometry
{

    public class KDTree
    {
        Triangle[] Triangles;
        Vector CMin;
        Vector CMax;
        Vector Min;
        Vector Max;
        KDTree Left;
        KDTree Right;
        bool Leaf;

        public KDTree(Triangle[] triangles, Vector[] Dimentions)
        {
            SetParameters(triangles, Dimentions);
        }

        public KDTree(Triangle[] triangles)
        {
            Vector[] Dimentions = CalculateDimentions(triangles);
            SetParameters(triangles, Dimentions);
        }

        public void SetParameters(Triangle[] triangles, Vector[] Dimentions)
        {
            Min = Dimentions[0];
            Max = Dimentions[1];
            CMin = Dimentions[2];
            CMax = Dimentions[3];

            if (triangles.Length <= 20)
            {
                Leaf = true;
                Triangles = triangles;
                Left = null;
                Right = null;
            }
            else
            {
                Leaf = false;
                Triangles = null;
                Split(triangles);
            }
        }

        public Vector[] CalculateDimentions(Triangle[] triangles)
        {
            Vector min = new Vector(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
            Vector max = new Vector(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);
            Vector cmin = new Vector(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
            Vector cmax = new Vector(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);

            foreach (Triangle tri in triangles)
            {
                min.X = Math.Min(min.X, tri.Min.X);
                min.Y = Math.Min(min.Y, tri.Min.Y);
                min.Z = Math.Min(min.Z, tri.Min.Z);
                max.X = Math.Max(max.X, tri.Max.X);
                max.Y = Math.Max(max.Y, tri.Max.Y);
                max.Z = Math.Max(max.Z, tri.Max.Z);

                cmin.X = Math.Min(cmin.X, tri.Center.X);
                cmin.Y = Math.Min(cmin.Y, tri.Center.Y);
                cmin.Z = Math.Min(cmin.Z, tri.Center.Z);
                cmax.X = Math.Max(cmax.X, tri.Center.X);
                cmax.Y = Math.Max(cmax.Y, tri.Center.Y);
                cmax.Z = Math.Max(cmax.Z, tri.Center.Z);
            }
            Vector[] resp = new Vector[4];
            resp[0] = min;
            resp[1] = max;
            resp[2] = cmin;
            resp[3] = cmax;
            return resp;
        }

        public void Split(Triangle[] triangles)
        {
            //Es el largo de cada dimension
            Vector large = CMax - CMin;
            string CutDimention;
            if (large.X >= large.Y && large.X >= large.Z) CutDimention = "x";
            else if (large.Y > large.X && large.Y >= large.Z) CutDimention = "y";
            else CutDimention = "z";

            //Calculo la mediana ordenando
            //Ordeno
            Triangle[] sorted;
            sorted = triangles.OrderBy(tri => tri.Center.Get(CutDimention)).ToArray();
            //Encuentro mediana
            Triangle Median = sorted[sorted.Length / 2];

            //Creo arreglos nuevos y los relleno
            List<Triangle> ListIzquierdos = new List<Triangle>();
            List<Triangle> ListDerechos = new List<Triangle>();
            Vector Imin = new Vector(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
            Vector Imax = new Vector(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);
            Vector Icmin = new Vector(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
            Vector Icmax = new Vector(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);
            Vector Dmin = new Vector(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
            Vector Dmax = new Vector(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);
            Vector Dcmin = new Vector(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
            Vector Dcmax = new Vector(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);
            foreach (Triangle tri in triangles)
            {
                if (tri.Min.Get(CutDimention) <= Median.Center.Get(CutDimention))
                {
                    ListIzquierdos.Add(tri);
                    Imin.X = Math.Min(Imin.X, tri.Min.X);
                    Imin.Y = Math.Min(Imin.Y, tri.Min.Y);
                    Imin.Z = Math.Min(Imin.Z, tri.Min.Z);
                    Imax.X = Math.Max(Imax.X, tri.Max.X);
                    Imax.Y = Math.Max(Imax.Y, tri.Max.Y);
                    Imax.Z = Math.Max(Imax.Z, tri.Max.Z);
                    Icmin.X = Math.Min(Icmin.X, tri.Center.X);
                    Icmin.Y = Math.Min(Icmin.Y, tri.Center.Y);
                    Icmin.Z = Math.Min(Icmin.Z, tri.Center.Z);
                    Icmax.X = Math.Max(Icmax.X, tri.Center.X);
                    Icmax.Y = Math.Max(Icmax.Y, tri.Center.Y);
                    Icmax.Z = Math.Max(Icmax.Z, tri.Center.Z);
                }
                if (tri.Max.Get(CutDimention) >= Median.Center.Get(CutDimention))
                {
                    ListDerechos.Add(tri);
                    Dmin.X = Math.Min(Dmin.X, tri.Min.X);
                    Dmin.Y = Math.Min(Dmin.Y, tri.Min.Y);
                    Dmin.Z = Math.Min(Dmin.Z, tri.Min.Z);
                    Dmax.X = Math.Max(Dmax.X, tri.Max.X);
                    Dmax.Y = Math.Max(Dmax.Y, tri.Max.Y);
                    Dmax.Z = Math.Max(Dmax.Z, tri.Max.Z);
                    Dcmin.X = Math.Min(Dcmin.X, tri.Center.X);
                    Dcmin.Y = Math.Min(Dcmin.Y, tri.Center.Y);
                    Dcmin.Z = Math.Min(Dcmin.Z, tri.Center.Z);
                    Dcmax.X = Math.Max(Dcmax.X, tri.Center.X);
                    Dcmax.Y = Math.Max(Dcmax.Y, tri.Center.Y);
                    Dcmax.Z = Math.Max(Dcmax.Z, tri.Center.Z);
                }
            }
            if (Math.Max(ListIzquierdos.Count, ListDerechos.Count) >= triangles.Length * 0.75)
            {
                Leaf = true;
                Triangles = triangles;
                Left = null;
                Right = null;
                return;
            }
            Vector[] IDimentions = new Vector[4];
            Vector[] DDimentions = new Vector[4];
            IDimentions[0] = Imin;
            IDimentions[1] = Imax;
            IDimentions[2] = Icmin;
            IDimentions[3] = Icmax;
            DDimentions[0] = Dmin;
            DDimentions[1] = Dmax;
            DDimentions[2] = Dcmin;
            DDimentions[3] = Dcmax;
            Triangle[] Izquierdos = ListIzquierdos.ToArray();
            Triangle[] Derechos = ListDerechos.ToArray();
            Left = new KDTree(Izquierdos, IDimentions);
            Right = new KDTree(Derechos, DDimentions);
        }

        public void Intersect(Ray rayo)
        {
            if (Leaf)
            {
                foreach (Triangle tri in Triangles)
                {
                    tri.Intersect(rayo);
                }
            }
            else
            {
                double dr = Right.RayBoxIntersect(rayo);
                double dl = Left.RayBoxIntersect(rayo);
                if (dr != -1 && dl != -1)
                {
                    if (dr < dl)
                    {
                        Body anterior = rayo.LastIntersection;
                        Right.Intersect(rayo);
                        if (rayo.LastIntersection != anterior)
                        {
                            if (Left.TriangleBoxIntersect((Triangle) rayo.LastIntersection))
                            {
                                Left.Intersect(rayo);
                            }
                        }
                        else
                        {
                            Left.Intersect(rayo);
                        }
                    }
                    else
                    {
                        Body anterior = rayo.LastIntersection;
                        Left.Intersect(rayo);
                        if (rayo.LastIntersection != anterior)
                        {
                            if (Right.TriangleBoxIntersect((Triangle)rayo.LastIntersection))
                            {
                                Right.Intersect(rayo);
                            }
                        }
                        else
                        {
                            Right.Intersect(rayo);
                        }
                    }
                }
                else if (dr == -1 && dl != -1)
                {
                    Left.Intersect(rayo);
                }
                else if (dr != -1 && dl == -1)
                {
                    Right.Intersect(rayo);
                }
            }
        }

        private double RayBoxIntersect(Ray rayo)
        {
            double tx1 = (Min.X - rayo.Position.X) / rayo.Direction.X;
            double tx2 = (Max.X - rayo.Position.X) / rayo.Direction.X;

            double tmin = Math.Min(tx1, tx2);
            double tmax = Math.Max(tx1, tx2);

            double ty1 = (Min.Y - rayo.Position.Y) / rayo.Direction.Y;
            double ty2 = (Max.Y - rayo.Position.Y) / rayo.Direction.Y;

            tmin = Math.Max(tmin, Math.Min(ty1, ty2));
            tmax = Math.Min(tmax, Math.Max(ty1, ty2));

            double tz1 = (Min.Z - rayo.Position.Z) / rayo.Direction.Z;
            double tz2 = (Max.Z - rayo.Position.Z) / rayo.Direction.Z;

            tmin = Math.Max(tmin, Math.Min(tz1, tz2));
            tmax = Math.Min(tmax, Math.Max(tz1, tz2));
            if (tmax >= tmin)
            {
                return tmin;
            }
            return -1;
                
        }

        private bool TriangleBoxIntersect(Triangle tri)
        {
            if (tri.Min.X > Max.X) return false;
            if (tri.Min.Y > Max.Y) return false;
            if (tri.Min.Z > Max.Z) return false;
            if (tri.Max.X < Min.X) return false;
            if (tri.Max.Y < Min.Y) return false;
            if (tri.Max.Z < Min.Z) return false;
            return true;
        }
    }
}
