using System;

namespace MohawkTerminalGame;

internal class TerminalGridWithColor : TerminalGridBase<ColoredText>
{
    public TerminalGridWithColor(ColoredText defaultValue) : base(defaultValue)
    {
    }

    public TerminalGridWithColor(int width, int height, ColoredText defaultValue) : base(width, height, defaultValue)
    {
    }

    public override void Write()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                var value = BackingArray[x, y];
                Console.BackgroundColor = value.bgColor;
                Console.ForegroundColor = value.fgColor;
                Console.Write(value.text);
            }
            Console.WriteLine();
        }
    }


    public override void Poke(int x, int y, ColoredText value)
    {
        Console.SetCursorPosition(x, y);
        Console.BackgroundColor = value.bgColor;
        Console.ForegroundColor = value.fgColor;
        Console.Write(value.text);
    }

}
