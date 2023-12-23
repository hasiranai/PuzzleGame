using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;                       // ⭐︎ 追加します

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text txtScore;

    [SerializeField]
    private Text txtTimer;

    [SerializeField]
    private Shuffle shuffle;

    [SerializeField]
    private Button btnShuffle;

    /// <summary>
    /// UIManagerの初期設定
    /// </summary>
    /// <returns></returns>
    public IEnumerator Initialize()
    {
        // シャッフルボタンを非活性化(半透明の押せない状態にする)
        ActivateShuffleButton(false);

        // シャッフル機能の設定
        shuffle.SetUpShuffle(this);

        // シャッフルボタンにメソッドを登録
        btnShuffle.onClick.AddListener(TriggerShuffle);

        yield break;
    }

    /// <summary>
    /// シャッフルボタンの活性化/非活性化の切り替え
    /// </summary>
    /// <param name="isSwitch"></param>
    public void ActivateShuffleButton(bool isSwitch)
    {
        btnShuffle.interactable = isSwitch;
    }

    /// <summary>
    /// シャッフル実行
    /// </summary>
    private void TriggerShuffle()
    {
        // シャッフルボタンを押せなくする。重複タップ防止
        ActivateShuffleButton(false);

        // シャッフル開始
        shuffle.StartShuffle();
    }

    /// <summary>
    /// 画面表示スコアの更新処理
    /// </summary>
    public void UpdateDisplayScore()
    {
        // 画面に表示しているスコアの値を更新
        txtScore.text = GameData.instance.score.ToString();
    }

    /// <summary>
    /// ゲームの残り時間の表示更新
    /// </summary>
    /// <param name="time"></param>
    public void UpdateDisplayGameTime(float time)
    {
        txtTimer.text = time.ToString("F0");
    }
}
