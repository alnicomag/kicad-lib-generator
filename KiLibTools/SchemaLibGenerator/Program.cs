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
							List<string> aa = new List<string>(divline);
							aa.RemoveAt(0);
							r.AddComponentName(aa);
						}
						else if (divline[0] == c.Label)
						{
							List<string> aa = new List<string>(divline);
							aa.RemoveAt(0);
							c.AddComponentName(aa);
						}
						else if (divline[0] == d.Label)
						{
							d.AddComponentName(divline[3], new string[] { divline[1], divline[2] });
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
		public abstract class KiSchemaLibFile
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
		public class Field
		{
			static Field()
			{
				DEFAULT_X = 0;
				DEFAULT_Y = 0;
				DEFAULT_SIZE = 45;
			}
			public Field(int fieldnumber)
				: this(fieldnumber, "~", "", DEFAULT_X, DEFAULT_Y, DEFAULT_SIZE, true)
			{

			}
			public Field(int fieldnumber, string text)
				: this(fieldnumber, text, "", DEFAULT_X, DEFAULT_Y, DEFAULT_SIZE, true)
			{

			}
			public Field(int fieldnumber, string text, string name, int x, int y, int size, bool reserved)
			{
				FieldNumber = fieldnumber;
				Text = text;
				Name = name;
				X = x;
				Y = y;
				Size = size;
				Visible = Visible.V;
				Orient = Orientation.H;
				HAlign = HorizontalAlign.C;
				VAlign = VerticalAlign.C;
				FontShape = FontShape.NN;
				Reserved = reserved;
			}

			public static readonly int DEFAULT_X;
			public static readonly int DEFAULT_Y;
			public static readonly int DEFAULT_SIZE;

			/// <summary>
			/// フィールド番号
			/// </summary>
			public int FieldNumber { get; set; }
			/// <summary>
			/// フィールド名称
			/// </summary>
			public string Name { get; set; }
			/// <summary>
			/// フィールド値（表示される文字列）
			/// </summary>
			public string Text { get; set; }
			/// <summary>
			/// 位置[mil]
			/// </summary>
			public int X { get; set; }
			/// <summary>
			/// 位置[mil]
			/// </summary>
			public int Y { get; set; }
			/// <summary>
			/// 文字サイズ[mil]
			/// </summary>
			public int Size { get; set; }
			/// <summary>
			/// 表示状態
			/// </summary>
			public Visible Visible { get; set; }
			/// <summary>
			/// 向き
			/// </summary>
			public Orientation Orient { get; set; }
			/// <summary>
			/// 水平位置合わせの基準
			/// </summary>
			public HorizontalAlign HAlign { get; set; }
			/// <summary>
			/// 垂直位置合わせの基準
			/// </summary>
			public VerticalAlign VAlign { get; set; }
			/// <summary>
			/// フォント
			/// </summary>
			public FontShape FontShape { get; set; }
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

			public string ToString()
			{
				string field;
				field = "F" + FieldNumber.ToString() + " "
					+ "\"" + Text + "\" "
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

			[Obsolete]
			public void SetLibField(string line)
			{
				List<string> tempstrs = LineParse(line);

				FieldNumber = int.Parse(tempstrs[0].Substring(1, tempstrs[0].Length - 1));
				Text = tempstrs[1];
				X = int.Parse(tempstrs[2]);
				Y = int.Parse(tempstrs[3]);
				Size = int.Parse(tempstrs[4]);
				Orient = (tempstrs[5] == Orientation.H.ToString() ? Orientation.H : Orientation.V);
				Visible = (tempstrs[6] == Visible.I.ToString() ? Visible.I : Visible.V);
				HAlign = (tempstrs[7] == HorizontalAlign.C.ToString() ? HorizontalAlign.C : tempstrs[7] == HorizontalAlign.L.ToString() ? HorizontalAlign.L : HorizontalAlign.R);
				VAlign = (tempstrs[8].Substring(0, 1) == VerticalAlign.C.ToString() ? VerticalAlign.C : tempstrs[8].Substring(0, 1) == VerticalAlign.B.ToString() ? VerticalAlign.B : VerticalAlign.T);

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

			[Obsolete]
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
		public enum HorizontalAlign
		{
			C,
			R,
			L
		}
		public enum VerticalAlign
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

		enum FieldNumberList : int
		{
			Reference = 0,
			Value,
			PCBFootprint,
			UserDocLink,
			Vendor
		}

		class Fields
		{
			public Fields(string component_name, string reference, string footprint, string vendor)
			{
				ComponentName = component_name;
				Reference = new Field((int)FieldNumberList.Reference, reference);
				Value = new Field((int)FieldNumberList.Value, component_name);
				PCBFootprint = new Field((int)FieldNumberList.PCBFootprint, footprint);
				UserDocLink = new Field((int)FieldNumberList.UserDocLink);
				Vendor = new Field((int)FieldNumberList.Vendor, vendor) { Name = "vendor" };
			}
			public string ComponentName { get; set; }
			public Field Reference { get; set; }
			public Field Value { get; set; }
			public Field PCBFootprint { get; set; }
			public Field UserDocLink { get; set; }
			public Field Vendor { get; set; }
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

