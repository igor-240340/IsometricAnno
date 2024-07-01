using UnityEngine;

public class Carrier : MonoBehaviour
{
    public Animation animation;

    void Start()
    {
    }

    void Update()
    {
        if (animation.isPlaying)
            return;

        animation.Play();
    }
}