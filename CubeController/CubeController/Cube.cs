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

		private enum AXIS { AXIS_X, AXIS_Y, AXIS_Z };
		private enum DIRECTION { UP, DOWN };

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
		/// Sets the plane indexed by x.
		/// </summary>
		/// <param name="x">The x axis.</param>
		internal void SetPlane_X(int x)
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
		internal void PatternSetPlane_X(int x, ref bool[,] pattern)
		{
			for (int y = 0; y < DIMENSION; ++y) {
				for (int z = 0; z < DIMENSION; ++z) {
					_cubeState [x, y, z] = pattern [y, z];
				}
			}
		}

		/// <summary>
		/// Sets the plane indexed by y.
		/// </summary>
		/// <param name="y">The y axis.</param>
		internal void SetPlane_Y(int y)
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
		internal void PatternSetPlane_Y(int y, ref bool[,] pattern)
		{
			for (int x = 0; x < DIMENSION; ++x) {
				for (int z = 0; z < DIMENSION; ++z) {
					_cubeState [x, y, z] = pattern [x, z];
				}
			}
		}

		/// <summary>
		/// Sets the plane indexed by z.
		/// </summary>
		/// <param name="z">The z axis.</param>
		internal void SetPlane_Z(int z)
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
		internal void PatternSetPlane_Z(int z, ref bool[,] pattern)
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
		private void Shift(AXIS axis, DIRECTION direction)
		{

		}

		#endregion


		#region DRAW_3D

		#endregion


		#region EFFECT

		#endregion

	}
}

