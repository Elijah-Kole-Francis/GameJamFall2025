/*////////////////////////////////////////////////////////////////////////
 * Copyright (c)
 * Mohawk College, 135 Fennell Ave W, Hamilton, Ontario, Canada L9C 0E5
 * Game Design (374): GAME 10003 Game Development Foundations
 *////////////////////////////////////////////////////////////////////////

using Raylib_cs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace MohawkTerminalGame;

/// <summary>
///     Access audio functions.
/// </summary>
/// <remarks>
///     A static wrapper to standardize raylib's audio API.
/// </remarks>
public static class Audio
{
    public static Thread MusicThread;

    /// <summary>
    ///     Initializes Audio library.
    /// </summary>
    public static void Initialize()
    {
        // Only run once
        if (MusicThread != null)
            return;

        Raylib.InitAudioDevice();
        MusicThread = CreateMusicThread();
    }

    private static Thread CreateMusicThread()
    {
        if (MusicThread != null)
        {
            string msg = "Music thread already exists!";
            throw new Exception(msg);
        }

        static void ThreadUpdateMusicBuffers()
        {
            while (true)
            {
                // Update music buffers every frame
                foreach (var music in LoadedMusic)
                    Raylib.UpdateMusicStream(music);
                Thread.Sleep(20);
            }
        }

        Thread musicThread = new(ThreadUpdateMusicBuffers);
        musicThread.Priority = ThreadPriority.Highest;
        musicThread.Start();
        return musicThread;
    }

    /// <summary>
    ///     Unloads all audio files.
    /// </summary>
    public static void UnloadAudio()
    {
        // Unload assets
        foreach (var music in Audio.LoadedMusic)
            Audio.UnloadMusic(music);
        foreach (var sound in Audio.LoadedSounds)
            Audio.UnloadSound(sound);
    }


    #region Fields and Properties

    /// <summary>
    ///     Keep list of music to auto-update in background, and also
    ///     speed up duplicate load, and properly unload on quit.
    /// </summary>
    private static readonly Dictionary<string, Music> loadedMusic = [];

    /// <summary>
    ///     Keep list of sounds to auto-update in background, and also
    ///     speed up duplicate load, and properly unload on quit.
    /// </summary>
    private static readonly Dictionary<string, Sound> loadedSounds = [];

    /// <summary>
    ///     Get an array of all loaded music.
    /// </summary>
    public static Music[] LoadedMusic => [.. loadedMusic.Values];

    /// <summary>
    ///     Get an array of all loaded sounds.
    /// </summary>
    public static Sound[] LoadedSounds => [.. loadedSounds.Values];

    #endregion

    #region Methods

    /// <summary>
    ///     Get the length of <paramref name="music"/> in seconds.
    /// </summary>
    /// <param name="music">The music data to check.</param>
    /// <returns>
    ///     Returns the length of audio file <paramref name="music"/> in seconds.
    /// </returns>
    public static float GetMusicLength(Music music) => Raylib.GetMusicTimeLength(music);

    /// <summary>
    ///     Checks if <paramref name="music"/> is playing.
    /// </summary>
    /// <param name="music">The music to check.</param>
    /// <returns>
    ///     Returns true if playing, false otherwise.
    /// </returns>
    public static bool IsPlaying(Music music) => Raylib.IsMusicStreamPlaying(music);

    /// <summary>
    ///     Checks if <paramref name="sound"/> is playing.
    /// </summary>
    /// <param name="sound">The sound to check.</param>
    /// <returns>
    ///     Returns true if playing, false otherwise.
    /// </returns>
    public static bool IsPlaying(Sound sound) => Raylib.IsSoundPlaying(sound);

    /// <summary>
    ///     Loads a music file at <paramref name="filePath"/>.
    /// </summary>
    /// <remarks>
    ///     Music should be more than 10s long, otherwise use <see cref="Sound"/>.
    ///     Supported file types: FLAC, MOD, MP3, OGG, QOA, WAV, XM.
    /// </remarks>
    /// <param name="filePath">The file path to the audio file.</param>
    /// <returns>
    ///     Returns the loaded music data.
    /// </returns>
    public static Music LoadMusic(string filePath)
    {
        // Return existing instance if reloading same asset.
        if (loadedMusic.TryGetValue(filePath, out Music value))
            return value;

        // Load asset from disk. Assign it file path and file name.
        Music music = new()
        {
            RaylibMusic = Raylib.LoadMusicStream(filePath),
            FilePath = filePath,
            FileName = Path.GetFileNameWithoutExtension(filePath)
        };

        // Add to reference dictionary for data reused on duplicate load calls.
        loadedMusic.Add(filePath, music);

        // Return newly loaded value.
        return music;
    }

    /// <summary>
    ///     Loads a sound file at <paramref name="filePath"/>.
    /// </summary>
    /// <remarks>
    ///     Sound should be 10s or less, otherwise use <see cref="Music"/>.
    ///     Supported file types: FLAC, MOD, MP3, OGG, QOA, WAV, XM.
    /// </remarks>
    /// <param name="filePath">The file path to the audio file.</param>
    /// <returns>
    ///     Returns the loaded sound data.
    /// </returns>
    public static Sound LoadSound(string filePath)
    {
        // Return existing instance if releading same asset.
        if (loadedSounds.TryGetValue(filePath, out Sound value))
            return value;

        // Load asset from disk. Assign it file path and file name.
        Sound sound = new()
        {
            RaylibSound = Raylib.LoadSound(filePath),
            FilePath = filePath,
            FileName = Path.GetFileNameWithoutExtension(filePath)
        };

        // Add to reference dictionary for data reused on duplicate load calls.
        loadedSounds.Add(filePath, sound);

        // Return newly loaded value.
        return sound;
    }

    /// <summary>
    ///     Pause a playing <paramref name="music"/>.
    /// </summary>
    /// <param name="music">The music to pause.</param>
    public static void Pause(Music music) => Raylib.PauseMusicStream(music);

    /// <summary>
    ///     Pause a playing <paramref name="sound"/>.
    /// </summary>
    /// <param name="sound">The sound to pause.</param>
    public static void Pause(Sound sound) => Raylib.PauseSound(sound);

    /// <summary>
    ///     Plays <paramref name="music"/>.
    /// </summary>
    /// <param name="music">The music to play.</param>
    public static void Play(Music music) => Raylib.PlayMusicStream(music);

    /// <summary>
    ///     Plays <paramref name="sound"/>.
    /// </summary>
    /// <param name="sound">The sound to play.</param>
    public static void Play(Sound sound) => Raylib.PlaySound(sound);

    /// <summary>
    ///     Resumes a paused <paramref name="music"/>.
    /// </summary>
    /// <param name="music">The music to resume.</param>
    public static void Resume(Music music) => Raylib.ResumeMusicStream(music);

    /// <summary>
    ///     Resumes a paused <paramref name="sound"/>.
    /// </summary>
    /// <param name="sound">The sound to resume.</param>
    public static void Resume(Sound sound) => Raylib.ResumeSound(sound);

    /// <summary>
    ///     Sets the <paramref name="pan"/> for <paramref name="music"/>.
    /// </summary>
    /// <param name="music">The music to pan.</param>
    /// <param name="pan">The pan value. 0.5 is center, 0 left, and 1 right.</param>
    public static void SetPan(Music music, float pan) => Raylib.SetMusicPan(music, pan);

    /// <summary>
    ///     Sets the <paramref name="pan"/> for <paramref name="sound"/>.
    /// </summary>
    /// <param name="sound">The sound to pan.</param>
    /// <param name="pan">The pan value. 0.5 is center, 0 left, and 1 right.</param>
    public static void SetPan(Sound sound, float pan) => Raylib.SetSoundPan(sound, pan);

    /// <summary>
    ///     Sets the <paramref name="pitch"/> for <paramref name="music"/>.
    /// </summary>
    /// <param name="music">The music to pitch.</param>
    /// <param name="pitch">The pitch value. 1.0 is the base pitch level.</param>
    public static void SetPitch(Music music, float pitch) => Raylib.SetMusicPitch(music, pitch);

    /// <summary>
    ///     Sets the <paramref name="pitch"/> for <paramref name="sound"/>.
    /// </summary>
    /// <param name="sound">The sound to pitch.</param>
    /// <param name="pitch">The pitch value. 1.0 is the base pitch level.</param>
    public static void SetPitch(Sound sound, float pitch) => Raylib.SetSoundPitch(sound, pitch);

    /// <summary>
    ///     Sets the <paramref name="volume"/> for <paramref name="music"/>.
    /// </summary>
    /// <param name="music">The music to set.</param>
    /// <param name="volume">The volume amount. 0 is silent, 1 is max volume.</param>
    public static void SetVolume(Music music, float volume) => Raylib.SetMusicVolume(music, volume);

    /// <summary>
    ///     Sets the <paramref name="volume"/> for <paramref name="sound"/>.
    /// </summary>
    /// <param name="sound">The sound to set.</param>
    /// <param name="volume">The volume amount. 0 is silent, 1 is max volume.</param>
    public static void SetVolume(Sound sound, float volume) => Raylib.SetSoundVolume(sound, volume);

    /// <summary>
    ///     Stop a playing or paused <paramref name="music"/>.
    /// </summary>
    /// <param name="music">The music to stop.</param>
    public static void Stop(Music music) => Raylib.StopMusicStream(music);

    /// <summary>
    ///     Stop a playing or paused <paramref name="sound"/>.
    /// </summary>
    /// <param name="sound">The sound to stop.</param>
    public static void Stop(Sound sound) => Raylib.StopSound(sound);

    /// <summary>
    ///     Unloads <paramref name="music"/> from memory.
    /// </summary>
    /// <param name="music">The music to unload.</param>
    public static void UnloadMusic(Music music)
    {
        loadedMusic.Remove(music.FilePath);
        Raylib.UnloadMusicStream(music);
    }

    /// <summary>
    ///     Unloads a <paramref name="sound"/> from memory.
    /// </summary>
    /// <param name="sound">The sound to unload.</param>
    public static void UnloadSound(Sound sound)
    {
        loadedSounds.Remove(sound.FilePath);
        Raylib.UnloadSound(sound);
    }

    #endregion

}
