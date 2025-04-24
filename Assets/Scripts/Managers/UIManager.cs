using RenderHeads.Services;
using TMPro;
using UnityEngine;

public class UIManager : MonoService
{
    [Header("References")]
    [SerializeField] TextMeshProUGUI _waterPipeCounter;
    [SerializeField] TextMeshProUGUI _partsCounter;
    [SerializeField] TextMeshProUGUI _cableCounter;

    public void SetWaterPipeCounter(int i)
    {
        _waterPipeCounter.text = i.ToString();
    }

    public void SetPartsCounter(int i)
    {
        _partsCounter.text = i.ToString();
    }

    public void SetCableCounter(int i)
    {
        _cableCounter.text = i.ToString();
    }
}
