using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UnitIcon : MonoBehaviour
{
    Button button;
    public Text hpText;
    public Text nameText;
    public Slider slider;
    public Entity unit;
    public byte index;
    int timer;
    public Panel panel;
    Image image;
    public enum TypeOfIcon
    {
        UnitQueueIcon,
        UnitGroupIcon,
        Building,
        Disabled
    }
    public TypeOfIcon typeOfIcon = TypeOfIcon.Disabled;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //panel = FindAnyObjectByType<Panel>();
        image = GetComponent<Image>();
        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (typeOfIcon)
        {
            case TypeOfIcon.UnitQueueIcon:
                if (panel.objectUnit != null &&!panel.objectUnit.GetComponent<Building>() || panel.objectUnit == null)
                {
                    typeOfIcon = TypeOfIcon.Disabled;
                }
                UpdateUnitInQueue();
                break;
            case TypeOfIcon.UnitGroupIcon:
                if(panel.group == null)
                {
                    typeOfIcon = TypeOfIcon.Disabled;
                }
                UpdateIconStats();
                break ;
            case TypeOfIcon.Building:
                if (!panel.objectUnit.GetComponent<BuildPlace>())
                {
                    typeOfIcon = TypeOfIcon.Disabled;
                }
                UpdateBuilding();
                break;
            case TypeOfIcon.Disabled:
                gameObject.SetActive(false);
                break;
        }
    }
    void UpdateUnitInQueue()
    {
        button.onClick.RemoveAllListeners();
        button.interactable = false;
        if (panel.objectUnit != null && panel.objectUnit.TryGetComponent(out Building building))
        {
            timer = unit.respawnSpeed-building.timer;
        }
        image.sprite = unit.spriteIcon;
        nameText.text = unit.name;
        if(gameObject.name == "Unit")
        {
            slider.value = timer;
            hpText.text = timer + "/" + unit.respawnSpeed;
        }
        else
        {
            hpText.text = "";
            slider.gameObject.SetActive(false);
        }
        slider.maxValue = unit.respawnSpeed;
    }
    void UpdateBuilding()
    {
        button.onClick.RemoveAllListeners();
        button.interactable = false;
        if (panel.objectUnit.TryGetComponent(out BuildPlace building))
        {
            timer = building.building.respawnSpeed - building.timer;
            image.sprite = building.building.spriteIcon;
            nameText.text = building.building.name;
            slider.value = timer;
            hpText.text = timer + "/" + building.building.respawnSpeed;
            slider.maxValue = building.building.respawnSpeed;
        }
        if(timer == building.respawnSpeed && typeOfIcon == TypeOfIcon.UnitQueueIcon)
        {
            typeOfIcon = TypeOfIcon.Disabled;
        }
    }
    void UpdateIconStats()
    {
        if (unit == null)
        {
            typeOfIcon = TypeOfIcon.Disabled;
        }
        else
        {
            slider.gameObject.SetActive(true);
            button.onClick.AddListener(() => unit.ChooseUnit(true));
            button.interactable = true;
            image.sprite = unit.spriteIcon;
            hpText.text = "HP:" + unit.hp + "/" + unit.maxhp;
            slider.value = unit.hp;
            slider.maxValue = unit.maxhp;
            nameText.text = unit.name;
        }
    }

}
