using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class RoomButton : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI text;

    public RoomInfo roomInfo;

    public void SetUp(RoomInfo info)
    {
        roomInfo = info;
        text.text = roomInfo.Name;
    }


    public void OnClick()
    {
        if (roomInfo.IsOpen) Launcher.Instance.JoinRoom(roomInfo);
        else FindObjectOfType<LogText>().Message("You cant join ongoing match!");
    }
}
