using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace CoverToFront
{
    public partial class Covers : Form
    {
        public Covers()
        {
            InitializeComponent();
        }

        Bitmap src;
        String SrcFn;
        int width;
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        void MakePic()
        {
            System.Drawing.Imaging.PixelFormat format = src.PixelFormat;

            Rectangle cloneRect;

            if (checkBox1.Checked)  cloneRect = new Rectangle(0, 0, width, src.Height);
            else
             cloneRect = new Rectangle(src.Width - width, 0, width, src.Height);

            Bitmap bmpCrop = src.Clone(cloneRect, format);

            if (pictureBox1.Image!=null) pictureBox1.Image.Dispose();

            pictureBox1.Image = bmpCrop;


        }

        private void button1_Click(object sender, EventArgs e)
        {
            width--;
            MakePic();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            width++;
            MakePic();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            width-=10;
            MakePic();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            width+= 10;
            MakePic();
        }



        public String Path;
        public void GetFiles()
        {
            listBox1.Items.Clear();
            try
            {
                foreach (var f in Directory.GetFiles(Path, "*.jpg", SearchOption.TopDirectoryOnly ))
                {
                   
                        listBox1.Items.Add(f);
                }
            }
            catch
            {
            }
        }

        private void listBox1_Click(object sender, EventArgs e)
        {
            SrcFn = listBox1.SelectedItem.ToString();
            src = new Bitmap(SrcFn);
            width = src.Width / 2;
            MakePic();
        }

          void SaveJpeg(Bitmap img, String Filename, int quality, int height=800)
        {



            if (img.Height > height)
            {

                double viewh, vieww;

                viewh = height;
                vieww = img.Width * viewh / img.Height;


                var thumbnailImage = new Bitmap((int)(vieww), (int)viewh);
                using (Graphics g = Graphics.FromImage((Image)thumbnailImage))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(img, 0, 0, (int)(vieww), (int)viewh);
                }

                img = thumbnailImage;
            }
           



            ImageCodecInfo ici = null;
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageEncoders())
            {
                if (codec.MimeType == "image/jpeg") ici = codec;
                break;
            }

            ici = ImageCodecInfo.GetImageEncoders().First(a => a.MimeType == "image/jpeg");


            EncoderParameters ep = new EncoderParameters();
            ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)quality);

            // thumbnailImage.Save("debug.bmp");
            img.Save(Filename, ici, ep);
        }


        private void button6_Click(object sender, EventArgs e)
        {
            SaveJpeg((Bitmap)pictureBox1.Image, System.IO.Path.Combine(Path,"MP4BOX-Cover.jpg")  , 95);

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            MakePic();
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
         {
            var b = new Bitmap(listBox1.SelectedItem.ToString());
            SaveJpeg(b, System.IO.Path.Combine(Path, "MP4BOX-Cover.jpg"), 95);
        }
    }
}
