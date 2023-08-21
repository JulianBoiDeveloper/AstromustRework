using UnityEngine;

public class ActiveHandlerEventButtonSetting : MonoBehaviour
{

    public GameObject SoundManagerGestion;
    
    

    void Start()
    {
        SoundManagerGestion = GameObject.Find("SoundManager (Don't Destroyed OnLoad)");
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(SoundManagerGestion.GetComponent<UIShowHide>().HideSwitchShowUI);
    }
}
