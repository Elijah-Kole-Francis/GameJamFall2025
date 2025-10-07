using System;
using System.Collections.Generic;
using System.Threading;

namespace MohawkTerminalGame
{
    /// <summary>
    ///     Input related functions.
    /// </summary>
    public static class Input
    {
        private readonly static Thread InputThread;
        private readonly static List<ConsoleKey> LastFrameKeys = [];
        private readonly static List<ConsoleKey> CurrentFrameKeys = [];

        /// <summary>
        ///     Called to reset current frame inputs.
        ///     Don't call this unless you have a reason to.
        /// </summary>
        internal static void PreparePollNextInput()
        {
            LastFrameKeys.Clear();
            LastFrameKeys.AddRange(CurrentFrameKeys);
            CurrentFrameKeys.Clear();
        }

        /// <summary>
        ///     Checks to see if the <paramref name="key"/> is not pressed.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>
        ///     True if key is not pressed.
        /// </returns>
        public static bool IsKeyUp(ConsoleKey key)
        {
            // Up if not currently pressed
            bool state = !CurrentFrameKeys.Contains(key);
            return state;
        }

        /// <summary>
        ///     Checks to see if the <paramref name="key"/> is pressed down.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>
        ///     True if key is pressed down.
        /// </returns>
        public static bool IsKeyDown(ConsoleKey key)
        {
            // Down if currently pressed
            bool state = CurrentFrameKeys.Contains(key);
            return state;
        }

        /// <summary>
        ///     Checks to see if the <paramref name="key"/> was pressed down this frame.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>
        ///     True if key was pressed down this frame.
        /// </returns>
        public static bool IsKeyPressed(ConsoleKey key)
        {
            // Pressed if currently pressed down but was previously unpressed
            bool state = CurrentFrameKeys.Contains(key) && !LastFrameKeys.Contains(key);
            return state;
        }

        /// <summary>
        ///     Checks to see if the <paramref name="key"/> was released this frame.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>
        ///     True if key was released this frame.
        /// </returns>
        public static bool IsKeyReleased(ConsoleKey key)
        {
            // Released if currently unpressed but was previously pressed down
            bool state = !CurrentFrameKeys.Contains(key) && LastFrameKeys.Contains(key);
            return state;
        }

        /// <summary>
        ///     Initialize background input thread.
        /// </summary>
        internal static void InitInputThread()
        {
            // Only run once
            if (InputThread != null)
                return;

            CreateInputThread();
        }

        private static Thread CreateInputThread()
        {
            if (InputThread != null)
            {
                string msg = "Input thread already exists!";
                throw new Exception(msg);
            }

            static void ThreadPollKeyboard()
            {
                while (true)
                {
                    if (Program.TerminalInputMode != TerminalInputMode.EnableInputDisableReadLine)
                        continue;

                    ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
                    if (consoleKeyInfo.Key != ConsoleKey.None)
                    {
                        CurrentFrameKeys.Add(consoleKeyInfo.Key);
                    }
                }
            }

            Thread inputThread = new(ThreadPollKeyboard);
            inputThread.Priority = ThreadPriority.Highest;
            inputThread.Start();
            return inputThread;
        }
    }
}