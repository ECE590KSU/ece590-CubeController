using System;
using CubeController;
using System.Security.Cryptography;

namespace TestCubeController
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Cube cube = new Cube ();
			cube.SetVoxel (0, 0, 0);
			cube.SetVoxel (0, 1, 0);
			cube.SetVoxel (0, 2, 0);

			cube.MirrorCubeByAxis (Cube.AXIS.AXIS_X);
		}
	}
}
