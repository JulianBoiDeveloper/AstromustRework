using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Photon.Pun;

public class Item1 : MonoBehaviourPunCallbacks, IInteractable
{
    public string id = "";
    public Sprite icon;

    public List<Recipe> recipe;
    public float craftingTime = 10f;
    public Item1 byProduct;

    public List<Recipe> upgradeRecipe;
    public List<Sprite> upgradeIcon;

    PhotonView view;

    [HideInInspector] public int amount = 1;

    public bool canBePicked = true;
    public bool canBeDropped = true;
    public bool canGoInInventory = true;
    public bool canBeStacked = true;
    public int maxAmountStack = 1;
    public bool canBeTakenFromInventory = true;

    public string itemName = "Object name";
    public string itemDescription = "This is an item description";
    [HideInInspector] public Outline outlineEffect;


    [System.Serializable]
    public struct Recipe
    {
        public Item1 material;
        public int amount;
    }

    protected virtual void Start()
    {
        outlineEffect = this.gameObject.AddComponent<Outline>();
        outlineEffect.OutlineMode = Outline.Mode.OutlineVisible;
        outlineEffect.OutlineWidth = 3.2f;
        outlineEffect.enabled = false;

        view = GetComponent<PhotonView>();
    }

    public virtual void Interact(GameObject go)
    {
        if(!go.GetComponent<TestCharacterController>().inventory.IsInventoryFull(this)) {
            Debug.Log("Interacing ITEM");
            go.GetComponent<TestCharacterController>().inventory.AddMaterial(this);

            if(view != null) {
                Debug.Log("Hiding gun1");
                view.RPC("UpdateGO", RpcTarget.All, false);
            }
            else
            {
                Debug.Log("No view, normal hiding item");
                this.gameObject.SetActive(false);
            }
        }
    }

    public void EnableGO(bool enable)
    {
        view.RPC("UpdateGO", RpcTarget.All, enable);
    }

    [PunRPC]
    public void UpdateGO(bool enable)
    {
        Debug.Log("Hiding gun2");
        this.gameObject.SetActive(enable);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(id == "") {
            GenerateUniqueID();
        }
    }

    public void GenerateUniqueID()
    {
        id = System.Guid.NewGuid().ToString();
        AddItemToListOfItems();
    }

    void AddItemToListOfItems()
    {
        GameObject itemsList = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/GameItems.prefab");

        // Get the script component on the prefab that holds the list
        GameItems gameItems = itemsList.GetComponent<GameItems>();

        // Add the object to the prefab's list if not already there
        if (!gameItems.items.Contains(this.gameObject))
            gameItems.items.Add(this.gameObject);

        EditorUtility.SetDirty(gameItems);
    }
#endif

    public void IncreaseToolLevel()
    {

    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MonoBehaviour), true)]
public class UniqueIDGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {

        Item1 targetScript;
        /*if (GUILayout.Button("Generate Icon"))
        {
            targetScript = (Item1)target;
          //  byte[] bytes = AssetPreview.GetAssetPreview(targetScript.transform.GetChild(0).gameObject).EncodeToPNG();
            System.IO.File.WriteAllBytes("mYPrefabIcon.png", AssetPreview.GetAssetPreview(targetScript.transform.GetChild(0).gameObject).EncodeToPNG());

          //  System.IO.File.WriteAllBytes("PrefabIcon.png", bytes);
           // Sprite iconSprite = Sprite.Create(iconTexture, new Rect(0, 0, iconTexture.width, iconTexture.height), Vector2.zero);
           // targetScript.icon = iconSprite;
        }*/

        try
        {
            targetScript = (Item1)target;
            if (GUILayout.Button("Generate Unique ID"))
            {
                targetScript.GenerateUniqueID();
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(targetScript);
            }

            base.OnInspectorGUI();
        }
        catch (System.Exception ex)
        {

            base.OnInspectorGUI();
            return;
        }
        /*
        EditorGUILayout.LabelField("Unique ID:", targetScript.id);*/
    }
}
#endif
