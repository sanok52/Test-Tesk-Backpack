using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControl : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private InteractiveSystem interactiveSystem;

    private void Start()
    {
        if(player == null)
            player = GetComponent<Player>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Vector2 inputMove = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        player.Move(inputMove);
        Vector2 inputRotate = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        player.Rotate(inputRotate);
        interactiveSystem.SetInputData(Input.GetKeyDown(KeyCode.Mouse0), Input.GetKey(KeyCode.Mouse0), Input.GetKeyUp(KeyCode.Mouse0));
    }
}
