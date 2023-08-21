using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationFinished : MonoBehaviour
{
    // Start is called before the first frame update

    public UnityEvent myEvent;
    public void OnAnimationFinished()
    {
        myEvent.Invoke();
    }
}
