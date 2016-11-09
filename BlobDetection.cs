using AForge;
using AForge.Imaging;
using AForge.Imaging.ColorReduction;
using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overwatch
{
    public class BlobDetection
    {
        static ColorImageQuantizer ciq = new ColorImageQuantizer(new MedianCutQuantizer());
        static Color[] colorTable = null;
        static Mean meanFilter = new Mean();
        public static Bitmap ProcessBlob(Bitmap img)
        {
            BlobCounterBase bc = new BlobCounter();
            // set filtering options
            bc.FilterBlobs = true;
            bc.MinWidth = 2;
            bc.MinHeight = 2;
            // set ordering options
            bc.ObjectsOrder = ObjectsOrder.Size;
            // process binary image
            bc.ProcessImage(Grayscale.CommonAlgorithms.BT709.Apply(img));
            Blob[] blobs = bc.GetObjectsInformation();
            // extract the biggest blob
            if (blobs.Length > 0)
            {
                bc.ExtractBlobsImage(img, blobs[0], true);
                // create filter
                BlobsFiltering filter = new BlobsFiltering();
                // apply the filter
                Bitmap biggestBlobsImage = filter.Apply(img);
                return biggestBlobsImage;
            }
            return img;
        }

        public static void FilterToRed(Bitmap img)
        {
            // create filter
            //EuclideanColorFiltering filter = new EuclideanColorFiltering();
            //// set center colol and radius
            //filter.CenterColor = new RGB(180, 30, 30);
            //filter.Radius = 30;
            //// apply the filter
            //filter.ApplyInPlace(img);

            //HSLFiltering filter = new HSLFiltering();
            //filter.Hue = new IntRange(0, 6); //these settings should works, if not
            //filter.Saturation = new Range(0.6f, 1f);
            //filter.Luminance = new Range(0.3f, 0.60f);
            ////filter

            ColorFiltering colorFilter = new ColorFiltering();
            colorFilter.Red = new IntRange(140, 210);
            colorFilter.Green = new IntRange(0, 80);
            colorFilter.Blue = new IntRange(0, 80);
            colorFilter.ApplyInPlace(img);        

            //BlobsFiltering filter = new BlobsFiltering();
            //filter.CoupledSizeFiltering = true;
            //filter.MinWidth = 3;
            //filter.MinHeight = 3;
            //// apply the filter
            //filter.ApplyInPlace(img);
        }

        public static void DetectBigBlobs(Bitmap bitmap)
        {
            //bitmap = Grayscale.CommonAlgorithms.BT709.Apply(bitmap);
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.ProcessImage(Grayscale.CommonAlgorithms.BT709.Apply(bitmap));
            Rectangle[] rects = blobCounter.GetObjectsRectangles();
            //Graphics object to draw
            Pen pen = new Pen(Color.Red, 2);
            Graphics g = Graphics.FromImage(bitmap);

            foreach (Rectangle rect in rects)
            {
                if (rect.Width > 200 && rect.Height > 150)
                {
                    g.DrawRectangle(pen, rect);
                }
            }

            pen.Dispose();
            g.Dispose();
        }

        public static AForge.Vision.Motion.MotionDetector GetDefaultMotionDetector()
        {
            AForge.Vision.Motion.IMotionDetector detector = null;
            AForge.Vision.Motion.IMotionProcessing processor = null;
            AForge.Vision.Motion.MotionDetector motionDetector = null;

            //detector = new AForge.Vision.Motion.TwoFramesDifferenceDetector()
            //{
               
            //    DifferenceThreshold = 1,
            //    SuppressNoise = true
            //};

            //detector = new AForge.Vision.Motion.CustomFrameDifferenceDetector()
            //{
            //    DifferenceThreshold = 255,
            //    KeepObjectsEdges = true,
            //    SuppressNoise = true
            //};

            detector = new AForge.Vision.Motion.SimpleBackgroundModelingDetector()
            {
                DifferenceThreshold = 5,
                FramesPerBackgroundUpdate = 1,
                KeepObjectsEdges = false,
                MillisecondsPerBackgroundUpdate = 1,
                SuppressNoise = false
            };

            //processor = new AForge.Vision.Motion.GridMotionAreaProcessing()
            //{
            //    HighlightColor = System.Drawing.Color.Red,
            //    HighlightMotionGrid = true,
            //    GridWidth = 1,
            //    GridHeight = 1,
            //    MotionAmountToHighlight = 10f
            //};

            processor = new AForge.Vision.Motion.BlobCountingObjectsProcessing()
            {
                HighlightColor = System.Drawing.Color.Red,
                HighlightMotionRegions = true,
                MinObjectsHeight = 3,
                MinObjectsWidth = 3,

            };

            motionDetector = new AForge.Vision.Motion.MotionDetector(detector, processor);

            return (motionDetector);
        }
    }
}
