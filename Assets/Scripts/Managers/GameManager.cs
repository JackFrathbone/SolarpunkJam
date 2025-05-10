using RenderHeads.Services;
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
}
