using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class EtoSelectPopUp : MonoBehaviour
{
    //[SerializeField, Header("干支ボタンのプレファブ")]
    //private EtoButton etoButtonPrefab;    ///発展７で解除するところ１

    [SerializeField, Header("干支ボタンの生成位置")]
    private Transform etoButtonTran;

    public Button btnStart;                             // スタートボタンの制御用

    public CanvasGroup canvasGroup;                     // CanvasGroupの制御用

    //private List<EtoButton> etoButtonList = new List<EtoButton>();   // 生成した干支ボタンを管理するリスト。
    //↑発展７で解除２
    private GameManager gameManager;                    // GameManagerクラスへの紐づけ

    /// <summary>
    /// 干支ボタンの生成
    /// </summary>
    /// <returns></returns>
    public IEnumerator CreateEtoButtons(GameManager gameManager)
    {
        // 干支のボタンが準備できるまでスタートは押せない
        btnStart.interactable = false;

        this.gameManager = gameManager;

        //干支データを元に干支の選択ボタンを生成
        for (int i = 0; i < (int)EtoType.Count; i++)
        {
            // 干支ボタンを生成
          //  EtoButton etoButton = Instantiate(etoButtonPrefab, etoButtonTran, false);

            // 干支ボタンの初期設定
          //  etoButton.SetUpEtoButton(this, GameData.instance.etoDataList[i]);

            if (i == 0)
            {
                // 初期は干支の子(ね)を選択している状態にする
         //       etoButton.imgEto.Color = new Color(0.65f, 0.65f, 0.65f);
                GameData.instance.selectedEtoData = GameData.instance.etoDataList[i];
            }

            // 干支ボタンをListへ追加
         //   etoButtonList.Add(etoButton);

            yield return new WaitForSeconds(0.15f);
        }

        // Startボタンへのメソッドの登録
        btnStart.onClick.AddListener(OnClickStart);

        // 干支のボタンの準備ができたのでスタートを押せるようにする
        btnStart.interactable = true;

        Debug.Log("Init Eto Buttuns");

        yield break;
    }

    /// <summary>
    /// スタートボタンを押した際の処理
    /// </summary>
    private void OnClickStart()
    {
        // スタートボタンを押せないようにして重複タップを防止
        btnStart.interactable = false;

        // ゲームの準備開始
        StartCoroutine(gameManager.PreparateGame());

        // 徐々に干支選択のポップアップを見えないようにしてから非表示にする
        canvasGroup.DOFade(0.0f, 0.5f);
        canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// EtoButtonから呼ばれる
    /// 選択された干支ボタンの色を選択中の色(灰色)に変更
    /// 他のボタンは選択中ではない色(通常の色)に変更
    /// </summary>
    /// <param name="etoType">選択中にする干支の種類</param>
    /// <param name="waitTime"></param>
    public void ChangeColerToEtoButton(EtoType etoType)
    {
//        for (int i = 0; i < etoButtonList.Count; i++)
        {
            // 干支ボタンの色を選択中か、選択中でないかで変更
     //       if (etoButtonList[i].etoData.Etotype == etoType)
     //       {
                // 選択中の色に変更(灰色)
    //            etoButtonList[i].imgEto.color = new Color(0.65f, 0.65f, 0.65f);
      //      }
      //      else
            {
                // 通常の色に変更
     //           etoButtonList[i].imgEto.color = new Color(1f, 1f, 1f);
            }
        }
    }
}
