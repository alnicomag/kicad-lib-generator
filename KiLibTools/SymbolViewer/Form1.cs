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

namespace SymbolViewer
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

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

			}
		}

	}
}
