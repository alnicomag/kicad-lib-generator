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

			KiLib_R r = new KiLib_R("R", "r");
			KiLib_C c = new KiLib_C("C", "c");
			KiLib_Transistor tr = new KiLib_Transistor("Transistor", "tr");
			KiLib_MOSFET mosfet = new KiLib_MOSFET("MOSFET", "mosfet");
			KiLib_PowerSupply ps = new KiLib_PowerSupply("supply", "sup");

			using (StreamReader sr = new StreamReader(stream))
			{
				string line;
				while ((line = sr.ReadLine()) != null)
				{
					if (line.Substring(0, 1) != "#")
					{
						string[] divline = line.Split(',');
						if (divline[0] == r.Label)
						{
							r.AddComponentName(divline[3], new string[] { divline[1], divline[2] });
						}
						else if (divline[0] == c.Label)
						{
							c.AddComponentName(divline[3], new string[] { divline[1], divline[2] });
						}
						else if (divline[0] == tr.Label)
						{
							tr.AddComponentName(divline[3], new string[] { divline[1], divline[2] });
						}
						else if (divline[0] == mosfet.Label)
						{
							mosfet.AddComponentName(divline[3], new string[] { divline[1], divline[2] });
						}
						else if (divline[0] == ps.Label)
						{
							ps.AddComponentName(divline[3], new string[] { divline[1], divline[2] });
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

			WriteFile(fbd, r);
			WriteFile(fbd, c);
			WriteFile(fbd, tr);
			WriteFile(fbd, mosfet);
			WriteFile(fbd, new KiLib_ConMale("ConMa"));
			WriteFile(fbd, new KiLib_ConFemale("ConFe"));
			WriteFile(fbd, ps);
		}

		private static void WriteFile(FolderBrowserDialog fbd, KiLib lib)
		{
			using (StreamWriter sw = new StreamWriter(@fbd.SelectedPath + "\\" + lib.FileName + ".lib"))
			{
				sw.WriteLine("EESchema-LIBRARY Version 2.3  Date: " + DateTime.Now.ToString());
				sw.WriteLine("#encoding utf-8");
				lib.Plot(sw);
				sw.WriteLine("#");
				sw.WriteLine("#End Library");
			}
		}
	}

	/// <summary>
	/// .libファイルに保存される回路図記号ライブラリを表す基底クラス
	/// </summary>
	class KiLib
	{
		public KiLib(string fname, string label)
		{
			filename = fname;
			this.label = label;
		}

		/// <summary>
		/// 保存する時の.libファイルの名前
		/// </summary>
		public virtual string FileName { get { return ""; } }
		/// <summary>
		/// 作成元csvファイルのレコード識別文字列
		/// </summary>
		public virtual string Label { get { return ""; } }

		public virtual void Plot(StreamWriter sw) { }

		protected string filename;
		protected string label;
	}

	class KiLib_PowerSupply : KiLib
	{
		public KiLib_PowerSupply(string fname, string label)
			: base(fname, label)
		{
			names_p1 = new List<string>();
			names_p2 = new List<string>();
			names_n1 = new List<string>();
			names_n2 = new List<string>();
			names_g1 = new List<string>();
			names_g2 = new List<string>();
			names_e1 = new List<string>();
		}

		public override string FileName { get { return this.filename; } }
		public override string Label { get { return this.label; } }

		public override void Plot(StreamWriter sw)
		{
			foreach (string name in names_p1)
			{
				PlotVp1(sw, name);
			}
			foreach (string name in names_p2)
			{
				PlotVp2(sw, name);
			}
			foreach (string name in names_n1)
			{
				PlotVn1(sw, name);
			}
			foreach (string name in names_n2)
			{
				PlotVn2(sw, name);
			}
			foreach (string name in names_g1)
			{
				PlotVg1(sw, name);
			}
			foreach (string name in names_g2)
			{
				PlotVg2(sw, name);
			}
		}

		public void AddComponentName(string name, string[] tags)
		{
			if (tags[0] == "p")
			{
				if (tags[1] == "1") names_p1.Add(name);
				else if (tags[1] == "2") names_p2.Add(name);
			}
			else if (tags[0] == "n")
			{
				if (tags[1] == "1") names_n1.Add(name);
				else if (tags[1] == "2") names_n2.Add(name);
			}
			else if (tags[0] == "g")
			{
				if (tags[1] == "1") names_g1.Add(name);
				else if (tags[1] == "2") names_g2.Add(name);
			}
		}

		[Obsolete]
		public void PlotType1(StreamWriter sw)
		{
			foreach (string label in Vnum)
			{
				PlotVp1(sw, "+" + label + "V");
				PlotVn1(sw, "-" + label + "V");
			}
			foreach (string label in Vp)
			{
				PlotVp1(sw, label);
			}
			foreach (string label in Vn)
			{
				PlotVn1(sw, label);
			}
			foreach (string label in Vgnd)
			{
				PlotVg1(sw, label);
			}
		}
		[Obsolete]
		public void PlotType2(StreamWriter sw)
		{
			foreach (string label in Vnum)
			{
				PlotVp2(sw, "+" + label + "V");
				PlotVn2(sw, "-" + label + "V");
			}
			foreach (string label in Vp)
			{
				PlotVp2(sw, label);
			}
			foreach (string label in Vn)
			{
				PlotVn2(sw, label);
			}
			foreach (string label in Vgnd)
			{
				PlotVg2(sw, label);
			}
		}

		private void PlotVp1(StreamWriter sw, string component_name)
		{
			sw.WriteLine("#");
			sw.WriteLine("# {0}", component_name);
			sw.WriteLine("#");
			sw.WriteLine("DEF {0} #PWR 0 0 N N 1 F P", component_name);
			sw.WriteLine("F0 \"#PWR\" 0 180 40 H I C CNN");
			sw.WriteLine("F1 \"{0}\" 0 125 40 H V C CNN", component_name);
			sw.WriteLine("F2 \"~\" 0 0 60 H V C CNN");
			sw.WriteLine("F3 \"~\" 0 0 60 H V C CNN");
			sw.WriteLine("DRAW");

			sw.WriteLine("X {0} 1 0 0 40 U 20 20 0 0 W", component_name);
			sw.WriteLine("C 0 70 30 0 1 0 N");

			sw.WriteLine("ENDDRAW");
			sw.WriteLine("ENDDEF");
		}

		private void PlotVp2(StreamWriter sw, string component_name)
		{
			sw.WriteLine("#");
			sw.WriteLine("# {0}", component_name);
			sw.WriteLine("#");
			sw.WriteLine("DEF {0} #PWR 0 0 N N 1 F P", component_name);
			sw.WriteLine("F0 \"#PWR\" 0 180 40 H I C CNN");
			sw.WriteLine("F1 \"{0}\" 0 125 40 H V C CNN", component_name);
			sw.WriteLine("F2 \"~\" 0 0 60 H V C CNN");
			sw.WriteLine("F3 \"~\" 0 0 60 H V C CNN");
			sw.WriteLine("DRAW");

			sw.WriteLine("X {0} 1 0 0 60 U 20 20 0 0 W", component_name);
			sw.WriteLine("C 0 75 15 0 1 0 F");
			sw.WriteLine("P 2 0 1 0  -40 75  40 75 N");

			sw.WriteLine("ENDDRAW");
			sw.WriteLine("ENDDEF");
		}

		private void PlotVn1(StreamWriter sw, string component_name)
		{
			sw.WriteLine("#");
			sw.WriteLine("# {0}", component_name);
			sw.WriteLine("#");
			sw.WriteLine("DEF {0} #PWR 0 0 N N 1 F P", component_name);
			sw.WriteLine("F0 \"#PWR\" 0 -195 40 H I C CNN");
			sw.WriteLine("F1 \"{0}\" 0 -140 40 H V C CNN", component_name);
			sw.WriteLine("F2 \"~\" 0 0 60 H V C CNN");
			sw.WriteLine("F3 \"~\" 0 0 60 H V C CNN");
			sw.WriteLine("DRAW");

			sw.WriteLine("X {0} 1 0 0 40 D 20 20 0 0 W", component_name);
			sw.WriteLine("C 0 -70 30 0 1 0 N");

			sw.WriteLine("ENDDRAW");
			sw.WriteLine("ENDDEF");
		}

		private void PlotVn2(StreamWriter sw, string component_name)
		{
			sw.WriteLine("#");
			sw.WriteLine("# {0}", component_name);
			sw.WriteLine("#");
			sw.WriteLine("DEF {0} #PWR 0 0 N N 1 F P", component_name);
			sw.WriteLine("F0 \"#PWR\" 0 -195 40 H I C CNN");
			sw.WriteLine("F1 \"{0}\" 0 -140 40 H V C CNN", component_name);
			sw.WriteLine("F2 \"~\" 0 0 60 H V C CNN");
			sw.WriteLine("F3 \"~\" 0 0 60 H V C CNN");
			sw.WriteLine("DRAW");

			sw.WriteLine("X {0} 1 0 0 60 D 20 20 0 0 W", component_name);
			sw.WriteLine("C 0 -75 15 0 1 0 F");
			sw.WriteLine("P 2 0 1 0  -40 -75  40 -75 N");

			sw.WriteLine("ENDDRAW");
			sw.WriteLine("ENDDEF");
		}

		private void PlotVg1(StreamWriter sw, string component_name)
		{
			sw.WriteLine("#");
			sw.WriteLine("# {0}", component_name);
			sw.WriteLine("#");
			sw.WriteLine("DEF {0} #PWR 0 0 N N 1 F P", component_name);
			sw.WriteLine("F0 \"#PWR\" 0 -195 40 H I C CNN");
			sw.WriteLine("F1 \"{0}\" 0 -140 40 H V C CNN", component_name);
			sw.WriteLine("F2 \"~\" 0 0 60 H V C CNN");
			sw.WriteLine("F3 \"~\" 0 0 60 H V C CNN");
			sw.WriteLine("DRAW");

			sw.WriteLine("X {0} 1 0 0 50 D 20 20 0 0 W", component_name);
			sw.WriteLine("S -50 -55 50 -45 0 1 1 F");
			sw.WriteLine("P 2 0 1 10  -20 -50  -40 -80 N");
			sw.WriteLine("P 2 0 1 10  10 -50  -10 -80 N");
			sw.WriteLine("P 2 0 1 10  40 -50  20 -80 N");

			sw.WriteLine("ENDDRAW");
			sw.WriteLine("ENDDEF");
		}

		private void PlotVg2(StreamWriter sw, string component_name)
		{
			sw.WriteLine("#");
			sw.WriteLine("# {0}", component_name);
			sw.WriteLine("#");
			sw.WriteLine("DEF {0} #PWR 0 0 N N 1 F P", component_name);
			sw.WriteLine("F0 \"#PWR\" 0 -195 40 H I C CNN");
			sw.WriteLine("F1 \"{0}\" 0 -140 40 H V C CNN", component_name);
			sw.WriteLine("F2 \"~\" 0 0 60 H V C CNN");
			sw.WriteLine("F3 \"~\" 0 0 60 H V C CNN");
			sw.WriteLine("DRAW");

			sw.WriteLine("X {0} 1 0 0 50 D 20 20 0 0 W", component_name);
			sw.WriteLine("P 4 0 1 0  0 -100  -50 -50  50 -50  0 -100 N");

			sw.WriteLine("ENDDRAW");
			sw.WriteLine("ENDDEF");
		}

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

		private List<string> names_p1;
		private List<string> names_p2;
		private List<string> names_n1;
		private List<string> names_n2;
		private List<string> names_g1;
		private List<string> names_g2;
		private List<string> names_e1;
	}

	class KiLib_ConMale : KiLib
	{
		public KiLib_ConMale(string fname)
			: base(fname, "")
		{

		}

		private const int MAX_SINGLE_PIN = 99;
		private const int MAX_DOUBLE_PIN = 99;

		public override string FileName
		{
			get
			{
				return this.filename;
			}
		}

		public override void Plot(StreamWriter sw)
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
	}

	class KiLib_ConFemale : KiLib
	{
		public KiLib_ConFemale(string fname)
			: base(fname, "")
		{

		}

		private const int MAX_SINGLE_PIN = 99;
		private const int MAX_DOUBLE_PIN = 99;

		public override string FileName
		{
			get
			{
				return this.filename;
			}
		}
		public override void Plot(StreamWriter sw)
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
	}

	class KiLib_R : KiLib
	{
		public KiLib_R()
			: this("R", "r")
		{

		}
		public KiLib_R(string fname)
			: this(fname, "r")
		{

		}
		public KiLib_R(string fname, string label)
			: base(fname, label)
		{
			names_box = new List<string>();
			names_zigzag = new List<string>();
		}

		public override string FileName { get { return this.filename; } }
		public override string Label { get { return this.label; } }

		public override void Plot(StreamWriter sw)
		{
			foreach (string name in names_box)
			{
				PlotBoxType(sw, name);
			}
			foreach (string name in names_zigzag)
			{
				PlotZigzagType(sw, name);
			}
		}

		public void AddComponentName(string name, string[] tags)
		{
			if (tags[0] == "1") names_box.Add(name);
			else if (tags[0] == "2") names_zigzag.Add(name);
		}

		private void PlotBoxType(StreamWriter sw, string component_name)
		{
			sw.WriteLine("#");
			sw.WriteLine("# {0}", component_name);
			sw.WriteLine("#");
			sw.WriteLine("DEF {0} R 0 0 N Y 1 F N", component_name);
			sw.WriteLine("F0 \"R\" -100 60 45 H V L CNN");
			sw.WriteLine("F1 \"{0}\" 0 -70 45 H V C CNN", component_name);
			sw.WriteLine("F2 \"~\" -70 0 30 V V C CNN");
			sw.WriteLine("F3 \"~\" 0 0 30 H V C CNN");
			sw.WriteLine("DRAW");
			sw.WriteLine("S -100 30 100 -30 0 1 0 N");
			sw.WriteLine("X ~ 1 -150 0 50 R 60 60 1 1 P");
			sw.WriteLine("X ~ 2 150 0 50 L 60 60 1 1 P");
			sw.WriteLine("ENDDRAW");
			sw.WriteLine("ENDDEF");
		}
		private void PlotZigzagType(StreamWriter sw, string component_name)
		{
			sw.WriteLine("#");
			sw.WriteLine("# {0}", component_name);
			sw.WriteLine("#");
			sw.WriteLine("DEF {0} R 0 0 N Y 1 F N", component_name);
			sw.WriteLine("F0 \"R\" -125 60 45 H V L CNN");
			sw.WriteLine("F1 \"{0}\" 0 -70 45 H V C CNN", component_name);
			sw.WriteLine("F2 \"~\" -70 0 30 V V C CNN");
			sw.WriteLine("F3 \"~\" 0 0 30 H V C CNN");
			sw.WriteLine("DRAW");
			sw.WriteLine("P 8 0 1 0  -105 0  -87 30  -52 -30  -17 30  17 -30  52 30  87 -30  105 0 N");
			sw.WriteLine("X ~ 1 -150 0 45 R 60 60 1 1 P");
			sw.WriteLine("X ~ 2 150 0 45 L 60 60 1 1 P");
			sw.WriteLine("ENDDRAW");
			sw.WriteLine("ENDDEF");
		}

		private List<string> names_box;
		private List<string> names_zigzag;
	}

	class KiLib_C : KiLib
	{
		public KiLib_C()
			: this("C", "c")
		{

		}
		public KiLib_C(string fname)
			: this(fname, "c")
		{

		}
		public KiLib_C(string fname, string label)
			: base(fname, label)
		{
			names_ceramic = new List<string>();
			names_pole = new List<string>();
			names_nonepole = new List<string>();
		}

		public override string FileName { get { return this.filename; } }
		public override string Label { get { return this.label; } }

		public override void Plot(StreamWriter sw)
		{
			foreach (string name in names_ceramic)
			{
				PlotCeramicType(sw, name);
			}
			foreach (string name in names_pole)
			{
				PlotPoleType(sw, name);
			}
			foreach (string name in names_nonepole)
			{
				PlotNonePoleType(sw, name);
			}
		}

		public void AddComponentName(string name, string[] tags)
		{
			if (tags[0] == "cera") names_ceramic.Add(name);
			else if (tags[0] == "pole") names_pole.Add(name);
			else if (tags[0] == "npole") names_nonepole.Add(name);
		}

		private void PlotCeramicType(StreamWriter sw, string component_name)      //セラミック，フィルム
		{
			sw.WriteLine("#");
			sw.WriteLine("# {0}", component_name);
			sw.WriteLine("#");
			sw.WriteLine("DEF {0} C 0 10 N N 1 F N", component_name);
			sw.WriteLine("F0 \"C\" 50 70 45 H V L CNN");
			sw.WriteLine("F1 \"{0}\" 50 -75 45 H V L CNN", component_name);
			sw.WriteLine("F2 \"~\" 38 -150 30 H V C CNN");
			sw.WriteLine("F3 \"~\" 0 0 60 H V C CNN");
			sw.WriteLine("DRAW");
			sw.WriteLine("S -60 -30 60 -15 0 1 1 F");
			sw.WriteLine("S -60 30 60 15 0 1 1 F");
			sw.WriteLine("X ~ 1 0 100 75 D 40 40 1 1 P");
			sw.WriteLine("X ~ 2 0 -100 75 U 40 40 1 1 P");
			sw.WriteLine("ENDDRAW");
			sw.WriteLine("ENDDEF");
		}

		private void PlotPoleType(StreamWriter sw, string component_name)     //有極電解
		{
			sw.WriteLine("#");
			sw.WriteLine("# {0}", component_name);
			sw.WriteLine("#");
			sw.WriteLine("DEF {0} C 0 10 N N 1 F N", component_name);
			sw.WriteLine("F0 \"C\" 50 70 45 H V L CNN");
			sw.WriteLine("F1 \"{0}\" 50 -75 45 H V L CNN", component_name);
			sw.WriteLine("F2 \"~\" 38 -150 30 H V C CNN");
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

		private void PlotNonePoleType(StreamWriter sw, string component_name)        //無極性電解
		{
			sw.WriteLine("#");
			sw.WriteLine("# {0}", component_name);
			sw.WriteLine("#");
			sw.WriteLine("DEF {0} C 0 10 N N 1 F N", component_name);
			sw.WriteLine("F0 \"C\" 50 70 45 H V L CNN");
			sw.WriteLine("F1 \"{0}\" 50 -75 45 H V L CNN", component_name);
			sw.WriteLine("F2 \"~\" 38 -150 30 H V C CNN");
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

		private List<string> names_ceramic;
		private List<string> names_pole;
		private List<string> names_nonepole;
	}

	class KiLib_Transistor : KiLib
	{
		public KiLib_Transistor()
			: this("Transistor", "tr")
		{

		}
		public KiLib_Transistor(string fname)
			: this(fname, "tr")
		{

		}
		public KiLib_Transistor(string fname, string label)
			: base(fname, label)
		{
			names_npn = new List<string>();
			names_pnp = new List<string>();
		}

		public override string FileName { get { return this.filename; } }
		public override string Label { get { return this.label; } }

		public override void Plot(StreamWriter sw)
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

		public void AddComponentName(string name, string[] tags)
		{
			if (tags[0] == "npn") names_npn.Add(name);
			else if (tags[0] == "pnp") names_pnp.Add(name);
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
	}

	class KiLib_MOSFET : KiLib
	{
		public KiLib_MOSFET()
			: this("MOSFET", "mosfet")
		{

		}
		public KiLib_MOSFET(string fname)
			: this(fname, "mosfet")
		{

		}
		public KiLib_MOSFET(string fname, string label)
			: base(fname, label)
		{
			names_mosfet_n = new List<string>();
			names_mosfet_p = new List<string>();
		}

		public override string FileName { get { return this.filename; } }
		public override string Label { get { return this.label; } }

		public override void Plot(StreamWriter sw)
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
	}

	class KiLib_JFET : KiLib
	{
		public KiLib_JFET(string fname, string label)
			: base(fname, label)
		{
			names_jfet_n = new List<string>()
			{

			};
			names_jfet_p = new List<string>()
			{

			};
		}

		public override string FileName { get { return this.filename; } }
		public override string Label { get { return "jfet"; } }

		public override void Plot(StreamWriter sw)
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
	}

}
