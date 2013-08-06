using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO;

using KiLibraries.SchematicLib.DrawSection;
using KiLibraries.SchematicLib;

namespace SymbolViewer
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			
			string a = "1";
			PinOrientation orientation;
			/*
			if (Enum.IsDefined(typeof(PinOrientation), a))
			{
				orientation = (PinOrientation)(Enum.Parse(typeof(PinOrientation), a));
			}
			else
			{
				throw new ArgumentException();
			}
			*/
		//	orientation = (PinOrientation)(Enum.Parse(typeof(PinOrientation), a));

			read_components = new List<KiLibraries.SchematicLib.Component>();
		}

		List<KiLibraries.SchematicLib.Component> read_components;

		private void SetProjection()
		{
			// ビューポートの設定
			GL.Viewport(0, 0, glControl.Width, glControl.Height);

			// 視体積の設定
			GL.MatrixMode(MatrixMode.Projection);
			float h = 12.0f;
			float w = h * glControl.AspectRatio;
			Matrix4 proj = Matrix4.CreateOrthographic(w, h, 0.1f, 2.0f);
			GL.LoadMatrix(ref proj);

			GL.MatrixMode(MatrixMode.Modelview);
		}

		private void glControl_Load(object sender, EventArgs e)
		{
			// 背景色の設定
			GL.ClearColor(glControl.BackColor);

			SetProjection();

			// 視界の設定
			Matrix4 look = Matrix4.LookAt(Vector3.UnitZ, Vector3.Zero, Vector3.UnitY);
			GL.LoadMatrix(ref look);
		}

		private void glControl_Resize(object sender, EventArgs e)
		{
			// Projection の設定
			SetProjection();

			// 再描画
			glControl.Refresh();
		}

		private void glControl_Paint(object sender, PaintEventArgs e)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit);

			GL.PushMatrix();
			{
				// (-2, -2) まで移動
				GL.Translate(-4.5, -4.5, 0.0);

				for (int i = 0; i < 10; i++) // x 方向
				{
					GL.PushMatrix();
					{
						for (int j = 0; j < 10; j++) // y 方向
						{
							// 色の変更
							GL.Color3(i * 0.125, j * 0.125, 0.0);
							// 正方形を描画
							DrawSquare();
							// y 方向に1移動
							GL.Translate(0.0, 1.0, 0.0);
						}
					}
					GL.PopMatrix();

					// x 方向に1移動
					GL.Translate(1.0, 0.0, 0.0);
				}
			}
			GL.PopMatrix();

			glControl.SwapBuffers();

		}

		// 正方形を描画する関数
		private void DrawSquare()
		{
			double side = 0.45;

			GL.Begin(BeginMode.TriangleStrip);
			{
				GL.Vertex2(side, side);
				GL.Vertex2(-side, side);
				GL.Vertex2(side, -side);
				GL.Vertex2(-side, -side);
			}
			GL.End();
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void ToolStripMenuItem_Open_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog()
			{
				Title = "開くコンポーネントライブラリファイルを選択してください",
				InitialDirectory = @Directory.GetCurrentDirectory(),
				Filter = "コンポーネントライブラリファイル(*.lib)|*.lib|すべてのファイル(*.*)|*.*",
				FilterIndex = 1
			};
			ofd.ShowDialog();

			Stream stream = ofd.OpenFile();

			using (StreamReader sr = new StreamReader(stream))
			{
				sr.ReadLine();
				sr.ReadLine();

				string line_str;
				while (sr.Peek()>-1)
				{
					line_str = sr.ReadLine();
					if (line_str.Substring(0, 1) != "#")
					{
						read_components.Add(new KiLibraries.SchematicLib.Component());
						List<string> div = new List<string>(line_str.Split(' '));
						read_components[read_components.Count - 1].ComponentName = div[1];
						read_components[read_components.Count - 1].Referance = div[2];
						read_components[read_components.Count - 1].TextOffset = int.Parse(div[4]);
						read_components[read_components.Count - 1].DrawPinNumber = (div[5] == "Y" ? true : false);
						read_components[read_components.Count - 1].DrawPinName = (div[6] == "Y" ? true : false);
						read_components[read_components.Count - 1].UnitCount = int.Parse(div[7]);
						read_components[read_components.Count - 1].UnitsLocked = (div[8] == "L" ? true : false);
						read_components[read_components.Count - 1].PowerType = (div[9] == "P" ? true : false);
						while ((line_str = sr.ReadLine())!="DRAW")
						{
							
								if (line_str.Substring(1, 1) == "0")
								{
									read_components[read_components.Count - 1].Reference.SetLibField(line_str);
								}
								else if (line_str.Substring(1, 1) == "1")
								{
									read_components[read_components.Count - 1].Value.SetLibField(line_str);
								}
								else if (line_str.Substring(1, 1) == "2")
								{
									read_components[read_components.Count - 1].PCBFootprint.SetLibField(line_str);
								}
								else if (line_str.Substring(1, 1) == "3")
								{
									read_components[read_components.Count - 1].UserDocLink.SetLibField(line_str);
								}
							
						}
						while ((line_str = sr.ReadLine()) != "ENDDRAW")
						{
							switch (line_str.Substring(0, 1))
							{
								case "P":
									read_components[read_components.Count - 1].AddDrawRecord(new Polyline(line_str));
									break;
								case "S":
									read_components[read_components.Count - 1].AddDrawRecord(new KiLibraries.SchematicLib.DrawSection.Rectangle(line_str));
									break;
								case "C":
									read_components[read_components.Count - 1].AddDrawRecord(new Circle(line_str));
									break;
								case "A":
									read_components[read_components.Count - 1].AddDrawRecord(new Arc(line_str));
									break;
								case "T":
									read_components[read_components.Count - 1].AddDrawRecord(new Text(line_str));
									break;
								case "X":
									read_components[read_components.Count - 1].AddDrawRecord(new Pin(line_str));
									break;
							}
						}
						sr.ReadLine();		//ENDDEF読み飛ばし
					}
				}

			}

			foreach (KiLibraries.SchematicLib.Component i in read_components)
			{
				listBox1.Items.Add(i.ComponentName);
			}
			
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}

	}
}
