using AForge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overwatch
{
    public class TargetDetector
    {
        static float DISTANCE = 100;
        static Queue<Point> processorQueue = new Queue<Point>();
        static HashSet<Point> processedPoints = new HashSet<Point>();
        static List<HashSet<Point>> listOfBlobs = new List<HashSet<Point>>();
        static List<Point> medians = new List<Point>();

        public static List<Point> GetMedianOfTargets(List<Point> allPoints)
        {
            //allPoints.ForEach(x => processorQueue.Enqueue(x));
            medians.Clear();
            foreach (Point p in allPoints)
            {
                if (!processedPoints.Contains(p))
                {
                    HashSet<Point> blob = new HashSet<Point>();
                    processorQueue.Enqueue(p);
                    processedPoints.Add(p);
                    while (processorQueue.Count != 0)
                    {
                        Point curPoint = processorQueue.Dequeue();
                        List<Point> closeByPoints = allPoints.FindAll(x => curPoint.SquaredDistanceTo(x) < DISTANCE);
                        closeByPoints.ForEach(x => blob.Add(x));
                        //closeByPoints.ForEach(x => AddToQueue(x));
                        closeByPoints.ForEach(x => processedPoints.Add(x));
                    }
                    if (blob.Count > 100)
                    {
                        //Console.WriteLine("BLOB: " + blob.Count);
                        listOfBlobs.Add(blob);
                    }
                }                
            }
            listOfBlobs.ForEach(x => medians.Add(FindMedian(x)));
            CleanUp();
            return medians;
        }

        private static void AddToQueue(Point x)
        {
            if (!processedPoints.Contains(x))
            {
                processorQueue.Enqueue(x);
            }
        }

        private static Point FindMedian(HashSet<Point> points)
        {
            float sumX = 0;
            float sumY = 0;
            float count = points.Count;
            foreach(Point p in points)
            {
                sumX += p.X;
                sumY += p.Y;
            }
            return new Point(sumX / count, sumY / count);
        }

        private static void CleanUp()
        {
            foreach (HashSet<Point> hs in listOfBlobs)
            {
                hs.Clear();
            }
            listOfBlobs.Clear();            
            processorQueue.Clear();
            processedPoints.Clear();
        }
    }
}
