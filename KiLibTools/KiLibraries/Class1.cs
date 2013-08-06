using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KiLibraries
{
	namespace SchematicLib
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

		public class Component
		{
			public Component()
			{
				Reference = new Field((int)FieldNumberList.Reference);
				Value = new Field((int)FieldNumberList.Value);
				PCBFootprint = new Field((int)FieldNumberList.PCBFootprint);
				UserDocLink = new Field((int)FieldNumberList.UserDocLink);
				user_fields = new List<Field>();
				draw_records = new List<DrawSection.DrawRecord>();
			}
			public Component(string component_name, string reference, string footprint, string vendor)
			{
				ComponentName = component_name;
				Reference = new Field((int)FieldNumberList.Reference, reference);
				Value = new Field((int)FieldNumberList.Value, component_name);
				PCBFootprint = new Field((int)FieldNumberList.PCBFootprint, footprint);
				UserDocLink = new Field((int)FieldNumberList.UserDocLink);
				//	Vendor = new Field((int)FieldNumberList.Vendor, vendor) { Name = "vendor" };
				user_fields = new List<Field>();
				draw_records = new List<DrawSection.DrawRecord>();
			}
			public string ComponentName { get; set; }
			public string Referance { get; set; }
			public int TextOffset { get; set; }
			public bool DrawPinNumber { get; set; }
			public bool DrawPinName { get; set; }
			public int UnitCount { get; set; }
			public bool UnitsLocked { get; set; }
			public bool PowerType { get; set; }

			public Field Reference { get; set; }
			public Field Value { get; set; }
			public Field PCBFootprint { get; set; }
			public Field UserDocLink { get; set; }

			//	public Field Vendor { get; set; }
			public Field GetUserFields(int i) { throw new NotImplementedException(); }
			public void SetUserField(int i, Field field) { throw new NotImplementedException(); }

			public int RecordCount { get { return draw_records.Count; } }

			public void AddDrawRecord(DrawSection.DrawRecord record)
			{
				draw_records.Add(record);
			}
			public List<DrawSection.DrawRecord> GetDrawRecords()
			{
				return draw_records;
			}



			private List<Field> user_fields;
			private List<DrawSection.DrawRecord> draw_records;

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

			public override string ToString()
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

			
			public void SetLibField(string line)
			{
				List<string> tempstrs = new List<string>(line.Split(' '));

				FieldNumber = int.Parse(tempstrs[0].Substring(1, tempstrs[0].Length - 1));
				Text = tempstrs[1].Substring(1, tempstrs[1].Length - 2);
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

		public enum FieldNumberList : int
		{
			Reference = 0,
			Value,
			PCBFootprint,
			UserDocLink,
		//	Vendor
		}

		

		public class ConMale : KiSchemaLibFile
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

		public class ConFemale : KiSchemaLibFile
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


		public class Inductor : KiSchemaLibFile
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

		public class MOSFET : KiSchemaLibFile
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

		public class JFET : KiSchemaLibFile
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

		public class Photo : KiSchemaLibFile
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


		namespace DrawSection
		{
			public abstract class DrawRecord
			{
				public DrawRecord(string record_name)
				{
					this.record_name = record_name;
			//		this.unit_id = unit_id;
			//		this.expression = expression;
				}

				public new abstract string ToString();

				public string RecordName { get { return record_name; } }
				public int UnitID { get { return unit_id; } set { unit_id = value; } }
				public ConvertExpression Expression { get { return expression; } set { expression = value; } }

				protected readonly string record_name;
				protected int unit_id;	// 所属するパーツ番号（1,2,...），単数パーツのみもしくは全パーツで共通の場合は0
				protected ConvertExpression expression;
			}

			public enum Figure : int
			{
				Polyline = 0,
				Rectangle,
				Circle,
				Arc,
				Text,
				Pin
			}

			/// <summary>
			/// 複数の点を順次結ぶ折れ線
			/// 座標値を小数にも対応させるべき．．．
			/// </summary>
			public class Polyline : DrawRecord
			{
				static Polyline()
				{
					RecordNamePolyline = "P";
					DefaultThickness = 6;
				}
				/// <summary>
				/// KiCadで決められたフォーマットを満たす文字列からPolylineクラスを初期化する．
				/// </summary>
				/// <param name="line"></param>
				public Polyline(string line)
					: base(RecordNamePolyline)
				{
					nodes = new List<Point<int>>();

					List<string> divstrs = new List<string>(line.Split(' '));
					//仕切りスペースが2文字分の場合に発生する空要素を削除
					divstrs.RemoveAll(ls => ls == "");

					if (divstrs.Count < 2)
					{
						throw new ArgumentException();
					}
					if (divstrs[0] != RecordName)
					{
						throw new ArgumentException();
					}

					try
					{
						int number_of_points = int.Parse(divstrs[1]);
						// divstrs[1]に記述されている点の数と実際に存在するデータの数に齟齬がないかチェック
						if (divstrs.Count != 6 + 2 * number_of_points)
						{
							throw new ArgumentException();
						}

						UnitID = int.Parse(divstrs[2]);
						Expression = (ConvertExpression)(Enum.Parse(typeof(ConvertExpression), divstrs[3]));
						Thickness = int.Parse(divstrs[4]);
						for (int i = 0; i < number_of_points; i++)
						{
							nodes.Add(new Point<int>(int.Parse(divstrs[5 + i * 2]), int.Parse(divstrs[5 + i * 2 + 1])));
						}

						string temp = divstrs[5 + 2 * number_of_points];
						// FillStyle列挙体に定義されている列挙体名である場合のみキャストする．（内部値を表す数値の文字列表記を除外）
						// Enum.Parse()メソッドは，列挙体名そのものの文字列だけでなく，内部値を表す数値の文字列表記でも変換してしまう
						if (Enum.IsDefined(typeof(FillStyle), temp))
						{
							Fill = (FillStyle)(Enum.Parse(typeof(FillStyle), temp));
						}
						else { throw new ArgumentException(); }
					}
					catch (ArgumentException)
					{
						throw;
					}
					catch (FormatException)
					{
						throw new ArgumentException();
					}
				}

				public override string ToString()
				{
					throw new NotImplementedException();
				}

				public Point<int> this[int i]
				{
					get { return nodes[i]; }
				}

				public int Thickness { get { return thickness == 0 ? DefaultThickness : thickness; } set { thickness = value; } }
				public FillStyle Fill { get { return fill; } set { fill = value; } }

				private static readonly string RecordNamePolyline;
				private static readonly int DefaultThickness;

				private List<Point<int>> nodes;
				private int thickness;
				private FillStyle fill;
			}

			/// <summary>
			/// 長方形
			/// 座標値を小数にも対応させるべき．．．
			/// </summary>
			public class Rectangle : DrawRecord
			{
				static Rectangle()
				{
					RecordNameRectangle = "S";
					DefaultThickness = 6;
				}
				/// <summary>
				/// KiCadで決められたフォーマットを満たす文字列からRectangleクラスを初期化する．
				/// </summary>
				/// <param name="line"></param>
				public Rectangle(string line)
					: base(RecordNameRectangle)
				{
					List<string> divstrs = new List<string>(line.Split(' '));
					//仕切りスペースが2文字分の場合に発生する空要素を削除
					divstrs.RemoveAll(ls => ls == "");

					if (divstrs.Count != 9)
					{
						throw new ArgumentException();
					}
					if (divstrs[0] != RecordName)
					{
						throw new ArgumentException();
					}

					try
					{
						Top = Math.Max(int.Parse(divstrs[2]), int.Parse(divstrs[4]));
						Bottom = Math.Min(int.Parse(divstrs[2]), int.Parse(divstrs[4]));
						Left = Math.Min(int.Parse(divstrs[1]), int.Parse(divstrs[3]));
						Right = Math.Max(int.Parse(divstrs[1]), int.Parse(divstrs[3]));

						UnitID = int.Parse(divstrs[5]);
						Expression = (ConvertExpression)(Enum.Parse(typeof(ConvertExpression), divstrs[6]));
						Thickness = int.Parse(divstrs[7]);

						// FillStyle列挙体に定義されている列挙体名である場合のみキャストする．（内部値を表す数値の文字列表記を除外）
						// Enum.Parse()メソッドは，列挙体名そのものの文字列だけでなく，内部値を表す数値の文字列表記でも変換してしまう
						if (Enum.IsDefined(typeof(FillStyle), divstrs[8]))
						{
							Fill = (FillStyle)(Enum.Parse(typeof(FillStyle), divstrs[8]));
						}
						else { throw new ArgumentException(); }
					}
					catch(ArgumentException)
					{
						throw;
					}
					catch (FormatException)
					{
						throw new ArgumentException();
					}
				}

				public override string ToString()
				{
					throw new NotImplementedException();
				}

				public int Top { get { return top; } set { top = value; } }
				public int Bottom { get { return bottom; } set { bottom = value; } }
				public int Left { get { return left; } set { left = value; } }
				public int Right { get { return right; } set { right = value; } }

				public int Thickness { get { return thickness == 0 ? DefaultThickness : thickness; } set { thickness = value; } }
				public FillStyle Fill { get { return fill; } set { fill = value; } }

				private static readonly string RecordNameRectangle;
				private static readonly int DefaultThickness;

				private int top;
				private int bottom;
				private int left;
				private int right;

				private int thickness;
				private FillStyle fill;
			}

			/// <summary>
			/// 円
			/// 座標値を小数にも対応させるべき．．．
			/// </summary>
			public class Circle : DrawRecord
			{
				static Circle()
				{
					RecordNameCircle = "C";
					DefaultThickness = 6;
				}
				/// <summary>
				/// KiCadで決められたフォーマットを満たす文字列からCircleクラスを初期化する．
				/// </summary>
				/// <param name="line"></param>
				public Circle(string line)
					: base(RecordNameCircle)
				{
					List<string> divstrs = new List<string>(line.Split(' '));
					//仕切りスペースが2文字分の場合に発生する空要素を削除
					divstrs.RemoveAll(ls => ls == "");

					if (divstrs.Count != 8)
					{
						throw new ArgumentException();
					}
					if (divstrs[0] != RecordName)
					{
						throw new ArgumentException();
					}

					try
					{
						Center = new Point<int>(int.Parse(divstrs[1]), int.Parse(divstrs[2]));
						Radius = int.Parse(divstrs[3]);
						UnitID = int.Parse(divstrs[4]);
						Expression = (ConvertExpression)(Enum.Parse(typeof(ConvertExpression), divstrs[5]));
						Thickness = int.Parse(divstrs[6]);

						// FillStyle列挙体に定義されている列挙体名である場合のみキャストする．（内部値を表す数値の文字列表記を除外）
						// Enum.Parse()メソッドは，列挙体名そのものの文字列だけでなく，内部値を表す数値の文字列表記でも変換してしまう
						if (Enum.IsDefined(typeof(FillStyle), divstrs[7]))
						{
							Fill = (FillStyle)(Enum.Parse(typeof(FillStyle), divstrs[7]));
						}
						else { throw new ArgumentException(); }
					}
					catch (ArgumentException)
					{
						throw;
					}
					catch (FormatException)
					{
						throw new ArgumentException();
					}
				}

				public override string ToString()
				{
					throw new NotImplementedException();
				}

				public Point<int> Center { get { return center; } set { center = value; } }
				public int Radius { get { return radius; } set { radius = value; } }
				public int Thickness { get { return thickness == 0 ? DefaultThickness : thickness; } set { thickness = value; } }
				public FillStyle Fill { get { return fill; } set { fill = value; } }

				private static readonly string RecordNameCircle;
				private static readonly int DefaultThickness;

				private Point<int> center;
				private int radius;
				private int thickness;
				private FillStyle fill;
			}

			/// <summary>
			/// 円弧
			/// </summary>
			public class Arc : DrawRecord
			{
				static Arc()
				{
					RecordNameArc = "A";
					DefaultThickness = 6;
				}
				/// <summary>
				/// KiCadで決められたフォーマットを満たす文字列からArcクラスを初期化する．
				/// </summary>
				/// <param name="line"></param>
				public Arc(string line)
					: base(RecordNameArc)
				{
					List<string> divstrs = new List<string>(line.Split(' '));
					//仕切りスペースが2文字分の場合に発生する空要素を削除
					divstrs.RemoveAll(ls => ls == "");

					if (divstrs.Count != 14)
					{
						throw new ArgumentException();
					}
					if (divstrs[0] != RecordName)
					{
						throw new ArgumentException();
					}
					
					try
					{
						Center = new Point<int>(int.Parse(divstrs[1]), int.Parse(divstrs[2]));
						Radius = int.Parse(divstrs[3]);
						StartAngle = double.Parse(divstrs[4]);
						EndAngle = double.Parse(divstrs[5]);
						UnitID = int.Parse(divstrs[6]);
						Expression = (ConvertExpression)(Enum.Parse(typeof(ConvertExpression), divstrs[7]));
						Thickness = int.Parse(divstrs[8]);

						// FillStyle列挙体に定義されている列挙体名である場合のみキャストする．（内部値を表す数値の文字列表記を除外）
						// Enum.Parse()メソッドは，列挙体名そのものの文字列だけでなく，内部値を表す数値の文字列表記でも変換してしまう
						if (Enum.IsDefined(typeof(FillStyle), divstrs[9]))
						{
							Fill = (FillStyle)(Enum.Parse(typeof(FillStyle), divstrs[9]));
						}
						else { throw new ArgumentException(); }
					}
					catch(ArgumentException)
					{
						throw;
					}
					catch (FormatException)
					{
						throw new ArgumentException();
					}
				}

				public override string ToString()
				{
					throw new NotImplementedException();
				}

				public Point<int> Center { get { return center; } set { center = value; } }
				public int Radius { get { return radius; } set { radius = value; } }
				public double StartAngle { get { return start_angle; } set { start_angle = value; } }
				public double EndAngle { get { return end_angle; } set { end_angle = value; } }
				public Point<double> StartPoint { get { throw new NotImplementedException(); } }
				public Point<double> EndPoint { get { throw new NotImplementedException(); } }
				public int Thickness { get { return thickness == 0 ? DefaultThickness : thickness; } set { thickness = value; } }
				public FillStyle Fill { get { return fill; } set { fill = value; } }

				private static readonly string RecordNameArc;
				private static readonly int DefaultThickness;

				private Point<int> center;
				private int radius;
				private double start_angle;
				private double end_angle;
				//	private Point<double> start;
				//	private Point<double> end;
				private int thickness;
				private FillStyle fill;
			}

			/// <summary>
			/// 文字
			/// </summary>
			public class Text : DrawRecord
			{
				static Text()
				{
					RecordNameText = "T";
				}
				public Text(string line)
					: base(RecordNameText)
				{
					List<string> divstrs = new List<string>(line.Split(' '));
					//仕切りスペースが2文字分の場合に発生する空要素を削除
					divstrs.RemoveAll(ls => ls == "");

					if (divstrs.Count != 13)
					{
						throw new ArgumentException();
					}
					if (divstrs[0] != RecordName)
					{
						throw new ArgumentException();
					}

					TextValue = divstrs[8];
					try
					{
						Orientation = (TextOrientation)(int.Parse(divstrs[1]));
						
						Location = new Point<int>(int.Parse(divstrs[2]), int.Parse(divstrs[3]));
						TextSize = int.Parse(divstrs[4]);
						UnitID = int.Parse(divstrs[6]);
						Expression = (ConvertExpression)(Enum.Parse(typeof(ConvertExpression), divstrs[7]));

						//プロパティにアクセスした方がよさげ．．．
						italic = (TextItalic)(Enum.Parse(typeof(TextItalic), divstrs[9]));
						bold = (TextBold)(Enum.Parse(typeof(TextBold), divstrs[10]));

						HAlign = (HorizontalAlign)(Enum.Parse(typeof(HorizontalAlign), divstrs[11]));
						VAlign = (VerticalAlign)(Enum.Parse(typeof(VerticalAlign), divstrs[12]));
					}
					catch(ArgumentException)		//Enum.Parse()での例外
					{
						throw;
					}
					catch (FormatException)		//int.Parse(string)の例外
					{
						throw new ArgumentException();
					}
				}

				public override string ToString()
				{
					throw new NotImplementedException();
				}

				public TextOrientation Orientation { get { return orientation; } set { orientation = value; } }
				public Point<int> Location { get { return point; } set { point = value; } }
				public int TextSize { get { return text_size; } set { text_size = value; } }
				public string TextValue { get { return text; } set { text = value; } }
				public bool Italic
				{
					get { return (italic == TextItalic.Italic ? true : false); }
					set { italic = (value == true ? TextItalic.Italic : TextItalic.Normal); }
				}
				public bool Bold
				{
					get { return (bold == TextBold.Bold ? true : false); }
					set { bold = (value == true ? TextBold.Bold : TextBold.Normal); }
				}
				public HorizontalAlign HAlign { get { return h_align; } set { h_align = value; } }
				public VerticalAlign VAlign { get { return v_align; } set { v_align = value; } }

				private static readonly string RecordNameText;

				private TextOrientation orientation;
				private Point<int> point;
				private int text_size;
				private int text_type;		//???
				private string text;
				private TextItalic italic;
				private TextBold bold;
				private HorizontalAlign h_align;
				private VerticalAlign v_align;
			}

			/// <summary>
			/// ピン
			/// </summary>
			public class Pin : DrawRecord
			{
				static Pin()
				{
					RecordNameText = "X";
				}
				public Pin(string line)
					:base(RecordNameText)
				{
					List<string> divstrs = new List<string>(line.Split(' '));
					divstrs.RemoveAll(ls => ls=="");	//仕切りスペースが2文字分の場合に発生する空要素を削除

					if ((divstrs.Count != 12) & (divstrs.Count != 13))
					{
						throw new ArgumentException();
					}
					if (divstrs[0] != RecordName)
					{
						throw new ArgumentException();
					}

					PinName = divstrs[1];
					PinNumber = divstrs[2];
					try
					{
						Position = new Point<int>(int.Parse(divstrs[3]), int.Parse(divstrs[4]));
						PinLength = int.Parse(divstrs[5]);

						// PinOrientation列挙体に定義されている列挙体名である場合のみキャストする．（内部値を表す数値の文字列表記を除外）
						// Enum.Parse()メソッドは，列挙体名そのものの文字列だけでなく，内部値を表す数値の文字列表記でも変換してしまう
						if (Enum.IsDefined(typeof(PinOrientation), divstrs[6]))
						{
							Orientation = (PinOrientation)(Enum.Parse(typeof(PinOrientation), divstrs[6]));
						}
						else { throw new ArgumentException(); }

						TextSizeOfPinNumber = int.Parse(divstrs[7]);
						TextSizeOfPinName = int.Parse(divstrs[8]);
						UnitID = int.Parse(divstrs[9]);
						Expression = (ConvertExpression)(Enum.Parse(typeof(ConvertExpression), divstrs[10]));

						if (Enum.IsDefined(typeof(EtypeOriginalString), divstrs[11]))
						{
							ElectricType = (Etype)(int)(EtypeOriginalString)(Enum.Parse(typeof(EtypeOriginalString), divstrs[11]));
						}
						else { throw new ArgumentException(); }

						if (divstrs.Count == 12)
						{
							PinShape = Gtype.Line;
						}
						else
						{
							PinShape = (Gtype)(int)(GtypeOriginalString)(Enum.Parse(typeof(GtypeOriginalString), divstrs[12]));
						}
					}
					catch (ArgumentException)		//Enum.Parse()での例外
					{
						throw;
					}
					catch (FormatException)		//int.Parse(string)の例外
					{
						throw new ArgumentException();
					}
				}

				public override string ToString()
				{
					throw new NotImplementedException();
				}

				public string PinName { get { return pin_name; } set { pin_name = value; } }
				public string PinNumber { get { return pin_number; } set { pin_number = value; } }
				public Point<int> Position { get { return position; } set { position = value; } }
				public int PinLength { get { return length; } set { length = value; } }
				public PinOrientation Orientation { get { return orientation; } set { orientation = value; } }
				public int TextSizeOfPinNumber { get { return size_of_pin_number; } set { size_of_pin_number = value; } }
				public int TextSizeOfPinName { get { return size_of_pin_name; } set { size_of_pin_name = value; } }
				public Etype ElectricType { get { return ele_type; } set { ele_type = value; } }
				public Gtype PinShape { get { return pin_shape; } set { pin_shape = value; } }

				private static readonly string RecordNameText;

				private string pin_name;
				private string pin_number;
				private Point<int> position;
				private int length;
				private PinOrientation orientation;
				private int size_of_pin_number;
				private int size_of_pin_name;
				private Etype ele_type;
				private Gtype pin_shape;
			}

			

			/// <summary>
			/// 塗りつぶし表現の選択．
			/// ファイルでの表記は文字列側．
			/// </summary>
			public enum FillStyle
			{
				/// <summary>
				/// 塗りつぶしなし
				/// </summary>
				N,
				/// <summary>
				/// 全面色で塗りつぶす
				/// </summary>
				F,
				/// <summary>
				/// 背景色で塗りつぶす
				/// </summary>
				f
			}

			/// <summary>
			/// 変換表現（ド・モルガン表現）の選択．
			/// ファイルでの表記は数値側．
			/// </summary>
			public enum ConvertExpression : int
			{
				NotUse = 0,		//変換表現の採用に関係なく，どちらの表現タイプでも共通して使用する．変換表現を用いない場合もこちら．
				Type1 = 1,		//変換表現1で使用する
				Type2 = 2		//変換表現2で使用する
			}

			/// <summary>
			/// ファイルでの表記は数値側
			/// </summary>
			public enum TextOrientation : int
			{
				Holyzontal = 0,
				Vertical = 900
			}

			/// <summary>
			/// ファイルでの表記は文字列側
			/// </summary>
			public enum TextItalic
			{
				Normal,
				Italic
			}

			/// <summary>
			/// ファイルでの表記は数値側
			/// </summary>
			public enum TextBold : int
			{
				Normal = 0,
				Bold = 1
			}

			/// <summary>
			/// ファイルでの表記は文字列側
			/// </summary>
			public enum PinOrientation
			{
				U,
				D,
				R,
				L
			}

			public enum EtypeOriginalString : int
			{
				I = 0,
				O,
				B,
				T,
				P,
				U,
				W,
				w,
				C,
				E,
				N
			}

			public enum Etype : int
			{
				Input = 0,
				Oouput,
				BiDi,
				Tristate,
				Passive,
				Unspecified,
				PowerInput,
				PowerOutput,
				OpenCollector,
				OpenEmittor,
				NotConnected
			}

			/*
			public static class Etype
			{
				public readonly static string Input = "I";
				public readonly static string Output = "O";
				public readonly static string BiDi = "B";
				public readonly static string Tristate = "T";
				public readonly static string Passive = "P";
				public readonly static string Unspecified = "U";
				public readonly static string PowerInput = "W";
				public readonly static string PowerOutput = "w";
				public readonly static string OpenCollector = "C";
				public readonly static string OpenEmittor = "E";
				public readonly static string NotConnected = "N";
			}
			*/

			public enum GtypeOriginalString : int
			{
				I = 0,
				C,
				CI,
				L,
				CL,
				V,
				F,
				X
			}

			public enum Gtype : int
			{
				Inverted=0,		//反転
				Clock,			//クロック
				InvertedClock,	//反転クロック
				InputLow,		//負論理入力
				ClockLow,		//負論理クロック
				OutputLow,		//負論理出力
				FallingEdgeClock,	//立下りエッジクロック
				NonLogic,		//非論理
				Line			//標準
			}

			/*
			public static class Gtype
			{
				public readonly static string Line = "";
				public readonly static string Inverted = "I";
				public readonly static string Clock = "C";
				public readonly static string InvertedClock = "CI";
				public readonly static string InputLow = "L";
				public readonly static string ClockLow = "CL";
				public readonly static string OutputLow = "V";
				public readonly static string FallingEdgeClock = "F";
				public readonly static string NonLogic = "X";
			}
			*/
		}

		
		

		public struct Point<Type>
		{
			public Point(Type x, Type y)
			{
				this.x = x;
				this.y = y;
			}
			public Type x;
			public Type y;
		}
	
	
	}
}
