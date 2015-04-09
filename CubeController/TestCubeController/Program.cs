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

			cube.PutChar (Cube.AXIS.AXIS_Z, 0, '?');
			RenderCube (cube.GetCubeState ());

			cube.Shift (Cube.AXIS.AXIS_Z, Cube.DIRECTION.FORWARD);
			cube.Shift (Cube.AXIS.AXIS_Z, Cube.DIRECTION.FORWARD);
			cube.Shift (Cube.AXIS.AXIS_Z, Cube.DIRECTION.FORWARD);
			RenderCube (cube.GetCubeState ());

			cube.MirrorCubeAlongAxis (Cube.AXIS.AXIS_X);
			RenderCube (cube.GetCubeState ());

			cube.ClearEntireCube ();

			cube.DrawLine (0, 0, 0, 7, 7, 7);
			RenderCube (cube.GetCubeState ());

			cube.SymmetryAlongAxis (Cube.AXIS.AXIS_Z, Cube.REFLECTION.TERMINUS);
			cube.SymmetryAlongAxis (Cube.AXIS.AXIS_Y, Cube.REFLECTION.TERMINUS);
			cube.SymmetryAlongAxis (Cube.AXIS.AXIS_X, Cube.REFLECTION.TERMINUS);

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

		public static void RenderPlane(bool[][] plane)
		{
			if (plane != null) {
				for (int i = 0; i < 8; ++i) {
					for (int j = 0; j < 8; ++j) {
						if (plane [i] [j]) {
							Console.Write ("# ");
						} else {
							Console.Write ("- ");
						}
					}
					Console.WriteLine ();
				}
			}
		}
	}
}
