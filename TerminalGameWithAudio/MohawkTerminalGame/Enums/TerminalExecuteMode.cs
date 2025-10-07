namespace MohawkTerminalGame;

/// <summary>
///     Execution mode for <see cref="TerminalGame"/>.
/// </summary>
public enum TerminalExecuteMode
{
    /// <summary>
    ///     <see cref="TerminalGame.Execute"/> runs only once. Once Execute() is done, program closes.
    /// </summary>
    ExecuteOnce,

    /// <summary>
    ///      <see cref="TerminalGame.Execute"/> runs in infinite loop. Next iteration starts at the top of Execute().
    /// </summary>
    ExecuteLoop,

    /// <summary>
    ///     <see cref="TerminalGame.Execute"/> runs at timed intervals (eg. "FPS"). Code tries to run at Program.TargetFPS.
    ///     Code must finish within the alloted time frame for this to work well.  
    /// </summary>
    ExecuteTime,
}
