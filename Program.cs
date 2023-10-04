using System.Collections;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

public class Program
{
    public static void Main(string[] args)
    {
        TrackingBox testBox = new TrackingBox(10, 10, 40, 40, 255, 255, 0);
        List<TrackingBox> boxes = new List<TrackingBox>();
        boxes.Add(testBox);

        CvInvoke.NamedWindow("test");
        using (Mat frame = new Mat())
        using (VideoCapture capture = new VideoCapture())
            while (CvInvoke.WaitKey(1) == -1)
            {
                capture.Read(frame);

                Image<Bgr, Byte> img = frame.ToImage<Bgr, Byte>();


                //-----DRAW BOXES-----//
                foreach (TrackingBox box in boxes)
                {
                    for (int x = box.topLeft.x; x < box.bottomRight.x; x++)
                    {
                        img.Data[x, box.topLeft.y, 0] = box.b;
                        img.Data[x, box.bottomRight.y, 0] = box.b;
                        img.Data[x, box.topLeft.y, 1] = box.g;
                        img.Data[x, box.bottomRight.y, 1] = box.g;
                        img.Data[x, box.topLeft.y, 2] = box.r;
                        img.Data[x, box.bottomRight.y, 2] = box.r;
                    }
                    for (int y = box.topLeft.y; y < box.bottomRight.y; y++)
                    {
                        img.Data[box.topLeft.x, y, 0] = box.b;
                        img.Data[box.bottomRight.x, y, 0] = box.b;
                        img.Data[box.topLeft.x, y, 1] = box.g;
                        img.Data[box.bottomRight.x, y, 1] = box.g;
                        img.Data[box.topLeft.x, y, 2] = box.r;
                        img.Data[box.bottomRight.x, y, 2] = box.r;
                    }
                }

                //-----Tracking inside boxes-----


                CvInvoke.Imshow("test", img);
            }
    }

    public struct Vector2Int
    {
        public int x;
        public int y;

        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public struct TrackingBox
    {
        public Vector2Int topLeft;
        public Vector2Int bottomRight;
        public byte r, g, b;

        public TrackingBox(Vector2Int tleft, Vector2Int bright, byte r, byte g, byte b)
        {
            this.topLeft = tleft;
            this.bottomRight = bright;
            this.r = r;
            this.g = g;
            this.b = b;
        }
        public TrackingBox(int minx, int miny, int maxx, int maxy, byte r, byte g, byte b)
        {
            this.topLeft = new Vector2Int(minx, miny);
            this.bottomRight = new Vector2Int(maxx, maxy);
            this.r = r;
            this.g = g;
            this.b = b;
        }
    }
}