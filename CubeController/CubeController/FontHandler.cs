using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CubeController
{
	public class FontHandler
	{
		private Dictionary<char, bool[][]> _alphabet;
		private static string pathDictionarySource = @"../../alphabitmap.txt";
		private Cube _cube;

		public FontHandler (Cube cube)
		{
			_alphabet = new Dictionary<char, bool[][]> ();
			_cube = cube;
			InitializeAlphabet (ref _alphabet);
		}

		private void InitializeAlphabet (ref Dictionary<char, bool[][]> alpha)
		{
			TextReader txtReader = File.OpenText (pathDictionarySource);

			string line = "";

			if (txtReader != null) {
				// Pull in line by line from the alphabitmap.txt.
				while ( (line = txtReader.ReadLine()) != null ){
					// Establish a new character map.
					bool[][] charMap = _cube.NewEmptyPlane (8);

					// Each character key is surrounded by square brackets, e.g.,
					// 	[A]. 
					// Pull out the middle, or, line[1]. 
					char key = line [1];

					// Each character bitmap is 8 rows in height. 
					for (int i = 0; i < 8; ++i) {
						line = txtReader.ReadLine ();
						// Convert bit string to boolean array using LINQ.
						charMap[i] = line.Select (c => c == '1').ToArray ();
					}

					// Add the key and its character bitmap to the dictionary. 
					alpha.Add (key, charMap);
				}

				txtReader.Close ();
			}
		}

		public bool[][] LookupByKey(char key)
		{
			return _alphabet [key];
		}
	}
}

