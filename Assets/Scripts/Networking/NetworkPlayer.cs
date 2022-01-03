using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using UnityEngine.UI;

public class NetworkPlayer : MonoBehaviourPun
{
    [SerializeField]
    private PhotonView _photonView;
    [SerializeField]
    private BettingManager _bettingManager;

    public static NetworkPlayer LocalPlayer = null;

    private void Start()
    {
        LocalPlayer = this;
        _bettingManager = GameObject.FindObjectOfType<BettingManager>();
    }

    public void BetUpdate(int totalWinnings)
    {
        _photonView.RPC("BetUpdate_RPC", RpcTarget.Others, totalWinnings);
    }

    [PunRPC]
    public void BetUpdate_RPC(int totalWinnings)
    {
        _bettingManager.UpdateOpponentUI(totalWinnings);
    }

    public void BetUpdate(int greenBetAmount, int redBetAmount, int totalWinnings)
    {
        _photonView.RPC("BetUpdate_RPC", RpcTarget.Others, greenBetAmount, redBetAmount, totalWinnings);
    }

    [PunRPC]
    public void BetUpdate_RPC(int greenBetAmount, int redBetAmount, int totalWinnings)
    {
        _bettingManager.UpdateOpponentUI(greenBetAmount, redBetAmount, totalWinnings);
    }

    public void BetUpdate(int chipNum, int chipAmount, int greenBetAmount, int redBetAmount, int totalWinnings)
    {
        _photonView.RPC("BetUpdate_RPC", RpcTarget.Others, chipNum, chipAmount, greenBetAmount, redBetAmount, totalWinnings);
    }

    [PunRPC]
    public void BetUpdate_RPC(int chipNum, int chipAmount, int greenBetAmount, int redBetAmount, int totalWinnings)
    {
        _bettingManager.UpdateOpponentUI(chipNum, chipAmount, greenBetAmount, redBetAmount, totalWinnings);
    }

    public void ChipsUpdate(int chipIndex, int chipAmount)
    {
        _photonView.RPC("ChipsUpdate_RPC", RpcTarget.Others, chipIndex, chipAmount);
    }

    [PunRPC]
    public void ChipsUpdate_RPC(int chipIndex, int chipAmount)
    {
        _bettingManager.UpdateOpponentChipAmounts(chipIndex, chipAmount);
    }

    public void BetIsQueued(bool isBet)
    {
        _photonView.RPC("BetIsQueued_RPC", RpcTarget.Others, isBet);
    }

    [PunRPC]
    public void BetIsQueued_RPC(bool isBet)
    {
        _bettingManager.UpdateOpponentBet(isBet);
    }

    public void AllBetsAreIn()
    {
        _photonView.RPC("AllBetsAreIn_RPC", RpcTarget.Others);
    }

    [PunRPC]
    public void AllBetsAreIn_RPC()
    {
        _bettingManager.MasterPlaceBet();
    }

    public void PlayersPlaceBets(int randColor)
    {
        _photonView.RPC("PlayersPlaceBets_RPC", RpcTarget.All, randColor);
    }

    [PunRPC]
    public void PlayersPlaceBets_RPC(int randColor)
    {
        _bettingManager.PlaceBet(randColor);
    }

    public void EnableNetworkChip(int viewID)
    {
        _photonView.RPC("EnableNetworkChip_RPC", RpcTarget.Others, viewID);
    }

    [PunRPC]
    public void EnableNetworkChip_RPC(int viewID)
    {
        _bettingManager.EnableOpponentChip(viewID);
    }

    public void DisableNetworkChip(int viewID)
    {
        _photonView.RPC("DisableNetworkChip_RPC", RpcTarget.Others, viewID);
    }

    [PunRPC]
    public void DisableNetworkChip_RPC(int viewID)
    {
        _bettingManager.DisableOpponentChip(viewID);
    }
}
