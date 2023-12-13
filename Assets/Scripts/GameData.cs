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

    [SerializeField, Header("1回辺りのゲーム時間")]
    private int initTime = 60;

    [Header("現在のゲームの残り時間")]
    public float gameTime;

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

        // ゲーム時間を設定
        gameTime = initTime;

        Debug.Log("Init Game");
    }
}