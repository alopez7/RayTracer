using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;
using VectorGeometry;
using Materials;
using Illumination;

namespace RayTracer
{
    public class Camera
    {
        public double Near;
        public double LensSize;
        public double Fov;
        public Vector Position;
        public Vector Up;
        public Vector Target;
        public int Width;
        public int Height;
        private double Top;
        private double Bottom;
        private double Right;
        private double Left;
        private Vector U;
        private Vector V;
        private Vector W;
        private Random random;
        private double Exposure;

        public Vector E
        {
            get
            {
                return Position;
            }
        }

        public Vector T
        {
            get
            {
                return Target;
            }
        }

        public Camera(Dictionary<string, dynamic> dict, int width, int height)
        {
            //Creo el random
            random = new Random();
            //Creo el exposure
            try {Exposure = dict["camera"]["exposure"];}
            catch {Exposure = 0;}
            //Creo el near
            try {Near = dict["camera"]["focal_distance"];}
            catch {Near = 0.1;}
            //Creo el LensSize
            try {LensSize = dict["camera"]["lens_size"];}
            catch {LensSize = 0;}
            //guardo el Fov
            Fov = (dict["camera"]["fov"] * Math.PI) / 180;
            //parseo los vectores y los guardo
            Position = new Vector(ParseVect(dict, "position"));
            Up = new Vector(ParseVect(dict, "up"));
            Up.Normalizar();
            Target = new Vector(ParseVect(dict, "target"));
            //obtengo width y height
            Width = width;
            Height = height;
            //Calculo top, bottom, right, y left
            Top = Near * Math.Tan(Fov / 2);
            Bottom = -Top;
            Right = Top * (Width / Height);
            Left = -Right;
            //Calculo U, V, W
            W = (Position - Target).Normalizado();
            U = Vector.ProductoCruz(Up, W).Normalizado();
            V = Vector.ProductoCruz(W, U).Normalizado();
        }

        private double[] ParseVect(Dictionary<string, dynamic> dic, string key)
        {
            double[] arr = new double[3];
            for (int i = 0; i < 3; i++)
            {
                arr[i] = dic["camera"][key][i];
            }
            return arr;
        }

        public Color Cast(Vector PixPosition, Body[] cuerpos, Color back_color, Light[] lights, Color AmbLight, int MaxReflections)
        {
            Vector Origin;
            if (LensSize != 1)
            {
                Origin = GetRandomOrigin();        
            }
            else
            {
                Origin = E;
            }
            Ray rayo = new Ray(Origin, (PixPosition - Origin), MaxReflections, 1, false, GetRandomTime());
            Color debuger = rayo.Cast(cuerpos, back_color, lights, AmbLight);
            return debuger;
        }

        private Vector PixelPosition(int i, int j, int ri, int rj, int RPL)
        {
            double iu = Left + ((Right - Left) / (Width * RPL)) * ((i * RPL + ri) + 0.5 / RPL);
            double jv = Bottom + ((Top - Bottom) / (Height * RPL)) * ((j * RPL + rj) + 0.5 / RPL);
            return (Position - W * Near + U * iu + V * jv);
        }

        public Vector GetRandomOrigin()
        {
            Vector Origin = E;
            Vector UpDesp = (U * (GetRandom() * LensSize - LensSize / 2));
            Vector SideDesp = (V * (GetRandom() * LensSize - LensSize / 2));
            return (Origin + UpDesp + SideDesp);
        }

        public double GetRandomTime()
        {
            return GetRandom() * Exposure;
        }

        public Color[,] Generate_Image(Body[] cuerpos, Color back_color, Light[] lights, Color AmbLight, int MaxReflections, int RaysPerPixel)
        {
            Color[,] color_matrix = new Color[Height, Width];
            int perc = 0;
            int ante = 0;
            //RPL = rayos por linea
            int RPL = (int) Math.Sqrt(RaysPerPixel);
            for (int row = 0; row < Height; row++)
            {

                Parallel.For(0, Width, col =>
                {
                    Color pixel = new Color(0, 0, 0);
                    for (int ri = 0; ri < RPL; ri++)
                    {
                        for (int rj = 0; rj < RPL; rj++)
                        {
                            Vector PP = PixelPosition(col, Height - row - 1, ri, rj, RPL);
                            Color C = Cast(PP, cuerpos, back_color, lights, AmbLight, MaxReflections);
                            pixel = pixel + (C * (1 / (double)RaysPerPixel));
                        }
                    }
                    color_matrix[row, col] = pixel;
                    Color debuguer = color_matrix[row, col];
                });

                /*for(int col = 0; col < Width; col++)
                {
                    Color pixel = new Color(0, 0, 0);
                    for (int ri = 0; ri < RPL; ri++)
                    {
                        for (int rj = 0; rj < RPL; rj++)
                        {
                            Vector PP = PixelPosition(col, Height - row - 1, ri, rj, RPL);
                            Color C = Cast(PP, cuerpos, back_color, lights, AmbLight, MaxReflections);
                            pixel = pixel + (C * (1 / (double)RaysPerPixel));
                        }
                    }
                    color_matrix[row, col] = pixel;
                    Color debuguer = color_matrix[row, col];
                }*/

                perc = (row * 100) / Height;
                if (perc != ante)
                {
                    Console.Write("\r{0}% ", perc);
                    ante = perc;
                }
            }
            Console.Write("\r100%\n");
            return color_matrix;
        }

        public Color[,] GenerateAdaptativeImage(Body[] cuerpos, Color back_color, Light[] lights, Color AmbLight, int MaxReflections, Color[,] image, List<CriticPixel> Critics)
        {
            Console.WriteLine("Aplicando Anti Aliasing Adaptativo a {0} pixeles", Critics.Count());
            int anterior = 0;
            int porcentaje;
            double contador = 0;
            Parallel.ForEach(Critics, crit =>
            {
                Color pixel = new Color(0, 0, 0);
                for (int ri = 0; ri < crit.RaysNumber; ri++)
                {
                    for (int rj = 0; rj < crit.RaysNumber; rj++)
                    {
                        Vector PP = PixelPosition(crit.Col, Height - crit.Row - 1, ri, rj, crit.RaysNumber);
                        Color C = Cast(PP, cuerpos, back_color, lights, AmbLight, MaxReflections);
                        pixel = pixel + (C * (1 / Math.Pow(crit.RaysNumber, 2)));
                    }
                }
                image[crit.Row, crit.Col] = pixel;
                porcentaje = (int)(100 * contador / Critics.Count);
                if (porcentaje != anterior)
                {
                    Console.Write("\r{0}% ", porcentaje);
                    anterior = porcentaje;
                }
                contador++;
            });
            Console.Write("\r100%\n");
            return image;
        }

        public double GetRandom()
        {
            double value;
            lock (random)
            {
                value = random.NextDouble();
            }
            return value;
        }
    }
}
