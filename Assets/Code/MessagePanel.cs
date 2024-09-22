using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessagePanel : MonoBehaviour
{
    [SerializeField] GameObject messagePanel;
    [SerializeField] TMP_Text titleText, MessageText;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void SendMessage(string title1, string message1)
    {
        titleText.text = title1;
        MessageText.text = message1;
        messagePanel.SetActive(true);
        Time.timeScale = 0;
    }
}
