using System;
using CubeController;
using System.IO;
using System.Threading;

namespace TestCubeController
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Cube cube = new Cube ();

            int i = 0;
		}

		public static void DIAMOND_MOVER(ref Cube cube)
		{
			cube.DrawLine (0, 0, 0, 7, 7, 7);
			cube.SymmetryAlongAxis (Cube.AXIS.AXIS_Z, Cube.REFLECTION.TERMINUS);
			cube.SymmetryAlongAxis (Cube.AXIS.AXIS_Y, Cube.REFLECTION.TERMINUS);
			cube.SymmetryAlongAxis (Cube.AXIS.AXIS_X, Cube.REFLECTION.TERMINUS);

			for (int i = 0; i < 1000; ++i) {
				cube.ShiftAndRoll (Cube.AXIS.AXIS_Z, Cube.DIRECTION.FORWARD);
				cube.RenderCube ();
				Thread.Sleep (100);
			}
		}

		public static void RenderCube(bool[][][] cube)
		{
			if (cube != null) {
				// Print each z plane. 
				for (int z = 0; z < 8; ++z) {
					//Console.WriteLine ("PLANE {0}", z);
					for (int x = 0; x < 8; ++x) {
						for (int y = 0; y < 8; ++y) {
							if (cube [x] [y] [z]) {
								Console.Write ("# ");
							} else {
								Console.Write ("  ");
							}
						}
						Console.WriteLine ();
					}
					Console.WriteLine ("\n");
				}
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
