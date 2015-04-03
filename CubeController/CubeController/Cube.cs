using System;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Security.Policy;
using System.Security.Cryptography.X509Certificates;

namespace CubeController
{
	public class Cube
	{
		private bool[,,] _cubeState;
		private const int DIMENSION = 8;

		public enum AXIS { AXIS_X, AXIS_Y, AXIS_Z };
		public enum DIRECTION { FORWARD, REVERSE };

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
			} else {
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
			} else {
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
		/// Turns on all the voxels on the plane indexed by x.
		/// </summary>
		/// <param name="x">The x axis.</param>
		public void SetPlane_X(int x)
		{
			if (x >= 0 && x < DIMENSION) {
				for (int y = 0; y < DIMENSION; ++y) {
					for (int z = 0; z < DIMENSION; ++z) {
						_cubeState [x, y, z] = true;
					}
				}
			}
		}

		/// <summary>
		/// Clears the plane indexed by x.
		/// </summary>
		/// <param name="x">The x axis.</param>
		internal void ClearPlane_X(int x)
		{
			if (x >= 0 && x < DIMENSION) {
				for (int y = 0; y < DIMENSION; ++y) {
					for (int z = 0; z < DIMENSION; ++z) {
						_cubeState [x, y, z] = false;
					}
				}
			}
		}

		/// <summary>
		/// Gets the plane indexed by x. This is so you can shift a plane
		/// much easier than the original source code. There will be
		/// a counterpart called PatternSetPlane_X. 
		/// </summary>
		/// <returns>The plane x.</returns>
		/// <param name="x">The x axis.</param>
		internal bool[,] GetPlane_X(int x)
		{
			bool[,] tmpplane = new bool[DIMENSION, DIMENSION];

			for (int y = 0; y < DIMENSION; ++y) {
				for (int z = 0; z < DIMENSION; ++z) {
					tmpplane [y, z] = _cubeState [x, y, z];
				}
			}

			return tmpplane;
		}

		/// <summary>
		/// Sets the plane indexed by x from a given pattern. 
		/// </summary>
		/// <param name="x">The plane x.</param>
		/// <param name="pattern">The pattern to fill x.</param>
		internal void PatternSetPlane_X(int x, bool[,] pattern)
		{
			for (int y = 0; y < DIMENSION; ++y) {
				for (int z = 0; z < DIMENSION; ++z) {
					_cubeState [x, y, z] = pattern [y, z];
				}
			}
		}

		/// <summary>
		/// Turns on all voxels on the plane indexed by y.
		/// </summary>
		/// <param name="y">The y axis.</param>
		public void SetPlane_Y(int y)
		{
			if (y >= 0 && y < DIMENSION) {
				for (int x = 0; x < DIMENSION; ++x) {
					for (int z = 0; z < DIMENSION; ++z) {
						_cubeState [x, y, z] = true;
					}
				}
			}
		}

		/// <summary>
		/// Clears the plane y.
		/// </summary>
		/// <param name="y">The y axis.</param>
		internal void ClearPlane_Y(int y)
		{
			if (y >= 0 && y < DIMENSION) {
				for (int x = 0; x < DIMENSION; ++x) {
					for (int z = 0; z < DIMENSION; ++z) {
						_cubeState [x, y, z] = false;
					}
				}
			}
		}

		/// <summary>
		/// Gets the plane indexed by y. This is so you can shift a plane
		/// much easier than the original source code. There will be
		/// a counterpart called PatternSetPlane_Y. 
		/// </summary>
		/// <returns>The plane y.</returns>
		/// <param name="y">The y axis.</param>
		internal bool[,] GetPlane_Y(int y)
		{
			bool[,] tmpplane = new bool[DIMENSION, DIMENSION];

			for (int x = 0; x < DIMENSION; ++x) {
				for (int z = 0; z < DIMENSION; ++z) {
					tmpplane [x, z] = _cubeState [x, y, z];
				}
			}

			return tmpplane;
		}

		/// <summary>
		/// Sets the plane indexed by y from a given pattern. 
		/// </summary>
		/// <param name="y">The plane y.</param>
		/// <param name="pattern">The pattern to fill y.</param>
		internal void PatternSetPlane_Y(int y, bool[,] pattern)
		{
			for (int x = 0; x < DIMENSION; ++x) {
				for (int z = 0; z < DIMENSION; ++z) {
					_cubeState [x, y, z] = pattern [x, z];
				}
			}
		}

		/// <summary>
		/// Turns on all voxels on the plane indexed by z.
		/// </summary>
		/// <param name="z">The z axis.</param>
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

		/// <summary>
		/// Clears the plane indexed by z.
		/// </summary>
		/// <param name="z">The z axis.</param>
		internal void ClearPlane_Z(int z)
		{
			if (z >= 0 && z < DIMENSION) {
				for (int x = 0; x < DIMENSION; ++x) {
					for (int y = 0; y < DIMENSION; ++y) {
						_cubeState [x, y, z] = false;
					}
				}
			}
		}

		/// <summary>
		/// Gets the plane indexed by z. This is so you can shift a plane
		/// much easier than the original source code. There will be
		/// a counterpart called PatternSetPlane_Z. 
		/// </summary>
		/// <returns>The plane z.</returns>
		/// <param name="z">The z axis.</param>
		internal bool[,] GetPlane_Z(int z)
		{
			bool[,] tmpplane = new bool[DIMENSION, DIMENSION];

			for (int x = 0; x < DIMENSION; ++x) {
				for (int y = 0; y < DIMENSION; ++y) {
					tmpplane [x, y] = _cubeState [x, y, z];
				}
			}

			return tmpplane;
		}

		/// <summary>
		/// Sets the plane indexed by z from a given pattern. 
		/// </summary>
		/// <param name="z">The plane z.</param>
		/// <param name="pattern">The pattern to fill z.</param>
		internal void PatternSetPlane_Z(int z, bool[,] pattern)
		{
			for (int x = 0; x < DIMENSION; ++x) {
				for (int y = 0; y < DIMENSION; ++y) {
					_cubeState [x, y, z] = pattern [x, y];
				}
			}
		}

		/// <summary>
		/// Delays the drawing buffer from updating for x milliseconds.
		/// </summary>
		/// <param name="x">The number of milliseconds to sleep.</param>
		private void DelayMS(int x)
		{
			Thread.Sleep (x);
		}

		/// <summary>
		/// Shift the specified axis in the specified direction.
		/// </summary>
		/// <param name="axis">Axis.</param>
		/// <param name="direction">Direction.</param>
		public void Shift(AXIS axis, DIRECTION direction)
		{
			bool[,] tmpplane;

			// SHIFTING THE X AXIS.
			if (axis == AXIS.AXIS_X) {
				if (direction == DIRECTION.FORWARD) {
					// Save the last plane so that it may be rotated through as element 0. 
					tmpplane = GetPlane_X (DIMENSION - 1);
					for (int i = DIMENSION - 1; i > 0; --i) {
						// Set the ith plane to the plane before it.
						PatternSetPlane_X (i, GetPlane_X (i-1));
					}
					// Rotate the last plane through to the first element.
					PatternSetPlane_X (0, tmpplane);
				} else {
					// Save the first plane so it will rotate through as last element.
					tmpplane = GetPlane_X (0);
					for (int i = 0; i < DIMENSION - 1; ++i) {
						// Set the i plane to the plane after it. 
						PatternSetPlane_X (i, GetPlane_X (i+1));
					}
					// Rotate the first plane through as the last element. 
					PatternSetPlane_X (DIMENSION - 1, tmpplane);
				}

			// SHIFTING THE Y AXIS
			} else if (axis == AXIS.AXIS_Y) {
				if (direction == DIRECTION.FORWARD) {
					// Save the last plane so that it may be rotated through as element 0. 
					tmpplane = GetPlane_Y (DIMENSION - 1);
					for (int i = DIMENSION - 1; i > 0; --i) {
						// Set the ith plane to the plane before it.
						PatternSetPlane_Y (i, GetPlane_Y (i-1));
					}
					// Rotate the last plane through to the first element.
					PatternSetPlane_Y (0, tmpplane);
				} else {
					// Save the first plane so it will rotate through as last element.
					tmpplane = GetPlane_Y (0);
					for (int i = 0; i < DIMENSION - 1; ++i) {
						// Set the i plane to the plane after it. 
						PatternSetPlane_Y (i, GetPlane_Y (i+1));
					}
					// Rotate the first plane through as the last element. 
					PatternSetPlane_Y (DIMENSION - 1, tmpplane);
				}

			// SHIFTING THE Z AXIS.
			} else {
				if (direction == DIRECTION.FORWARD) {
					// Save the last plane so that it may be rotated through as element 0. 
					tmpplane = GetPlane_Z (DIMENSION - 1);
					for (int i = DIMENSION - 1; i > 0; --i) {
						// Set the ith plane to the plane before it.
						PatternSetPlane_Z (i, GetPlane_Z (i-1));
					}
					// Rotate the last plane through to the first element.
					PatternSetPlane_Z (0, tmpplane);
				} else {
					// Save the first plane so it will rotate through as last element.
					tmpplane = GetPlane_Z (0);
					for (int i = 0; i < DIMENSION - 1; ++i) {
						// Set the i plane to the plane after it. 
						PatternSetPlane_Z (i, GetPlane_Z (i+1));
					}
					// Rotate the first plane through as the last element. 
					PatternSetPlane_Z (DIMENSION - 1, tmpplane);
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

