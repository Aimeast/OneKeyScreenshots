using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace OneKeyScreenshots
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var filename = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName);
            Apply(filename);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
        }

        public System.Windows.Input.KeyGesture ParseKeyGesture(string aKeyGestureString)
        {
            try
            {
                return (System.Windows.Input.KeyGesture)new System.Windows.Input.KeyGestureConverter().ConvertFrom(aKeyGestureString);
            }
            catch
            {
                return null;
            }
        }

        private void Apply(string hotkey)
        {
            var gesture = ParseKeyGesture(hotkey) ?? ParseKeyGesture("Ctrl+Alt+Q");
            hotkey = new System.Windows.Input.KeyGestureConverter().ConvertToString(gesture);

            label2.Text = notifyIcon1.Text = hotkey;

            var registered = GlobalHotKey.RegisterHotKey(gesture.Modifiers, gesture.Key, ClipAllScreens);
            if (!registered)
            {
                MessageBox.Show($"{hotkey} in use");
                this.Dispose();
            }
        }

        private void ClipAllScreens()
        {
            // https://stackoverflow.com/a/15862043/1011174
            // Determine the size of the "virtual screen", which includes all monitors.
            int screenLeft = SystemInformation.VirtualScreen.Left;
            int screenTop = SystemInformation.VirtualScreen.Top;
            int screenWidth = SystemInformation.VirtualScreen.Width;
            int screenHeight = SystemInformation.VirtualScreen.Height;

            using (var bmp = new Bitmap(screenWidth, screenHeight))
            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(screenLeft, screenTop, 0, 0, bmp.Size);
                bmp.Save($"D:\\{DateTime.Now.ToString("yyyyMMdd-HHmmss.ff")}.png", ImageFormat.Png);
            }
        }
    }
}
