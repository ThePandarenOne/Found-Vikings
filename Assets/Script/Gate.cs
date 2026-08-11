using UnityEngine;
using System.Collections.Generic;
public class Gate : MonoBehaviour
{
    Dictionary<Unit, Vector2> unitsPlaces = new Dictionary<Unit, Vector2>();
    public Gate teleportGate;
    public Transform tpPlace;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Unit unit))
        {
            if (unitsPlaces.ContainsKey(unit) == false)
            {
                teleportGate.unitsPlaces.Add(unit, unit.transform.position);
                collision.transform.position = teleportGate.tpPlace.position;
            }
            else if(teleportGate == null)
            {
                collision.transform.position = unitsPlaces[unit];
                unitsPlaces.Remove(unit);
            }
        }
    }
}
