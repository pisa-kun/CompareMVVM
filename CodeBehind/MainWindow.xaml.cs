using System;
using System.Collections.Generic;
using System.IO;
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

namespace CodeBehind
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// マイピクチャのフォルダーパス
        /// </summary>
        private static readonly string picPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// オプションボタンを押したときの処理
        /// StackPanel x:Name = "option"領域のVisiblityを切り替え
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e) 
            => this.option.Visibility = this.option.Visibility != Visibility.Visible ? Visibility.Visible : Visibility.Hidden;

        /// <summary>
        /// TextBlockに入出力があった場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            // 撮影停止状態かつテキストに文字が入力されている場合、保存ボタンを押せるようにする
            if (this.stop.IsEnabled == false && this.input.Text.Length != 0)
            {
                this.photo.IsEnabled = true;
            }
            else
            {
                this.photo.IsEnabled = false;
            }

            this.name.Text = this.input.Text; // 入力された文字を反映
        }

        /// <summary>
        /// 撮影ボタンを押したときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            // 撮影処理

            this.start.IsEnabled = false;
            this.stop.IsEnabled = true;
            this.photo.IsEnabled = false;
        }

        /// <summary>
        /// 停止ボタンを押したときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            this.start.IsEnabled = true;
            this.stop.IsEnabled = false;
            this.photo.IsEnabled = this.input.Text.Length != 0 ? true : false;
        }

        /// <summary>
        /// 保存ボタンを押したときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Photo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var filePath = "SampleMVVM";
                var folderPath = System.IO.Path.Combine(picPath, filePath);
                if (!Directory.Exists(folderPath)) // フォルダが存在しないときに作成
                {
                    Directory.CreateDirectory(folderPath);
                }
                var saveName = (System.IO.Path.Combine(folderPath, this.name.Text) + ".png");
                SaveImageSourceToFile(this.save.Source, saveName);
                MessageBox.Show("画像を保存できました。");
            }
            catch
            {
                MessageBox.Show("画像を保存できませんでした。");
            }
        }

        /// <summary>
        /// ImageSourceをBitmapにキャストしてfilepathに保存
        /// </summary>
        /// <param name="bitmapSource"></param>
        /// <param name="filePath"></param>
        private void SaveImageSourceToFile(ImageSource imageSource, string filePath)
        {
            using(var fileStream = new FileStream(filePath, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imageSource));
                encoder.Save(fileStream);
            }
        }
    }
}
