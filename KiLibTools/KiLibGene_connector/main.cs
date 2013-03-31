using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace KiLibGene_connector
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog()
			{ 
				Description = "ライブラリを保存するフォルダを選択して下さい．",
				RootFolder = System.Environment.SpecialFolder.Desktop,
				SelectedPath = @Directory.GetCurrentDirectory(),
				ShowNewFolderButton = true
			};
			if (fbd.ShowDialog() == DialogResult.Cancel)
			{

			}

			using (StreamWriter sw = new StreamWriter(@fbd.SelectedPath+"\\connector-male.lib"))
			{
				sw.WriteLine("EESchema-LIBRARY Version 2.3  Date: " + DateTime.Now.ToString());
				sw.WriteLine("#encoding utf-8");
				KiLib_ConMale.Plot(sw);
				sw.WriteLine("#");
				sw.WriteLine("#End Library");
			}

			using (StreamWriter sw = new StreamWriter(@fbd.SelectedPath + "\\connector-female.lib"))
			{
				sw.WriteLine("EESchema-LIBRARY Version 2.3  Date: " + DateTime.Now.ToString());
				sw.WriteLine("#encoding utf-8");
				KiLib_ConFemale.Plot(sw);
				sw.WriteLine("#");
				sw.WriteLine("#End Library");
			}

			KiLib_PowerSupply ps = new KiLib_PowerSupply();
			using (StreamWriter sw = new StreamWriter(@fbd.SelectedPath + "\\supply1.lib"))
			{
				sw.WriteLine("EESchema-LIBRARY Version 2.3  Date: " + DateTime.Now.ToString());
				sw.WriteLine("#encoding utf-8");
				ps.PlotType1(sw);
				sw.WriteLine("#");
				sw.WriteLine("#End Library");
			}
			using (StreamWriter sw = new StreamWriter(@fbd.SelectedPath + "\\supply2.lib"))
			{
				sw.WriteLine("EESchema-LIBRARY Version 2.3  Date: " + DateTime.Now.ToString());
				sw.WriteLine("#encoding utf-8");
				ps.PlotType2(sw);
				sw.WriteLine("#");
				sw.WriteLine("#End Library");
			}
		}
	}

	class KiLib_ConMale
	{
		private const int MAX_SINGLE_PIN = 99;
		private const int MAX_DOUBLE_PIN = 99;

		public static void Plot(StreamWriter sw)
		{
			PlotMaleSingle(sw);	
			PlotMaleDouble(sw);
		}

		private static void PlotMaleSingle(StreamWriter sw)
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

		private static void PlotMaleDouble(StreamWriter sw)
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

	class KiLib_ConFemale
	{
		private const int MAX_SINGLE_PIN = 99;
		private const int MAX_DOUBLE_PIN = 99;

		public static void Plot(StreamWriter sw)
		{
			PlotFemaleSingle(sw);
			PlotFemaleDouble(sw);
		}
		
		private static void PlotFemaleSingle(StreamWriter sw)
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

		private static void PlotFemaleDouble(StreamWriter sw)
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

	class KiLib_PowerSupply
	{

		public void PlotType1(StreamWriter sw)
		{
			foreach (string label in Vnum)
			{
				PlotUpType1(sw, "+" + label + "V");
				PlotDownType1(sw, "-" + label + "V");
			}
			foreach(string label in Vp)
			{
				PlotUpType1(sw, label);
			}
			foreach (string label in Vn)
			{
				PlotDownType1(sw, label);
			}
			foreach (string label in Vgnd)
			{
				PlotGNDType1(sw, label);
			}
		}

		public void PlotType2(StreamWriter sw)
		{
			foreach (string label in Vnum)
			{
				PlotUpType2(sw, "+" + label + "V");
				PlotDownType2(sw, "-" + label + "V");
			}
			foreach (string label in Vp)
			{
				PlotUpType2(sw, label);
			}
			foreach (string label in Vn)
			{
				PlotDownType2(sw, label);
			}
			foreach(string label in Vgnd)
			{
				PlotGNDType2(sw, label);
			}
		}

		private static void PlotUpType1(StreamWriter sw, string component_name)
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

			sw.WriteLine("X {0} 1 0 0 50 U 20 20 0 0 W", component_name);
			sw.WriteLine("C 0 75 25 0 1 0 N");

			sw.WriteLine("ENDDRAW");
			sw.WriteLine("ENDDEF");
		}

		private static void PlotUpType2(StreamWriter sw, string component_name)
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

		private static void PlotDownType1(StreamWriter sw, string component_name)
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
			sw.WriteLine("C 0 -75 25 0 1 0 N");

			sw.WriteLine("ENDDRAW");
			sw.WriteLine("ENDDEF");
		}

		private static void PlotDownType2(StreamWriter sw, string component_name)
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

		private static void PlotGNDType1(StreamWriter sw, string component_name)
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

		private static void PlotGNDType2(StreamWriter sw, string component_name)
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

		private List<string> Vnum = new List<string> { "1.2", "1.8", "2.4", "2.5", "3.3", "3.6", "3.7", "4.8", "5", "6", "7.4", "8", "9", "9.4", "10", "11.1", "12", "15", "18", "22.2", "24", "36", "48" };
		private string[] Vp = new string[] { "VCC", "VCC2", "+VCC", "VCCIO", "VDD", "VDD2", "+VDD", "VDDIO", "V+", "VM" };
		private string[] Vn = new string[] { "VEE", "VEE2", "-VCC", "VEEIO", "VSS", "VSS2", "-VDD", "VSSIO", "V-" };
		private string[] Vgnd = new string[] { "GND", "GND1", "GND2", "GND3", "GND4", "AGND", "AGND1", "AGND2", "DGND", "DGND1", "DGND2" };
		private string[] Vearth = new string[] { "FG", "PE" };
	}
}
