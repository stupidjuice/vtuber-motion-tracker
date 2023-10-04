using System.Collections;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

public class Program
{
    public static void Main(string[] args)
    {
        TrackingBox testBox = new TrackingBox(10, 10, 40, 40);
        List<TrackingBox> boxes = new List<TrackingBox>();
        boxes.Add(testBox);

        CvInvoke.NamedWindow("test");
        using (Mat frame = new Mat())
        using (VideoCapture capture = new VideoCapture())
            while (CvInvoke.WaitKey(1) == -1)
            {
                capture.Read(frame);

                Image<Bgr, Byte> img = frame.ToImage<Bgr, Byte>();

                foreach (TrackingBox box in boxes)
                {
                    for (int x = box.topLeft.x; x < box.bottomRight.x; x++)
                    {
                        img.Data[x, box.topLeft.y, 0] = 0;
                        img.Data[x, box.bottomRight.y, 0] = 0;
                        img.Data[x, box.topLeft.y, 1] = 0;
                        img.Data[x, box.bottomRight.y, 1] = 0;
                        img.Data[x, box.topLeft.y, 2] = 255;
                        img.Data[x, box.bottomRight.y, 2] = 255;
                    }
                    for (int y = box.topLeft.y; y < box.bottomRight.y; y++)
                    {
                        img.Data[box.topLeft.x, y, 0] = 0;
                        img.Data[box.bottomRight.x, y, 0] = 0;
                        img.Data[box.topLeft.x, y, 1] = 0;
                        img.Data[box.bottomRight.x, y, 1] = 0;
                        img.Data[box.topLeft.x, y, 2] = 255;
                        img.Data[box.bottomRight.x, y, 2] = 255;
                    }

                    CvInvoke.Imshow("test", img);
                }
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

        public TrackingBox(Vector2Int tleft, Vector2Int bright)
        {
            this.topLeft = tleft;
            this.bottomRight = bright;
        }
        public TrackingBox(int minx, int miny, int maxx, int maxy)
        {
            this.topLeft = new Vector2Int(minx, miny);
            this.bottomRight = new Vector2Int(maxx, maxy);
        }
    }
}