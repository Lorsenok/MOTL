using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogText : MonoBehaviour
{
    [SerializeField] private float messageExistingTimeSet;
    private float messageExistingTime = 0;

    private TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void Message(string message)
    {
        messageExistingTime = messageExistingTimeSet;
        text.text = message;
    }

    public void Update()
    {
        if (messageExistingTime > 0) messageExistingTime -= Time.deltaTime;
        else text.text = "";
        text.color = new Color(1, 0, 0, 1 / messageExistingTimeSet * messageExistingTime);
    }
}
