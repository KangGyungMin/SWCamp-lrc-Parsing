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
using System.Windows.Threading;

namespace Presto.SWCamp.Lyrics
{
    /// <summary>
    /// LyricsWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LyricsWindow : Window
    {
        SortedList<double, string> _lyrics = new SortedList<double, string>();

        public LyricsWindow()
        {
            InitializeComponent();
            PrestoSDK.PrestoService.Player.StreamChanged += Player_StreamChanged;
        }

        private void Player_StreamChanged(object sender, Common.StreamChangedEventArgs e)
        {
            _lyrics.Clear();
            textLyrics.Text = null;
            var fileName = PrestoSDK.PrestoService.Player.CurrentMusic.Path;
            var lrcName = Path.GetFileNameWithoutExtension(fileName) + ".lrc";
            var path = Path.Combine(Path.GetDirectoryName(fileName), lrcName);
            var lines = File.ReadAllLines(path);

            for (int i = 3; i < lines.Length; i++)
            {
                var splitData = lines[i];
                var time = TimeSpan.ParseExact(splitData.Substring(1,8).Trim(), @"mm\:ss\.ff", CultureInfo.InvariantCulture);
              
                _lyrics.Add(time.TotalMilliseconds, splitData.Substring(10));

                //textLyrics.Text = time.ToString();
                //MessageBox.Show(_lyrics.Keys[i-3].TotalMilliseconds.ToString());
            }

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var current = PrestoSDK.PrestoService.Player.Position;
            List<double> bin = _lyrics.Keys.ToList();
            var BS = bin.BinarySearch(current);
            BS += 1;
            //MessageBox.Show(BS.ToString());
            if (BS <= 0)
            {
                BS = ~BS;
                if (BS >= 0 && BS < bin.Count)
                {
                    textLyrics.Text = _lyrics.Values[BS];
                }
                else
                {
                    textLyrics.Text = "가사 준비";
                }
            }
        }
    }
}