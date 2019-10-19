namespace ReactiveProperty.Models
{
    using OpenCvSharp;
    using OpenCvSharp.Extensions;
    using System;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;
    using Reactive.Bindings;
    using Reactive.Bindings.Extensions;

    public class Camera
    {
        #region フィールド変数
        private VideoCapture capture = null;
        private Mat frame = null;

        #endregion

        #region プロパティ
        /// <summary>
        /// ReactivePropertyクラスとしてnew演算子で生成
        /// </summary>
        public ReactiveProperty<BitmapSource> ViewImage { get; private set; } = new ReactiveProperty<BitmapSource>();
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタでカメラを起動しframeに渡す
        /// </summary>
        public Camera()
        {
            capture = new VideoCapture(0);
            if (!capture.IsOpened())
                throw new Exception("Camera Initialize failed");
            frame = new Mat();
        }
        #endregion

        #region publicメソッド
        /// <summary>
        /// ViewModelに渡すようにWritalbleBitmapでカメラ映像を渡す
        /// </summary>
        /// <returns></returns>
        public async Task Capture()
        {
            await Task.Delay(30);
            capture.Read(frame);
            ViewImage.Value = frame.ToWriteableBitmap(); // hoge.ValueでT型の値をGetする
        }
        #endregion

    }
}
