using UnityEngine;
using static Object;
using static UnityEngine.Rendering.DebugUI;

public class CameraScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    Vector2 clickPosition = new Vector2(-270, 270);
    Vector2 cursorPosition = new Vector2(-270, 270);
    void Update()
    {
        /*
        cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (cursorPosition )
        {

        }
        if (clickPosition.x > transform.position.x)
        {
            transform.position += new Vector3(speed, 0, 0) * Time.deltaTime;
        }
        else if (clickPosition.x < transform.position.x)
        {
            transform.position += new Vector3(-speed, 0, 0) * Time.deltaTime;
        }
        */
    }
    void MoveCamera()
    {
        clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
