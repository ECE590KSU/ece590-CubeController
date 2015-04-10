using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using CubeController;

namespace CubeController
{
	public class Cube
	{
		private bool[][][] _cubeState;		// A collection of voxels. 
		private const int DIMENSION = 8;	// How many voxels per anode column. 

		private FontHandler _fontHandler;

		public enum AXIS { AXIS_X, AXIS_Y, AXIS_Z };
		public enum DIRECTION { FORWARD, REVERSE };
		public enum REFLECTION { ORIGIN, TERMINUS };

		public int Dimension { get; set; }

		/// <summary>
		/// Initializes a new instance of the Cube class.
		/// Default public constructor. 
		/// </summary>
		public Cube ()
		{
			// Allocate space for a NxNxN cube, where N = DIMENSION.
			_cubeState = new bool[DIMENSION][][];
			for (int i = 0; i < DIMENSION; ++i) {
				_cubeState[i] = new bool[DIMENSION][];
				for (int j = 0; j < DIMENSION; ++j) {
					_cubeState [i] [j] = new bool[DIMENSION];
				}
			}

			_fontHandler = new FontHandler ();
		}

		/// <summary>
		/// Gets the state of the cube.
		/// </summary>
		/// <returns>The cube state.</returns>
		public bool[][][] GetCubeState()
		{
			return _cubeState;
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
		/// Clears the entire cube by erasing Z-Planes.
		/// </summary>
		public void ClearEntireCube()
		{
			for (int i = 0; i < DIMENSION; ++i) {
				ClearPlane_Z (i);
			}
		}

		/// <summary>
		/// Sets the entire cube by setting Z-Planes.
		/// </summary>
		public void SetEntireCube()
		{
			for (int i = 0; i < DIMENSION; ++i) {
				SetPlane_Z (i);
			}
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
		public void ClearPlane_X(int x)
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
		public void SetPlane_Y(int y)
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
		public void ClearPlane_Y(int y)
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
		public void SetPlane_Z(int z)
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
		public void ClearPlane_Z(int z)
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
		/// Creates an empty plane for use of filling. 
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
		/// Gets a plane by axis, indexed by pl. See documentation for graphical
		/// explanation. Complications arise due to cube orientation towards end user.
		/// 
		/// For instance: 
		/// GetPlane(AXIS_Y, 1) --> return the X-Z plane at Y=1.
		/// GetPlane(AXIS_X, 0) --> return the Y-Z plane at X=0.
		/// GetPlane(AXIS_Z, 3) --> return the X-Y plane at Z=3.
		/// 
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
						tmpplane [y] [z] = _cubeState [pl] [z] [y];
					}
				}
				break;

			case AXIS.AXIS_Y:
				for (int x = 0; x < DIMENSION; ++x) {
					for (int z = 0; z < DIMENSION; ++z) {
						tmpplane [x] [z] = _cubeState [z] [pl] [DIMENSION - 1 - x];
					}
				}
				break;

			case AXIS.AXIS_Z:
				for (int x = 0; x < DIMENSION; ++x) {
					for (int y = 0; y < DIMENSION; ++y) {
						tmpplane [x] [y] = _cubeState [y] [DIMENSION - 1 - x] [pl];
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
		/// 
		/// See documentation for graphical explanation. Complications arise due 
		/// to cube orientation towards end user.
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
						_cubeState [pl] [y] [z] = pattern [z] [y];
					}
				}
				break;
			
			// 
			case AXIS.AXIS_Y:
				for (int x = 0; x < DIMENSION; ++x) {
					for (int z = 0; z < DIMENSION; ++z) {
						_cubeState [x] [pl] [z] = pattern [DIMENSION - 1 - z] [x];
					}
				}
				break;

			case AXIS.AXIS_Z:
				for (int x = 0; x < DIMENSION; ++x) {
					for (int y = 0; y < DIMENSION; ++y) {
						_cubeState [x] [y] [pl] = pattern [DIMENSION - 1 - y] [x];
					}
				}
				break;

			default:
				break;
			}
		}

#endregion // DRAW

#region ADVANCED_DRAW
			
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
					// Set the ith plane to the plane after it.
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
				RowReversal2D (ref plane);
				break;

			case 180:
				Transpose2D (ref plane);
				RowReversal2D (ref plane);
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

		public void RenderPlane(bool[][] plane)
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
				Console.WriteLine ();
				Console.WriteLine ();
			}
		}

		/// <summary>
		/// Transposes a 2D square matrix.
		/// </summary>
		/// <param name="mtx">Matrix to transpose.</param>
		private void Transpose2D(ref bool[][] mtx)
		{
			bool[][] temp = NewEmptyPlane (DIMENSION);

			for (int i = 0; i < DIMENSION; ++i) {
				for (int j = 0; j < DIMENSION; ++j) {
					temp[j][i] = mtx [i] [j];
				}
			}

			mtx = temp;
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

		/// <summary>
		/// Reverses the rows of a 2D matrix.
		/// </summary>
		/// <param name="mtx">Matrix source of rows.</param>
		private void RowReversal2D(ref bool[][] mtx)
		{
			foreach (bool[] row in mtx) {
				Array.Reverse (row);
			}
		}

		/// <summary>
		/// Mirrors the cube along a given axis.
		/// 
		/// If a cube is mirrored along it's Z-axis, the voxels at the top will
		/// now become the voxels at the bottom. They DO NOT change x-y positions
		/// though within the x-y plane from that Z-slice. 
		/// </summary>
		/// <param name="axis">Axis to mirror across.</param>
		public void MirrorCubeAlongAxis(AXIS axis)
		{
			// Get the outer-two most planes along the given axis. Swap them. 
			// Then move on to the next inner-two. Swap them as well.
			// Repeat until no planes are left!
			bool[][] plane1;
			bool[][] plane2;

			for (int i = 0; i < (DIMENSION / 2); ++i) {
				plane1 = GetPlane (axis, i);				 // Get the plane closest to origin.
				plane2 = GetPlane (axis, DIMENSION - 1 - i); // Get the plane closest to terminus.

				PatternSetPlane (axis, i, plane2);	// Put plane2 at the beginning.
				PatternSetPlane (axis, DIMENSION - 1 - i, plane1);	// Put plane1 at the end. 
			}
		}

		/// <summary>
		/// Provides symmetry of the cube along a given axis. 
		/// You can reflect the axis either from origin or from the terminating
		/// end. 
		/// </summary>
		/// <param name="axis">Axis to provide symmetry along.</param>
		/// <param name="refl">Reflection direction.</param>
		public void SymmetryAlongAxis(AXIS axis, REFLECTION refl)
		{
			bool[][] plane_source;

			switch (refl) {
			// Reflect the cube along the plane along the specified axis, starting
			// from origin. The closest planes to 0,0,0 are what source the symmetry.
			case REFLECTION.ORIGIN:
				for (int i = 0; i < (DIMENSION / 2); ++i) {
					plane_source = GetPlane (axis, i);
					PatternSetPlane (axis, DIMENSION - 1 - i, plane_source);
				}
				break;
			case REFLECTION.TERMINUS:
				for (int i = 0; i < (DIMENSION / 2); ++i) {
					plane_source = GetPlane (axis, DIMENSION - 1 - i);
					PatternSetPlane (axis, i, plane_source);
				}
				break;
			default:
				break;
			}
		}

		/// <summary>
		/// Draws a line across the cube, in 3D.
		/// 
		/// Line segment equations between two points in 3D:
		/// http://math.kennesaw.edu/~plaval/math2203/linesplanes.pdf, pg.4, eq(1.13).
		/// </summary>
		/// <param name="x1">The first x coordinate.</param>
		/// <param name="y1">The first y coordinate.</param>
		/// <param name="z1">The first z coordinate.</param>
		/// <param name="x2">The second x coordinate.</param>
		/// <param name="y2">The second y coordinate.</param>
		/// <param name="z2">The second z coordinate.</param>
		public void DrawLine(int x1, int y1, int z1, int x2, int y2, int z2)
		{
			// Parametric equations for line segments between two points in
			// 3D Euclidean Space + Cartesian Plane. 

			// x = (1-t) x_1 + t * x_2
			// y = (1-t) y_1 + t * y_2
			// z = (1-t) z_1 + t * z_2
			//
			//	t ismember { [0,1] }. Divide into DIMENSION segments.
			float delta_t = 1.0f / (float)(DIMENSION);
			float t = 0.0f;
			int x = 0, y = 0, z = 0;

			for (int i=0; i <= DIMENSION; ++i){
				x = (int)((1 - t) * (x1) + (t * x2));
				y = (int)((1 - t) * (y1) + (t * y2));
				z = (int)((1 - t) * (z1) + (t * z2));
				SetVoxel (x, y, z);
				t += delta_t;
			}
		}

		/// <summary>
		/// Draws a line across the cube, in 3D.
		/// 
		/// Line segment equations between two points in 3D:
		/// http://math.kennesaw.edu/~plaval/math2203/linesplanes.pdf, pg.4, eq(1.13).
		/// </summary>
		/// <param name="p1">The source x,y,z point</param>
		/// <param name="p2">The destination x,y,z point</param>
		public void DrawLine(Point p1, Point p2)
		{
			// Parametric equations for line segments between two points in
			// 3D Euclidean Space + Cartesian Plane. 

			// x = (1-t) x_1 + t * x_2
			// y = (1-t) y_1 + t * y_2
			// z = (1-t) z_1 + t * z_2
			//
			//	t ismember { [0,1] }. Divide into DIMENSION segments.
			float delta_t = 1.0f / (float)(DIMENSION);
			float t = 0.0f;
			int x = 0, y = 0, z = 0;

			for (int i=0; i <= DIMENSION; ++i){
				x = (int)((1 - t) * (p1.X) + (t * p2.X));
				y = (int)((1 - t) * (p1.Y) + (t * p2.Y));
				z = (int)((1 - t) * (p1.Z) + (t * p2.Z));
				SetVoxel (x, y, z);
				t += delta_t;
			}
		}

		/// <summary>
		/// Clears a line across the cube, in 3D. 
		/// 
		/// Line segment equations between two points in 3D:
		/// http://math.kennesaw.edu/~plaval/math2203/linesplanes.pdf, pg.4, eq(1.13).
		/// </summary>
		/// <param name="x1">The first x value.</param>
		/// <param name="y1">The first y value.</param>
		/// <param name="z1">The first z value.</param>
		/// <param name="x2">The second x value.</param>
		/// <param name="y2">The second y value.</param>
		/// <param name="z2">The second z value.</param>
		public void ClearLine(int x1, int y1, int z1, int x2, int y2, int z2)
		{
			// Parametric equations for line segments between two points in
			// 3D Euclidean Space + Cartesian Plane. 

			// x = (1-t) x_1 + t * x_2
			// y = (1-t) y_1 + t * y_2
			// z = (1-t) z_1 + t * z_2
			//
			//	t ismember { [0,1] }. Divide into DIMENSION segments.
			float delta_t = 1.0f / (float)(DIMENSION);
			float t = 0.0f;
			int x = 0, y = 0, z = 0;

			for (int i=0; i <= DIMENSION; ++i){
				x = (int)((1 - t) * (x1) + (t * x2));
				y = (int)((1 - t) * (y1) + (t * y2));
				z = (int)((1 - t) * (z1) + (t * z2));
				ClearVoxel (x, y, z);
				t += delta_t;
			}
		}

		/// <summary>
		/// Clears a line across the cube, in 3D.
		/// 
		/// Line segment equations between two points in 3D:
		/// http://math.kennesaw.edu/~plaval/math2203/linesplanes.pdf, pg.4, eq(1.13).
		/// </summary>
		/// <param name="p1">The source x,y,z point</param>
		/// <param name="p2">The destination x,y,z point</param>
		public void ClearLine(Point p1, Point p2)
		{
			// Parametric equations for line segments between two points in
			// 3D Euclidean Space + Cartesian Plane. 

			// x = (1-t) x_1 + t * x_2
			// y = (1-t) y_1 + t * y_2
			// z = (1-t) z_1 + t * z_2
			//
			//	t ismember { [0,1] }. Divide into DIMENSION segments.
			float delta_t = 1.0f / (float)(DIMENSION);
			float t = 0.0f;
			int x = 0, y = 0, z = 0;

			for (int i=0; i <= DIMENSION; ++i){
				x = (int)((1 - t) * (p1.X) + (t * p2.X));
				y = (int)((1 - t) * (p1.Y) + (t * p2.Y));
				z = (int)((1 - t) * (p1.Z) + (t * p2.Z));
				ClearVoxel (x, y, z);
				t += delta_t;
			}
		}

		/// <summary>
		/// Draws a wireframe box. The length of each side is
		/// dist. The box will have its closest corner to the cube's origin (0,0,0)
		/// as the source vertex. 
		/// 
		///       F * - - - - - * C
		///    	   /|          /|
		///   	  / |         / |
		///  	 /  |        /  |
		///   E * - | - - - * D |
		///  	| G * - - - |- -* B
		/// 	| /         |  /
		/// 	|/          | /
		///   S * - - - - - */ A
		/// 
		///     
		///  Z|  /Y
		///   | /
		///   |/_ _ _X
		/// (0,0)
		/// 
		/// Vertices are labeled in the diagram above. 
		/// </summary>
		/// <param name="S">Source vertex.</param>
		/// <param name="dist">Distance between vertices (side length).</param>
		public void BoxWireFrame(Point S, int dist)
		{
			Point A, B, C, D, E, F, G;
						// X-coordinate  Y-coordinate	Z-Coordinate
			A = new Point (S.X + dist, 	 S.Y, 			S.Z);
			B = new Point (S.X + dist, 	 S.Y + dist, 	S.Z);
			C = new Point (S.X + dist, 	 S.Y + dist, 	S.Z + dist);
			D = new Point (S.X + dist, 	 S.Y, 			S.Z + dist);
			E = new Point (S.X, 		 S.Y, 			S.Z + dist);
			F = new Point (S.X, 		 S.Y + dist,	S.Z + dist);
			G = new Point (S.X, 		 S.Y + dist, 	S.Z);

			DrawLine (S, A);
			DrawLine (S, G);
			DrawLine (S, E);

			DrawLine (E, F);
			DrawLine (E, D);

			DrawLine (F, C);
			DrawLine (F, G);

			DrawLine (C, B);
			DrawLine (C, D);

			DrawLine (B, G);
			DrawLine (B, A);

			DrawLine (A, D);
		}

#endregion // ADVANCED_DRAW

#region FONT

		/// <summary>
		/// Writes a specified to a plane along axis. 
		/// </summary>
		/// <param name="axis">Axis to write along.</param>
		/// <param name="pl">Plane to modify.</param>
		/// <param name="c">Character to write (lookup bitmap).</param>
		public void PutChar(AXIS axis, int pl, char c)
		{
			PatternSetPlane (axis, pl, GetChar (c));
		}

		/// <summary>
		/// Gets the character specified by 'c'. 
		/// </summary>
		/// <returns>The char.</returns>
		/// <param name="c">C.</param>
		private bool[][] GetChar(char c)
		{
			return _fontHandler.LookupByKey (c);
		}

		/// <summary>
		/// Prints a message character by character on a given axis, and sends it flying
		/// either front-to-back or back-to-front. 
		/// 
		/// NOTE: front-to-back is relative to axis, specifically from ORIGIN-to-TERMINUS,
		/// or as close as possible. 
		/// </summary>
		/// <param name="message">Message to transmit.</param>
		/// <param name="axis">Axis to send message along.</param>
		/// <param name="direction">Direction of travel.</param>
		public void MessageFlyOnAxis(string message, AXIS axis, DIRECTION direction)
		{
			// Make sure that the string is a valid message. If it is null or empty, then we
			// have no message to transmit. 
			if (String.IsNullOrEmpty (message)) {
				message = "INVALID TEXT";
			}

			foreach (char c in message) {
				PutChar (axis, 0, c);
				for (int i = 0; i < DIMENSION; ++i) {
					RenderPlane (GetPlane (axis, i));
					Shift (axis, direction);
					DelayMS (200);
				}
				ClearEntireCube ();
			}
		}

		/// <summary>
		/// Sends a message "around" the cube in a banner-like manner (rhyme!). 
		/// 
		/// A character is put on either the 0th X plane, or th 7th X plane, and
		/// then is printed out character by character, and rotated around the cube. 
		/// Only three characters will be printed at a time (see documentation 
		/// for explanation). 
		/// </summary>
		/// <param name="message">Message to print.</param>
		/// <param name="direction">Direction to send around cube.</param>
		public void MessageBanner(string message, DIRECTION direction)
		{

			// A message will be sent: LAST CHARACTER --> around cube R-L --> gone.
			// And then each character preceding it will be sent in quick succession.
			// At any given point up to three characters will be printed out.
			if (direction == DIRECTION.FORWARD) {
				// The message needs to be reversed to send the last character out first. 
				var chars = message.ToCharArray ();
				Array.Reverse (chars);
				// Tack on three extra blank characters, so that the actual last
				// character will "fall-off" the cube. 
				message = new string(chars);
				message += "   ";

				for (int i = 0; i < message.Length; ++i) {
					// Put the ith character on the LEFT-FACE of CUBE.
					PatternSetPlane (AXIS.AXIS_X, 0, GetChar (message [i]));
					DelayMS (400);

					// Take the character from the LEFT-FACE, and put it on FRONT-FACE.
					PatternSetPlane (AXIS.AXIS_Y, 0, GetPlane (AXIS.AXIS_X, 0));
					DelayMS (400);

					// Take the character from the FRONT-FACE, and put it on the RIGHT-FACE.
					PatternSetPlane (AXIS.AXIS_X, 7, GetPlane (AXIS.AXIS_Y, 0));
					DelayMS (400);
				}
			
			// A Message will be sent: FIRST CHARACTER --> around cube L-R --> gone.
			// And then each character following it will be sent in quick session.
			} else {

				message += "   ";

				for (int i = 0; i < message.Length; ++i) {
					// Put the ith character on the RIGHT-FACE of CUBE.
					PatternSetPlane (AXIS.AXIS_X, 7, GetChar (message [i]));
					DelayMS (400);

					// Take the character from the RIGHT-FACE, and put it on FRONT-FACE.
					PatternSetPlane (AXIS.AXIS_Y, 0, GetPlane (AXIS.AXIS_X, 7));
					DelayMS (400);

					// Take the character from the FRONT-FACE, and put it on the LEFT-FACE.
					PatternSetPlane (AXIS.AXIS_X, 0, GetPlane (AXIS.AXIS_Y, 0));
					DelayMS (400);
				}

			}
		}

#endregion // FONT

#region EFFECT

#endregion

	}

	/// <summary>
	/// Helper class. Defines a point in x,y,z space. Used to
	/// eliminate function calling signatures with many x,y,z args needed.
	/// </summary>
	public class Point
	{
		public int X { get; set; }
		public int Y { get; set; }
		public int Z { get; set; }

		public Point()
		{

		}

		public Point(int x, int y, int z) : base()
		{
			X = x;
			Y = y;
			Z = z;
		}
	}
}

