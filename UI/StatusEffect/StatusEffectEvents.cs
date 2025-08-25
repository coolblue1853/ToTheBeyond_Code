using System;
using System.Collections.Generic;
using UnityEngine;

public static class StatusEffectEvents
{
    // 버프나 디버프가 적용될 때 호출
    public static Action<SkillSO> OnEffectApplied;

    // 효과가 강제로 제거될 때 호출
    public static Action<string> OnEffectRemoved;
}
