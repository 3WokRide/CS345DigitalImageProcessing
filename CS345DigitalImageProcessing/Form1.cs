using System.Windows.Forms;

namespace CS345DigitalImageProcessing
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void basicCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(openFileDialog1.FileName);
                pictureBox2.Image = null;
            }
        }

        private void basicCopyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Bitmap original = new Bitmap(pictureBox1.Image);
            Bitmap output = new Bitmap(original.Width, original.Height);

            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    Color pixelColor = original.GetPixel(x, y);
                    output.SetPixel(x, y, pixelColor);
                }
            }

            pictureBox2.Image = output;
        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image == null)
            {
                MessageBox.Show("No image loaded to save.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SaveFileDialog saveFileDialog = new();

            saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
            saveFileDialog.Title = "Save an Image File";
            saveFileDialog.FileName = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                pictureBox2.Image.Save(saveFileDialog.FileName);
            }
        }

        private void greyscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap original = new Bitmap(pictureBox1.Image);
            Bitmap output = new Bitmap(original.Width, original.Height);

            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    Color pixelColor = original.GetPixel(x, y);
                    int greyColor = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    output.SetPixel(x, y, Color.FromArgb(greyColor, greyColor, greyColor));
                }
            }

            pictureBox2.Image = output;
        }

        private void colorInversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap original = new Bitmap(pictureBox1.Image);
            Bitmap output = new Bitmap(original.Width, original.Height);

            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    Color pixelColor = original.GetPixel(x, y);
                    output.SetPixel(x, y, Color.FromArgb(255 - pixelColor.R, 255 - pixelColor.G, 255 - pixelColor.B));
                }
            }

            pictureBox2.Image = output;
        }

        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap original = new Bitmap(pictureBox1.Image);
            Bitmap output = new Bitmap(original.Width, original.Height);

            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    Color pixelColor = original.GetPixel(x, y);
                    int red = Math.Clamp((int)(0.393 * pixelColor.R + 0.769 * pixelColor.G + 0.189 * pixelColor.B), 0, 255);
                    int green = Math.Clamp((int)(0.349 * pixelColor.R + 0.686 * pixelColor.G + 0.168 * pixelColor.B), 0, 255);
                    int blue = Math.Clamp((int)(0.272 * pixelColor.R + 0.534 * pixelColor.G + 0.131 * pixelColor.B), 0, 255);
                    output.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }

            pictureBox2.Image = output;
        }

        private void histogramToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int[] histogram = new int[256];
            Bitmap original = new Bitmap(pictureBox1.Image);

            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    Color pixelColor = original.GetPixel(x, y);
                    int greyColor = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    histogram[greyColor]++;
                }
            }

            // Draw to screen
            int width = 256;
            int height = 200;

            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);

                int max = histogram.Max();

                for (int i = 0; i < 256; i++)
                {
                    int barHeight = (int)((histogram[i] / (float)max) * height);

                    g.DrawLine(Pens.Black,
                        i, height - 1,
                        i, height - barHeight);
                }
            }

            pictureBox2.Image = bmp;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
