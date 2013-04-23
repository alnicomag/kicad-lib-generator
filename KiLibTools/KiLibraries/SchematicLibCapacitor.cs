using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;


namespace KiLibraries
{
	namespace SchematicLib
	{
		public enum SymbolTagOfCapacitor
		{
			Ceramic,
			Pole,
			NonePole
		}

		public class ComponentOfCapacitor : Fields
		{
			public ComponentOfCapacitor(string name, string reference, string footprint, string vendor, SymbolTagOfCapacitor tag)
				: base(name, reference, footprint, vendor)
			{
				this.tag = tag;
			}
			public SymbolTagOfCapacitor tag;
		}

		public class Capacitor : KiSchemaLibFile
		{
			static Capacitor()
			{
				DEFAULT_FILE_NAME = "Capacitor";
				LABEL = "c";
			}
			public Capacitor()
				: this(Capacitor.DEFAULT_FILE_NAME)
			{

			}
			public Capacitor(string fname)
				: base(fname)
			{
				component = new List<ComponentOfCapacitor>();
				PlotSymbolMethods = new List<PlotSymbol>()
				{
					PlotCeramic,
					PlotPole,
					PlotNonePole
				};
			}

			public override string FileName { get { return this.filename; } }
			public override string Label { get { return Capacitor.LABEL; } }

			public void AddComponentName(List<string> args)
			{
				try
				{
					for (int i = 0; i < Enum.GetNames(typeof(SymbolTagOfCapacitor)).Length; i++)
					{
						if (args[0] == i.ToString())
						{
							component.Add(new ComponentOfCapacitor(args[1], args[2], args[3], args[4], (SymbolTagOfCapacitor)i));
						}
					}
				}
				catch (IndexOutOfRangeException)
				{

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
				foreach (ComponentOfCapacitor i in component)
				{
					PlotSymbolMethods[(int)(i.tag)](sw, i);
				}
			}

			private delegate void PlotSymbol(StreamWriter sw, ComponentOfCapacitor comp);

			private List<PlotSymbol> PlotSymbolMethods;

			private void PlotCeramic(StreamWriter sw, ComponentOfCapacitor comp)      //セラミック，フィルム
			{
				sw.WriteLine("#");
				sw.WriteLine("# {0}", comp.ComponentName);
				sw.WriteLine("#");
				sw.WriteLine("DEF {0} C 0 10 N N 1 F N", comp.ComponentName);
				sw.WriteLine("F0 \"C\" 50 70 45 H V L CNN");
				sw.WriteLine("F1 \"{0}\" 50 -75 45 H V L CNN", comp.ComponentName);
				sw.WriteLine("F2 \"{0}\" 38 -150 30 H V C CNN", comp.PCBFootprint.Text);
				sw.WriteLine("F3 \"~\" 0 0 60 H V C CNN");
				sw.WriteLine("DRAW");
				sw.WriteLine("S -60 -30 60 -15 0 1 1 F");
				sw.WriteLine("S -60 30 60 15 0 1 1 F");
				sw.WriteLine("X ~ 1 0 100 75 D 40 40 1 1 P");
				sw.WriteLine("X ~ 2 0 -100 75 U 40 40 1 1 P");
				sw.WriteLine("ENDDRAW");
				sw.WriteLine("ENDDEF");
			}
			private void PlotPole(StreamWriter sw, ComponentOfCapacitor comp)     //有極電解
			{
				sw.WriteLine("#");
				sw.WriteLine("# {0}", comp.ComponentName);
				sw.WriteLine("#");
				sw.WriteLine("DEF {0} C 0 10 N N 1 F N", comp.ComponentName);
				sw.WriteLine("F0 \"C\" 50 70 45 H V L CNN");
				sw.WriteLine("F1 \"{0}\" 50 -75 45 H V L CNN", comp.ComponentName);
				sw.WriteLine("F2 \"{0}\" 38 -150 30 H V C CNN", comp.PCBFootprint.Text);
				sw.WriteLine("F3 \"~\" 0 0 60 H V C CNN");
				sw.WriteLine("DRAW");
				sw.WriteLine("T 0 -30 55 30 0 0 0 +  Normal 0 C C");
				sw.WriteLine("S -60 -35 60 -20 0 1 1 F");
				sw.WriteLine("S -60 35 60 20 0 1 1 F");
				sw.WriteLine("P 2 0 1 10  -20 25  -50 -25 N");
				sw.WriteLine("P 2 0 1 10  15 25  -15 -25 N");
				sw.WriteLine("P 2 0 1 10  50 25  20 -25 N");
				sw.WriteLine("X p 1 0 100 70 D 40 40 1 1 P");
				sw.WriteLine("X m 2 0 -100 70 U 40 40 1 1 P");
				sw.WriteLine("ENDDRAW");
				sw.WriteLine("ENDDEF");
			}
			private void PlotNonePole(StreamWriter sw, ComponentOfCapacitor comp)        //無極性電解
			{
				sw.WriteLine("#");
				sw.WriteLine("# {0}", comp.ComponentName);
				sw.WriteLine("#");
				sw.WriteLine("DEF {0} C 0 10 N N 1 F N", comp.ComponentName);
				sw.WriteLine("F0 \"C\" 50 70 45 H V L CNN");
				sw.WriteLine("F1 \"{0}\" 50 -75 45 H V L CNN", comp.ComponentName);
				sw.WriteLine("F2 \"{0}\" 38 -150 30 H V C CNN", comp.PCBFootprint.Text);
				sw.WriteLine("F3 \"~\" 0 0 60 H V C CNN");
				sw.WriteLine("DRAW");
				sw.WriteLine("S -60 -35 60 -20 0 1 1 F");
				sw.WriteLine("S -60 35 60 20 0 1 1 F");
				sw.WriteLine("P 2 0 1 10  -20 25  -50 -25 N");
				sw.WriteLine("P 2 0 1 10  15 25  -15 -25 N");
				sw.WriteLine("P 2 0 1 10  50 25  20 -25 N");
				sw.WriteLine("X p 1 0 100 70 D 40 40 1 1 P");
				sw.WriteLine("X m 2 0 -100 70 U 40 40 1 1 P");
				sw.WriteLine("ENDDRAW");
				sw.WriteLine("ENDDEF");
			}

			private List<ComponentOfCapacitor> component;

			private readonly static string DEFAULT_FILE_NAME;
			private readonly static string LABEL;
		}

	}
}