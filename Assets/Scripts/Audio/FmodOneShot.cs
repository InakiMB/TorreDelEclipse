using FMODUnity;
using UnityEngine;

public static class FmodOneShot
{
    public static void Play(EventReference sound, Vector3 position)
    {
        if (!sound.IsNull) RuntimeManager.PlayOneShot(sound, position);
    }
}
