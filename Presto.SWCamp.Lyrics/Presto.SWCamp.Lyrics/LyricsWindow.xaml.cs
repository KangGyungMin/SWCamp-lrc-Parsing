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

/*
 * String[] lines = File.ReadAllLines("@"");
 * foreach(var line in lines)
 * {
 * var datas = line.split(']');
 * messagebox.show(line);
 * }
 */

namespace Presto.SWCamp.Lyrics
{
    /// <summary>
    /// LyricsWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LyricsWindow : Window
    {


        public LyricsWindow()
        {
            InitializeComponent();
            PrestoSDK.PrestoService.Player.StreamChanged += Player_StreamChanged;
        }

        private void Player_StreamChanged(object sender, Common.StreamChangedEventArgs e)
        {
            textLyrics.Text = null;
            var fileName = PrestoSDK.PrestoService.Player.CurrentMusic.Path;
            var lrcName = Path.GetFileNameWithoutExtension(fileName) + ".lrc";
            var path = Path.Combine(Path.GetDirectoryName(fileName), lrcName);
            //MessageBox.Show(path.ToString());
            var lines = File.ReadAllLines(path);
            for (int i = 3; i < lines.Length; i++)
            {
                var splitData = lines[i].Split(']');
                var time = TimeSpan.ParseExact(splitData[0].Substring(1).Trim(), @"mm\:ss\.ff", CultureInfo.InvariantCulture);
                var timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(100)
                };
                timer.Tick += Timer_Tick;
                //textLyrics.Text = time.ToString() + splitData[1] + '\n';
                timer.Start();
                
                //textLyrics.Text = time.ToString();
                //MessageBox.Show(time.TotalMilliseconds.ToString());
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            textLyrics.Text = PrestoSDK.PrestoService.Player.Position.ToString();
        }
    }
}