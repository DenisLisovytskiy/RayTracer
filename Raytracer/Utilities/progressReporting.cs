using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.Utilities
{
    public class ProgressReporter
    {
        private readonly object _lock = new(); // Lock for thread safety
        private readonly int _barSize;
        private Tuple<int, WorkerData> worker;
        
        
        private int imageWidth;
        private int imageHeight;
        private string imageName;
        private int timeLeft = 0;
        private int estimatedTime = 0;
        public int elapsedTime;
        private double aspectRatio;
        Stopwatch _stopwatch;
        public ProgressReporter(int _imageWidth, string _imageName, double _aspectRatio)
        {
            Console.Clear();
            Console.CursorVisible = false;
            _barSize = 50;
            worker = new Tuple<int, WorkerData>(0, new WorkerData());
            imageWidth = _imageWidth;
            aspectRatio = _aspectRatio;
            imageHeight = (int)(imageWidth / aspectRatio);
            imageHeight = Math.Max(1, imageHeight); // Prevent zero or negative values
            imageName = _imageName;
            elapsedTime = 0;
            _stopwatch = Stopwatch.StartNew();
            Redraw();

        }

        public void UpdateWorkerProgress(int _linesFinished, string message = "")
        {
            lock (_lock)
            {
                // Update progress considering finished commands
                worker.Item2.linesFinished += _linesFinished;
                worker.Item2.Progress = (double)worker.Item2.linesFinished * 100/ (double)imageHeight;
                worker.Item2.Message = message;
                elapsedTime = (int)(_stopwatch.ElapsedMilliseconds / 1000);
                estimatedTime = (int)(elapsedTime * 100 / worker.Item2.Progress);
                timeLeft = estimatedTime - elapsedTime ;
                if (_linesFinished == imageHeight)
                { 
                    _stopwatch.Stop();
                    timeLeft = 0;
                }
                Redraw();
            }
        }

        private void Redraw()
        {
            // Redraw all progress bars safely
            lock (_lock)
            {
                Console.SetCursorPosition(0, 0);
                var sb = new StringBuilder();
                sb.AppendLine($"Generating image: {imageName} ");
                sb.AppendLine($"of size {imageWidth}x{imageHeight} pixels ...");
                var progressBar = DrawProgressBar(worker.Item2.Progress, 100, _barSize);
                sb.AppendLine($" {progressBar}  {worker.Item2.Message}");

                sb.AppendLine($"Estimated time left: {timeLeft} seconds ");

                Console.Write(sb.ToString());
            }
        }

        private string DrawProgressBar(double progress, int total, int barSize)
        {
            double percentage = progress / total;
            int filled = (int)Math.Round(percentage * barSize);

            char[] barChars = new char[barSize];
            for (int i = 0; i < barSize; i++)
            {
                barChars[i] = i < filled ? '#' : '-';
            }

            // Add dividers for commands

            string progressBar = $"[{new string(barChars)}]";
            string percentageText = $"{progress.ToString("F3")}%";

            return $"{progressBar} {percentageText}";
        }
    }

    public record WorkerData
    {
        public double Progress { get; set; } = 0;
        public int linesFinished { get; set; } = 0;
        public int CommandsFinished { get; set; } = 0;
        public string Message { get; set; } = "";
    }
}
