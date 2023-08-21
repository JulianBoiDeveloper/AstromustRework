using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetEnemyDie : MonoBehaviour
{
    [SerializeField] EnemyAI enemyAI;

    // Called from animataion event
    public void OnEnemyDie()
    {
    //    enemyAI.OnDeathAnimationEnd();
    }
}
