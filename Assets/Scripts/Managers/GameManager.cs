using RenderHeads.Services;
using UnityEngine;

public class GameManager : MonoService
{
    [Header("References")]
    public PlayerController playerController;

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
