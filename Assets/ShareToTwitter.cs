using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using VoxelBusters;
//using VoxelBusters.EssentialKit;
using UnityEngine.Networking;

public class ShareToTwitter : MonoBehaviour
{
    //private SocialShareComposer m_activeComposer = null;
    public Texture2D tex;


    /* TODO: Resolve Voxelbuster errors.
    public void ShareTweet()
    {
        bool isTwitterAvailable = SocialShareComposer.IsComposerAvailable(SocialShareComposerType.Twitter);
        Debug.Log("is twitter avail? " + isTwitterAvailable);
        if (isTwitterAvailable)
        {
            Debug.Log("it is avail");
            SocialShareComposer composer = SocialShareComposer.CreateInstance(SocialShareComposerType.Twitter);
            composer.AddImage(tex);

            composer.SetText($"I just built my first AI Robot in {LeaderboardUI.playerFinalScore} seconds and all I got was this allowlist👀🚀.\nDare you to beat my score!\n\n @TheMustGame\n#TheMustGame");
            Debug.Log("it is avail 3");
            composer.SetCompletionCallback((result, error) => {
                Debug.Log("Social Share Composer was closed. Result code: " + result.ResultCode);
            });
            composer.Show();
        }
    }
    */
}
