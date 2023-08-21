using UnityEngine;

public class UnlockShop : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public bool unlock_game;
    public bool extra_gun;
    public bool powerup;
    public bool random_box;
    public bool turret;
}
