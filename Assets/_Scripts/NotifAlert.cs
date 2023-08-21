using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotifAlert : MonoBehaviour
{
    
    public GameObject notif;
    public AlertTypeEnum type = AlertTypeEnum.INFO;
    public Sprite[] alertImg;
    public string message;
    public float secondTimeShow = 1f;
    public float fadeDuration = 1.0f;
    
    private string prefix;
    private Sprite alertImgSelected;
    private float currentLerpTime = 0f;

    private void Start()
    {
        switch (type)
        {
            case AlertTypeEnum.INFO:
                prefix = "[INFORMATION] ";
                alertImgSelected = alertImg[0];
                break;
            case AlertTypeEnum.WARNING:
                prefix = "[WARNING] ";
                alertImgSelected = alertImg[1];
                break;  
            case AlertTypeEnum.ERROR:
                prefix = "[ERROR] ";
                alertImgSelected = alertImg[2];
                break;  
            case AlertTypeEnum.SUCCES:
                prefix = "";
                alertImgSelected = alertImg[3];
                break;  
        }
        
    }

    public void ShowTime()
    {
        notif.SetActive(true);
        StartCoroutine("ChangeNotif");
    }

    public IEnumerator ChangeNotif()
    {
        notif.GetComponent<Image>().sprite = alertImgSelected;
        notif.transform.GetChild(1).GetComponent<TMP_Text>().text = prefix + message;
        yield return new WaitForSeconds(secondTimeShow);
        if (currentLerpTime < fadeDuration)
        {
            currentLerpTime += Time.deltaTime;
            float t = currentLerpTime / fadeDuration;
        }
        notif.SetActive(false);
    }
    
    

}

public enum AlertTypeEnum
{
    INFO,
    WARNING,
    ERROR,
    SUCCES
}
