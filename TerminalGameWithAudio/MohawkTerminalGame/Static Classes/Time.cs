using System;
using System.Diagnostics;

namespace MohawkTerminalGame;

/// <summary>
///     Time related information.
/// </summary>
public static class Time
{
    private static readonly Stopwatch stopwatch = new();

    /// <summary>
    ///     If true, the time starts once the program begins.
    /// </summary>
    public static bool AutoStart { get; set; } = true;

    /// <summary>
    ///     The format used for <see cref="DisplayText"/>.
    /// </summary>
    public static string TimeFormat { get; set; } = "mm:ss.fff";

    /// <summary>
    ///     Display how much time has elapsed using <see cref="TimeFormat"/>.
    /// </summary>
    public static string DisplayText
        => new DateTime().AddSeconds(stopwatch.ElapsedMilliseconds / 1000.0).ToString(TimeFormat);

    /// <summary>
    ///     How many seconds (as decimal number) have elapsed.
    /// </summary>
    public static float ElapsedSeconds
        => stopwatch.ElapsedMilliseconds / 1000f;

    /// <summary>
    ///     How many seconds (as whole number) have elapsed.
    /// </summary>
    public static int ElapsedSecondsWhole
        => (int)(stopwatch.ElapsedMilliseconds / 1000);

    /// <summary>
    ///     How many milliseconds have elapsed.
    /// </summary>
    public static int ElapsedMilliseconds
        => (int)(stopwatch.ElapsedMilliseconds);

    /// <summary>
    ///     Manually start the timer.
    /// </summary>
    public static void Start() => stopwatch.Start();

    /// <summary>
    ///     Manually stop the timer.
    /// </summary>
    public static void Stop() => stopwatch.Stop();

    /// <summary>
    ///     Manually reset the timer time.
    /// </summary>
    public static void Reset() => stopwatch.Reset();

}
