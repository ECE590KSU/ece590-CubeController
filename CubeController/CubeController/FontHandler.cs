using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;

namespace CubeController
{
	public class FontHandler
	{
		private Dictionary<char, bool[][]> _alphabet;
		private static string pathDictionarySource = @"../../alphabitmap.txt";

		/// <summary>
		/// Initializes a new instance of the FontHandler class.
		/// 
		/// Non-default public constructor. 
		/// </summary>
		public FontHandler ()
		{
			_alphabet = new Dictionary<char, bool[][]> ();
			InitializeAlphabet (ref _alphabet);
		}

		/// <summary>
		/// Initializes the alphabet lookup Dictionary.
		/// </summary>
		/// <param name="alpha">A reference to the alphabet lookup Dictionary.</param>
		private void InitializeAlphabet (ref Dictionary<char, bool[][]> alpha)
		{
			TextReader txtReader = File.OpenText (pathDictionarySource);

			string line = "";

			if (txtReader != null) {
				// Pull in line by line from the alphabitmap.txt.
				while ( (line = txtReader.ReadLine()) != null ){
					// Establish a new character map.
					bool[][] charMap = NewEmptyPlane (8);

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

		/// <summary>
		/// Public method to lookup the private Dictionary values by key.
		/// </summary>
		/// <returns>The the character bitmap for the char key.</returns>
		/// <param name="key">The character key to lookup the character bitmap by.</param>
		public bool[][] LookupByKey(char key)
		{
			return _alphabet [key];
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
	}
}

