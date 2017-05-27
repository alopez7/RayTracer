using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorGeometry;
using Materials;
using Illumination;

namespace Geometry
{
    public class Lockable
    {
        public int IDGenerator;

        public Lockable(int i)
        {
            IDGenerator = i;
        }

        public int GetID()
        {
            return IDGenerator++;
        }
    }

    public class Ray
    {
        static Lockable IDGenerator = new Lockable(0);
        public Vector Position;
        public Vector Direction;
        public Vector IntersectionPoint;
        public Body LastIntersection;
        public double IntersectionDistance;
        public Vector IntersectionNormal;
        public int Recursion;
        public double IR;
        public bool IN;
        public double Time;
        public int ID;

        public Ray(Vector pos, Vector dir, int recursion, double ir, bool dentro, double time)
        {
            Position = pos;
            Direction = dir;
            Direction.Normalizar();
            LastIntersection = null;
            IntersectionDistance = double.PositiveInfinity;
            IntersectionPoint = null;
            IntersectionNormal = null;
            Recursion = recursion;
            IR = ir;
            IN = dentro;
            Time = time;
            lock (IDGenerator)
            {
                ID = IDGenerator.GetID();
            }
        }

        public void Intersect(Body cuerpo)
        {
            cuerpo.Intersect(this);
        }

        public Color Cast(Body[] cuerpos, Color back_color, Light[] lights, Color AmbLight)
        {
            for (int i = 0; i < cuerpos.Length; i++)
            {
                Intersect(cuerpos[i]);
            }

            if (LastIntersection == null)
            {
                return back_color;
            }
            else
            {
                Color debuguer = GetColor(lights, AmbLight, cuerpos, back_color);
                return debuguer;
            }
        }

        public Color GetColor(Light[] lights, Color AmbLight, Body[] Cuerpos, Color back_color)
        {
            return LastIntersection.GetColor(this, lights, AmbLight, Cuerpos, back_color);
        }
    }
}
