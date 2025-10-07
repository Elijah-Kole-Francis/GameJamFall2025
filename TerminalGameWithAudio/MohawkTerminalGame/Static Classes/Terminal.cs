using System;
using System.Threading;

namespace MohawkTerminalGame
{
    /// <summary>
    ///     A utility class built on top of <see cref="Console"/>.
    /// </summary>
    public static class Terminal
    {
        private static bool isCursorVisible = true;

        private delegate void WriteAction(string value);
        private delegate void WriteObjAction(object obj);
        private static WriteAction BasicWrite => UseRoboType ? RoboWrite : Console.Write;
        private static WriteAction BasicWriteLine => UseRoboType ? RoboWriteLine : Console.WriteLine;
        private static WriteObjAction BasicWriteObj => UseRoboType ? RoboWrite : Console.Write;
        private static WriteObjAction BasicWriteLineObj => UseRoboType ? RoboWriteLine : Console.WriteLine;
        private static WriteAction OnWrite => WriteWithWordBreaks ? FancyWrite : BasicWrite;
        private static WriteAction OnWriteLine => WriteWithWordBreaks ? FancyWriteLine : BasicWriteLine;
        private static WriteObjAction OnWriteObj => WriteWithWordBreaks ? FancyWrite : BasicWriteObj;
        private static WriteObjAction OnWriteLineObj => WriteWithWordBreaks ? FancyWriteLine : BasicWriteLineObj;


        /// <summary>
        ///     Gets or sets the background color of the console.
        /// </summary>
        public static ConsoleColor BackgroundColor
        {
            get => Console.BackgroundColor;
            set => Console.BackgroundColor = value;
        }

        /// <summary>
        ///     The cursor position in columns from the left edge of the console window.
        /// </summary>
        public static int CursorLeft
        {
            get => Console.CursorLeft;
            set => Console.CursorLeft = value;
        }

        /// <summary>
        ///     The cursor position in rows from the top edge of the console window.
        /// </summary>
        public static int CursorTop
        {
            get => Console.CursorTop;
            set => Console.CursorTop = value;
        }

        /// <summary>
        ///     Whether the cursor is visible or not. Can be set.
        /// </summary>
        public static bool CursorVisible
        {
            get => isCursorVisible;
            set => isCursorVisible = Console.CursorVisible = value;
        }

        /// <summary>
        ///     Gets or sets the foreground color of the console.
        /// </summary>
        public static ConsoleColor ForegroundColor
        {
            get => Console.ForegroundColor;
            set => Console.ForegroundColor = value;
        }

        /// <summary>
        ///     True is Caps Lock key is on.
        /// </summary>
        public static bool IsCapsLockOn => Console.CapsLock;

        /// <summary>
        ///     True if Num Lock key is on.
        /// </summary>
        public static bool IsNumLockOn => Console.NumberLock;

        /// <summary>
        ///     The maximum number of rows the window can be on the current display.
        /// </summary>
        public static int LargestWindowHeight => Console.LargestWindowHeight;

        /// <summary>
        ///     The maximum number of columns the window can be on the current display.
        /// </summary>
        public static int LargestWindowWidth => Console.LargestWindowWidth;

        /// <summary>
        ///     The interval in milliseconds between character writes to the screen.
        /// </summary>
        public static int RoboTypeIntervalMilliseconds { get; set; } = 50;

        /// <summary>
        ///     Makes Terminal.Write and .WriteLine write each character one by one
        ///     using the timing defined in <see cref="RoboTypeIntervalMilliseconds"/>.
        /// </summary>
        public static bool UseRoboType { get; set; } = false;

        /// <summary>
        ///     The current number of rows the window height is.
        public static int WindowHeight
        {
            get => Console.WindowHeight;
            set => Console.WindowHeight = value;
        }

        /// <summary>
        ///     The current number of columns the window width is.
        /// </summary>
        public static int WindowWidth
        {
            get => Console.WindowWidth;
            set => Console.WindowWidth = value;
        }

        /// <summary>
        ///     Terminal.Write and .WriteLine functions will put words on new line rather
        ///     than wrapping by breaking within a word.
        /// </summary>
        public static bool WriteWithWordBreaks { get; set; } = false;

        /// <summary>
        ///     The character to use for word breaks.
        /// </summary>
        public static char WordBreakCharacter { get; set; } = ' ';


        /// <summary>
        ///     Plays the sound of a beep through the console speaker.
        /// </summary>
        public static void Beep()
            => Console.Beep();

        /// <summary>
        ///     Clear the current screen.
        /// </summary>
        public static void Clear()
            => Console.Clear();

        /// <summary>
        ///     Clears the current line without advancing to the next line.
        /// </summary>
        public static void ClearLine()
        {
            CursorLeft = 0;
            char[] spaces = new char[WindowWidth];
            for (int i = 0; i < spaces.Length; i++)
                spaces[i] = ' ';
            Console.Write(spaces);
            CursorLeft = 0;
        }

        /// <summary>
        ///     Delays the terminal by <paramref name="milliseconds"/>.
        /// </summary>
        /// <param name="milliseconds">The amount of time to delay by.</param>
        public static void Delay(int milliseconds)
            => Thread.Sleep(milliseconds);

        /// <summary>
        ///     Reads the next line of text, then clears that line without advancing to a new line.
        /// </summary>
        /// <returns>
        ///     String input from user.
        /// </returns>
        public static string ReadAndClearLine()
        {
            string value = ReadLine();
            SetCursorPosition(0, Math.Clamp(CursorTop - 1, 0, WindowWidth));
            ClearLine();
            return value;
        }

        /// <summary>
        ///     Reads the next line of text.
        /// </summary>
        /// <returns>
        ///     String input from user.
        /// </returns>
        public static string ReadLine()
            => Console.ReadLine() ?? string.Empty;

        /// <summary>
        ///     Sets foreground and background colors to their defaults.
        /// </summary>
        public static void ResetColor()
            => Console.ResetColor();

        /// <summary>
        ///     Sets the cursor position on the current screen view.
        /// </summary>
        /// <param name="left">The column position of the cursor.</param>
        /// <param name="top">The row position of the cursor.</param>
        public static void SetCursorPosition(int left, int top)
            => Console.SetCursorPosition(left, top);

        /// <summary>
        ///     Set console window title.
        /// </summary>
        /// <param name="title">The title to use.</param>
        public static void SetTitle(string title)
            => Console.Title = title;

        /// <summary>
        ///     Set the window size in rows/columns.
        /// </summary>
        /// <param name="columnsWidth">The number of columns.</param>
        /// <param name="rowsHeight">The number of rows.</param>
        public static void SetWindowSize(int columnsWidth, int rowsHeight)
        {
            int x = Math.Clamp(columnsWidth, 1, LargestWindowWidth);
            int y = Math.Clamp(rowsHeight, 1, LargestWindowHeight);
            Console.SetWindowSize(x, y);
        }


        // "Auto-type" looking feature.
        private static void RoboWrite(string value)
        {
            char[] chars = value.ToCharArray();
            foreach (char @char in chars)
            {
                Console.Write(@char);
                Thread.Sleep(RoboTypeIntervalMilliseconds);
            }
        }
        private static void RoboWriteLine(string value)
        {
            RoboWrite(value);
            Console.Write('\n');
            Thread.Sleep(RoboTypeIntervalMilliseconds);
        }
        private static void RoboWrite(object obj) => RoboWrite(obj.ToString() ?? string.Empty);
        private static void RoboWriteLine(object obj) => RoboWriteLine(obj.ToString() ?? string.Empty);

        // Word break writting
        private static void FancyWrite(string value)
        {
            string[] elements = value.Split(WordBreakCharacter);
            for (int i = 0; i < elements.Length; i++)
            {
                var element = elements[i];
                int remainingLineCharacters = WindowWidth - CursorLeft - 1;
                // If no room, new line
                bool canFit = element.Length < remainingLineCharacters;
                if (!canFit)
                    Console.WriteLine();
                // A. Print as usual, or
                // B. falls through and still doesn't fit on its own, print anyways
                BasicWrite(element);
                // Add space where removed
                if (i < elements.Length - 1)
                    BasicWriteObj(WordBreakCharacter);
            }
        }
        private static void FancyWrite(object obj) => FancyWrite(obj.ToString() ?? string.Empty);
        private static void FancyWriteLine(string value)
        {
            FancyWrite(value);
            Console.WriteLine();
        }
        private static void FancyWriteLine(object obj) => FancyWriteLine(obj.ToString() ?? string.Empty);

        // Terminal writing but with color support
        private static void Write(Action consoleWrite, ConsoleColor foregroundColor)
        {
            var fgColor = Console.ForegroundColor;
            Console.ForegroundColor = foregroundColor;
            consoleWrite.Invoke();
            Console.ForegroundColor = fgColor;
        }
        private static void Write(Action consoleWrite, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            var fgColor = Console.ForegroundColor;
            var bgColor = Console.BackgroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            consoleWrite.Invoke();
            Console.ForegroundColor = fgColor;
            Console.BackgroundColor = bgColor;
        }

        // Write
        public static void Write(string value) => OnWrite(value);
        public static void Write(ulong value) => OnWriteObj(value);
        public static void Write(uint value) => OnWriteObj(value);
        public static void Write(object value) => OnWriteObj(value);
        public static void Write(float value) => OnWriteObj(value);
        public static void Write(char value) => OnWriteObj(value);
        public static void Write(char[] value) => OnWriteObj(value);
        public static void Write(bool value) => OnWriteObj(value);
        public static void Write(double value) => OnWriteObj(value);
        public static void Write(int value) => OnWriteObj(value);
        public static void Write(long value) => OnWriteObj(value);
        public static void Write(decimal value) => OnWriteObj(value);

        // WriteLine
        public static void WriteLine() => Console.WriteLine();
        public static void WriteLine(string value) => OnWriteLine(value);
        public static void WriteLine(ulong value) => OnWriteLineObj(value);
        public static void WriteLine(uint value) => OnWriteLineObj(value);
        public static void WriteLine(object value) => OnWriteLineObj(value);
        public static void WriteLine(float value) => OnWriteLineObj(value);
        public static void WriteLine(char value) => OnWriteLineObj(value);
        public static void WriteLine(char[] value) => OnWriteLineObj(value);
        public static void WriteLine(bool value) => OnWriteLineObj(value);
        public static void WriteLine(double value) => OnWriteLineObj(value);
        public static void WriteLine(int value) => OnWriteLineObj(value);
        public static void WriteLine(long value) => OnWriteLineObj(value);
        public static void WriteLine(decimal value) => OnWriteLineObj(value);

        // Write with console color fg
        public static void Write(string value, ConsoleColor foregroundColor) => Write(() => OnWrite(value), foregroundColor);
        public static void Write(object value, ConsoleColor foregroundColor) => Write(() => OnWriteObj(value), foregroundColor);
        public static void Write(char[] value, ConsoleColor foregroundColor) => Write(() => OnWriteObj(value), foregroundColor);
        public static void Write(char value, ConsoleColor foregroundColor) => Write(() => OnWriteObj(value), foregroundColor);
        public static void Write(bool value, ConsoleColor foregroundColor) => Write(() => OnWriteObj(value), foregroundColor);
        public static void Write(int value, ConsoleColor foregroundColor) => Write(() => OnWriteObj(value), foregroundColor);
        public static void Write(long value, ConsoleColor foregroundColor) => Write(() => OnWriteObj(value), foregroundColor);
        public static void Write(uint value, ConsoleColor foregroundColor) => Write(() => OnWriteObj(value), foregroundColor);
        public static void Write(ulong value, ConsoleColor foregroundColor) => Write(() => OnWriteObj(value), foregroundColor);
        public static void Write(float value, ConsoleColor foregroundColor) => Write(() => OnWriteObj(value), foregroundColor);
        public static void Write(double value, ConsoleColor foregroundColor) => Write(() => OnWriteObj(value), foregroundColor);
        public static void Write(string value, ConsoleColor foregroundColor, ConsoleColor backgroundColor) => Write(() => OnWrite(value), foregroundColor, backgroundColor);
        public static void Write(object value, ConsoleColor foregroundColor, ConsoleColor backgroundColor) => Write(() => OnWriteObj(value), foregroundColor, backgroundColor);
        public static void Write(char[] value, ConsoleColor foregroundColor, ConsoleColor backgroundColor) => Write(() => OnWriteObj(value), foregroundColor, backgroundColor);
        public static void Write(char value, ConsoleColor foregroundColor, ConsoleColor backgroundColor) => Write(() => OnWriteObj(value), foregroundColor, backgroundColor);
        public static void Write(bool value, ConsoleColor foregroundColor, ConsoleColor backgroundColor) => Write(() => OnWriteObj(value), foregroundColor, backgroundColor);
        public static void Write(int value, ConsoleColor foregroundColor, ConsoleColor backgroundColor) => Write(() => OnWriteObj(value), foregroundColor, backgroundColor);
        public static void Write(long value, ConsoleColor foregroundColor, ConsoleColor backgroundColor) => Write(() => OnWriteObj(value), foregroundColor, backgroundColor);
        public static void Write(uint value, ConsoleColor foregroundColor, ConsoleColor backgroundColor) => Write(() => OnWriteObj(value), foregroundColor, backgroundColor);
        public static void Write(ulong value, ConsoleColor foregroundColor, ConsoleColor backgroundColor) => Write(() => OnWriteObj(value), foregroundColor, backgroundColor);
        public static void Write(float value, ConsoleColor foregroundColor, ConsoleColor backgroundColor) => Write(() => OnWriteObj(value), foregroundColor, backgroundColor);
        public static void Write(double value, ConsoleColor foregroundColor, ConsoleColor backgroundColor) => Write(() => OnWriteObj(value), foregroundColor, backgroundColor);

        // Write with console color fg/bg
        public static void WriteLine(string value, ConsoleColor foregroundColor) => Write(() => OnWriteLine(value), foregroundColor);
        public static void WriteLine(object value, ConsoleColor foregroundColor) => Write(() => OnWriteLineObj(value), foregroundColor);
        public static void WriteLine(char[] value, ConsoleColor foregroundColor) => Write(() => OnWriteLineObj(value), foregroundColor);
        public static void WriteLine(char value, ConsoleColor foregroundColor) => Write(() => OnWriteLineObj(value), foregroundColor);
        public static void WriteLine(bool value, ConsoleColor foregroundColor) => Write(() => OnWriteLineObj(value), foregroundColor);
        public static void WriteLine(int value, ConsoleColor foregroundColor) => Write(() => OnWriteLineObj(value), foregroundColor);
        public static void WriteLine(long value, ConsoleColor foregroundColor) => Write(() => OnWriteLineObj(value), foregroundColor);
        public static void WriteLine(uint value, ConsoleColor foregroundColor) => Write(() => OnWriteLineObj(value), foregroundColor);
        public static void WriteLine(ulong value, ConsoleColor foregroundColor) => Write(() => OnWriteLineObj(value), foregroundColor);
        public static void WriteLine(float value, ConsoleColor foregroundColor) => Write(() => OnWriteLineObj(value), foregroundColor);
        public static void WriteLine(double value, ConsoleColor foregroundColor) => Write(() => OnWriteLineObj(value), foregroundColor);
        public static void WriteLine(string value, ConsoleColor foregroundColor, ConsoleColor backgroundColor) => Write(() => OnWriteLine(value), foregroundColor, backgroundColor);
        public static void WriteLine(object value, ConsoleColor foregroundColor, ConsoleColor backgroundColor) => Write(() => OnWriteLineObj(value), foregroundColor, backgroundColor);
        public static void WriteLine(char[] value, ConsoleColor foregroundColor, ConsoleColor backgroundColor) => Write(() => OnWriteLineObj(value), foregroundColor, backgroundColor);
        public static void WriteLine(char value, ConsoleColor foregroundColor, ConsoleColor backgroundColor) => Write(() => OnWriteLineObj(value), foregroundColor, backgroundColor);
        public static void WriteLine(bool value, ConsoleColor foregroundColor, ConsoleColor backgroundColor) => Write(() => OnWriteLineObj(value), foregroundColor, backgroundColor);
        public static void WriteLine(int value, ConsoleColor foregroundColor, ConsoleColor backgroundColor) => Write(() => OnWriteLineObj(value), foregroundColor, backgroundColor);
        public static void WriteLine(long value, ConsoleColor foregroundColor, ConsoleColor backgroundColor) => Write(() => OnWriteLineObj(value), foregroundColor, backgroundColor);
        public static void WriteLine(uint value, ConsoleColor foregroundColor, ConsoleColor backgroundColor) => Write(() => OnWriteLineObj(value), foregroundColor, backgroundColor);
        public static void WriteLine(ulong value, ConsoleColor foregroundColor, ConsoleColor backgroundColor) => Write(() => OnWriteLineObj(value), foregroundColor, backgroundColor);
        public static void WriteLine(float value, ConsoleColor foregroundColor, ConsoleColor backgroundColor) => Write(() => OnWriteLineObj(value), foregroundColor, backgroundColor);
        public static void WriteLine(double value, ConsoleColor foregroundColor, ConsoleColor backgroundColor) => Write(() => OnWriteLineObj(value), foregroundColor, backgroundColor);

    }
}