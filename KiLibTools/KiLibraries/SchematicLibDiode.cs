using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;


namespace KiLibraries
{
	namespace SchematicLib
	{
		public enum SymbolTagOfDiode
		{
			Recti,
			Zener,
			Sbd,
			Undef
		}

		public class ComponentOfDiode : Component
		{
			public ComponentOfDiode(string name, string reference, string footprint, string vendor, SymbolTagOfDiode tag)
				: base(name, reference, footprint, vendor)
			{
				this.tag = tag;
			}
			public SymbolTagOfDiode tag;
		}

		public class Diode : KiSchemaLibFile
		{
			static Diode()
			{
				DEFAULT_FILE_NAME = "Diode";
				LABEL = "d";
				TAGS = new string[] { "recti", "sbd", "zener" };
			}
			public Diode()
				: this(Diode.DEFAULT_FILE_NAME)
			{

			}
			public Diode(string fname)
				: base(fname)
			{
				names_pn = new List<string>();
				names_schottky = new List<string>();
				names_zener = new List<string>();
				names_crd = new List<string>();
			}

			public override string FileName { get { return this.filename; } }
			public override string Label { get { return Diode.LABEL; } }

			public void AddComponentName(string name, string[] tags)
			{
				if (tags[0] == Diode.TAGS[0])
				{
					names_pn.Add(name);
				}
				else if (tags[0] == Diode.TAGS[1])
				{
					names_schottky.Add(name);
				}
				else if (tags[0] == Diode.TAGS[2])
				{
					names_zener.Add(name);
				}
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
				foreach (string name in names_pn)
				{
					PlotRecti(sw, name);
				}
				foreach (string name in names_schottky)
				{
					PlotSBD(sw, name);
				}
				foreach (string name in names_zener)
				{
					PlotZener(sw, name);
				}
			}

			private void PlotRecti(StreamWriter sw, string component_name)
			{
				sw.WriteLine("#");
				sw.WriteLine("# {0}", component_name);
				sw.WriteLine("#");
				sw.WriteLine("DEF {0} D 0 40 N N 1 F N", component_name);
				sw.WriteLine("F0 \"D\" -50 70 40 H V L CNN");
				sw.WriteLine("F1 \"{0}\" 0 -80 40 H V C CNN", component_name);
				sw.WriteLine("F2 \"~\" 0 0 60 H V C CNN");
				sw.WriteLine("F3 \"~\" 0 0 60 H V C CNN");
				sw.WriteLine("DRAW");
				sw.WriteLine("S 35 40 40 -40 0 1 1 F");
				sw.WriteLine("P 3 0 1 1  -40 40  40 0  -40 -40 F");
				sw.WriteLine("X A 1 -100 0 60 R 40 40 1 1 P");
				sw.WriteLine("X K 2 100 0 60 L 40 40 1 1 P");
				sw.WriteLine("ENDDRAW");
				sw.WriteLine("ENDDEF");
			}

			private void PlotSBD(StreamWriter sw, string component_name)
			{
				sw.WriteLine("#");
				sw.WriteLine("# {0}", component_name);
				sw.WriteLine("#");
				sw.WriteLine("DEF {0} D 0 40 N N 1 F N", component_name);
				sw.WriteLine("F0 \"D\" -50 70 40 H V L CNN");
				sw.WriteLine("F1 \"{0}\" 0 -80 40 H V C CNN", component_name);
				sw.WriteLine("F2 \"~\" 0 0 60 H V C CNN");
				sw.WriteLine("F3 \"~\" 0 0 60 H V C CNN");
				sw.WriteLine("DRAW");
				sw.WriteLine("S 20 -40 25 -20 0 1 1 F");
				sw.WriteLine("S 20 -40 40 -35 0 1 1 F");
				sw.WriteLine("S 35 35 55 40 0 1 1 F");
				sw.WriteLine("S 35 40 40 -40 0 1 1 F");
				sw.WriteLine("S 50 20 55 40 0 1 1 F");
				sw.WriteLine("P 3 0 1 1  -40 40  40 0  -40 -40 F");
				sw.WriteLine("X A 1 -100 0 60 R 40 40 1 1 P");
				sw.WriteLine("X K 2 100 0 60 L 40 40 1 1 P");
				sw.WriteLine("ENDDRAW");
				sw.WriteLine("ENDDEF");
			}

			private void PlotZener(StreamWriter sw, string component_name)
			{
				sw.WriteLine("#");
				sw.WriteLine("# {0}", component_name);
				sw.WriteLine("#");
				sw.WriteLine("DEF {0} D 0 40 N N 1 F N", component_name);
				sw.WriteLine("F0 \"D\" -50 70 40 H V L CNN");
				sw.WriteLine("F1 \"{0}\" 0 -80 40 H V C CNN", component_name);
				sw.WriteLine("F2 \"~\" 0 0 60 H V C CNN");
				sw.WriteLine("F3 \"~\" 0 0 60 H V C CNN");
				sw.WriteLine("DRAW");
				sw.WriteLine("S 20 35 40 40 0 1 1 F");
				sw.WriteLine("S 35 -40 55 -35 0 1 1 F");
				sw.WriteLine("S 35 40 40 -40 0 1 1 F");
				sw.WriteLine("P 3 0 1 1  -40 40  40 0  -40 -40 F");
				sw.WriteLine("X A 1 -100 0 60 R 40 40 1 1 P");
				sw.WriteLine("X K 2 100 0 60 L 40 40 1 1 P");
				sw.WriteLine("ENDDRAW");
				sw.WriteLine("ENDDEF");
			}

			private List<string> names_pn;
			private List<string> names_schottky;
			private List<string> names_zener;
			private List<string> names_crd;

			private readonly static string DEFAULT_FILE_NAME;
			private readonly static string LABEL;
			private readonly static string[] TAGS;
		}


	}
}