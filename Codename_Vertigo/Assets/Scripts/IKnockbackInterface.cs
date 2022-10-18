using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKnockbackInterface
{
    IEnumerator KnockbackCo(Vector2 knockbackDir);
}
