using System;

namespace MultiLoader.ConsoleFacade.ProgressReporter
{
    public class ConsoleState
    {
        public bool CursorVisible { get; set; }
        public int Top { get; set; }
        public int Left { get; set; }
        public ConsoleColor ForegroundColor { get; set; }
        public ConsoleColor BackgroundColor { get; set; }

        public ConsoleState(ConsoleColor foreground, ConsoleColor background, int top, int left, bool cursorVisible)
        {
            ForegroundColor = foreground;
            BackgroundColor = background;
            Top = top;
            Left = left;
            CursorVisible = cursorVisible;
        }
    }
}