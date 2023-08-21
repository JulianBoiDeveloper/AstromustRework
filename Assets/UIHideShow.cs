using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHideShow : MonoBehaviour
{
    [SerializeField] GameObject uiToHide;

    public void Hide()
    {
        uiToHide.SetActive(!uiToHide.activeInHierarchy);
    }
}
