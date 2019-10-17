using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Windows.Media.Imaging;

namespace CodeBehind
{
    public class Camera
    {
        #region フィールド変数
        VideoCapture capture = null;
        Mat frame = null;

        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタでカメラを起動しframeに渡す
        /// </summary>
        public Camera()
        {
            this.capture = new VideoCapture(0);
            if (!this.capture.IsOpened())
                throw new Exception("Camera Initialize failed");
            this.frame = new Mat();
        }
        #endregion

        #region publicメソッド
        /// <summary>
        /// ViewModelに渡すようにWritalbleBitmapでカメラ映像を渡す
        /// </summary>
        /// <returns></returns>
        public WriteableBitmap Capture()
        {
            this.capture.Read(this.frame);
            if (this.frame.Empty()) return null;
            return this.frame.ToWriteableBitmap();
        }
        #endregion

    }
}
