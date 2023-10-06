using System.Collections;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

public class Fail
{
   public static void Main(string[] args)
    {
        //----PARAMETERS----
        double similarityForegivenss = 8; //Color distance is divided by this
        double distanceWeight = 0.8; //Color distance is raised to this power, making smaller distances more noticeable. i have no idea if this actually works
        double cutoff = 500;

        TrackingBox testBox = new TrackingBox(300, 190, 310, 200, 255, 255, 0);
        List<TrackingBox> boxes = new List<TrackingBox>();
        boxes.Add(testBox);
        Byte targetR = 50, targetG = 86, targetB = 168;

        CvInvoke.NamedWindow("test");
        using (Mat frame = new Mat())
        using (VideoCapture capture = new VideoCapture())
            while (CvInvoke.WaitKey(1) == -1)
            {
                capture.Read(frame);

                Image<Bgr, Byte> img = frame.ToImage<Bgr, Byte>();

                foreach (TrackingBox box in boxes)
                {
                    //-----DRAW BOXES-----//
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

                    double boxTotalColorDistance = 0;
                    double estimatedOriginX = 0, estimatedOriginY = 0;
                    //tracking
                    for (int x = box.topLeft.x; x < box.bottomRight.x; x++)
                    {
                        for (int y = box.topLeft.y; y < box.bottomRight.y; y++)
                        {
                            double rDifference = img.Data[y, x, 2] - targetR;
                            double gDifference = img.Data[y, x, 1] - targetG;
                            double bDifference = img.Data[y, x, 0] - targetB;
                            double colorDistance = rDifference*rDifference * 0.3 + gDifference*gDifference * 0.59 + bDifference*bDifference * 0.11;
                            colorDistance /= similarityForegivenss;

                            img[x, y] = new Bgr(colorDistance, colorDistance, colorDistance);
                            boxTotalColorDistance += Math.Pow(colorDistance, distanceWeight);

                            if(colorDistance < 200)
                            {
                                estimatedOriginX += x - box.topLeft.x; estimatedOriginY += y - box.topLeft.y;
                            }
                        }
                    }
                    boxTotalColorDistance /= ((box.topLeft.x - box.bottomRight.x) * (box.topLeft.y - box.bottomRight.y)) * distanceWeight;

                    estimatedOriginX /= (box.bottomRight.x - box.topLeft.x);
                    estimatedOriginY /= (box.bottomRight.y - box.topLeft.y);

                    if (boxTotalColorDistance < cutoff)
                    {
                        box.topLeft.x = (int)(box.topLeft.x + estimatedOriginX);
                        box.topLeft.y = (int)(box.topLeft.y + estimatedOriginY);
                        box.bottomRight.x = (int)(box.bottomRight.x + estimatedOriginX);
                        box.bottomRight.y = (int)(box.bottomRight.y + estimatedOriginY);
                    }

                    Console.WriteLine(estimatedOriginX.ToString() + " " + estimatedOriginY.ToString());
                }

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
}

public class TrackingBox
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