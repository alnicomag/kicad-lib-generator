using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;


namespace KiLibraries
{
	namespace SchematicLib
	{
		public enum SymbolTagOfResistor : int
		{
			FixedIEC = 0,
			FixedAmerican,
			VariableIEC,
			VariableAmerican
		}

		public class ResistorComponent : Component
		{
			public ResistorComponent(string name, string reference, string footprint, string vendor, SymbolTagOfResistor tag)
				: base(name, reference, footprint, vendor)
			{
				this.tag = tag;
			}
			public SymbolTagOfResistor tag;
		}

		public class Resistor : KiSchemaLibFile
		{
			static Resistor()
			{
				DEFAULT_FILE_NAME = "Resistor";
				LABEL = "r";
			}
			public Resistor()
				: this(Resistor.DEFAULT_FILE_NAME)
			{

			}
			public Resistor(string fname)
				: base(fname)
			{
				components = new List<ResistorComponent>();
				PlotSymbolMethods = new List<PlotSymbol>()
				{
					PlotFixedIEC,
					PlotFixedAmerican,
					PlotVariableIEC,
					PlotVariableAmerican
				};
			}

			public override string FileName { get { return this.filename; } }
			public override string Label { get { return Resistor.LABEL; } }

			public void AddComponentName(List<string> args)
			{
				try
				{
					for (int i = 0; i < Enum.GetNames(typeof(SymbolTagOfResistor)).Length; i++)
					{
						if (args[0] == i.ToString())
						{
							components.Add(new ResistorComponent(args[1], args[2], args[3], args[4], (SymbolTagOfResistor)i));
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
				foreach (ResistorComponent i in components)
				{
					PlotSymbolMethods[(int)(i.tag)](sw, i);
				}
			}

			private delegate void PlotSymbol(StreamWriter sw, ResistorComponent comp);

			private List<PlotSymbol> PlotSymbolMethods;

			private void PlotFixedIEC(StreamWriter sw, ResistorComponent comp)
			{
				sw.WriteLine("#");
				sw.WriteLine("# {0}", comp.ComponentName);
				sw.WriteLine("#");
				sw.WriteLine("DEF {0} R 0 0 N Y 1 F N", comp.ComponentName);
				sw.WriteLine("F0 \"R\" -100 60 45 H V L CNN");
				sw.WriteLine("F1 \"{0}\" 0 -70 45 H V C CNN", comp.ComponentName);
				sw.WriteLine("F2 \"{0}\" -70 0 30 V V C CNN", comp.PCBFootprint.Text);
				sw.WriteLine("F3 \"~\" 0 0 30 H V C CNN");
				sw.WriteLine("DRAW");
				sw.WriteLine("S -100 30 100 -30 0 1 0 N");
				sw.WriteLine("X ~ 1 -150 0 50 R 60 60 1 1 P");
				sw.WriteLine("X ~ 2 150 0 50 L 60 60 1 1 P");
				sw.WriteLine("ENDDRAW");
				sw.WriteLine("ENDDEF");
			}
			private void PlotFixedAmerican(StreamWriter sw, ResistorComponent comp)
			{
				sw.WriteLine("#");
				sw.WriteLine("# {0}", comp.ComponentName);
				sw.WriteLine("#");
				sw.WriteLine("DEF {0} R 0 0 N Y 1 F N", comp.ComponentName);
				sw.WriteLine("F0 \"R\" -125 60 45 H V L CNN");
				sw.WriteLine("F1 \"{0}\" 0 -70 45 H V C CNN", comp.ComponentName);
				sw.WriteLine("F2 \"{0}\" -70 0 30 V V C CNN", comp.PCBFootprint.Text);
				sw.WriteLine("F3 \"~\" 0 0 30 H V C CNN");
				sw.WriteLine("DRAW");
				sw.WriteLine("P 8 0 1 0  -105 0  -87 30  -52 -30  -17 30  17 -30  52 30  87 -30  105 0 N");
				sw.WriteLine("X ~ 1 -150 0 45 R 60 60 1 1 P");
				sw.WriteLine("X ~ 2 150 0 45 L 60 60 1 1 P");
				sw.WriteLine("ENDDRAW");
				sw.WriteLine("ENDDEF");
			}
			private void PlotVariableIEC(StreamWriter sw, ResistorComponent comp)
			{

			}
			private void PlotVariableAmerican(StreamWriter sw, ResistorComponent comp)
			{

			}

			private List<ResistorComponent> components;

			private readonly static string DEFAULT_FILE_NAME;
			private readonly static string LABEL;
		}

	}
}