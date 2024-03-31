using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogText : MonoBehaviour
{
    [SerializeField] private float messageExistingTime = 0;

    public void Message(string message)
    {
        messageExistingTime = 5;
        gameObject.GetComponent<TextMeshProUGUI>().text = message;
    }

    public void Update()
    {
        if (messageExistingTime > 0) { messageExistingTime -= Time.deltaTime; }
        else { gameObject.GetComponent<TextMeshProUGUI>().text = ""; }
    }
}
