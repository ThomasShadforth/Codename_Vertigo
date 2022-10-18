using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageInterface
{
    //Interface used to damage objects such as players, enemies, obstacles, etc.
    void Damage(float damageDealt, Transform attackerPos = null);

    void CheckForHealth();
}
