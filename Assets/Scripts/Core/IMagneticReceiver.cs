using UnityEngine;

public interface IMagneticReceiver
{
    void ApplyMagneticForce(Vector3 origin, float force, MagneticChargeType sourceCharge);
    MagneticChargeType GetChargeType();
}
