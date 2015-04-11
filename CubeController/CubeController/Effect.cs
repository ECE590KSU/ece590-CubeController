using System;

namespace CubeController
{
	public static class Effect
	{
		public const int TEST_WAVE_ITERATIONS = 1000;

		/// <summary>
		/// A "magic" number used in some of the original Instructables C-code. 
		/// It seems to be used in connection with finding the distance from the center
		/// of the cube when propagating a sinusoidal wave.
		/// 
		/// The actual definition comes from the original author doing the following:
		///   7	| \
		///     |  \ X?
		/// 	|___\ 
		///       7
		/// X = sqrt(7^2 + 7^2) ~= 9.899495
		/// </summary>
		public const double WAVE_CONSTANT = 9.899495;
		public const double RIPPLE_INTERVAL = 1.3;
		public static double PI = 4.0 * Math.Atan(1.0);
		public const double HELIX_BRAID_LENGTH = 0.05;

	}
}

