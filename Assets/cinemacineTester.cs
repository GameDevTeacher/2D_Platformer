using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class cinemacineTester : MonoBehaviour
{
    public CinemachineVirtualCamera _VirtualCamera;
    
    public bool isLooking;

    public PlayerInputCopy input;
    public Rigidbody2D _rb;
    

    void Update()
    {
        _VirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_DeadZoneHeight = _rb.linearVelocity != Vector2.zero ? 0.22f : 0f;

        if (input.Move().x == 0)
        {
            //ScreenYCameraLook();
            TrackedPointOffsetCameraLook();
        }
       
    }

    private void ScreenYCameraLook()
    {
        float screenY = _VirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY;
        
        
        if (Keyboard.current.downArrowKey.isPressed)
        {
            screenY = Mathf.Lerp(screenY, .2f, 1f);
        }
        else if (Keyboard.current.upArrowKey.isPressed)
        {
            screenY = Mathf.Lerp(screenY, 0.8f, 1f);
        }
        else
        {
            screenY = Mathf.Lerp(screenY, 0.5f, 1f);
        }
        

        _VirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = screenY;
    }

    void TrackedPointOffsetCameraLook()
    {
        float screenY = _VirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset.y;

        
        if (Keyboard.current.downArrowKey.isPressed)
        {
            screenY = Mathf.Lerp(screenY, -5f, 1f);

        }
        else if (Keyboard.current.upArrowKey.isPressed)
        {
            screenY = Mathf.Lerp(screenY, 5f, 1f);
        }
        else
        {
            screenY = Mathf.Lerp(screenY, 0f, 1f);
        }
 

        _VirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = new Vector3(0f, screenY, 0f);
    }
}
