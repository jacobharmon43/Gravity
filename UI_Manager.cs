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

    //Who really knows what this does
    public void UpdateGravityModeText(string S)
    {
        this.gravityModeText.text = "Gravity Mode: " + S;
    }
}
