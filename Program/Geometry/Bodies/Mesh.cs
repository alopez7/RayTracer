using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using VectorGeometry;
using Materials;
using System.Globalization;

namespace Geometry
{
    public class Mesh : Body
    {
        public Triangle[] Triangles;
        public KDTree Kdtree;
        public bool ComputeVertexNormals;
        public bool BFC;
        public bool BoundingBox;
        public Vector MaxPos;
        public Vector MinPos;

        public Mesh(Dictionary<string, dynamic> dict, Dictionary<string, Material> materiales, Dictionary<string, Texture> textures, bool bfc, bool boundingbox)
        {
            BFC = bfc;
            BoundingBox = boundingbox;
            MinPos = new Vector(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
            MaxPos = new Vector(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);

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
            Console.WriteLine("Creando Triángulos");
            try
            {
                ComputeVertexNormals = dict["compute_vertex_normals"];
            }
            catch
            {
                ComputeVertexNormals = false;
            }
            Triangles = TriangleParser(dict["file_path"]);
            Console.WriteLine("Triangulos: {0}", Triangles.Length);
            Kdtree = new KDTree(Triangles);
        }

        public override void Intersect(Ray rayo)
        {
            Kdtree.Intersect(rayo);
            /*
            if (RayBoxIntersect(rayo))
            {
                foreach (Triangle tri in Triangles)
                {
                    tri.Intersect(rayo);
                }
            }
            */
        }

        public override Color GetTextureColor(Ray rayo, Texture texture)
        {
            throw new NotImplementedException();
        }

        public Triangle[] TriangleParser(string path)
        {
            //Leer el archivo al mesh
            StreamReader file = new StreamReader(path);

            //Creo las listas de triangulos, vertices y normales
            List<Triangle> triangles = new List<Triangle>();
            int t_count = 0;
            Dictionary<int, Vertex> vertexs = new Dictionary<int, Vertex>();
            int v_count = 1;
            Dictionary<int, Vector> normals = new Dictionary<int, Vector>();
            int n_count = 1;
            Dictionary<int, Vector> textures = new Dictionary<int, Vector>();
            int te_count = 1;

            //Creo los Vertices, normales, y triangulos
            Vertex v1;
            Vertex v2;
            Vertex v3;
            string[] sv1;
            string[] sv2;
            string[] sv3;
            double[] nums;
            string line;
            bool NormalGiven = true;
            while ((line = file.ReadLine()) != null)
            {
                string[] linea = line.Split(' ');
                switch (linea[0])
                {
                    case "v":
                        //Creo el vertice solo con posicion
                        nums = new double[3];
                        for (int i = 0; i < 3; i++)
                        {
                            nums[i] = double.Parse(linea[i + 1].Replace('.',','));
                        }
                        vertexs.Add(v_count, new Vertex(new Vector(nums)));
                        if (nums[0] < MinPos[0]) MinPos[0] = nums[0];
                        if (nums[1] < MinPos[1]) MinPos[1] = nums[1];
                        if (nums[2] < MinPos[2]) MinPos[2] = nums[2];
                        if (nums[0] > MaxPos[0]) MaxPos[0] = nums[0];
                        if (nums[1] > MaxPos[1]) MaxPos[1] = nums[1];
                        if (nums[2] > MaxPos[2]) MaxPos[2] = nums[2];
                        v_count++;
                        break;

                    case "vt":
                        //Creo los pares u,v (texture mapping)
                        nums = new double[2];
                        for (int i = 0; i < 2; i++)
                        {
                            nums[i] = double.Parse(linea[i + 1].Replace('.', ','));
                        }
                        textures.Add(te_count, new Vector(nums));
                        te_count++;
                        break;

                    case "vn":
                        //Creo las normales (si hay)
                        nums = new double[3];
                        for (int i = 0; i < 3; i++)
                        {
                            nums[i] = double.Parse(linea[i + 1].Replace('.', ','));
                        }
                        normals.Add(n_count, new Vector(nums));
                        n_count++;
                        break;

                    case "f":
                        //Creo los triangulos
                        sv1 = linea[1].Split('/');
                        sv2 = linea[2].Split('/');
                        sv3 = linea[3].Split('/');
                        //Le asigno vertices al triangulo
                        v1 = vertexs[int.Parse(sv1[0])];
                        v2 = vertexs[int.Parse(sv2[0])];
                        v3 = vertexs[int.Parse(sv3[0])];
                        //Le asigno normales al vertice (Si no hay, se tiene que normalGiven = false)
                        //Le asigno u,v de textura (Si hay)
                        if (sv1.Length == 3)
                        {
                            if (sv1[2].Length != 0)
                            {
                                v1.Normal = normals[int.Parse(sv1[2])];
                                v2.Normal = normals[int.Parse(sv2[2])];
                                v3.Normal = normals[int.Parse(sv3[2])];
                            }
                            if (sv1[1].Length != 0)
                            {
                                v1.TexPosition = textures[int.Parse(sv1[1])];
                                v2.TexPosition = textures[int.Parse(sv2[1])];
                                v3.TexPosition = textures[int.Parse(sv3[1])];
                            }
                        }
                        else
                        {
                            NormalGiven = false;
                        }
                        triangles.Add(new Triangle(v1, v2, v3, brdfMaterials, MirrorMaterials, DielectricMaterials, Textures, ComputeVertexNormals, BFC));
                        //Agrego el índice del tripangulo al vértice para que pueda calcular su normal (si es necesario)
                        v1.Triangle_IDs.Add(t_count);
                        v2.Triangle_IDs.Add(t_count);
                        v3.Triangle_IDs.Add(t_count);
                        t_count++;
                        break;
                }
            }
            //Si los vertices no tienen normal y la necesitan, se calcula
            Triangle[] triArray = triangles.ToArray();
            if (ComputeVertexNormals || !NormalGiven)
            {
                Console.WriteLine("Calculando las normales");
                Vertex V;
                Vector norm;
                for (int i = 1; i < v_count; i++)
                {
                    V = vertexs[i];
                    norm = new Vector(0, 0, 0);
                    foreach (int j in V.Triangle_IDs)
                    {
                        norm = (norm + triArray[j].PlaneNormal);
                    }
                    if (norm.Magnitud != 0)
                    {
                        norm.Normalizar();
                    }
                    V.Normal = norm;
                }
            }

            foreach (Triangle tr in triArray)
            {
                tr.PlaneNormal.Normalizar();
            }
            return triArray;
        }

        private bool RayBoxIntersect(Ray rayo)
        {
            double tx1 = (MinPos.X - rayo.Position.X) / rayo.Direction.X;
            double tx2 = (MaxPos.X - rayo.Position.X) / rayo.Direction.X;

            double tmin = Math.Min(tx1, tx2);
            double tmax = Math.Max(tx1, tx2);

            double ty1 = (MinPos.Y - rayo.Position.Y) / rayo.Direction.Y;
            double ty2 = (MaxPos.Y - rayo.Position.Y) / rayo.Direction.Y;

            tmin = Math.Max(tmin, Math.Min(ty1, ty2));
            tmax = Math.Min(tmax, Math.Max(ty1, ty2));

            double tz1 = (MinPos.Z - rayo.Position.Z) / rayo.Direction.Z;
            double tz2 = (MaxPos.Z - rayo.Position.Z) / rayo.Direction.Z;

            tmin = Math.Max(tmin, Math.Min(tz1, tz2));
            tmax = Math.Min(tmax, Math.Max(tz1, tz2));
            return tmax >= tmin;
        }
    }
}
