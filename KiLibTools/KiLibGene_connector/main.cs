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
		const int MAX_PIN = 99;

		[STAThread]
		static void Main(string[] args)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.FileName = "connector.lib";
			sfd.InitialDirectory = @Directory.GetCurrentDirectory();
			sfd.Filter = "KiCad Library File|*.lib|KiCad Component Library File|*.lib|All Files|*.*";
			sfd.FilterIndex = 1;
			sfd.RestoreDirectory = true;

			if (sfd.ShowDialog() == DialogResult.Cancel)
			{
				MessageBox.Show("Save \"connector.lib\" at current directory.", "注意", MessageBoxButtons.OK);
			}

			using (StreamWriter sw = new StreamWriter(@sfd.FileName))
			{
				sw.WriteLine("EESchema-LIBRARY Version 2.3  Date: " + DateTime.Now.ToString());
				sw.WriteLine("#encoding utf-8");

				PlotMaleSingle(sw);
				PlotFemaleSingle(sw);
				PlotMaleDouble(sw);
				PlotFemaleDouble(sw);

				sw.WriteLine("#");
				sw.WriteLine("#End Library");
			}

		}

		
		private static void PlotMaleSingle(StreamWriter sw)
		{
			for (int pin = 1; pin <= MAX_PIN; pin++)
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

		private static void PlotFemaleSingle(StreamWriter sw)
		{
			for (int pin = 1; pin <= MAX_PIN; pin++)
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

		private static void PlotMaleDouble(StreamWriter sw)
		{
			for (int pin = 2; pin <= MAX_PIN; pin += 2)
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

		private static void PlotFemaleDouble(StreamWriter sw)
		{
			for (int pin = 2; pin <= MAX_PIN; pin += 2)
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
}
