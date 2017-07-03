using System;

namespace MultiLoader.ConsoleFacade.ProgressReporter
{
    public class ProgressBarTwoLine : IProgressBar
    {
        private int _max;
        private readonly char _character;
        private readonly IConsole _console;
        private int _current = 1;
        private readonly ConsoleColor _c;

        private const string Format = "Item {0,-5} of {1,-5}. ({2,-3}%) ";

        public int Y { get; }

        public string Line1 { get; private set; } = "";

        public string Line2 { get; private set; } = "";

        internal ProgressBarTwoLine(int max, int? textWidth, char character, IConsole console) 
        {
            lock (Locker)
            {
                _console = console;
                Y = _console.CursorTop;
                _c = _console.ForegroundColor;
                _current = 0;
                _max = max;
                _character = character;
                _console.WriteLine("");
                _console.WriteLine("");
            }
        }

        public int Max
        {
            get => _max;
            set
            {
                _max = value;
                Refresh(_current, _item);
            }
        }

        public void Refresh(int current, string format, params object[] args)
        {
            var item = string.Format(format, args);
            Refresh(current, item);
        }

        private static readonly object Locker = new object();

        private string _item = "";

        public void Refresh(int current, string item)
        {
            lock (Locker)
            {
                _item = item;
                var itemText = item ?? "";
                var state = _console.State;
                _current = current;
                try
                {
                    var perc = Max > 0 ? (float) current/(float) _max : 0;
                    var bar = new string(_character, (int) ((float) (_console.WindowWidth - 30)*perc));
                    var line = string.Format(Format, current, _max, (int) (perc*100));
                    var barWhitespace = _console.WindowWidth - (bar.Length + line.Length  + 1);
                    _console.CursorTop = Y;
                    _console.CursorLeft = 0;
                    _console.ForegroundColor = _c;
                    _console.Write(line);
                    _console.ForegroundColor = ConsoleColor.Green;
                    _console.Write(bar);
                    _console.WriteLine(barWhitespace > 0 ? new String(' ', barWhitespace) : "");
                    _console.ForegroundColor = _c;
                    Line2 = itemText.PadRight(_console.WindowWidth - 2);
                    _console.WriteLine(Line2);
                    Line1 = $"{line} {bar}";
                }
                finally
                {
                    _console.State = state;
                }
            }
        }

        public void Next(string item)
        {
            _current++;
            Refresh(_current, item);
        }
    }

}

