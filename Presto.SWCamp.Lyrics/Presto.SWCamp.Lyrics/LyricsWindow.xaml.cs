using Presto.SDK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Presto.SWCamp.Lyrics
{
    /// <summary>
    /// LyricsWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LyricsWindow : Window
    {
        SortedList<TimeSpan, string> _lyrics = new SortedList<TimeSpan, string>();
        public LyricsWindow()
        {
            InitializeComponent();

            string[] lines = File.ReadAllLines(@"C:\Users\Kang\Desktop\Presto.Lyrics.Sample\Musics\볼빨간사춘기 - 여행.lrc");
            for(int i = 3; i<=lines.Length; i++)
            {
                var splitData = lines[i].Split(']');
                var time = TimeSpan.ParseExact(splitData[0].Substring(1).Trim(), 
                    @"mm\:ss\.ff", CultureInfo.InvariantCulture);
                MessageBox.Show(splitData[1].Substring(0));
                //MessageBox.Show(time.TotalMilliseconds.ToString());
            }
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            timer.Tick += Timer_Tick;
            timer.Start();

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            textLyrics.Text = PrestoSDK.PrestoService.Player.Position.ToString();
        }
    }
}
