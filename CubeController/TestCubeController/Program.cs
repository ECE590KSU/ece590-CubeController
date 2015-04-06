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

			cube.DrawLine (0, 0, 0, 7, 7, 7);
			RenderCube (cube.GetCubeState ());
			cube.MirrorCubeByAxis (Cube.AXIS.AXIS_Z);
			RenderCube (cube.GetCubeState ());
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
	}
}
