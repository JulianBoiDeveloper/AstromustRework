using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class TechTreeButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Item1 myItem;
    public bool isResearched = false;
    [HideInInspector] public string techID;
    [HideInInspector] public Image slot;

    [SerializeField] TechTreeButton nextToUnlock;

    public float timeToUnlock = 5f;
    [HideInInspector] public float timer = 0f;

    private void Awake()
    {
        slot = GetComponent<Image>();

        techID = myItem.id;
        slot.sprite = myItem.icon;
        if(nextToUnlock != null && nextToUnlock.slot != null)
        {
            nextToUnlock.slot.raycastTarget = false;
        }

        if(isResearched)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            if (nextToUnlock != null)
            {
                nextToUnlock.slot.raycastTarget = true;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isResearched || TechTree.Instance.queueToUnlock.Contains(this)) return;

        GameObject clickedButton = eventData.pointerPress.gameObject;
        Debug.Log("Clicked button: " + clickedButton.name);

        if(TechTree.Instance.queueToUnlock.Count < 3) {
            TechTree.Instance.AddItemQueue(this);

            if (nextToUnlock != null)
            {
                nextToUnlock.slot.raycastTarget = true;
            }
        }
    }

    public void Unlock()
    {
        timer += Time.deltaTime;
        Debug.Log("Unlocking...");
        if(timer > timeToUnlock)
        {
            isResearched = true;
            transform.GetChild(0).gameObject.SetActive(false);

            TechTree.Instance.queueToUnlock.Remove(this);
            TechTree.Instance.OnItemFinishedUnlock();
        }
    }
}