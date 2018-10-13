using Assets.Scripts;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Camera[] cameras;
    public static int lastCamera;

    Vector3 hit_position = Vector3.zero;
    Vector3 current_position = Vector3.zero;
    Vector3 camera_position = Vector3.zero;
    Vector3 offset;

    static int lim_left = -5, lim_right = 25;

    void Start()
    {
        offset = transform.position - AvatarController.avatar.gameObject.transform.position;
    }

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
        if (!Input.GetMouseButton(0))
        {
            transform.position = AvatarController.avatar.gameObject.transform.position + offset;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // hit_position = Input.mousePosition;
            // camera_position = transform.position;

        }
        if (Input.GetMouseButton(0))
        {
            // current_position = Input.mousePosition;
            // LeftMouseDrag();
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

        if (position.x < lim_left)
        {
            position.x = lim_left;
        }
        else if (position.x > lim_right)
        {
            position.x = lim_right;
        }

        transform.position = position;
    }



    public void BedroomCamera()
    {
        AvatarController.SetTarget(0);
    }

    public void KitchenCamera()
    {
        AvatarController.SetTarget(1);
    }

    public void OfficeCamera()
    {
        AvatarController.SetTarget(3);
    }

    public void ChangeTo(int cam)
    {

    }

}
