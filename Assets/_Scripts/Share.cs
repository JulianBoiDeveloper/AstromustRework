using UnityEngine;
using System.IO;
using System.Collections;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class Share : MonoBehaviour
{
	public int bestScore = 0;
    // Start is called before the first frame update
    public void ShareBestScore()
    {
        StartCoroutine("TakeScreenShotAndShare");
    }


	IEnumerator TakeScreenShotAndShare()
	{
		yield return new WaitForEndOfFrame();

		Texture2D tx = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		tx.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		tx.Apply();

		string path = Path.Combine(Application.temporaryCachePath, "sharedImage.png");//image name
		File.WriteAllBytes(path, tx.EncodeToPNG());

		Destroy(tx); //to avoid memory leaks

        new NativeShare()
			.AddFile(path)
			.SetSubject("Astromust: My Exceptional Score!")
			.SetText("I just achieved an outstanding score of " + PhotonNetwork.LocalPlayer.GetScore()  + " in " + PhotonNetwork.CurrentRoom.Name + "! " +
			         " 🚀🌌 Can you beat it? 🌠 The best score is :  "+bestScore+" #Astromust #HighScore #GamingJourney")
			.Share();
	}

}
