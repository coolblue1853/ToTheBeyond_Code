using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectController : MonoBehaviour
{
    [SerializeField] private Transform footPoint; 

    private ShakeEffect shakeEffect;
    private BlinkEffect blinkEffect;
    private GhostTrail ghostTrail; // 대쉬 고스트
    private void Awake()
    {
        shakeEffect = GetComponent<ShakeEffect>();  
        blinkEffect = GetComponent<BlinkEffect>();
        //ghostTrail = GetComponent<GhostTrail>();
    }
    public void PlayShakePlayerEffect()
    {
        shakeEffect.Shake();
    }
    public void PlayBlinkEffect(float duration)
    {
        blinkEffect.StartBlink(duration);
    }
    public void PlayJumpEffect()
    {
        ParticleManager.Instance.Play(ParticleType.JumpDust, footPoint.position);
    }
    public void PlayLandEffect()
    {
        ParticleManager.Instance.Play(ParticleType.LandDust, footPoint.position);
    }
    public void PlayTrailEffect()
    {
     //   ghostTrail.StartTrail();
    }
    public void StopTrailEffect()
    {
       // ghostTrail.StopTrail();
    }
}
