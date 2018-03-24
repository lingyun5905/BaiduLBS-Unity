using UnityEngine;
using UnityEngine.UI;

public class Demo : MonoBehaviour
{
    [SerializeField]
    private Button btnLoc;
    [SerializeField]
    private Text txtContent;

    private string location;

    void Start()
    {
        LocationManager.Instance.SetAppKey(Configs.AppKey);
        btnLoc.onClick.AddListener(delegate ()
        {
            LocationManager.Instance.ReqLocation(delegate (LocationInfo li)
            {
                location = JsonUtility.ToJson(li);
                txtContent.text = location;
                Debug.Log("data => " + location);
            });
        });
    }
}
