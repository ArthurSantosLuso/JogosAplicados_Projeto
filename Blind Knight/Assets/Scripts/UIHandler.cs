using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [Tooltip("Slide bars for UI. Ex: Health and Stamina")]
    [SerializeField]
    private List<Slider> bars;

    [SerializeField]
    private Slider currentEnemyLifeBar;

    [Space]
    [SerializeField]
    private GameObject deathScreen;

    public void SetBarValue(int barIdx, float currentValue, float maxValue)
    {
        bars[barIdx].value = currentValue / maxValue;
    }

    public void ShowDeathScreen()
    {
        deathScreen.SetActive(true);
    }

    public void SetCurrentEnemyHealthBar(float currentValue, float maxValue)
    {
        currentEnemyLifeBar.value = currentValue / maxValue;
    }
}
