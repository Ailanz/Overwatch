using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Vision.Motion;
using Gma.System.MouseKeyHook;
using MouseKeyboardActivityMonitor;
using MouseKeyboardActivityMonitor.WinApi;
using ScreenShotDemo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VectorCluster;
using VectorCluster.DBScanCluster;
using WindowsInput;
using WindowsInput.Native;

namespace Overwatch
{
    public partial class Form1 : Form
    {
        ScreenCapture sc = new ScreenCapture();
        UnmanagedImage img;
        Stopwatch sw = new Stopwatch();
        //ScreenDraw sd = new ScreenDraw();
        bool isOn = false;
        public static UnmanagedImage imageToDraw = null;

        CannyEdgeDetector filter = new CannyEdgeDetector();
        MotionDetector detector = BlobDetection.GetDefaultMotionDetector();

        private GlobalHooker gh = new GlobalHooker();
        private KeyboardHookListener m_keyboardHookListener;
        private InputSimulator sim = new InputSimulator();

        Thread worker;

        public Form1()
        {
            InitializeComponent();
            m_keyboardHookListener = new KeyboardHookListener(gh);
            m_keyboardHookListener.Enabled = true;
            m_keyboardHookListener.KeyDown += HookManager_KeyDown;
            m_keyboardHookListener.KeyUp += HookManager_KeyUp;
            Bitmap screenSize = (Bitmap)sc.CaptureWholeScreen();
            this.Location = new Point(2 * screenSize.Width, 2 * screenSize.Height);

            worker = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        //dispose();
                        update();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.StackTrace);
                    }
                  
                }
            });

            worker.IsBackground = true;
            worker.Start();
        }

        private void HookManager_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D4)
            {
                isOn = false;
            }
        }

        private void HookManager_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D4)
            {
                isOn = true;
            }
        }

        private void dispose()
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
            }

            if (img != null)
            {
                img.Dispose();
            }
        }

        private void update()
        {
            // capture entire screen, and save it to a file
            sw.Reset();

            sw.Start();
            Bitmap screenshot = (Bitmap)sc.CaptureScreen();
            //Bitmap screenshot = (Bitmap)sc.CaptureScreenWithGraphics();

            Console.WriteLine("1--: " + sw.ElapsedMilliseconds);

            BitmapData imageData = screenshot.LockBits(
                new Rectangle(0, 0, screenshot.Width, screenshot.Height),
                ImageLockMode.ReadWrite, screenshot.PixelFormat);

            img = new UnmanagedImage(imageData);
            screenshot.UnlockBits(imageData);

            CustomFilters.YCBFilter(img);
            img = Grayscale.CommonAlgorithms.BT709.Apply(img);
            CustomFilters.DifferenceEdgeFilter(img);
            Console.WriteLine("2: " + sw.ElapsedMilliseconds);

            // BlobDetection.FilterToRed(img);
            //detector.ProcessFrame(img);
            //CustomFilters.SubtractFilter(img, new Bitmap(previousImage, new Size(pictureBox1.Width, pictureBox1.Height)));
            //detector.ProcessFrame(img);
            //if (isOn)
            {
                //imageToDraw = FindTargets(img);
                //pictureBox1.Image = img.ToManagedImage();
                pictureBox1.Image = FindTargets(img).ToManagedImage();
            }
            //pictureBox1.Image = filterColor(img, previousImage);
            Console.WriteLine("3: " + sw.ElapsedMilliseconds);
            screenshot.Dispose();
            sw.Stop();
        }

        private UnmanagedImage FindTargets(UnmanagedImage image)
        {


            AForge.Point centerPoint = new AForge.Point(image.Width / 2, image.Height / 2);

            List<AForge.Point> targets = new List<AForge.Point>();

            unsafe
            {
                for (int y = 0; y < image.Height; y++)
                {

                    for (int x = 0; x < image.Width; x++)
                    {
                        Color pixel = image.GetPixel(x, y);

                        //if (Math.Abs(x - centerPoint.X) < 80 && Math.Abs(y - centerPoint.Y) < 30)
                        {
                            bool isTarget = (pixel.R != 0 || pixel.G != 0 || pixel.B != 0);
                            if (isTarget)
                            {
                                targets.Add(new AForge.Point(x, y));
                            }
                            else
                            {
                                image.SetPixel(x, y, Color.White);
                            }
                            //row[x * PixelSize + 0] = 255;
                            //row[x * PixelSize + 1] = 255;
                            //row[x * PixelSize + 2] = 255;
                        }

                    }
                }
            }

            if (targets.Count > 0)
            {
                int xBox = 3;
                int yBox = 3;
                AForge.Point medianOfMedian = new AForge.Point();

                //LOL
                List<AForge.Point> medians = TargetDetector.GetMedianOfTargets(targets);
                foreach (AForge.Point p in medians)
                {
                    //Calculate Hit Box
                    medianOfMedian.X += p.X;
                    medianOfMedian.Y += p.Y;

                    for (int x = Math.Max((int)p.X - xBox, 0); x < Math.Min((int)p.X + xBox, image.Width); x++)
                    {
                        for (int y = Math.Max((int)p.Y - yBox, 0); y < Math.Min((int)p.Y + yBox, image.Height); y++)
                        {
                            image.SetPixel(x, y, Color.Plum);
                        }
                    }

                    for (int x = (int)centerPoint.X - xBox; x < (int)centerPoint.X + xBox; x++)
                    {
                        for (int y = (int)centerPoint.Y - yBox; y < (int)centerPoint.Y + yBox; y++)
                        {
                            //image.SetPixel(x, y, Color.GreenYellow);
                        }
                    }
                }

                medianOfMedian.X = medianOfMedian.X / medians.Count;
                medianOfMedian.Y = medianOfMedian.Y / medians.Count;

                for (int x = (int)medianOfMedian.X - xBox; x < (int)medianOfMedian.X + xBox; x++)
                {
                    for (int y = (int)medianOfMedian.Y - yBox; y < (int)medianOfMedian.Y + yBox; y++)
                    {
                        image.SetPixel(x, y, Color.Red);
                    }
                }



                int moveX = (int)((medianOfMedian.X - centerPoint.X) * 1);
                int moveY = (int)((medianOfMedian.Y - centerPoint.Y) * 1); 
                //sim.Mouse.MoveMouseBy((int)((medianX - centerX) * ratioX), (int)((medianY - centerY) * ratioY));

                if (isOn)
                {
                    Console.WriteLine("Targets: " + medians.Count);
                    //double ratioWidth = screenShot.HorizontalResolution / img.HorizontalResolution;
                    //double ratioHeight = screenShot.VerticalResolution / img.VerticalResolution;
                    Mouse.FreezeMouse();
                    AimHard(moveX/2, moveY/2);
                    Mouse.ThawMouse();
                    //Console.WriteLine("Move: " + moveX + ", " + moveY);
                    //AimSmooth(moveX, moveY);
                    //Console.WriteLine("Targets: " + medians.Count);

                }


                return image;
            }

            return image;
        }

        private void AimSmooth(int moveX, int moveY)
        {
            Mouse.SmoothMoveX(sim, moveX, 1);
            Mouse.SmoothMoveY(sim, moveY, 1);
        }

        private void AimHard(int moveX, int moveY)
        {
            Mouse.HardMoveX(sim, moveX, 1);
            Mouse.HardMoveY(sim, moveY, 1);
        }

        private bool withinDistance(byte r, byte g, byte b, byte pr, byte pg, byte pb)
        {
            double magnitude = Math.Sqrt(Math.Pow(r - pr, 2) + Math.Pow(g - pg, 2) + Math.Pow(b - pb, 2));
            return magnitude < 30;
        }
    }
}
