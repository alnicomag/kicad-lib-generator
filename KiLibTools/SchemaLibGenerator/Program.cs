using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace SchemaLibGenerator
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			Stream stream;      //コンポーネントリストファイルのストリーム
			OpenFileDialog ofd = new OpenFileDialog()
			{
				Title = "開くコンポーネントリストファイルを選択してください",
				InitialDirectory = @Directory.GetCurrentDirectory(),
				Filter = "CSVファイル(*.csv)|*.csv|すべてのファイル(*.*)|*.*",
				FilterIndex = 1
			};
			do
			{
				if (ofd.ShowDialog() == DialogResult.Cancel)
				{
					ofd.FileName = @Directory.GetCurrentDirectory() + "database.csv";
				}
			}
			while ((stream = ofd.OpenFile()) == null);

			KiSchemaLib.Resistor r = new KiSchemaLib.Resistor();
			KiSchemaLib.Capacitor c = new KiSchemaLib.Capacitor();
			KiSchemaLib.Transistor tr = new KiSchemaLib.Transistor();
			KiSchemaLib.MOSFET mosfet = new KiSchemaLib.MOSFET();
			KiSchemaLib.PowerSupply ps = new KiSchemaLib.PowerSupply();
			KiSchemaLib.Photo pd = new KiSchemaLib.Photo();
			KiSchemaLib.Diode d = new KiSchemaLib.Diode();

			using (StreamReader sr = new StreamReader(stream))
			{
				string line;
				while ((line = sr.ReadLine()) != null)
				{
					if (line.Substring(0, 1) != "#")
					{
						string[] divline = line.Split(',');
						if (divline[0] == ps.Label)
						{
							ps.AddComponentName(divline[1], divline[2]);
						}
						else if (divline[0] == r.Label)
						{
							r.AddComponentName(divline[1], divline[2], divline[3], divline[4]);
						}
						else if (divline[0] == c.Label)
						{
							c.AddComponentName(divline[1], divline[2], divline[3], divline[4]);
						}
						else if (divline[0] == tr.Label)
						{
							tr.AddComponentName(divline[3], new string[] { divline[1], divline[2] });
						}
						else if (divline[0] == mosfet.Label)
						{
							mosfet.AddComponentName(divline[3], new string[] { divline[1], divline[2] });
						}
						else if (divline[0] == pd.Label)
						{
							pd.AddComponentName(divline[3], new string[] { divline[1], divline[2] });
						}
						else if (divline[0] == d.Label)
						{
							d.AddComponentName(divline[3], new string[] { divline[1], divline[2] });
						}
					}
				}
			}
			stream.Close();

			FolderBrowserDialog fbd = new FolderBrowserDialog()
			{
				Description = "ライブラリを保存するフォルダを選択して下さい．",
				RootFolder = System.Environment.SpecialFolder.Desktop,
				SelectedPath = @Directory.GetCurrentDirectory(),
				ShowNewFolderButton = true
			};
			fbd.ShowDialog();

			ps.WriteFile(fbd);
			r.WriteFile(fbd);
			c.WriteFile(fbd);
		//	tr.WriteFile(fbd);
		//	mosfet.WriteFile(fbd);
		//	pd.WriteFile(fbd);
		//	d.WriteFile(fbd);
		//	new KiSchemaLib.ConMale().WriteFile(fbd);
		//	new KiSchemaLib.ConFemale().WriteFile(fbd);
		}
	}

	
	namespace KiSchemaLib
	{
		/// <summary>
		/// .libファイルに保存される回路図記号ライブラリを表す抽象クラス
		/// </summary>
		abstract class KiSchemaLibFile
		{
			public KiSchemaLibFile(string fname)
			{
				filename = fname;
			}

			/// <summary>
			/// 保存する時の.libファイルの名前
			/// </summary>
			public abstract string FileName { get; }
			/// <summary>
			/// 作成元csvファイルのレコード識別文字列
			/// </summary>
			public abstract string Label { get; }

			public virtual void WriteFile(FolderBrowserDialog fbd) { }

			protected string filename;

			protected static void WriteHeader(StreamWriter sw)
			{
				sw.WriteLine("EESchema-LIBRARY Version 2.3  Date: " + DateTime.Now.ToString());
				sw.WriteLine("#encoding utf-8");
			}
			protected static void WriteFooter(StreamWriter sw)
			{
				sw.WriteLine("#");
				sw.WriteLine("#End Library");
			}
		}

		/// <summary>
		/// Field of SchematicLibrariesFiles (contains UserFields).
		/// </summary>
		public class SchemaField
		{
			public SchemaField(int id)
				: this(id, "", "~", 0, 0, 60, true)
			{

			}
			public SchemaField(int id, string value)
				: this(id, "", value, 0, 0, 60, true)
			{

			}
			public SchemaField(int id, string name, string value)
				: this(id, name, value, 0, 0, 60, true)
			{

			}
			public SchemaField(int id, string name, string value, int x, int y, int size, bool reserved)
			{
				ID = id;
				Name = name;
				Value = value;
				X = x;
				Y = y;
				Size = size;
				Visible = Visible.V;
				Orient = Orientation.H;
				HAlign = AlignH.C;
				VAlign = AlignV.C;
				FontShape = FontShape.NN;
				Reserved = reserved;
			}

			public bool Reserved
			{
				get { return reserved; }
				set
				{
					reserved = value;
					if (value)
					{
						Name = "";
					}
				}
			}
			public int ID { get { return id; } set { id = value; } }
			public string Name { get { return name; } set { name = value; } }
			public string Value { get { return value; } set { this.value = value; } }
			public int X { get { return x; } set { x = value; } }
			public int Y { get { return y; } set { y = value; } }
			public int Size { get { return size; } set { size = value; } }
			public Visible Visible { get { return visi; } set { visi = value; } }
			public Orientation Orient { get { return orient; } set { orient = value; } }
			public AlignH HAlign { get { return h_align; } set { h_align = value; } }
			public AlignV VAlign { get { return v_align; } set { v_align = value; } }
			public FontShape FontShape { get { return fontshape; } set { fontshape = value; } }

			public string ToString()
			{
				string field;
				field = "F" + ID.ToString() + " "
					+ "\"" + Value + "\" "
					+ X.ToString() + " "
					+ Y.ToString() + " "
					+ Size.ToString() + " "
					+ Orient.ToString() + " "
					+ Visible.ToString() + " "
					+ HAlign.ToString() + " "
					+ VAlign.ToString() + FontShape.ToString();
				if (!Reserved)
				{
					field += " \"" + Name + "\"";
				}

				return field;
			}

			public void SetLibField(string line)
			{
				List<string> tempstrs = LineParse(line);

				ID = int.Parse(tempstrs[0].Substring(1, tempstrs[0].Length - 1));
				Value = tempstrs[1];
				X = int.Parse(tempstrs[2]);
				Y = int.Parse(tempstrs[3]);
				Size = int.Parse(tempstrs[4]);
				Orient = (tempstrs[5] == Orientation.H.ToString() ? Orientation.H : Orientation.V);
				Visible = (tempstrs[6] == Visible.I.ToString() ? Visible.I : Visible.V);
				HAlign = (tempstrs[7] == AlignH.C.ToString() ? AlignH.C : tempstrs[7] == AlignH.L.ToString() ? AlignH.L : AlignH.R);
				VAlign = (tempstrs[8].Substring(0, 1) == AlignV.C.ToString() ? AlignV.C : tempstrs[8].Substring(0, 1) == AlignV.B.ToString() ? AlignV.B : AlignV.T);

				string a = tempstrs[8].Substring(1, 2);
				if (a == FontShape.NN.ToString())
				{
					FontShape = FontShape.NN;
				}
				else if (a == FontShape.NB.ToString())
				{
					FontShape = FontShape.NB;
				}
				else if (a == FontShape.IN.ToString())
				{
					FontShape = FontShape.IN;
				}
				else if (a == FontShape.IB.ToString())
				{
					FontShape = FontShape.IB;
				}

				if (tempstrs.Count == 10)
				{
					Name = tempstrs[9];
					Reserved = false;
				}
			}

			private List<string> LineParse(string line)
			{
				List<string> tempstrs = new List<string>();

				bool in_double_quotation = false;
				int readcount = 0;
				tempstrs.Add("");
				while (readcount < line.Length)
				{
					if ((line[readcount] == ' ') & (!in_double_quotation))
					{
						tempstrs.Add("");
					}
					else
					{
						if (line[readcount] == '\"')
						{
							in_double_quotation = !in_double_quotation;
						}
						else
						{
							tempstrs[tempstrs.Count - 1] += line[readcount];
						}
					}
					readcount++;
				}

				return tempstrs;
			}

			private int id;					//フィールド番号
			private string name;			//フィールド名
			private string value;			//フィールド値
			private int x;					//[mil]
			private int y;					//[mil]
			private int size;				//[mil]

			private Visible visi;
			private Orientation orient;
			private AlignH h_align;
			private AlignV v_align;
			private FontShape fontshape;

			private bool reserved;
		}

		public enum Orientation
		{
			H,
			V
		}
		public enum Visible
		{
			V,
			I
		}
		public enum AlignH
		{
			C,
			R,
			L
		}
		public enum AlignV
		{
			C,
			B,
			T
		}
		public enum FontShape
		{
			NN,
			IN,
			NB,
			IB
		}

		class Fields
		{
			public Fields(string name, string footprint, string vendor)
			{
				ComponentName = name;
				PCBFootprint = footprint;
				Vendor = vendor;
			}
			public string ComponentName { get; set; }
			public string Value { get; set; }
			public string PCBFootprint { get; set; }
			public string UserDocLink { get; set; }
			public string Vendor { get; set; }
		}


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
				: base(name, footprint, vendor)
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
				catch(IndexOutOfRangeException)
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
				sw.WriteLine("F2 \"{0}\" 0 0 60 H V C CNN",comp.PCBFootprint);
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
				sw.WriteLine("F2 \"{0}\" 0 0 60 H V C CNN",comp.PCBFootprint);
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
				sw.WriteLine("F2 \"{0}\" 0 0 60 H V C CNN",comp.PCBFootprint);
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
				sw.WriteLine("F2 \"{0}\" 0 0 60 H V C CNN",comp.PCBFootprint);
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
				sw.WriteLine("F2 \"{0}\" 0 0 60 H V C CNN",comp.PCBFootprint);
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


		enum SymbolTagOfResistor
		{
			FixedIEC,
			FixedAmerican,
			VariableIEC,
			VariableAmerican
		}

		class ComponentOfResistor : Fields
		{
			public ComponentOfResistor(string name, SymbolTagOfResistor tag, string footprint, string vendor)
				: base(name, footprint, vendor)
			{
				this.tag = tag;
			}
			public SymbolTagOfResistor tag;
		}

		class Resistor : KiSchemaLibFile
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
				component = new List<ComponentOfResistor>();
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

			public void AddComponentName(string name, params string[] tags)
			{
				try
				{
					for (int i = 0; i < Enum.GetNames(typeof(SymbolTagOfResistor)).Length; i++)
					{
						if (tags[0] == i.ToString())
						{
							component.Add(new ComponentOfResistor(name, (SymbolTagOfResistor)i, tags[1], tags[2]));
						}
					}
				}
				catch(IndexOutOfRangeException)
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
				foreach (ComponentOfResistor i in component)
				{
					PlotSymbolMethods[(int)(i.tag)](sw, i);
				}
			}

			private delegate void PlotSymbol(StreamWriter sw, ComponentOfResistor comp);

			private List<PlotSymbol> PlotSymbolMethods;

			private void PlotFixedIEC(StreamWriter sw, ComponentOfResistor comp)
			{
				sw.WriteLine("#");
				sw.WriteLine("# {0}", comp.ComponentName);
				sw.WriteLine("#");
				sw.WriteLine("DEF {0} R 0 0 N Y 1 F N", comp.ComponentName);
				sw.WriteLine("F0 \"R\" -100 60 45 H V L CNN");
				sw.WriteLine("F1 \"{0}\" 0 -70 45 H V C CNN", comp.ComponentName);
				sw.WriteLine("F2 \"{0}\" -70 0 30 V V C CNN", comp.PCBFootprint);
				sw.WriteLine("F3 \"~\" 0 0 30 H V C CNN");
				sw.WriteLine("DRAW");
				sw.WriteLine("S -100 30 100 -30 0 1 0 N");
				sw.WriteLine("X ~ 1 -150 0 50 R 60 60 1 1 P");
				sw.WriteLine("X ~ 2 150 0 50 L 60 60 1 1 P");
				sw.WriteLine("ENDDRAW");
				sw.WriteLine("ENDDEF");
			}
			private void PlotFixedAmerican(StreamWriter sw, ComponentOfResistor comp)
			{
				sw.WriteLine("#");
				sw.WriteLine("# {0}", comp.ComponentName);
				sw.WriteLine("#");
				sw.WriteLine("DEF {0} R 0 0 N Y 1 F N", comp.ComponentName);
				sw.WriteLine("F0 \"R\" -125 60 45 H V L CNN");
				sw.WriteLine("F1 \"{0}\" 0 -70 45 H V C CNN", comp.ComponentName);
				sw.WriteLine("F2 \"{0}\" -70 0 30 V V C CNN",comp.PCBFootprint);
				sw.WriteLine("F3 \"~\" 0 0 30 H V C CNN");
				sw.WriteLine("DRAW");
				sw.WriteLine("P 8 0 1 0  -105 0  -87 30  -52 -30  -17 30  17 -30  52 30  87 -30  105 0 N");
				sw.WriteLine("X ~ 1 -150 0 45 R 60 60 1 1 P");
				sw.WriteLine("X ~ 2 150 0 45 L 60 60 1 1 P");
				sw.WriteLine("ENDDRAW");
				sw.WriteLine("ENDDEF");
			}
			private void PlotVariableIEC(StreamWriter sw, ComponentOfResistor comp)
			{

			}
			private void PlotVariableAmerican(StreamWriter sw, ComponentOfResistor comp)
			{

			}

			private List<ComponentOfResistor> component;

			private readonly static string DEFAULT_FILE_NAME;
			private readonly static string LABEL;
		}


		enum SymbolTagOfCapacitor
		{
			Ceramic,
			Pole,
			NonePole
		}

		class ComponentOfCapacitor : Fields
		{
			public ComponentOfCapacitor(string name, SymbolTagOfCapacitor tag, string footprint, string vendor)
				: base(name, footprint, vendor)
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

			public void AddComponentName(string name, params string[] tags)
			{
				try
				{
					for (int i = 0; i < Enum.GetNames(typeof(SymbolTagOfResistor)).Length; i++)
					{
						if (tags[0] == i.ToString())
						{
							component.Add(new ComponentOfCapacitor(name, (SymbolTagOfCapacitor)i, tags[1], tags[2]));
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
				sw.WriteLine("F2 \"{0}\" 38 -150 30 H V C CNN",comp.PCBFootprint);
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
				sw.WriteLine("F2 \"{0}\" 38 -150 30 H V C CNN",comp.PCBFootprint);
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
				sw.WriteLine("F2 \"{0}\" 38 -150 30 H V C CNN",comp.PCBFootprint);
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

		class ConMale : KiSchemaLibFile
		{
			static ConMale()
			{
				DEFAULT_FILE_NAME = "ConMale";
				LABEL = "";
			}
			public ConMale()
				: this(ConMale.DEFAULT_FILE_NAME)
			{

			}
			public ConMale(string fname)
				: base(fname)
			{

			}

			private const int MAX_SINGLE_PIN = 99;
			private const int MAX_DOUBLE_PIN = 99;

			public override string FileName { get { return this.filename; } }
			public override string Label { get { throw new NotImplementedException(); } }

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
				PlotMaleSingle(sw);
				PlotMaleDouble(sw);
			}

			private void PlotMaleSingle(StreamWriter sw)
			{
				for (int pin = 1; pin <= MAX_SINGLE_PIN; pin++)
				{
					string component_name = "con-male-single-" + pin.ToString("D2");
					sw.WriteLine("#");
					sw.WriteLine("# {0}", component_name);
					sw.WriteLine("#");
					sw.WriteLine("DEF {0} CN 0 1 Y N 1 F N", component_name);
					sw.WriteLine("F0 \"CN\" -50 {0} 40 H V L CNN", 50 * pin + 30);
					sw.WriteLine("F1 \"{0}\" 50 {1} 40 H V C CNN", component_name, -50 * pin - 40);
					sw.WriteLine("F2 \"~\" -50 {0} 60 H V C CNN", 50 * (pin - 1));
					sw.WriteLine("F3 \"~\" -50 {0} 60 H V C CNN", 50 * (pin - 1));
					sw.WriteLine("DRAW");
					sw.WriteLine("S -50 {0} 150 -{0} 0 1 0 f", 50 * pin);		//外枠
					for (int pinid = 1; pinid <= pin; pinid++)
					{
						int pin_y = 50 * pin - 50 - (pinid - 1) * 100;
						const int WIDTH = 15;
						sw.WriteLine("X {0} {0} -200 {1} {2} R 50 50 1 1 P", pinid, pin_y, 200 - WIDTH);		//ピン
						sw.WriteLine("C 0 {0} {1} 0 1 1 F", pin_y, WIDTH);
						sw.WriteLine("C 50 {0} {1} 0 1 1 F", pin_y, WIDTH);
						sw.WriteLine("S 0 {0} 50 {1} 0 1 1 F", pin_y - WIDTH, pin_y + WIDTH);
					}
					sw.WriteLine("ENDDRAW");
					sw.WriteLine("ENDDEF");
				}
			}

			private void PlotMaleDouble(StreamWriter sw)
			{
				for (int pin = 2; pin <= MAX_DOUBLE_PIN; pin += 2)
				{
					string component_name = "con-male-double-" + pin.ToString("D2");
					sw.WriteLine("#");
					sw.WriteLine("# {0}", component_name);
					sw.WriteLine("#");
					sw.WriteLine("DEF {0} CN 0 1 Y N 1 F N", component_name);
					sw.WriteLine("F0 \"CN\" -150 {0} 40 H V L CNN", 25 * pin + 30);
					sw.WriteLine("F1 \"{0}\" 0 {1} 40 H V C CNN", component_name, -25 * pin - 40);
					sw.WriteLine("F2 \"~\" -50 {0} 60 H V C CNN", 25 * (pin - 1));
					sw.WriteLine("F3 \"~\" -50 {0} 60 H V C CNN", 25 * (pin - 1));
					sw.WriteLine("DRAW");
					sw.WriteLine("S -150 {0} 150 -{0} 0 1 0 f", 25 * pin);		//外枠
					for (int pinid = 1; pinid <= pin; pinid++)
					{
						const int WIDTH = 15;
						if (pinid % 2 == 1)
						{
							int pin_y = 25 * pin - 50 - (int)((double)pinid * 0.5) * 100;
							sw.WriteLine("X {0} {0} -300 {1} {2} R 50 50 1 1 P", pinid, pin_y, 200 - WIDTH);		//ピン
							sw.WriteLine("C -100 {0} {1} 0 1 1 F", pin_y, WIDTH);
							sw.WriteLine("C -50 {0} {1} 0 1 1 F", pin_y, WIDTH);
							sw.WriteLine("S -100 {0} -50 {1} 0 1 1 F", pin_y - WIDTH, pin_y + WIDTH);
						}
						else
						{
							int pin_y = 25 * pin - 50 - (pinid / 2 - 1) * 100;
							sw.WriteLine("X {0} {0} 300 {1} {2} L 50 50 1 1 P", pinid, pin_y, 200 - WIDTH);		//ピン
							sw.WriteLine("C 100 {0} {1} 0 1 1 F", pin_y, WIDTH);
							sw.WriteLine("C 50 {0} {1} 0 1 1 F", pin_y, WIDTH);
							sw.WriteLine("S 50 {0} 100 {1} 0 1 1 F", pin_y - WIDTH, pin_y + WIDTH);
						}
					}
					sw.WriteLine("ENDDRAW");
					sw.WriteLine("ENDDEF");
				}
			}

			private readonly static string DEFAULT_FILE_NAME;
			private readonly static string LABEL;
			private readonly static string[] TAGS;
		}

		class ConFemale : KiSchemaLibFile
		{
			static ConFemale()
			{
				DEFAULT_FILE_NAME = "ConFemale";
				LABEL = "";
			}
			public ConFemale()
				: this(ConFemale.DEFAULT_FILE_NAME)
			{

			}
			public ConFemale(string fname)
				: base(fname)
			{

			}

			private const int MAX_SINGLE_PIN = 99;
			private const int MAX_DOUBLE_PIN = 99;

			public override string FileName { get { return this.filename; } }
			public override string Label { get { throw new NotImplementedException(); } }

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
				PlotFemaleSingle(sw);
				PlotFemaleDouble(sw);
			}

			private void PlotFemaleSingle(StreamWriter sw)
			{
				for (int pin = 1; pin <= MAX_SINGLE_PIN; pin++)
				{
					string component_name = "con-female-single-" + pin.ToString("D2");
					sw.WriteLine("#");
					sw.WriteLine("# {0}", component_name);
					sw.WriteLine("#");
					sw.WriteLine("DEF {0} CN 0 1 Y N 1 F N", component_name);
					sw.WriteLine("F0 \"CN\" -50 {0} 40 H V L CNN", 50 * pin + 30);
					sw.WriteLine("F1 \"{0}\" 50 {1} 40 H V C CNN", component_name, -50 * pin - 40);
					sw.WriteLine("F2 \"~\" -50 {0} 60 H V C CNN", 50 * (pin - 1));
					sw.WriteLine("F3 \"~\" -50 {0} 60 H V C CNN", 50 * (pin - 1));
					sw.WriteLine("DRAW");
					sw.WriteLine("S -50 {0} 150 -{0} 0 1 0 f", 50 * pin);		//外枠
					for (int pinid = 1; pinid <= pin; pinid++)
					{
						int pin_y = 50 * pin - 50 - (pinid - 1) * 100;
						const int WIDTH = 18;
						sw.WriteLine("X {0} {0} -200 {1} {2} R 50 50 1 1 P", pinid, pin_y, 200 - WIDTH);		//ピン
						sw.WriteLine("A 0 {0} {1} 901 -901 0 1 0 N 0 {2} 0 {3}", pin_y, WIDTH, pin_y + WIDTH, pin_y - WIDTH);
						sw.WriteLine("P 2 0 1 0  0 {0}  25 {0} F", pin_y + WIDTH);
						sw.WriteLine("P 2 0 1 0  0 {0}  25 {0} F", pin_y - WIDTH);
					}
					sw.WriteLine("ENDDRAW");
					sw.WriteLine("ENDDEF");
				}
			}

			private void PlotFemaleDouble(StreamWriter sw)
			{
				for (int pin = 2; pin <= MAX_DOUBLE_PIN; pin += 2)
				{
					string component_name = "con-female-double-" + pin.ToString("D2");
					sw.WriteLine("#");
					sw.WriteLine("# {0}", component_name);
					sw.WriteLine("#");
					sw.WriteLine("DEF {0} CN 0 1 Y N 1 F N", component_name);
					sw.WriteLine("F0 \"CN\" -150 {0} 40 H V L CNN", 25 * pin + 30);
					sw.WriteLine("F1 \"{0}\" 0 {1} 40 H V C CNN", component_name, -25 * pin - 40);
					sw.WriteLine("F2 \"~\" -50 {0} 60 H V C CNN", 25 * (pin - 1));
					sw.WriteLine("F3 \"~\" -50 {0} 60 H V C CNN", 25 * (pin - 1));
					sw.WriteLine("DRAW");
					sw.WriteLine("S -150 {0} 150 -{0} 0 1 0 f", 25 * pin);		//外枠
					for (int pinid = 1; pinid <= pin; pinid++)
					{
						const int WIDTH = 18;
						if (pinid % 2 == 1)
						{
							int pin_y = 25 * pin - 50 - (int)((double)pinid * 0.5) * 100;
							sw.WriteLine("X {0} {0} -300 {1} {2} R 50 50 1 1 P", pinid, pin_y, 200 - WIDTH);		//ピン
							sw.WriteLine("A -100 {0} {1} 901 -901 0 1 0 N -100 {2} -100 {3}", pin_y, WIDTH, pin_y + WIDTH, pin_y - WIDTH);
							sw.WriteLine("P 2 0 1 0  -100 {0}  -75 {0} F", pin_y + WIDTH);
							sw.WriteLine("P 2 0 1 0  -100 {0}  -75 {0} F", pin_y - WIDTH);
						}
						else
						{
							int pin_y = 25 * pin - 50 - (pinid / 2 - 1) * 100;
							sw.WriteLine("X {0} {0} 300 {1} {2} L 50 50 1 1 P", pinid, pin_y, 200 - WIDTH);		//ピン
							sw.WriteLine("A 100 {0} {1} 899 -899 0 1 0 N 100 {2} 100 {3}", pin_y, WIDTH, pin_y + WIDTH, pin_y - WIDTH);
							sw.WriteLine("P 2 0 1 0  75 {0}  100 {0} F", pin_y + WIDTH);
							sw.WriteLine("P 2 0 1 0  75 {0}  100 {0} F", pin_y - WIDTH);
						}
					}
					sw.WriteLine("ENDDRAW");
					sw.WriteLine("ENDDEF");
				}
			}

			private readonly static string DEFAULT_FILE_NAME;
			private readonly static string LABEL;
			private readonly static string[] TAGS;
		}

	}
}
