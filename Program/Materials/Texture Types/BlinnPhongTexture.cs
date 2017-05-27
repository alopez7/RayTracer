using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorGeometry;

namespace Materials
{
    public class BlinnPhongTexture: Texture
    {

        public double Shininess;

        public BlinnPhongTexture(Dictionary<string, dynamic> dict): base(dict)
        {
            Shininess = dict["brdfParams"]["shininess"];
            UseForAmbient = false;
        }

        public override double FuncionReflectancia(Vector camDir, Vector normal, Vector lightDir)
        {
            lightDir.Normalizar();
            camDir.Normalizar();
            Vector h = ((lightDir + camDir) / 2).Normalizado();
            return Math.Max(0, Math.Pow(Vector.ProductoPunto(normal, h), Shininess));
        }

    }
}
