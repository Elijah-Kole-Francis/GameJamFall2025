namespace MohawkTerminalGame;

/// <summary>
///     
/// </summary>
public class TerminalGrid : TerminalGridBase<string>
{
    public TerminalGrid(string defaultValue) : base(defaultValue)
    {
    }

    public TerminalGrid(int width, int height, string defaultValue) : base(width, height, defaultValue)
    {
    }
}
