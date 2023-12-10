using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    [Header("ゲームに登場する干支の最大種類数")]
    public int etoTypeCount = 5;

    [Header("ゲーム開始時に生成する干支の数")]
    public int createEtoCount = 50;

    [Header("現在のスコア")]
    public int score = 0;

    [Header("干支を消した際に加算されるスコア")]
    public int etoPoint = 100;

    [Header("消した干支の数")]
    public int eraseEtoCount = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // ゲームの初期化
        InitGame();
    }

    /// <summary>
    /// ゲーム初期化
    /// </summary>
    private void InitGame()
    {
        score = 0;
        eraseEtoCount = 0;
        Debug.Log("Init Game");
    }
}