using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
using System.Threading;
using System.IO;
using EmotionFer_Plus;


namespace Project2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {

        private ObservableCollection<image_analysis> images_Analysis = new ObservableCollection<image_analysis>();

        private ObservableCollection<image_analysis> arr_neutral = new ObservableCollection<image_analysis>();
        private ObservableCollection<image_analysis> arr_happiness = new ObservableCollection<image_analysis>();
        private ObservableCollection<image_analysis> arr_surprise = new ObservableCollection<image_analysis>();
        private ObservableCollection<image_analysis> arr_sadness = new ObservableCollection<image_analysis>();
        private ObservableCollection<image_analysis> arr_anger = new ObservableCollection<image_analysis>();
        private ObservableCollection<image_analysis> arr_disgust = new ObservableCollection<image_analysis>();
        private ObservableCollection<image_analysis> arr_fear = new ObservableCollection<image_analysis>();
        private ObservableCollection<image_analysis> arr_contempt = new ObservableCollection<image_analysis>();
        private bool Cancel = false;
        private bool can_Clear = false;
        private CancellationTokenSource cts;
        EmotionFerPlus Emotion = new EmotionFerPlus();

        List<string> picArr = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
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
        private async Task<image_analysis> Images_Analysis(string path, CancellationTokenSource cts)
        {

            var tmp = path.Split("\\");
            var relPath = tmp[tmp.Length - 1];
            var image_analysis = new image_analysis(path);
            var rt = await Task.Run(async () =>
            {
                var img = await File.ReadAllBytesAsync(path, cts.Token);
                var res = await Emotion.EmotionalAnalysisAsync(img, cts);

                res.Sort((x, y) => -(x.Item2.CompareTo(y.Item2)));
                image_analysis.set_values(res);
                return res;
            }, cts.Token);
            if (image_analysis.Result.First().Item1 == "neutral")
            {
                arr_neutral.Add(image_analysis);
            }
            else if (image_analysis.Result.First().Item1 == "happiness")
            {
                arr_happiness.Add(image_analysis);
            }
            else if (image_analysis.Result.First().Item1 == "surprise")
            {
                arr_surprise.Add(image_analysis);
            }
            else if (image_analysis.Result.First().Item1 == "sadness")
            {
                arr_sadness.Add(image_analysis);
            }
            else if (image_analysis.Result.First().Item1 == "anger")
            {
                arr_anger.Add(image_analysis);
            }
            else if (image_analysis.Result.First().Item1 == "disgust")
            {
                arr_disgust.Add(image_analysis);

            }
            else if (image_analysis.Result.First().Item1 == "fear")
            {
                arr_fear.Add(image_analysis);
            }
            else if (image_analysis.Result.First().Item1 == "contempt")
            {
                arr_contempt.Add(image_analysis);
            }


            return image_analysis;

        }

        private void btn_Open_Click(object sender, RoutedEventArgs? e = null)
        {

            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "All Image Files|*.bmp;*.ico;*.gif;*.jpeg;*.jpg;*.png;*.tif;*.tiff;*";

            if ((bool)openFileDialog.ShowDialog())
            {
                string[] picArr_open = openFileDialog.FileNames;
                foreach (string pic in picArr_open)
                {
                    images_Analysis.Add(new image_analysis(pic));
                    picArr.Add(pic);
                }

            }
            ProgressBar_1.Visibility = Visibility.Hidden;

            analysis_Progress.Visibility = Visibility.Hidden;
        }

        private void btn_Clear_Click(object sender, RoutedEventArgs e)
        {

            if (can_Clear == true)
            {
                MessageBox.Show("Task in progress");
            }
            else
            {
                ProgressBar_1.Visibility = Visibility.Hidden;

                analysis_Progress.Visibility = Visibility.Hidden;
                images_Analysis.Clear();
                arr_neutral.Clear();
                arr_happiness.Clear();
                arr_surprise.Clear();
                arr_sadness.Clear();
                arr_anger.Clear();
                arr_disgust.Clear();
                arr_fear.Clear();
                arr_contempt.Clear();
                picArr.Clear();
                list_View.ItemsSource = images_Analysis;

                list_View_neutral.ItemsSource = arr_neutral;
                list_View_happiness.ItemsSource = arr_happiness;
                list_View_surprise.ItemsSource = arr_surprise;
                list_View_sadness.ItemsSource = arr_sadness;
                list_View_anger.ItemsSource = arr_anger;
                list_View_disgust.ItemsSource = arr_disgust;
                list_View_fear.ItemsSource = arr_fear;
                list_View_contempt.ItemsSource = arr_contempt;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Cancel = true;
            can_Clear = true;
            arr_neutral.Clear();
            arr_happiness.Clear();
            arr_surprise.Clear();
            arr_sadness.Clear();
            arr_anger.Clear();
            arr_disgust.Clear();
            arr_fear.Clear();
            arr_contempt.Clear();
            cts = new CancellationTokenSource();
            if (picArr.Count == 0)
            {
                MessageBox.Show("Image resource not added!");
            }
            else
            {
                ProgressBar_1.Visibility = Visibility.Visible;

                analysis_Progress.Visibility = Visibility.Visible;
                try
                {

                    ProgressBar_1.Maximum = images_Analysis.Count;
                    for (int index = 0; index < images_Analysis.Count; index++)
                    {
                        images_Analysis[index].set_values((await Images_Analysis(picArr[index], cts)).Result);
                        ProgressBar_1.Value += 1;
                    }
                    list_View.ItemsSource = images_Analysis;
                    list_View_neutral.ItemsSource = arr_neutral;
                    list_View_happiness.ItemsSource = arr_happiness;
                    list_View_surprise.ItemsSource = arr_surprise;
                    list_View_sadness.ItemsSource = arr_sadness;
                    list_View_anger.ItemsSource = arr_anger;
                    list_View_disgust.ItemsSource = arr_disgust;
                    list_View_fear.ItemsSource = arr_fear;
                    list_View_contempt.ItemsSource = arr_contempt;
                }

                catch (TaskCanceledException)
                {

                    MessageBox.Show("Task was cancelled!");
                }
            }
            Cancel = false;
            can_Clear = false;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
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
        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

    }
}
