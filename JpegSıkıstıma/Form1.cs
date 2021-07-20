using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using Encoder = System.Drawing.Imaging.Encoder;

namespace JpegSıkıstıma
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }

        
       
        
        public static string GetFileSize(long fileLength)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            while (fileLength >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                fileLength = fileLength / 1024;
            }
            string result = String.Format("{0:0.##} {1}", fileLength, sizes[order]);
            return result;
        }

        public static Image ResizeImage(Bitmap image, int maxWidth, int maxHeight, int quality, string filePath)
        {
            
            // Resmin orijinal genişliğini ve yüksekliğini al
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // En boy oranını koru
            float ratioX = (float)maxWidth / (float)originalWidth;
            float ratioY = (float)maxHeight / (float)originalHeight;
            float ratio = Math.Min(ratioX, ratioY);

            // En boy oranına göre yeni genişlik ve yükseklik
            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            // Diğer biçimleri (CMYK dahil) RGB'ye dönüştürün.
            Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

            // Görüntüyü, kalite modu HighQuality olarak ayarlanmış olarak belirtilen boyutta çizer
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            
             //JPEG codec bileşenini temsil eden bir ImageCodecInfo nesnesi alın.
             ImageCodecInfo imageCodecInfo = GetEncoderInfo(ImageFormat.Jpeg);

            // Kalite parametresi için bir Kodlayıcı nesnesi oluşturun.
            Encoder encoder = System.Drawing.Imaging.Encoder.Quality;

            // Bir EncoderParameters nesnesi oluşturun. 
            EncoderParameters encoderParameters = new EncoderParameters(1);

            // Görüntüyü kalite düzeyine sahip bir JPEG dosyası olarak kaydedin.
            EncoderParameter encoderParameter = new EncoderParameter(encoder, quality);
            encoderParameters.Param[0] = encoderParameter;
            newImage.Save(filePath, imageCodecInfo, encoderParameters);
            

            return Image.FromFile(filePath);
        }

       
        private static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName.ToString()!="")
            {
                pictureBox1.Load(openFileDialog1.FileName);
                label3.Text = GetFileSize(new FileInfo(openFileDialog1.FileName).Length);
            }
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

            int boyut1 = Convert.ToInt32(textBox1.Text);
            pictureBox2.Image = ResizeImage(new Bitmap(pictureBox1.Image), pictureBox1.Image.Width, pictureBox1.Image.Height, boyut1, "deneme.jpg");
            label4.Text = GetFileSize(new FileInfo("deneme.jpg").Length);
        }

        public void button3_Click(object sender, EventArgs e)
        {
          
            saveFileDialog1.FileName = "";
            saveFileDialog1.ShowDialog();
            try
            {
            if (saveFileDialog1.FileName.ToString()!="")
                        {
                            pictureBox2.Image.Save(saveFileDialog1.FileName);
                        }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Fotoğraf Seçmediniz..  ");
            }
            
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void btnTemizle_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = "deneme.jpg";
            pictureBox1.Image = null;
            pictureBox2.Image = null;
            label3.Text = null;
            label4.Text = null;
            textBox1.Text = null;
        }
    }
}
