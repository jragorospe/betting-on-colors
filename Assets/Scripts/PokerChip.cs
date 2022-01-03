using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokerChip
{
    public int ChipValue;
    public const int StartNumOfChips = 10;
    private int _numOfChips;

    public void AddChip()
    {
        _numOfChips++;
    }

    public void AddMultipleChips(int amount)
    {
        _numOfChips += amount;
    }

    public void RemoveChip()
    {
        _numOfChips--;
    }

    public void ReloadChips()
    {
        _numOfChips = StartNumOfChips;
    }

    public void EmptyChips()
    {
        _numOfChips = 0;
    }

    public int GetNumOfChips()
    {
        return _numOfChips;
    }

    public int CollectWinnings(int betAmount)
    {
        int newChips = 0;

        newChips = (int)Math.Floor((double)(betAmount / ChipValue));
        Debug.Log("New Chips: " + newChips);
        _numOfChips += newChips;

        return betAmount - (newChips * ChipValue);
    }

    public int GetTotalValue()
    {
        return _numOfChips * ChipValue;
    }

    public int GetStartNumChips()
    {
        return StartNumOfChips;
    }
}



