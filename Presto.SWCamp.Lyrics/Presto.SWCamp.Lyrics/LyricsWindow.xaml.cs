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
        SortedList<TimeSpan, string> _lyrics = new SortedList<TimeSpan, string>();

        public LyricsWindow()
        {
            InitializeComponent();
            PrestoSDK.PrestoService.Player.StreamChanged += Player_StreamChanged;
        }

        private void Player_StreamChanged(object sender, Common.StreamChangedEventArgs e)
        {
            SortedList<TimeSpan, string> _lyrics = new SortedList<TimeSpan, string>();
            textLyrics.Text = null;
            var fileName = PrestoSDK.PrestoService.Player.CurrentMusic.Path;
            var lrcName = Path.GetFileNameWithoutExtension(fileName) + ".lrc";
            var path = Path.Combine(Path.GetDirectoryName(fileName), lrcName);
            var lines = File.ReadAllLines(path);

            for (int i = 3; i < lines.Length; i++)
            {
                var splitData = lines[i].Split(']');
                var time = TimeSpan.ParseExact(splitData[0].Substring(1).Trim(), @"mm\:ss\.ff", CultureInfo.InvariantCulture);
              
                _lyrics.Add(time, splitData[1]);

                //textLyrics.Text = time.ToString();
                //MessageBox.Show(_lyrics.Keys[i-3].TotalMilliseconds.ToString());
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
            for (int i = 0; i < _lyrics.Count; i++)
            {
                if (_lyrics.Keys[i].TotalMilliseconds >= PrestoSDK.PrestoService.Player.Position && _lyrics.Keys[Math.Min(_lyrics.Count-1,i+1)].TotalMilliseconds <= PrestoSDK.PrestoService.Player.Position)
                {
                    textLyrics.Text = _lyrics.Values[i];
                }
            }

            // times[i] <> PrestoSDK.PrestoService.Player.Position

        }
    }
}