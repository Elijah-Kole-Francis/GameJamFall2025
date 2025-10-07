/*////////////////////////////////////////////////////////////////////////
 * Copyright (c)
 * Mohawk College, 135 Fennell Ave W, Hamilton, Ontario, Canada L9C 0E5
 * Game Design (374): GAME 10003 Game Development Foundations
 *////////////////////////////////////////////////////////////////////////

namespace MohawkTerminalGame;

/// <summary>
///     Represents a music file (audio over 10s long).
/// </summary>
/// <remarks>
///     Wrapper around Raylib.Music
/// </remarks>
public struct Music
{
    private Raylib_cs.Music music;

    /// <summary>
    ///     File path of this music.
    /// </summary>
    public string FilePath { get; init; }

    /// <summary>
    ///     Name of this music file.
    /// </summary>
    public string FileName { get; init; }

    /// <summary>
    ///     Whether or not the music file loops.
    /// </summary>
    public bool Looping
    {
        readonly get => music.Looping;
        set => music.Looping = value;
    }

    [GeneratorTools.OmitFromDocumentation]
    public Raylib_cs.Music RaylibMusic
    {
        readonly get => music;
        init => music = value;
    }

    [GeneratorTools.OmitFromDocumentation]
    public static implicit operator Music(Raylib_cs.Music raylibMusic)
    {
        var music = new Music()
        {
            RaylibMusic = raylibMusic,
        };
        return music;
    }

    [GeneratorTools.OmitFromDocumentation]
    public static implicit operator Raylib_cs.Music(Music music)
    {
        var raylibMusic = music.RaylibMusic;
        return raylibMusic;
    }

    public override readonly string ToString()
    {
        string value = $"{nameof(Music)}({FilePath})";
        return value;
    }
}