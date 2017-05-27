# RayTracer

This repo has all the code of an c# implementation of a RayTracer.

The RayTracer supports: 
* Spheres and meshes on .obj format.
* Lambert, blinn-phong, dielectric and mirror materials. 
* Reflection, refraction and shadows.
* Lambert and blinn-phong textures.
* Common anti aliassing
* Two types of Adaptative Anti Aliasing made by kernel filters to the image.
* KDTree to organize the triangles of the meshes.
* Parallel RayTracing (On CPU).
* Motion Blur.
* Lens Camera.
* Diffused shadow.

The usage of the RayTracer is in the following format:

`-s <scene.json> -r <resources.json> -w <width> -h <height> -i <output.png> -p <rays_per_pixel> [-a] [-c]`

Where:
* <scene.json> has the description of the camera, objects and illumination.
* <resources.json> has the descrition of the elements of the scene.
* <*width*> and <*height*> are the dimentions of the output image.
* <output.json> is the output file path in png format.
* <rays_per_pixel> is the ammount of rays that the camera casts to every pixel. This is used to get the Anti Aliasing.
* Optional parameters:
  * [-a] is for the Adaptative Anti Aliasing used for Motion Blur, Lens Camera and Diffused shadow effects.
  * [-c] is for the lighter version of the Adaptative Anti Aliasing that works very well for the scenes without effects.
