using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorGeometry;

namespace Materials
{
    public class DiffuseTexture: TextureMaterial
    {

        public DiffuseTexture(Dictionary<string, dynamic> dict): base(dict)
        {
            throw new NotImplementedException();
        }

        public override double FuncionReflectancia(Vector3 camDir, Vector3 normal, Vector3 lightDir)
        {
            throw new NotImplementedException();
        }

    }
}
