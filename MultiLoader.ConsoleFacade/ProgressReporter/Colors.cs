using System;

namespace MultiLoader.ConsoleFacade.ProgressReporter
{
    public class Colors
    {
        public ConsoleColor Foreground { get; }
        public ConsoleColor Background { get; } 

        public Colors()
        {
            Foreground = ConsoleColor.White;
            Background = ConsoleColor.Black;
        }

        public Colors(ConsoleColor foreground, ConsoleColor background)
        {
            Foreground = foreground;
            Background = background;
        }
    }
}
