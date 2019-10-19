namespace ReactiveProperty.Command
{
    using System;
    using System.Windows.Input;


    /// <summary>
    /// viewにActionを渡すクラス
    /// </summary>
    public class DelegateCommand : ICommand
    {
        //Command実行時に実行するアクション、引数を受け取りたい場合はこのActionをAction<object>などにする
        private Action _action;

        public DelegateCommand(Action action)
        {//コンストラクタでActionを登録
            _action = action;
        }

        #region ICommandインターフェースの必須実装

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {//とりあえずActionがあれば実行可能
            return _action != null;
        }

        public void Execute(object parameter)
        {//今回は引数を使わずActionを実行
            _action?.Invoke();
        }

        #endregion
    }
}
