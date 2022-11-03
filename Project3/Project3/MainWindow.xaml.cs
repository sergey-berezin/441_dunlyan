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
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Security.Cryptography;
using System.Windows.Input;
using System.Diagnostics.Eventing.Reader;

namespace Project3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window 
    {

        private ObservableCollection<Image> images_Analysis = new ObservableCollection<Image>();

        private ObservableCollection<Image> arr_neutral = new ObservableCollection<Image>();
        private ObservableCollection<Image> arr_happiness = new ObservableCollection<Image>();
        private ObservableCollection<Image> arr_surprise = new ObservableCollection<Image>();
        private ObservableCollection<Image> arr_sadness = new ObservableCollection<Image>();
        private ObservableCollection<Image> arr_anger = new ObservableCollection<Image>();
        private ObservableCollection<Image> arr_disgust = new ObservableCollection<Image>();
        private ObservableCollection<Image> arr_fear = new ObservableCollection<Image>();
        private ObservableCollection<Image> arr_contempt = new ObservableCollection<Image>();
        private bool Cancel = false;
        private bool can_Clear = true;
        private bool can_delete = true;
        private CancellationTokenSource cts;
        EmotionFerPlus Emotion = new EmotionFerPlus();

        List<string> picArr = new List<string>();
        public MainWindow()
        {   
            InitializeComponent();
            Load();
            testDataGrid.Visibility = Visibility.Hidden;
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

        public void Load()
        {
            using (var db = new DataContext())
            {
                if (db.Images.Any())
                {
                    foreach (var item in db.Images)
                    {   
                        if(db.Results.Any(r=>r.ImageId.Equals(item.ImageId)))
                        {
                            item.results = db.Results.Where(r => r.ImageId.Equals(item.ImageId)).ToList();
                            images_Analysis.Add(item);
                            emotionClassification(item);
                        }
                       
                    }
                    
                }

            }
        }
        private async Task<Image> Images_Analysis(string path, CancellationTokenSource cts)
        {

            var tmp = path.Split("\\");
            var relPath = tmp[tmp.Length - 1];
            var image_analysis = new Image(path);
            var rt = await Task.Run(async () =>
            {
                var img = await File.ReadAllBytesAsync(path, cts.Token);
                var res = await Emotion.EmotionalAnalysisAsync(img, cts);

                res.Sort((x, y) => -(x.Item2.CompareTo(y.Item2)));
                return res;
            }, cts.Token);
            foreach(var item in rt)
            {   
                image_analysis.results.Add(new Result { res=item.Item2,EmotionName = item.Item1});
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
                using (var db = new DataContext())
                {   
                    foreach (string pic in picArr_open)
                    {
                      picArr.Add(pic);
                      images_Analysis.Add(new Image(pic));
                    }
                }
            }
            ProgressBar_1.Visibility = Visibility.Hidden;

            analysis_Progress.Visibility = Visibility.Hidden;
        }

        private void btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            if (images_Analysis.Count == 0)
            {
                using (var db=new DataContext())
                {
                    if (!(db.Images.Any()&&db.Hashkeys.Any()&&db.Results.Any()))
                    {
                        MessageBox.Show("Image resource not added and no data in database!");
                    }
                    else
                    {
                            if(db.Images.Any())
                            foreach (var item in db.Images)
                            {
                                db.Images.Remove(item);
                            }
                            if (db.Hashkeys.Any())
                            foreach (var item in db.Hashkeys)
                            {
                                db.Hashkeys.Remove(item);
                            }
                            if(db.Results.Any())
                            foreach (var item in db.Bytedatas)
                            {
                                db.Bytedatas.Remove(item);
                            }
                            db.SaveChanges();
                        MessageBox.Show("The contents of the database have been emptied!");
                    }
                }
            }
            else
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
                        testDataGrid.ItemsSource = null;
                        using (var db = new DataContext())
                        {
                            foreach (var item in db.Images)
                            {
                                db.Images.Remove(item);
                            }
                            foreach (var item in db.Hashkeys)
                            {
                                db.Hashkeys.Remove(item);
                            }
                            foreach (var item in db.Bytedatas)
                            {
                                db.Bytedatas.Remove(item);
                            }
                            db.SaveChanges();
                        }
                        ProgressBar_1.Visibility = Visibility.Hidden;
                        testDataGrid.Visibility = Visibility.Hidden;
                        analysis_Progress.Visibility = Visibility.Hidden;
                    }

                }
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Cancel = true;
            can_Clear = false;
            can_delete=false;
            cts = new CancellationTokenSource();
            ProgressBar_1.Visibility = Visibility.Hidden;
            ProgressBar_1.Value = 0;
            analysis_Progress.Visibility = Visibility.Hidden;
            if (images_Analysis.Count == 0)
            {
                MessageBox.Show("New image resource not added!");
            }
            else
            {
                ProgressBar_1.Visibility = Visibility.Visible;

                analysis_Progress.Visibility = Visibility.Visible;
                try
                {
                    using (var db = new DataContext())
                    {
                        ProgressBar_1.Maximum = images_Analysis.Count;
                        for (int index = 0; index < images_Analysis.Count; index++)
                        {
                            var byte_img = File.ReadAllBytes(images_Analysis[index].Path); // picture to byte[] 
                            var img_hashcode = byte_img.GetHashCode(); // get a hashcode
                            // byte[] tmpHash = new MD5CryptoServiceProvider().ComputeHash(byte_img);
                            
                            if (!db.Hashkeys.Any(h => h.hashkey.Equals(img_hashcode))) 
                            {
                               
                                if (!db.Bytedatas.Any(b => b.byte_image.Equals(byte_img)))
                                {
                                    Image img = new Image(images_Analysis[index].Path);
                                    db.Images.Add(img);
                                    db.SaveChanges();
                                    var image = db.Images.OrderBy(d => d.ImageId).Last();
                                    images_Analysis[index].ImageId=image.ImageId;
                                    db.Hashkeys.Add(new Hashkey { hashkey = img_hashcode, ImageId = image.ImageId });
                                    db.Bytedatas.Add(new Bytedata { byte_image = byte_img, ImageId = image.ImageId });
                                    db.SaveChanges();
                                    
                                    images_Analysis[index].results = (await Images_Analysis(images_Analysis[index].Path, cts)).results;
                                    foreach (var result in images_Analysis[index].results)
                                    {
                                        db.Results.Add(new Result { EmotionName = result.EmotionName, res = result.res, ImageId = images_Analysis[index].ImageId });  
                                    }
                                    db.SaveChanges();

                                    emotionClassification(images_Analysis[index]);
                                }
                            }
                            ProgressBar_1.Value += 1;

                        

                        }

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
            can_Clear = true;
            can_delete = true;
        }
        public void emotionClassification(Image image)
        {


            if (image.results.FirstOrDefault() != null)
            {
                if (image.results.FirstOrDefault().EmotionName == "neutral")
                {
                    arr_neutral.Add(image);
                }
                else if (image.results.FirstOrDefault().EmotionName == "happiness")
                {
                    arr_happiness.Add(image);
                }
                else if (image.results.FirstOrDefault().EmotionName == "surprise")
                {
                    arr_surprise.Add(image);
                }
                else if (image.results.FirstOrDefault().EmotionName == "sadness")
                {
                    arr_sadness.Add(image);
                }
                else if (image.results.FirstOrDefault().EmotionName == "anger")
                {
                    arr_anger.Add(image);
                }
                else if (image.results.FirstOrDefault().EmotionName == "disgust")
                {
                    arr_disgust.Add(image);

                }
                else if (image.results.FirstOrDefault().EmotionName == "fear")
                {
                    arr_fear.Add(image);
                }
                else if (image.results.FirstOrDefault().EmotionName == "contempt")
                {
                    arr_contempt.Add(image);
                }

            }
            
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
        private void list_View_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            testDataGrid.AutoGenerateColumns = false;
           // MessageBox.Show(list_View.SelectedItem.GetType().ToString());
            Image img= list_View.SelectedItem as Image;
            if (img != null && img is Image)
            {
                using (var db = new DataContext())
                {
                    var query = db.Results.Where(r => r.ImageId.Equals(img.ImageId));
                    testDataGrid.ItemsSource = query.ToList();
                }
            }
            testDataGrid.Visibility = Visibility.Visible;
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
          //   MessageBox.Show(tabControl.SelectedIndex.ToString());
            //MessageBox.Show(img.ImageId.ToString());

            if(can_delete==false)
            {
                MessageBox.Show("Task in progress");
            }
            else
            {
                Image img = new Image();
                if (tabControl.SelectedIndex.ToString() == "0")
                {
                    img = list_View.SelectedItem as Image;
                    if (img != null && img is Image)
                    {
                        for (int i = 0; i < images_Analysis.Count; i++)
                        {
                            if (img == images_Analysis[i])
                            {
                                images_Analysis.RemoveAt(i);
                                break;
                            }
                        }
                        if (img.results.Count > 0)
                        {
                            if (img.results.First().EmotionName == "neutral")
                            {
                                for (int i = 0; i < arr_neutral.Count; i++)
                                {
                                    if (img == arr_neutral[i])
                                    {
                                        arr_neutral.RemoveAt(i);
                                        break;
                                    }
                                }
                            }
                            else if (img.results.First().EmotionName == "happiness")
                            {
                                for (int i = 0; i < arr_happiness.Count; i++)
                                {
                                    if (img == arr_happiness[i])
                                    {
                                        arr_happiness.RemoveAt(i);
                                        break;
                                    }
                                }
                            }
                            else if (img.results.First().EmotionName == "surprise")
                            {
                                for (int i = 0; i < arr_surprise.Count; i++)
                                {
                                    if (img == arr_surprise[i])
                                    {
                                        arr_surprise.RemoveAt(i);
                                        break;
                                    }
                                }
                            }
                            else if (img.results.First().EmotionName == "sadness")
                            {
                                for (int i = 0; i < arr_sadness.Count; i++)
                                {
                                    if (img == arr_sadness[i])
                                    {
                                        arr_sadness.RemoveAt(i);
                                        break;
                                    }
                                }
                            }
                            else if (img.results.First().EmotionName == "anger")
                            {
                                for (int i = 0; i < arr_anger.Count; i++)
                                {
                                    if (img == arr_anger[i])
                                    {
                                        arr_anger.RemoveAt(i);
                                        break;
                                    }
                                }
                            }
                            else if (img.results.First().EmotionName == "disgust")
                            {
                                for (int i = 0; i < arr_disgust.Count; i++)
                                {
                                    if (img == arr_disgust[i])
                                    {
                                        arr_disgust.RemoveAt(i);
                                        break;
                                    }
                                }
                            }
                            else if (img.results.First().EmotionName == "fear")
                            {
                                for (int i = 0; i < arr_fear.Count; i++)
                                {
                                    if (img == arr_fear[i])
                                    {
                                        arr_fear.RemoveAt(i);
                                        break;
                                    }
                                }
                            }
                            else if (img.results.First().EmotionName == "contempt")
                            {
                                for (int i = 0; i < arr_contempt.Count; i++)
                                {
                                    if (img == arr_contempt[i])
                                    {
                                        arr_contempt.RemoveAt(i);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image selected!");
                    }
                }
                else if (tabControl.SelectedIndex.ToString() == "1")
                {

                    img = list_View_contempt.SelectedItem as Image;
                    if (img != null && img is Image)
                    {
                        for (int i = 0; i < arr_contempt.Count; i++)
                        {
                            if (img == arr_contempt[i])
                            {
                                arr_contempt.RemoveAt(i);
                                break;
                            }
                        }
                        for (int i = 0; i < images_Analysis.Count; i++)
                        {
                            if (img == images_Analysis[i])
                            {
                                images_Analysis.RemoveAt(i);
                                break;
                            }
                        }
                    }


                    else
                    {
                        MessageBox.Show("No image selected!");
                    }
                }
                else if (tabControl.SelectedIndex.ToString() == "2")
                {

                    img = list_View_fear.SelectedItem as Image;
                    if (img != null && img is Image)
                    {
                        for (int i = 0; i < arr_fear.Count; i++)
                        {
                            if (img == arr_fear[i])
                            {
                                arr_fear.RemoveAt(i);
                                break;
                            }
                        }
                        for (int i = 0; i < images_Analysis.Count; i++)
                        {
                            if (img == images_Analysis[i])
                            {
                                images_Analysis.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image selected!");
                    }
                }
                else if (tabControl.SelectedIndex.ToString() == "3")
                {
                    img = list_View_disgust.SelectedItem as Image;
                    if (img != null && img is Image)
                    {
                        for (int i = 0; i < arr_disgust.Count; i++)
                        {
                            if (img == arr_disgust[i])
                            {
                                arr_disgust.RemoveAt(i);
                                break;
                            }
                        }
                        for (int i = 0; i < images_Analysis.Count; i++)
                        {
                            if (img == images_Analysis[i])
                            {
                                images_Analysis.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image selected!");
                    }
                }
                else if (tabControl.SelectedIndex.ToString() == "4")
                {
                    img = list_View_anger.SelectedItem as Image;
                    if (img != null && img is Image)
                    {
                        for (int i = 0; i < arr_anger.Count; i++)
                        {
                            if (img == arr_anger[i])
                            {
                                arr_anger.RemoveAt(i);
                                break;
                            }
                        }
                        for (int i = 0; i < images_Analysis.Count; i++)
                        {
                            if (img == images_Analysis[i])
                            {
                                images_Analysis.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image selected!");
                    }
                }
                else if (tabControl.SelectedIndex.ToString() == "5")
                {
                    img = list_View_sadness.SelectedItem as Image;
                    if (img != null && img is Image)
                    {
                        for (int i = 0; i < arr_sadness.Count; i++)
                        {
                            if (img == arr_sadness[i])
                            {
                                arr_sadness.RemoveAt(i);
                                break;
                            }
                        }
                        for (int i = 0; i < images_Analysis.Count; i++)
                        {
                            if (img == images_Analysis[i])
                            {
                                images_Analysis.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image selected!");
                    }
                }
                else if (tabControl.SelectedIndex.ToString() == "6")
                {
                    img = list_View_surprise.SelectedItem as Image;
                    if (img != null && img is Image)
                    {
                        for (int i = 0; i < arr_surprise.Count; i++)
                        {
                            if (img == arr_surprise[i])
                            {
                                arr_surprise.RemoveAt(i);
                                break;
                            }
                        }
                        for (int i = 0; i < images_Analysis.Count; i++)
                        {
                            if (img == images_Analysis[i])
                            {
                                images_Analysis.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image selected!");
                    }
                }
                else if (tabControl.SelectedIndex.ToString() == "7")
                {
                    img = list_View_happiness.SelectedItem as Image;
                    if (img != null && img is Image)
                    {
                        for (int i = 0; i < arr_happiness.Count; i++)
                        {
                            if (img == arr_happiness[i])
                            {
                                arr_happiness.RemoveAt(i);
                                break;
                            }
                        }
                        for (int i = 0; i < images_Analysis.Count; i++)
                        {
                            if (img == images_Analysis[i])
                            {
                                images_Analysis.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image selected!");
                    }
                }
                else if (tabControl.SelectedIndex.ToString() == "8")
                {
                    img = list_View_neutral.SelectedItem as Image;
                    if (img != null && img is Image)
                    {
                        for (int i = 0; i < arr_neutral.Count; i++)
                        {
                            if (img == arr_neutral[i])
                            {
                                arr_neutral.RemoveAt(i);
                                break;
                            }
                        }
                        for (int i = 0; i < images_Analysis.Count; i++)
                        {
                            if (img == images_Analysis[i])
                            {
                                images_Analysis.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image selected!");
                    }
                }

                if (img != null && img is Image)
                {
                    using (var db = new DataContext())
                    {
                        var query = db.Images.Where(x => x.ImageId.Equals(img.ImageId)).FirstOrDefault();
                        if (query != null)
                        {
                            db.Images.Remove(query);
                            testDataGrid.ItemsSource = null;

                            db.SaveChanges();
                        }
                    }

                }
                testDataGrid.Visibility = Visibility.Hidden;
     
            }
            

        }
    }
}
