namespace MohawkTerminalGame;

public enum TerminalInputMode
{
    /// <summary>
    ///     Allows for ReadLine as usual
    /// </summary>
    KeyboardReadAndReadLine,

    /// <summary>
    ///     Allows <see cref="Input"/> background polling, however this
    ///     means you cannot use ReadLine.
    /// </summary>
    EnableInputDisableReadLine,
}
