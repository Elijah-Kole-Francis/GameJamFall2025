using System;

namespace MohawkTerminalGame;

public struct ColoredText
{
    public ConsoleColor fgColor;
    public ConsoleColor bgColor;
    public string text;

    public ColoredText(string text = "  ") : this(text, ConsoleColor.White, ConsoleColor.Black)
    {
    }

    public ColoredText(string text, ConsoleColor fgColor, ConsoleColor bgColor)
    {
        this.text = text;
        this.fgColor = fgColor;
        this.bgColor = bgColor;
    }

    public static implicit operator string(ColoredText terminalText)
    {
        return terminalText.text;
    }

    public static implicit operator ColoredText(string stringValue)
    {
        return new ColoredText(stringValue);
    }
}