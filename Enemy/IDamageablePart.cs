using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageablePart
{
    void TakePartDamage(Vector3 attackerPosition, float baseDamage, bool isCrit);
}
