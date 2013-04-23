using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;


namespace KiLibraries
{
	namespace SchematicLib
	{
		public class Transistor : KiSchemaLibFile
		{
			static Transistor()
			{
				DEFAULT_FILE_NAME = "Transistor";
				LABEL = "tr";
				TAGS = new string[] { "npn", "pnp" };
			}
			public Transistor()
				: this(Transistor.DEFAULT_FILE_NAME)
			{

			}
			public Transistor(string fname)
				: base(fname)
			{
				names_npn = new List<string>();
				names_pnp = new List<string>();
			}

			public override string FileName { get { return this.filename; } }
			public override string Label { get { return Transistor.LABEL; } }

			public void AddComponentName(string name, string[] tags)
			{
				if (tags[0] == TAGS[0]) names_npn.Add(name);
				else if (tags[0] == TAGS[1]) names_pnp.Add(name);
			}

			public override void WriteFile(FolderBrowserDialog fbd)
			{
				using (StreamWriter sw = new StreamWriter(@fbd.SelectedPath + "\\" + FileName + ".lib"))
				{
					WriteHeader(sw);
					WriteComponents(sw);
					WriteFooter(sw);
				}
			}

			private void WriteComponents(StreamWriter sw)
			{
				foreach (string name in names_npn)
				{
					PlotNPN(sw, name);
				}
				foreach (string name in names_pnp)
				{
					PlotPNP(sw, name);
				}
			}

			private void PlotNPN(StreamWriter sw, string component_name)
			{
				sw.WriteLine("#");
				sw.WriteLine("# {0}", component_name);
				sw.WriteLine("#");
				sw.WriteLine("DEF {0} Q 0 40 Y N 1 F N", component_name);
				sw.WriteLine("F0 \"Q\" 150 50 45 H V L CNN");
				sw.WriteLine("F1 \"{0}\" 150 -50 45 H V L CNN", component_name);
				sw.WriteLine("F2 \"~\" -50 0 60 H V C CNN");
				sw.WriteLine("F3 \"~\" -50 0 60 H V C CNN");
				sw.WriteLine("DRAW");
				sw.WriteLine("C 0 0 111 0 1 0 f");
				sw.WriteLine("S -60 -75 -40 75 0 1 1 F");
				sw.WriteLine("P 2 0 1 0  -50 -25  50 -100 N");
				sw.WriteLine("P 2 0 1 0  -50 25  50 100 N");
				sw.WriteLine("P 4 0 1 0  42 -94  0 -80  16 -57  42 -94 F");
				sw.WriteLine("X B ~ -150 0 100 R 40 40 1 1 I");
				sw.WriteLine("X C ~ 50 150 50 D 40 40 1 1 P");
				sw.WriteLine("X E ~ 50 -150 50 U 40 40 1 1 P");
				sw.WriteLine("ENDDRAW");
				sw.WriteLine("ENDDEF");
			}

			private void PlotPNP(StreamWriter sw, string component_name)
			{
				sw.WriteLine("#");
				sw.WriteLine("# {0}", component_name);
				sw.WriteLine("#");
				sw.WriteLine("DEF {0} Q 0 40 Y N 1 F N", component_name);
				sw.WriteLine("F0 \"Q\" 150 50 45 H V L CNN");
				sw.WriteLine("F1 \"{0}\" 150 -50 45 H V L CNN", component_name);
				sw.WriteLine("F2 \"~\" -50 0 60 H V C CNN");
				sw.WriteLine("F3 \"~\" -50 0 60 H V C CNN");
				sw.WriteLine("DRAW");
				sw.WriteLine("C 0 0 111 0 1 0 f");
				sw.WriteLine("S -60 -75 -40 75 0 1 1 F");
				sw.WriteLine("P 2 0 1 0  -50 -25  50 -100 N");
				sw.WriteLine("P 2 0 1 0  -50 25  50 100 N");
				sw.WriteLine("P 4 0 1 0  -34 -37  8 -51  -8 -73  -34 -37 F");
				sw.WriteLine("X B ~ -150 0 100 R 40 40 1 1 I");
				sw.WriteLine("X C ~ 50 150 50 D 40 40 1 1 P");
				sw.WriteLine("X E ~ 50 -150 50 U 40 40 1 1 P");
				sw.WriteLine("ENDDRAW");
				sw.WriteLine("ENDDEF");
			}

			private List<string> names_npn;
			private List<string> names_pnp;

			private readonly static string DEFAULT_FILE_NAME;
			private readonly static string LABEL;
			private readonly static string[] TAGS;
		}

	}
}