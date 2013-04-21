using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SchemaLibGenerator
{
	namespace KiSchemaLib
	{
		enum SymbolTagOfCapacitor
		{
			Ceramic,
			Pole,
			NonePole
		}

		class ComponentOfCapacitor : Fields
		{
			public ComponentOfCapacitor(string name, string reference, string footprint, string vendor, SymbolTagOfCapacitor tag)
				: base(name, reference, footprint, vendor)
			{
				this.tag = tag;
			}
			public SymbolTagOfCapacitor tag;
		}

		class Capacitor : KiSchemaLibFile
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


		class Inductor : KiSchemaLibFile
		{
			static Inductor()
			{
				DEFAULT_FILE_NAME = "Inductor";
				LABEL = "l";
				TAGS = new string[] { "1", "2", "3" };
			}
			public Inductor()
				: this(Inductor.DEFAULT_FILE_NAME)
			{

			}
			public Inductor(string fname)
				: base(fname)
			{
				names_1 = new List<string>();
				names_2 = new List<string>();
				names_3 = new List<string>();
			}

			public override string FileName { get { return this.filename; } }
			public override string Label { get { return Inductor.LABEL; } }

			public override void WriteFile(FolderBrowserDialog fbd)
			{
				using (StreamWriter sw = new StreamWriter(@fbd.SelectedPath + "\\" + FileName + ".lib"))
				{
					WriteHeader(sw);
					WriteComponents(sw);
					WriteFooter(sw);
				}
			}

			public void AddComponentName(string name, string[] tags)
			{
				if (tags[0] == TAGS[0]) names_1.Add(name);
				else if (tags[0] == TAGS[1]) names_2.Add(name);
				else if (tags[0] == TAGS[2]) names_3.Add(name);
			}

			private void WriteComponents(StreamWriter sw)
			{
				foreach (string name in names_1)
				{
					Plot1(sw, name);
				}
				foreach (string name in names_2)
				{
					Plot2(sw, name);
				}
				foreach (string name in names_3)
				{
					Plot3(sw, name);
				}
			}

			private void Plot1(StreamWriter sw, string component_name)
			{
				/*
				#
				# L-type1
				#
				DEF L-type1 L 0 0 N Y 1 F N
				F0 "L" -125 60 40 H V L CNN
				F1 "L-type1" 0 -50 40 H V C CNN
				F2 "~" -70 0 30 V V C CNN
				F3 "~" 0 0 30 H V C CNN
				F4 "4.7m" 25 60 35 H V L CNN "Inductance"
				DRAW
				A -75 0 25 1 1799 0 1 0 N -50 0 -100 0
				A -25 0 25 1 1799 0 1 0 N 0 0 -50 0
				A 25 0 25 1 1799 0 1 0 N 50 0 0 0
				A 75 0 25 1 1799 0 1 0 N 100 0 50 0
				X ~ 1 -150 0 50 R 60 60 1 1 P
				X ~ 2 150 0 50 L 60 60 1 1 P
				*/
			}

			private void Plot2(StreamWriter sw, string component_name)
			{
				/*
				#
				# L-type2
				#
				DEF L-type2 L 0 0 N Y 1 F N
				F0 "L" -125 80 40 H V L CNN
				F1 "L-type2" 0 -50 40 H V C CNN
				F2 "~" -70 0 30 V V C CNN
				F3 "~" 0 0 30 H V C CNN
				F4 "4.7m" 25 80 35 H V L CNN "Inductance"
				DRAW
				A -75 0 25 1 1799 0 1 0 N -50 0 -100 0
				A -25 0 25 1 1799 0 1 0 N 0 0 -50 0
				A 25 0 25 1 1799 0 1 0 N 50 0 0 0
				A 75 0 25 1 1799 0 1 0 N 100 0 50 0
				S -95 40 95 50 0 1 1 F
				X ~ 1 -150 0 50 R 60 60 1 1 P
				X ~ 2 150 0 50 L 60 60 1 1 P
				ENDDRAW
				ENDDEF
				*/
			}

			private void Plot3(StreamWriter sw, string component_name)
			{
				/*
				#
				# L-type3
				#
				DEF L-type3 L 0 0 N Y 1 F N
				F0 "L" -125 80 40 H V L CNN
				F1 "L-type3" 0 -50 40 H V C CNN
				F2 "~" -70 0 30 V V C CNN
				F3 "~" 0 0 30 H V C CNN
				F4 "4.7m" 25 80 35 H V L CNN "Inductance"
				DRAW
				A -75 0 25 1 1799 0 1 0 N -50 0 -100 0
				A -25 0 25 1 1799 0 1 0 N 0 0 -50 0
				A 25 0 25 1 1799 0 1 0 N 50 0 0 0
				A 75 0 25 1 1799 0 1 0 N 100 0 50 0
				S -95 40 -65 50 0 1 1 F
				S -55 40 -25 50 0 1 1 F
				S -15 40 15 50 0 1 1 F
				S 25 40 55 50 0 1 1 F
				S 65 40 95 50 0 1 1 F
				X ~ 1 -150 0 50 R 60 60 1 1 P
				X ~ 2 150 0 50 L 60 60 1 1 P
				ENDDRAW
				ENDDEF
				*/
			}

			private List<string> names_1;
			private List<string> names_2;
			private List<string> names_3;

			private readonly static string DEFAULT_FILE_NAME;
			private readonly static string LABEL;
			private readonly static string[] TAGS;
		}

		class Diode : KiSchemaLibFile
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

		class Transistor : KiSchemaLibFile
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

		class MOSFET : KiSchemaLibFile
		{
			static MOSFET()
			{
				DEFAULT_FILE_NAME = "MOSFET";
				LABEL = "mosfet";
				TAGS = new string[] { "n", "p" };
			}
			public MOSFET()
				: this(MOSFET.DEFAULT_FILE_NAME)
			{

			}
			public MOSFET(string fname)
				: base(fname)
			{
				names_mosfet_n = new List<string>();
				names_mosfet_p = new List<string>();
			}

			public override string FileName { get { return this.filename; } }
			public override string Label { get { return MOSFET.LABEL; } }

			public void AddComponentName(string name, string[] tags)
			{
				if (tags[0] == "n")
				{
					names_mosfet_n.Add(name);
				}
				else if (tags[0] == "p")
				{
					names_mosfet_p.Add(name);
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
				foreach (string name in names_mosfet_n)
				{
					PlotMOSFETn(sw, name);
				}
				foreach (string name in names_mosfet_p)
				{
					PlotMOSFETp(sw, name);
				}
			}

			private void PlotMOSFETn(StreamWriter sw, string component_name)
			{
				sw.WriteLine("#");
				sw.WriteLine("# {0}", component_name);
				sw.WriteLine("#");
				sw.WriteLine("DEF {0} Q 0 40 Y N 1 F N", component_name);
				sw.WriteLine("F0 \"Q\" 100 100 40 H V L CNN");
				sw.WriteLine("F1 \"{0}\" 100 -100 40 H V L CNN", component_name);
				sw.WriteLine("F2 \"~\" -50 0 60 H V C CNN");
				sw.WriteLine("F3 \"~\" -50 0 60 H V C CNN");
				sw.WriteLine("DRAW");
				sw.WriteLine("C 0 0 111 0 1 0 f");
				sw.WriteLine("S -75 -53 -60 53 0 1 1 F");
				sw.WriteLine("S -50 -30 -35 -70 0 1 1 F");
				sw.WriteLine("S -50 -20 -35 20 0 1 1 F");
				sw.WriteLine("S -50 30 -35 70 0 1 1 F");
				sw.WriteLine("S 30 15 70 20 0 1 1 F");
				sw.WriteLine("P 2 0 1 0  -35 -50  0 -50 N");
				sw.WriteLine("P 2 0 1 0  -35 0  0 0 N");
				sw.WriteLine("P 2 0 1 0  -35 50  0 50 N");
				sw.WriteLine("P 4 0 1 1  -40 0  -10 -12  -10 12  -40 0 F");
				sw.WriteLine("P 4 0 1 0  0 75  50 75  50 -75  0 -75 N");
				sw.WriteLine("P 4 0 1 1  50 20  30 -20  70 -20  50 20 F");
				sw.WriteLine("X D ~ 0 150 100 D 40 40 1 1 P");
				sw.WriteLine("X G ~ -150 -50 80 R 40 40 1 1 I");
				sw.WriteLine("X S ~ 0 -150 150 U 40 40 1 1 P");
				sw.WriteLine("ENDDRAW");
				sw.WriteLine("ENDDEF");
			}

			private void PlotMOSFETp(StreamWriter sw, string component_name)
			{
				sw.WriteLine("#");
				sw.WriteLine("# {0}", component_name);
				sw.WriteLine("#");
				sw.WriteLine("DEF {0} Q 0 40 Y N 1 F N", component_name);
				sw.WriteLine("F0 \"Q\" 100 100 40 H V L CNN");
				sw.WriteLine("F1 \"{0}\" 100 -100 40 H V L CNN", component_name);
				sw.WriteLine("F2 \"~\" -50 0 60 H V C CNN");
				sw.WriteLine("F3 \"~\" -50 0 60 H V C CNN");
				sw.WriteLine("DRAW");
				sw.WriteLine("C 0 0 111 0 1 0 f");
				sw.WriteLine("S -75 -53 -60 53 0 1 1 F");
				sw.WriteLine("S -50 -30 -35 -70 0 1 1 F");
				sw.WriteLine("S -50 -20 -35 20 0 1 1 F");
				sw.WriteLine("S -50 30 -35 70 0 1 1 F");
				sw.WriteLine("S 30 -15 70 -20 0 1 1 F");
				sw.WriteLine("P 2 0 1 0  -35 -50  0 -50 N");
				sw.WriteLine("P 2 0 1 0  -35 0  0 0 N");
				sw.WriteLine("P 2 0 1 0  -35 50  0 50 N");
				sw.WriteLine("P 4 0 1 1  0 0  -30 -12  -30 12  0 0 F");
				sw.WriteLine("P 4 0 1 0  0 75  50 75  50 -75  0 -75 N");
				sw.WriteLine("P 4 0 1 0  0 75  50 75  50 -75  0 -75 N");
				sw.WriteLine("P 4 0 1 1  50 -20  30 20  70 20  50 -20 F");
				sw.WriteLine("X D ~ 0 150 100 D 40 40 1 1 P");
				sw.WriteLine("X G ~ -150 -50 80 R 40 40 1 1 I");
				sw.WriteLine("X S ~ 0 -150 150 U 40 40 1 1 P");
				sw.WriteLine("ENDDRAW");
				sw.WriteLine("ENDDEF");
			}

			private List<string> names_mosfet_n;
			private List<string> names_mosfet_p;

			private readonly static string DEFAULT_FILE_NAME;
			private readonly static string LABEL;
			private readonly static string[] TAGS;
		}

		class JFET : KiSchemaLibFile
		{
			static JFET()
			{
				DEFAULT_FILE_NAME = "JFET";
				LABEL = "jfet";
				TAGS = new string[] { "n", "p" };
			}
			public JFET()
				: this(JFET.DEFAULT_FILE_NAME)
			{

			}
			public JFET(string fname)
				: base(fname)
			{
				names_jfet_n = new List<string>();
				names_jfet_p = new List<string>();
			}

			public override string FileName { get { return this.filename; } }
			public override string Label { get { return JFET.LABEL; } }

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
				foreach (string name in names_jfet_n)
				{
					PlotJFETn(sw, name);
				}
				foreach (string name in names_jfet_p)
				{
					PlotJFETp(sw, name);
				}
			}

			private void PlotJFETn(StreamWriter sw, string component_name)
			{

			}

			private void PlotJFETp(StreamWriter sw, string component_name)
			{

			}

			private List<string> names_jfet_n;
			private List<string> names_jfet_p;

			private readonly static string DEFAULT_FILE_NAME;
			private readonly static string LABEL;
			private readonly static string[] TAGS;
		}


		class Photo : KiSchemaLibFile
		{
			static Photo()
			{
				DEFAULT_FILE_NAME = "PhotoDevice";
				LABEL = "photo";
				TAGS = new string[] { "recti", "sbd", "zener" };
			}
			public Photo()
				: this(Photo.DEFAULT_FILE_NAME)
			{

			}
			public Photo(string fname)
				: base(fname)
			{
				names_led_2pin = new List<string>();
			}

			public override string FileName { get { return this.filename; } }
			public override string Label { get { return Photo.LABEL; } }

			public void AddComponentName(string name, string[] tags)
			{
				if (tags[0] == "led")
				{
					if (tags[1] == "2pin") names_led_2pin.Add(name);
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
				foreach (string name in names_led_2pin)
				{
					PlotLED2pin(sw, name);
				}
			}

			private void PlotLED2pin(StreamWriter sw, string component_name)
			{
				sw.WriteLine("#");
				sw.WriteLine("# {0}", component_name);
				sw.WriteLine("#");
				sw.WriteLine("DEF {0} LED 0 40 N N 1 F N", component_name);
				sw.WriteLine("F0 \"LED\" -100 120 40 H V L CNN");
				sw.WriteLine("F1 \"{0}\" 0 -80 40 H V C CNN", component_name);
				sw.WriteLine("F2 \"~\" 0 0 60 H V C CNN");
				sw.WriteLine("F3 \"~\" 0 0 60 H V C CNN");
				sw.WriteLine("DRAW");
				sw.WriteLine("S 35 40 40 -40 0 1 1 F");
				sw.WriteLine("P 2 0 1 0  10 50  45 85 N");
				sw.WriteLine("P 2 0 1 0  50 50  85 85 N");
				sw.WriteLine("P 3 0 1 1  -40 40  40 0  -40 -40 F");
				sw.WriteLine("P 3 0 1 0  30 60  45 85  20 70 F");
				sw.WriteLine("P 3 0 1 0  70 60  85 85  60 70 F");
				sw.WriteLine("X A 1 -100 0 60 R 40 40 1 1 P");
				sw.WriteLine("X K 2 100 0 60 L 40 40 1 1 P");
				sw.WriteLine("ENDDRAW");
				sw.WriteLine("ENDDEF");
			}

			private List<string> names_led_2pin;

			private readonly static string DEFAULT_FILE_NAME;
			private readonly static string LABEL;
			private readonly static string[] TAGS;
		}
	}

}