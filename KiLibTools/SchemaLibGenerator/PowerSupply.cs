using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SchemaLibGenerator
{
	namespace KiSchemaLib
	{
		enum SymbolTagOfPowerSupply
		{
			PositiveCircle,
			NegativeCircle,
			PositiveBar,
			NegativeBar,
			GND,
			Earth
		}

		class ComponentOfPowerSupply : Fields
		{
			public ComponentOfPowerSupply(string name, SymbolTagOfPowerSupply tag, string footprint, string vendor)
				: base(name, "#PWR", footprint, vendor)
			{
				this.tag = tag;
			}
			public SymbolTagOfPowerSupply tag;
		}

		class PowerSupply : KiSchemaLibFile
		{
			static PowerSupply()
			{
				DEFAULT_FILE_NAME = "PowerSupply";
				LABEL = "sup";
			}
			public PowerSupply()
				: this(PowerSupply.DEFAULT_FILE_NAME)
			{

			}
			public PowerSupply(string fname)
				: base(fname)
			{
				component = new List<ComponentOfPowerSupply>();
				PlotSymbolMethods = new List<PlotSymbol>()
				{
					PlotPositiveCircle,
					PlotNegativeCircle,
					PlotPositiveBar,
					PlotNegativeBar,
					PlotGND,
					PlotEarth
				};
			}

			public override string FileName { get { return this.filename; } }
			public override string Label { get { return PowerSupply.LABEL; } }

			public void AddComponentName(string name, params string[] tags)
			{
				try
				{
					for (int i = 0; i < Enum.GetNames(typeof(SymbolTagOfPowerSupply)).Length; i++)
					{
						if (tags[0] == i.ToString())
						{
							component.Add(new ComponentOfPowerSupply(name, (SymbolTagOfPowerSupply)i, "", ""));
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
				foreach (ComponentOfPowerSupply i in component)
				{
					PlotSymbolMethods[(int)(i.tag)](sw, i);
				}
			}

			private delegate void PlotSymbol(StreamWriter sw, ComponentOfPowerSupply comp);

			private List<PlotSymbol> PlotSymbolMethods;

			private void PlotPositiveCircle(StreamWriter sw, ComponentOfPowerSupply comp)
			{
				sw.WriteLine("#");
				sw.WriteLine("# {0}", comp.ComponentName);
				sw.WriteLine("#");
				sw.WriteLine("DEF {0} #PWR 0 0 N N 1 F P", comp.ComponentName);
				sw.WriteLine("F0 \"#PWR\" 0 180 40 H I C CNN");
				sw.WriteLine("F1 \"{0}\" 0 125 40 H V C CNN", comp.ComponentName);
				sw.WriteLine("F2 \"{0}\" 0 0 60 H V C CNN", comp.PCBFootprint.Text);
				sw.WriteLine("F3 \"~\" 0 0 60 H V C CNN");
				sw.WriteLine("DRAW");

				sw.WriteLine("X {0} 1 0 0 40 U 20 20 0 0 W", comp.ComponentName);
				sw.WriteLine("C 0 70 30 0 1 0 N");

				sw.WriteLine("ENDDRAW");
				sw.WriteLine("ENDDEF");
			}
			private void PlotPositiveBar(StreamWriter sw, ComponentOfPowerSupply comp)
			{
				sw.WriteLine("#");
				sw.WriteLine("# {0}", comp.ComponentName);
				sw.WriteLine("#");
				sw.WriteLine("DEF {0} #PWR 0 0 N N 1 F P", comp.ComponentName);
				sw.WriteLine("F0 \"#PWR\" 0 180 40 H I C CNN");
				sw.WriteLine("F1 \"{0}\" 0 125 40 H V C CNN", comp.ComponentName);
				sw.WriteLine("F2 \"{0}\" 0 0 60 H V C CNN", comp.PCBFootprint.Text);
				sw.WriteLine("F3 \"~\" 0 0 60 H V C CNN");
				sw.WriteLine("DRAW");

				sw.WriteLine("X {0} 1 0 0 60 U 20 20 0 0 W", comp.ComponentName);
				sw.WriteLine("C 0 75 15 0 1 0 F");
				sw.WriteLine("P 2 0 1 0  -40 75  40 75 N");

				sw.WriteLine("ENDDRAW");
				sw.WriteLine("ENDDEF");
			}
			private void PlotNegativeCircle(StreamWriter sw, ComponentOfPowerSupply comp)
			{
				sw.WriteLine("#");
				sw.WriteLine("# {0}", comp.ComponentName);
				sw.WriteLine("#");
				sw.WriteLine("DEF {0} #PWR 0 0 N N 1 F P", comp.ComponentName);
				sw.WriteLine("F0 \"#PWR\" 0 -195 40 H I C CNN");
				sw.WriteLine("F1 \"{0}\" 0 -140 40 H V C CNN", comp.ComponentName);
				sw.WriteLine("F2 \"{0}\" 0 0 60 H V C CNN", comp.PCBFootprint);
				sw.WriteLine("F3 \"~\" 0 0 60 H V C CNN");
				sw.WriteLine("DRAW");

				sw.WriteLine("X {0} 1 0 0 40 D 20 20 0 0 W", comp.ComponentName);
				sw.WriteLine("C 0 -70 30 0 1 0 N");

				sw.WriteLine("ENDDRAW");
				sw.WriteLine("ENDDEF");
			}
			private void PlotNegativeBar(StreamWriter sw, ComponentOfPowerSupply comp)
			{
				sw.WriteLine("#");
				sw.WriteLine("# {0}", comp.ComponentName);
				sw.WriteLine("#");
				sw.WriteLine("DEF {0} #PWR 0 0 N N 1 F P", comp.ComponentName);
				sw.WriteLine("F0 \"#PWR\" 0 -195 40 H I C CNN");
				sw.WriteLine("F1 \"{0}\" 0 -140 40 H V C CNN", comp.ComponentName);
				sw.WriteLine("F2 \"{0}\" 0 0 60 H V C CNN", comp.PCBFootprint);
				sw.WriteLine("F3 \"~\" 0 0 60 H V C CNN");
				sw.WriteLine("DRAW");

				sw.WriteLine("X {0} 1 0 0 60 D 20 20 0 0 W", comp.ComponentName);
				sw.WriteLine("C 0 -75 15 0 1 0 F");
				sw.WriteLine("P 2 0 1 0  -40 -75  40 -75 N");

				sw.WriteLine("ENDDRAW");
				sw.WriteLine("ENDDEF");
			}
			private void PlotGND(StreamWriter sw, ComponentOfPowerSupply comp)
			{
				sw.WriteLine("#");
				sw.WriteLine("# {0}", comp.ComponentName);
				sw.WriteLine("#");
				sw.WriteLine("DEF {0} #PWR 0 0 N N 1 F P", comp.ComponentName);
				sw.WriteLine("F0 \"#PWR\" 0 -195 40 H I C CNN");
				sw.WriteLine("F1 \"{0}\" 0 -140 40 H V C CNN", comp.ComponentName);
				sw.WriteLine("F2 \"{0}\" 0 0 60 H V C CNN", comp.PCBFootprint);
				sw.WriteLine("F3 \"~\" 0 0 60 H V C CNN");
				sw.WriteLine("DRAW");

				sw.WriteLine("X {0} 1 0 0 50 D 20 20 0 0 W", comp.ComponentName);
				sw.WriteLine("S -50 -55 50 -45 0 1 1 F");
				sw.WriteLine("P 2 0 1 10  -20 -50  -40 -80 N");
				sw.WriteLine("P 2 0 1 10  10 -50  -10 -80 N");
				sw.WriteLine("P 2 0 1 10  40 -50  20 -80 N");

				sw.WriteLine("ENDDRAW");
				sw.WriteLine("ENDDEF");
			}
			private void PlotEarth(StreamWriter sw, ComponentOfPowerSupply comp)
			{
				sw.WriteLine("#");
				sw.WriteLine("# {0}", comp.ComponentName);
				sw.WriteLine("#");
				sw.WriteLine("DEF {0} #PWR 0 0 N N 1 F P", comp.ComponentName);
				sw.WriteLine("F0 \"#PWR\" 0 -195 40 H I C CNN");
				sw.WriteLine("F1 \"{0}\" 0 -140 40 H V C CNN", comp.ComponentName);
				sw.WriteLine("F2 \"{0}\" 0 0 60 H V C CNN", comp.PCBFootprint);
				sw.WriteLine("F3 \"~\" 0 0 60 H V C CNN");
				sw.WriteLine("DRAW");

				sw.WriteLine("X {0} 1 0 0 50 D 20 20 0 0 W", comp.ComponentName);
				sw.WriteLine("P 4 0 1 0  0 -100  -50 -50  50 -50  0 -100 N");

				sw.WriteLine("ENDDRAW");
				sw.WriteLine("ENDDEF");
			}

			private List<ComponentOfPowerSupply> component;

			private readonly static string DEFAULT_FILE_NAME;
			private readonly static string LABEL;

			[Obsolete]
			private List<string> Vnum = new List<string> { "1.2", "1.8", "2.4", "2.5", "3.3", "3.6", "3.7", "4.8", "5", "6", "7.4", "8", "9", "9.4", "10", "11.1", "12", "15", "18", "22.2", "24", "36", "48" };
			[Obsolete]
			private string[] Vp = new string[] { "VCC", "VCC2", "+VCC", "VCCIO", "VDD", "VDD2", "+VDD", "VDDIO", "V+", "VM", "VBAT", "VDD_VBUS", "VDD3.3", "VDD1.8", "VDD_CORE", "VDD_MEM", "VDD_LED" };
			[Obsolete]
			private string[] Vn = new string[] { "VEE", "VEE2", "-VCC", "VEEIO", "VSS", "VSS2", "-VDD", "VSSIO", "V-" };
			[Obsolete]
			private string[] Vgnd = new string[] { "GND", "GND1", "GND2", "GND3", "GND4", "AGND", "AGND1", "AGND2", "DGND", "DGND1", "DGND2" };
			[Obsolete]
			private string[] Vearth = new string[] { "FG", "PE" };
		}


	}
}