using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace ComponentNameGetter
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			List<string> componentnames = new List<string>();

			OpenFileDialog ofd = new OpenFileDialog();
			ofd.FileName = "default.html";
			ofd.InitialDirectory = @"C:\";
			ofd.Filter = "KiCad Library File|*.lib|すべてのファイル(*.*)|*.*";
			ofd.FilterIndex = 1;
			ofd.Title = "開くファイルを選択してください";
			ofd.RestoreDirectory = true;
			
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				Stream stream = ofd.OpenFile();
				if (stream != null)
				{
					using (StreamReader sr = new StreamReader(stream))
					{
						string line;

						while ((line = sr.ReadLine()) != null)
						{
							try
							{
								if (line.Substring(0, 2) == "# ")
								{
									componentnames.Add(line.Substring(2, line.Length - 2));
								}
							}
							catch (ArgumentOutOfRangeException)
							{

							}
						}
					}
					stream.Close();
				}
			}

			foreach (string name in componentnames)
			{
				Console.WriteLine(name);
			}
			Console.ReadLine();
		}
	}
}
