using UnityEngine;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{
    PluginManager pm = new PluginManager();
    public Button btn;

    private void Start()
    {
        btn.onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick()
    {

    }
}
