using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UnitIcon : MonoBehaviour
{
    Button button;
    public ActionData action;
    private Sprite icon;
    public Text hpText;
    public Text nameText;
    public Slider slider;
    public Object unit;
    int timer;
    public enum TypeOfIcon
    {
        UnitQueueIcon,
        UnitGroupIcon,
        Disabled
    }
    public TypeOfIcon typeOfIcon = TypeOfIcon.Disabled;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (typeOfIcon)
        {
            case TypeOfIcon.UnitQueueIcon:
                UpdateUnitInQueue();
                break;
                case TypeOfIcon.UnitGroupIcon:
                UpdateIconStats();
                break ;
                case TypeOfIcon.Disabled:
                gameObject.SetActive(false);
                break;
        }
    }
    void UpdateUnitInQueue()
    {
        button.onClick.RemoveAllListeners();
        if(unit.TryGetComponent(out Building building))
        {
            timer = building.timer;
        }
        icon = action.icon;
        nameText.text = action.name;
        hpText.text = timer + "/" +unit.respawnSpeed;
        slider.value = timer;
        slider.maxValue = unit.respawnSpeed;
    }
    void UpdateIconStats()
    {
        button.onClick.AddListener(unit.ChooseUnit);
        icon = unit.GetComponent<SpriteRenderer>().sprite;
        hpText.text = "HP:" + unit.hp + "/" + unit.maxhp;
        slider.value = unit.hp;
        slider.maxValue = unit.maxhp;
        nameText.text = unit.name;
    }

}
