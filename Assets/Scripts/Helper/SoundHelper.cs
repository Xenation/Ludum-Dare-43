using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD43
{
    public class SoundHelper : Singleton<SoundHelper>
    {
        IEnumerator VolumeFade(AudioSource audioSource, float endVolume = 0.0f, float fadeLength = 0.1f)
        {

            float startVolume = audioSource.volume;
            float startTime = Time.time;

            while (Time.time < startTime + fadeLength)
            {
                audioSource.volume = startVolume + ((endVolume - startVolume) * ((Time.time - startTime) / fadeLength));
                yield return null;
            }

            if (endVolume == 0)
                audioSource.Stop();

            audioSource.volume = 1;
        }

        public void StopWithFade(AudioSource audioSource, float endVolume = 0.0f, float fadeLength = 0.1f)
        {
            StartCoroutine(VolumeFade(audioSource));
        }

    } 
}
