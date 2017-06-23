using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using JarrettVance.ChapterTools;
using MeGUI;
using Newtonsoft.Json;
using CoverToFront;
using System.Globalization;
using Microsoft.Win32;
using System.Media;

namespace TFFtest
{
    public partial class Form1 : Form
    {

        public class Config
        {
            public decimal bps { set; get; }
            public decimal crf { set; get;}
            public String Preset { set; get; }
            public String EncoderType { set; get; }
            public String Workshop { set; get; }

            public String WorkPath { set; get; }

            public String CoderValues { set; get; }

            public int Parts { set; get; }


        }

         [Flags] public enum CodeType { TFF,BFF,Progressive, TIVTC };

     

        const String configfile = "config.json";

  

        //String rootdir = "k";

        String FilePrefix = "";


        private String AvsFile
        {
            get { return textBox1.Text.BeforeLast(".") + ".avs"; }
        }



        void StoreJson()
        {
            File.WriteAllText(Path.ChangeExtension(textBox1.Text, "json"), JsonConvert.SerializeObject(dvddata));
        }


        private Dictionary<String, String> FilmNames = new Dictionary<String, String>();
        public Form1()
        {



            InitializeComponent();

            if (File.Exists("Last.txt"))  textBox1.Text =  File.ReadAllText("Last.txt");

            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1) 
            {
                 textBox1.Text = args[1]; 
            }

            if ( String.IsNullOrEmpty(textBox1.Text)) return;
            
            Other.Items.Clear();

            try
            {
                foreach (var d2v in Directory.GetFiles(Path.GetDirectoryName(textBox1.Text), "*.d2v"))
                {
                    Other.Items.Add(d2v);
                }

            }
            catch 
            {

            }

           
           
        }


        private void DisplayInfo()
        {
            if (dvddata.Info == null || dvddata.Vobs == null) return;


            listBox1.Items.Clear();
            listBox1.Items.AddRange(dvddata.Info.Split(new String[] { "\r\n" }, StringSplitOptions.None));

            listBox2.Items.Clear();
            var v = File.ReadAllLines(textBox1.Text,Encoding.GetEncoding(1251));
            for (int i = 2; i < 100; i++)
                if (v[i] == "") break;
                else
                    listBox2.Items.Add(v[i]);


         //  listBox2.DataSource= dvddata.Vobs.Split(new String[] { "\r\n" }, StringSplitOptions.None);

            if (dvddata.DetectResult != null)
            {
                label1.Text = @"SourceType :" + dvddata.DetectResult.Type + @", FieldOrder : " +
                              dvddata.DetectResult.FieldOrder;

                Analyse.Text = dvddata.DetectResult.Text + "\r\n\r\n"; ;



                Analyse.Text += dvddata.DetectResult.FieldOrder + "\r\n";
                Analyse.Text += dvddata.DetectResult.Type + "\r\n\r\n";
            }


            textBox3.Text = dvddata.FilmName;
            textBox4.Text = dvddata.PartName;

            if (File.Exists(textBox1.Text.BeforeLast(".") + ".avs"))
            {
                Script.Text = File.ReadAllText(textBox1.Text.BeforeLast(".") + ".avs", Encoding.GetEncoding(1251));
            }
            else
            {
                Script.Text = null;
            }

            if (!dvddata.MatchChapter) MessageBox.Show("No Mached chapters", "Warning", MessageBoxButtons.OK);

            if (String.IsNullOrEmpty(textBox3.Text))
            {
                var path = Path.GetDirectoryName(textBox1.Text);
                var fn = "";
                if (FilmNames.TryGetValue(path, out fn))
                    textBox3.Text = fn;
            }



            DVDInfo.Text = File.ReadAllText(Path.ChangeExtension((String)listBox2.Items[0], ".log"));


            IsFilm.Text = File.ReadAllLines(textBox1.Text).Last().AfterFirst("FINISHED  ");

            listBox1.Items.Add(GetColometry());


        }

        String GetColometry()
        {

            var values = new Dictionary<String, String>()
            {
                { "BT.470-2 B,G","bt470bg" },
                { "SMPTE 170M","smpte170m" },
                { "SMPTE 240M","smpte240m" },
                 

            };
                
            var c =    DVDInfo.Text.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(f => f.StartsWith("Colorimetry: ")).AfterFirst("Colorimetry: ")
                .Replace("*","") ;

            return values[c];


        }

        private void ClearDisplayInfo()
        {
            listBox1.Text = "";
            listBox2.Text = "";
            label1.Text = "";
        }


        private void PrepareScript(CodeType type, FieldOrder fieldorder = FieldOrder.UNKNOWN)
        {
            

             if (fieldorder == FieldOrder.UNKNOWN) fieldorder = dvddata.DetectResult.FieldOrder;

            dvddata.codetype = type; StoreJson();

            var dvdtml = File.ReadAllText(@"data\"+ type.ToString() + "_DVD.template", Encoding.GetEncoding(1251));

            Script.Text = "#" + FilePrefix + " " + type.ToString()+ "\r\n" + dvdtml
                .Replace("{d2v}", textBox1.Text)
                .Replace("{fieldorder}", fieldorder == FieldOrder.TFF ? "1" : "0");

            SaveAndTest();
        }

        //***************************************************************************************************************************************
        private void DoScript(object sender, EventArgs e)
        {

            var templatenames = ((Button)sender).Text;

            var t = (CodeType)Enum.Parse(typeof(CodeType), (String)templatenames);

            PrepareScript(t);

            button31_Click(null, null);

            //dvddata.codetype = CodeType.TFF; StoreJson();

            //var dvdtml = File.ReadAllText(@"data\TFF_DVD.template", Encoding.GetEncoding(1251));
            //Script.Text = "#" + FilePrefix + " TFF\r\n" + dvdtml.Replace("{0}", textBox1.Text);

            //SaveAndTest();
        }

        //private void button3_Click(object sender, EventArgs e)
        //{
          
        //    dvddata.codetype = CodeType.BFF; StoreJson();
            
        //    var dvdtml = File.ReadAllText(@"data\BFF_DVD.template", Encoding.GetEncoding(1251));
        //    Script.Text = "#" + FilePrefix + " BFF\r\n" + dvdtml.Replace("{0}", textBox1.Text);

        //    SaveAndTest();
        //}


        //private void button8_Click(object sender, EventArgs e)
        //{
           
        //    dvddata.codetype = CodeType.Progressive; StoreJson();
            
        //    var dvdtml = File.ReadAllText(@"data\FILM_DVD.template", Encoding.GetEncoding(1251));
        //    Script.Text = "#" + FilePrefix + " Progressive\r\n" + dvdtml.Replace("{0}", textBox1.Text);

        //    SaveAndTest();
        //}

        


        //private void button12_Click(object sender, EventArgs e)
        //{

        //    dvddata.codetype = CodeType.TIVTC; StoreJson();

        //    var dvdtml = File.ReadAllText(@"data\TIVTC_DVD.template", Encoding.GetEncoding(1251));
        //    Script.Text = "#" + FilePrefix + " TIVTC\r\n" + dvdtml.Replace("{0}", textBox1.Text).Replace("{1}", dvddata.DetectResult.FieldOrder == FieldOrder.TFF ? "1" : "0");

        //    SaveAndTest();
        //}

        //***************************************************************************************************************************************


        static int Nod(int n, int d)
        {
            int temp;
            n = Math.Abs(n);
            d = Math.Abs(d);
            while (d != 0 && n != 0)
            {
                if (n % d > 0)
                {
                    temp = n;
                    n = d;
                    d = temp % d;
                }
                else break;
            }
            if (d != 0 && n != 0) return d;
            else return 0;
        }



      

        public class DvdData
        {
            public IEnumerable<String> AudioFiles { set; get; }
            public String  SAR { set; get; }

            public Double Frame_Rate { set; get; }
            public String Aspect_Ratio { set; get; }
            public String Picture_Size { set; get; }
            public TimeSpan Duration { set; get; }

            public int Frames { set; get; }

            public bool MatchChapter { set; get; }

            public String ChapterFile { set; get; }

            public ChapterInfo Chapter {set; get;}

            public String Info { set; get; }
            public String Vobs { set; get; }

            public DetectResult DetectResult { set; get; }

            public String FilmName { set; get; }
            public String PartName { set; get; }

          

            public CodeType codetype { get; set; }

            public Dictionary<String,String> AudioLang { set; get; }


        }

        private DvdData dvddata;

        private void GetDvdData(String d2vfile)
        {

            MakeInfo(  Path.ChangeExtension(d2vfile,"avs"));
            var jsonfile = Path.ChangeExtension(textBox1.Text, "json");
            if (File.Exists(jsonfile))
            {
                dvddata = JsonConvert.DeserializeObject<DvdData>(File.ReadAllText(jsonfile));
             //   if (dvddata.Info != null && dvddata.Vobs != null) return;
                //  if (dvddata.DetectResult == null) button11_Click(null, null);
             
            }
            else
            {
                dvddata = new DvdData();
            }
          


     

            listBox1.Items.Clear();
            var d2vname = Path.GetFileNameWithoutExtension(d2vfile);

            


            dvddata.AudioFiles = Directory.GetFiles(Path.GetDirectoryName(d2vfile)).Where(f => Path.GetFileNameWithoutExtension(f).StartsWith(d2vname) &&
                (Path.GetExtension(f) == ".mp2" || Path.GetExtension(f) == ".ac3"));       

            FilePrefix = "";
            var lines = File.ReadAllLines(d2vfile,Encoding.GetEncoding(1251));

            int pcnt = 0; int fcnt = 0; int w, h, dh, dw,sar1,sar2;
            w=h=dh=dw=sar1=sar2=0;
            String  dvdpath = "";
     

            for (int i = 0; i < lines.Count(); i++)
            {
                if (i == 2)
                {
                    dvdpath = Path.GetDirectoryName(lines[i]).BeforeLast("\\");
                    int j = i;
                    while (lines[j]!="") dvddata.Vobs += lines[j++] + "\r\n";

                }


                var s =lines[i];
                if (s.StartsWith("Aspect_Ratio="))
                {
                    dvddata.Aspect_Ratio = s.AfterLast("=");
                    dw = int.Parse(dvddata.Aspect_Ratio.BeforeFirst(":"));
                    dh = int.Parse(dvddata.Aspect_Ratio.AfterFirst(":"));
                }
                if (s.StartsWith("Picture_Size="))
                {
                    dvddata.Picture_Size = s.AfterLast("=");
                    w = int.Parse(dvddata.Picture_Size.BeforeFirst("x"));
                    h = int.Parse(dvddata.Picture_Size.AfterFirst("x"));
                }

                if (s.StartsWith("Frame_Rate="))
                {
                    dvddata.Frame_Rate = Double.Parse(s.TextBetween("=", " "));
                    if (dvddata.Frame_Rate == 29970) dvddata.Frame_Rate = 30000000 / 1001.0;
                }

                if (lines[i]=="") {
                    pcnt++;
                    continue;
                }

            //   if (pcnt==3) break;
                if (pcnt == 2)
                {
                    var f = lines[i].Split(' ');
                    fcnt += f.Length - 7;
                    if (f[f.Length - 1] == "ff")
                    {
                        fcnt--;
                        break;
                    }
                }

                

            }

            var nod = Nod(dw * h,dh * w);

            using (AvsFile af = MeGUI.AvsFile.ParseScript($"LoadPlugin(\"C:\\programs\\MeGUI\\tools\\dgindex\\DGDecode.dll\")\r\nDGDecode_mpeg2source(\"{d2vfile}\")"))
            {
                fcnt = af.FrameCount;
            }

            dvddata.Frames = fcnt;
            dvddata.SAR = $"{dw*h/nod}:{dh*w/nod}";
            dvddata.Duration = TimeSpan.FromSeconds(fcnt / (dvddata.Frame_Rate / 1000.0));

            FilePrefix += String.Format("{2:F2}FPS {3} {4} sar -{0}:{1} duration {5}",
                dw * h / nod, dh * w / nod, dvddata.Frame_Rate / 1000.0, dvddata.Aspect_Ratio, dvddata.Picture_Size, dvddata.Duration);

            dvddata.Info += FilePrefix + "\r\n";


            dvddata.Info += fcnt.ToString() + " " + dvddata.Aspect_Ratio + " " + dvddata.Picture_Size + " " +
                            dvddata.Frame_Rate
                            + " " + TimeSpan.FromSeconds(fcnt/(dvddata.Frame_Rate/1000.0)).ToString() + "\r\n";


            var ext = new JarrettVance.ChapterTools.Extractors.DvdExtractor();
            var ch = ext.GetStreams(dvdpath);

            bool conv = false;
            var t = Path.GetFileNameWithoutExtension(textBox1.Text); t=t.BeforeLast("_")+"_0";
            var chapter = ch.FirstOrDefault(f => f.Title.Substring(1,7) == t.Substring(1,7));
            if (chapter != null)
            {
                dontchapters.Checked = chapter.Chapters.Count == 0;// Duration.TotalMinutes < 30;
                dvddata.Info += "Chapter-Duration " + chapter.Duration.ToString() + "\r\n";
                dvddata.MatchChapter = Math.Abs(chapter.Duration.TotalMilliseconds - dvddata.Duration.TotalMilliseconds) < 500;

                if (Math.Abs(dvddata.Frame_Rate - 30000000 / 1001.0) < 0.1)
              //  if (!dvddata.MatchChapter)
                 {
                       
                     var CorDuration = TimeSpan.FromMilliseconds(chapter.Duration.TotalMilliseconds * 30000.0 / dvddata.Frame_Rate);
                        dvddata.Info += "Corrected Chapter-Duration " + CorDuration.ToString() + "\r\n";

                     var dif = Math.Abs(CorDuration.TotalMilliseconds - dvddata.Duration.TotalMilliseconds);
                         dvddata.MatchChapter =
                             (dif < 500);

                         dvddata.Info += "Match 30>29,970 converson\r\n";

                    dvddata.Info += "Different :"+dif+" \r\n";

                    conv = true;
                         for (int i = 0; i < chapter.Chapters.Count; i++)
                         {
                             var f = chapter.Chapters[i];
                             f.Time = TimeSpan.FromMilliseconds(chapter.Chapters[i].Time.TotalMilliseconds * 30000.0 / dvddata.Frame_Rate);
                             chapter.Chapters[i] = f;
                         }
                     
                         if (!dvddata.MatchChapter) dvddata.Info += "Not Matched Сonverson\r\n";
                       
                     }

                 dvddata.ChapterFile = textBox1.Text.BeforeLast(".") + "_Chapters" + (!conv ? "" : "_FIX") + ".txt";
                 dvddata.Chapter = chapter; 

                 chapter.SaveText(dvddata.ChapterFile);


                StoreJson();

             

            }

          
        }


        private String GetLang(String fn)
        {
            if (dvddata.AudioLang == null) GetAudioLang();

            try
            {
                return dvddata.AudioLang[Path.GetFileName(fn).Substring(10, 2)];
            }
            catch
            {
                return null;
            }
        }

        bool LastHi10 = true;
        private void button5_Click(object sender, EventArgs e)
        {
            String Dest264File, AudioFile;

       


            var coverfile = Path.Combine(Path.GetDirectoryName(AvsFile), "MP4BOX-Cover.jpg");

            if (String.IsNullOrEmpty(textBox4.Text) && !packet)
                if (!File.Exists(coverfile))
                {
                    MessageBox.Show("No Cover");
                    return;
                }

            var cover = File.Exists(coverfile) && String.IsNullOrEmpty(textBox4.Text) ? $" -itags \"cover={coverfile}\"" : "";



            var existaudio = dvddata.AudioFiles.Where(f => File.Exists(f)).ToList();

 
            if (existaudio.Count() == 0)
            {
                MessageBox.Show("No Audio");
                return;
            }

            if (existaudio.Count() > 1)
            {
                MessageBox.Show("Many Audio");
                return;
            }

            // выясняем битрейт


            int bitrate;
            using (AvsFile af = MeGUI.AvsFile.ParseScript(Script.Text))
            {
                bitrate =  (int)Math.Round(af.FPS * af.Width * af.Height *  (double)Bitrate.Value / 1000);
            }


            label7.Text = bitrate.ToString();



            if (sender != null)
                LastHi10 = ((Button)sender).Tag == "0";

            // #trackID=1:lang=en:name=
            // N:\Private\[Convert]\(2003) - Fashionistas\Disc2-conv\Fashionistas DVD2.rus.mp4#trackID=1:lang=ru:name=

            //"C:\programs\MeGUI\tools\mp4box\mp4box.exe"
            //-add "N:\Private\[Convert]\(2003) - Fashionistas\Disc2-conv\Fashionistas DVD2.264#trackID=1:fps=23.976:name="
            //-add "N:\Private\[Convert]\(2003) - Fashionistas\Disc2-conv\Fashionistas DVD2.rus.mp4#trackID=1:lang=ru:name="
            //-add "N:\Private\[Convert]\(2003) - Fashionistas\Disc2-conv\Fashionistas DVD2.eng.mp4#trackID=1:lang=en:name="
            //-chap "N:\Private\[Convert]\(2003) - Fashionistas\Disc2-conv\Fashionistas DVD2.chapters.txt"
            //-tmp "N:\Private\[Convert]\(2003) - Fashionistas\Disc2-conv"
            //-new "N:\Private\[Convert]\(2003) - Fashionistas\Disc2-conv\Fashionistas DVD2.rus-muxed.mp4"

            File.WriteAllText(AvsFile, Script.Text, Encoding.GetEncoding(1251));

            AudioFile = existaudio.First().BeforeLast(".") + ".m4a";



           var WavAudioFile = existaudio.First().BeforeLast(".") + ".wav";

           var AudioDelay = AudioFile.BeforeLast(".").AfterLast(" "); if (AudioDelay == "0ms") AudioDelay = "";

           Dest264File = AvsFile.BeforeLast(".")+".264";

            var lng = GetLang(AudioFile);
            var audiopart = lng==null ? $" -add \"{AudioFile}#Audio\"" :
                $" -add \"{AudioFile}#Audio:lang={lng}\"";
            //  -add "{dstaudio}#trackID=1" :lang=ru

            var temp = File.ReadAllText(@"DATA\" + EncodeType.Text + ".cmd")
                .Replace("{AvsFile}", AvsFile)
                .Replace("{muxed}", AvsFile.BeforeLast(".") + "_muxed.mp4")
                .Replace("{srcaudio}", existaudio.First())
                .Replace("{dstaudio}", AudioFile)
                .Replace("{dstaudiocode}", audiopart)
                .Replace("{tempaudio}", WavAudioFile)
                .Replace("{AudioDelay}", AudioDelay)

                .Replace("{Preset}", Preset.Text)
                .Replace("{StatFile}", AvsFile+".stats")
                .Replace("{bitrate}", bitrate.ToString())
                .Replace("{CRF}",  ((int)crf.Value).ToString()  )

                .Replace("{SAR}", dvddata.SAR)
                .Replace("{LogFile}", Path.ChangeExtension(AvsFile, ".log"))
                .Replace("{ChapterCode}", dontchapters.Checked ? "" : "-chap \"" + dvddata.ChapterFile + "\"")
                .Replace("{Dest264}", Dest264File)
                .Replace("{codervalues}",  CoderValues.Text )
                .Replace("{colormatrix}",  GetColometry() )
                .Replace("{cover}", cover)


                .Replace("{binary}", LastHi10 ? @"c:\programs\MeGUI\tools\x264_10b\avs4x264mod.exe --x264-binary c:\programs\MeGUI\tools\x264_10b\x264-10b_64.exe"
                  : @"c:\programs\MeGUI\tools\x264\avs4x264mod.exe --x264-binary c:\programs\MeGUI\tools\x264\x264_64.exe");


            //"C:\programs\MeGUI\tools\mp4box\mp4box.exe"
            //-add "N:\Private\[Convert]\(2003) - Fashionistas\Disc2-conv\Fashionistas DVD2.264#trackID=1:fps=23.976:name="
            //-add "N:\Private\[Convert]\(2003) - Fashionistas\Disc2-conv\Fashionistas DVD2.rus.mp4#trackID=1:lang=ru:name="
            //-add "N:\Private\[Convert]\(2003) - Fashionistas\Disc2-conv\Fashionistas DVD2.eng.mp4#trackID=1:lang=en:name="
            //-chap "N:\Private\[Convert]\(2003) - Fashionistas\Disc2-conv\Fashionistas DVD2.chapters.txt"
            //-tmp "N:\Private\[Convert]\(2003) - Fashionistas\Disc2-conv"

            //-new "N:\Private\[Convert]\(2003) - Fashionistas\Disc2-conv\Fashionistas DVD2.rus-muxed.mp4"

            var rusfile = Directory.GetFiles(Path.GetDirectoryName(AudioFile), Path.GetFileName(AudioFile).Substring(0,8) + ".rus.m4a");

            var rus = rusfile.Length==0 ? "" : " -add \"" + rusfile[0] + "#trackID=1:lang=ru\" ";
            //#trackID=1:lang=ru


            File.WriteAllText(textBox1.Text.BeforeLast(".") + "_Encode.cmd",temp, Encoding.GetEncoding(866));


            if (!String.IsNullOrWhiteSpace(dvddata.FilmName)) { 
                var _DVDFile =  Path.GetDirectoryName(textBox1.Text)+ "\\" + dvddata.FilmName +
                          (String.IsNullOrWhiteSpace(dvddata.PartName) ? "" : " - " + dvddata.PartName) + ".avs";

                var n =
    string.Format(
    "\r\n\r\nfunction getDAR(clip c, float SAR) {{ return Float(c.width) * SAR / Float(c.height) }}\r\n" +
    "DAR = getDAR(Float({0}) / Float({1}))\r\n" +
    "LanczosResize(width, Round(width / DAR))\r\n", dvddata.SAR.BeforeFirst(":"),
    dvddata.SAR.AfterFirst(":"));

                File.WriteAllText(_DVDFile, Script.Text.Replace("FFT3DFilter(", "#FFT3DFilter(") + n, Encoding.GetEncoding(1251));

            }

        }

        private void listBox2_DragOver(object sender, DragEventArgs e)
        {
        //    if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;  
        }

        private void listBox2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        string[] vodfiles;
        private void listBox2_DragDrop(object sender, DragEventArgs e)
        {
            vodfiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            listBox2.Items.Clear();
            listBox1.Items.Clear();
            foreach (string file in vodfiles) listBox2.Items.Add(file);

            FilmName.Text = Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(vodfiles[0]))).Trim();

            if (FilmName.Text=="") {
               
            var d = new DriveInfo(vodfiles[0]);
            FilmName.Text = d.VolumeLabel.Trim();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var dir = root.Text+ @":\[MAKER]\" +
                (String.IsNullOrEmpty(WorkPath.Text) ?   DateTime.Today.Year.ToString()+"_"+DateTime.Today.Month.ToString()  : WorkPath.Text) 
                
                +@"\" + FilmName.Text;

            Directory.CreateDirectory(dir);
            textBox1.Text = dir + @"\" + Path.GetFileNameWithoutExtension(vodfiles[0]) + ".d2v";
//            var s = String.Format(@" -IF=[{0}] -OF=[{1}] -EXIT", String.Join(",", vodfiles), dir + @"\"+Path.GetFileNameWithoutExtension(vodfiles[0]));

            var s = String.Format(@" -i {0} -o ""{1}"" -exit", "\"" + String.Join("\" \"", vodfiles) + "\"", dir + @"\" + Path.GetFileNameWithoutExtension(vodfiles[0]));
            var pr= System.Diagnostics.Process.Start(@"c:\programs\DGMPGDec\DGIndex.exe", s);
            pr.WaitForExit();
            SystemSounds.Hand.Play();

            GetDvdData(textBox1.Text);
            DisplayInfo();

           

        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
          
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var l = "";

            foreach (var f in Directory.GetFiles(textBox2.Text, "*.cmd", SearchOption.AllDirectories))
            {
                l+= "\r\n\r\n"+File.ReadAllText(f);
            }

            File.WriteAllText(textBox2.Text + "all.cmd", l);

        }


        void SaveAndTest()
        {
            File.WriteAllText(AvsFile, Script.Text, Encoding.GetEncoding(1251));
         
        }
        private void button9_Click(object sender, EventArgs e)
        {
            SaveAndTest();
            if (!packet) System.Diagnostics.Process.Start(AvsFile);
        }

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            vodfiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            textBox1.Text = vodfiles[0];

            Other.DataSource = Directory.GetFiles(Path.GetDirectoryName(textBox1.Text), "*.d2v");

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveState();
        }
        private void SaveState()
        {

            SaveAndTest();

            File.WriteAllText("Last.txt", textBox1.Text);



                var cfg = new Config();

                cfg.crf = crf.Value;
                cfg.bps = Bitrate.Value;
                cfg.EncoderType = EncodeType.Text;
                cfg.Preset = Preset.Text;
                cfg.Workshop = textBox2.Text;
                cfg.WorkPath = WorkPath.Text;
                 cfg.Parts = (int)parts.Value;

            cfg.CoderValues = CoderValues.Text;

            File.WriteAllText(configfile,JsonConvert.SerializeObject(cfg));


            RegistryKey currentUserKey = Registry.CurrentUser;
            RegistryKey helloKey = currentUserKey.OpenSubKey("Software", true).OpenSubKey("mp4maker", true);

            if (helloKey == null) currentUserKey.OpenSubKey("Software", true).CreateSubKey("mp4maker", true);

            helloKey.SetValue("Root", root.Text );

            helloKey.Close();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //  textBox2.Text = @"k:\[MAKER]\" + DateTime.Today.ToShortDateString().Replace(":", "_")+@"\";

            if (File.Exists(configfile)) {

              var cfg = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configfile));

                crf.Value = cfg.crf;
                Bitrate.Value = cfg.bps;
                EncodeType.Text = cfg.EncoderType;
                Preset.Text = cfg.Preset;
                textBox2.Text = cfg.Workshop;

                WorkPath.Text = cfg.WorkPath;

                CoderValues.Text = cfg.CoderValues;

                parts.Value = cfg.Parts;

            }

            var filterlist = Directory.GetFiles("Data\\", "*.filter", SearchOption.TopDirectoryOnly).Select(f => Path.GetFileNameWithoutExtension(f)).ToArray();

            filters.DataSource = filterlist;

            RegistryKey currentUserKey = Registry.CurrentUser;
            RegistryKey helloKey = currentUserKey.OpenSubKey("Software", true).OpenSubKey("mp4maker",true);

            if (helloKey==null) currentUserKey.OpenSubKey("Software", true).CreateSubKey("mp4maker", true);

            root.Text = (String)helloKey.GetValue("Root", "K");
            
            helloKey.Close();

        }

        private void button10_Click(object sender, EventArgs e)
        {
            var p = Process.GetProcessesByName("Janetter");
            foreach (var pp in p)
            {
                listBox1.Items.Add(pp.ProcessName);
               pp.Kill();

                //if (pp.CloseMainWindow()) {
                //    pp.Kill();
                //    listBox1.Items.Add("Kill");
                //}
                //else
                    
                //pp.Close();


            }
           
            TaskBarUtil.RefreshNotificationArea();
            Thread.Sleep(5000);
            Process.Start(@"C:\Program Files (x86)\Janetter2\bin\Janetter.exe");
        }



        public struct RECT
        {
            public int bottom;
            public int left;
            public int right;
            public int top;

        }
        internal class TaskBarUtil
        {
            public static void RefreshNotificationArea()
            {
                var notificationAreaHandle = GetNotificationAreaHandle();
                if (notificationAreaHandle == IntPtr.Zero)
                    return;
                RefreshWindow(notificationAreaHandle);
            }
            private static void RefreshWindow(IntPtr windowHandle)
            {
                const uint wmMousemove = 0x0200;
                RECT rect;
                GetClientRect(windowHandle, out rect);
                for (var x = 0; x < rect.right; x += 5)
                    for (var y = 0; y < rect.bottom; y += 5)
                        SendMessage(
                            windowHandle,
                            wmMousemove,
                            0,
                            (y << 16) + x);
            }
            private static IntPtr GetNotificationAreaHandle_ld()
            {
                const string notificationAreaTitle = "Notification Area";
                const string notificationAreaTitleInWindows7 = "User Promoted Notification Area";
                var systemTrayContainerHandle = FindWindowEx(IntPtr.Zero, IntPtr.Zero,
                                                            "Shell_TrayWnd", string.Empty);
                var systemTrayHandle = FindWindowEx(systemTrayContainerHandle, IntPtr.Zero,
                                                            "TrayNotifyWnd", string.Empty);
                var sysPagerHandle = FindWindowEx(systemTrayHandle, IntPtr.Zero, "SysPager",
                                                            string.Empty);
                var notificationAreaHandle = FindWindowEx(sysPagerHandle, IntPtr.Zero,
                                                            "ToolbarWindow32",
                                                            notificationAreaTitle);
                if (notificationAreaHandle == IntPtr.Zero)
                    notificationAreaHandle = FindWindowEx(sysPagerHandle, IntPtr.Zero,
                                                            "ToolbarWindow32",
                                                            notificationAreaTitleInWindows7);
                return notificationAreaHandle;
            }


            static IntPtr GetNotificationAreaHandle()
            {
                IntPtr hWndTray = FindWindow("Shell_TrayWnd", null);
                if (hWndTray != IntPtr.Zero)
                {
                    hWndTray = FindWindowEx(hWndTray, IntPtr.Zero, "TrayNotifyWnd", null);
                    if (hWndTray != IntPtr.Zero)
                    {
                        hWndTray = FindWindowEx(hWndTray, IntPtr.Zero, "SysPager", null);
                        if (hWndTray != IntPtr.Zero)
                        {
                            hWndTray = FindWindowEx(hWndTray, IntPtr.Zero, "ToolbarWindow32", null);
                            return hWndTray;
                        }
                    }
                }

                return IntPtr.Zero;
            }

            [DllImport("user32.dll", SetLastError = true)]
            static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

            [DllImport("user32.dll", SetLastError = true)]
            static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter,
                                                            string className,
                                                            string windowTitle);
            [DllImport("user32.dll")]
            static extern bool GetClientRect(IntPtr handle, out RECT rect);
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            static extern IntPtr SendMessage(IntPtr handle, UInt32 message, Int32 wParam,
                                                            Int32 lParam);
        }

        bool packet = false;
        String plugintext = null;
        private void button10_Click_1(object sender, EventArgs e)
        {
          

            var codetype = dvddata.codetype;
            var fieldorder = dvddata.DetectResult.FieldOrder;

            plugintext = Script.Text.AfterFirst("#PLUGINS");
            
            var path = Path.GetDirectoryName(listBox2.Items[0].ToString());
            packet = true;
            for (int i =1; i<100; i++)
            {
                var first = String.Format("VTS_{0:d2}_1.VOB",i);
                if (File.Exists(Path.Combine(path,first)))
                {
                    var root = Path.GetDirectoryName(textBox1.Text);
                    var d2vfile =  String.Format("VTS_{0:d2}_1.d2v",i);
                    if (!File.Exists(Path.Combine( root,d2vfile)))
                    {
                        listBox1.Items.Clear();
                        listBox2.Items.Clear();

                        vodfiles = Directory.GetFiles(path, String.Format("VTS_{0:d2}_*.VOB", i), SearchOption.TopDirectoryOnly).Where(f=>!f.EndsWith("_0.VOB")).ToArray();
                     
                                    FilmName.Text = Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(vodfiles[0]))).Trim();

                                    if (FilmName.Text == "")
                                    {

                                        var d = new DriveInfo(vodfiles[0]);
                                        FilmName.Text = d.VolumeLabel.Trim();
                                    }

                        listBox1.Items.Clear();
                        foreach (string file in vodfiles)
                            if (!file.EndsWith("_0.VOB"))  listBox2.Items.Add(file);

                        foreach (var f in vodfiles) listBox1.Items.Add(f);

                            button6_Click(null,null);

                        PrepareScript(codetype, fieldorder);


                        Script.Text = Script.Text.BeforeFirst("#PLUGINS") + plugintext;

                           

                            button5_Click(null, null);
                        
                    }
                }
            }

            packet = false;
        }



        String norm(String src)
        {
            return src.Replace("\n", "").Replace("\r", "").Replace("  ", " ").Trim();
        }

        public void UpdateSourceDetectionStatus(int numDone, int total)
        {
            Invoke((Action)(() => label1.Text = numDone +" / "+total));
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            var t = File.ReadAllText( Path.ChangeExtension(textBox1.Text,"json") );
            dvddata = JsonConvert.DeserializeObject<DvdData>(t);

            DisplayInfo();
        }

        private void button4_Click(object sender, EventArgs e)
        {

            ClearDisplayInfo();

            try
            {
                GetDvdData(textBox1.Text);
                DisplayInfo();
                label5.Text = Path.GetDirectoryName(File.ReadAllLines(textBox1.Text)[2]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void button14_Click(object sender, EventArgs e)
        {

        }

        private void button15_Click(object sender, EventArgs e)
        {
            var scripts = Directory.GetFiles(textBox2.Text, "*.avs", SearchOption.AllDirectories)
                .Where(f => Path.GetFileName(f).Length < 14);

            foreach (var script in scripts)
            {
                MakeInfo(script);
            }
        }

        public  void MakeInfo(String script)
        { 
            if ( !File.Exists(script) ) return;

        var r = "";
                var ll = File.ReadAllLines(script, Encoding.GetEncoding(1251));



                var l = ll.Where(f => !f.StartsWith("#") && !f.StartsWith("LoadPlugin") && !f.StartsWith("Load_Stdcall_Plugin")
                                      && !f.StartsWith("DGDecode_mpeg2source") &&
                                      !f.StartsWith("ColorMatrix") && f.Trim() != "");

                var encfile = Path.GetDirectoryName(script) + @"\" + Path.GetFileNameWithoutExtension(script) +
                              "_Encode.cmd";

                if (!File.Exists(encfile)) return;

                var enc =
                    File.ReadAllLines(encfile)
                        .FirstOrDefault(f => f.Contains(@"tools\x264"))
                        .AfterLast(".exe ")
                        .BeforeLast("--output ");

                if (ll[0].StartsWith("#")) r += "Source: " + ll[0].Substring(1) + "\r\n\r\n";

                r += "avs:\r\n" + String.Join("\r\n", l) + "\r\n\r\n";

                r += "encoder type : " + (File.ReadAllText(encfile).Contains("x264-10b") ? "Hi10P" : "8bit") +
                     "\r\n\r\n";

                   r += "encode:\r\n" + enc + "\r\n\r\n";


            long vobsize = 0;
                var d2v = ll.First(f => f.StartsWith("DGDecode_mpeg2source"))
                    .TextBetween("DGDecode_mpeg2source(\"", "\"");
                try
                {



                    var d2vtext = File.ReadAllLines(d2v);
                    r += "source:\r\n";
                    int i = 2;
                    while (d2vtext[i] != "")
                    {
                       vobsize += new FileInfo(d2vtext[i]).Length; 

                        r += d2vtext[i++] + "\r\n";
                        
                    }
                }
                catch 
                {

                }


            var muxed =   script.Replace(".avs" , "_muxed.mp4");

            if (File.Exists(muxed))
            {
                var muxedlength = (new FileInfo(muxed)).Length;

                var logfn = script.Replace(".avs", ".log");

                var log = File.Exists(logfn) ? File.ReadAllText(logfn)+"\r\n\r\n" : "";

                r += "\r\n\r\n" + log+ vobsize / 1024 / 1024 + "mb -> " + muxedlength / 1024 / 1024 + "mb Compress:" + (int)(100 - (float)muxedlength / (float)vobsize * 100.0) + "%\r\n" +
                   new String('_', 80);
            }

            File.WriteAllText(Path.GetDirectoryName(script) + @"\" + Path.GetFileNameWithoutExtension(script) +
                                        "_EncodeInfo.txt",r);
                

        }

        private void button16_Click(object sender, EventArgs e)
        {
            var scripts = Directory.GetFiles(textBox2.Text, "*.ac3", SearchOption.AllDirectories)
            .Where(f => !f.Contains(" 0ms"));
            Analyse.Text = String.Join("\r\n", scripts);
        }

        private void Other_Click(object sender, EventArgs e)
        {
            var d2v = Other.SelectedItem as String;
            textBox1.Text = d2v;
            button4_Click(null,null);
            //.Replace("FFT3DFilter(", "#FFT3DFilter(")
         //   System.Diagnostics.Process.Start(d2v.Replace(".d2v", " DVD.avs"));

        }

        private void button17_Click(object sender, EventArgs e)
        {
            dvddata.FilmName = textBox3.Text;
            dvddata.PartName = textBox4.Text;
            StoreJson();

            var path = Path.GetDirectoryName(textBox1.Text);
            if ( !FilmNames.ContainsKey(path)) FilmNames.Add(path, dvddata.FilmName);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            var scripts = Directory.GetFiles(textBox2.Text, "*.avs", SearchOption.TopDirectoryOnly)
                .Where(f => Path.GetFileName(f).Length < 14);

            String  AudioFile;

            var l = new List<String>();

            //VTS_08_1.avs
            //VTS_08_1.264
            //VTS_08_1_Chapters_FIX.txt
            //VTS_08_1 T80 2_0ch 192Kbps DELAY 0ms



            foreach (var script in scripts)
            {
                var d2vname = Path.GetFileNameWithoutExtension(script);
                var AudioFiles = Directory.GetFiles(Path.GetDirectoryName(script))
                    .Where(f => Path.GetFileNameWithoutExtension(f).StartsWith(d2vname) && (Path.GetExtension(f) == ".mp2" || Path.GetExtension(f) == ".ac3"));


                var cf = Path.GetFileNameWithoutExtension(script) + "_Chapters_FIX";

                var ChapterFile = Directory.GetFiles(Path.GetDirectoryName(script),"*.txt",SearchOption.TopDirectoryOnly)
                    .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f).StartsWith(cf) );

                if (ChapterFile==null)
                {
                     cf = Path.GetFileNameWithoutExtension(script) + "_Chapters";

                    ChapterFile = Directory.GetFiles(Path.GetDirectoryName(script), "*.txt", SearchOption.TopDirectoryOnly)
                       .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f).StartsWith(cf));
                }

                var h264 = Path.ChangeExtension(script, "264");

                AudioFile = AudioFiles.First().BeforeLast(".") + ".mp4";
                var WavAudioFile = AudioFiles.First().BeforeLast(".") + ".wav";
                var AudioDelay = AudioFile.BeforeLast(".").AfterLast(" "); if (AudioDelay == "0ms") AudioDelay = "";


                l.Add(String.Format(@"c:\programs\eac3to\eac3to.exe ""{0}"" ""{1}"" -downStereo -down32 -normalize", AudioFiles.First(), WavAudioFile) + " " + AudioDelay);
                l.Add(String.Format(@"c:\programs\MeGUI\tools\eac3to\neroAacEnc.exe -if ""{0}"" -of ""{1}"" -br 128000",WavAudioFile, AudioFile));
                l.Add("del \"" + WavAudioFile + "\"");
                
                l.Add(String.Format(@"C:\programs\MeGUI\tools\mp4box\mp4box.exe -add ""{0}#video"" -add ""{1}#audio"" {2} -new ""{3}""",
                     h264, AudioFile, ChapterFile==null ? "" : "-chap \"" + ChapterFile + "\"", script.BeforeLast(".") + "_muxed.mp4"));
            }


            File.WriteAllLines( Path.Combine(textBox2.Text,"audiorecode.cmd"), l ) ;
        }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                Other.Items.Clear();
                foreach (var d2v in Directory.GetFiles(Path.GetDirectoryName(textBox1.Text), "*.d2v"))
                {
                    Other.Items.Add(d2v);
                }
            }
        }


        //**************************************************************
        void MoveMakedDir(String path)
        {
            var maked_files = Directory.GetFiles(path, "*_muxed.mp4");

            var firstfile = true;

            foreach (var f in maked_files)
            {
                try
                {


                    var jsonfile = f.Replace("_muxed.mp4", ".json");
                    var avsfile = f.Replace("_muxed.mp4", ".avs");
                    var d2vfile = f.Replace("_muxed.mp4", ".d2v");

                    if (!File.Exists(jsonfile)) continue;

                    var data = JsonConvert.DeserializeObject<DvdData>(File.ReadAllText(jsonfile));
                    if (String.IsNullOrWhiteSpace(data.FilmName))
                    {
                        //MessageBox.Show("Нет названия фильма : "+f, "Ошибка", MessageBoxButtons.OK);
                        continue;
                    }

                    var infofile = f.Replace("_muxed.mp4", "_EncodeInfo.txt");
                    var logfile = f.Replace("_muxed.mp4", ".log");
                    var info = File.ReadAllText(infofile);

                    var hi = info.Contains("encoder type : Hi10P") ? " [Hi10P]" : "";

                  
                    if (String.IsNullOrWhiteSpace(data.FilmName))
                    {
                        MessageBox.Show("Нет имени фильма : " + f, "Ошибка", MessageBoxButtons.OK);
                        continue;
                    }

                    if (firstfile)
                    {
                        firstfile = false;
                        if (Directory.Exists(root.Text + @":\[WorkShop]\Ready\" + data.FilmName))
                        {
                            if (DialogResult.No == MessageBox.Show("Директория существует : " + data.FilmName, "Ошибка", MessageBoxButtons.YesNo))

                                return;
                        }


                        Directory.CreateDirectory(root.Text + @":\[WorkShop]\Ready\" + data.FilmName);

                        try
                        {
                            var dir = Path.GetDirectoryName(f) + @"\shots";
                            if (Directory.Exists(dir)) Directory.Move(dir, root.Text + @":\[WorkShop]\Ready\" + data.FilmName + @"\shots");

                            dir = Path.GetDirectoryName(f) + @"\info";
                            if (Directory.Exists(dir)) Directory.Move(dir, root.Text + @":\[WorkShop]\Ready\" + data.FilmName + @"\info");
                        }
                        catch
                        {
                            MessageBox.Show("Cant copy shot files");
                        }
                    }
                   

                  //  Directory.CreateDirectory(rootdir + @":\[WorkShop]\Ready\" + data.FilmName);

                    var filename = root.Text + @":\[WorkShop]\Ready\" + data.FilmName + @"\" + data.FilmName +
                                   (String.IsNullOrWhiteSpace(data.PartName) ? "" : " - " + data.PartName) + hi + ".mp4";

                    if (File.Exists(filename))
                    {
                        MessageBox.Show("Файл уже существует : " + f + "->" + filename, "Ошибка", MessageBoxButtons.OK);
                        break;
                    }

                    File.Move(f, filename);

                    File.AppendAllText(root.Text + @":\[WorkShop]\Ready\" + data.FilmName + @"\EncodeInfo.txt", f + "->" + filename + "\r\n\r\n" + info + "\r\n\r\n");

                    File.Delete(f.Replace("_muxed.mp4", ".264"));
                    File.Move(avsfile, avsfile+".Complete");
                    File.Move(d2vfile, d2vfile + ".Complete");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK);
                    break;
                }
            }
        }


        private void button19_Click(object sender, EventArgs e)
        {

            button15_Click(null, null); // make info file


            var dirs = Directory.GetDirectories(textBox2.Text, "*.*", SearchOption.TopDirectoryOnly);

            foreach (var dir in dirs)
            {
                var makes = Directory.GetFiles(dir, "VTS_??_1_muxed.mp4");

                bool shots = false;
                try
                {
                    shots = Directory.EnumerateFiles(Path.Combine(dir, "Shots"), "*.jpg").FirstOrDefault() != null;
                }
                catch { }
                  

                var missingcnt =
                    Directory.GetFiles(dir, "VTS_??_1.avs").Count(f => !makes.Contains(f.Replace(".avs", "_muxed.mp4")));



                int missingjsoncnt = 0;
                foreach (var f in makes)
                {
                    var jsonfile = f.Replace("_muxed.mp4", ".json");

                    if (!File.Exists(jsonfile))
                    {

                        missingjsoncnt++;
                            continue;
                    }

                    var data = JsonConvert.DeserializeObject<DvdData>(File.ReadAllText(jsonfile));
                    if (String.IsNullOrWhiteSpace(data.FilmName)) missingjsoncnt++;
                }


                listBox3.Items.Add(dir + " total:" + makes.Length+" nocoded:"+missingcnt +@" nonamed: "+ missingjsoncnt + " shots:"+shots.ToString() );

                if (missingcnt == 0 && missingjsoncnt == 0 && makes.Length>0  && shots)
                {
                    if (MessageBox.Show("Process Copy "+ dir,"Warning", MessageBoxButtons.YesNo )==DialogResult.Yes)
                        MoveMakedDir(dir);
                }

            }

            //VTS_02_1.avs
            //VTS_02_1_muxed.mp4




        }

        private void Workdir_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {

                textBox2.Text = root.Text + @":\[MAKER]\" + WorkPath.Text;

               Dirs.Items.Clear();
                foreach (var d in Directory.GetDirectories(textBox2.Text))
                {
                    Dirs.Items.Add(d+@"\");
                }
            }
        }

        private void Dirs_MouseClick(object sender, MouseEventArgs e)
        {
           textBox1.Text = (String)Dirs.SelectedItem+"\\";


            Other.DataSource = Directory.GetFiles(Path.GetDirectoryName(textBox1.Text), "*.d2v");

        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode== Keys.Enter)
            {
                button17_Click(null, null);
                e.Handled = true;
                
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            var scripts = Directory.GetFiles(root.Text + @":\[MAKER]\", "*.d2v", SearchOption.AllDirectories);

            foreach (var script in scripts)
            {
                if (!File.Exists(script.BeforeLast(".") + "_muxed.mp4")) listBox3.Items.Add(script);
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            var files = Directory.GetFiles(root.Text + @":\[WorkShop]\", "EncodeInfo.txt", SearchOption.AllDirectories);

            foreach (var f in files)
            {
                var text = File.ReadAllLines(f);
                foreach (var line in text)
                {
                    bool flist = false;
                    if (line == "source:") flist = true;
                     else
                      if (String.IsNullOrWhiteSpace(line)) flist = false;
                      else
                      {
                        if (File.Exists(line))
                              File.Delete(line);
                      } 


                }
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            var files = Directory.GetFiles(root.Text + @":\[WorkShop]\", "EncodeInfo.txt", SearchOption.AllDirectories);

            var list = new List<String>();
            var vobs = new List<String>();
            foreach (var fn in files)
            {
                list.AddRange(File.ReadAllLines(fn).Where(f => f.Contains("->")).Select(f => f.BeforeFirst("->")));
            }

            foreach (var muxed in list)
            {
                var d2v = muxed.Replace("_muxed.mp4", ".d2v");
                var avs = muxed.Replace("_muxed.mp4", ".avs");

                if (!File.Exists(d2v))
                {
                    listBox3.Items.Add(d2v + " -");
                    continue;
                }
                
                var lines = File.ReadAllLines(d2v);
                int j = 2;
                while (lines[j] != "")
                {
                    var fn = lines[j++];
                    vobs.Add(fn);
                    if ( File.Exists(fn)) File.Delete(fn);
                }

                File.Move(d2v, d2v+".complete");
                File.Move(avs, avs + ".complete");

            }

        }

        public void FinishedAnalysis(DetectResult text, DeinterlaceFilter[] filters, bool error, string errorMessage)
        {
            
            Invoke((Action)(() =>
           {

               dvddata.DetectResult = text;
               var s = JsonConvert.SerializeObject(dvddata);
               File.WriteAllText(Path.ChangeExtension(textBox1.Text, "json"), s);

               Analyse.Text = text.Text + "\r\n\r\n";

               Analyse.Text+= text.FieldOrder + "\r\n";
               Analyse.Text+= text.Type + "\r\n\r\n";
               foreach (var f in filters)
               {
                   Analyse.Text += "\r\n\r\n" + f.Title + ":\r\n" + f.Script;
               }



               
               switch (text.Type )
               {
                   case SourceType.INTERLACED:
                       if (text.FieldOrder == FieldOrder.TFF) PrepareScript(CodeType.TFF);
                       if (text.FieldOrder == FieldOrder.BFF) PrepareScript(CodeType.BFF);
                       break;

                       case  SourceType.PROGRESSIVE:
                       PrepareScript(CodeType.Progressive);
                       break;

                       case  SourceType.FILM:
                       PrepareScript(CodeType.TIVTC);
                       break;
                       

               }
               //  PROGRESSIVE, INTERLACED, FILM

               SystemSounds.Hand.Play();

           }));
        }

        private void Other_SelectedIndexChanged(object sender, EventArgs e)
        {
            label4.Text = " - ";
            var FileMame = ((String) Other.SelectedItem).Replace(".d2v", "_muxed.mp4");

            if (!File.Exists(FileMame)) return;
            
            if (File.Exists(FileMame+".XML")) return;

            try
            {
                var param = $" --Output=XML \"{FileMame}\" --LogFile=\"{FileMame}.XML\"";
                ProcessStartInfo ps = new ProcessStartInfo(@"c:\programs\MediaInfo\MediaInfo.exe", param);
                ps.WindowStyle = ProcessWindowStyle.Hidden;
                Process p = new Process();
                p.StartInfo = ps;
                p.Start();
                p.WaitForExit();

                label4.Text = FileMame + " " + proc(FileMame + ".XML");
            }
            catch { }

        }

        private void button11_Click(object sender, EventArgs e)
        {
            var txt = String.Format(
                "LoadPlugin(\"C:\\programs\\MeGUI\\tools\\avisynth_plugin\\TIVTC.dll\")\nLoadPlugin(\"C:\\programs\\MeGUI\\tools\\dgindex\\DGDecode.dll\")\nDGDecode_mpeg2source(\"{0}\")",
                textBox1.Text);
            var sd = new SourceDetector(txt,
                textBox1.Text,false, new SourceDetectorSettings(), UpdateSourceDetectionStatus, FinishedAnalysis);

            sd.analyse();
            

            return;
            ;
            var  finded = new bool[8,24];

            var findednames = new String[8, 24];

            var musicidx = 0;
            String musicurl = null;

            for (int w=0;w<24; w++)
                for (int h = 0; h < 8; h++) findednames[h, w] = null;

            var dnames = new String[] { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс", "По будням", "По выходным", "Вт, Ср, Чт", "Пн, Вт, Ср, Чт", "Пт, Сб", "Пн, Вт, Ср, Чт, Пт, Сб, Вс", "Пн, Пт" };
            var dValues = new String[] { "1", "2", "3", "4", "5", "6", "7", "1-5", "6-7","2-4","1-4","5-6","1-7","1,5" };

            int idx = 0;
            var src = File.ReadAllText(@"c:\1\Progs.html");
            foreach (var row in src.AllTextBetween("<tr>", "</tr>"))
            {
                var cols = row.AllTextBetween("<td>", "</td>");
                if (cols.Count() != 4) continue;

                idx++;
                var name = norm(cols[1].TextBetween("\">", "</a>"));
                    var Url = norm(cols[1].TextBetween("href=\"", "\">"));

                    if (name == "Новости") continue;

                if (name == "Музыка")
                {
                    musicidx = idx;
                    musicurl = Url;

                    musicidx=0;
                    musicurl = "/programms/muzyka/";
                    continue;
                }
            
                var vname = cols[2].AllTextBetween("\">", "</a>").Select(f=>norm(f));

                var times = cols[3].AllTextBetween("<li>", "</li>");

                foreach (var t in times)
                {
                    var w = t.BeforeFirst(":").Trim();
                    String resw = null;
                    for (int i = 0; i < dnames.Length; i++)
                    {
                        if (dnames[i] == w)
                        {
                            resw = dValues[i];
                        }
                    }

                    var h = t.AfterFirst(":").Trim();
                    var sh = h.Substring(0, 2);
                    var sm = h.Substring(3, 2);

                    var eh = h.Substring(6, 2);
                    var em = h.Substring(9, 2);

                    if (sm == "00")
                    {
                        for (int ii=int.Parse(sh); ii<int.Parse(eh); ii++)
                        {
                            if (resw == "1,5")
                            {
                                finded[1, ii] = true;
                                finded[5, ii] = true;
                                findednames[1, ii] = name;
                                findednames[5, ii] = name;
                   
                            }
                            else
                                if (resw.Contains("-"))
                                    for (int jj = int.Parse(resw.Substring(0, 1)); jj <= int.Parse(resw.Substring(2, 1)); jj++)
                                    {
                                        finded[jj, ii] = true;
                                        findednames[jj, ii] = name;
                                    }

                                else
                                {
                                    finded[int.Parse(resw), ii] = true;
                                    findednames[int.Parse(resw), ii] = name;
                                }

                        }
                    }


                    Func<String> GetVname = () =>
                        {
                            return vname.Count() == 0 ? "" : ( "/"+String.Join("/", vname));
                        };

                    if (resw == "1,5")
                    {
                        var s = String.Join(",", 1, sh, eh, sm, idx.ToString(), "\"" + name.Replace(",", "") + "\"" + GetVname(), "http://www.silver.ru" + Url);
                        Script.AppendText(s + "\r\n");

                        s = String.Join(",", 5, sh, eh, sm, idx.ToString(), "\"" + name.Replace(",", "") + "\"" + GetVname(), "http://www.silver.ru" + Url);
                        Script.AppendText(s + "\r\n");
                    }
                    var s2 = String.Join(",", resw, sh, eh, sm, idx.ToString(), "\"" + name.Replace(",", "") + "\"" + GetVname(), "http://www.silver.ru" + Url);
                    Script.AppendText(s2+ "\r\n");
                }

               

              //  Script.AppendText(name + ", " + String.Join(",", vname) + " | " + String.Join(",", times) + " | " + Url + "\r\n");
            }


            bool innull = false;
            int inidx = -1;
            for (int h = 1; h < 8; h++)
            {
                for (int w = 0; w < 24; w++)
                {
                    if (!innull) //ищем входы
                    {
                        if (!finded[h, w])
                        {
                            inidx = w;
                            innull = true;
                        }
                    }
                    else
                    {
                        if (finded[h, w])
                        {
                            var s2 = String.Join(",", h.ToString(), inidx.ToString(), (w).ToString(), "0", musicidx.ToString(), "Музыка", "http://www.silver.ru" + musicurl);
                            Script.AppendText(s2 + "\r\n");
                            innull = false;
                        }
                    }

                    //if (!finded[h, w])
                    //{
                    //    var s2 = String.Join(",", h.ToString(), w.ToString(), (w + 1).ToString(), "0", musicidx.ToString(), "Музыка", "http://www.silver.ru" + musicurl);
                    //    Script.AppendText(s2 + "\r\n");
                    //}

                    // if (!finded[h, w])

                }

                 if (innull) 
                {
                    var s2 = String.Join(",", h.ToString(), inidx.ToString(), (24).ToString(), "0", musicidx.ToString(), "Музыкальный эфир", "http://www.silver.ru" + musicurl);
                    Script.AppendText(s2 + "\r\n");
                    innull = false;
                }

            }


            var r = findednames;
        }

        private void button23_Click(object sender, EventArgs e)
        {

        }

        private void button24_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(dvddata.FilmName) || true)
            {
                var _DVDFile = Path.GetDirectoryName(textBox1.Text) + "\\" + dvddata.FilmName +
                               (String.IsNullOrWhiteSpace(dvddata.PartName) ? "" : " - " + dvddata.PartName) + ".avs";

                var n =
                    string.Format(
                        "\r\n\r\nfunction getDAR(clip c, float SAR) {{ return Float(c.width) * SAR / Float(c.height) }}\r\n" +
                        "DAR = getDAR(Float({0}) / Float({1}))\r\n" +
                        "LanczosResize(width, Int(width / DAR  / 2 )*2 )\r\n", dvddata.SAR.BeforeFirst(":"),
                        dvddata.SAR.AfterFirst(":"));

                File.WriteAllText(_DVDFile, Script.Text.Replace("FFT3DFilter(", "#FFT3DFilter(").Replace("#Yadif(", "Yadif(") + n,
                    Encoding.GetEncoding(1251));


               
            }
        }

        private void Other_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //

            var dvfile = (String)Other.SelectedItem;

            var ifofile = File.ReadAllLines(dvfile)[2].Replace("_1.VOB", "_0.IFO");

               

            //var avs = Path.ChangeExtension(dvfile, ".avs");


            //if (File.Exists(avs))
            //{

            //    button9_Click(null, null);
            //    return;
            //}

            //var playfile = (File.Exists(avs))  ? avs : File.ReadAllLines(dvfile)[2];


            try
            {
                System.Diagnostics.Process.Start(ifofile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(label5.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        String NormTorent(String src)
        {
            for (int i = 1; i < 10; i++) src = src.Replace($" ({i}).", ".");
            return src;
        }
        private void button26_Click(object sender, EventArgs e)
        {
            //FilmName.Text = Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName( (String)listBox2.Items[0] ))).Trim();

            //if (FilmName.Text == "")
            //{

            //    var d = new DriveInfo(vodfiles[0]);
            //    FilmName.Text = d.VolumeLabel.Trim();
            //}

            var readyfiles = Directory.GetDirectories(root.Text + @":\[WorkShop]\Ready\");
            var files = (new String[] { @"f:\1\", @"m:\1\" }).SelectMany(f => Directory.GetFiles(f, "*.torrent")).ToArray();

            foreach (var rf in readyfiles)
            {
                try
                {
                    if (Directory.GetFiles(rf, "*.torrent").Length > 0) continue;

                    var info = File.ReadAllText(rf + @"\EncodeInfo.txt");
                    var src = info.BeforeFirst("->");
                    var dir = Path.GetFileName(Path.GetDirectoryName(src)).Trim();

                    var finded = new List<String>();
                    foreach (var f in files)
                    {
                        var s = File.ReadAllText(f);
                        if (s.Contains(dir))
                        {
                            if (finded.Count>0)
                            {
                                if (NormTorent(Path.GetFileName(f))!= NormTorent(Path.GetFileName(finded[0])))
                                  if ( s != File.ReadAllText(finded[0]) ) finded.Add(f);
                            }
                            else
                             finded.Add(f);
                        }
                    }

                    if (finded.Count == 0) listBox3.Items.Add(rf + " -> Not Found");
                    if (finded.Count == 1)
                    {
                     //   listBox3.Items.Add(rf + " -> OK");
                        File.Copy(finded[0], Path.Combine(rf, Path.GetFileName(finded[0])));
                    }
                    if (finded.Count > 1)
                        listBox3.Items.Add(rf + " -> Many Files");
                }
                catch ( Exception ex)
                {
                    listBox3.Items.Add(rf + " -"+ex.Message);
                }


            }
        }

        private void textBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }


        String GetNode(XmlNode n, String Name, String Format = "")
        {
            var btt = n[Name];
            if (btt == null) return "";

            if (String.IsNullOrEmpty(Format)) return btt.InnerText;

            return String.Format(Format, btt.InnerText);

        }

        public String proc(String fn)
        {

            StringBuilder result = new StringBuilder();

            var doc = new System.Xml.XmlDocument();
            try
            {
                var data = File.ReadAllText(fn, Encoding.UTF8);
                data.Replace("\n01", " ");
                doc.LoadXml(data);
            }
            catch
            {
                return "";
            }
            XmlNodeList e = doc.SelectNodes("/Mediainfo/File/track");




            String bitrate = "";
            foreach (XmlNode n in e)
            {


                //  listBox1.Items.Add("-" + n.Name + "/" + n.Attributes["type"].Value);
                String type = n.Attributes["type"].Value;

                if (type.Contains("General"))
                {
                    try
                    {
                        bitrate = n["Overall_bit_rate"].InnerText;

                        /*
                        result.Append(

                            n["Complete_name"].InnerText.AfterLast(@"\") + " , " +

                            GetNode(n, "Duration") + " , " +
                            GetNode(n, "File_size") + "<br>"
                            );
                        */
                    }
                    catch
                    { bitrate = ""; result.Append("DVD VIDEO<br>"); }
                }

                if (result.ToString() == "")
                    if (type.Contains("Video"))
                    {
                        var vs = String.Format("{0}x{1} {2} {3}",

                             n["Width"].InnerText.Replace(" pixels", "").Replace(" ", ""),
                             n["Height"].InnerText.Replace(" pixels", "").Replace(" ", ""),
                             //   n["Display_aspect_ratio"].InnerText,
                             GetNode(n, "Frame_rate"),
                             n["Format"].InnerText
                             //+ GetNode(n, "Codec_ID_Hint", " {0}")
                             //+ GetNode(n, "Format_profile", " {0}")
                             )
                             //+GetNode(n,"Stream_size").Replace("MiB", "mb").Replace("GiB", "gb").Replace(" ", ""))
                             // +            GetNode(n, "Bit_rate", " ~{0}") 

                             + GetNode(n, "Bits__Pixel_Frame_", ", {0}bps")


                             ;

                        result.Append(vs.Replace("MPEG-4 Visual", "DivX"));

                    }

               
                if (type.Contains("Audio"))
                {


                    String bt = GetNode(n, "Bit_rate_mode");

                    var vs = String.Format(type +
                        GetNode(n, "ID", " id{0}") +
                         ": {0}, {1}, {2} {3}",
                         n["Sampling_rate"].InnerText,
                         n["Format"].InnerText + GetNode(n, "Format_profile", " {0}"),
                         n["Channel_count"].InnerText,
                         GetNode(n, "Bit_rate", ", ~{0}" + (bt == "Variable" ? " avg" : ""))

                        )

                         ;

                    result.Append(" "+vs);

                }

                //  listBox1.Items.Add("*" + n["Format"].InnerText );

                /*  foreach (XmlNode nn in n.ChildNodes)
                  {
                      var f = n[nn.Name];

                      listBox1.Items.Add(nn.Name + "=" + f.InnerText );
                  }
                 */


            }

            return result.ToString();
        }

        private void DelCodedBtn_Click(object sender, EventArgs e)
        {


            var coded = Directory.GetFiles(textBox2.Text, "*.d2v.Complete", SearchOption.AllDirectories);
            foreach (var d2v in coded)
            {
                var l = File.ReadAllLines(d2v);
                for (int i=2;i<100;i++)
                {
                    if (l[i] == "") break;
                    listBox3.Items.Add(l[i]);
                    if (l[i].ToLower().StartsWith(root.Text + @":\dvd_remaked"))
                        if ( File.Exists(l[i]))
                         File.Delete(l[i]);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {


            label8.Text = "";


            var avsfile =  textBox1.Text.BeforeLast(".")+ "_CRF"+ ((int)crf.Value).ToString() + (pref.Text=="" ? "" : "_") + pref.Text  +"_test.avs";

            var Dest264File = avsfile.BeforeLast(".") + ".264";
            var logfile = Path.ChangeExtension(avsfile, ".log");

            if (File.Exists(logfile)) File.Delete(logfile);

            if (!Script.Text.Contains("#PLUGINS"))
            {
                MessageBox.Show("No #PLUGINS area");
                return;
            }

            //var test = Script.Text.Replace("#PLUGINS",
            //    "selectTotal1 = framecount() / 100\r\nselectTotal2 = selectTotal1 * 2\r\nselectrangeevery(selectTotal2, 50,1000)\r\n#PLUGINS");


            var test = Script.Text.Replace("#PLUGINS",
                $"selinterval=last.framecount()/{(int)parts.Value}\r\nseloffset=(selinterval-Round(last.FrameRate*2))/2\r\nselectrangeevery(selinterval, Round(last.FrameRate*2),seloffset)\r\n#PLUGINS");

            var l = new List<String>();

            l.Add(String.Format(@"c:\programs\MeGUI\tools\x264\avs4x264mod.exe --x264-binary c:\programs\MeGUI\tools\x264\x264_64.exe --preset veryslow  --colormatrix {7}  --crf {5} {6} --psnr --ssim  {0} --sar {1} --output ""{3}"" ""{2}"" --log-file ""{4}"" ",
    null, dvddata.SAR, avsfile, Dest264File, logfile,(int)crf.Value, CoderValues.Text,GetColometry()));


            l.Add(String.Format(@"C:\programs\MeGUI\tools\mp4box\mp4box.exe -add ""{0}#trackID=1"" -new ""{1}""",
     Dest264File, avsfile.BeforeLast(".") + ".mp4"

     ));

            l.Add("echo ----- >> \""+ logfile+"\"");
            l.Add(@"c:\programs\MediaInfo\MediaInfo2.exe """ + (avsfile.BeforeLast(".") + ".mp4\"") + " >> \"" + logfile+"\"");


            l.Add("echo ----- >> \"" + logfile + "\"");
            l.Add(String.Format(@"type ""{0}"" >> ""{1}""", avsfile,  logfile));

            l.Add(String.Format(@"del ""{0}""", Dest264File));
            l.Add(String.Format(@"del ""{0}""", avsfile));
           l.Add(String.Format(@"del ""{0}""", Path.ChangeExtension(avsfile, ".cmd")));

            File.WriteAllText(avsfile, test, Encoding.GetEncoding(1251));


            


            File.WriteAllLines(Path.ChangeExtension(avsfile, ".cmd"), l, Encoding.GetEncoding(866));
            System.Diagnostics.Process.Start(Path.ChangeExtension(avsfile, ".cmd")).WaitForExit();

            try
            {
                var lines = File.ReadAllLines(logfile).FirstOrDefault(f => f.StartsWith("Bits/(Pixel*Frame)")).AfterLast(":").Trim();
                File.Move(logfile, logfile.BeforeLast(".") + " [" + lines + "," + (float.Parse(lines.Replace(".", NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator)) * 2)+ "].log");
                label8.Text = lines ;

                label8.Text = lines+" / "+ (float.Parse(lines.Replace(".", NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator)) *2);
            }
            catch
            { }


            SystemSounds.Hand.Play();


        }

        private void EncodeType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Preset_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void WorkPath_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            WorkPath.Text = DateTime.Today.Year.ToString() + "_" + DateTime.Today.Month.ToString();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            SaveState();

            var testscript = Script.Text;
            MeGUI.AvsFile avs;
            try
            {
                avs = MeGUI.AvsFile.ParseScript(testscript);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in script:\r\n" + ex.Message);
                return;
            }

            var avis = new List<Tuple<String, String, MeGUI.AvsFile>>();
            
            var crop = testscript.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(f => f.StartsWith("Crop"));

            var srcstr = "";

            // avs.FPS
            if (dvddata.codetype == CodeType.TFF || dvddata.codetype == CodeType.BFF)
                srcstr = $"TDeint(mode={(avs.FPS > 30 ? 1 : 0)},order={(dvddata.codetype == CodeType.TFF ? 1 : 0)})";
            else
                srcstr = "\r\nreturn last";

            avis.Add(Tuple.Create<String, String, MeGUI.AvsFile>("[SRC]", testscript.BeforeFirst("#PLUGINS") + "\r\n" + crop + srcstr,null));


            var tests = Directory.GetFiles(Path.GetDirectoryName(textBox1.Text), "*_[VER].avs");


            avis.Add(Tuple.Create("Current",testscript, avs));

            foreach (var t in tests)

            {
                avis.Add(Tuple.Create<String, String, MeGUI.AvsFile>(Path.GetFileNameWithoutExtension(t).Substring(9).BeforeFirst("_[VER]"),File.ReadAllText(t),null));
            }

            using (var cmp = new Compares())
            {
                cmp.d2vfile = textBox1.Text;
                cmp.InitVideo(avis.ToArray());
                cmp.ShowDialog();
            }

        }

        private void button27_Click(object sender, EventArgs e)
        {
            var muxed = AvsFile.BeforeLast(".") + "_muxed.mp4";
            
          

            if (!File.Exists(muxed)) return;

            using (var cmp = new Compares())
            {
                var s = Script.Text;
                var crop = s.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(f=>f.StartsWith("Crop"));
                var i_str = "";
               

                var muxedavs = File.ReadAllText(@"data\ffvideo.avs").Replace("{muxed}", muxed);
                cmp.d2vfile = textBox1.Text;
                cmp.InitVideo(
                    s.BeforeFirst("#PLUGINS") + "\r\n"+crop + i_str + "\r\nsubtitle(\"src\")",
                    s+i_str + "\r\nsubtitle(\"cur\")",
                    muxedavs + i_str



                    );
                cmp.ShowDialog();
            }

        }

        private void button28_Click(object sender, EventArgs e)
        {
            var testscript = Script.Text.Replace("#PLUGINS",
    "selectTotal1 = framecount() / 100\r\nselectTotal2 = selectTotal1 * 2\r\nselectrangeevery(selectTotal2, 50,1000)\r\n#PLUGINS");

            var i_str = "";



            var tests = Directory.GetFiles(Path.GetDirectoryName(textBox1.Text), "*_test.mp4");

            var avis = new List<String>();
            avis.Add(testscript + i_str+"\r\nsubtitle(\"src\")");

            foreach (var t in tests)
            
            {
                avis.Add(File.ReadAllText(@"data\ffvideo.avs").Replace("{muxed}", t).Replace("{testname}", t.AfterLast("CRF").BeforeFirst("_test")) + i_str);
            }

            using (var cmp = new Compares())
            {
                cmp.d2vfile = textBox1.Text;
                cmp.InitVideo(avis.ToArray());
                cmp.ShowDialog();
            }
        }

        private void button29_Click(object sender, EventArgs e)
        {
            var testscript = Script.Text;

            var avsfile = textBox1.Text.BeforeLast(".")  + (pref.Text == "" ? "" : "_") + pref.Text + "_[VER].avs";
            File.WriteAllText(avsfile, testscript, Encoding.GetEncoding(1251));

           
        }

        private void FakeCheckBtn_Click(object sender, EventArgs e)
        {

            //  SeparateFields().SelectEvery(2,1,2).WEAVE()
            // SeparateFields().SelectEvery(2, 0, 3).WEAVE()
            // "\r\nSeparateFields()\r\nTrim(1, 0)\r\nWEAVE()"

            using (var cmp = new Compares())
            {
                cmp.d2vfile = textBox1.Text;
                cmp.InitVideo(Script.Text.BeforeFirst("#PLUGINS") + "SeparateFields().SelectEvery(2, 1, 2).WEAVE()",
                    Script.Text.BeforeFirst("#PLUGINS")+ "\r\nSubtitle(\"orig\")"
                    );
                cmp.ShowDialog();
            }
        }

        private void label11_Click(object sender, EventArgs e)
        {
            var lines = Script.Text.AfterLast("#PLUGINS").Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Where(f=>!f.StartsWith("#"))
                .Select(f => f.BeforeFirst("(")).Where(f => !String.IsNullOrEmpty(f));

            pref.Text = String.Join("_", lines);

        }

        private void button5_Click_1(object sender, EventArgs e)
        {



            // var l = GetLang(dvddata.AudioFiles.First() );

            using (var c = new Covers())
            {
                c.Path = Path.GetDirectoryName(textBox1.Text);
                c.GetFiles();
                c.ShowDialog();
            }


        }

        private void GetAudioLang()
        { 
            try
            {
                var fn = ((String)listBox2.Items[0]).Replace("_1.VOB", "_0.IFO");

                var param = $" --Output=XML \"{fn}\" --LogFile=\"{textBox1.Text}.audio.XML\"";
                ProcessStartInfo ps = new ProcessStartInfo(@"c:\programs\MediaInfo\MediaInfo.exe", param);
                ps.WindowStyle = ProcessWindowStyle.Hidden;
                Process p = new Process();
                p.StartInfo = ps;
                p.Start();
                p.WaitForExit();

               
           


      

            XmlDocument doc = new XmlDocument();
            doc.Load($"{textBox1.Text}.audio.XML");

                dvddata.AudioLang = new  Dictionary<string, string>();

                foreach (XmlElement f in doc["Mediainfo"]["File"])
            {

                    if (f.Attributes["type"].Value == "Audio")


                        dvddata.AudioLang.Add(f["ID"].InnerText.TextBetween("0x", ")"),
                            f["Language"].InnerText
                            );

       
            }

            }
            catch { }

            StoreJson();

        }

        private void button25_Click(object sender, EventArgs e)
        {
            textBox2.Text = root.Text + @":\[MAKER]\" + WorkPath.Text;

            Dirs.DataSource = Directory.GetDirectories(textBox2.Text);

           
        }

        private void button30_Click(object sender, EventArgs e)
        {
            
            var src = GetDVDpathes(@"g:\DVD_IMAGES\").Select(f => new dvdsrc(f)).ToList();

            var remaked = GetDVDpathes(@"g:\DVD_IMAGES_Remaked\").Select(f=>new dvdsrc(f)).ToList();


            var missed = (from s in src where remaked.Any(f=>f.Name==s.Name) select s).ToList();

            Dirs.DataSource = remaked;
                //(from s in src where remaked.Select(f => Path.GetFileName(f)).Contains(Path.GetFileName(s)) select s).ToList();
        }

        IEnumerable<String> GetDVDpathes(  String rootpath)
        {
            return from dir in Directory.EnumerateDirectories(rootpath, "*.*", SearchOption.AllDirectories)
                       where Path.GetFileName(dir) != "VIDEO_TS"
                       where Directory.Exists(dir + "\\VIDEO_TS") || Directory.EnumerateFiles(dir, "*.iso").Any() || File.Exists(dir + "\\VIDEO_TS.IFO")
                       select dir;
        }

        public class dvdsrc
        {
            public String Dir { get; set; }
            public String SrcDir { get; set; }
            public IEnumerable<String> Parts { get; set; }
            public String Name { get { return Path.GetFileName(Dir); }  }
            public bool IsRemaked { get { return SrcDir != null; } }

            public dvdsrc(String dir)
            {
                Dir = dir;
                var vobfilespath = Directory.Exists(Dir + "\\VIDEO_TS") ? Dir + "\\VIDEO_TS" : dir;
                Parts = Directory.GetFiles(vobfilespath, "VTS_*.IFO").Select(f => Path.GetFileNameWithoutExtension(f));
            }

            public override string ToString()
            {
                return Name;
            }

        }

        private void button31_Click(object sender, EventArgs e)
        {

            var key = dvddata.codetype == CodeType.BFF || dvddata.codetype == CodeType.TFF ? "<Interlace>" : "<Plane>";
            var d = File.ReadAllLines("DATA\\" + filters.SelectedValue + ".filter")
                .Where(f => !f.Trim().StartsWith("<") || f.Trim().StartsWith(key)).Select(f => f.Replace(key, ""));

            Script.Text = Script.Text.BeforeLast("#PLUGINS") + "#PLUGINS\r\n" + String.Join("\r\n",d);

            pref.Text = (String)filters.SelectedValue;
        }

        private void button32_Click(object sender, EventArgs e)
        {

            int start = Script.SelectionStart;

            var t = Script.Text;
            var i = Script.GetFirstCharIndexOfCurrentLine();

            if (t[i]=='#') Script.Text = t.Substring(0, i)  + t.Substring(i+1);
            else
            Script.Text = t.Substring(0, i) + "#" + t.Substring(i );

            Script.SelectionStart = start;
            Script.SelectionLength = 0;

            ActiveControl = Script;
        }

        private void button14_Click_1(object sender, EventArgs e)
        {
            SaveState();

            var testscript = Script.Text;
            MeGUI.AvsFile avs;
            try
            {
                avs = MeGUI.AvsFile.ParseScript(testscript);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in script:\r\n" + ex.Message);
                return;
            }

            var avis = new List<Tuple<String, String, MeGUI.AvsFile>>();



          
            var i = Script.GetFirstCharIndexOfCurrentLine();
            var end = 0;
            for (int j = i ; j < testscript.Length; j++)
            {
                if (testscript[j] == '\r')
                {
                    end = j;
                    break;
                }
            }
            var p = testscript.Substring(i, end - i);
            avis.Add(Tuple.Create<String, String, MeGUI.AvsFile>("Without ["+ testscript.Substring(i,end-i) + "]", testscript.Substring(0, i) + "#" + testscript.Substring(i), null));
            avis.Add(Tuple.Create("Current", testscript, avs));

            avis.Add(Tuple.Create<String, String, MeGUI.AvsFile>("Mask",
                testscript.Substring(0, i) + "\r\nd="+p+"\r\nsubtract(last,d)"
                , null));


            using (var cmp = new Compares())
            {
                cmp.d2vfile = textBox1.Text;
                cmp.InitVideo(avis.ToArray());
                cmp.ShowDialog();
            }
        }
    }
}
