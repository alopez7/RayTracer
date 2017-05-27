using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using Newtonsoft.Json;
using System.Drawing.Imaging;
using Geometry;
using Materials;
using Illumination;
using VectorGeometry;

namespace RayTracer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Tiempo total
            var watch = System.Diagnostics.Stopwatch.StartNew();
            //Veo los flags del input
            int Width = 0;
            int Height = 0;
            string SceneFile = "";
            string ResourcesFile = "";
            string OutputFile = "";
            bool BFC = false;
            bool BoundingBox = false;
            int RaysPerPixel = 1;
            bool AdadptativeAntialiasing = false;
            bool CommonAdadptativeAntialiasing = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i][0].Equals('-'))
                {
                    if (args[i][1].Equals('s')) SceneFile = args[i + 1];
                    else if (args[i][1].Equals('r')) ResourcesFile = args[i + 1];
                    else if (args[i][1].Equals('w')) Width = int.Parse(args[i + 1]);
                    else if (args[i][1].Equals('h')) Height = int.Parse(args[i + 1]);
                    else if (args[i][1].Equals('i')) OutputFile = args[i + 1];
                    else if (args[i][1].Equals('b') && args[i][2].Equals('f')) BFC = true;
                    else if (args[i][1].Equals('b') && args[i][2].Equals('b')) BoundingBox = true;
                    else if (args[i][1].Equals('p')) RaysPerPixel = int.Parse(args[i + 1]);
                    else if (args[i][1].Equals('a')) AdadptativeAntialiasing = true;
                    else if (args[i][1].Equals('c')) CommonAdadptativeAntialiasing = true;
                }
            }
            Console.WriteLine("Parseando los datos");
            //Parseo el .Json en un diccionario
            Dictionary<string, dynamic> values = FileParser(SceneFile);
            //Parseo el .Json de los materiales
            Dictionary<string, dynamic> mate = FileParser(ResourcesFile);
            //Creo la escena
            Scene scen = Parser(Width, Height, values, mate, BFC, BoundingBox);

            //Creo la imagen
            Materials.Color[,] image;
            //Con Adaptative Antialiasing comun
            if (CommonAdadptativeAntialiasing)
            {
                Console.WriteLine("Creando imagen previa");
                var prewatch = System.Diagnostics.Stopwatch.StartNew();

                Materials.Color[,] preimage = scen.Generate_Image(1);

                prewatch.Stop();
                double preelapsedMs = ((double)prewatch.ElapsedMilliseconds) / 1000;
                Console.WriteLine("Tiempo generando imagen: " + preelapsedMs + "s");

                var postwatch = System.Diagnostics.Stopwatch.StartNew();
                Console.WriteLine("Obteniendo pixeles criticos");
                List<CriticPixel> Critics = GetCommonCriticPixels(preimage, (int)Math.Sqrt(RaysPerPixel));
                image = scen.GenerateAdaptativeImage(preimage, Critics);

                postwatch.Stop();
                double postelapsedMs = ((double)postwatch.ElapsedMilliseconds) / 1000;
                Console.WriteLine("Tiempo generando Anti Aliasing: " + postelapsedMs + "s");
            }
            //Con Adaptative Antialiasing
            else if (AdadptativeAntialiasing)
            {
                Console.WriteLine("Creando imagen previa");
                var prewatch = System.Diagnostics.Stopwatch.StartNew();

                Materials.Color[,] preimage = scen.Generate_Image(1);
                Console.WriteLine("Guardando premagen");

                prewatch.Stop();
                double preelapsedMs = ((double)prewatch.ElapsedMilliseconds) / 1000;
                Console.WriteLine("Tiempo generando imagen: " + preelapsedMs + "s");

                var postwatch = System.Diagnostics.Stopwatch.StartNew();
                Console.WriteLine("Obteniendo pixeles criticos");
                List<CriticPixel> Critics = GetCriticPixels(preimage, (int)Math.Sqrt(RaysPerPixel));
                image = scen.GenerateAdaptativeImage(preimage, Critics);

                postwatch.Stop();
                double postelapsedMs = ((double)postwatch.ElapsedMilliseconds) / 1000;
                Console.WriteLine("Tiempo generando Anti Aliasing: " + postelapsedMs + "s");
            }
            //Sin Adaptative Antialiasing
            else
            {
                Console.WriteLine("Creando imagen");
                var prewatch = System.Diagnostics.Stopwatch.StartNew();

                image = scen.Generate_Image(RaysPerPixel);

                prewatch.Stop();
                double preelapsedMs = ((double)prewatch.ElapsedMilliseconds) / 1000;
                Console.WriteLine("Tiempo generando imagen: " + preelapsedMs + "s");

            }

            //Guardo la imagen en el archivo de salida
            Console.WriteLine("Guardando Imagen");
            WriteImage(image, OutputFile);

            watch.Stop();
            double elapsedMs = ((double)watch.ElapsedMilliseconds) / 1000;
            Console.WriteLine("Tiempo Total: " + elapsedMs + "s");
            Console.WriteLine("Presione cualquier tecla para terminar");
            Console.ReadKey();
        }

        static Scene Parser(int Width, int Height, Dictionary<string, dynamic> values, Dictionary<string, dynamic> mate, bool BFC, bool BoundingBox)
        {
            //Creo la camara
            Camera cam = new Camera(values, Width, Height);
            //Creo diccionario de materiales
            Dictionary<string, Material> materials = new Dictionary<string, Material>();
            //Creo diccionario de texture_name -> path
            Dictionary<string, string> TexturePaths = new Dictionary<string, string>();
            //Creo diccionario de texturas
            Dictionary<string, Texture> textures = new Dictionary<string, Texture>();


            //Creo las relaciones texture_name -> path
            string name;
            string path;
            for (int i = 0; i < mate["textures"].Count; i++)
            {
                name = mate["textures"][i]["name"];
                path = mate["textures"][i]["file_path"];
                TexturePaths.Add(name, path);
            }

            //Creo los Materials
            string mateType;
            string brdf;
            Material m;
            Texture t;
            for (int i = 0; i < mate["materials"].Count; i++)
            {
                mateType = mate["materials"][i]["__type__"];
                if (mateType.Equals("brdf_material"))
                {
                    brdf = mate["materials"][i]["brdf"];
                    if (brdf.Equals("lambert"))
                    {
                        m = new Lambert(JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(mate["materials"][i].ToString()));
                        materials.Add(m.Name, m);
                    }
                    else if (brdf.Equals("blinnPhong"))
                    {
                        m = new BlinnPhong(JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(mate["materials"][i].ToString()));
                        materials.Add(m.Name, m);
                    }
                    else if (brdf.Equals("toon"))
                    {
                        m = new Toon(JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(mate["materials"][i].ToString()));
                        materials.Add(m.Name, m);
                    }
                    else if (brdf.Equals("experiment"))
                    {
                        m = new Experiment(JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(mate["materials"][i].ToString()));
                        materials.Add(m.Name, m);
                    }
                }
                else if (mateType.Equals("reflective_material"))
                {
                    m = new Mirror(JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(mate["materials"][i].ToString()));
                    materials.Add(m.Name, m);
                }
                else if (mateType.Equals("dielectric_material"))
                {
                    m = new Dielectric(JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(mate["materials"][i].ToString()));
                    materials.Add(m.Name, m);
                }
                else if (mateType.Equals("brdf_color_texture_material"))
                {
                    string brdfTexture = mate["materials"][i]["brdf"];
                    if (brdfTexture.Equals("lambert"))
                    {
                        t = new LambertTexture(JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(mate["materials"][i].ToString()));
                        t.Path = TexturePaths[t.ColorTexture];
                        textures.Add(t.Name, t);
                    }
                    if (brdfTexture.Equals("blinnPhong"))
                    {
                        t = new BlinnPhongTexture(JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(mate["materials"][i].ToString()));
                        t.Path = TexturePaths[t.ColorTexture];
                        textures.Add(t.Name, t);
                    }
                }
            }

            //Creo lista de luces
            List<Light> lights = new List<Light>();

            Materials.Color AmbLight = new Materials.Color(0, 0, 0);

            //Creo las Luces
            string type;
            for (int i = 0; i < values["lights"].Count; i++)
            {
                type = values["lights"][i]["__type__"];
                if (type.Equals("directional_light"))
                {
                    lights.Add(new Directional(JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(values["lights"][i].ToString())));
                }
                else if (type.Equals("point_light"))
                {
                    lights.Add(new Punctual(JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(values["lights"][i].ToString())));
                }
                else if (type.Equals("ambient_light"))
                {
                    AmbLight.R = values["lights"][i]["color"][0];
                    AmbLight.G = values["lights"][i]["color"][1];
                    AmbLight.B = values["lights"][i]["color"][2];
                }
                else if (type.Equals("area_light"))
                {
                    lights.Add(new AreaLight(JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(values["lights"][i].ToString())));
                }
            }
            Light[] lig = lights.ToArray();

            //Creo arreglo de Bodies
            List<Body> lista = new List<Body>();
            //Creo los Bodies
            for (int i = 0; i < values["objects"].Count; i++)
            {
                if (values["objects"][i]["__type__"] == "sphere")
                {
                    lista.Add(new Sphere(JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(values["objects"][i].ToString()), materials, textures));
                }
                else if (values["objects"][i]["__type__"] == "mesh")
                {
                    lista.Add(new Mesh(JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(values["objects"][i].ToString()), materials, textures, BFC, BoundingBox));
                }
            }
            //Creo la escena
            Scene scen = new Scene(lista.ToArray(), lig, cam, JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(values["params"].ToString()), AmbLight);

            return scen;
        }

        static Dictionary<string, dynamic> FileParser(string path)
        {
            Dictionary<string, dynamic> values = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(File.ReadAllText(path));
            return values;
        }

        static void WriteImage(Materials.Color[,] color_matrix, string path)
        {
            int height = color_matrix.GetLength(0);
            int width = color_matrix.GetLength(1);
            Bitmap map = new Bitmap(width, height);
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    color_matrix[row, col].Truncate();

                    int R = (int)(color_matrix[row, col].R * 255);
                    int G = (int)(color_matrix[row, col].G * 255);
                    int B = (int)(color_matrix[row, col].B * 255);

                    map.SetPixel(col, row, System.Drawing.Color.FromArgb(R, G, B));
                }
            }
            map.Save(path, ImageFormat.Png);
        }

        static void Normalize(Materials.Color[,] color_matrix)
        {
            int height = color_matrix.GetLength(0);
            int width = color_matrix.GetLength(1);
            Materials.Color max = new Materials.Color(0, 0, 0);
            Materials.Color min = new Materials.Color(255, 255, 255);
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (color_matrix[row, col].R < min.R) min.R = color_matrix[row, col].R;
                    if (color_matrix[row, col].G < min.G) min.G = color_matrix[row, col].G;
                    if (color_matrix[row, col].B < min.B) min.B = color_matrix[row, col].B;
                    if (color_matrix[row, col].R > max.R) max.R = color_matrix[row, col].R;
                    if (color_matrix[row, col].G > max.G) max.G = color_matrix[row, col].G;
                    if (color_matrix[row, col].B > max.B) max.B = color_matrix[row, col].B;
                }
            }
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    color_matrix[row, col] = (color_matrix[row, col] - min) * (max - min);
                }
            }
        }

        static List<CriticPixel> GetCriticPixels(Materials.Color[,] color_matrix, int MaxRaysPerDimention)
        {
            //Para generar imagen de niveles de rayos
            Materials.Color[,] niveles = new Materials.Color[color_matrix.GetLength(0), color_matrix.GetLength(1)];

            //Defino los kernels y la lista de pixeles criticos
            List<CriticPixel> Critics = new List<CriticPixel>();
            int[,] edgekernel = {
                { 0, 0, 0, 0,-1, 0, 0, 0, 0},
                { 0, 0,-1,-1,-2,-1,-1, 0, 0},
                { 0,-1,-2,-2,-3,-2,-2,-1, 0},
                { 0,-1,-2,-3,-4,-3,-2,-1, 0},
                {-1,-2,-3,-4,92,-4,-3,-2,-1},
                { 0,-1,-2,-3,-4,-3,-2,-1, 0},
                { 0,-1,-2,-2,-3,-2,-2,-1, 0},
                { 0, 0,-1,-1,-2,-1,-1, 0, 0},
                { 0, 0, 0, 0,-1, 0, 0, 0, 0} };

            double[,] blurkernel = {
                {((double)1)/((double)25),((double)1)/((double)25),((double)1)/((double)25),((double)1)/((double)25),((double)1)/((double)25)},
                {((double)1)/((double)25),((double)1)/((double)25),((double)1)/((double)25),((double)1)/((double)25),((double)1)/((double)25)},
                {((double)1)/((double)25),((double)1)/((double)25),((double)1)/((double)25),((double)1)/((double)25),((double)1)/((double)25)},
                {((double)1)/((double)25),((double)1)/((double)25),((double)1)/((double)25),((double)1)/((double)25),((double)1)/((double)25)},
                {((double)1)/((double)25),((double)1)/((double)25),((double)1)/((double)25),((double)1)/((double)25),((double)1)/((double)25)} };

            //Creo una pre imagen de pixeles criticos
            double[,] Pre_Critic = new double[color_matrix.GetLength(0), color_matrix.GetLength(1)];

            //Guardo para cada valor de rayos por pixel la cantidad de pixeles
            Dictionary<int, int> cantidades = new Dictionary<int, int>();
            for (int i = 2; i <= MaxRaysPerDimention; i++)
            {
                cantidades.Add(i, 0);
            }

            //Inicializo variables utiles
            int MAX = MaxRaysPerDimention;
            double CotaMinima = 0.3;
            double m = ((double)(MAX - 2)) / ((double)(6 - CotaMinima));
            double n = MAX - m * 6;

            //aplico el filtro edge
            Console.WriteLine("Detectando bordes");
            for (int row = 0; row < color_matrix.GetLength(0); row++)
            {
                int lenght = color_matrix.GetLength(1);


                Parallel.For(0, lenght, col =>
                {
                    Vector Suma = new Vector(0, 0, 0);
                    for (int rowk = -4; rowk < 5; rowk++)
                    {
                        for (int colk = -4; colk < 5; colk++)
                        {
                            if (edgekernel[rowk + 4, colk + 4] != 0)
                            {
                                int r = row + rowk;
                                int c = col + colk;
                                if (r < 0) r = 0;
                                else if (r >= color_matrix.GetLength(0)) r = color_matrix.GetLength(0) - 1;
                                if (c < 0) c = 0;
                                else if (c >= color_matrix.GetLength(0)) c = color_matrix.GetLength(0) - 1;
                                Suma += color_matrix[r, c] * edgekernel[rowk + 4, colk + 4];
                            }
                        }
                    }
                    //Guardo el "pixel" en pre_critic
                    Pre_Critic[row, col] = Suma.Magnitud;
                });
            }

            //APlico el filtro blur
            Console.WriteLine("Reajustando Filtro");
            for (int row = 0; row < Pre_Critic.GetLength(0); row++)
            {
                int lenght = Pre_Critic.GetLength(1);
                Parallel.For(0, lenght, col =>
               {
                   double Suma = 0;
                   for (int rowk = -2; rowk < 3; rowk++)
                   {
                       for (int colk = -2; colk < 3; colk++)
                       {
                           if (blurkernel[rowk + 2, colk + 2] != 0)
                           {
                               int r = row + rowk;
                               int c = col + colk;
                               if (r < 0) r = 0;
                               else if (r >= Pre_Critic.GetLength(0)) r = Pre_Critic.GetLength(0) - 1;
                               if (c < 0) c = 0;
                               else if (c >= Pre_Critic.GetLength(0)) c = Pre_Critic.GetLength(0) - 1;
                               Suma += Pre_Critic[r, c] * blurkernel[rowk + 2, colk + 2];
                           }
                       }
                   }
                   double Magnitud = Suma * 0.8;
                   if (Magnitud > CotaMinima)
                   {
                       niveles[row, col] = new Materials.Color(0, 1.0 / (MAX - 1), 0);
                       int rn;
                       if (Magnitud >= 6)
                       {
                           rn = MAX;
                       }
                       else
                       {

                           rn = (int)(m * Magnitud + n);
                       }
                       lock (Critics)
                       {
                           Critics.Add(new CriticPixel(row, col, rn));
                           cantidades[rn]++;
                       }
                       niveles[row, col].Ponderar(rn - 1);
                   }
                   else
                   {
                       niveles[row, col] = new Materials.Color(0,0,0);
                   }
               });
            }
            for (int i = 2; i <= MaxRaysPerDimention; i++)
            {
                Console.WriteLine("Rayos por pixel: {0}, Cantidad de pixeles: {1}", Math.Pow(i, 2), cantidades[i]);
            }
            WriteImage(niveles, "Niveles.png");
            return Critics;
        }

        static List<CriticPixel> GetCommonCriticPixels(Materials.Color[,] color_matrix, int MaxRaysPerDimention)
        {
            //Defino los kernels y la lista de pixeles criticos
            List<CriticPixel> Critics = new List<CriticPixel>();
            int[,] edgekernel = {
                {-1,-2,-1 },
                {-2,12,-2 },
                {-1,-2,-1 } };

            //Guardo para cada valor de rayos por pixel la cantidad de pixeles
            Dictionary<int, int> cantidades = new Dictionary<int, int>();
            for (int i = 2; i <= MaxRaysPerDimention; i++)
            {
                cantidades.Add(i, 0);
            }

            //Inicializo variables utiles
            int MAX = MaxRaysPerDimention;
            double CotaMinima = 0.3;
            double m = ((double)(MAX - 2)) / ((double)(6 - CotaMinima));
            double n = MAX - m * 6;

            //aplico el filtro edge
            Console.WriteLine("Detectando bordes");
            for (int row = 0; row < color_matrix.GetLength(0); row++)
            {
                int lenght = color_matrix.GetLength(1);
                Parallel.For(0, lenght, col =>
               {
                   Vector Suma = new Vector(0, 0, 0);
                   for (int rowk = -1; rowk < 2; rowk++)
                   {
                       for (int colk = -1; colk < 2; colk++)
                       {
                           if (edgekernel[rowk + 1, colk + 1] != 0)
                           {
                               int r = row + rowk;
                               int c = col + colk;
                               if (r < 0) r = 0;
                               else if (r >= color_matrix.GetLength(0)) r = color_matrix.GetLength(0) - 1;
                               if (c < 0) c = 0;
                               else if (c >= color_matrix.GetLength(0)) c = color_matrix.GetLength(0) - 1;
                               Suma += color_matrix[r, c] * edgekernel[rowk + 1, colk + 1];
                           }
                       }
                   }
                   double Magnitud = Suma.Magnitud * 2;
                   if (Magnitud > CotaMinima)
                   {
                       int rn;
                       if (Magnitud >= 6)
                       {
                           rn = MAX;
                       }
                       else
                       {

                           rn = (int)(m * Magnitud + n);
                       }
                       lock (Critics)
                       {
                           Critics.Add(new CriticPixel(row, col, rn));
                           cantidades[rn]++;
                       }
                   }
               });
            }
            for (int i = 2; i <= MaxRaysPerDimention; i++)
            {
                Console.WriteLine("Rayos por pixel: {0}, Cantidad de pixeles: {1}", Math.Pow(i, 2), cantidades[i]);
            }
            return Critics;
        }
    }
}
