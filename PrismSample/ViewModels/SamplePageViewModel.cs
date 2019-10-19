namespace PrismSample.ViewModels
{
    using Prism.Commands;
    using Prism.Mvvm;
    using PrismSample.Models;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    public class SamplePageViewModel : BindableBase
    {
        #region フィールド変数

        /// <summary>
        /// インスタンスを格納
        /// </summary>
        private Camera camera;

        /// <summary>
        /// カメラ映像を表示する変数
        /// </summary>
        private BitmapSource bitmapSource;

        /// <summary>
        /// 保存画像の名前
        /// </summary>
        private string inputText;

        private bool isStart = true;

        private bool isStop = false;

        private bool isPhoto = false;

        private bool isOption = true;

        private bool isTask = true;

        private DelegateCommand startCommand;

        private DelegateCommand stopCommand;

        private DelegateCommand photoCommand;

        private DelegateCommand optionCommand;
        /// <summary>
        /// マイピクチャのフォルダーパス
        /// </summary>
        private static readonly string picPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

        #endregion

        public SamplePageViewModel() { }

        #region プロパティ
        /// <summary>
        /// 
        /// </summary>
        public String InputText
        {
            get => this.inputText;
            set
            {
                SetProperty(ref this.inputText, value);
                IsPhoto = (this.camera != null) && (IsStop == false) && (this.inputText.Length != 0) ? true : false;
            }
        }

        /// <summary>
        /// 画面に表示するBitmap
        /// </summary>
        public BitmapSource BitmapSource
        {
            get => this.bitmapSource;
            set => SetProperty(ref this.bitmapSource, value);
        }

        public bool IsStart
        {
            get => this.isStart;
            set => SetProperty(ref this.isStart, value);
        }

        public bool IsStop
        {
            get => this.isStop;
            set => SetProperty(ref this.isStop, value);
        }

        public bool IsPhoto
        {
            get => this.isPhoto;
            set => SetProperty(ref this.isPhoto, value);
        }

        public bool IsOption
        {
            get => this.isOption;
            set => SetProperty(ref this.isOption, value);
        }

        /// <summary>
        /// StartCommandの処理を記述する
        /// startCommandがnullなら右辺で処理内容を変数に代入する
        /// </summary>
        public DelegateCommand StartCommand => startCommand ?? (startCommand = new DelegateCommand(async () => await StartPreview()));

        /// <summary>
        /// 停止ボタンを押したときのアクション
        /// </summary>
        public DelegateCommand StopCommand => stopCommand ?? (stopCommand = new DelegateCommand(() => StopPreview()));

        /// <summary>
        /// 保存ボタンを押したときのアクション
        /// </summary>
        public DelegateCommand PhotoCommand => photoCommand ?? (photoCommand = new DelegateCommand(() => photo()));

        /// <summary>
        /// オプションボタンを押したときのアクション
        /// </summary>
        public DelegateCommand OptionCommand => optionCommand ?? (optionCommand = new DelegateCommand(() => IsOption = !IsOption));

        #endregion

        /// <summary>
        /// BitmapSourceにModelから取得するプロパティを代入
        /// </summary>
        /// <returns></returns>
        private async Task StartPreview()
        {
            this.camera = this.camera ?? new Camera();
            this.isTask = true;
            IsStart = false;
            IsStop = true;
            IsPhoto = false;
            while (isTask)
            {
                try
                {
                    await this.camera.Capture();
                    BitmapSource = camera.ViewImage; // プロパティにカメラの映像をセット
                }
                catch
                {
                    MessageBox.Show("カメラが起動できませんでした");
                }
            }
        }

        /// <summary>
        /// カメラの映像を止めた時の処理
        /// </summary>
        private void StopPreview()
        {
            this.isTask = false;
            IsStart = true;
            IsStop = false;
            IsPhoto = (this.inputText != null) && (this.inputText.Length != 0) ? true : false;
        }

        /// <summary>
        /// ImageSourceをBitmapにキャストしてfilepathに保存
        /// </summary>
        /// <param name="bitmapSource"></param>
        /// <param name="filePath"></param>
        private void SaveImageSourceToFile(ImageSource imageSource, string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imageSource));
                encoder.Save(fileStream);
            }
        }

        private void photo()
        {
            try
            {
                var filePath = "SampleMVVM";
                var folderPath = System.IO.Path.Combine(picPath, filePath);
                if (!Directory.Exists(folderPath)) // フォルダが存在しないときに作成
                {
                    Directory.CreateDirectory(folderPath);
                }
                var saveName = (System.IO.Path.Combine(folderPath, InputText) + ".png");
                SaveImageSourceToFile(BitmapSource, saveName);
                MessageBox.Show("画像を保存できました。");
            }
            catch
            {
                MessageBox.Show("画像を保存できませんでした。");
            }
        }
    }
}
