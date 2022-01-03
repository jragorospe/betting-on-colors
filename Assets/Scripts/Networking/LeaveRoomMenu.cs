﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveRoomMenu : MonoBehaviour
{
    private LobbyCanvases _lobbyCanvases;

    public void FirstInitialize(LobbyCanvases canvases)
    {
        _lobbyCanvases = canvases;
    }

    public void OnClick_LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(true);
        _lobbyCanvases.CurrentRoomCanvas.Hide();
    }
}
