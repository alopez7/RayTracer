using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;
using Materials;
using Illumination;

namespace RayTracer
{
    public class Scene
    {
        private Body[] Bodies;
        private Camera Cam;
        public Color BackroundColor;
        public Light[] Lights;
        public Color AmbientLight;
        public int MaxReflections;

        public Scene(Body[] bodies, Light[] lights, Camera cam, Dictionary<string, dynamic> dict, Color AmbLight)
        {
            Bodies = bodies;
            Lights = lights;
            Cam = cam;
            double[] Values = ParseVect(dict, "background_color");
            BackroundColor = new Color(Values[0], Values[1], Values[2]);
            AmbientLight = AmbLight;
            try
            {
                MaxReflections = (int)dict["max_reflections"];
            }
            catch
            {
                MaxReflections = int.MaxValue;
            }
        }

        private double[] ParseVect(Dictionary<string, dynamic> dic, string key)
        {
            double[] arr = new double[3];
            for (int i = 0; i < 3; i++)
            {
                arr[i] = dic[key][i];
            }
            return arr;
        }

        public Color[,] Generate_Image(int RaysPerPixel)
        {
            return Cam.Generate_Image(Bodies, BackroundColor, Lights, AmbientLight, MaxReflections, RaysPerPixel);
        }

        public Color[,] GenerateAdaptativeImage(Color[,] image, List<CriticPixel> Critics)
        {
            return Cam.GenerateAdaptativeImage(Bodies, BackroundColor, Lights, AmbientLight, MaxReflections, image, Critics);
        }
    }
}
