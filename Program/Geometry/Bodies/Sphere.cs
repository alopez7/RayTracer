using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorGeometry;
using Materials;

namespace Geometry
{
    public class Sphere: Body
    {
        public double Radius;
        private Vector Velocity;
        private Vector Position;

        public Sphere(Dictionary<string, dynamic> dict, Dictionary<string, Material> materiales, Dictionary<string, Texture> textures)
        {
            try {Velocity = new Vector(ParseVect(dict, "velocity"));}
            catch {Velocity = new Vector(0, 0, 0);}
            Radius = dict["radius"];
            Position = new Vector(ParseVect(dict, "position"));

            List<Texture> Textureslist = new List<Texture>();
            List<brdf> brdfmatlist = new List<brdf>();
            List<Mirror> Mirrormatlist = new List<Mirror>();
            List<Dielectric> Dielectricmatlist = new List<Dielectric>();
            for (int i = 0; i < dict["materials"].Count; i++)
            {
                try
                {
                    if (materiales[dict["materials"][i].ToString()] is brdf)
                    {
                        brdfmatlist.Add(materiales[dict["materials"][i].ToString()]);
                    }
                    else if (materiales[dict["materials"][i].ToString()] is Mirror)
                    {
                        Mirrormatlist.Add(materiales[dict["materials"][i].ToString()]);
                    }
                    else if (materiales[dict["materials"][i].ToString()] is Dielectric)
                    {
                        Dielectricmatlist.Add(materiales[dict["materials"][i].ToString()]);
                    }
                }
                catch
                {
                    Textureslist.Add(textures[dict["materials"][i].ToString()]);
                }
            }
            brdfMaterials = brdfmatlist.ToArray();
            MirrorMaterials = Mirrormatlist.ToArray();
            DielectricMaterials = Dielectricmatlist.ToArray();
            Textures = Textureslist.ToArray();
        }

        private double[] ParseVect(Dictionary<string, dynamic> dic, string key)
        {
            double[] arr = new double[3];
            for (int i = 0; i < 3; i++)
            {
                arr[i] = dic[key][i];
            }
            return arr;
        }

        //Interseccion esfera-rayo
        public override void Intersect(Ray rayo)
        {
            //Calculo a, b y c de la ecuacion cuadratica
            double a = 1;
            double b = 2 * (Vector.ProductoPunto(rayo.Position, rayo.Direction) - Vector.ProductoPunto(GetPosition(rayo), rayo.Direction));
            double c = -2 * (Vector.ProductoPunto(rayo.Position, GetPosition(rayo))) + Vector.ProductoPunto(GetPosition(rayo), GetPosition(rayo)) + Vector.ProductoPunto(rayo.Position, rayo.Position) - Math.Pow(Radius,2);
            
            //Si el discriminante es positivo prosigo
            if (Math.Pow(b,2) >= 4 * a * c)
            {
                //Calculo las intersecciones
                double d1 = (-b + Math.Sqrt(Math.Pow(b, 2) - 4 * a * c)) / (2 * a);
                double d2 = (-b - Math.Sqrt(Math.Pow(b, 2) - 4 * a * c)) / (2 * a);
                double min;

                //Veo cual es la minima
                if (d1 >= 0 && d2 >= 0)
                {
                    min = Math.Min(d1, d2);
                }
                else if (d1 >= 0)
                {
                    min = d1;
                }
                else if (d2 >= 0)
                {
                    min = d2;
                }
                //Si ambas son negativas las descarto
                else
                {
                    return;
                }

                //Si alguna es positiva la comparo con la interseccion guardada en el rayo
                if (min < rayo.IntersectionDistance)
                {
                    rayo.IntersectionDistance = min;
                    rayo.LastIntersection = this;
                    rayo.IntersectionPoint = (rayo.Position + (rayo.Direction * rayo.IntersectionDistance));
                    rayo.IntersectionNormal = GetNormal(rayo);
                }
            }
        }

        public override Color GetTextureColor(Ray rayo, Texture texture)
        {
            Vector P = rayo.IntersectionPoint;
            double Theta = Math.Acos((P.Y - Position.Y) / Radius);
            double Psi = Math.Atan2(P.Z - Position.Z, P.X - Position.X);
            double u = 1 - ((Psi + Math.PI) / (2 * Math.PI));
            double v = (Math.PI - Theta) / Math.PI;
            return texture.GetTextureColor(u, v); 
        }

        public Vector GetNormal(Ray rayo)
        {
            Vector intPoint = (rayo.Position + (rayo.Direction * rayo.IntersectionDistance));
            Vector normal = (intPoint - GetPosition(rayo)).Normalizado();
            if ((rayo.Position - GetPosition(rayo)).Magnitud < Radius)
            {
                return (normal * -1);
            }
            return normal;
        }

        public Vector GetPosition(Ray rayo)
        {
            if (Velocity.Magnitud != 0)
            {
                return (Position + Velocity * rayo.Time);
            }
            return Position;
        }
    }
}
