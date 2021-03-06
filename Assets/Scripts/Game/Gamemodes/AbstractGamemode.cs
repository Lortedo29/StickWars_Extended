﻿using NDream.AirConsole;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public enum GamemodeType
{
    DeathMatch
}

public static class GamemodeTypeExtension
{
    public static AbstractGamemode ToGamemodeClass(this GamemodeType g)
    {
        switch (g)
        {
            case GamemodeType.DeathMatch:
                return new GamemodeDeathMatch();
        }

        return null;
    }
}

public delegate void IntArrayDelegate(int[] score, int scoreForVictory);

public abstract class AbstractGamemode
{
    #region Fields
    public static int valueForVictory = -1;

    public CharIdDelegate OnCharacterKill;
    public IntArrayDelegate OnScoreUpdate;

    protected Dictionary<CharId, int> _charactersValue = new Dictionary<CharId, int>();
    protected CharId? _mvp = null;

    private int _killCount = 0;
    #endregion

    #region Properties
    public int SumCharactersValue
    {
        get
        {
            int sum = 0;

            foreach (CharId item in Enum.GetValues(typeof(CharId)))
            {
                sum += _charactersValue[item];
            }

            return sum;
        }
    }

    public int[] CharactersValueArray
    {
        get
        {
            int[] charactersValueArray = new int[Enum.GetValues(typeof(CharId)).Length];

            foreach (CharId item in Enum.GetValues(typeof(CharId)))
            {
                charactersValueArray[(int)item] = _charactersValue[item];
            }

            return charactersValueArray;
        }
    }

    public int ValueForVictory { get => valueForVictory; set => valueForVictory = value; }
    public int KillCount { get => _killCount; }
    public int MaxKillsPossibleSum { get => GameManager.Instance.InstantiatedCharactersCount * valueForVictory; }
    public CharId? MVP { get => _mvp; }
    #endregion

    #region Methods
    public AbstractGamemode()
    {
        // init _charactersValue
        foreach (CharId item in Enum.GetValues(typeof(CharId)))
        {
            _charactersValue[item] = 0;
        }
    }

    public bool CheckForVictory()
    {
        Assert.AreNotEqual(valueForVictory, -1, "Value for victory has not be set!");

        foreach (CharId item in Enum.GetValues(typeof(CharId)))
        {
            if (_charactersValue[item] >= valueForVictory)
            {
                GameManager.Instance.Victory(item);
                Victory(item);

                return true;
            }
        }

        return false;
    }

    protected void CheckForNewMVP(CharId? charIDToCheck)
    {
        // prevent check for new mvp is charID is null
        if (charIDToCheck == null) return;

        // if there is no current mvp, let charId become the mvp
        if (_mvp == null)
        {
            _mvp = charIDToCheck;
            GameManager.Instance.Characters[(CharId)charIDToCheck].IsMVP = true;
            return;
        }

        // converting from CharId? to CharId
        CharId currentMVPCharIDConverted = (CharId)_mvp;
        CharId charIDToCheckConverted = (CharId)charIDToCheck;

        if (_charactersValue[currentMVPCharIDConverted] < _charactersValue[charIDToCheckConverted])
        {
            // remove isMvp status from old MVP
            GameManager.Instance.Characters[currentMVPCharIDConverted].IsMVP = false;

            // set new MVP
            _mvp = charIDToCheck;
            currentMVPCharIDConverted = charIDToCheckConverted;

            // set isMvp to true to new MVP
            GameManager.Instance.Characters[charIDToCheckConverted].IsMVP = true;
        }
    }

    protected abstract void Victory(CharId winnerID);

    public virtual void Kill(CharId? killerCharID, CharId victim)
    {
        TriggerKillEvent(killerCharID, victim);

        if (killerCharID == null)
        {
            Debug.LogWarningFormat("{0} has been killed by a NULL CharId", victim);
            return;
        }        

        _killCount++;

        OnCharacterKill?.Invoke((CharId)killerCharID);
    }

    void TriggerKillEvent(CharId? killerCharID, CharId victim)
    {
        // getting type
        var victimCharacterEntity = GameManager.Instance.Characters[victim].GetComponent<CharacterEntity>();
        AttackType characterAttackType = victimCharacterEntity.AttacksHistory.Last();
        string killType = characterAttackType.ToString();

        // getting players' name
        string victimName = victim.ToString();
        string killerName = killerCharID != null ? killerCharID.ToString() : "NONE";


        ExtendedAnalytics.SendEvent("Player Makes Kill", new Dictionary<string, object>()
        {
            { "Type", killType },
            { "Killer", killerName },
            { "Victim", victimName }
        });
    }

    #region Getter
    public int GetPlayersCountAtScore(int score)
    {
        return _charactersValue.Select(x => x.Value == score).Count();
    }

    public int GetPositionInPlayersAtScore(CharId charId)
    {
        int charIdScore = _charactersValue[charId];

        var array = _charactersValue
                        .Where(x => x.Value == charIdScore)
                        .OrderByDescending(x => x.Key)
                        .Select(x => x.Key)
                        .ToArray();

        return Array.FindIndex(array, x => x == charId);
    }

    public int GetScore(CharId charId)
    {
        return _charactersValue[charId];
    }

    /// <summary>
    /// Return -1 if there is no MVP.
    /// </summary>
    /// <returns></returns>
    public int GetMVPScore()
    {
        if (MVP == null)
            return -1;

        return _charactersValue[(CharId)MVP];
    }

    public CharId[] GetCharIdsOrderByScore()
    {
        CharId[] charIds = (CharId[])Enum.GetValues(typeof(CharId));

        CharId[] charIdsOrderByScore = charIds.OrderBy(x => GetScore(x)).ToArray();
        return charIdsOrderByScore;
    }
    #endregion
    #endregion
}
