using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Linq;

public class BettingManager : MonoBehaviour
{
    [Header("Player Attributes")]
    public int GreenBetAmount;
    public int RedBetAmount;
    public int TotalWinnings;
    public int ChipsSelected;
    public const int ChipIncrement = 10;

    public Dictionary<Enums.ChipColor, PokerChip> ChipStacks = new Dictionary<Enums.ChipColor, PokerChip>();
    public List<Text> ChipAmountsText = new List<Text>();
    public ChipSpawner ChipSpawner;

    public Text QueuedText;
    public Text RedBetAmountText;
    public Text GreenBetAmountText;
    public Text TotalWinningsText;
    public Text ColorText;
    public Text ChipsSelectedText;

    [Header("Opponent Attributes")]
    public List<Text> OpponentChipAmountsText = new List<Text>();

    public Text OpponentGreenBetAmountText;
    public Text OpponentRedBetAmountText;
    public Text OpponentTotalWinningsText;

    private bool _betIsPlaced = false;
    private bool _opponentBetIsPlaced = false;

    public BettingPlatform RedPlatform;
    public BettingPlatform GreenPlatform;

    void Start()
    {
        GreenBetAmount = 0;
        RedBetAmount = 0;
        TotalWinnings = 0;
        ChipsSelected = 0;

        InitalizeChipStacks();
    }

    private void InitalizeChipStacks()
    {
        ChipStacks = new Dictionary<Enums.ChipColor, PokerChip>
        {
            { Enums.ChipColor.Green, new PokerChip { ChipValue = 1 } },
            { Enums.ChipColor.Black, new PokerChip { ChipValue = 5 } },
            { Enums.ChipColor.Rose, new PokerChip { ChipValue = 10 } },
            { Enums.ChipColor.Yellow, new PokerChip { ChipValue = 20 } },
            { Enums.ChipColor.Orange, new PokerChip { ChipValue = 25 } },
            { Enums.ChipColor.Olive, new PokerChip { ChipValue = 50 } },
            { Enums.ChipColor.Pink, new PokerChip { ChipValue = 100 } },
            { Enums.ChipColor.Purple, new PokerChip { ChipValue = 250 } },
            { Enums.ChipColor.Brown, new PokerChip { ChipValue = 500 } },
            { Enums.ChipColor.Red, new PokerChip { ChipValue = 1000 } },
        };

        ResetAllChips();
    }

    public void ResetAllChips()
    {
        foreach (KeyValuePair<Enums.ChipColor, PokerChip> chip in ChipStacks)
        {
            chip.Value.ReloadChips();
            Debug.Log(chip.Key + ": " + chip.Value.GetNumOfChips());
        }

        CalculateTotalWinnings();
    }

    public int GetChipCount()
    {
        int chipCount = 0;

        foreach (PokerChip chip in ChipStacks.Values)
        {
            chipCount += chip.GetNumOfChips();
        }

        return chipCount;
    }

    public void BetWithChip(int chipNum, int platformNum)
    {
        Enums.ChipColor color = (Enums.ChipColor)chipNum;

        if (ChipStacks.TryGetValue(color, out PokerChip chip))
        {
            if (platformNum == 0)
            {
                RedBetAmount += chip.ChipValue;
                RedBetAmountText.text = ("$" + RedBetAmount.ToString());
            }
            else if (platformNum == 1)
            {
                GreenBetAmount += chip.ChipValue;
                GreenBetAmountText.text = ("$" + GreenBetAmount.ToString());
            }

            TotalWinnings -= chip.ChipValue;
            chip.RemoveChip();
            ChipsSelected++;
            ChipsSelectedText.text = (ChipsSelected.ToString() + " / " + RoundUp(ChipsSelected).ToString());
            ChipAmountsText[chipNum].text = ("x" + chip.GetNumOfChips().ToString());
            TotalWinningsText.text = ("$" + TotalWinnings.ToString());

            if (PhotonNetwork.IsConnected)
            {
                NetworkPlayer.LocalPlayer.BetUpdate(chipNum, chip.GetNumOfChips(), GreenBetAmount, RedBetAmount, TotalWinnings);
            }
        }
    }

    public void TakeBackChip(int chipNum, int platformNum)
    {
        Enums.ChipColor color = (Enums.ChipColor)chipNum;

        if (ChipStacks.TryGetValue(color, out PokerChip chip))
        {
            if (platformNum == 0)
            {
                RedBetAmount -= chip.ChipValue;
                RedBetAmountText.text = ("$" + RedBetAmount.ToString());
            }
            else if (platformNum == 1)
            {
                GreenBetAmount -= chip.ChipValue;
                GreenBetAmountText.text = ("$" + GreenBetAmount.ToString());
            }

            TotalWinnings += chip.ChipValue;
            chip.AddChip();
            ChipsSelected--;
            ChipsSelectedText.text = (ChipsSelected.ToString() + " / " + RoundUp(ChipsSelected).ToString());
            ChipAmountsText[chipNum].text = ("x" + chip.GetNumOfChips().ToString());
            TotalWinningsText.text = ("$" + TotalWinnings.ToString());

            if (PhotonNetwork.IsConnected)
            {
                NetworkPlayer.LocalPlayer.BetUpdate(chipNum, chip.GetNumOfChips(), GreenBetAmount, RedBetAmount, TotalWinnings);
            }  
        }
    }

    public void CalculateTotalWinnings()
    {
        int counter = 0;
        TotalWinnings = 0;

        foreach (KeyValuePair<Enums.ChipColor, PokerChip> chip in ChipStacks)
        {
            TotalWinnings += chip.Value.GetTotalValue();
            UpdateChipAmounts(counter, chip.Value.GetNumOfChips());
            ChipSpawner.SpawnChip((int)chip.Key, chip.Value.GetNumOfChips());

            if (PhotonNetwork.IsConnected)
            {
                NetworkPlayer.LocalPlayer.ChipsUpdate(counter, chip.Value.GetNumOfChips());
            }

            counter++;
        }

        TotalWinningsText.text = ("$" + TotalWinnings.ToString());

        if (PhotonNetwork.IsConnected)
        {
            NetworkPlayer.LocalPlayer.BetUpdate(TotalWinnings);
            Debug.Log(TotalWinnings);
        }
    }

    public void RedistributeChips()
    {
        int counter = ChipStacks.Count - 1;
        int tempAmount = 0;

        foreach (KeyValuePair<Enums.ChipColor, PokerChip> chip in ChipStacks.Reverse())
        {
            if (GetChipCount() > 10)
                return;

            if (chip.Value.GetNumOfChips() > 0)
            { 
                tempAmount = chip.Value.ChipValue;
                chip.Value.RemoveChip();
            }  
            else
            {
                tempAmount = 0;
            }
                
            tempAmount = chip.Value.CollectWinnings(tempAmount);
            Debug.Log(chip.Key + ": " + chip.Value.GetNumOfChips());

            UpdateChipAmounts(counter, chip.Value.GetNumOfChips());

            if (PhotonNetwork.IsConnected)
            {
                NetworkPlayer.LocalPlayer.ChipsUpdate(counter, chip.Value.GetNumOfChips());
            }

            counter--;
        }

        if (TotalWinnings < ChipStacks[0].ChipValue * ChipIncrement)
        {
            ResetAllChips();
            QueuedText.text = "Not Enough Chips";
        }
        else
        {
            RedistributeChips();
        }
    }

    public void UpdateChipAmounts(int chipIndex, int chipAmount)
    {
        ChipAmountsText[chipIndex].text = ("x" + chipAmount.ToString());
    }

    public void UpdateOpponentChipAmounts(int chipIndex, int chipAmount)
    {
        OpponentChipAmountsText[chipIndex].text = ("x" + chipAmount.ToString());
    }

    public int RoundUp(int toRound)
    {
        if (toRound % 10 == 0 && toRound != 0)
            return toRound;

        return (10 - toRound % 10) + toRound;
    }

    public void UpdateOpponentUI(int totalWinnings)
    {
        OpponentTotalWinningsText.text = ("$" + totalWinnings.ToString());
    }

    public void UpdateOpponentUI(int greenBetAmount, int redBetAmount, int totalWinnings)
    {
        OpponentGreenBetAmountText.text = ("$" + greenBetAmount.ToString());
        OpponentRedBetAmountText.text = ("$" + redBetAmount.ToString());
        OpponentTotalWinningsText.text = ("$" + totalWinnings.ToString());
    }

    public void UpdateOpponentUI(int chipNum, int chipAmount, int greenBetAmount, int redBetAmount, int totalWinnings)
    {
        OpponentChipAmountsText[chipNum].text = ("x" + chipAmount.ToString());
        OpponentGreenBetAmountText.text = ("$" + greenBetAmount.ToString());
        OpponentRedBetAmountText.text = ("$" + redBetAmount.ToString());
        OpponentTotalWinningsText.text = ("$" + totalWinnings.ToString());
    }

    public void BetQueued()
    {
        if (ChipsSelected % ChipIncrement == 0 && ChipsSelected != 0 && !_betIsPlaced)
        {
            _betIsPlaced = true;
            GreenBetAmount = GreenPlatform.BetAmount;
            RedBetAmount = RedPlatform.BetAmount;

            DisableChipMovement();

            if (PhotonNetwork.IsConnected && PhotonNetwork.PlayerList.Length > 1)
            {
                NetworkPlayer.LocalPlayer.BetIsQueued(_betIsPlaced);
                
                if (!_betIsPlaced || !_opponentBetIsPlaced)
                {
                    QueuedText.text = "Waiting on Opponent...";
                    return;
                }

                if (PhotonNetwork.IsMasterClient)
                {
                    MasterPlaceBet();
                }
                else
                {
                    NetworkPlayer.LocalPlayer.AllBetsAreIn();
                }
            }
            
            else
            {
                MasterPlaceBet();
            }
        }

        else
        {
            QueuedText.text = "Please select " + (ChipIncrement - ChipsSelected % ChipIncrement) + " more chips.";
        }
    }

    public void DisableAllChips()
    {
        foreach (Transform child in ChipSpawner.transform)
        {
            child.gameObject.SetActive(false);
            
            if (PhotonNetwork.IsConnected)
            {
                NetworkPlayer.LocalPlayer.DisableNetworkChip(child.GetComponent<PhotonView>().ViewID);
            }
        }
    }

    public void DisableChipMovement()
    {
        foreach (Transform child in ChipSpawner.transform)
        {
            child.gameObject.GetComponent<MoveItem>().DisableMovement();
        }
    }

    public void EnableChipMovement()
    {
        foreach (Transform child in ChipSpawner.transform)
        {
            child.gameObject.GetComponent<MoveItem>().EnableMovement();
        }
    }

    public void EnableOpponentChip(int viewID)
    {
        PhotonView.Find(viewID).gameObject.SetActive(true);
    }

    public void DisableOpponentChip(int viewID)
    {
        PhotonView.Find(viewID).gameObject.SetActive(false);
    }


    public void UpdateOpponentBet(bool isBet)
    {
        _opponentBetIsPlaced = isBet;
    }

    public void MasterPlaceBet()
    {
        int randColor = Random.Range(0, 2);

        if (PhotonNetwork.IsConnected)
        {
            NetworkPlayer.LocalPlayer.PlayersPlaceBets(randColor);
        }
        else
        {
            PlaceBet(randColor);
        }
    }


    public void PlaceBet(int randColor)
    {
        QueuedText.text = "";

        if (randColor == 1)
        {
            ColorText.text = "GREEN";
            ColorText.color = Color.green;
        }
        else
        {
            ColorText.text = "RED";
            ColorText.color = Color.red;
        }

        RedPlatform.CollectChipWinnings(randColor);
        GreenPlatform.CollectChipWinnings(randColor);
        DisableAllChips();

        if (TotalWinnings <= 0)
        {
            ResetAllChips();
            QueuedText.text = "Bankrupt! Chips Reset";
        }
        else
        {
            RedistributeChips();
        }

        CalculateTotalWinnings();

        GreenBetAmount = 0;
        RedBetAmount = 0;
        ChipsSelected = 0;

        RedBetAmountText.text = ("$" + RedBetAmount.ToString());
        GreenBetAmountText.text = ("$" + GreenBetAmount.ToString());
        TotalWinningsText.text = ("$" + TotalWinnings.ToString());
        ChipsSelectedText.text = (ChipsSelected.ToString() + " / " + RoundUp(ChipsSelected).ToString());

        _betIsPlaced = false;
        _opponentBetIsPlaced = false;

        EnableChipMovement();

        if (PhotonNetwork.IsConnected)
        {
            NetworkPlayer.LocalPlayer.BetUpdate(GreenBetAmount, RedBetAmount, TotalWinnings);
        }
    }
}
