using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class MenuManager : MonoBehaviour
{
   
   public void ContinuGame()
   {
      Debug.Log("Create Continu Level Game");
   }
   public void NewGame()
   {
      Debug.Log("Create New Level Game");
   }


   public void OptionsGame()
   {
      Debug.Log("Create Option Game");
   }

   public void ExitGame()
   {
      Debug.Log("Create Exit Level Game");
   }
}
