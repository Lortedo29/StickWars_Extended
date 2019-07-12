﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharID
{
    red = 0,
    blue = 1,
    green = 2,
    yellow = 3
}

public struct CharControls
{
    public KeyCode Left { get; private set; }
    public KeyCode Right { get; private set; }
    public KeyCode Jump { get; private set; }
    public KeyCode Tackle { get; private set; }

    public CharControls(KeyCode left, KeyCode right, KeyCode jump, KeyCode tackle)
    {
        Left = left;
        Right = right;
        Jump = jump;
        Tackle = tackle;
    }
}

static class CharIDExtensions
{
    public static Color ToColor(this CharID playerId)
    {
        switch (playerId)
        {
            case CharID.red:
                return "c40233".HexToColor();

            case CharID.blue:
                return "0088bf".HexToColor();

            case CharID.green:
                return "00a568".HexToColor();

            case CharID.yellow:
                return "ffd400".HexToColor();
        }

        return Color.white;
    }

    public static CharControls ToControls(this CharID playerId)
    {
        switch (playerId)
        {
            case CharID.red:
                return new CharControls(KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow);

            case CharID.blue:
                return new CharControls(KeyCode.Q, KeyCode.D, KeyCode.Z, KeyCode.S);

            case CharID.green:
                return new CharControls(KeyCode.K, KeyCode.M, KeyCode.O, KeyCode.L);

            case CharID.yellow:
                return new CharControls(KeyCode.Keypad4, KeyCode.Keypad6, KeyCode.Keypad8, KeyCode.Keypad5);
        }

        return new CharControls(KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3);
    }
}