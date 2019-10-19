namespace ReactiveProperty.ViewModels
{
    using ReactiveProperty.Command;
    using ReactiveProperty.Models;
    using ReactiveProperty.Notify;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Reactive.Bindings;
    using Reactive.Bindings.Extensions;
    using System.Reactive.Linq;

    public class MainWindowViewModel : BindableBase
    {
        #region フィールド変数

        /// <summary>
        /// インスタンスを格納
        /// </summary>
        private Camera camera;

        /// <summary>
        /// マイピクチャのフォルダーパス
        /// </summary>
        private static readonly string picPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

        #endregion

        /// <summary>
        /// コンストラクタで処理を購読
        /// </summary>
        public MainWindowViewModel()
        {
            StartCommand.Subscribe(async() => await StartPreview());
            StopCommand.Subscribe(_ => StopPreview());
            PhotoCommand = IsStop.ObserveHasErrors.CombineLatest(
                InputText.ObserveHasErrors, (x, y) => !x && !y).ToReactiveCommand();
            OptionCommand.Subscribe(_ => IsOption.Value = !IsOption.Value);
            PhotoCommand.Subscribe(_ => Photo());
            IsPhoto = InputText.Select(x => x.Length != 0).ToReactiveProperty();
        }

        #region プロパティ
        public ReactiveProperty<BitmapSource> BitmapSource { get; private set; } = new ReactiveProperty<BitmapSource>();

        public ReactiveProperty<string> InputText { get; private set;} = new ReactiveProperty<string>("");

        public ReactiveProperty<bool> IsStart { get; private set; } = new ReactiveProperty<bool>(true);

        public ReactiveProperty<bool> IsStop { get; private set; } = new ReactiveProperty<bool>(false);

        public ReactiveProperty<bool> IsPhoto { get; private set; }

        public ReactiveProperty<bool> IsOption { get; private set; } = new ReactiveProperty<bool>(true);

        public ReactiveProperty<bool> IsTask { get; private set; } = new ReactiveProperty<bool>(true);

        public ReactiveCommand StartCommand { get; private set; } = new ReactiveCommand();

        public ReactiveCommand StopCommand { get; private set; } = new ReactiveCommand();

        public ReactiveCommand PhotoCommand { get; private set; } = new ReactiveCommand();
        public ReactiveCommand OptionCommand { get; private set; } = new ReactiveCommand();

        #endregion

        /// <summary>
        /// BitmapSourceにModelから取得するプロパティを代入
        /// </summary>
        /// <returns></returns>
        private async Task StartPreview()
        {
            this.camera = this.camera ?? new Camera();
            IsTask.Value = true;
            IsStart.Value = false;
            IsStop.Value = true;
            IsPhoto.Value = false;
            while (IsTask.Value)
            {
                try
                {
                    await this.camera.Capture();
                    BitmapSource.Value = camera.ViewImage.Value; // プロパティにカメラの映像をセット
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
            IsTask.Value = false;
            IsStart.Value = true;
            IsStop.Value = false;
            IsPhoto.Value = (InputText != null) && (InputText.Value != "") ? true : false;
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

        private void Photo()
        {
            try
            {
                var filePath = "SampleMVVM";
                var folderPath = System.IO.Path.Combine(picPath, filePath);
                if (!Directory.Exists(folderPath)) // フォルダが存在しないときに作成
                {
                    Directory.CreateDirectory(folderPath);
                }
                var saveName = (System.IO.Path.Combine(folderPath, InputText.Value) + ".png");
                SaveImageSourceToFile(BitmapSource.Value, saveName);
                MessageBox.Show("画像を保存できました。");
            }
            catch
            {
                MessageBox.Show("画像を保存できませんでした。");
            }
        }
    }
}
