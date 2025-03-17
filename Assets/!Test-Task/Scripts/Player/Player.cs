using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private new Camera camera;
    private new Rigidbody rigidbody;

    [Space]
    [SerializeField] private float speed = 5f;
    [SerializeField] private Vector2 speedRotate = new Vector2(50f, 50f);
    [SerializeField] private Vector2 minMaxRotateY = new Vector2(-40f, 40f);

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Move(Vector2 direction)
    {
        rigidbody.MovePosition(transform.position + (transform.TransformDirection(new Vector3(direction.x, 0f, direction.y) * speed * Time.deltaTime)));
    }

    public void Rotate(Vector2 rotate)
    {
        // Поворот игрока по горизонтали
        transform.Rotate(new Vector3(0, rotate.x * speedRotate.x * Time.fixedDeltaTime, 0f));

        // Поворот камеры по вертикали
        float newRotationX = camera.transform.localEulerAngles.x + rotate.y * -speedRotate.y * Time.deltaTime;
        newRotationX = NormalizeAngle(newRotationX); // Нормализация угла
        newRotationX = Mathf.Clamp(newRotationX, minMaxRotateY.x, minMaxRotateY.y);

        camera.transform.localRotation = Quaternion.Euler(newRotationX, 0, 0);
    }

    // Нормализация угла
    private float NormalizeAngle(float angle)
    {
        while (angle > 180) angle -= 360;
        while (angle < -180) angle += 360;
        return angle;
    }
}