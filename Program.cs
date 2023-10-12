using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Collections;
public class Program
{
    public static void Main(string[] args)
    {
        //----Parameters----
        //camera settings
        int leftCameraIndex = 0; //these are almost CERTAINLY not the same for your system
        int rightCameraIndex = 1;
        double cameraFOV = 52.0;
        double distanceBetweenCameras = 6.73; //cm
        double imgWidth = 640;

        //other settings
        double cutoff = 240;

        List<TrackingBox> boxes = new List<TrackingBox>();
        TrackingBox test = new TrackingBox(120, 120, 30, 255, 255, 0, "test");
        TrackingBox test2 = new TrackingBox(120, 120, 30, 255, 255, 0, "test");
        boxes.Add(test);
        boxes.Add(test2);

        StereoTrackingBoxes testboxes = new StereoTrackingBoxes(test, test2);

        Dictionary<string, StereoTrackingBoxes> trackingBoxLookup = new Dictionary<string, StereoTrackingBoxes>();
        trackingBoxLookup.Add(testboxes.left.tag, testboxes);

        CvInvoke.NamedWindow("left");
        CvInvoke.NamedWindow("right");
        using (Mat frame = new Mat())
        using (Mat frame2 = new Mat())
        using (VideoCapture capture = new VideoCapture(leftCameraIndex))
        using (VideoCapture capture2 = new VideoCapture(rightCameraIndex))
            while (CvInvoke.WaitKey(1) == -1)
            {
                capture.Read(frame);
                capture2.Read(frame2);
                Image<Bgr, Byte> img = frame.ToImage<Bgr, Byte>();
                Image<Bgr, Byte> img2 = frame2.ToImage<Bgr, Byte>();

                Parallel.For(0, boxes.Count, i =>
                {
                    Image<Bgr, Byte> activeImg = i % 2 == 0 ? img : img2;

                    TrackingBox box = boxes[i];
                    //-----DRAW BOXES-----//
                    for (int x = box.x - box.radius; x < box.x + box.radius; x++)
                    {
                        activeImg.Data[x, box.y + box.radius, 0] = box.b;
                        activeImg.Data[x, box.y - box.radius, 0] = box.b;
                        activeImg.Data[x, box.y + box.radius, 1] = box.g;
                        activeImg.Data[x, box.y - box.radius, 1] = box.g;
                        activeImg.Data[x, box.y + box.radius, 2] = box.r;
                        activeImg.Data[x, box.y - box.radius, 2] = box.r;
                    }
                    for (int y = box.y - box.radius; y < box.y + box.radius; y++)
                    {
                        activeImg.Data[box.x + box.radius, y, 0] = box.b;
                        activeImg.Data[box.x - box.radius, y, 0] = box.b;
                        activeImg.Data[box.x + box.radius, y, 1] = box.g;
                        activeImg.Data[box.x - box.radius, y, 1] = box.g;
                        activeImg.Data[box.x + box.radius, y, 2] = box.r;
                        activeImg.Data[box.x - box.radius, y, 2] = box.r;
                    }

                    //find median of circle inside tracking feature
                    List<int> yList = new List<int>(); //lists are used to find medians.
                    List<int> xList = new List<int>(); //there's probably a better way but idk
                    int medianX = 0, medianY = 0;

                    for (int x = box.x - box.radius + 1; x < box.x + box.radius; x++)
                    {
                        for (int y = box.y - box.radius + 1; y < box.y + box.radius; y++)
                        {
                            double pixelLuminance = 0.2126 * activeImg.Data[x, y, 2] + 0.7152 * activeImg.Data[x, y, 1] + 0.0722 * activeImg.Data[x, y, 0];

                            if (pixelLuminance > cutoff)
                            {
                                xList.Add(x);
                                yList.Add(y);
                                activeImg.Data[x, y, 0] = 0;
                            }
                        }
                    }

                    yList.Sort();

                    if (xList.Count > 0)
                    {
                        medianX = xList[(xList.Count - 1) >> 1];
                        medianY = yList[(yList.Count - 1) >> 1];
                    }
                    else
                    {
                        medianX = box.x;
                        medianY = box.y;
                    }

                    box.x = medianX; box.y = medianY;

                    activeImg[medianX, medianY] = new Bgr(0.0, 0.0, 255.0);
                });

                Parallel.For(0, boxes.Count >> 1, i =>
                {
                    StereoTrackingBoxes currentBoxes = trackingBoxLookup[boxes[i * 2].tag];

                    //https://www.csfieldguide.org.nz/en/chapters/computer-vision/depth/ used this to calculate the depth
                    currentBoxes.depth = (distanceBetweenCameras * imgWidth) / (2 * Math.Tan(cameraFOV / 2) * (currentBoxes.left.x - currentBoxes.right.x));
                    Console.WriteLine(currentBoxes.depth);
                });

                CvInvoke.Imshow("left", img);
                CvInvoke.Imshow("right", img2);
                img.Dispose();
                img2.Dispose();
            }
    }

    public struct StereoTrackingBoxes
    {
        public TrackingBox left, right;
        public double depth;

        public StereoTrackingBoxes(TrackingBox left, TrackingBox right)
        {
            this.left = left;
            this.right = right;
            this.depth = 0.0;
        }
    }
}

public class TrackingBox
{
    public int x, y;
    public int radius;

    public Byte r, g, b;
    public string tag;

    public TrackingBox(int x, int y, int radius, byte r, byte g, byte b, string tag)
    {
        this.x = x;
        this.y = y;
        this.radius = radius;
        this.r = r;
        this.g = g;
        this.b = b;
        this.tag = tag;
    }
}