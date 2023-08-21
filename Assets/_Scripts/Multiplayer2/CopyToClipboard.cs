using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CopyToClipboard : MonoBehaviour
{
    public TMP_Text textToCopy;
    public Button copyButton;

    private TextEditor textEditor = new TextEditor();

    private void Start()
    {
        copyButton.onClick.AddListener(CopyText);
    }

    private void CopyText()
    {
        textEditor.text = PhotonNetwork.CurrentRoom.Name;
        textEditor.SelectAll();
        textEditor.Copy();
        textEditor.text = "Copied !";
        StartCoroutine("ResetCopy");
    }

    private IEnumerator ResetCopy()
    {
        yield return new WaitForSeconds(1f);
        textEditor.text = "Copy Invite";
    }
}
