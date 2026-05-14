using UnityEngine;

public class VRViewRecenter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Transform vrCamera;

    [Header("Default Look Target")]
    [SerializeField] private Transform defaultLookTarget;

    [Header("Settings")]
    [SerializeField] private bool yawOnly = true;

    public void FaceDefaultTarget()
    {
        if (defaultLookTarget == null)
        {
            Debug.LogWarning("VRViewRecenter: No default look target assigned.");
            return;
        }

        FaceTarget(defaultLookTarget);
    }

    public void FaceTarget(Transform target)
    {
        if (playerRoot == null || vrCamera == null || target == null)
        {
            Debug.LogWarning("VRViewRecenter: Missing reference.");
            return;
        }

        Vector3 cameraForward = vrCamera.forward;
        Vector3 targetDirection = target.position - vrCamera.position;

        if (yawOnly)
        {
            cameraForward.y = 0f;
            targetDirection.y = 0f;
        }

        if (cameraForward.sqrMagnitude < 0.0001f || targetDirection.sqrMagnitude < 0.0001f)
            return;

        cameraForward.Normalize();
        targetDirection.Normalize();

        float angle = Vector3.SignedAngle(cameraForward, targetDirection, Vector3.up);

        ResetHeadToWorldCenter();
        playerRoot.RotateAround(vrCamera.position, Vector3.up, angle);
  
    }

    public void ResetHeadToWorldCenter()
    {
        if (playerRoot == null || vrCamera == null)
            return;

        Vector3 delta = Vector3.zero - vrCamera.position;

        // Keep real headset height.
        delta.y = 0f;

        playerRoot.position += delta;
    }
}
