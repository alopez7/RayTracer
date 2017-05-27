using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using VectorGeometry;

namespace Materials
{
    public abstract class TextureMaterial: brdf
    {

        public Bitmap Texture;
        public int Width;
        public int Height;
        public bool Bilineal;


        public TextureMaterial(Dictionary<string, dynamic> dict)
        {
            Type = dict["__type__"];
            Name = dict["name"];
            string FilePath = dict["file_path"];
            Texture = new Bitmap(FilePath);
            Width = Texture.Width;
            Height = Texture.Height;
            Bilineal = false;
        }

        public Color GetNNColor(double u, double v)
        {
            int ti = (int)(u * (Width - 1) + 0.5);
            int tj = (int)(v * (Height - 1) + 0.5);
            return TexelColor(ti, tj);
        }

        public Color GetBilinealColor(double u, double v)
        {
            //Obtengo posiciones
            int ti = (int)(u * (Width - 1));
            int tj = (int)(v * (Height - 1));
            double tu = u * (Width - 1);
            double tv = v * (Height - 1);

            //Calculo distancias a los texels
            double du = tu - ti;
            double dv = tv - tj;

            //Calculo colores de texels
            Color T00 = TexelColor(ti,tj);
            Color T10 = TexelColor(ti + 1, tj);
            Color T01 = TexelColor(ti, tj + 1);
            Color T11 = TexelColor(ti + 1, tj + 1);

            //Hago primera aproximacion
            Color A1 = (T00 * (1 - du)) + (T10 * du);
            Color A2 = (T01 * (1 - du)) + (T11 * du);

            //Hago segunda aproximacion y la retorno
            return (A1 * (1 - dv)) + (A2 * dv);
        }

        public Color TexelColor(int i, int j)
        {
            Color col = new Color(0, 0, 0);
            System.Drawing.Color pix = Texture.GetPixel(i, j);
            col.R = pix.R;
            col.G = pix.G;
            col.B = pix.B;
            return col;
        }

    }
}
