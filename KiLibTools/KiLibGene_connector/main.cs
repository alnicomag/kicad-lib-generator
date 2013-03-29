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
		const int MAX_PIN = 100;

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

				for (int pin = 1; pin < MAX_PIN; pin++)
				{
					string component_name = "con-male-single-" + pin.ToString("D3");
					sw.WriteLine("#");
					sw.WriteLine("# {0}", component_name);
					sw.WriteLine("#");
					sw.WriteLine("DEF {0} CN 0 1 Y N 1 F N", component_name);
					sw.WriteLine("F0 \"CN\" -50 {0} 40 H V L CNN", 50 * pin + 30);
					sw.WriteLine("F1 \"{0}\" 50 {1} 40 H V C CNN", component_name, -50 * pin -40);
					sw.WriteLine("F2 \"~\" -50 {0} 60 H V C CNN", 50 * (pin - 1));
					sw.WriteLine("F3 \"~\" -50 {0} 60 H V C CNN", 50 * (pin - 1));
					sw.WriteLine("DRAW");
					sw.WriteLine("S -50 {0} 150 -{0} 0 1 0 f", 50 * pin);		//外枠
					for (int j = 0; j < pin; j++)
					{
						int pin_y = 100 * (pin - 1) / 2 - j * 100;
						sw.WriteLine("X {0} {0} -200 {1} 150 R 50 50 1 1 P", j + 1, pin_y);		//ピン
						sw.WriteLine("A 0 {0} 15 901 -901 0 1 1 F 0 {1} 0 {2}", pin_y, pin_y - 15, pin_y + 15);
						sw.WriteLine("A 50 {0} 15 -899 899 0 1 1 F 50 {1} 50 {2}", pin_y, pin_y - 15, pin_y + 15);
						sw.WriteLine("S 0 {0} 50 {1} 0 1 1 F", pin_y - 15, pin_y + 15);
						sw.WriteLine("P 2 0 1 0  -50 {0}  -15 {0} F", pin_y);
					}
					sw.WriteLine("ENDDRAW");
					sw.WriteLine("ENDDEF");
				}


				for (int pin = 1; pin < MAX_PIN; pin++)
				{
					string component_name = "con-female-single-" + pin.ToString("D3");
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
					for (int j = 0; j < pin; j++)
					{
						int pin_y = 100 * (pin - 1) / 2 - j * 100;
						const int WIDTH = 18;
						sw.WriteLine("X {0} {0} -200 {1} 150 R 50 50 1 1 P", j + 1, pin_y);		//ピン
						sw.WriteLine("P 2 0 1 0  -50 {0}  -{1} {0} F", pin_y, WIDTH);
						sw.WriteLine("A 0 {0} {3} 901 -901 0 1 0 N 0 {1} 0 {2}", pin_y, pin_y + WIDTH, pin_y - WIDTH, WIDTH);
						sw.WriteLine("P 2 0 1 0  0 {0}  25 {0} F", pin_y + WIDTH);
						sw.WriteLine("P 2 0 1 0  0 {0}  25 {0} F", pin_y - WIDTH);
					}
					sw.WriteLine("ENDDRAW");
					sw.WriteLine("ENDDEF");
				}

				sw.WriteLine("#");
				sw.WriteLine("#End Library");
			}

		}
	}
}
