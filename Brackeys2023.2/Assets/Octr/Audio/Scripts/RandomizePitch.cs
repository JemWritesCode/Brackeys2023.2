using UnityEngine;

namespace octr.Audio
{
    public class RandomizePitch : MonoBehaviour
    {
        public AudioSource audioSource;
        public float minPitch, maxPitch;

        // Update is called once per frame
        public void PlayRandom()
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.Play();
        }
    }
}
