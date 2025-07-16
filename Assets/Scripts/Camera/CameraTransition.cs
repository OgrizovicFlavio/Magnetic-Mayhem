using UnityEngine;

public class CameraTransition
{
    private Transform cameraTransform;
    private float duration;
    private Vector3 startPos;
    private Quaternion startRot;
    private Vector3 endPos;
    private Quaternion endRot;
    private float elapsed;
    private bool active;

    public CameraTransition(Transform cameraTransform, float duration = 0.5f)
    {
        this.cameraTransform = cameraTransform;
        this.duration = duration;
        active = false;
    }

    public void StartTransition(Transform target)
    {
        startPos = cameraTransform.position;
        startRot = cameraTransform.rotation;

        Vector3 localOffset = cameraTransform.localPosition;
        endPos = target.TransformPoint(localOffset);
        endRot = cameraTransform.rotation;

        elapsed = 0f;
        active = true;

        GameManager.Instance.IsTransitioning = true;
    }

    public void Update()
    {
        if (!active) return;

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);

        cameraTransform.position = Vector3.Lerp(startPos, endPos, t);
        cameraTransform.rotation = Quaternion.Slerp(startRot, endRot, t);

        if (t >= 1f)
        {
            cameraTransform.position = endPos;
            cameraTransform.rotation = endRot;

            active = false;
            GameManager.Instance.IsTransitioning = false;
        }
    }

    public bool IsActive => active;
}
