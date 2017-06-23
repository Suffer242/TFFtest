using MeGUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TFFtest
{
    public partial class Compares : Form
    {
        public Compares()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }


        AvsFile[] script;
        String[] scripttext;
        int[] loadidx;

        public void InitVideo(params String[] Script)
        {
        }

            int activeno = 1;
        public void InitVideo(params Tuple<String, String, AvsFile>[] Script )
        {

            //  InitVideo(Tuple.Create("", ""), Tuple.Create("", ""));

            
           
            script = new AvsFile[Script.Length];
            scripttext = new String[Script.Length];
            loadidx = new int[Script.Length];

            for (int i = 0; i < Script.Length; i++)
            {
                loadidx[i] = -1;

                var p = new TabPage((i+1).ToString()+"."+Script[i].Item1);

                var picture = new PictureBox();
                picture.Dock = System.Windows.Forms.DockStyle.Fill;
                picture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;

                picture.MouseDoubleClick += Picture_MouseDoubleClick;

                picture.MouseMove += track_MouseMove;
                p.Controls.Add(picture);


                Tabs.TabPages.Add(p);



                script[i] = Script[i].Item3;  //AvsFile.ParseScript(Script[i]);
                scripttext[i] = Script[i].Item2;
            }



            Tabs.SelectedIndex = 1;

         //   script[1] = AvsFile.ParseScript(Script[1].Item2);


            track.Maximum = script[1].FrameCount;
            track.LargeChange = track.Maximum / 50;

            track.TickFrequency = track.Maximum / 50;

            ShowPic();
        }

        private void Picture_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var msg = scripttext[Tabs.SelectedIndex].Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Where(f => !f.Trim().StartsWith("#"));
            MessageBox.Show(String.Join("\r\n",msg) ,"Script Text");
        }

        private void track_ValueChanged(object sender, EventArgs e)
        {
            Text = "Compares: " + script.Length + " exaples / CurFrame: " + track.Value;
            ShowPic();
        }

        void ShowPic()
        {
            //if (pictureBox1.Image != null) pictureBox1.Image.Dispose();

            //if (script[activeno] ==null) script[activeno] = AvsFile.ParseScript(scripttext[activeno]);

            //pictureBox1.Image = script[activeno].GetVideoReader().ReadFrameBitmap(track.Value);


            var a = Tabs.SelectedIndex;

            if (loadidx[a] == track.Value) return;

            PictureBox pic = (PictureBox)Tabs.SelectedTab.Controls[0];


            if (pic.Image != null) pic.Image.Dispose();
            

           
            panel2.BackColor = Color.Red;
            panel2.Refresh();
           // Application.DoEvents();


            if (script[a] == null) script[a] = AvsFile.ParseScript(scripttext[a]);

     
                   
       
                pic.Image = script[a].GetVideoReader().ReadFrameBitmap(track.Value);

            panel2.BackColor = Color.White;

            loadidx[a] = track.Value;

                label1.Text = "FPS:" + script[activeno].FPS.ToString();
                label2.Text = "Fames:" + script[activeno].FrameCount.ToString();
            
           
        }



        private void Compares_KeyDown(object sender, KeyEventArgs e)
        {
            if ((int)e.KeyCode>=49 && (int)e.KeyCode<=57)
            {
                if ((int)e.KeyCode - 49 < script.Length)
                {
                    activeno = (int)e.KeyCode - 49;

                    Tabs.SelectedIndex = activeno;
                    ShowPic();

                   
                }

            }



        }

        private void fav_DoubleClick(object sender, EventArgs e)
        {
           
        }

        private void fav_SelectedIndexChanged(object sender, EventArgs e)
        {
          
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            fav.Items.Add(track.Value);

            System.IO.File.WriteAllLines(FavFile(), fav.Items.Cast<Object>().Select(f => f.ToString()));


            //System.IO.File.WriteAllLines(FavFile(), fav.Items.Cast<int>().Select(f => f.ToString())  ) ;
        }

        public String d2vfile;
        String FavFile()
        {
            return @"fav\" + d2vfile.Replace(':', '_').Replace('\\', '_')+".dat";
        }
        private void fav_MouseClick(object sender, MouseEventArgs e)
        {
            if (fav.SelectedItem != null)
            {

                if (fav.SelectedItem is int)
                track.Value = (int)fav.SelectedItem;
                else
                 if (fav.SelectedItem is TimeSpan)
                    track.Value = (int)Math.Round(((TimeSpan)fav.SelectedItem).TotalSeconds * script[Tabs.SelectedIndex].FPS);

            }
            ActiveControl = track;
        }

        private void Compares_Shown(object sender, EventArgs e)
        {
            fav.Items.Clear();
            if ( File.Exists(FavFile()))

                //.Select( f=> int.Parse(f))

            foreach ( var l in  File.ReadAllLines(FavFile()))
            {
                    if (l.Contains(":")) fav.Items.Add(TimeSpan.Parse(l));
                    else 
                     fav.Items.Add(int.Parse(l));
            }
        }

        private void Compares_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (var a in script) if (a!=null) a.Dispose();
     
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Tabs_TabIndexChanged(object sender, EventArgs e)
        {

        }

        private void Tabs_Selected(object sender, TabControlEventArgs e)
        {
            ShowPic();
        }

        private void track_MouseMove(object sender, MouseEventArgs e)
        {
            var zoom = ((PictureBox)sender).Image.Height / (float)((PictureBox)sender).Height;
            label3.Text = e.Y*zoom + " / " + (((PictureBox)sender).Height - e.Y)*zoom;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fav.Items.Add( TimeSpan.FromSeconds(track.Value / script[Tabs.SelectedIndex].FPS));


            System.IO.File.WriteAllLines(FavFile(), fav.Items.Cast<Object>().Select(f => f.ToString()));
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            fav.Items.RemoveAt(fav.SelectedIndex);
            System.IO.File.WriteAllLines(FavFile(), fav.Items.Cast<Object>().Select(f => f.ToString()));

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var sd = Path.GetDirectoryName(d2vfile) + @"\Shots\";
            Directory.CreateDirectory(sd);

            PictureBox pic = (PictureBox)Tabs.SelectedTab.Controls[0];
            pic.Image.Save(sd+track.Value.ToString() +" "+ Tabs.SelectedTab.Text + " ["+ textBox1.Text +"].png");
        }
    }
}
