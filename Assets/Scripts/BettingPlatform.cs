using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BettingPlatform : MonoBehaviour
{
    public int BetAmount = 0;
    private BettingManager _bettingManager;
    private int _platformNum;
    public Dictionary<Enums.ChipColor, PokerChip> BetChipStacks = new Dictionary<Enums.ChipColor, PokerChip>();

    private void Start()
    {
        _bettingManager = GameObject.FindObjectOfType<BettingManager>();

        if (gameObject.tag == "Red Platform")
            _platformNum = 0;
        else if (gameObject.tag == "Green Platform")
            _platformNum = 1;

        InitalizeChipStacks();
    }

    private void InitalizeChipStacks()
    {
        BetChipStacks = new Dictionary<Enums.ChipColor, PokerChip>
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
        foreach (KeyValuePair<Enums.ChipColor, PokerChip> chip in BetChipStacks)
        {
            chip.Value.EmptyChips();
            Debug.Log(chip.Key + ": " + chip.Value.GetNumOfChips());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PhotonView>().IsMine || PhotonNetwork.PlayerList.Length == 1)
        {
            int tagIndex = TagToInt(other.tag);
            BetAmount += _bettingManager.ChipStacks[(Enums.ChipColor)tagIndex].ChipValue;
            _bettingManager.BetWithChip(tagIndex, _platformNum);
            BetChipStacks[(Enums.ChipColor)tagIndex].AddChip();
            Debug.Log(BetAmount);
        }   
    }

    private void OnTriggerExit(Collider other)
    { 
        if (other.GetComponent<PhotonView>().IsMine || PhotonNetwork.PlayerList.Length == 1)
        {
            int tagIndex = TagToInt(other.tag);
            BetAmount -= _bettingManager.ChipStacks[(Enums.ChipColor)tagIndex].ChipValue;
            _bettingManager.TakeBackChip(tagIndex, _platformNum);
            BetChipStacks[(Enums.ChipColor)tagIndex].RemoveChip();
            Debug.Log(BetAmount);
        }
    }

    public void CollectChipWinnings(int randColor)
    {
        if (_platformNum == randColor)
        {
            int counter = BetChipStacks.Count - 1;
            int tempBetAmount = BetAmount;

            foreach (KeyValuePair<Enums.ChipColor, PokerChip> chip in BetChipStacks.Reverse())
            {
                tempBetAmount = chip.Value.CollectWinnings(tempBetAmount);

                int tempChipAmount = chip.Value.GetNumOfChips();
                _bettingManager.ChipStacks[chip.Key].AddMultipleChips(tempChipAmount);

                counter--;
            }
        }

        BetAmount = 0;
        EmptyBetChipStacks();
    }

    public int TagToInt(string convertTag)
    {
        if (convertTag == "Green Chip")
            return 0;
        else if (convertTag == "Black Chip")
            return 1;
        else if (convertTag == "Rose Chip")
            return 2;
        else if (convertTag == "Yellow Chip")
            return 3;
        else if (convertTag == "Orange Chip")
            return 4;
        else if (convertTag == "Olive Chip")
            return 5;
        else if (convertTag == "Pink Chip")
            return 6;
        else if (convertTag == "Purple Chip")
            return 7;
        else if (convertTag == "Brown Chip")
            return 8;
        else if (convertTag == "Red Chip")
            return 9;

        return -1;
    }

    public void EmptyBetChipStacks()
    {
        foreach (PokerChip chip in BetChipStacks.Values)
        {
            chip.EmptyChips();
        }
    }

    public int GetBetAmount()
    {
        return BetAmount;
    }

    public Dictionary<Enums.ChipColor, PokerChip> GetBetChipsStacks()
    {
        return BetChipStacks;
    }
}
