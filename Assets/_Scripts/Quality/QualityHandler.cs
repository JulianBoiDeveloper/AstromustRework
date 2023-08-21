using System;
using UnityEngine;
using TMPro;

public class QualityHandler : MonoBehaviour
{
    public QualityListChange settings;
    
    public event Action<int> OnTakaValueChanged;

    private int index;

    // Reference to the TMP dropdown UI element
    public TMP_Dropdown dropdown;

    // Called when the TMP dropdown selection changes
    public void OnDropdownValueChanged()
    {
        // Update the index based on the TMP dropdown's value
        index = dropdown.value;

        // Invoke the event with the current index
        OnTakaValueChanged?.Invoke(index);
        
        
        // Update quality settings based on the index
        switch (index)
        {
            case 0:
                QualitySettings.SetQualityLevel(0, true);
                settings.quality = QualityLevel.Low;
                settings.reloadQuality = true;
                break;
            case 1:
                QualitySettings.SetQualityLevel(1, true);
                settings.quality = QualityLevel.Medium;
                settings.reloadQuality = true;
                break;
            case 2:
                QualitySettings.SetQualityLevel(2, true);
                settings.quality = QualityLevel.High;
                settings.reloadQuality = true;
                break;
        }
    }
}