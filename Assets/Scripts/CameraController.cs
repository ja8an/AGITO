using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Camera[] cameras;
    private int lastCamera;

    Vector3 hit_position = Vector3.zero;
    Vector3 current_position = Vector3.zero;
    Vector3 camera_position = Vector3.zero;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            hit_position = Input.mousePosition;
            camera_position = transform.position;

        }
        if (Input.GetMouseButton(0))
        {
            current_position = Input.mousePosition;
            LeftMouseDrag();
        }
    }

    void LeftMouseDrag()
    {
        // From the Unity3D docs: "The z position is in world units from the camera."  In my case I'm using the y-axis as height
        // with my camera facing back down the y-axis.  You can ignore this when the camera is orthograhic.
        current_position.z = hit_position.z = camera_position.y;

        // Get direction of movement.  (Note: Don't normalize, the magnitude of change is going to be Vector3.Distance(current_position-hit_position)
        // anyways.  
        Vector3 direction = Camera.main.ScreenToWorldPoint(current_position) - Camera.main.ScreenToWorldPoint(hit_position);

        // Invert direction to that terrain appears to move with the mouse.
        direction = direction * -1;

        Vector3 position = new Vector3(camera_position.x + direction.x, camera_position.y, camera_position.z);

        if (position.x < -11)
            position.x = -11;
        else if (position.x > 4)
            position.x = 4;

        transform.position = position;
    }

    public void changeTo(int index)
    {

        if (index > cameras.Length - 1)
            return;

        if (index < 0)
        {
            gameObject.SetActive(true);
            cameras[lastCamera].gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
            lastCamera = index;
            cameras[index].gameObject.SetActive(true);
        }

    }

}
