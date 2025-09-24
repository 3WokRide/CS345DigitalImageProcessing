using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Windows.Forms;

namespace CS345DigitalImageProcessing
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        // For webcam capture
        VideoCapture capture;
        Mat frame;
        Bitmap image;
        bool isRunning = false;
        CurrentFilter currentFilter = CurrentFilter.None;

        enum CurrentFilter
        {
            None,
            BasicCopy,
            Greyscale,
            ColorInversion,
            Sepia,
            Histogram,
            Subtraction
        }

        private async void StartCamera()
        {
            capture = new VideoCapture(0);
            if (!capture.IsOpened())
            {
                MessageBox.Show("Camera not found!");
                return;
            }

            isRunning = true;
            frame = new Mat();

            await Task.Run(() =>
            {
                while (isRunning)
                {
                    capture.Read(frame);
                    if (!frame.Empty())
                    {
                        // Show camera input to picturebox1
                        image = BitmapConverter.ToBitmap(frame);
                        pictureBox1.Invoke(new Action(() =>
                        {
                            pictureBox1.Image?.Dispose();
                            pictureBox1.Image = (Bitmap)image.Clone();
                        }));
                        // Show filter result to picturebox2
                        pictureBox2.Invoke(new Action(() =>
                        {
                            pictureBox2.Image?.Dispose();
                            switch (currentFilter)
                            {
                                case CurrentFilter.None:
                                    break;
                                case CurrentFilter.BasicCopy:
                                    pictureBox2.Image = (Bitmap)image.Clone();
                                    break;
                                case CurrentFilter.Greyscale:
                                    Cv2.CvtColor(frame, frame, ColorConversionCodes.BGR2GRAY);
                                    pictureBox2.Image = BitmapConverter.ToBitmap(frame);
                                    break;
                                case CurrentFilter.ColorInversion:
                                    Cv2.BitwiseNot(frame, frame);
                                    pictureBox2.Image = BitmapConverter.ToBitmap(frame);
                                    break;
                                case CurrentFilter.Sepia:
                                    float[,] sepiaArray = new float[,]
                                    {
                                        { 0.272f, 0.534f, 0.131f },
                                        { 0.349f, 0.686f, 0.168f },
                                        { 0.393f, 0.769f, 0.189f }
                                    };
                                    Mat kernel = Mat.FromArray(sepiaArray);
                                    Cv2.Transform(frame, frame, kernel);
                                    pictureBox2.Image = BitmapConverter.ToBitmap(frame);
                                    break;
                                case CurrentFilter.Histogram:
                                    Bitmap histImage = new(512, 400);
                                    Filters.ComputeGrayHistogram(frame, ref histImage);
                                    pictureBox2.Image = histImage;
                                    break;
                                case CurrentFilter.Subtraction:
                                    if (pictureBox3.Image == null)
                                    {
                                        MessageBox.Show("Please load a background image first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        currentFilter = CurrentFilter.None;
                                        break;
                                    }
                                    Mat background = BitmapConverter.ToMat((Bitmap)pictureBox3.Image);
                                    try
                                    {
                                        Mat result = Filters.SubtractGreen(ref frame, ref background);
                                        pictureBox2.Image = BitmapConverter.ToBitmap(result);
                                    } catch (ArgumentException ex)
                                    {
                                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        currentFilter = CurrentFilter.None;
                                        break;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }));

                    }
                }
            });
        }

        private void StopCamera()
        {
            currentFilter = CurrentFilter.None;
            isRunning = false;
            capture?.Release();
            capture?.Dispose();
            frame?.Dispose();
            pictureBox1.Invoke(new Action(() =>
            {
                pictureBox1.Image?.Dispose();
                pictureBox1.Image = null;
            }));
            pictureBox2.Invoke(new Action(() =>
            {
                pictureBox2.Image?.Dispose();
                pictureBox2.Image = null;
            }));
        }

        private void basicCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isRunning)
            {
                MessageBox.Show("Please stop the camera before loading an image.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(openFileDialog1.FileName);
                pictureBox2.Image = null;
            }
        }

        private void basicCopyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("No image loaded to apply filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            currentFilter = CurrentFilter.BasicCopy;
            Bitmap original = new Bitmap(pictureBox1.Image);
            Bitmap output = new Bitmap(original.Width, original.Height);

            Filters.BasicCopy(ref original, ref output);

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
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("No image loaded to apply filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            currentFilter = CurrentFilter.Greyscale;
            Bitmap original = new Bitmap(pictureBox1.Image);
            Bitmap output = new Bitmap(original.Width, original.Height);

            Filters.Greyscale(ref original, ref output);

            pictureBox2.Image = output;
        }

        private void colorInversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("No image loaded to apply filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            currentFilter = CurrentFilter.ColorInversion;
            Bitmap original = new Bitmap(pictureBox1.Image);
            Bitmap output = new Bitmap(original.Width, original.Height);

            Filters.ColorInversion(ref original, ref output);

            pictureBox2.Image = output;
        }

        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("No image loaded to apply filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            currentFilter = CurrentFilter.Sepia;
            Bitmap original = new Bitmap(pictureBox1.Image);
            Bitmap output = new Bitmap(original.Width, original.Height);

            Filters.Sepia(ref original, ref output);

            pictureBox2.Image = output;
        }

        private void histogramToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("No image loaded to apply filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            currentFilter = CurrentFilter.Histogram;
            Bitmap original = new Bitmap(pictureBox1.Image);
            Bitmap output = new Bitmap(original.Width, original.Height);

            Filters.Histogram(ref original, ref output);

            pictureBox2.Image = output;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void subtractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("No image loaded to apply filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (pictureBox3.Image == null)
            {
                MessageBox.Show("Please load a background image first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            currentFilter = CurrentFilter.Subtraction;
            Bitmap imageB, imageA, resultImage;
            imageB = new Bitmap(pictureBox1.Image);
            imageA = new Bitmap(pictureBox3.Image);
            resultImage = new Bitmap(imageB.Width, imageB.Height);
            Color mygreen = Color.FromArgb(0, 255, 0);
            int greygreen = (mygreen.R + mygreen.G + mygreen.B) / 3;
            int threshold = 5;

            for (int x = 0; x < imageB.Width; x++)
            {
                for (int y = 0; y < imageB.Height; y++)
                {
                    Color pixel = imageB.GetPixel(x, y);
                    Color backpixel = imageA.GetPixel(x, y);
                    int grey = (pixel.R + pixel.G + pixel.B) / 3;
                    int subtractvalue = Math.Abs(grey - greygreen);
                    if (subtractvalue > threshold)
                        resultImage.SetPixel(x, y, pixel);
                    else
                        resultImage.SetPixel(x, y, backpixel);
                }
            }

            pictureBox2.Image = resultImage;
        }

        private void loadBackgroundSubtractionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox3.Image = new Bitmap(openFileDialog1.FileName);
                pictureBox2.Image = null;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isRunning)
                StopCamera();
        }

        private void startCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isRunning)
                StartCamera();
            else
                MessageBox.Show("Camera is already running.");
        }

        private void stopCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isRunning)
                StopCamera();
            else
                MessageBox.Show("Camera is not running.");
        }
    }

    static class Filters
    {
        public static void BasicCopy(ref Bitmap original, ref Bitmap output)
        {
            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    Color pixelColor = original.GetPixel(x, y);
                    output.SetPixel(x, y, pixelColor);
                }
            }
        }

        public static void Greyscale(ref Bitmap original, ref Bitmap output)
        {
            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    Color pixelColor = original.GetPixel(x, y);
                    int greyColor = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    output.SetPixel(x, y, Color.FromArgb(greyColor, greyColor, greyColor));
                }
            }
        }

        public static void ColorInversion(ref Bitmap original, ref Bitmap output)
        {
            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    Color pixelColor = original.GetPixel(x, y);
                    output.SetPixel(x, y, Color.FromArgb(255 - pixelColor.R, 255 - pixelColor.G, 255 - pixelColor.B));
                }
            }
        }

        public static void Sepia(ref Bitmap original, ref Bitmap output)
        {
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
        }

        public static void Histogram(ref Bitmap original, ref Bitmap output)
        {
            int[] histogram = new int[256];
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
            output = DrawHistogram(histogram);
        }


        public static void ComputeGrayHistogram(Mat frame, ref Bitmap output)
        {
            // Convert to grayscale
            Mat gray = new Mat();
            if (frame.Channels() == 3)
                Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);
            else
                gray = frame.Clone();

            // Prepare histogram parameters
            int histSize = 256;
            Rangef[] ranges = { new(0, 256) };  // intensity range
            Mat hist = new();

            // Compute histogram
            Cv2.CalcHist(
                images: [gray],
                channels: [0],
                mask: null,
                hist: hist,
                dims: 1,
                histSize: [histSize],
                ranges: ranges
            );

            // Convert histogram Mat to int array
            int[] histValues = new int[histSize];
            for (int i = 0; i < histSize; i++)
                histValues[i] = (int)hist.Get<float>(i);

            output = DrawHistogram(histValues);
        }

        private static Bitmap DrawHistogram(int[] histogram, int width = 512, int height = 400)
        {
            Bitmap histImage = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(histImage))
            {
                g.Clear(Color.White);
                int max = histogram.Max();
                float scaleX = (float)width / histogram.Length;
                float scaleY = (float)height / max;
                for (int i = 0; i < histogram.Length; i++)
                {
                    float x = i * scaleX;
                    float y = height - (histogram[i] * scaleY);
                    g.DrawLine(Pens.Black, x, height, x, y);
                }
            }
            return histImage;
        }

        public static Mat SubtractGreen(ref Mat frame, ref Mat background, int threshold = 50)
        {
            if (frame.Size() != background.Size())
                throw new ArgumentException("Frame and background must have the same size.");
            if (frame.Channels() != 3 || background.Channels() != 3)
                throw new ArgumentException("Only 3-channel BGR images are supported.");

            Mat result = new Mat(frame.Size(), frame.Type());

            // Split channels
            Mat[] bgr = Cv2.Split(frame);
            Mat B = bgr[0];
            Mat G = bgr[1];
            Mat R = bgr[2];

            // mask = pixels where G is significantly higher than R and B
            Mat mask = new Mat();
            Mat tmp1 = new Mat(), tmp2 = new Mat();

            Cv2.Subtract(G, R, tmp1);
            Cv2.Subtract(G, B, tmp2);
            Cv2.Min(tmp1, tmp2, mask);       // take min(G-R, G-B)
            Cv2.Threshold(mask, mask, threshold, 255, ThresholdTypes.Binary);

            // Convert mask to 3 channels
            Mat mask3 = new Mat();
            Cv2.CvtColor(mask, mask3, ColorConversionCodes.GRAY2BGR);

            // Invert mask
            Mat mask3Inv = new Mat();
            Cv2.BitwiseNot(mask3, mask3Inv);

            // Apply mask: frame where mask inverted, background where mask
            frame.CopyTo(result, mask3Inv);      // keep pixels that are NOT green
            background.CopyTo(result, mask3);    // replace green with background

            return result;
        }
    }
}