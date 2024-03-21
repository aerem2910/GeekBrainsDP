using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.ServiceModel;
using ChatClient.ServiceChat;
using System.Windows.Media.Imaging;

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class MainWindow : Window, IServiceChatCallback
    {
        private bool isConnected = false;
        private ServiceChatClient client;
        private int ID;
        private OpenFileDialog fileDialog;
        private SaveFileDialog saveFileDialog;

        public MainWindow()
        {
            InitializeComponent();
            fileDialog = new OpenFileDialog();
            saveFileDialog = new SaveFileDialog();
            fileDialog.Filter = "Image Files (.png, .jpg, .bmp)|*.png;*.jpg;*.bmp";
            saveFileDialog.Filter = "Image Files (.png, .jpg, .bmp)|*.png;*.jpg;*.bmp";
        }

        private void ConnectUser()
        {
            try
            {
                if (!isConnected)
                {
                    client = new ServiceChatClient(new InstanceContext(this));
                    ID = client.Connect(tbUserName.Text);
                    tbUserName.IsEnabled = false;
                    bConnDicon.Content = "Disconnect";
                    isConnected = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting: {ex.Message}");
            }
        }

        private void DisconnectUser()
        {
            try
            {
                if (isConnected)
                {
                    client.Disconnect(ID);
                    client = null;
                    tbUserName.IsEnabled = true;
                    bConnDicon.Content = "Connect";
                    isConnected = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error disconnecting: {ex.Message}");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                DisconnectUser();
            }
            else
            {
                ConnectUser();
            }
        }

        public void MessageCallback(string message)
        {
            lbChat.Items.Add(message);
            lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DisconnectUser();
        }

        private void tbMessage_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    if (client != null)
                    {
                        client.SendMessage(tbMessage.Text, ID);
                        tbMessage.Text = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending message: {ex.Message}");
            }
        }


        public void btnSendImage_Click(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                try
                {
                    if (fileDialog.ShowDialog() == true)
                    {
                        
                        string imagePath = fileDialog.FileName;
                        byte[] dataUpload = File.ReadAllBytes(imagePath);
                        ListBoxItem lbi = new ListBoxItem();
                        lbi.Tag = dataUpload; // Сохранение массива байтов в Tag
                        
                        ImageMessage imageMessage = new ImageMessage
                        {
                            ImageName = System.IO.Path.GetFileName(imagePath),
                            ImageData = dataUpload
                        };

                        client.DisplayImage(dataUpload, System.IO.Path.GetFileName(fileDialog.FileName));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error sending image: {ex.Message}");
                }
            }
        }

        public void ImageCallback(ImageMessage imageMessage, int id, string senderName)
        {
            // Вызвать метод для отображения изображения в интерфейсе
            DisplayImageCallback(imageMessage.ImageData, imageMessage.ImageName);
        }

        public void DisplayImageCallback(byte[] imageData, string imageName)
        {
            

            try
            {
                using (var stream = new MemoryStream(imageData))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    var image = new Image();
                    image.Source = bitmap;
                    image.Width = 64;
                    image.Height = 64;

                    lbChat.Items.Add(image);
                }

                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying image: {ex.Message}");
            }
        }

              
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //  Код, который может быть выполнен при загрузке окна
        }

        private void lbChat_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lbChat.SelectedItem != null)
            {
                if (saveFileDialog.ShowDialog() == true)
                {
                    byte[] dataDownload = (byte[])((lbChat.SelectedItem as ListBoxItem).Tag); // Извлечение массива байтов из Tag
                    File.WriteAllBytes(saveFileDialog.FileName, dataDownload);
                }
            }
        }

        
    }
}
