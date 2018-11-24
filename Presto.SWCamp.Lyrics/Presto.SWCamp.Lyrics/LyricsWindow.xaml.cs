using Presto.SDK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            PrestoSDK.PrestoService.Player.StreamChanged += Player_StreamChanged; // When music changes
        }

        private void Player_StreamChanged(object sender, Common.StreamChangedEventArgs e)
        {
            _lyrics.Clear(); // SortedList Clear
            var fileName = PrestoSDK.PrestoService.Player.CurrentMusic.Path;
            var lrcName = Path.GetFileNameWithoutExtension(fileName) + ".lrc";
            var path = Path.Combine(Path.GetDirectoryName(fileName), lrcName);
            var lines = File.ReadAllLines(path);
            String Data2 = null;

            for (int i = 3; i < lines.Length; i++)
            {
                var Data = lines[i];
                var time = TimeSpan.ParseExact(Data.Substring(1,8).Trim(), @"mm\:ss\.ff", CultureInfo.InvariantCulture);

                if (_lyrics.ContainsKey(time.TotalMilliseconds))
                {
                    _lyrics.Remove(time.TotalMilliseconds);
                    Data2 = Data2 + '\n' + Data.Substring(10);
                    _lyrics.Add(time.TotalMilliseconds, Data2.Substring(10));
                }
                else
                {
                    _lyrics.Add(time.TotalMilliseconds, Data.Substring(10));
                    Data2 = Data.Substring(10);
                }
                
            }

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var current = PrestoSDK.PrestoService.Player.Position;
            List<double> bin = _lyrics.Keys.ToList(); // SortedList -> List // for BinarySearch 
            var BS = bin.BinarySearch(current);
            BS += 1;
            if (BS <= 0)
            {
                BS = ~BS;
                if (BS >= 0 && BS < bin.Count)
                {
                    textLyrics.Text = _lyrics.Values[BS];
                }
                else
                {
                    textLyrics.Text = "가사 준비중";
                }
            }
        }
    }
}