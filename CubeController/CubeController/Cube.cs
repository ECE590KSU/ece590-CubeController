using System;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace CubeController
{
	public class Cube
	{
		private bool[][][] _cubeState;
		private const int DIMENSION = 8;

		public enum AXIS { AXIS_X, AXIS_Y, AXIS_Z };
		public enum DIRECTION { FORWARD, REVERSE };

		public Cube ()
		{
			_cubeState = new bool[DIMENSION][][];
			for (int i = 0; i < DIMENSION; ++i) {
				_cubeState[i] = new bool[DIMENSION][];
				for (int j = 0; j < DIMENSION; ++j) {
					_cubeState [i] [j] = new bool[DIMENSION];
				}
			}
		}

		public bool[][][] GetCubeState()
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
				_cubeState [x] [y] [z] = true;
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
				_cubeState [x] [y] [z] = false;
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
				return _cubeState [x] [y] [z];
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
			_cubeState [x] [y] [z] = !_cubeState [x] [y] [z];
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
						_cubeState [x] [y] [z] = true;
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
						_cubeState [x] [y] [z] = false;
					}
				}
			}
		}

		/// <summary>
		/// Turns on all voxels on the plane indexed by y.
		/// </summary>
		/// <param name="y">The y axis.</param>
		internal void SetPlane_Y(int y)
		{
			if (y >= 0 && y < DIMENSION) {
				for (int x = 0; x < DIMENSION; ++x) {
					for (int z = 0; z < DIMENSION; ++z) {
						_cubeState [x] [y] [z] = true;
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
						_cubeState [x] [y] [z] = false;
					}
				}
			}
		}

		/// <summary>
		/// Turns on all voxels on the plane indexed by z.
		/// </summary>
		/// <param name="z">The z axis.</param>
		internal void SetPlane_Z(int z)
		{
			if (z >= 0 && z < DIMENSION) {
				for (int x = 0; x < DIMENSION; ++x) {
					for (int y = 0; y < DIMENSION; ++y) {
						_cubeState [x] [y] [z] = true;
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
						_cubeState [x] [y] [z] = false;
					}
				}
			}
		}

		/// <summary>
		/// Creates the empty plane.
		/// </summary>
		/// <returns>The empty plane.</returns>
		/// <param name="size">Size.</param>
		private bool[][] NewEmptyPlane(int size)
		{
			bool[][] tmpplane = new bool[size][];
			for (int i = 0; i < size; ++i){
				tmpplane [i] = new bool[size];
			}

			return tmpplane;
		}

		/// <summary>
		/// Gets a plane by axis, indexed by pl.
		/// </summary>
		/// <returns>The plane indexed by pl on the axis axis.</returns>
		/// <param name="axis">The axis of the plane.</param>
		/// <param name="pl">The index of the plane.</param>
		public bool[][] GetPlane(AXIS axis, int pl)
		{
			bool[][] tmpplane = NewEmptyPlane(DIMENSION);

			switch (axis) {

			case AXIS.AXIS_X:
				for (int y = 0; y < DIMENSION; ++y) {
					for (int z = 0; z < DIMENSION; ++z) {
						tmpplane [y] [z] = _cubeState [pl] [y] [z];
					}
				}
				break;

			case AXIS.AXIS_Y:
				for (int x = 0; x < DIMENSION; ++x) {
					for (int z = 0; z < DIMENSION; ++z) {
						tmpplane [x] [z] = _cubeState [x] [pl] [z];
					}
				}
				break;

			case AXIS.AXIS_Z:
				for (int x = 0; x < DIMENSION; ++x) {
					for (int y = 0; y < DIMENSION; ++y) {
						tmpplane [x] [y] = _cubeState [x] [y] [pl];
					}
				}
				break;

			default:
				break;
			}

			return tmpplane;
		}

		/// <summary>
		/// Sets a plane indexed by pl on the axis axis to a
		/// given pattern.
		/// </summary>
		/// <param name="axis">The axis to set the plane on.</param>
		/// <param name="pl">The index of the plane.</param>
		/// <param name="pattern">The pattern to fill the plane with.</param>
		public void PatternSetPlane(AXIS axis, int pl, bool [][] pattern)
		{
			switch (axis) {

			case AXIS.AXIS_X:
				for (int y = 0; y < DIMENSION; ++y) {
					for (int z = 0; z < DIMENSION; ++z) {
						_cubeState [pl] [y] [z] = pattern [y] [z];
					}
				}
				break;

			case AXIS.AXIS_Y:
				for (int x = 0; x < DIMENSION; ++x) {
					for (int z = 0; z < DIMENSION; ++z) {
						_cubeState [x] [pl] [z] = pattern [x] [z];
					}
				}
				break;

			case AXIS.AXIS_Z:
				for (int x = 0; x < DIMENSION; ++x) {
					for (int y = 0; y < DIMENSION; ++y) {
						_cubeState [x] [y] [pl] = pattern [x] [y];
					}
				}
				break;

			default:
				break;
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
			bool[][] tmpplane;

				if (direction == DIRECTION.FORWARD) {
					// Save the last plane so that it may be rotated through as element 0.
					tmpplane = GetPlane (axis, DIMENSION - 1);
					for (int i = DIMENSION - 1; i > 0; --i) {
						// Set the ith plane to the plane before it.
						PatternSetPlane (axis, i, GetPlane(axis, i-1));
					}
					// Rotate the last plane through to the first element.
					PatternSetPlane (axis, 0, tmpplane);
				} else {
					// Save the first plane so it will rotate through as last element.
					tmpplane = GetPlane (axis, 0);
					for (int i = 0; i < DIMENSION - 1; ++i) {
						// Set the i plane to the plane after it.
						PatternSetPlane (axis, i, GetPlane (axis, i+1));
					}
					// Rotate the first plane through as the last element.
					PatternSetPlane (axis, DIMENSION - 1, tmpplane);
				}
		}

		/// <summary>
		/// Rotates the plane.
		/// 
		/// Rotate by +90:
		/// 	Transpose matrix.
		/// 	Reverse rows.
		///
		/// Rotate by +180:
		/// 	Reverse each row, then each column. 
		/// 
		/// Rotate by +270:
		/// 	Transpose matrix.
		/// 	Reverse columns.
		/// 
		/// See: http://stackoverflow.com/a/8664879
		/// </summary>
		/// <param name="axis">Axis.</param>
		/// <param name="pl">Pl.</param>
		/// <param name="theta">Theta.</param>
		public void RotatePlane(AXIS axis, int pl, int theta)
		{
			// Acquire the plane on the axis you intend to rotate. 
			bool[][] plane = GetPlane (axis, pl);

			switch (theta) {

			case 90:
				Transpose2D (ref plane);
				// Reverse each row of the 'matrix'.
				foreach (bool[] row in plane) {
					Array.Reverse (row);
				}
				break;

			case 180:
				Transpose2D (ref plane);
				// Reverse each row of the 'matrix'. 
				foreach (bool[] row in plane) {
					Array.Reverse (row);
				}
				ColumnReversal2D (ref plane);
				break;

			case 270:
				Transpose2D (ref plane);
				ColumnReversal2D (ref plane);
				break;

			default:
				break;
			}

			// Write your changes to the cube. 
			PatternSetPlane (axis, pl, plane);
		}

		/// <summary>
		/// Transposes a 2D matrix.
		/// </summary>
		/// <param name="mtx">Matrix to transpose.</param>
		private void Transpose2D(ref bool[][] mtx)
		{
			for (int i = 1; i < DIMENSION; ++i) {
				for (int j = 0; j < DIMENSION; ++j) {
					bool temp = mtx [i] [j];
					mtx [i] [j] = mtx [j] [i];
					mtx [j] [i] = temp;
				}
			}
		}

		/// <summary>
		/// Reverses the columns of a 2D matrix.
		/// </summary>
		/// <param name="mtx">Matrix source of columns.</param>
		private void ColumnReversal2D(ref bool[][] mtx)
		{
			List<bool[]> rowList = new List<bool[]> ();
			foreach (bool[] row in mtx) {
				rowList.Add (row);
			}
			rowList.Reverse ();
			mtx = rowList.ToArray ();
		}

		#endregion


		#region DRAW_3D

		#endregion


		#region EFFECT

		#endregion

	}
}

