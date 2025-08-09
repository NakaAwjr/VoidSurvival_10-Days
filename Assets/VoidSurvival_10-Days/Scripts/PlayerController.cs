using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D _rigidbody;
    private Vector2 _moveInput;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get input from the virtual joystick
        _moveInput.x = CrossPlatformInputManager.GetAxis("Horizontal");
        _moveInput.y = CrossPlatformInputManager.GetAxis("Vertical");

        // Normalize the input vector to prevent faster diagonal movement
        if (_moveInput.magnitude > 1f)
        {
            _moveInput.Normalize();
        }

        _rigidbody.velocity = _moveInput * moveSpeed;
    }
}
