using System;
using CubeController;

namespace TestCubeController
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Cube cube = new Cube ();
			cube.SetPlane_X (0);
			cube.SetPlane_X (2);

			cube.Shift (Cube.AXIS.AXIS_X, Cube.DIRECTION.FORWARD);
		}
	}
}
