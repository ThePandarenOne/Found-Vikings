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
    Vector2 clickPosition = new Vector2();

    void Update()
    {
        clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (transform.position.x > 35)
        {
            transform.position = new Vector3(35,0,-10);
        }
        else if(transform.position.x < -35)
        {
            transform.position = new Vector3(-35,0,-10);
        }
        if (clickPosition.x >= transform.position.x + 26 || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(10, 0, 0) * Time.deltaTime;
        }
        else if (clickPosition.x <= transform.position.x + -26 || Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += new Vector3(-10, 0, 0) * Time.deltaTime;
        }
    }
}
