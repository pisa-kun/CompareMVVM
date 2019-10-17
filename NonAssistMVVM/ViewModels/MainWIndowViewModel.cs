using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NonAssistMVVM.Notify;

namespace NonAssistMVVM.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        /// <summary>
        /// 保存画像の名前
        /// </summary>
        private string _inputText = null;

        public MainWindowViewModel() { }

        public String InputText
        {
            get => this._inputText;
            set
            {
                SetProperty(ref this._inputText, value); // フィールド変数の値を変更
            }
        }

    }
}
