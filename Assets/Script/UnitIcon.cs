using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UnitIcon : MonoBehaviour
{
    Button button;
    public Text hpText;
    public Text nameText;
    public Slider slider;
    public Object unit;
    public byte index;
    int timer;
    Panel panel;
    Image image;
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
        panel = FindAnyObjectByType<Panel>();
        image = GetComponent<Image>();
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
        button.interactable = false;
        if (panel.objectUnit.TryGetComponent(out Building building))
        {
            timer = unit.respawnSpeed-building.timer;
        }
        image.sprite = unit.GetComponent<SpriteRenderer>().sprite;
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
    void UpdateIconStats()
    {
        if (unit == null)
        {
            typeOfIcon = TypeOfIcon.Disabled;
        }
        slider.gameObject.SetActive(true);
        button.onClick.AddListener(unit.ChooseUnit);
        button.interactable = true;
        image.sprite = unit.GetComponent<SpriteRenderer>().sprite;
        hpText.text = "HP:" + unit.hp + "/" + unit.maxhp;
        slider.value = unit.hp;
        slider.maxValue = unit.maxhp;
        nameText.text = unit.name;
    }

}
