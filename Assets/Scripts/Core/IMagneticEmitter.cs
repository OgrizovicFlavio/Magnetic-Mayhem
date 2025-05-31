using UnityEngine;

public interface IMagneticEmitter
{
    Vector3 GetPosition();
    float GetEffectRadius();
    float GetForceStrength();
    MagneticChargeType GetChargeType();
}
