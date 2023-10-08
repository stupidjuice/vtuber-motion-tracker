using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Collections;

public class Program
{
    public static void Main(string[] args)
    {
        //----Parameters----
        double cutoff = 200;

        List<TrackingBox> boxes = new List<TrackingBox>();
        TrackingBox test = new TrackingBox(80, 80, 40, 255, 255, 0);
        boxes.Add(test);

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
                    for (int x = box.x - box.radius; x < box.x + box.radius; x++)
                    {
                        img.Data[x, box.x + box.radius, 0] = box.b;
                        img.Data[x, box.x - box.radius, 0] = box.b;
                        img.Data[x, box.x + box.radius, 1] = box.g;
                        img.Data[x, box.x - box.radius, 1] = box.g;
                        img.Data[x, box.x + box.radius, 2] = box.r;
                        img.Data[x, box.x - box.radius, 2] = box.r;
                    }
                    for (int y = box.x - box.radius; y < box.x + box.radius; y++)
                    {
                        img.Data[box.x + box.radius, y, 0] = box.b;
                        img.Data[box.x - box.radius, y, 0] = box.b;
                        img.Data[box.x + box.radius, y, 1] = box.g;
                        img.Data[box.x - box.radius, y, 1] = box.g;
                        img.Data[box.x + box.radius, y, 2] = box.r;
                        img.Data[box.x - box.radius, y, 2] = box.r;
                    }

                    double circleCentroidX = 0, circleCentroidY = 0;
                    int numX = 0, numY = 0;
                    for (int x = box.x - box.radius; x < box.x + box.radius; x++)
                    {
                        for (int y = box.x - box.radius; y < box.x + box.radius; y++)
                        {
                            double pixelLuminance = 0.2126 * img.Data[x, y, 2] + 0.7152 * img.Data[x, y, 1] + 0.0722 * img.Data[x, y, 0];

                            if(pixelLuminance > cutoff)
                            {
                                numX += 1; numY += 1;

                                circleCentroidX += x - box.x / 2; circleCentroidY += y - box.y / 2;
                                img[x, y] = new Bgr(pixelLuminance, pixelLuminance, pixelLuminance);
                            }
                            else
                            {
                                img[x, y] = new Bgr(0.0, 0.0, 0.0);
                            }
                        }
                    }
                    if (numX > 0 && numY > 0)
                    {
                        circleCentroidX /= numX; circleCentroidX += box.x / 2;
                        circleCentroidY /= numY; circleCentroidY += box.y / 2;
                    }

                    img[(byte)circleCentroidX, (byte)circleCentroidY] = new Bgr(0.0, 0.0, 255.0);
                    Console.WriteLine(circleCentroidX + " " + circleCentroidY);
                }
                CvInvoke.Imshow("test", img);
            }
    }
}

public class TrackingBox
{
    public int x, y;
    public int radius;

    public Byte r, g, b;

    public TrackingBox(int x, int y, int radius, byte r, byte g, byte b)
    {
        this.x = x;
        this.y = y;
        this.radius = radius;
        this.r = r;
        this.g = g;
        this.b = b;
    }
}