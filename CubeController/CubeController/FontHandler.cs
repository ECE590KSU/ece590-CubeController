using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CubeController
{
	public class FontHandler
	{
		private Dictionary<char, bool[][]> Alphabet;
		private static string pathDictionarySource = "alphabitmap.txt";

		public FontHandler ()
		{
			Alphabet = new Dictionary<char, bool[][]> ();
			InitializeAlphabet (ref Alphabet);
		}

		private void InitializeAlphabet (ref Dictionary<char, bool[][]> alpha)
		{
			TextReader txtReader = File.OpenText (pathDictionarySource);

			string line = "";

			if (txtReader != null) {
				while ( (line = txtReader.ReadLine()) != null ){
					char key = line [1];
					for (int i = 0; i < 8; ++i) {
						line = txtReader.ReadLine ();
						bool[] characterRow = line.Select (chr => Convert.ToBoolean (chr));
					}
				}

				txtReader.Close ();
			}
		}
	}
}

