using UnityEngine;

namespace LethalModelSwitcher.Managers;

public static class SoundManager
{
    public static void PlaySound(AudioClip clip, Vector3 position)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position);
            LethalModelSwitcher.Logger.LogInfo($"Played sound: {clip.name}");
        }
        else
        {
            LethalModelSwitcher.Logger.LogError("Failed to play sound: clip is null");
        }
    }
}