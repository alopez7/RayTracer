using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using VectorGeometry;

namespace Materials
{
    public abstract class Texture
    {
        public string Type;
        public string ColorTexture;
        public string Name;
        public Color[,] Colors;
        public string TextureFiltering;
        public int Width;
        public int Height;
        public string Path;
        public bool UseForAmbient;

        public Texture(Dictionary<string, dynamic> dict)
        {
            Type = dict["__type__"];
            Name = dict["name"];
            ColorTexture = dict["color_texture"];
            TextureFiltering = dict["texture_filtering"];
            Colors = null;
        }

        public void ReadFile()
        {
            Bitmap bitmap = new Bitmap(Path);
            Width = bitmap.Width;
            Height = bitmap.Height;
            Colors = new Color[Height, Width];
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    Color color = new Color(0, 0, 0);
                    System.Drawing.Color pixel = bitmap.GetPixel(col, Height - row - 1);
                    color.R = ((double)pixel.R) / 255;
                    color.G = ((double)pixel.G) / 255;
                    color.B = ((double)pixel.B) / 255;
                    Colors[row, col] = color;
                }
            }
        }

        public abstract double FuncionReflectancia(Vector camDir, Vector normal, Vector lightDir);

        public double[] ParseVect(Dictionary<string, dynamic> dic, string key)
        {
            double[] arr = new double[3];
            for (int i = 0; i < 3; i++)
            {
                arr[i] = dic[key][i];
            }
            return arr;
        }

        public Color GetColor(Color lightColor, Vector CamDir, Vector Normal, Vector lightDir, Color TextColor)
        {
            return (lightColor * (TextColor * FuncionReflectancia(CamDir, Normal, lightDir)));
        }

        public Color GetTextureColor(double u, double v)
        {
            if (Colors == null)
            {
                ReadFile();
            }
            if (TextureFiltering.Equals("bilinear")) return TextureBilinealColor(u, v);
            return TextureNNColor(u, v);
        }

        public Color TextureBilinealColor(double u, double v)
        {
            double tu = u * (Width - 1);
            double tv = v * (Height - 1);
            int ti = (int)(u * (Width - 1));
            int tj = (int)(v * (Height - 1));
            double du = tu - ti;
            double dv = tv - tj;

            Color T00 = GetTexelColor(ti, tj);
            Color T10 = GetTexelColor(ti + 1, tj);
            Color T01 = GetTexelColor(ti, tj + 1);
            Color T11 = GetTexelColor(ti + 1, tj + 1);

            Color A1 = T00 * (1 - du) + T10 * du;
            Color A2 = T01 * (1 - du) + T11 * du;

            return A1 * (1 - dv) + A2 * dv;
        }

        public Color TextureNNColor(double u, double v)
        {
            int ti = (int)(u * (Width - 1) + 0.5);
            int tj = (int)(v * (Height - 1) + 0.5);
            return GetTexelColor(ti, tj);
        }

        public Color GetTexelColor(int i, int j)
        {
            return Colors[j,i];
        }
    }
}
