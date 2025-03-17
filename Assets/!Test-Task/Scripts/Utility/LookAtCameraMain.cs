using UnityEngine;

public class LookAtCameraMain : MonoBehaviour
{
    public void Update()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(Camera.main.transform.position - transform.position, Vector3.up));
    }
}