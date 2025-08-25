using System.Collections;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemySound : MonoBehaviour
{
    public void PlayAttackSound(string soundName)
    {
        if (!string.IsNullOrEmpty(soundName))
        {
            MasterAudio.PlaySound(soundName);
        }
    }
}
