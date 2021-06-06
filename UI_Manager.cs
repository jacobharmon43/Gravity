using UnityEngine;
using TMPro;

public class UI_Manager : MonoBehaviour
{

    public TextMeshProUGUI gravityModeText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateGravityModeText(string S)
    {
        gravityModeText.text = "Gravity Mode: " + S;
    }
}
