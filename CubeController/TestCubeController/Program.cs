using System;
using CubeController;
using System.IO;

namespace TestCubeController
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Cube cube = new Cube ();
		}

		public static void RenderCube(bool[][][] cube)
		{
			if (cube != null) {
				// Print each z plane. 
				for (int z = 0; z < 8; ++z) {
					Console.WriteLine ("PLANE {0}", z);
					for (int x = 0; x < 8; ++x) {
						for (int y = 0; y < 8; ++y) {
							if (cube [x] [y] [z]) {
								Console.Write ("# ");
							} else {
								Console.Write ("- ");
							}
						}
						Console.WriteLine ();
					}
					Console.WriteLine ("\n");
				}
			}
		}

		public static void AllPlanesClearSet_Test(ref Cube cube)
		{
			for (int i = 0; i < 8; ++i) {
				cube.SetPlane_X (i);
				cube.SetPlane_Y (i);
				cube.ClearPlane_Z (i);
				RenderCube (cube.GetCubeState ());
			}

			for (int i = 7; i >= 0; --i) {
				cube.ClearPlane_X (i);
				cube.ClearPlane_Y (i);
				cube.SetPlane_Z (i);
				RenderCube (cube.GetCubeState ());
			}
		}
	}
}
