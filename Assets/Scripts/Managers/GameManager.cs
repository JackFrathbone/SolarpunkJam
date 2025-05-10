using RenderHeads.Services;
using System.Collections;
using UnityEngine;

public class GameManager : MonoService
{
    [Header("References")]
    public PlayerController playerController;
    public AudioSource universalAudioSource;
    private void Awake()
    {
        universalAudioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        StartCoroutine(StartMute());
    }

    public void PlayAudioClip(AudioClip clip, float volume, float pitch)
    {
        universalAudioSource.pitch = pitch;
        universalAudioSource.PlayOneShot(clip, volume);
    }

    public void PauseGame()
    {
        playerController.SetFreezeInput(true);
    }

    public void UnPauseGame()
    {
        playerController.SetFreezeInput(false);
    }

    public bool SwitchPlayerPlacementMode()
    {
        if (playerController != null)
        {
            return playerController.SwitchMode();
        }

        return false;
    }

    IEnumerator StartMute()
    {
        universalAudioSource.mute = true;
        yield return new WaitForSeconds(2f);
        universalAudioSource.mute = false;
    }
}
