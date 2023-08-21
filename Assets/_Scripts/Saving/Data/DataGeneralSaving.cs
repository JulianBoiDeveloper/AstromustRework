using UnityEngine;  
using Astromust.Saving;

public class DataGeneralSaving : MonoBehaviour
{
    // Players Data Saving
    public DataPlayerSaving player;
    // Tech Data Saving
    public DataTechTreeSaving[] tech;
    // Chest Data Saving
    public DataChestSaving[] chest;
    // Quest Data Saving
    public DataQuestSaving[] quest;
    // Custom Data Saving
    public DataCustomSaving[] custom;
    // Object map state Data Saving
    public DataPositionElement[] objMap;
    // Object Saving with tag = "SavingObject"
    public DataObjectMapSaving objAllPositionWithTag;
    

}