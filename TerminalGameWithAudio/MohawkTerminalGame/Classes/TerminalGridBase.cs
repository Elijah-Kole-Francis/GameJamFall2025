using System;
using System.Numerics;
using System.Text;

namespace MohawkTerminalGame;

/// <summary>
///     A grid of strings for easy addressing and writing.
/// </summary>
public class TerminalGridBase<T>
{
    protected readonly T[,] BackingArray;
    protected readonly StringBuilder stringBuilder = new();

    /// <summary>
    ///     Grid width in column values (values can be multiple columns / characters).
    /// </summary>
    public int Width { get; init; }
    
    /// <summary>
    ///     Grid height in rows.
    /// </summary>
    public int Height { get; init; }

    /// <summary>
    ///     Default value to set grid to and reset to.
    /// </summary>
    public T DefaultValue { get; set; }

    /// <summary>
    ///     Create a grid size of screen with <paramref name="defaultValue"/>.
    /// </summary>
    /// <param name="defaultValue">The default value to set all elements to and reset to.</param>
    public TerminalGridBase(T defaultValue) : this(Terminal.WindowWidth, Terminal.WindowHeight, defaultValue)
    {
    }

    /// <summary>
    ///     Create a grid size (<paramref name="widthInValues"/>, <paramref name="heightInRows"/>) with <paramref name="defaultValue"/>.
    /// </summary>
    /// <param name="widthInValues">Width in values (values can be multiple columns / characters).</param>
    /// <param name="heightInRows">Height in rows.</param>
    /// <param name="defaultValue">The default value to set all elements to and reset to.</param>
    public TerminalGridBase(int widthInValues, int heightInRows, T defaultValue)
    {
        Width = widthInValues;
        Height = heightInRows;
        BackingArray = new T[widthInValues, heightInRows];
        DefaultValue = defaultValue;
        SetAll(defaultValue);
    }

    /// <summary>
    ///     Reset the grid to the default value.
    /// </summary>
    public void Reset()
    {
        SetAll(DefaultValue);
    }

    /// <summary>
    ///     Non-permanent write of <paramref name="value"/> into coordinate
    ///     (<paramref name="x"/>, <paramref name="y"/>).
    /// </summary>
    /// <param name="x">The x coordinate (column, assumes single character width).</param>
    /// <param name="y">The y coordinate (row).</param>
    /// <param name="value">The value to poke.</param>
    public virtual void Poke(int x, int y, T value)
    {
        Console.SetCursorPosition(x, y);
        Console.Write(value);
    }

    /// <summary>
    ///     Get the value in the grid at position (<paramref name="x"/>, <paramref name="y"/>).
    /// </summary>
    /// <param name="x">The x coordinate within the grid.</param>
    /// <param name="y">The y coordinate within the grid.</param>
    /// <returns>
    ///     The value at that position.
    /// </returns>
    public T Get(int x, int y)
    {
        T value = BackingArray[x, y];
        return value;
    }

    /// <summary>
    ///     Set all values in table to <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value to assign.</param>
    public void SetAll(T value)
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                BackingArray[x, y] = value;
            }
        }
    }

    /// <summary>
    ///     Set all values in <paramref name="row"/> to <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value to assign.</param>
    /// <param name="row">The row to set.</param>
    public void SetRow(T value, int row)
    {
        // Ignore malformed request
        if (row < 0 || row >= Height)
            return;

        // Set row
        for (int x = 0; x < Width; x++)
        {
            BackingArray[x, row] = value;
        }
    }

    /// <summary>
    ///     Set all values in <paramref name="column"/> to <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value to assign.</param>
    /// <param name="column">The column to set.</param>
    public void SetCol(T value, int column)
    {
        // Ignore malformed request
        if (column < 0 || column >= Width)
            return;

        // Set row
        for (int y = 0; y < Height; y++)
        {
            BackingArray[column, y] = value;
        }
    }

    /// <summary>
    ///     Set all values in <paramref name="area"/> to <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value to assign.</param>
    /// <param name="area">The area to set.</param>
    public void SetRectangle(T value, Rectangle area)
    {
        int minX = int.Max(area.StartX, 0);
        int minY = int.Max(area.StartY, 0);
        int maxX = int.Min(area.EndX, Width);
        int maxY = int.Min(area.EndY, Height);
        for (int y = minY; y < maxY; y++)
        {
            for (int x = minX; x < maxX; x++)
            {
                BackingArray[x, y] = value;
            }
        }
    }

    /// <summary>
    ///     Set all values in defined rectangle area to <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value to assign.</param>
    /// <param name="x">The rectangle's upper left x coordinate.</param>
    /// <param name="y">The rectangle's upper left y coordinate.</param>
    /// <param name="w">The rectangle's width.</param>
    /// <param name="h">The rectangle's height.</param>
    public void SetRectangle(T value, int x, int y, int w, int h)
        => SetRectangle(value, new Rectangle(x, y, w, h));

    /// <summary>
    ///     Set all values in defined circle arez to <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value to assign.</param>
    /// <param name="cx">The circle's centre x coordinate.</param>
    /// <param name="cy">The circle's centre y coordinate</param>
    /// <param name="r">The ridcles radius.</param>
    public void SetCircle(T value, int cx, int cy, float r)
    {
        int minX = int.Max((int)Math.Floor(cx - r), 0);
        int minY = int.Max((int)Math.Floor(cy - r), 0);
        int maxX = int.Min((int)Math.Ceiling(cx + r), Width - 1);
        int maxY = int.Min((int)Math.Ceiling(cy + r), Height - 1);
        float threshold = r + 0.25f;
        Vector2 centre = new(cx, cy);
        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                // Is cell inside circle?
                Vector2 point = new(x, y);
                float distance = Vector2.Distance(centre, point);
                if (distance <= threshold)
                {
                    BackingArray[x, y] = value;
                }
            }
        }
    }

    /// <summary>
    ///     Create a new buffer from the <paramref name="area"/> specified of
    ///     the source buffer <paramref name="src"/>.
    /// </summary>
    /// <param name="src">The source buffer to copy from.</param>
    /// <param name="area">The area to copy.</param>
    /// <returns>
    ///     A new buffer of the specified <paramref name="area"/> of <paramref name="src"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="area"/> specified does not fit into <paramref name="src"/>.
    /// </exception>

    public static TerminalGridBase<T> CopyArea(TerminalGridBase<T> src, Rectangle area)
    {
        if (area.EndX > src.Width)
        {
            string msg = $"{nameof(area)}.{nameof(area.EndX)} value {area.EndX} is larger than {nameof(src)} width {src.Width}.";
            throw new ArgumentOutOfRangeException(msg);
        }
        if (area.EndY > src.Height)
        {
            string msg = $"{nameof(area)}.{nameof(area.EndY)} value {area.EndY} is larger than {nameof(src)} height {src.Height}.";
            throw new ArgumentOutOfRangeException(msg);
        }

        TerminalGridBase<T> copy = new(area.w, area.h, src.DefaultValue);
        for (int y = 0; y < area.y; y++)
        {
            int srcY = y + area.StartY;
            int dstY = y;
            for (int x = 0; x < area.x; x++)
            {
                int srcX = x + area.StartX;
                int dstX = x;
                copy.BackingArray[dstX, dstY] = src.BackingArray[srcX, srcY];
            }
        }
        return copy;
    }

    /// <summary>
    ///     Copy <paramref name="srcRect"/> from <paramref name="srcRect"/> to <paramref name="dest"/>
    ///     at location (<paramref name="destX"/>,<paramref name="destY"/>).
    /// </summary>
    /// <param name="src">The source grid to copy.</param>
    /// <param name="srcRect">The source rectangle to copy from <paramref name="src"/>.</param>
    /// <param name="dest">The destination grid.</param>
    /// <param name="destX">The desintation's upper left x coordinate.</param>
    /// <param name="destY">The desintation's upper left y coordinate.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="srcRect"/> bounds are incorrect (either initial coordinate
    ///     is negative or upper bounds are larger than <paramref name="srcRect"/>.
    /// </exception>
    public static void CopyFromTo(TerminalGridBase<T> src, Rectangle srcRect, TerminalGridBase<T> dest, int destX, int destY)
    {
        if (srcRect.EndX > src.Width)
        {
            string msg = $"{nameof(srcRect)}.{nameof(srcRect.EndX)} value {srcRect.EndX} is larger than {nameof(src)} width {src.Width}.";
            throw new ArgumentOutOfRangeException(msg);
        }
        if (srcRect.EndY > src.Height)
        {
            string msg = $"{nameof(srcRect)}.{nameof(srcRect.EndY)} value {srcRect.EndY} is larger than {nameof(src)} height {src.Height}.";
            throw new ArgumentOutOfRangeException(msg);
        }

        // Copy area of source
        TerminalGridBase<T> srcAreaCopy = CopyArea(src, srcRect);
        // Place into destination
        for (int y = 0; y < srcRect.y; y++)
        {
            int srcY = y;
            int dstY = y + srcRect.h;
            for (int x = 0; x < srcRect.w; x++)
            {
                int srcX = x;
                int dstX = x + srcRect.x;
                dest.BackingArray[dstX, dstY] = srcAreaCopy.BackingArray[srcX, srcY];
            }
        }
    }

    /// <summary>
    ///     Copy <paramref name="area"/> from self to self at (<paramref name="newX"/>, <paramref name="newY"/>).
    /// </summary>
    /// <param name="area">The area to copy.</param>
    /// <param name="newX">The desintation's upper left x coordinate.</param>
    /// <param name="newY">The desintation's upper left y coordinate.</param>
    public void CopyToSelf(Rectangle area, int newX, int newY) => CopyFromTo(this, area, this, newX, newY);

    public override string ToString()
    {
        stringBuilder.Clear();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                stringBuilder.Append(BackingArray[x, y]);
            }
            stringBuilder.AppendLine();
        }
        string result = stringBuilder.ToString();
        return result;
    }

    /// <summary>
    ///     Write this grid to the terminal.
    /// </summary>
    public virtual void Write()
    {
        string display = this.ToString();
        Terminal.Write(display);
    }

    /// <summary>
    ///     Write this grid to the terminal at (<paramref name="x"/>, <paramref name="y"/>).
    /// </summary>
    /// <param name="x">The current screen's x coordinate (column).</param>
    /// <param name="y">The current screen's y coordinate (row).</param>
    public void Overwrite(int x = 0, int y = 0)
    {
        Terminal.SetCursorPosition(x, y);
        Write();
    }

    /// <summary>
    ///     Clear the screen then write the grid to the terminal.
    /// </summary>
    public void ClearWrite()
    {
        Terminal.Clear();
        Write();
    }
}
