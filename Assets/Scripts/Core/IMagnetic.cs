using UnityEngine;

public enum MagneticChargeType
{
    Positive,
    Negative
}

public interface IMagnetic
{
    void ApplyMagneticForce(Vector3 origin, float force, MagneticChargeType chargeType);
    MagneticChargeType GetChargeType();
}
