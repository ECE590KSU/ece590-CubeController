using System;
using System.Dynamic;
using System.Runtime.InteropServices;

namespace CubeController
{
	public class Cube
	{
		private bool[,,] _cubeState;
		private const int DIMENSION = 8;

		public Cube ()
		{
			_cubeState = new bool[DIMENSION,DIMENSION,DIMENSION];
		}

		public bool[,,] GetCubeState()
		{
			return _cubeState;
		}

		#region DRAW

		/// <summary>
		/// Determines if the specified coordinates are in range
		/// of the cube dimensions. 
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="z">The z coordinate.</param>
		private bool InRange(int x, int y, int z)
		{
			if ((x >= 0 && x < DIMENSION)
			    && (y >= 0 && y < DIMENSION)
			    && (z >= 0 && z < DIMENSION)) {
				return true;
			} 
			else {
				return false;
			}
		}

		/// <summary>
		/// Sets the voxel.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="z">The z coordinate.</param>
		public void SetVoxel(int x, int y, int z)
		{
			if (InRange (x, y, z)) {
				_cubeState [x, y, z] = true;
			}
		}

		/// <summary>
		/// Clears the voxel.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="z">The z coordinate.</param>
		public void ClearVoxel(int x, int y, int z)
		{
			if (InRange (x, y, z)) {
				_cubeState [x, y, z] = false;
			}
		}

		/// <summary>
		/// Gets the voxel.
		/// </summary>
		/// <returns><c>true</c>, if voxel was set, <c>false</c> otherwise.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="z">The z coordinate.</param>
		public bool GetVoxel(int x, int y, int z)
		{
			if (!InRange (x, y, z)) {
				return false;
			} 
			else {
				return _cubeState [x, y, z];
			}
		}

		/// <summary>
		/// Performs logical NOT on voxel value. 
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="z">The z coordinate.</param>
		public void SwapVoxel(int x, int y, int z)
		{
			_cubeState [x, y, z] = !_cubeState [x, y, z];
		}

		/// <summary>
		/// Sets the plane z.
		/// </summary>
		/// <param name="z">The z coordinate.</param>
		public void SetPlane_Z(int z)
		{
			if (z >= 0 && z < DIMENSION) {
				for (int x = 0; x < DIMENSION; ++x) {
					for (int y = 0; y < DIMENSION; ++y) {
						_cubeState [x, y, z] = true;
					}
				}
			}
		}

		#endregion


		#region DRAW_3D

		#endregion


		#region EFFECT

		#endregion

	}
}

