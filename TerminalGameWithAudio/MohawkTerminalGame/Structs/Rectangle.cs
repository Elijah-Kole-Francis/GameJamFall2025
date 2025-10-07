using System;

namespace MohawkTerminalGame;

/// <summary>
///     Rectangle for manipulating areas for <see cref="TerminalGrid"/>.
/// </summary>
public struct Rectangle
{
    public int x;
    public int y;
    public int w;
    public int h;

    public readonly int StartX => x;
    public readonly int EndX => x + w;
    public readonly int StartY => y;
    public readonly int EndY => y + h;

    public Rectangle()
    {
    }
    public Rectangle(int x, int y, int w, int h)
    {
        this.x = x;
        this.y = y;
        this.w = w;
        this.h = h;
    }

    /// <summary>
    ///     Create a rectangle from 2 opposite corners (upper-left and lower-right).
    /// </summary>
    /// <param name="x1">Upper left X coordinate.</param>
    /// <param name="y1">Upper left Y coordinate.</param>
    /// <param name="x2">Lower right X coordinate.</param>
    /// <param name="y2">Lower right Y coordinate.</param>
    /// <returns>
    ///     A rectangled from the specified corners.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if coordinate values are less than zero.
    /// </exception>
    public static Rectangle FromCorners(int x1, int y1, int x2, int y2)
    {
        // Lower bounds for X1
        if (x1 < 0)
        {
            string msg = $"Corner value {nameof(x1)} is {x1} but must be a minimum of 0.";
            throw new ArgumentOutOfRangeException(msg);
        }
        // Lower bounds for X2
        if (x2 < 0)
        {
            string msg = $"Corner value {nameof(x2)} is {x2} but must be a minimum of 0.";
            throw new ArgumentOutOfRangeException(msg);
        }
        // Lower bounds for Y1
        if (y1 < 0)
        {
            string msg = $"Corner value {nameof(y1)} is {y1} but must be a minimum of 0.";
            throw new ArgumentOutOfRangeException(msg);
        }
        // Lower bounds for Y2
        if (y1 < 0)
        {
            string msg = $"Corner value {nameof(y2)} is {y2} but must be a minimum of 0.";
            throw new ArgumentOutOfRangeException(msg);
        }

        Rectangle rectangle = new()
        {
            x = x1,
            y = y1,
            w = x2 - x1,
            h = y2 - y1,
        };
        return rectangle;
    }

    /// <summary>
    ///     Creates a rectangle from a centre point.
    /// </summary>
    /// <param name="x">The central X coordinate.</param>
    /// <param name="y">The central Y coordinate.</param>
    /// <param name="w">The rectangle width.</param>
    /// <param name="h">The rectangle height.</param>
    /// <returns>
    ///     A rectangled from the specified coordinates.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if centre coordinates are less than zero, and
    ///     thrown if dimensions are less than or equal to zero.
    /// </exception>
    public static Rectangle FromCentre(int x, int y, int w, int h)
    {
        // Lower bounds for X
        if (x < 0)
        {
            string msg = $"Centre value {nameof(x)} is {x} but must be a minimum of 0.";
            throw new ArgumentOutOfRangeException(msg);
        }
        // Lower bounds for Y
        if (y < 0)
        {
            string msg = $"Centre value {nameof(y)} is {y} but must be a minimum of 0.";
            throw new ArgumentOutOfRangeException(msg);
        }
        // Lower bounds for W
        if (w < 1)
        {
            string msg = $"Width {nameof(w)} is {w} but must be a minimum of 1.";
            throw new ArgumentOutOfRangeException(msg);
        }
        // Lower bounds for H
        if (h < 1)
        {
            string msg = $"Height {nameof(h)} is {h} but must be a minimum of 1.";
            throw new ArgumentOutOfRangeException(msg);
        }

        int offsetX = (w - 1) / 2;
        int offsetY = (h - 1) / 2;
        Rectangle rectangle = new()
        {
            x = x - offsetX,
            y = y - offsetY,
            w = w,
            h = h,
        };
        return rectangle;
    }
}
