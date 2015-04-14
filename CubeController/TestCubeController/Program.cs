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

			//cube.LineSpin (2000, 50);
			//cube.SideWaves (2000, 40);

			/*
			int i = 0;
			while (i++ < 2000) {
				cube.BoxWoopWoop (1, Effect.COMFORTABLE_BOX_WOOP_WOOP_DELAY, true);
				cube.BoxWoopWoop (1, Effect.COMFORTABLE_BOX_WOOP_WOOP_DELAY, false);
			}
			*/

			cube.SetPlane (Cube.AXIS.AXIS_Z, 0);
			for (int i = 0; i < cube.Dimension; ++i) {
				for (int j = 0; j < cube.Dimension; ++j) {
					cube.SendVoxelZ (i, j, 0, 100);
				}
			}
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
