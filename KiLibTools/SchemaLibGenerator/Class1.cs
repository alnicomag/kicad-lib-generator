using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace KiSchemaLib
{
	public class KiSchemaLibFile : IDisposable
	{
		public KiSchemaLibFile(string directory)
		{
			if (File.Exists(directory))
			{
				this.directory = directory;
			}
			else
			{
				throw new FileNotFoundException();
			}
		}

		public SchemaComponent this[int i]
		{
			get
			{
				return ComponentsSection[i];
			}
		}

		public void Dispose()
		{
			//throw new NotImplementedException();
		}

		public void ReadLib()
		{
			using (StreamReader r = new StreamReader(@directory))
			{
				string line;

				header = r.ReadLine();
				encodeing = r.ReadLine();

				while (r.Peek() >= 0)
				{
					List<string> ComponentBlockStrs = new List<string>();

					bool stop = false;
					while (!stop)
					{
						line = r.ReadLine();
						if ((line != null) & (line != "ENDDEF"))
						{
							if (line.Substring(0, 1) != "#")
							{
								ComponentBlockStrs.Add(line);
							}
						}
						else
						{
							stop = true;
						}
					}
					if (ComponentBlockStrs.Count > 0)
					{
						ComponentBlockStrs.Add("ENDDEF");
						ComponentsSection.Add(new SchemaComponent(ComponentBlockStrs));
					}
				}
			}

		}

		public List<string> GetLib()
		{
			List<string> strs = new List<string>();
			strs.Add(header);
			strs.Add(encodeing);
			foreach (SchemaComponent i in ComponentsSection)
			{
				List<string> tempstrs = new List<string>();
				tempstrs = i.GetComponent();
				foreach (string j in tempstrs)
				{
					strs.Add(j);
				}
			}
			strs.Add("#");
			strs.Add("#End Library");

			return strs;
		}

		public int Count { get { return ComponentsSection.Count; } }

		private string directory;       //.libファイルのディレクトリ
	//	private StreamReader r;

		private string header;
		private string encodeing;

		private List<SchemaComponent> ComponentsSection = new List<SchemaComponent>();



	}

	public class SchemaComponent
	{
		public SchemaComponent()
		{

		}
		public SchemaComponent(List<string> strs)
		{
			string[] tempstrs = strs[0].Split(' ');
			name = tempstrs[1];
			reference = tempstrs[2];
			//   unused = tempstrs[3];
			text_offset = tempstrs[4];
			draw_pinnumber = tempstrs[5];
			draw_pinname = tempstrs[6];
			unit_count = tempstrs[7];
			units_locked = tempstrs[8];
			option_flag = tempstrs[9];

			f0 = new SchemaField(0);
			f0.SetLibField(strs[1]);

			f1 = new SchemaField(1);
			f1.SetLibField(strs[2]);

			int index = 3;
			while (strs[index] != "DRAW")
			{
				Fs.Add(new SchemaField(index + 1));
				Fs[Fs.Count - 1].SetLibField(strs[index]);
				index++;
			}
			index++;
			while (strs[index] != "ENDDRAW")
			{
				DrawSection.Add(strs[index]);
				index++;
			}
		}


		public List<string> GetComponent()
		{
			List<string> strs = new List<string>();

			strs.Add("#");
			strs.Add("# " + name);
			strs.Add("#");
			strs.Add("DEF "
				+ name + " "
				+ reference + " "
				+ unused + " "
				+ text_offset + " "
				+ draw_pinnumber + " "
				+ draw_pinname + " "
				+ unit_count + " "
				+ units_locked + " "
				+ option_flag
			);
			strs.Add(f0.ToString());
			strs.Add(f1.ToString());
			foreach (SchemaField i in Fs)
			{
				strs.Add(i.ToString());
			}
			strs.Add("DRAW");
			foreach (string i in DrawSection)
			{
				strs.Add(i);
			}
			strs.Add("ENDDRAW");
			strs.Add("ENDDEF");
			return strs;
		}

		public void AddField(SchemaField field)
		{
			if (Fs.Exists(ls => ls.ID == field.ID))
			{
				Fs.RemoveAt(Fs.FindIndex(ls => ls.ID == field.ID));
			}
			Fs.Add(field);
			Fs.Sort((x, y) => x.ID - y.ID);
		}

		public string Name { get { return name; } }	//component name in library
		public int F1Y { get { return f1.Y; } }

		private string name;
		private string reference;
		private string unused = "0";
		private string text_offset;		//offset for pin name position
		private string draw_pinnumber;	//display pin name or not
		private string draw_pinname;	//Y (display pin name) or N (do not display pin name).
		private string unit_count;		//Number of part (or section) in a component package.
		private string units_locked;	//L (units are not identical and cannot be swapped) or F (units are identical and therefore can be swapped) (Used only if unit_count > 1)
		private string option_flag;		//N (normal) or P (component type "power")

		private SchemaField f0;
		private SchemaField f1;
		private SchemaField f2;
		private SchemaField f3;
		private List<SchemaField> Fs = new List<SchemaField>();

		private List<string> footprint;

		private List<string> alias;

		private List<string> DrawSection = new List<string>();
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



	public class SchemaDrawArc
	{
		public SchemaDrawArc()
		{

		}
		public string ToString()
		{
			throw new NotImplementedException();
		}

		//	private int x;
		//	private int y;
		private Point center;
		private int radius;
		private int start_angle;
		private int end_angle;
		private string unit;
		private string convert;
		private int thickness;	//thickness of the outline or 0 to use the default line thickness.
		private string cc;
	}
	public class SchemaDrawCircle
	{
		public SchemaDrawCircle()
		{

		}
		public string ToString()
		{
			throw new NotImplementedException();
		}
		//	private int x;
		//	private int y;
		private Point center;
		private int radius;
		private string unit;
		private string convert;
		private int thickness;
		private string cc;
	}
	public class SchemaLibDrawPolyline
	{

		public string ToString()
		{
			throw new NotImplementedException();
		}

		private string unit;
		private string convert;
		private int thickness;
		private List<Point> nodes;
		private string cc;
	}
	public class SchemaLibDrawRectangle
	{
		public SchemaLibDrawRectangle(string line)
		{
			string[] divstrs = line.Split(' ');
			try
			{
				if (divstrs[0] == "X")
				{
					Top = Math.Max(int.Parse(divstrs[2]), int.Parse(divstrs[4]));
					Bottom = Math.Min(int.Parse(divstrs[2]), int.Parse(divstrs[4]));
					Left = Math.Min(int.Parse(divstrs[1]), int.Parse(divstrs[3]));
					Right = Math.Max(int.Parse(divstrs[1]), int.Parse(divstrs[3]));
				}
				else
				{
					throw new ArgumentException();
				}
			}
			catch (ArgumentOutOfRangeException)
			{
				throw new ArgumentException();
			}
		}

		public int Top { get { return top; } set { top = value; } }
		public int Bottom { get { return bottom; } set { bottom = value; } }
		public int Left { get { return left; } set { left = value; } }
		public int Right { get { return right; } set { right = value; } }

		private int top;
		private int bottom;
		private int left;
		private int right;

		private string unit;
		private string convert;
		private int thickness;
		private string cc;
	}
	public class SchemaLibDrawText
	{

		public SchemaLibDrawText()
		{
			
		}

		public string ToString()
		{
			throw new NotImplementedException();
		}

		private int orientation;
		private string unit;
		private string convert;
		private string text;
	}
	public class SchemaLibDrawPin
	{
		public SchemaLibDrawPin()
		{

		}

		public string ToString()
		{
			throw new NotImplementedException();
		}



	}

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

	public static class Gstyle
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


	public struct Point
	{
		public Point(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
		public int x;
		public int y;
	}

}
