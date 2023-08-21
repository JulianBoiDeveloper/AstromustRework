using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ManageJoystick : MonoBehaviour,IDragHandler,IPointerDownHandler,IPointerUpHandler
{
    private Image imgJoystickBg;
    private Image imgJoystickStick;

    private Vector2 posInput; 
    
    // Start is called before the first frame update
    void Start()
    {
        imgJoystickBg = GetComponent<Image>();
        imgJoystickStick = transform.GetChild(0).GetComponent<Image>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                imgJoystickBg.rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out posInput
            ))
        {
            var sizeDelta = imgJoystickBg.rectTransform.sizeDelta;
            posInput.x /= (sizeDelta.x);
            posInput.y /= (sizeDelta.y);


            if (posInput.magnitude > 1.0f)
            {
                posInput = posInput.normalized;
            }


            var delta = imgJoystickBg.rectTransform.sizeDelta;
            imgJoystickStick.rectTransform.anchoredPosition = 
                new Vector2(posInput.x * (delta.x /3)
                    , posInput.y * (delta.y)/3);
            
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        posInput = Vector2.zero;
        imgJoystickStick.rectTransform.anchoredPosition = Vector2.zero;
    }

    public float inputHorizontal()
    {
        if (posInput.x != 0)
        {
            return posInput.x;
        }
        else
        {
            return Input.GetAxis("Horizontal");
        }
    }
    public float inputVertical()
    {
        if (posInput.y != 0)
        {
            return posInput.y;
        }
        else
        {
            return Input.GetAxis("Vertical");
        }
    }
    
}
