using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PanelButton : MonoBehaviour
{
    Panel panel;
    public ActionData action;
    private Image icon;
    public Text nameText;
    public Text costText;
    public Text keyText;

    private Button button;
    public Button acsessButton
    {
        get { return button; }
        set { button = value; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //button.onClick.AddListener(() => Paid(action.cost));
        panel = FindAnyObjectByType<Panel>();
        icon = GetComponent<Image>();
        button = GetComponent<Button>();
    }
    public void GetAction()
    {
        icon.sprite = action.icon;
        nameText.text = action.namE;
        costText.text = action.cost.ToString();
        keyText.text = action.keyCode.ToString();
    }
    public void GetCost()
    {
        panel.playerManager.money -= action.cost;
    }
    // Update is called once per frame
    void Update()
    {
        if (action == null)
        {
            gameObject.SetActive(false);
        }
        if (action != null &&Input.GetKeyDown(action.keyCode.ToLower()))
        {
            button.onClick.Invoke();
        }
        if(action != null)
        {
            if (action.cost == 0)
            {
                costText.gameObject.SetActive(false);
            }
            else
            {
                costText.gameObject.SetActive(true);
            }
            if (action.cost > panel.playerManager.money)
            {
                button.interactable = false;
            }
            else
            {
                button.interactable = true;
            }
        }
    }
}
