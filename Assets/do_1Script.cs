using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class do_1Script : MonoBehaviour
{
    public Animator animator;
    public AudioSource audioSource;
    
    public void PlayAudio()
    {
        audioSource.Play();
    }
    
    public void StopAudio()
    {
        audioSource.Stop();
    }
    
    public void AnimationEvent(string eventName)
    {
        if (eventName == "PlayAudio")
        {
            PlayAudio();
        }
        else if (eventName == "StopAudio")
        {
            StopAudio();
        }
    }
    
    public void StartThis()
    {
        if(animator == null)
        {
            PlayAudio();
        }
        else
        {
            animator.Play("Scene");
        }
    }
}
