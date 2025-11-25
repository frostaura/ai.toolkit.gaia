using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FrostAura.MCP.Gaia.Utilities;

/// <summary>
/// Utility for playing sound files asynchronously
/// </summary>
public static class SoundPlayer
{
    private static readonly string SoundDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        "Desktop", "Projects", "Experimentation", "ai.toolkit.gaia", ".claude", "hooks", "sounds"
    );

    /// <summary>
    /// Plays a sound file asynchronously (non-blocking)
    /// </summary>
    /// <param name="soundName">Name of the sound file (without extension)</param>
    public static void PlayAsync(string soundName)
    {
        var soundFile = Path.Combine(SoundDirectory, $"{soundName}.mp3");

        if (!File.Exists(soundFile))
        {
            // Silently fail if sound file doesn't exist
            return;
        }

        try
        {
            // Use afplay on macOS (non-blocking)
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "afplay",
                    Arguments = soundFile,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                });
            }
            // Could add Windows/Linux support here if needed
        }
        catch
        {
            // Silently fail if sound playback fails
        }
    }
}
