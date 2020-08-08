using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using NetMQ;
using NetMQ.Sockets;


namespace ClientSideTestImage
{
    public partial class WebCam : Form
    {
        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice videoCaptureDevice;
        //MemoryStream stream;
        public WebCam()
        {
            InitializeComponent();
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in filterInfoCollection)
            {
                cboCamera.Items.Add(filterInfo.Name);
            }
            cboCamera.SelectedIndex = 0;
            videoCaptureDevice = new VideoCaptureDevice();
            //stream = new MemoryStream();

        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            videoCaptureDevice = new VideoCaptureDevice(filterInfoCollection[cboCamera.SelectedIndex].MonikerString);
            videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
            videoCaptureDevice.Start();
        }

        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            pic.Image = image;
            //image.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
            //byte[] bytes = stream.ToArray();
            using (var client = new RequestSocket())
            {
                client.Connect("tcp://localhost:5555");
                client.SendFrame(image);
                var message = client.ReceiveFrameString();
            }
        }

        private void btnClose_MouseClick(object sender, MouseEventArgs e)
        {
            if (videoCaptureDevice.IsRunning == true)
            {
                videoCaptureDevice.Stop();
            }
        }

        /*
        public byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
        */

        /*
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (videoCaptureDevice.IsRunning == true)
            {
                videoCaptureDevice.Stop();
            }
        }
        */
    }
}
