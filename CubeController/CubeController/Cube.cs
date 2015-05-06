using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace CubeController
{
	public class Cube
	{
		private bool[][][] _cubeState;		// A collection of voxels. 
		private const int DIMENSION = 8;	// How many voxels per anode column. 
		private Random _rgen;

		private FontHandler _fontHandler;

        private Timer _serialDriverTimer;
        private TimerCallback _writeCubeCallback;
        private SerialDriver _serialDriver;

		public enum AXIS { AXIS_X, AXIS_Y, AXIS_Z };
		public enum DIRECTION { FORWARD, REVERSE };
		public enum REFLECTION { ORIGIN, TERMINUS };

		public int Dimension
		{
			get 
			{
				return DIMENSION;
			}
		}

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
			_rgen = new Random ();
            _writeCubeCallback = WriteCube;
            _serialDriverTimer = new Timer(_writeCubeCallback, null, 10000, 5);
            _serialDriver = new SerialDriver();
            _serialDriver.OpenPort();
		}

        private void WriteCube(object stateInfo)
        {
            _serialDriver.WriteCube(_cubeState);
        }
        
#region UTILITY

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
		/// Sets the entire cube by setting Z-Planes.
		/// </summary>
		public void SetEntireCube()
		{
			for (int i = 0; i < DIMENSION; ++i) {
				SetPlane (AXIS.AXIS_Z, i);
			}
		}

		/// <summary>
		/// Clears the entire cube by erasing Z-Planes.
		/// </summary>
		public void ClearEntireCube()
		{
			for (int i = 0; i < DIMENSION; ++i) {
				ClearPlane (AXIS.AXIS_Z, i);
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
            for (int i = 0; i < size; ++i)
            {
                tmpplane[i] = new bool[size];
            }

            return tmpplane;
        }
        
        /// <summary>
        /// Renders a specified plane. 
        /// </summary>
        /// <param name="plane"></param>
        public void RenderPlane(bool[][] plane)
        {
            if (plane != null)
            {
                for (int i = 0; i < 8; ++i)
                {
                    for (int j = 0; j < 8; ++j)
                    {
                        if (plane[i][j])
                        {
                            Console.Write("# ");
                        }
                        else
                        {
                            Console.Write("  ");
                        }
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
                Console.WriteLine();
            }
        }

		/// <summary>
		/// Renders the cube by Z-Planes.
		/// </summary>
		public void RenderCube()
		{
			if (_cubeState != null) {
				// Print each z plane. 
				for (int z = 0; z < 8; ++z) {
					//Console.WriteLine ("PLANE {0}", z);
					for (int x = 0; x < 8; ++x) {
						for (int y = 0; y < 8; ++y) {
							if (_cubeState [x] [y] [z]) {
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

#endregion // UTILITY

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
		/// Turns on all voxels on a given plane of the specified axis.
		/// </summary>
		/// <param name="axis">Axis to manipulate.</param>
		/// <param name="pl">Plane index on axis.</param>
		public void SetPlane(AXIS axis, int pl)
		{
			if (pl >= 0 && pl < DIMENSION) {

				switch (axis) {
				case AXIS.AXIS_X:
					for (int y = 0; y < DIMENSION; ++y) {
						for (int z = 0; z < DIMENSION; ++z) {
							_cubeState [pl] [y] [z] = true;
						}
					}
					break;

				case AXIS.AXIS_Y:
					for (int x = 0; x < DIMENSION; ++x) {
						for (int z = 0; z < DIMENSION; ++z) {
							_cubeState [x] [pl] [z] = true;
						}
					}
					break;

				case AXIS.AXIS_Z:
					for (int x = 0; x < DIMENSION; ++x) {
						for (int y = 0; y < DIMENSION; ++y) {
							_cubeState [x] [y] [pl] = true;
						}
					}
					break;

				default:
					break;
				}
			}

		}

		/// <summary>
		/// Turns off all voxels on a given plane of the specified axis.
		/// </summary>
		/// <param name="axis">Axis to manipulate.</param>
		/// <param name="pl">Plane index on axis.</param>
		public void ClearPlane(AXIS axis, int pl)
		{
			if (pl >= 0 && pl < DIMENSION) {

				switch (axis) {
				case AXIS.AXIS_X:
					for (int y = 0; y < DIMENSION; ++y) {
						for (int z = 0; z < DIMENSION; ++z) {
							_cubeState [pl] [y] [z] = false;
						}
					}
					break;

				case AXIS.AXIS_Y:
					for (int x = 0; x < DIMENSION; ++x) {
						for (int z = 0; z < DIMENSION; ++z) {
							_cubeState [x] [pl] [z] = false;
						}
					}
					break;

				case AXIS.AXIS_Z:
					for (int x = 0; x < DIMENSION; ++x) {
						for (int y = 0; y < DIMENSION; ++y) {
							_cubeState [x] [y] [pl] = false;
						}
					}
					break;

				default:
					break;
				}
			}
		}

		/// <summary>
		/// Gets a plane by axis, indexed by pl. See documentation for graphical
		/// explanation. Complications arise due to cube orientation towards end user.
		/// 
		/// For instance: 
		/// GetPlane(AXIS_Y, 1) --> return the A-Z plane at B=1.
		/// GetPlane(AXIS_X, 0) --> return the B-Z plane at A=0.
		/// GetPlane(AXIS_Z, 3) --> return the A-B plane at Z=3.
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
						tmpplane [x] [y] = _cubeState [y] [x] [pl];
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
						_cubeState [pl] [y] [z] = pattern [y] [z];
					}
				}
				break;
			
			// 
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

#endregion // DRAW

#region ADVANCED_DRAW
			
		/// <summary>
		/// Shift the specified axis in the specified direction. Roll planes
		/// through (do not discard planes). 
		/// </summary> 
		/// <param name="axis">Axis.</param>
		/// <param name="direction">Direction.</param>
		public void ShiftAndRoll(AXIS axis, DIRECTION direction)
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
		/// Shift the specified axis in the specified direction. Discard planes
		/// as they reach the boundary. 
		/// </summary> 
		/// <param name="axis">Axis.</param>
		/// <param name="direction">Direction.</param>
		public void ShiftNoRoll(AXIS axis, DIRECTION direction)
		{
			if (direction == DIRECTION.FORWARD) {
				for (int i = DIMENSION - 1; i > 0; --i) {
					// Set the ith plane to the plane before it.
					PatternSetPlane (axis, i, GetPlane(axis, i-1));
				}
				ClearPlane (axis, 0);
			} else {
				for (int i = 0; i < DIMENSION - 1; ++i) {
					PatternSetPlane (axis, i, GetPlane (axis, i+1));
				}
				ClearPlane (axis, DIMENSION - 1);
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

		/// <summary>
		/// Partially rotates a given plane, not based on strictly 90°, 180°, or
		/// -90° turns. 
		/// </summary>
		/// <param name="axis">Axis to rotate along.</param>
		/// <param name="pl">Plane of axis to rotate.</param>
		/// <param name="theta">Degree of rotation.</param>
        public void PartialRotation(AXIS axis, int pl, double theta)
        {
            // Get the plane that you need to rotate. 
            bool[][] tmpplane = GetPlane(axis, pl);

            // List of coordinates of voxels that are set, and which need to
            // be rotated through. 
            var coords = new List<Point>();
            var rotated = new List<Point>();

            // Parital rotation is accomplished via the following matrix expansion:
            // | x' | = | cos(θ) - sin(θ) | | x |
            // | y' | = | sin(θ) + cos(θ) | | y |
            //		Therefore
            // x' = x * cos(θ) - y * sin(θ)
            // y' = x * sin(θ) + y * cos(θ)
            double sin_t = Math.Sin(theta);
            double cos_t = Math.Cos(theta);

            int x_prime = 0;
            int y_prime = 0;

            // Determine which voxels are currently set on this plane. 
            //	x_prime = (int)((P.B * cos_t) - (P.Z * sin_t));
            //	y_prime = (int)((P.B * sin_t) + (P.Z * cos_t));
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
		/// </summary>
		/// <param name="x1">The first x coordinate.</param>
		/// <param name="y1">The first y coordinate.</param>
		/// <param name="z1">The first z coordinate.</param>
		/// <param name="x2">The second x coordinate.</param>
		/// <param name="y2">The second y coordinate.</param>
		/// <param name="z2">The second z coordinate.</param>
		public void DrawLine(int x1, int y1, int z1, int x2, int y2, int z2)
		{
            // Check if there are any matched pairs of coordinates. You can reduce the
            // complexity of the pixelated line by making this check.
            if ((x1 == x2) && (y1 == y2))
            {
                // Always start from lower z and go to upper z. 
                int zStart = (z1 < z2) ? z1 : z2;
                int zEnd = (z1 < z2) ? z2 : z1;

                for (int i = zStart; i <= zEnd; ++i)
                {
                    // A and B are equal between the points. Arbitrary choice of x1 or x2.
                    SetVoxel(x1, y1, i);
                }
            }
            else if ((y1 == y2) && (z1 == z2))
            {
                // Always start from lower x and go to upper x. 
                int xStart = (x1 < x2) ? x1 : x2;
                int xEnd = (x1 < x2) ? x2 : x1;

                for (int i = xStart; i <= xEnd; ++i)
                {
                    // B and Z are equal between the points. Arbitrary choice of y1 or y2.
                    SetVoxel(i, y1, z1);
                }
            }
            else if ((z1 == z2) && (x1 == x2))
            {
                // Always start from lower y and go to upper y.
                int yStart = (y1 < y2) ? y1 : y2;
                int yEnd = (y1 < y2) ? y2 : y1;

                for (int i = yStart; i <= yEnd; ++i)
                {
                    // A and Z are equal between the points. Arbitrary choice of x1 or x2.
                    SetVoxel(x1, i, z1);
                }
            }
            // Otherwise, there are no straight lines, and we have to use
            // Bresenham's Line Algorithm:
            // http://csunplugged.org/wp-content/uploads/2014/12/Lines.pdf
            else
            {
                BresenhamsLine3D(x1, y1, z1, x2, y2, z2, true);
            }
		}

		/// <summary>
		/// Draws a line across the cube, in 3D.
		/// 
		/// </summary>
		/// <param name="p1">The source x,y,z point</param>
		/// <param name="p2">The destination x,y,z point</param>
		public void DrawLine(Point p1, Point p2)
		{
            // Check if there are any matched pairs of coordinates. You can reduce the
            // complexity of the pixelated line by making this check.
            if ((p1.X == p2.X) && (p1.Y == p2.Y))
            {
                // Always start from lower z and go to upper z. 
                int zStart = (p1.Z < p2.Z) ? p1.Z : p2.Z;
                int zEnd   = (p1.Z < p2.Z) ? p2.Z : p1.Z;

                for (int i = zStart; i <= zEnd; ++i)
                {
                    // A and B are equal between the points. Arbitrary choice.
                    SetVoxel(p1.X, p1.Y, i);
                }
            }
            else if ((p1.Y == p2.Y) && (p1.Z == p2.Z)) 
            {
                // Always start from lower x and go to upper x. 
                int xStart = (p1.X < p2.X) ? p1.X : p2.X;
                int xEnd   = (p1.X < p2.X) ? p2.X : p1.X;

                for (int i = xStart; i <= xEnd; ++i)
                {
                    // B and Z are equal between the points. Arbitrary choice.
                    SetVoxel(i, p1.Y, p1.Z);
                }

            }
            else if ((p1.Z == p2.Z) && (p1.X == p2.X))
            {
                // Always start from lower y and go to upper y.
                int yStart = (p1.Y < p2.Y) ? p1.Y : p2.Y;
                int yEnd = (p1.Y < p2.Y) ? p2.Y : p1.Y;

                for (int i = yStart; i <= yEnd; ++i)
                {
                    // A and Z are equal between the points. Arbitrary choice.
                    SetVoxel(p1.X, i, p1.Z);
                }
            }
            // Otherwise, there are no straight lines, and we have to use
            // Bresenham's Line Algorithm:
            // http://csunplugged.org/wp-content/uploads/2014/12/Lines.pdf
            else
            {
                BresenhamsLine3D(p1.X, p1.Y, p1.Z, p2.X, p2.Y, p2.Z, true);
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
            // Check if there are any matched pairs of coordinates. You can reduce the
            // complexity of the pixelated line by making this check.
            if ((x1 == x2) && (y1 == y2))
            {
                // Always start from lower z and go to upper z. 
                int zStart = (z1 < z2) ? z1 : z2;
                int zEnd = (z1 < z2) ? z2 : z1;

                for (int i = zStart; i <= zEnd; ++i)
                {
                    // A and B are equal between the points. Arbitrary choice of x1 or x2.
                    ClearVoxel(x1, y1, i);
                }
            }
            else if ((y1 == y2) && (z1 == z2))
            {
                // Always start from lower x and go to upper x. 
                int xStart = (x1 < x2) ? x1 : x2;
                int xEnd = (x1 < x2) ? x2 : x1;

                for (int i = xStart; i <= xEnd; ++i)
                {
                    // B and Z are equal between the points. Arbitrary choice of y1 or y2.
                    ClearVoxel(i, y1, z1);
                }
            }
            else if ((z1 == z2) && (x1 == x2))
            {
                // Always start from lower y and go to upper y.
                int yStart = (y1 < y2) ? y1 : y2;
                int yEnd = (y1 < y2) ? y2 : y1;

                for (int i = yStart; i <= yEnd; ++i)
                {
                    // A and Z are equal between the points. Arbitrary choice of x1 or x2.
                    ClearVoxel(x1, i, z1);
                }
            }
            // Otherwise, there are no straight lines, and we have to use
            // Bresenham's Line Algorithm:
            // http://csunplugged.org/wp-content/uploads/2014/12/Lines.pdf
            else
            {
                BresenhamsLine3D(x1, y1, z1, x2, y2, z2, false);
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
            // Check if there are any matched pairs of coordinates. You can reduce the
            // complexity of the pixelated line by making this check.
            if ((p1.X == p2.X) && (p1.Y == p2.Y))
            {
                // Always start from lower z and go to upper z. 
                int zStart = (p1.Z < p2.Z) ? p1.Z : p2.Z;
                int zEnd = (p1.Z < p2.Z) ? p2.Z : p1.Z;

                for (int i = zStart; i <= zEnd; ++i)
                {
                    // A and B are equal between the points. Arbitrary choice.
                    ClearVoxel(p1.X, p1.Y, i);
                }
            }
            else if ((p1.Y == p2.Y) && (p1.Z == p2.Z))
            {
                // Always start from lower x and go to upper x. 
                int xStart = (p1.X < p2.X) ? p1.X : p2.X;
                int xEnd = (p1.X < p2.X) ? p2.X : p1.X;

                for (int i = xStart; i <= xEnd; ++i)
                {
                    // B and Z are equal between the points. Arbitrary choice.
                    ClearVoxel(i, p1.Y, p1.Z);
                }

            }
            else if ((p1.Z == p2.Z) && (p1.X == p2.X))
            {
                // Always start from lower y and go to upper y.
                int yStart = (p1.Y < p2.Y) ? p1.Y : p2.Y;
                int yEnd = (p1.Y < p2.Y) ? p2.Y : p1.Y;

                for (int i = yStart; i <= yEnd; ++i)
                {
                    // A and Z are equal between the points. Arbitrary choice.
                    ClearVoxel(p1.X, i, p1.Z);
                }
            }
            // Otherwise, there are no straight lines, and we have to use
            // Bresenham's Line Algorithm:
            // http://csunplugged.org/wp-content/uploads/2014/12/Lines.pdf
            else
            {
                BresenhamsLine3D(p1.X, p1.Y, p1.Z, p2.X, p2.Y, p2.Z, false);
            }
		}

        /// <summary>
        /// This is an attempt to characterize Bresenham's Line Algorithm in 3D, extrapolating
        /// information from the 2D version. A 3D implementation has been modified from it's source:
        /// https://www.ict.griffith.edu.au/anthony/info/graphics/bresenham.procs
        /// 
        /// Basic idea in 2D:
        ///     Let A = 2 times change in B
        ///     Let B = A - 2 times change in A
        ///     Let M = A - change in A
        /// 
        ///     Set the starting point. 
        ///     
        ///     Then, for every position along A:
        ///     while (!atEnd):
        ///         M < 0
        ///             new pixel on same line as last pixel; 
        ///             M += A;
        ///         M >= 0
        ///             new pixel on line higher than last pixel; 
        ///             M += B;
        ///
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="z1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="z2"></param>
        private void BresenhamsLine3D(int x1, int y1, int z1, int x2, int y2, int z2, bool setting)
        {
            #region setup
            // First you need to determine the changes in the x, y, and z coordinates.
            int dx = x2 - x1;
            int dy = y2 - y1;
            int dz = z2 - z1;

            // The voxel to draw through each step of the algorithm. 
            CubeController.Point v = new Point(x1, y1, z1);

            // Determines which way to "increment", depending on the slope of the line.
            // The increment will ALWAYS be by one voxel, whether 'up' or 'down'. 
            int x_inc = (dx < 0) ? -1 : 1;
            int y_inc = (dy < 0) ? -1 : 1;
            int z_inc = (dz < 0) ? -1 : 1;

            int len_x = Math.Abs(dx);
            int len_y = Math.Abs(dy);
            int len_z = Math.Abs(dz);

            // Twice the change in value. 
            int two_dx = 2 * len_x;
            int two_dy = 2 * len_y;
            int two_dz = 2 * len_z;

            // M1 and M2
            int m1 = 0;
            int m2 = 0;
            #endregion

            // BEGIN ALGORITHM
            // If the distance to travel in x is the largest...
            if ((len_x >= len_y) && (len_x >= len_z))
            {
                m1 = two_dy - len_x;    // M = A - dx = (2 * dy) - dx
                m2 = two_dz - len_x;    // Now perform for other axis

                for (int i = 0; i < len_x; ++i)
                {
                    if (setting) {
                        SetVoxel(v.X, v.Y, v.Z);
                    } else {
                        ClearVoxel(v.X, v.Y, v.Z);
                    }

                    if (m1 > 0)
                    {
                        v.Y += y_inc;
                        m1 -= two_dx;
                    }
                    if (m2 > 0)
                    {
                        v.Z += z_inc;
                        m2 -= two_dx;
                    }
                    m1 += two_dy;
                    m2 += two_dz;
                    v.X += x_inc;
                }
            }
            // Or if the distance to travel in y is the largest...
            else if ((len_y >= len_x) && (len_y >= len_z))
            {
                m1 = two_dx - len_y;
                m2 = two_dz - len_y;

                for (int i = 0; i < len_y; ++i)
                {
                    if (setting) {
                        SetVoxel(v.X, v.Y, v.Z);
                    } else {
                        ClearVoxel(v.X, v.Y, v.Z);
                    }

                    if (m1 > 0)
                    {
                        v.X += x_inc;
                        m1 -= two_dy;
                    }
                    if (m2 > 0)
                    {
                        v.Z += z_inc;
                        m2 -= two_dy;
                    }
                    m1 += two_dx;
                    m2 += two_dz;
                    v.Y += y_inc;
                }
            }
            // Otherwise, z is the largest distance to travel...
            else
            {
                m1 = two_dy - len_z;
                m2 = two_dx - len_z;

                for (int i = 0; i < len_z; ++i)
                {
                    if (setting) {
                        SetVoxel(v.X, v.Y, v.Z);
                    } else {
                        ClearVoxel(v.X, v.Y, v.Z);
                    }

                    if (m1 > 0)
                    {
                        v.Y += y_inc;
                        m1 -= two_dz;
                    }
                    if (m2 > 0)
                    {
                        v.X += x_inc;
                        m2 -= two_dz;
                    }
                    m1 += two_dy;
                    m2 += two_dx;
                    v.Z += z_inc;
                }

            }
            if (setting) {
                SetVoxel(v.X, v.Y, v.Z);
            } else {
                ClearVoxel(v.X, v.Y, v.Z);
            }
        }

        /// <summary>
        /// Draws a rectangle using point A and point D. 
        /// 
        /// Rectangle must be drawn on the coordinate that the points share, i.e., you
        /// cannot (or should not) draw a rectangle between a point at (0,0,7) and (1,2,3),
        /// as they have no common plane to drawn cleanly across at 90° angles.
        ///
        /// </summary>
        /// <param name="A">The first point to draw from (inside-originating corner).</param>
        /// <param name="D">The terminating point (outside-opposing corner).</param>
        public void DrawRectangle(Cube.AXIS axis, CubeController.Point A, CubeController.Point D)
        {
            // Draw the lines to the non-named points:
            //
            //                SIDE 1
            //         A _______________ B
            //          |               |
            // SIDE 4   |               |   SIDE 2
            //          |_______________|
            //          C               D
            //                SIDE 3
            switch (axis)
            {
                case AXIS.AXIS_X:
                                       // X IS FIXED FOR BOTH A AND D
                    DrawLine(A, new Point(A.X, A.Y, D.Z));  // Draw SIDE 1
                    DrawLine(D, new Point(A.X, A.Y, D.Z));  // Draw SIDE 2
                    DrawLine(D, new Point(A.X, D.Y, A.Z));  // Draw SIDE 3
                    DrawLine(A, new Point(A.X, D.Y, A.Z));  // Draw SIDE 4
                    break;

                case AXIS.AXIS_Y:           // Y IS FIXED FOR BOTH A AND D
                    DrawLine(A, new Point(A.X, A.Y, D.Z));  // Draw SIDE 1
                    DrawLine(D, new Point(A.X, A.Y, D.Z));  // Draw SIDE 2
                    DrawLine(D, new Point(D.X, A.Y, A.Z));  // Draw SIDE 3
                    DrawLine(A, new Point(D.X, A.Y, A.Z));  // Draw SIDE 4
                    break;

                case AXIS.AXIS_Z:                // Z IS FIXED FOR BOTH A AND D
                    DrawLine(A, new Point(D.X, A.Y, A.Z));  // Draw SIDE 1
                    DrawLine(D, new Point(D.X, A.Y, A.Z));  // Draw SIDE 2
                    DrawLine(D, new Point(A.X, D.Y, A.Z));  // Draw SIDE 3
                    DrawLine(A, new Point(A.X, D.Y, A.Z));  // Draw SIDE 4
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Draws a circle at [center] with radius [radius]. 
        /// 
        /// Follows the Midpoint Circle Algorithm:
        /// http://csunplugged.org/wp-content/uploads/2014/12/Lines.pdf, pg 9.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public void DrawCircle(Cube.AXIS axis, CubeController.Point center, CubeController.Point rad)
        {
            int radius = int.MinValue;
            int E;  // E = -radius
            int A;  // A = +radius
            int B = 0;

            // Until B becomes greater than A, repeat the following rules in order:
            // Fill the pixel at coordinate (A + center.A, B+Center.B)

            // Increase E by (2*B + 1)
            // Increase B by 1
            // If E >= 0
            //      E -= (2 * A - 1)
            //      --A

            // This covers one octave, you must repeat 7 times with varying reflections
            // in order to cover the circle. 

            // In three dimensions:
            switch (axis)
            {
                // On the Y-Z plane, fix to X coordinate
                // and draw for Y-Z. 
                case AXIS.AXIS_X:
                    radius = (int)(Point.Distance(center.Y, rad.Y, center.Z, rad.Z));
                    E = -radius;
                    A = radius;
                    while (B < A)
                    {
                        SetVoxel(center.X,  A + center.Y,  B + center.Z); 
                        SetVoxel(center.X,  A + center.Y, -B + center.Z);
                        SetVoxel(center.X, -A + center.Y,  B + center.Z);
                        SetVoxel(center.X, -A + center.Y, -B + center.Z);
                        
                        SetVoxel(center.X,  B + center.Y,  A + center.Z);
                        SetVoxel(center.X,  B + center.Y, -A + center.Z);
                        SetVoxel(center.X, -B + center.Y,  A + center.Z);
                        SetVoxel(center.X, -B + center.Y, -A + center.Z);

                        E += ((2 * B) + 1);
                        ++B;
                        if (E >= 0)
                        {
                            E -= ((2 * A) - 1);
                            --A;
                        }
                    }
                    break;
                // On the X-Z plane, fix to Y coordinate
                // and draw for X-Z.
                case AXIS.AXIS_Y:
                    radius = (int)(Point.Distance(center.X, rad.X, center.Z, rad.Z));
                    E = -radius;
                    A = radius;
                    while (B < A)
                    {
                        SetVoxel( A + center.X, center.Y,  B + center.Z);
                        SetVoxel( A + center.X, center.Y, -B + center.Z);
                        SetVoxel(-A + center.X, center.Y,  B + center.Z);
                        SetVoxel(-A + center.X, center.Y, -B + center.Z);

                        SetVoxel( B + center.X, center.Y,  A + center.Z);
                        SetVoxel( B + center.X, center.Y, -A + center.Z);
                        SetVoxel(-B + center.X, center.Y,  A + center.Z);
                        SetVoxel(-B + center.X, center.Y, -A + center.Z);
                        E += ((2 * B) + 1);
                        ++B;
                        if (E >= 0)
                        {
                            E -= ((2 * A) - 1);
                            --A;
                        }
                    }
                    break;
                // On the X-Y plane, fix to Z coordinate
                // and draw for X-Y.
                case AXIS.AXIS_Z:
                    radius = (int)(Point.Distance(center.X, rad.X, center.Y, rad.Y));
                    E = -radius;
                    A = radius;
                    while (B < A)
                    {
                        SetVoxel( A + center.X,  B + center.Y, center.Z);
                        SetVoxel( A + center.X, -B + center.Y, center.Z);
                        SetVoxel(-A + center.X,  B + center.Y, center.Z);
                        SetVoxel(-A + center.X, -B + center.Y, center.Z);

                        SetVoxel( B + center.X,  A + center.Y, center.Z);
                        SetVoxel( B + center.X, -A + center.Y, center.Z);
                        SetVoxel(-B + center.X,  A + center.Y, center.Z);
                        SetVoxel(-B + center.X, -A + center.Y, center.Z);
                        E += ((2 * B) + 1);
                        ++B;
                        if (E >= 0)
                        {
                            E -= ((2 * A) - 1);
                            --A;
                        }
                    }
                    break;
                default:
                    break;
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
		///  Z|  /B
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
						// A-coordinate  B-coordinate	Z-Coordinate
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
					ShiftAndRoll (axis, direction);
					DelayMS (200);
				}
				ClearEntireCube ();
			}
		}

		/// <summary>
		/// Sends a message "around" the cube in a banner-like manner (rhyme!). 
		/// 
		/// A character is put on either the 0th A plane, or th 7th A plane, and
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
					// Take the character from the FRONT-FACE, and put it on the RIGHT-FACE.
					ClearPlane (AXIS.AXIS_X, 7);
					PatternSetPlane (AXIS.AXIS_X, 7, GetPlane (AXIS.AXIS_Y, 0));

					// Take the character from the LEFT-FACE, and put it on FRONT-FACE.
					ClearPlane (AXIS.AXIS_Y, 0);
					PatternSetPlane (AXIS.AXIS_Y, 0, GetPlane (AXIS.AXIS_X, 0));

					// Put the ith character on the LEFT-FACE of CUBE.
					ClearPlane (AXIS.AXIS_X, 0);
					PatternSetPlane (AXIS.AXIS_X, 0, GetChar (message [i]));

					Console.WriteLine ("LEFT FACE");
					RenderPlane (GetPlane (AXIS.AXIS_X, 0)); DelayMS (800);
					Console.WriteLine ("FRONT FACE");
					RenderPlane (GetPlane (AXIS.AXIS_Y, 0)); DelayMS (800);
					Console.WriteLine ("RIGHT FACE");
					RenderPlane (GetPlane (AXIS.AXIS_X, 7)); DelayMS (800);

					Console.WriteLine ("NEXT ITERATION\n\n\n");
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

		/// <summary>
		/// A single plane of all-set voxels is sent along
		/// [axis] away from ORIGIN towards TERMINUS. 
		/// 
		/// When the plane reaches TERMINUS, it delays for [speed] milliseconds.
		/// </summary>
		/// <param name="axis">Axis.</param>
		/// <param name="speed">Speed.</param>
		public void AxisBoing(AXIS axis, int speed)
		{
			// Always set the ORIGIN plane first. 
			SetPlane (axis, 0);

			// Because the ORIGIN plane is already set, we only have to Shift DIMENSION-1 times.
			for (int i = 0; i < DIMENSION-1; ++i) {
				ShiftAndRoll (axis, DIRECTION.FORWARD);
				RenderCube ();
				DelayMS (speed);
			}
			DelayMS (speed);
			for (int i = 0; i < DIMENSION-1; ++i) {
				ShiftAndRoll (axis, DIRECTION.REVERSE);
				RenderCube ();
				DelayMS (speed);
			}
		}

		/// <summary>
		/// Spins a line in a sinusoidal fashion. Implementation nearly directly
		/// taken from 3d.cpp::linespin(). Some of the values have been arbitrarily chosen
		/// by the team from CHR, so I've chosen not to mess with them too much.
		/// </summary>
		/// <param name="iterations">Iterations.</param>
		/// <param name="delay">Delay.</param>
		public void LineSpin(int iterations, int delay)
		{
			double top_x = 0.0, top_y = 0.0, bot_x = 0.0, bot_y = 0.0;
			double sine_base = 0.0;
			double center_x = 4.0, center_y = 4.0;

			for (int i = 0; i < iterations; ++i) {

				// For each plane
				for (int z = 0; z < DIMENSION; ++z) {
					sine_base = (double)(i / (double)20) + 
						(double)(z / ((double)(10 + (7.0 * Math.Sin ((double)i / 20)))));

					top_x = center_x + Math.Sin (sine_base) * 5;
					top_y = center_y + Math.Cos(sine_base) * 5;

					bot_x = center_x + Math.Sin (sine_base + Effect.PI) * 10;
					bot_y = center_y + Math.Cos (sine_base + Effect.PI) * 10;

					DrawLine ((int)top_x, (int)top_y, z,
						(int)bot_x, (int)bot_y, z);
				}

				RenderCube ();
				DelayMS (delay);
				ClearEntireCube ();
			}
		}
			
		/// <summary>
		/// Pretty much like line spin, but with a twist on
		/// which axis dominates the DrawLine() invocation. Leads
		/// to some interesting effects.
		/// </summary>
		/// <param name="iterations">Iterations.</param>
		/// <param name="delay">Delay.</param>
		public void VertSpiral(int iterations, int delay)
		{
			double top_x = 0.0, top_y = 0.0, bot_x = 0.0, bot_y = 0.0;
			double sine_base = 0.0;
			double center_x = 4.0, center_y = 4.0;

			for (int i = 0; i < iterations; ++i) {

				// For each plane
				for (int z = 0; z < DIMENSION; ++z) {
					sine_base = (double)(i / (double)20) + 
						(double)(z / ((double)(10 + (7.0 * Math.Sin ((double)i / 20)))));

					top_x = center_x + Math.Sin (sine_base) * 5;
					top_y = center_y + Math.Cos(sine_base) * 5;

					bot_x = center_x + Math.Sin (sine_base + Effect.PI) * 10;
					bot_y = center_y + Math.Cos (sine_base + Effect.PI) * 10;

					DrawLine (z, (int)top_x, (int)top_y,
						z, (int)bot_x, (int)bot_y);
				}

				RenderCube ();
				DelayMS (delay);
				ClearEntireCube ();
			}
		}

		/// <summary>
		/// Create a rain-shower for the specified iterations, with [delay] ms
		/// between each frame.
		/// </summary>
		/// <param name="iterations">Iterations.</param>
		/// <param name="delay">Delay.</param>
		public void Rain(int iterations, int delay)
		{
			int rnd_x = 0, rnd_y = 0;

			for (int j = 0; j < iterations; ++j){
				int randnum = _rgen.Next () % 4;

				for (int i = 0; i < randnum; ++i) {
					rnd_x = _rgen.Next () % 8;
					rnd_y = _rgen.Next () % 8;
					SetVoxel (rnd_x, rnd_y, 7);
				}

				RenderCube ();
				DelayMS (delay);
				ShiftNoRoll (Cube.AXIS.AXIS_Z, Cube.DIRECTION.REVERSE);
				RenderCube ();
			}
		}

		/// <summary>
		/// Creates a sine wave that ripples from the center of the cube.
		/// </summary>
		/// <param name="iterations">Iterations to run to.</param>
		/// <param name="delay">Animation speed (delay between frames in milliseconds).</param>
		public void Ripples(int iterations, int delay)
		{
			double 	distance = 0.0,
				   	height = 0.0,
					ripple_interval = Effect.RIPPLE_INTERVAL;

			int x = 0,
				y = 0;

			for (int i = 0; i < iterations; ++i) {
				for (x = 0; x < DIMENSION; ++x) {
					for (y = 0; y < DIMENSION; ++y) {
						// Calculate distance of this point from the center of cube in 
						// relation to the sine wave. 
						distance = Point.Distance ( 
							(double)((DIMENSION - 1) / 2), // First x is center of 0:7. 
							(double)x,					 
							(double)((DIMENSION - 1) / 2), // First y is center of 0:7.
							(double)(y / Effect.WAVE_CONSTANT)) // Second y is distance to hypotenous 
							* 8.0; 
															 

						height = 4.0 + (Math.Sin ((distance / ripple_interval) + (i / 50.0)) * 4.0);
						SetVoxel (x, y, (int)height);
					}
				}
				RenderCube ();
				DelayMS (delay);
				ClearEntireCube ();
			}
		}

		/// <summary>
		/// Shows sinusoidal wave.
		/// Will only be shown from the front of the cube, i.e. the A-Z plane. 
		/// </summary>
		/// <param name="iterations">Iterations to run the effect to.</param>
		/// <param name="delay">Delay between frames (in milliseconds).</param>
		public void SineWave(int iterations, int delay, double delta_t)
		{
			double t = 0.0;
			double[] xvals = new double[8];
			double[] zvals = new double[8];

			for (int i = 0; i < iterations; ++i) {
				for (int j = 0; j < DIMENSION; ++j){
					for (int k = 0; k < DIMENSION; ++k) {
						xvals [k] = t;
						t += delta_t;
					}
					zvals [j] = 3.5 + (Math.Sin (i+xvals[j]))*3.0;

					// Purposefully backwards for testing purposes. 
					for (int z = 0; z < DIMENSION; ++z) {
						SetVoxel (j, (int)zvals [j], z);
					}
				}
				RenderCube ();
				DelayMS (delay);
				ClearEntireCube ();
			}
		}

		/// <summary>
		/// Generates waves that spin from side-to-side. 
		/// </summary>
		/// <param name="iterations">Iterations.</param>
		/// <param name="delay">Delay.</param>
		public void SideWaves(int iterations, int delay)
		{
			double origin_x = 0.0, origin_y = 0.0, 
				distance = 0.0, height = 0.0;

			ClearEntireCube ();

			for (int i = 0; i < iterations; ++i) {
				origin_x = 3.5 + Math.Sin ((double)i / 500) * 4.0;
				origin_y = 3.5 + Math.Cos ((double)i / 500) * 4.0;

				for (int x = 0; x < DIMENSION; ++x) {
					for (int y = 0; y < DIMENSION; ++y) {
						distance = Point.Distance (origin_x, origin_y, (double)x, (double)y / Effect.WAVE_CONSTANT) * 8.0;
						height = 4.0 + Math.Sin ((distance / 2.0) + ((double)i / 500)) * 3.6;

						SetVoxel (x, y, (int)height);
					}
				}

				RenderCube ();
				DelayMS (delay);
				ClearEntireCube ();
			}
		}

		/// <summary>
		/// Takes a voxel and sends it from one face of the cube
		/// to another along the Z-axis. 
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="z">The z coordinate.</param>
		/// <param name="delay">Delay between frames.</param>
		public void SendVoxelZ(int x, int y, int z, int delay)
		{
			int dest = 0;
			for (int i = 0; i < DIMENSION; ++i) {
				if (z == (DIMENSION - 1)) {
					dest = DIMENSION - 1 - i;
					ClearVoxel (x, y, dest+1);
				} else {
					dest = i;
					ClearVoxel (x, y, dest-1);
				}
				SetVoxel (x, y, dest);
				RenderCube ();
				DelayMS (delay);
			}
		}

		/// <summary>
		/// Grows or shrinks a wireframe box given the value
		/// of [grow]. A really neat effect if used in the following
		/// manner:
		/// 	while (iteration < max){
		/// 		BoxWoopWoop(1, delay, true);	// Grow
		/// 		BoxWoopWoop(1, delay, false);	// Shrink
		/// 	} // Repeatedly
		/// </summary>
		/// <param name="iterations">Iterations to run to.</param>
		/// <param name="delay">Delay between animation frames.</param>
		/// <param name="grow">If set to <c>true</c>, then grow.</param>
		public void BoxWoopWoop(int iterations, int delay, bool grow)
		{
			ClearEntireCube ();

			for (int k = 0; k < iterations; ++k) {
				if (grow) {
					for (int i = 0; i < (DIMENSION / 2); ++i) {
						BoxWireFrame (new Point (i, i, i), (4 - i));
						RenderCube ();
						DelayMS (delay);
						ClearEntireCube ();
					}
				} else {
					for (int i = 3; i >= 0; --i) {
						BoxWireFrame (new Point (i, i, i), (4 - i));
						RenderCube ();
						DelayMS (delay);
						ClearEntireCube ();
					}
				}
			}
		}

		/// <summary>
		/// Set every voxel on the cube, but plane by plane. 
		/// </summary>
		/// <param name="delay">Delay between plane refreshes.</param>
		public void VoxelTest(int delay)
		{
			ClearEntireCube ();

			// Go Z-plane by Z-plane.
			for (int z = 0; z < DIMENSION; ++z) {
				// Set every voxel on this Z-plane, 1x1.
				for (int x = 0; x < DIMENSION; ++x) {
					for (int y = 0; y < DIMENSION; ++y) {
						SetVoxel (x, y, z);
						DelayMS (15);
						RenderCube ();
					}
					DelayMS (delay);
				}
				// Clear out the entire plane.
				ClearPlane (AXIS.AXIS_Z, z);
			}
		}

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

		/// <summary>
		/// Calculates the distance between two points.
		/// </summary>
		/// <returns>The d.</returns>
		/// <param name="x1">The first x value.</param>
		/// <param name="x2">The second x value.</param>
		/// <param name="y1">The first y value.</param>
		/// <param name="y2">The second y value.</param>
		/// <param name="zcoords">Z coordinates (if provided) for a 3D distance calculation</param>
		public static double Distance(double x1, double x2, double y1, double y2, params double[] zcoords)
		{
			double distance = 0.0;

			if (zcoords.Length > 0) {
				distance += ((zcoords [1] - zcoords [0]) * (zcoords [1] - zcoords [0]));
			}
			distance += ((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1));

			return Math.Sqrt(distance);
		}
	}
}

