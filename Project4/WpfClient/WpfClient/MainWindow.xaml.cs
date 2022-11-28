using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Http.Json;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Reflection;
using System.IO;
using System.Threading;
using CancellationTokenSource = System.Threading.CancellationTokenSource;
using System.Threading.Channels;

namespace WpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool Cancel = false;
        private bool can_Clear = true;
        private bool can_delete = true;
        private CancellationTokenSource cts;
        //  private ObservableCollection<ImageInfo> images_Analysis = new ObservableCollection<ImageInfo>();
        private ObservableCollection<ImageInfo> images_Analysis = new ObservableCollection<ImageInfo>();
        private ObservableCollection<byte[]> arr_neutral = new ObservableCollection<byte[]>();
        private ObservableCollection<byte[]> arr_happiness = new ObservableCollection<byte[]>();
        private ObservableCollection<byte[]> arr_surprise = new ObservableCollection<byte[]>();
        private ObservableCollection<byte[]> arr_sadness = new ObservableCollection<byte[]>();
        private ObservableCollection<byte[]> arr_anger = new ObservableCollection<byte[]>();
        private ObservableCollection<byte[]> arr_disgust = new ObservableCollection<byte[]>();
        private ObservableCollection<byte[]> arr_fear = new ObservableCollection<byte[]>();
        private ObservableCollection<byte[]> arr_contempt = new ObservableCollection<byte[]>();
        public  MainWindow()
        {
            InitializeComponent();
            load(); 
            list_View.ItemsSource = images_Analysis;
            list_View_neutral.ItemsSource = arr_neutral;
            list_View_happiness.ItemsSource = arr_happiness;
            list_View_surprise.ItemsSource = arr_surprise;
            list_View_sadness.ItemsSource = arr_sadness;
            list_View_anger.ItemsSource = arr_anger;
            list_View_disgust.ItemsSource = arr_disgust;
            list_View_fear.ItemsSource = arr_fear;
            list_View_contempt.ItemsSource = arr_contempt;
            ProgressBar_1.Visibility = Visibility.Hidden;

            analysis_Progress.Visibility = Visibility.Hidden;
        }

        public async void load() //load image and result from server
        {
            var httpclient = new HttpClient();
            httpclient.BaseAddress = new Uri("https://localhost:7285/");
            httpclient.DefaultRequestHeaders.Accept.Clear();
            httpclient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response_load = await httpclient.GetAsync("api/Images/AllImages");
            try
            {
                response_load.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException HRex)
            {
               
            }
            finally
            {
                //List < (string, double) > res = await response.Content.ReadFromJsonAsync<List<(string, double)>>();
                // var res = response.Content.ReadAsHttpRequestMessageAsync();
                List<byte[]> values = await response_load.Content.ReadFromJsonAsync<List<byte[]>>();
                foreach(var item in values)
                {
                 
                    images_Analysis.Add(new ImageInfo { imageDataByte = item});
                }
            }
            HttpResponseMessage response1_load = await httpclient.GetAsync("api/Images/AllResults");
            try
            {
                response1_load.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException HRex)
            {
                
            }
            finally
            {
       
                List<string> values = await response1_load.Content.ReadFromJsonAsync<List<string>>();
                for (int i= 0; i < values.Count;i++)
                {
                    images_Analysis[i].result = values[i];
                }
                //
            }
            emotionClassification();
        }
        private void btn_Open_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "All Image Files|*.bmp;*.ico;*.gif;*.jpeg;*.jpg;*.png;*.tif;*.tiff;*";

            if ((bool)openFileDialog.ShowDialog())
            {
                string[] picArr_open = openFileDialog.FileNames;
                foreach (string pic in picArr_open)
                {
                    //  System.Windows.Controls.Image img=new System.Windows.Controls.Image();
                    //  img.Source = new BitmapImage(new Uri(pic, UriKind.Relative)); ;
                    //images_Analysis.Add(new ImageInfo { imgSource = new BitmapImage(new Uri(pic)),imagePath=pic });
                    images_Analysis.Add(new ImageInfo { imageDataByte = File.ReadAllBytes(pic) });

                }

            }
            ProgressBar_1.Visibility = Visibility.Hidden;

            analysis_Progress.Visibility = Visibility.Hidden;
        }

        private void list_View_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private async void btn_Post_Click(object sender, RoutedEventArgs e)
        {
            Cancel = true;
            can_Clear = false;
            can_delete = false;
            cts = new CancellationTokenSource();
            ProgressBar_1.Value = 0;
            ProgressBar_1.Maximum = images_Analysis.Count;
            if (images_Analysis.Count == 0)
            {
                MessageBox.Show("New image resource not added!");
                can_Clear = true;
            }
            else
            {
               
                try

                {
                    ProgressBar_1.Visibility = Visibility.Visible;

                    analysis_Progress.Visibility = Visibility.Visible;
                    var httpclient = new HttpClient();
                    httpclient.BaseAddress = new Uri("https://localhost:7285/");
                    httpclient.DefaultRequestHeaders.Accept.Clear();
                    httpclient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                    _Image img = new _Image();
                    for (int i = 0; i < images_Analysis.Count; i++)
                    {

                        if (images_Analysis[i].result==""|| images_Analysis[i].result ==null)
                        {
                            //var byte_img = File.ReadAllBytes(images_Analysis[i].imagePath);
                            img.byte_image = images_Analysis[i].imageDataByte;

                            HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(httpclient, "api/Images", img, cts.Token);
                            try
                            {
                                response.EnsureSuccessStatusCode();
                            }
                            catch (HttpRequestException HRex)
                            {

                            }
                            finally
                            {
                                //List < (string, double) > res = await response.Content.ReadFromJsonAsync<List<(string, double)>>();
                                // var res = response.Content.ReadAsHttpRequestMessageAsync();
                                images_Analysis[i].result = await response.Content.ReadFromJsonAsync<string>();

                            }

                           
                        }
                        ProgressBar_1.Value += 1;
                    }
                    
                }
                catch (TaskCanceledException)
                {

                    MessageBox.Show("Task was cancelled!");
                    emotionClassification();

                }
                finally 
                {
                    emotionClassification();
                }
               
                Cancel = false;
                can_Clear = true;
                can_delete = true;
            }

        }
        public async Task emotionClassification()
        {
            var httpclient = new HttpClient();
            httpclient.BaseAddress = new Uri("https://localhost:7285/");
            httpclient.DefaultRequestHeaders.Accept.Clear();
            httpclient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await httpclient.GetAsync("api/Images?Emotion=happiness");
            HttpResponseMessage response1 = await httpclient.GetAsync("api/Images?Emotion=neutral");
            HttpResponseMessage response2 = await httpclient.GetAsync("api/Images?Emotion=sadness");
            HttpResponseMessage response3 = await httpclient.GetAsync("api/Images?Emotion=contempt");
            HttpResponseMessage response4 = await httpclient.GetAsync("api/Images?Emotion=surprise");
            HttpResponseMessage response5 = await httpclient.GetAsync("api/Images?Emotion=anger");
            HttpResponseMessage response6 = await httpclient.GetAsync("api/Images?Emotion=disgust");
            HttpResponseMessage response7 = await httpclient.GetAsync("api/Images?Emotion=fear");
            try
            {
                response7.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                List<byte[]> lis_Imgfear = await response7.Content.ReadFromJsonAsync<List<byte[]>>();

                // Box1.Text += lis_Img._ImageId;

                foreach (var item in lis_Imgfear)
                {
                   /* System.IO.MemoryStream ms = new System.IO.MemoryStream(item);
                    var decoder = BitmapDecoder.Create(ms,
                    BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

                    System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(img);
                    IntPtr hBitmap = bmp.GetHbitmap();
                    System.Windows.Media.ImageSource WpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());*/
                    // System.Windows.Controls.Image im = new System.Windows.Controls.Image();
                    // im.Source = WpfBitmap;
                    arr_fear.Add(item);
                }
            }
            try
            {
                response6.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                List<byte[]> lis_Imgdisgust = await response6.Content.ReadFromJsonAsync<List<byte[]>>();

                // Box1.Text += lis_Img._ImageId;

                foreach (var item in lis_Imgdisgust)
                {
                   /* System.IO.MemoryStream ms = new System.IO.MemoryStream(item);
                    var decoder = BitmapDecoder.Create(ms,
                    BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

                    System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(img);
                    IntPtr hBitmap = bmp.GetHbitmap();
                    System.Windows.Media.ImageSource WpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());*/
                    // System.Windows.Controls.Image im = new System.Windows.Controls.Image();
                    // im.Source = WpfBitmap;
                    arr_disgust.Add(item);
                }
            }
            try
            {
                response5.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                List<byte[]> lis_Imgsanger = await response5.Content.ReadFromJsonAsync<List<byte[]>>();

                // Box1.Text += lis_Img._ImageId;

                foreach (var item in lis_Imgsanger)
                {
                    /*System.IO.MemoryStream ms = new System.IO.MemoryStream(item);
                    var decoder = BitmapDecoder.Create(ms,
                    BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

                    System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(img);
                    IntPtr hBitmap = bmp.GetHbitmap();
                    System.Windows.Media.ImageSource WpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());*/
                    // System.Windows.Controls.Image im = new System.Windows.Controls.Image();
                    // im.Source = WpfBitmap;
                    arr_anger.Add(item);
                }
            }
            try
            {
                response4.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                List<byte[]> lis_Imgssurprise = await response4.Content.ReadFromJsonAsync<List<byte[]>>();

                // Box1.Text += lis_Img._ImageId;

                foreach (var item in lis_Imgssurprise)
                {
                   /* System.IO.MemoryStream ms = new System.IO.MemoryStream(item);
                    var decoder = BitmapDecoder.Create(ms,
                    BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

                    System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(img);
                    IntPtr hBitmap = bmp.GetHbitmap();
                    System.Windows.Media.ImageSource WpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());*/
                    // System.Windows.Controls.Image im = new System.Windows.Controls.Image();
                    // im.Source = WpfBitmap;
                    arr_surprise.Add(item);
                }
            }
            try
            {
                response3.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                List<byte[]> lis_Imgscontempt = await response3.Content.ReadFromJsonAsync<List<byte[]>>();

                // Box1.Text += lis_Img._ImageId;

                foreach (var item in lis_Imgscontempt)
                {
                   /* System.IO.MemoryStream ms = new System.IO.MemoryStream(item);
                    var decoder = BitmapDecoder.Create(ms,
                    BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

                    System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(img);
                    IntPtr hBitmap = bmp.GetHbitmap();
                    System.Windows.Media.ImageSource WpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());*/
                    // System.Windows.Controls.Image im = new System.Windows.Controls.Image();
                    // im.Source = WpfBitmap;
                    arr_contempt.Add(item);
                }
            }
            try
            {
                response2.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                List<byte[]> lis_Imgsadness = await response2.Content.ReadFromJsonAsync<List<byte[]>>();

                // Box1.Text += lis_Img._ImageId;

                foreach (var item in lis_Imgsadness)
                {
                    /*System.IO.MemoryStream ms = new System.IO.MemoryStream(item);
                    var decoder = BitmapDecoder.Create(ms,
                    BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

                    System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(img);
                    IntPtr hBitmap = bmp.GetHbitmap();
                    System.Windows.Media.ImageSource WpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());*/
                    // System.Windows.Controls.Image im = new System.Windows.Controls.Image();
                    // im.Source = WpfBitmap;
                    arr_sadness.Add(item);
                }
            }
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
               
            }
            finally
            {
                List<byte[]> lis_Imghappiness = await response.Content.ReadFromJsonAsync<List<byte[]>>();

                // Box1.Text += lis_Img._ImageId;

                foreach(var item in lis_Imghappiness)
                {
                    /*System.IO.MemoryStream ms = new System.IO.MemoryStream(item);
                    var decoder = BitmapDecoder.Create(ms,
                    BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

                    System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(img);
                    IntPtr hBitmap = bmp.GetHbitmap();
                    System.Windows.Media.ImageSource WpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());*/
                   // System.Windows.Controls.Image im = new System.Windows.Controls.Image();
                   // im.Source = WpfBitmap;
                    arr_happiness.Add(item);
                }
            }
            try
            {
                response1.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                List<byte[]> lis_Imgneutral = await response1.Content.ReadFromJsonAsync<List<byte[]>>();
                foreach (var item in lis_Imgneutral)
                {
                    /*System.IO.MemoryStream ms = new System.IO.MemoryStream(item);
                    var decoder = BitmapDecoder.Create(ms,
                    BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

                    System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(img);
                    IntPtr hBitmap = bmp.GetHbitmap();
                    System.Windows.Media.ImageSource WpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());*/
                    // System.Windows.Controls.Image im = new System.Windows.Controls.Image();
                    // im.Source = WpfBitmap;
                    arr_neutral.Add(item);
                }
            }
        }



        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {

            if (Cancel == true)
            {
                cts.Cancel();
                ProgressBar_1.Value = 0;

                ProgressBar_1.Visibility = Visibility.Hidden;

                analysis_Progress.Visibility = Visibility.Hidden;
                Cancel = false;
            }
            else
            {
                MessageBox.Show("Task has not started!");
            }
        }

        private async void btn_Clear_Click(object sender, RoutedEventArgs e)
        {

            
                if (can_Clear == false)
                {
                    MessageBox.Show("Task in progress");
                }
                else
                {

                    if (System.Windows.MessageBox.Show("This operation will delete all images and clear all data in the database", "Warning：", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        images_Analysis.Clear();
                        arr_neutral.Clear();
                        arr_happiness.Clear();
                        arr_surprise.Clear();
                        arr_sadness.Clear();
                        arr_anger.Clear();
                        arr_disgust.Clear();
                        arr_fear.Clear();
                        arr_contempt.Clear();
                        list_View_neutral.ItemsSource = arr_neutral;
                        list_View_happiness.ItemsSource = arr_happiness;
                        list_View_surprise.ItemsSource = arr_surprise;
                        list_View_sadness.ItemsSource = arr_sadness;
                        list_View_anger.ItemsSource = arr_anger;
                        list_View_disgust.ItemsSource = arr_disgust;
                        list_View_fear.ItemsSource = arr_fear;
                        list_View_contempt.ItemsSource = arr_contempt;
                        var httpclient = new HttpClient();
                        httpclient.BaseAddress = new Uri("https://localhost:7285/");
                        httpclient.DefaultRequestHeaders.Accept.Clear();
                        httpclient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await httpclient.DeleteAsync("/api/Images/All");
                        try
                        {
                            response.EnsureSuccessStatusCode();
                        }
                        catch (HttpRequestException HRex)
                        {

                        }
                        finally
                        {

                        }

                    }
                }
           
               
        }

        
    }
    public class _Image
    {
        public int _ImageId { get; set; }
        public byte[] byte_image { get; set; }
    }
    
}
