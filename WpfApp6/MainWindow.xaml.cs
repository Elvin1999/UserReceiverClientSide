using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public string ImagePath { get; set; }
        private void uploadBtn_Click(object sender, RoutedEventArgs e)
        {
            // open file dialog   
            OpenFileDialog open = new OpenFileDialog();
            // image filters  
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (open.ShowDialog() == true)
            {
                // display image in picture box
                ImagePath = open.FileName;
                img.Source = new BitmapImage(new Uri(open.FileName));

            }
        }
        private static Socket ClientSocket = new Socket(
          AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // public  User User { get; set; } = new User() { Age = 22, Name = "Elvin", Surname = "Camalzade" };


        private void ConnectToServer()
        {
            while (!ClientSocket.Connected)
            {
                try
                {

                    ClientSocket.Connect(IPAddress.Parse("10.1.18.39"), 27001);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            MessageBox.Show("Connected Successfully");
        }

        private void sendBtn_Click(object sender, RoutedEventArgs e)
        {

            Task.Run(() =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    var imageBytes = ImageHelper.GetBytesOfImage(ImagePath);
                    var user = new UserModel
                    {
                        Age = int.Parse(AgeTxtbx.Text),
                        Fullname = nameTxtbx.Text,
                        ImageBytes = imageBytes
                    };
                    if (!ClientSocket.Connected)
                    {
                        ConnectToServer();
                    }
                    var jsonString = JsonConvert.SerializeObject(user);
                    byte[] buffer = Encoding.ASCII.GetBytes(jsonString);
                    ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
                });
            });
        }
    }
}
