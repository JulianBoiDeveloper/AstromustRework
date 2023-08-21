using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIShowHide : MonoBehaviour
{
    private bool isActivate = false;

    public GameObject UIActivate;

    public GameObject[] UiBlocked;

    public GameObject SoundManagerMenu;
    

    public void HideUI()
    {
        UIActivate.SetActive(false);
    }
    public  void ShowUI()
    {
        UIActivate.SetActive(true);
    }

    private void Update()
    {
        if (UIActivate != null)
        {
            if (UIActivate.activeInHierarchy)
            {
                for (int i = 0; i < UiBlocked.Length; i++)
                {
                    UiBlocked[i].SetActive(false);
                }
            }
        }

    }

    public void HideSwitchShowUI()
    {
        if (isActivate)
        {
            UIActivate.SetActive(false);
            isActivate = false;
        }
        else
        {
            UIActivate.SetActive(true);
            isActivate = true;
        }
       
    }

 



}
