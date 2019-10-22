MVVMの練習
====


## Description
### 

本稿ではMVVMパターンの勉強のため、WPFで簡単なカメラアプリを作成します。  
パターンの勉強として、以下の4パターンで同じような動作をするアプリを目指します。

- コードビハインドに処理を記述
- MVVM支援ライブラリを使用せずにMVVMパターンで記述
- Prismを使用してMVVMパターンで記述
- ReactivePropertyを使用してMVVMパターンで記述

## アプリケーションの全容
下のような画面のカメラアプリを作成します。  
![pic1](https://github.com/pisa-kun/CompareMVVM/blob/gh-pages/Image/Readme/1.png)

- 画面右側のスタックパネルコントロールを表示・非表示にする「オプションボタン」
- 保存するファイル名を決定する「テキストボックス」
- テキストボックスに表示している文字列を反映する「テキストブロック」
- USBカメラを起動して撮影した映像を表示させる「撮影開始ボタン」
- 映像を一旦止める「撮影停止ボタン」
- テキストブロックに表示されている名前で画像を保存する「画像保存ボタン」


![pic2](https://github.com/pisa-kun/CompareMVVM/blob/gh-pages/Image/Readme/2.png)


アプリ起動時はスタックパネルコントロールをVisibleにしており、オプションボタンを押下するとスタックパネルコントロールが非表示になります。

![pic3](https://github.com/pisa-kun/CompareMVVM/blob/gh-pages/Image/Readme/3.png)

撮影開始ボタンを押下すると、画面中にUSBカメラから取得される映像を表示して、
- 撮影開始ボタンが押下できなくなる
- 撮影停止ボタンが押下できる  
状態になります。


![pic4](https://github.com/pisa-kun/CompareMVVM/blob/gh-pages/Image/Readme/4.png)

撮影停止ボタンを押下すると、
- USBカメラの映像を止める
- テキストボックスが空文字("")でなければ、画像保存ボタンが押下できる
- 撮影開始ボタンが押下できる
状態になります。

![pic5](https://github.com/pisa-kun/CompareMVVM/blob/gh-pages/Image/Readme/5.png)

このように撮影停止状態でテキストボックスが空文字だと、画像保存ボタンが押下できません。


![pic6](https://github.com/pisa-kun/CompareMVVM/blob/gh-pages/Image/Readme/6.png)

オプションボタンは撮影状態・停止状態のいつでも押下できます。  

アプリケーションの全容を説明したところで、実際にコードを見ていきましょう。

## コード解説
### 1. コードビハインドに処理を記述

- コードビハインドとは？

まず例として次の.aspxと呼ばれるサーバー側で処理を行ってユーザーにHTMLを返すプログラムを見てみましょう。
``` aspx
<%@ Page Language="C#" %>
<html>
<script runat="server">

protected void button1_OnClick(object sender, EventArgs e){

     label1.Text = textbox1.Text;

}

</script>
<body>
<form runat="server">
<p>
<asp:TextBox runat="server" id="textbox1"/>
<asp:Button runat="server" id="button1" Text="OK" OnClick="button1_OnClick"/><br>
<asp:Label runat="server" id="label1" BackColor="#aaaaaa" Width="150px"/>
</p>
</form>
</body>
</html>
```

button1_OnClick()メソッドはc#で記述され、body以下はHTMLで記述されています。  
OKボタンを押下したら、テキストボックスに記述されている文字列をラベルに反映させる。といった内容です。

このプログラムはよく見てみると、  

> _ボタンやテキストなどの表示部分を構築するHTML_  
> _処理部分を記述するC#_

画面表示のプログラムと処理(ロジック)のプログラムが混在しているプログラムになっています。  
この分量であれば、1つのファイルに記述しても問題ないようにみえますが   
プログラムが大きくなればなるほど、画面表示部分と処理部分の見分けがつかないようになります。

そこでWPFアプリケーションはプロジェクト作成時に
> 表示部分の 〇〇.xaml  
> 処理部分の 〇〇.xaml.cs 

を自動で作ってくれます。  
これにより、UIデザイナーはxamlファイルのみを編集して表示部分を整え、  
プログラマーはボタンが押下された時の処理をcsファイルに記述する分担作業が可能になります。

![pic7](https://github.com/pisa-kun/CompareMVVM/blob/gh-pages/Image/Readme/7.png)

この **〇〇.xaml.cs**を **コードビハインド**といいます。

まず最初に、コードビハインドに処理を記述してアプリケーションを作成していきましょう。

- xamlファイルへの記述

今回作成するアプリケーションのxamlファイルは次のように記述しました。  
コードビハインドで **どのコンポーネントを対象に処理を行うか** を記述する必要があるので、  
各コンポーネントに **x:Name=""** で名前を付けています。  
※ コンポーネント : buttonやlabelやviewboxなどの各パーツのことをコンポーネントと総称しています。

```xaml
<Grid>
        <StackPanel>
            <Viewbox x:Name="view" Width="500" Height="300" Stretch="Fill">
                <Image x:Name="save"/>
            </Viewbox>
            <Button HorizontalAlignment="Left" Width="100" Height="50" Content="オプション" Click="Button_Click"/>
        </StackPanel>
        
        <!--StackPanelは縦方向にコンテントを並べる-->
        <StackPanel x:Name="option" VerticalAlignment="Top" HorizontalAlignment="Right" Width="300"
                    Height="450" Background="Blue" Visibility="Visible">
            <TextBox x:Name="input" TextChanged="Input_TextChanged" />
            <!--WrapPanelは横方向にコンテントを並べる-->
            <WrapPanel>
                <TextBlock x:Name="img" Text="保存ファイル名 : " Foreground="White"/>
                <TextBlock x:Name="name" Foreground="White"/>
            </WrapPanel>
            <WrapPanel>
                <Button x:Name="start" Content="撮影開始" Width="100" Height="50" HorizontalAlignment="Center" Click="Start_Click" />
                <Button x:Name="stop" Content="撮影停止" Width="100" Height="50" HorizontalAlignment="Center" IsEnabled="False" Click="Stop_Click"/>
                <Button x:Name="photo" Content="画像保存" Width="100" Height="50" HorizontalAlignment="Center" IsEnabled="False" Click="Photo_Click" />
            </WrapPanel>
        </StackPanel>
    </Grid>
```

- カメラ処理  
USBカメラから映像を取得するコードは以下のようになります。  
OpenCVSharpをNugetからインストールする必要があるので注意してください。
```C#
### Camera.cs
namespace CodeBehind
{
    using OpenCvSharp;
    using OpenCvSharp.Extensions;
    using System;
    using System.Windows.Media.Imaging;
    public class Camera
    {
        #region フィールド変数
        VideoCapture capture = null;
        Mat frame = null;

        #endregion

        #region コンストラクタ
        public Camera()
        {
            this.capture = new VideoCapture(0);
            if (!this.capture.IsOpened())
                throw new Exception("Camera Initialize failed");
            this.frame = new Mat();
        }
        #endregion

        #region publicメソッド
        public WriteableBitmap Capture()
        {
            this.capture.Read(this.frame);
            if (this.frame.Empty()) return null;
            return this.frame.ToWriteableBitmap();
        }
        #endregion
    }
}

```

- コードビハインド  
コードビハインドの処理はまずオプションボタンの処理を見てみましょう。
```c#
        private void Button_Click(object sender, RoutedEventArgs e) 
            => this.option.Visibility = this.option.Visibility != Visibility.Visible ? Visibility.Visible : Visibility.Hidden;
```
xaml側のソースコードを見てもらえればわかると思いますが、  
オプションボタンのClickイベントに**Button_Click**という名前が割り当てられています。

Button_Clickイベントはワンライナーで書いているので見づらいかもしれませんが処理はシンプルで  
**optionというコンポーネントは、それ自身が表示されていれば非表示にして、非表示であれば表示状態にする**
という処理です。

三項演算子で処理を記述しない場合は以下のような記述になります。

```c#
        private void Button_Click(object sender, RoutedEventArgs e) 
        {
            if(this.option.Visiblity != Visibility.visible)
            {
                this.option.Visibility = Visibility.Visibility;
            }
            else
            {
                this.option.Visibility = Visibility.Hidden;
            }
        }
```

### 2. MVVM支援ライブラリを使用せずにMVVMパターンで記述


### 3. Prismを使用してMVVMパターンで記述

### 4. ReactivePropertyを使用してMVVMパターンで記述


## Author
pisa