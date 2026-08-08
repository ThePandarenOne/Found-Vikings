using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ActionData", menuName = "Scriptable Objects/ActionData")]
public class ActionData : ScriptableObject
{
    public Sprite icon;
    public string namE;
    public string keyCode;
    public int cost;
}
