namespace CodeBehind
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        #region フィールド変数
        /// <summary>
        /// マイピクチャのフォルダーパス
        /// </summary>
        private static readonly string picPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

        /// <summary>
        /// インスタンスを格納
        /// </summary>
        private Camera camera;

        private bool isTask = true;

        #endregion

        public MainWindow() => InitializeComponent();

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
            this.photo.IsEnabled = (this.camera != null && this.stop.IsEnabled == false && this.input.Text.Length != 0) ? true : false;

            this.name.Text = this.input.Text; // 入力された文字を反映
        }

        /// <summary>
        /// 撮影ボタンを押したときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Start_Click(object sender, RoutedEventArgs e)
        {

            this.isTask = true;
            this.start.IsEnabled = false;
            this.stop.IsEnabled = true;
            this.photo.IsEnabled = false;
            // 撮影処理
            // isTaskがboolになるまでwaitし続ける
            await StartCapture();
        }

        /// <summary>
        /// 停止ボタンを押したときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            StopCapture();
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

        /// <summary>
        /// カメラを起動する
        /// フィールド変数がnullのときにカメラインスタンスを代入
        /// </summary>
        private async Task StartCapture()
        {
            this.camera = this.camera ?? new Camera();

            while (isTask)
            {
                try
                {
                    await this.camera.Capture();
                    this.save.Source = this.camera.ViewImage; // カメラの映像をセット
                    if (this.save.Source == null) break;
                }
                catch
                {
                    MessageBox.Show("カメラが起動できませんでした");
                }
            }
        }

        /// <summary>
        /// カメラを止める
        /// </summary>
        private void StopCapture() => this.isTask = false;

    }
}
