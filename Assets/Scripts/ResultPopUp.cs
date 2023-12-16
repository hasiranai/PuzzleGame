using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ResultPopUp : MonoBehaviour
{
    [SerializeField]
    private Text txtScore;

    [SerializeField]
    private Text txtEraseEtoCount;

    [SerializeField]
    private Button btnClosePopUp;

    private float posY;    // ResultPopUpゲームオブジェクトのY軸位置保存用(元の位置に戻す際に使う)

    void Start()
    {
        // ResultPopUpゲームオブジェクトのY軸の初期位置を保持
        posY = transform.position.y;

        // ボタンにメソッドを登録
        btnClosePopUp.onClick.AddListener(OnClickMovePopUp);

        // btnClosePopUpゲームオブジェクトの持つcanvasGruopのAlphaを 0 に設定して透明にしておく(最初はタップできず、かつ見えないようにしておく)
        btnClosePopUp.gameObject.GetComponent<CanvasGroup>().alpha = 0.0f;
    }

    /// <summary>
    /// ゲーム結果(点数と消した干支の数)をアニメ表示
    /// </summary>
    /// <param name="score"></param>
    /// <param name="eraseEtoCount"></param>
    public void DisplayResult(int score, int eraseEtoCount)
    {
        // 計算用の初期値を設定
        int initValue = 0;

        // DOTweenのSequence(シーケンス)機能を初期化して使用できるようにする
        Sequence sequence = DOTween.Sequence();

        // シーケンスを利用して、DOTweenの処理を制御したい順番で記述する。まずは①スコアの数字をアニメして表示
        sequence.Append(DOTween.To(() => initValue,
            (num) =>
            {
                initValue = num;
                txtScore.text = num.ToString();
            },
            score,
            1.0f).OnComplete(() => { initValue = 0; }).SetEase(Ease.InCirc));  // 次の処理のためにOnCompleteを使ってinitValueを初期化

        // ②シーケンス処理を0.5秒だけ待機
        sequence.AppendInterval(0.5f);

        // ③消した干支の数字をアニメして表示
        sequence.Append(DOTween.To(() => initValue,
            (num) =>
            {
                initValue = num;
                txtEraseEtoCount.text = num.ToString();
            },
            eraseEtoCount,
            1.0f).SetEase(Ease.InCirc));

        // ④シーケンス処理を0.5秒だけ待機
        sequence.AppendInterval(1.0f);

        // ⑤透明になっているbtnClosePopUpとその子要素をCanvasGroupのAlphaを使用して徐々に表示
        sequence.Append(btnClosePopUp.gameObject.GetComponent<CanvasGroup>().DOFade(1.0f, 1.0f).SetEase(Ease.Linear));
    }

    /// <summary>
    /// リザルト表示を元の位置に戻して、ゲームを再スタートする
    /// </summary>
    private void OnClickMovePopUp()
    {
        // ボタンを非活性化してタップを感知させなくする
        btnClosePopUp.interactable = false;

        // DOTweenの処理を使って、ResultPopUpゲームオブジェクトをゲーム画面外の位置に戻す
        transform.DOMoveY(posY, 1.0f);

        // ゲームの再スタート処理を呼び出す
        StartCoroutine(GameData.instance.RestartGame());
    }
}
