using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorGeometry;
using Materials;

namespace Geometry
{
    public class Triangle : Body
    {
        private Vertex[] Vertices;
        public bool CVN; //Compute Vertex normals
        public bool BFC;
        public Vector PlaneNormal;
        public Vector Center;
        public Vector Min;
        public Vector Max;
        public Vertex[] V
        {
            get
            {
                return Vertices;
            }
        }
        public Vertex V1
        {
            get
            {
                return Vertices[0];
            }
        }
        public Vertex V2
        {
            get
            {
                return Vertices[1];
            }
        }
        public Vertex V3
        {
            get
            {
                return Vertices[2];
            }
        }

        public Triangle(Vertex v1, Vertex v2, Vertex v3, brdf[] brdfmat, Mirror[] mirrormat, Dielectric[] dielectricmat, Texture[] textures, bool cvn, bool bfc)
        {
            CVN = cvn;
            BFC = bfc;
            brdfMaterials = brdfmat;
            MirrorMaterials = mirrormat;
            DielectricMaterials = dielectricmat;
            Textures = textures;
            Vertices = new Vertex[3];
            Vertices[0] = v1;
            Vertices[1] = v2;
            Vertices[2] = v3;
            CalculatePlaneNormal();
            Center = ((V1.pos + V2.pos + V3.pos) / 3);
            Min = new Vector(0, 0, 0);
            Min.X = Math.Min(Math.Min(V1.pos.X, V2.pos.X), V3.pos.X);
            Min.Y = Math.Min(Math.Min(V1.pos.Y, V2.pos.Y), V3.pos.Y);
            Min.Z = Math.Min(Math.Min(V1.pos.Z, V2.pos.Z), V3.pos.Z);
            Max = new Vector(0, 0, 0);
            Max.X = Math.Max(Math.Max(V1.pos.X, V2.pos.X), V3.pos.X);
            Max.Y = Math.Max(Math.Max(V1.pos.Y, V2.pos.Y), V3.pos.Y);
            Max.Z = Math.Max(Math.Max(V1.pos.Z, V2.pos.Z), V3.pos.Z);

        }

        private void CalculatePlaneNormal()
        {
            PlaneNormal = Vector.ProductoCruz((V2.pos - V1.pos), (V3.pos - V1.pos));
        }

        public override void Intersect(Ray rayo)
        {
            if (RayBoxIntersect(rayo)) return;

            if (BFC && DielectricMaterials.Length == 0 && Vector.ProductoPunto(rayo.Direction, PlaneNormal) > 0) return;

            Vector a = (rayo.Direction * -1);
            Vector b = (V2.pos - V1.pos);
            Vector c = (V3.pos - V1.pos);
            Vector d = (rayo.Position - V1.pos);

            double D = Determinante(a, b, c);
            if (D == 0) return;
            double invD = 1 / D;

            double D2 = Determinante(a, d, c);
            double Beta = D2 * invD;
            if (Beta < 0 || Beta > 1 + 0.0005) return;

            double D3 = Determinante(a, b, d);
            double Gamma = D3 * invD;
            if (Gamma < 0 || Gamma + Beta > 1 + 0.0005) return;

            double Alpha = 1 - Beta - Gamma;

            double D1 = Determinante(d, b, c);
            double T = D1 * invD;
            if (T < rayo.IntersectionDistance && T > 0)
            {
                rayo.IntersectionDistance = T;
                rayo.IntersectionPoint = (rayo.Direction * T) + rayo.Position;
                rayo.LastIntersection = this;
                rayo.IntersectionNormal = (V1.Normal * Alpha + V2.Normal * Beta + V3.Normal * Gamma).Normalizado();
            }
        }

        public override Color GetTextureColor(Ray rayo, Texture texture)
        {
            Vector a = rayo.Direction * -1;
            Vector b = V2.pos - V1.pos;
            Vector c = V3.pos - V1.pos;
            Vector d = rayo.Position - V1.pos;

            double D = Determinante(a, b, c);
            double invD = 1 / D;

            double D2 = Determinante(a, d, c);
            double Beta = D2 * invD;

            double D3 = Determinante(a, b, d);
            double Gamma = D3 * invD;

            double Alpha = 1 - Beta - Gamma;

            double u = Alpha * V1.TexPosition[0] + Beta * V2.TexPosition[0] + Gamma * V3.TexPosition[0];
            double v = Alpha * V1.TexPosition[1] + Beta * V2.TexPosition[1] + Gamma * V3.TexPosition[1];

            return texture.GetTextureColor(u, v);

        }

        private double Determinante(Vector a, Vector b, Vector c)
        {
            double s1 = a.X * (b.Y * c.Z - b.Z * c.Y);
            double s2 = -b.X * (a.Y * c.Z - a.Z * c.Y);
            double s3 = c.X * (a.Y * b.Z - a.Z * b.Y);
            return s1 + s2 + s3;
        }

        private bool RayBoxIntersect(Ray rayo)
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
            return tmax >= tmin;
        }
    }
}
