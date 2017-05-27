using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorGeometry;

namespace Materials
{
    public class LambertTexture: Texture
    {

        public LambertTexture(Dictionary<string, dynamic> dict): base(dict)
        {
            UseForAmbient = dict["use_for_ambient"];
        }

        public override double FuncionReflectancia(Vector camDir, Vector normal, Vector lightDir)
        {
            lightDir.Normalizar();
            camDir.Normalizar();
            return Math.Max(0, Vector.ProductoPunto(normal, lightDir));
        }

    }
}
