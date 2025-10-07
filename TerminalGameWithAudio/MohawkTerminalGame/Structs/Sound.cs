/*////////////////////////////////////////////////////////////////////////
 * Copyright (c)
 * Mohawk College, 135 Fennell Ave W, Hamilton, Ontario, Canada L9C 0E5
 * Game Design (374): GAME 10003 Game Development Foundations
 *////////////////////////////////////////////////////////////////////////

namespace MohawkTerminalGame;

/// <summary>
///     Represents a sound file (audio 10s or less).
/// </summary>
/// <remarks>
///     Wrapper around Raylib.Sound
/// </remarks>
public readonly record struct Sound
{
    /// <summary>
    ///     File path of this sound.
    /// </summary>
    public string FilePath { get; init; }

    /// <summary>
    ///     Name of this sound file.
    /// </summary>
    public string FileName { get; init; }


    [GeneratorTools.OmitFromDocumentation]
    public Raylib_cs.Sound RaylibSound { get; init; }

    [GeneratorTools.OmitFromDocumentation]
    public static implicit operator Sound(Raylib_cs.Sound raylibSound)
    {
        var font = new Sound()
        {
            RaylibSound = raylibSound,
        };
        return font;
    }

    [GeneratorTools.OmitFromDocumentation]
    public static implicit operator Raylib_cs.Sound(Sound sound)
    {
        var raylibSound = sound.RaylibSound;
        return raylibSound;
    }

    public override readonly string ToString()
    {
        string value = $"{nameof(Sound)}({FilePath})";
        return value;
    }
}