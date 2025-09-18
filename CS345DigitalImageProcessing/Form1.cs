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
    }
}
