using System;
using System.Collections.Generic;
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
using System.Net.NetworkInformation;

namespace PingAsyncWPF
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //ボタンのEnableの設定
            pingSendAsync.IsEnabled = true;
            pingCancelAsync.IsEnabled = false;

            //デフォルトのURL"www.yahoo.com"
            inputTextbox.Text = "www.yahoo.com";
        }

        //Pingのインスタンス化
        Ping mainPing = null;

        private void SendPingAsync_Button_Click(object sender, RoutedEventArgs e)
        {
            pingSendAsync.IsEnabled = false;
            pingCancelAsync.IsEnabled = true;

            
            //非同期Pingインスタンスの生成
            if(mainPing == null)
            {
                mainPing = new Ping();
                //イベントハンドラ追加
                mainPing.PingCompleted +=
                    new PingCompletedEventHandler(Ping_PingCompleted);

            }

            //Pingのオプションの設定
            //TTLを64,フラグメンテーションを無効にする
            PingOptions opts = new PingOptions(64, true);

            //Pingを送信する32バイトのデータを作成
            byte[] bs = Encoding.ASCII.GetBytes(new String('A', 32));

            //www.yahoo.comにpingを送信する
            //タイムアウトを10秒
            mainPing.SendAsync(inputTextbox.Text, 10000, bs, opts, null);
        }

        private void CancelPingAsync_Button_Click(object sender, RoutedEventArgs e)
        {
            //Pingをキャンセル
            if(mainPing != null)
            {
                mainPing.SendAsyncCancel();
            }
        }

        //Ping_PingCompletedイベントハンドラ
        private void  Ping_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            if(e.Cancelled)
            {
                Console.WriteLine("Pingがキャンセルされました");
            }
            else if(e.Error != null)
            {
                Console.WriteLine("エラー：" + e.Error.Message);
            }
            else
            {
                //結果を取得
                if(e.Reply.Status == IPStatus.Success)
                {
                    Console.WriteLine($"Reply from {0}:bytes={1} time={2}ms TTL={3}",
                        e.Reply.Address, e.Reply.Buffer.Length,
                        e.Reply.RoundtripTime, e.Reply.Options.Ttl);
                }
                else
                {
                    Console.WriteLine($"Ping送信に失敗。({0})", e.Reply.Status);
                }      
            }

            pingSendAsync.IsEnabled = true;
            pingCancelAsync.IsEnabled = false;
        }

        private void Input_URL_textChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            if(inputTextbox.Text == "")
            {
                pingSendAsync.IsEnabled=false;
            }
            else
            {
                pingSendAsync.IsEnabled=true;
            }
        }
    }
}
