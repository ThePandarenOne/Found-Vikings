using UnityEngine;

public class SelectionSquare : MonoBehaviour
{
    public Group group;
    public Vector3 startPoint;
    Vector2 v;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.localScale = new Vector3(v.x, v.y);
        }
        if (!Input.GetMouseButton(0))
        {
            Destroy(gameObject);
        }
    }
}
