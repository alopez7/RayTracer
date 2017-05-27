using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorGeometry;
using Materials;
using Illumination;

namespace Geometry
{
    public abstract class Body
    {
        public brdf[] brdfMaterials;
        public Mirror[] MirrorMaterials;
        public Dielectric[] DielectricMaterials;
        public Texture[] Textures;

        public abstract void Intersect(Ray rayo);

        public abstract Color GetTextureColor(Ray rayo, Texture texture);

        public bool Sombra(Ray rayo, Light lig, Body[] cuerpos)
        {
            Vector dir = lig.GetDirection(rayo.IntersectionPoint, rayo.ID);
            Vector pos = (rayo.IntersectionPoint + (dir * 0.0005));
            double distance = lig.GetDistance(pos, rayo.ID);
            Ray disp = new Ray(pos, dir, 0, 0, true, rayo.Time);
            foreach (Body cuerpo in cuerpos)
            {
                cuerpo.Intersect(disp);
                if (disp.LastIntersection != null)
                {
                    if (disp.IntersectionDistance < distance)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public Color GetColor(Ray rayo, Light[] lights, Color AmbLight, Body[] cuerpos, Color back_color)
        {
            Color Suma = new Color(0, 0, 0);
            Vector CamDir = (rayo.Direction * -1);
            Vector Normal = rayo.IntersectionNormal;
            if (Vector.ProductoPunto(Normal, CamDir) < 0)
            {
                Normal = (Normal * -1);
            }

            //Calculo color brdf
            if (brdfMaterials.Length != 0)
            {
                Vector lightDir;
                Color lightColor;
                foreach (Light lig in lights)
                {
                    if (!Sombra(rayo, lig, cuerpos))
                    {
                        lightDir = (lig.GetDirection(rayo.IntersectionPoint, rayo.ID));
                        lightColor = lig.Color;
                        foreach (brdf mat in brdfMaterials)
                        {
                            Suma = Suma + mat.GetColor(lightColor, CamDir, Normal, lightDir);
                        }
                    }
                }
            }
            //Calculo color dielectrico y mirror
            if ((MirrorMaterials.Length + DielectricMaterials.Length) != 0)
            {
                //Creo el rayo de reflexion
                Vector ReflectedDir = (rayo.Direction - Normal * (Vector.ProductoPunto(rayo.Direction, Normal) * 2)).Normalizado();
                Vector ReflectedPos = (rayo.IntersectionPoint + ReflectedDir * 0.001);
                Ray ReflectedRay = new Ray(ReflectedPos, ReflectedDir, rayo.Recursion - 1, rayo.IR, rayo.IN, rayo.Time);

                //Si la recursion es mayor a 0, csteo una reflexion y calculo su color
                //En el caso contrario no hago nada
                if (rayo.Recursion > 0)
                {
                    //Casteo el rayo reflejado y obtengo su color
                    Color ReflRayColor = ReflectedRay.Cast(cuerpos, back_color, lights, AmbLight);
                    
                    //Si hay material reflectivo, reflejo
                    foreach (Mirror mir in MirrorMaterials)
                    {
                        Suma = Suma + ReflRayColor * mir.color;
                    }

                    //Calculo el color para cada rayo defractado
                    foreach (Dielectric die in DielectricMaterials)
                    {
                        //Veo que es in y que es out
                        double IRin;
                        double IRout;
                        if (rayo.IN)
                        {
                            IRin = 1;
                            IRout = die.IR;
                        }
                        else
                        {
                            IRin = die.IR;
                            IRout = 1;
                        }
                        //Si no se ha llegado al angulo limite
                        double determinant = 1 - (((Math.Pow(IRout, 2)) / (Math.Pow(IRin, 2))) * (1 - Math.Pow(Vector.ProductoPunto(Normal, rayo.Direction), 2)));
                        if (determinant >= 0)
                        {
                            //Calculo la direccion de la refraccion
                            Vector V1 = ((rayo.Direction - Normal * (Vector.ProductoPunto(rayo.Direction, Normal))) * (IRout / IRin));
                            Vector V2 = (Normal * Math.Sqrt(determinant));
                            Vector RefractedDir = (V1 - V2).Normalizado();

                            //Calculo la posicion del vector refractado con un epsilon
                            Vector RefractedPos = (rayo.IntersectionPoint + RefractedDir * 0.001);
                            //Creo el rayo refractado
                            Ray RefractedRay = new Ray(RefractedPos, RefractedDir, rayo.Recursion - 1, die.IR, !rayo.IN, rayo.Time);

                            //Calculo el color del rayo refractado con cast
                            Color RefrRayColor = RefractedRay.Cast(cuerpos, back_color, lights, AmbLight);

                            //Calculo la proporcion de ambos rayos (revisar coseno)
                            double R0 = Math.Pow((IRin - IRout) / (IRin + IRout), 2);
                            double R = R0 + (1 - R0) * Math.Pow(1 - Vector.ProductoPunto(CamDir, Normal), 5);

                            //Obtengo el color de la reflexion y refraccion
                            Color ReflColor = ReflRayColor * R;
                            Color RefrColor = RefrRayColor * (1 - R);

                            //Calculo la atenuacion
                            Color Attenuation;
                            if (RefractedRay.IntersectionDistance == double.PositiveInfinity || rayo.IN)
                            {
                                Attenuation = new Color(1, 1, 1);
                            }
                            else
                            {
                                Attenuation = die.Attenuated(RefractedRay.IntersectionDistance);
                            }

                            //Calculo el color final
                            Color FinalColor = die.color * (ReflColor + Attenuation * RefrColor);

                            //Agrego el color al material
                            Suma = Suma + FinalColor;
                        }
                        //Si solo hay reflexion
                        else
                        {
                            //Color es solo el reflejado 
                            Color ReflColor = ReflRayColor;

                            //Calculo la atenuacion
                            Color Attenuation = die.Attenuated(ReflectedRay.IntersectionDistance);

                            //Calculo el color final
                            Color FinalColor = die.color * (Attenuation * ReflColor);

                            //Agrego el color al material
                            Suma = Suma + FinalColor;
                        }
                    }
                }
            }

            //Calculo el color de texturas
            if (Textures.Length != 0)
            {
                Vector lightDir;
                Color lightColor;
                foreach (Light lig in lights)
                {
                    if (!Sombra(rayo, lig, cuerpos))
                    {
                        lightDir = (lig.GetDirection(rayo.IntersectionPoint, rayo.ID));
                        lightColor = lig.Color;
                        foreach (Texture tex in Textures)
                        {
                            Suma = Suma + tex.GetColor(lightColor, CamDir, Normal, lightDir, GetTextureColor(rayo, tex));
                        }
                    }
                }
            }

            //Calculo color ambiente de materiales
            foreach (Material mat in brdfMaterials)
            {
                if (mat.UseForAmbient)
                {
                    Suma = Suma + (AmbLight * mat.color);
                }
            }

            //Calculo color ambiente de texturas
            foreach (Texture tex in Textures)
            {
                if (tex.UseForAmbient)
                {
                    Color TextureColor = GetTextureColor(rayo, tex);
                    Suma = Suma + (AmbLight * TextureColor);
                }
            }

            return Suma;
        }
    }

}
