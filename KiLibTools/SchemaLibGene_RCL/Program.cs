using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace SchemaLibGene_RCL
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

            KiLib_R r = new KiLib_R();
            using (StreamWriter sw = new StreamWriter(@fbd.SelectedPath + "\\r.lib"))
            {
                sw.WriteLine("EESchema-LIBRARY Version 2.3  Date: " + DateTime.Now.ToString());
                sw.WriteLine("#encoding utf-8");
                r.Plot(sw);
                sw.WriteLine("#");
                sw.WriteLine("#End Library");
            }

			KiLib_C c = new KiLib_C();
			using (StreamWriter sw = new StreamWriter(@fbd.SelectedPath + "\\c.lib"))
			{
				sw.WriteLine("EESchema-LIBRARY Version 2.3  Date: " + DateTime.Now.ToString());
				sw.WriteLine("#encoding utf-8");
				c.Plot(sw);
				sw.WriteLine("#");
				sw.WriteLine("#End Library");
			}

			KiLib_Tr tr = new KiLib_Tr();
			using (StreamWriter sw = new StreamWriter(@fbd.SelectedPath + "\\tr.lib"))
			{
				sw.WriteLine("EESchema-LIBRARY Version 2.3  Date: " + DateTime.Now.ToString());
				sw.WriteLine("#encoding utf-8");
				tr.Plot(sw);
				sw.WriteLine("#");
				sw.WriteLine("#End Library");
			}
        }
    }

    public class KiLib_R
    {
        public KiLib_R()
        {
			names = new List<string>()
			{
				"R",
				"R(0402)",
				"R(0603)",
				"R(1005)"
			};
        }

        public void Plot(StreamWriter sw)
        {
            foreach (string name in names)
            {
                //PlotBoxType(sw,name);
				PlotZigzagType(sw, name);
            }
        }

        private void PlotBoxType(StreamWriter sw,string component_name)
        {
            sw.WriteLine("#");
            sw.WriteLine("# {0}", component_name);
            sw.WriteLine("#");
            sw.WriteLine("DEF {0} R 0 0 N Y 1 F N",component_name);
            sw.WriteLine("F0 \"R\" -100 60 40 H V L CNN");
            sw.WriteLine("F1 \"{0}\" 0 -70 40 H V C CNN",component_name);
            sw.WriteLine("F2 \"~\" -70 0 30 V V C CNN");
            sw.WriteLine("F3 \"~\" 0 0 30 H V C CNN");
            sw.WriteLine("DRAW");
            sw.WriteLine("S -100 30 100 -30 0 1 0 N");
            sw.WriteLine("X ~ 1 -150 0 50 R 60 60 1 1 P");
            sw.WriteLine("X ~ 2 150 0 50 L 60 60 1 1 P");
            sw.WriteLine("ENDDRAW");
            sw.WriteLine("ENDDEF");
        }

        private void PlotZigzagType(StreamWriter sw, string component_name)
        {
            sw.WriteLine("#");
            sw.WriteLine("# {0}", component_name);
            sw.WriteLine("#");
            sw.WriteLine("DEF {0} R 0 0 N Y 1 F N", component_name);
            sw.WriteLine("F0 \"R\" -125 60 40 H V L CNN");
            sw.WriteLine("F1 \"{0}\" 0 -70 40 H V C CNN", component_name);
            sw.WriteLine("F2 \"~\" -70 0 30 V V C CNN");
            sw.WriteLine("F3 \"~\" 0 0 30 H V C CNN");
            sw.WriteLine("DRAW");
            sw.WriteLine("P 8 0 1 0  -105 0  -87 30  -52 -30  -17 30  17 -30  52 30  87 -30  105 0 N");
            sw.WriteLine("X ~ 1 -150 0 45 R 60 60 1 1 P");
            sw.WriteLine("X ~ 2 150 0 45 L 60 60 1 1 P");
            sw.WriteLine("ENDDRAW");
            sw.WriteLine("ENDDEF");
        }

        private List<string> names;
    }

    public class KiLib_C
    {
        public KiLib_C()
        {
			names_ceramic = new List<string>()
			{
				"C",
				"C(1608)",
				"C(2012)"
			};
			names_pole = new List<string>()
			{ 
				
			};
        }


		public void Plot(StreamWriter sw)
		{
			foreach (string name in names_ceramic)
			{
				PlotCeramicType(sw, name);
			}
			foreach (string name in names_pole)
			{
				PlotPoleType(sw, name);
			}
		}

        

        private void PlotCeramicType(StreamWriter sw, string component_name)      //セラミック，フィルム
        {
            sw.WriteLine("#");
            sw.WriteLine("# {0}", component_name);
            sw.WriteLine("#");
            sw.WriteLine("DEF {0} C 0 10 N N 1 F N", component_name);
            sw.WriteLine("F0 \"C\" 10 70 40 H V L CNN");
            sw.WriteLine("F1 \"{0}\" 10 -75 40 H V L CNN", component_name);
            sw.WriteLine("F2 \"~\" 38 -150 30 H V C CNN");
            sw.WriteLine("F3 \"~\" 0 0 60 H V C CNN");
            sw.WriteLine("DRAW");
            sw.WriteLine("S -60 -30 60 -15 0 1 1 F");
            sw.WriteLine("S -60 30 60 15 0 1 1 F");
            sw.WriteLine("X ~ 1 0 100 75 D 40 40 1 1 P");
            sw.WriteLine("X ~ 2 0 -100 75 U 40 40 1 1 P");
            sw.WriteLine("ENDDRAW");
            sw.WriteLine("ENDDEF");
        }

        private void PlotPoleType(StreamWriter sw, string component_name)     //有極電解
        {
			sw.WriteLine("#");
			sw.WriteLine("# {0}", component_name);
			sw.WriteLine("#");
			sw.WriteLine("DEF {0} C 0 10 N N 1 F N", component_name);
			sw.WriteLine("F0 \"C\" 10 70 40 H V L CNN");
			sw.WriteLine("F1 \"{0}\" 10 -75 40 H V L CNN", component_name);
			sw.WriteLine("F2 \"~\" 38 -150 30 H V C CNN");
			sw.WriteLine("F3 \"~\" 0 0 60 H V C CNN");
			sw.WriteLine("DRAW");
			sw.WriteLine("T 0 -30 55 30 0 0 0 +  Normal 0 C C");
			sw.WriteLine("S -60 -35 60 -20 0 1 1 F");
			sw.WriteLine("S -60 35 60 20 0 1 1 F");
			sw.WriteLine("P 2 0 1 10  -20 25  -50 -25 N");
			sw.WriteLine("P 2 0 1 10  15 25  -15 -25 N");
			sw.WriteLine("P 2 0 1 10  50 25  20 -25 N");
			sw.WriteLine("X p 1 0 100 70 D 40 40 1 1 P");
			sw.WriteLine("X m 2 0 -100 70 U 40 40 1 1 P");
			sw.WriteLine("ENDDRAW");
			sw.WriteLine("ENDDEF");
        }

		private void PlotPoleType2(StreamWriter sw, string component_name)        //無極性電解
        {
			sw.WriteLine("#");
			sw.WriteLine("# {0}", component_name);
			sw.WriteLine("#");
			sw.WriteLine("DEF {0} C 0 10 N N 1 F N", component_name);
			sw.WriteLine("F0 \"C\" 10 70 40 H V L CNN");
			sw.WriteLine("F1 \"{0}\" 10 -75 40 H V L CNN", component_name);
			sw.WriteLine("F2 \"~\" 38 -150 30 H V C CNN");
			sw.WriteLine("F3 \"~\" 0 0 60 H V C CNN");
			sw.WriteLine("DRAW");
			sw.WriteLine("S -60 -35 60 -20 0 1 1 F");
			sw.WriteLine("S -60 35 60 20 0 1 1 F");
			sw.WriteLine("P 2 0 1 10  -20 25  -50 -25 N");
			sw.WriteLine("P 2 0 1 10  15 25  -15 -25 N");
			sw.WriteLine("P 2 0 1 10  50 25  20 -25 N");
			sw.WriteLine("X p 1 0 100 70 D 40 40 1 1 P");
			sw.WriteLine("X m 2 0 -100 70 U 40 40 1 1 P");
			sw.WriteLine("ENDDRAW");
			sw.WriteLine("ENDDEF");
        }

		private List<string> names_ceramic;
		private List<string> names_pole;
    }

	public class KiLib_Tr
	{
		public KiLib_Tr()
		{
			names_npn = new List<string>()
			{
				"2SC2712",
				"2SC2713",
				"2SC2859",
				"2SC2881",
				"2SC3076",
				"2SC3138",
				"2SC3265",
				"2SC3303",
				"2SC3324",
				"2SC3325",
				"2SC3326",
				"2SC4116",
				"2SC4117",
				"2SC4118",
				"2SC4213",
				"2SC4497",
				"2SC4738",
				"2SC5198",
				"2SC5199",
				"2SC5200N",
				"2SC5200",
				"2SC5232v",
				"2SC5233",
				"2SC5242v",
				"2SC5354v",
				"2SC5358",
				"2SC5359",
				"2SC5376CT",
				"2SC5376FV",
				"2SC5376",
				"2SC5548A",
				"2SC5692",
				"2SC5703",
				"2SC5712",
				"2SC5713",
				"2SC5714",
				"2SC5738",
				"2SC5755",
				"2SC5784",
				"2SC5785",
				"2SC5810",
				"2SC5819",
				"2SC5886A",
				"2SC5906v",
				"2SC5948v",
				"2SC5949v",
				"2SC5976v",
				"2SC6000",
				"2SC6026CTv",
				"2SC6026MFV",
				"2SC6033",
				"2SC6061",
				"2SC6076",
				"2SC6100",
				"2SC6124",
				"2SC6125",
				"2SC6126",
				"2SC6127",
				"2SC6133",
				"2SC6134",
				"2SC6135",
				"2SC6142",
				"2SD1223",
				"2SD2686",
				"2SD2719",
				"TPC6501",
				"TPC6502",
				"TPC6503",
				"TPC6504",
				"TPCP8504",
				"TPCP8505",
				"TPCP8507",
				"TPCP8510",
				"TPCP8511",
				"TTC0001",
				"TTC0002",
				"TTC005",
				"TTC007",
				"TTC008",
				"TTC012",
				"TTC013",
				"TTC3710B",
				"TTC4116FU",
				"TTC5200",
				"TTD1409B",
				"TTD1410B",
				"TTD1415B"

			};
			names_pnp = new List<string>()
			{
				"2SA1162",
				"2SA1163",
				"2SA1201",
				"2SA1225",
				"2SA1241",
				"2SA1244",
				"2SA1298",
				"2SA1312",
				"2SA1313",
				"2SA1362",
				"2SA1483",
				"2SA1586",
				"2SA1587",
				"2SA1588",
				"2SA1620",
				"2SA1621v",
				"2SA1721v",
				"2SA1832v",
				"2SA1941",
				"2SA1942",
				"2SA1943N",
				"2SA1943",
				"2SA1953",
				"2SA1954",
				"2SA1955FV",
				"2SA1955",
				"2SA1962",
				"2SA1971",
				"2SA1986",
				"2SA1987",
				"2SA2034",
				"2SA2056",
				"2SA2058",
				"2SA2059",
				"2SA2060",
				"2SA2061",
				"2SA2065",
				"2SA2066",
				"2SA2069",
				"2SA2070",
				"2SA2097",
				"2SA2120",
				"2SA2121",
				"2SA2142",
				"2SA2154CT",
				"2SA2154MF",
				"2SA2184",
				"2SA2195",
				"2SA2206",
				"2SA2214",
				"2SA2215",
				"2SB906",
				"TPC6601",
				"TPC6602",
				"TPC6603",
				"TPC6604",
				"TPCP8601",
				"TPCP8602",
				"TPCP8604",
				"TTA0001",
				"TTA0002",
				"TTA003",
				"TTA005",
				"TTA007",
				"TTA1452B",
				"TTA1586FU",
				"TTA1943",
				"TTB001",
				"TTB002",
				"TTB1020B"

			};
			names_mosfet_n = new List<string>()
			{
				"2SK1062",
				"2SK1359",
				"2SK1828",
				"2SK1829",
				"2SK1830",
				"2SK2009",
				"2SK2033",
				"2SK2034",
				"2SK2035"
			};
			names_mosfet_p = new List<string>()
			{
				"2SJ168",
				"2SJ305",
				"2SJ360",
				"2SJ668",
				"2SJ680",
				"2SJ681"
			};
		}

		public void Plot(StreamWriter sw)
		{
			foreach(string name in names_npn)
			{
				PlotNPN(sw, name);
			}
			foreach (string name in names_pnp)
			{
				PlotPNP(sw, name);
			}
			foreach (string name in names_mosfet_n)
			{
				PlotMOSFETn(sw, name);
			}
			foreach (string name in names_mosfet_p)
			{
				PlotMOSFETp(sw, name);
			}
		}

		private void PlotNPN(StreamWriter sw, string component_name)
		{
			sw.WriteLine("#");
            sw.WriteLine("# {0}", component_name);
            sw.WriteLine("#");
            sw.WriteLine("DEF {0} Q 0 40 Y N 1 F N",component_name);
            sw.WriteLine("F0 \"Q\" 100 100 40 H V L CNN");
            sw.WriteLine("F1 \"{0}\" 100 -100 40 H V L CNN",component_name);
            sw.WriteLine("F2 \"~\" -50 0 60 H V C CNN");
            sw.WriteLine("F3 \"~\" -50 0 60 H V C CNN");
            sw.WriteLine("DRAW");
			sw.WriteLine("C 0 0 111 0 1 0 f");
			sw.WriteLine("S -60 -75 -40 75 0 1 1 F");
			sw.WriteLine("P 2 0 1 0  -50 -25  50 -100 N");
			sw.WriteLine("P 2 0 1 0  -50 25  50 100 N");
			sw.WriteLine("P 4 0 1 0  42 -94  0 -80  16 -57  42 -94 F");
			sw.WriteLine("X B ~ -150 0 100 R 40 40 1 1 I");
			sw.WriteLine("X C ~ 50 150 50 D 40 40 1 1 P");
			sw.WriteLine("X E ~ 50 -150 50 U 40 40 1 1 P");
            sw.WriteLine("ENDDRAW");
            sw.WriteLine("ENDDEF");
		}
		private void PlotPNP(StreamWriter sw, string component_name)
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
			sw.WriteLine("S -60 -75 -40 75 0 1 1 F");
			sw.WriteLine("P 2 0 1 0  -50 -25  50 -100 N");
			sw.WriteLine("P 2 0 1 0  -50 25  50 100 N");
			sw.WriteLine("P 4 0 1 0  -34 -37  8 -51  -8 -73  -34 -37 F");
			sw.WriteLine("X B ~ -150 0 100 R 40 40 1 1 I");
			sw.WriteLine("X C ~ 50 150 50 D 40 40 1 1 P");
			sw.WriteLine("X E ~ 50 -150 50 U 40 40 1 1 P");
			sw.WriteLine("ENDDRAW");
			sw.WriteLine("ENDDEF");
		}
		private void PlotJFETn(StreamWriter sw, string component_name)
		{

		}
		private void PlotJFETp(StreamWriter sw, string component_name)
		{

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

		private List<string> names_npn;
		private List<string> names_pnp;
		private List<string> names_mosfet_n;
		private List<string> names_mosfet_p;
	}
}
