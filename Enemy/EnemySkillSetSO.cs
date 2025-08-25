using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Enemy/SkillSet")]
public class EnemySkillSetSO : ScriptableObject
{
    public List<SkillProfileSO> skills;
}
