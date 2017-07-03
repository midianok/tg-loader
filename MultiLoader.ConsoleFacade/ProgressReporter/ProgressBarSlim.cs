using System;

namespace MultiLoader.ConsoleFacade.ProgressReporter
{

    public class ProgressBarSlim : IProgressBar
    {
        private int _max;
        private readonly char _character;
        private readonly IConsole _console;
        private readonly ConsoleColor _c;
        private static readonly object Locker = new object();

        private string _item = "";

        public int TextWidth { get; } = 30;

        public int Y { get; }

        [Obsolete("Not used in this 'slim' 1 liner implementation of a progress bar.")]
        public string Line2 => "";

        /// <summary>
        /// the resulting rendered text line, e.g. "MyFiles.zip : ( 50%) ########"
        /// </summary>
        public string Line1 { get; private set; } = "";

        /// <summary>
        /// this is the item that the progressbar represents, e.g, "MyFiles.zip"
        /// </summary>
        public string Item
        {
            get => _item;
            set
            {
                _item = value;
                Refresh(Current, Item);
            }
        }

        public ProgressBarSlim(int max)                                  : this(max, '#', new Writer()) { }
        public ProgressBarSlim(int max, int textWidth)                   : this(max, textWidth,'#', new Writer()) { }
        public ProgressBarSlim(int max, int textWidth, char character)   : this(max, textWidth, character, new Writer()) { }
        public ProgressBarSlim(int max, IConsole console)                : this(max, null, '#', console) { }
        public ProgressBarSlim(int max, int textWidth, IConsole console) : this(max, textWidth, '#', console) { }

        public ProgressBarSlim(int max, int? textWidth, char character, IConsole console)
        {
            lock (Locker)
            {
                TextWidth = GetTextWidth(console, textWidth);
                _console = console;
                Y = _console.CursorTop;
                _c = _console.ForegroundColor;
                Current = 0;
                _max = max;
                _character = character;
                _console.WriteLine("");
            }
        }

        internal static int GetTextWidth(IConsole console, int? width)
        {
            return width.HasValue
                ? width.Value
                : console.WindowWidth/4;
        }

        public int Max
        {
            get => _max;
            set
            {
                _max = value;
                Refresh(Current, _item);
            }
        }

        public int Current { get; private set; } = 0;

        public void Refresh(int current, string format, params object[] args)
        {
            var text = string.Format(format, args);
            Refresh(current, text);
        }


        public void Refresh(int current, string itemText)
        {
            var item = itemText ?? "";
            var clippedText = item.FixLeft(TextWidth);
            lock (Locker)
            {                
                _item = item;
                var state = _console.State;
                Current = current.Max(Max);
                try
                {
                    var perc = (float) Current/(float) _max;
                    var barWidth = _console.WindowWidth - (TextWidth+8);
                    var bar = Current > 0
                        ? new string(_character, (int) ((float) (barWidth)*perc)).PadRight(barWidth)
                        : new string(' ', barWidth);
                    var text = string.Format("{0} ({1,-3}%) ", clippedText, (int) (perc*100));
                    _console.CursorTop = Y;
                    _console.CursorLeft = 0; 
                    _console.ForegroundColor = _c;
                    _console.Write(text);
                    _console.ForegroundColor = ConsoleColor.Green;
                    _console.Write(bar);
                    Line1 = $"{text} {bar}";
                }
                finally
                {
                    _console.State = state;
                }
            }
        }

        public void Next(string item)
        {
            Current++;
            Refresh(Current, item);
        }
    }

}

