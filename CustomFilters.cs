using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overwatch
{
    public class CustomFilters
    {

        static YCbCrFiltering filter = new YCbCrFiltering();

        public static void YCBFilter(UnmanagedImage img)
        {

                // set color ranges to keep
                filter.Cb = new Range(-0.2f, 0.0f);
                filter.Cr = new Range(0.26f, 0.5f);
                // apply the filter
                filter.ApplyInPlace(img);

        }

        public static void DifferenceEdgeFilter(UnmanagedImage img)
        {
            DifferenceEdgeDetector filter2 = new DifferenceEdgeDetector();
            filter2.ApplyInPlace(img);
        }

        public static void SubtractFilter(Bitmap img, Bitmap overlay)
        {
            //try
            //{


               // DifferenceEdgeDetector filter2 = new DifferenceEdgeDetector( );
               // filter2.ApplyInPlace(img);


            //    HSLFiltering filter = new HSLFiltering();
            //    // set color ranges to keep
            //    filter.Hue = new IntRange(335, 0);
            //    filter.Saturation = new Range(0.6f, 1);
            //    filter.Luminance = new Range(0.1f, 1);
            //    // apply the filter
            //    filter.FillOutsideRange = false;
            //    filter.ApplyInPlace(img);
            //    // create instance of blob counter
            //    BlobCounter blobCounter = new BlobCounter();
            //    // process input image
            //    blobCounter.MaxHeight = 3;
            //    blobCounter.MaxWidth = 3;
            //    blobCounter.ProcessImage(img);
            //    // get information about detected objects
            //    Blob[] blobs = blobCounter.GetObjectsInformation();

            //    Graphics g = Graphics.FromImage(img);

            //    for (int i = 0, n = blobs.Length; i < n; i++)
            //    {
            //        List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);
            //        List<IntPoint> corners = PointsCloud.FindQuadrilateralCorners(edgePoints);
            //        g.DrawPolygon(new Pen(Color.Red), ToPointsArray(corners));

            //    }

            //}
            //catch { }
        }

        private static System.Drawing.Point[] ToPointsArray(List<IntPoint> points)
        {
            System.Drawing.Point[] array = new System.Drawing.Point[points.Count];

            for (int i = 0, n = points.Count; i < n; i++)
            {
                array[i] = new System.Drawing.Point(points[i].X, points[i].Y);
            }

            return array;
        }

    }
}
