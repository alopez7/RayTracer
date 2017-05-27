using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorGeometry;

namespace Materials
{
    public abstract class brdf: Material
    {

        public abstract double FuncionReflectancia(Vector camDir, Vector normal, Vector lightDir);

        public Color GetColor(Color lightColor, Vector CamDir, Vector Normal, Vector lightDir)
        {
            return (lightColor * (color * FuncionReflectancia(CamDir, Normal, lightDir)));
        }
    }
}
