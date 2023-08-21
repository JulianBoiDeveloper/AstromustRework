using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatGPTManagerView : MonoBehaviour
{
    private bool openChatGPT = false;
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    private void Update()
    {

        if (openChatGPT == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameObject.transform.GetChild(0).gameObject.SetActive(false);
                openChatGPT = false;
            }
        }
        
    }

    public void OpenChatGPT()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        openChatGPT = true;
    }
    
}
