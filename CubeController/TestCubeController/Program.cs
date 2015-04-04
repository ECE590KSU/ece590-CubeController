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
			cube.SetPlane_X (0);
			cube.ClearVoxel (0, 0, 0);
			cube.RotatePlane (Cube.AXIS.AXIS_X, 0, 90);
		}
	}
}
