using UnityEngine;
using UnityEngine.UI;

public class Demo : MonoBehaviour
{
    [SerializeField]
    private Button btnLoc;
    [SerializeField]
    private Text txtContent;

    void Start()
    {
        LocationManager.Instance.SetAppKey(Configs.AppKey);
        btnLoc.onClick.AddListener(delegate ()
        {
            LocationManager.Instance.ReqLocation(delegate (LocationInfo li)
            {
                Debug.Log("data => " + JsonUtility.ToJson(li));
                txtContent.text = JsonUtility.ToJson(li);
            });
        });
    }

}
