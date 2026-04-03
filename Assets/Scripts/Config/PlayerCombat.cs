using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Weapon weapon;

    public void EnableHitbox()
    {
        if (weapon!= null) weapon.EnableHitbox();
    }

    public void DisableHitbox()
    {
        if (weapon!= null) weapon.DisableHitbox();
    }
}