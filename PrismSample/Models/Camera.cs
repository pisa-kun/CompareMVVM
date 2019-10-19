namespace PrismSample.Models
{
    using OpenCvSharp;
    using OpenCvSharp.Extensions;
    using System;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;
    public class Camera
    {
        #region フィールド変数
        private VideoCapture capture = null;
        private Mat frame = null;

        #endregion

        #region プロパティ
        public WriteableBitmap ViewImage { get; private set; }
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
            ViewImage = frame.ToWriteableBitmap();
        }
        #endregion

    }
}
