using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QualityListChange : MonoBehaviour
{

    public TMP_Dropdown dropdown;

    public QualityLevel quality = QualityLevel.Low;

    public bool reloadQuality;

    private void Start()
    {
        SetQualityLevel(quality);
    }

    private void Update()
    {
        if(reloadQuality){
            SetQualityLevel(quality);
            reloadQuality = false;
        }
    }

    public void SetQualityLevel(QualityLevel level)
    {
        switch (level)
        {
            case QualityLevel.Low:
                QualitySettings.SetQualityLevel(0, true);
                dropdown.value = 0;
                break;
            case QualityLevel.Medium:
                QualitySettings.SetQualityLevel(1, true);
                dropdown.value = 1;
                break;
            case QualityLevel.High:
                QualitySettings.SetQualityLevel(2, true);
                dropdown.value = 2;
                break;
        }
       
    }

}

public enum QualityLevel
{
    Low = 0,
    Medium = 1,
    High = 2
}