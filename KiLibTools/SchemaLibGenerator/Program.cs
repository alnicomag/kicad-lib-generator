using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using KiLibraries.SchematicLib;

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

			Resistor r = new Resistor();
			Capacitor c = new Capacitor();
			Transistor tr = new Transistor();
			MOSFET mosfet = new MOSFET();
			PowerSupply ps = new PowerSupply();
			Photo pd = new Photo();
			Diode d = new Diode();
			
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
}

