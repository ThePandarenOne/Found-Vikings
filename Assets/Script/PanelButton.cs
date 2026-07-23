using UnityEngine;
using UnityEngine.UI;

public class PanelButton : MonoBehaviour
{
    public ActionData action;
    private Image icon;
    public Text nameText;
    public Text costText;

    private Button button;
    public Button acsessButton
    {
        get { return button; }
        set { button = value; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        icon = GetComponent<Image>();
        button = GetComponent<Button>();
    }
    public void GetAction()
    {
        icon.sprite = action.icon;
        nameText.text = action.namE;
        costText.text = action.cost;
        //button.onClick.AddListener(action.);
    }

    // Update is called once per frame
    void Update()
    {
        if(action == false)
        {
            gameObject.SetActive(false);
        }
    }
}
