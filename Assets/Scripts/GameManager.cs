using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField, Header("干支のプレファブ")]
    private Eto etoPrefab; // <= ⭐︎ 宣言する型を GameObject型 から Eto型に変更。EtoPrefabは前と同じようにProject内からアサインできる

    [SerializeField, Header("干支の生成位置")]
    private Transform etoSetTran;

    [SerializeField, Header("干支生成時の最大回転角度")]
    private float maxRotateAngle = 35.0f;

    [SerializeField, Header("干支生成時の左右のランダム幅")]
    private float maxRange = 400.0f;

    [SerializeField, Header("干支生成時の落下位置")]
    private float fallPos = 1400.0f;

    [SerializeField, Header("生成された干支のリスト")]
    private List<Eto> etoList = new List<Eto>();

    [SerializeField, Header("干支の画像データ")]
    private Sprite[] etoSprites;

    // 最初にドラッグした干支の情報
    private Eto firstSelectEto;

    // 最後にドラッグした干支の情報
    private Eto lastSelectEto;

    // 最初にドラッグした干支の種類
    private EtoType? currentEtoType;

    [SerializeField, Header("削除対象となる干支を登録するリスト")]
    private List<Eto> eraseEtoList = new List<Eto>();

    [SerializeField, Header("つながっている干支の数")]
    private int linkCount = 0;

    [Header("スワイプでつながる干支の範囲")]
    public float etoDistance = 1.0f;

    [SerializeField]
    private UIManager uiManager;


    IEnumerator Start()　　// <= ⭐︎ 戻り値を void から IEnumerator型に変更して、コルーチンメソッドにする
    {
        // 干支の画像を読み込む。この処理が終了するまで、次の処理には行かないようにする
        yield return StartCoroutine(LoadEtoSprites());

        // 引数で指定した数の干支を生成する
        StartCoroutine(CreateEtos(GameData.instance.createEtoCount));
    }

    /// <summary>
    /// 干支の画像を読み込んで配列から使用できるようにする
    /// </summary>
    private IEnumerator LoadEtoSprites()
    {
        // 配列の初期化(12個の画像が入るようにSprite型の配列を12個用意する)
        etoSprites = new Sprite[(int)EtoType.Count];

        // Resources.LoadAllを行い、分割されている干支の画像を順番に全て読み込んで配列に代入
        //etoSprites = Resources.LoadAll<Sprite>("Sprites/eto");

        // ※ 1つのファイルを１２分割していない場合は、以下の処理を行います。12分割している場合には使用しません。
        for(int i = 0; i < etoSprites.Length; i++)
        {
            etoSprites[i] = Resources.Load<Sprite>("Sprites/eto_" + i);
        }

        yield break;
    }

    /// <summary>
    /// 干支を生成
    /// </summary>
    /// <param name="count">生成する数</param>
    /// <returns></returns>
    private IEnumerator CreateEtos(int generateCount)
    {
        for (int i = 0; i < generateCount; i++)
        {
            // 干支プレファブのクローンを、干支の生成位置に生成
            Eto eto = Instantiate(etoPrefab, etoSetTran, false);  // <= ⭐︎ 変更。　生成された干支を代入する型をGameObject型からEto型に変更する

            // 生成された干支の回転情報を設定(色々な角度になるように)
            eto.transform.rotation = Quaternion.AngleAxis(Random.Range(-maxRotateAngle, maxRotateAngle), Vector3.forward);

            // 生成位置をランダムにして落下位置を変化させる
            eto.transform.localPosition = new Vector2(Random.Range(-maxRange, maxRange), fallPos);

            // ランダムな干支を12種類の中から１つ選択
            int randomValue = Random.Range(0, (int)EtoType.Count);

            // 生成された干支の初期設定(干支の種類と干支の画像を引数を使ってEtoへ渡す)
            eto.SetUpEto((EtoType)randomValue, etoSprites[randomValue]);
           
            // etoListに追加
            etoList.Add(eto);

            // 0.03秒待って次の干支を生成
            yield return new WaitForSeconds(0.03f);
        }
    }

    private void Update()
    {
        // 干支をつなげる処理
        if (Input.GetMouseButtonDown(0) && firstSelectEto == null)
        {
            // 干支を最初にドラッグした際の処理
            OnStartDrag();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // 干支のドラッグをやめた（指を離した）際の処理
            OnEndDrag();
        }
        else if (firstSelectEto != null)
        {
            // 干支のドラッグ（スワイプ）中の処理
            OnDragging();
        }
    }

    /// <summary>
    /// 干支を最初にドラッグした際の処理
    /// </summary>
    private void OnStartDrag()
    {
        // 画面をタップした際の位置情報を、CameraクラスのScreenToWorldPointメソッドを利用してCanvas上の座標に変換
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        // 干支がつながっている数を初期化
        linkCount = 0;

        // 変換した座標のコライダーを持つゲームオブジェクトがあるか確認
        if (hit.collider != null)
        {
            // ゲームオブジェクトがあった場合、そのゲームオブジェクトがEtoクラスを持っているかどうか確認
            if (hit.collider.gameObject.TryGetComponent(out Eto dragEto))
            {
                // Etoクラスを持っていた場合には、以下の処理を行う

                // 最初にドラッグした干支の情報を変数に代入
                firstSelectEto = dragEto;

                // 最後にドラッグした干支の情報を変数に代入(最初のドラッグなので、最後のドラッグも同じ干支)
                lastSelectEto = dragEto;

                // 最初にドラッグしている干支の種類を代入 = 後ほど、この情報を使ってつながる干支かどうかを判別する
                currentEtoType = dragEto.etoType;

                // 干支の状態が「選択中」であると更新
                dragEto.num = linkCount;

                // 削除する対象の干支を登録するリストを初期化
                eraseEtoList = new List<Eto>();

                // ドラッグ中の干支を削除の対象としてリストに登録
                AddEraseEtoList(dragEto);
            }
        }
    }

    /// <summary>
    /// 干支のドラッグ（スワイプ）中処理
    /// </summary>
    private void OnDragging()
    {
        // OnStartDragメソッドと同じ処理で、指の位置をワールド座標に変換しRayを発射し、その位置にあるコライダーを持つオブジェクトを取得してhit変数へ代入
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        // Rayの戻り値があり(hit変数がnullではない)、hit変数のゲームオブジェクトがEtoクラスを持っていたら
        if (hit.collider != null && hit.collider.gameObject.TryGetComponent(out Eto dragEto))
        {
            // 現在選択中の干支の種類がnullなら処理は行わない
            if (currentEtoType == null)
            {
                return;
            }

            // dragEto変数の干支の種類が最初に選択した干支の種類と同じであり、最後にタップしている干支と現在の干支が違うオブジェクトであり、かつ、現在の干支がすでに「選択中」でなければ
            if (dragEto.etoType == currentEtoType && lastSelectEto != dragEto && !dragEto.isSelected)
            {
                // 現在タップしている干支の位置情報と最後にタップした干支の位置情報と比べて、差分の値（干支通しの距離）を取る
                float distance = Vector2.Distance(dragEto.transform.position, lastSelectEto.transform.position);

                // 干支同士の距離が設定値よりも小さければ(２つの干支が離れていなければ)、干支をつなげる
                if (distance < etoDistance)
                {
                    // 現在の干支を選択中にする
                    dragEto.isSelected = true;

                    // 最後に選択している干支を現在の干支に更新
                    lastSelectEto = dragEto;

                    // 干支のつながった数のカウントを１つ増やす
                    linkCount++;

                    // 干支に通し番号を設定
                    dragEto.num = linkCount;

                    // 削除リストに現在の干支を追加
                    AddEraseEtoList(dragEto);
                }
            }

            // 現在の干支の種類を確認(現在の干支(dragEtoの情報であれば、他の情報でもよい。ちゃんと選択されているかの確認用))
            Debug.Log(dragEto.etoType);

            // 削除リストに２つ以上の干支が追加されている場合
            if (eraseEtoList.Count > 1)
            {
                // 現在の干支の通し番号を確認
                Debug.Log(dragEto.num);

                // 条件に合致する場合、削除リストから干支を除外する(ドラッグしたまま１つ前の干支に戻る場合、現在の干支を削除リストから除外する)
                if (eraseEtoList[linkCount - 1] != lastSelectEto && eraseEtoList[linkCount - 1].num == dragEto.num && dragEto.isSelected)
                {
                    // 選択中のボールを取り除く
                    RemoveEraseEtoList(lastSelectEto);

                    lastSelectEto.GetComponent<Eto>().isSelected = false;

                    // 最後のボールの情報を、前のボールに戻す
                    lastSelectEto = dragEto;

                    // 繋がっている干支の数を減らす
                    linkCount--;
                }
            }
        }
    }

    /// <summary>
    /// 干支のドラッグをやめた（指を画面から離した）際の処理
    /// </summary>
    private void OnEndDrag()
    {
        // つながっている干支が３つ以上あったら削除する処理にうつる
        if (eraseEtoList.Count >= 3)
        {
            // 選択されている干支を消す
            for (int i = 0; i < eraseEtoList.Count; i++)
            {
                // 干支リストから取り除く
                etoList.Remove(eraseEtoList[i]);

                // 干支を削除
                Destroy(eraseEtoList[i].gameObject);
            }

            // スコアと消した干支の数の加算
            AddScores(currentEtoType, eraseEtoList.Count);

            // 消した干支の数だけ新しい干支をランダムに生成
            StartCoroutine(CreateEtos(eraseEtoList.Count));

            // 削除リストをクリアする
            eraseEtoList.Clear();
        }
        else
        {
            // つながっている干支が２つ以下なら削除はしない

            // 削除リストから、削除候補であった干支を取り除く
            for (int i = 0; i < eraseEtoList.Count; i++)
            {
                // 各干支の選択中の状態を解除する
                eraseEtoList[i].isSelected = false;

                // 干支の色の透明度を元の透明度に戻す
                ChangeEtoAlpha(eraseEtoList[i], 1.0f);
            }
        }

        // 次回の干支を消す処理のために、各変数の値をnullにする
        firstSelectEto = null;
        lastSelectEto = null;
        currentEtoType = null;
    }

    /// <summary>
    /// 選択された干支を削除リストに追加
    /// </summary>
    /// <param name="dragEto"></param>
    private void AddEraseEtoList(Eto dragEto)
    {
        // 削除リストにドラッグ中の干支を追加
        eraseEtoList.Add(dragEto);

        // ドラッグ中の干支のアルファ値を0.5fにする(半透明にすることで、選択中であることをユーザーに伝える)
        ChangeEtoAlpha(dragEto, 0.5f);
    }

    /// <summary>
    /// 前の干支に戻った際に削除リストから削除
    /// </summary>
    /// <param name="dragEto"></param>
    private void RemoveEraseEtoList(Eto dragEto)
    {
        // 削除リストから削除
        eraseEtoList.Remove(dragEto);

        // 干支の透明度を元の値(1.0f)に戻す
        ChangeEtoAlpha(dragEto, 1.0f);

        // 干支の「選択中」の情報がtrneの場合
        if (dragEto.isSelected)
        {
            // falseにして選択中ではない状態に戻す
            dragEto.isSelected = false;
        }
    }

    /// <summary>
    /// 干支のアルファ値を変更
    /// </summary>
    /// <param name="dragEto"></param>
    /// <param name="alphaValue"></param>
    private void ChangeEtoAlpha(Eto dragEto, float alphaValue)
    {
        // 現在ドラッグしている干支のアルファ値を変更
        dragEto.imgEto.color = new Color(dragEto.imgEto.color.r, dragEto.imgEto.color.g, dragEto.imgEto.color.b, alphaValue);
    }

    /// <summary>
    /// スコアと消した干支の数を加算
    /// </summary>
    /// <param name="etoType">消した干支の種類</param>
    /// <param name="count">消した干支の数</param>
    private void AddScores(EtoType? etoType, int count)
    {
        // スコアを加算(EtoPoint * 消した数)
        GameData.instance.score += GameData.instance.etoPoint * count;

        // 消した干支の数を加算
        GameData.instance.eraseEtoCount += count;

        // スコア加算と画面の更新処理
        uiManager.UpdateDisplayScore();
    }
}
