using UnityEngine;

public class Shuffle : MonoBehaviour
{
    [SerializeField, Header("シャッフルの力")]
    private float shufflePower = 10.0f;

    [SerializeField, Header("シャッフルの速度")]
    private Vector2 shuffleVelosity = new Vector2(10.0f, 10.0f);

    [SerializeField, Header("シャッフルの時間")]
    private float duration = 1.0f;

    private CapsuleCollider2D capsuleCol;      // Colliderのオン/オフ制御用
    private float shuffleTimer;                // シャッフル時間のカウント用

    private UIManager uiManager;

    void Start()
    {
      //  uiManager = new UIManager();  // または他の方法でインスタンスを取得
      //  StartCoroutine(uiManager.Initialize());
    }

    /// <summary>
    /// シャッフルの初期設定
    /// </summary>
    /// <param name="uiManager"></param>
    public void SetUpShuffle(UIManager uiManager)
    {
        this.uiManager = uiManager;

        capsuleCol = GetComponent<CapsuleCollider2D>();

        // シャッフル用のコライダーをオフにしておく
        capsuleCol.enabled = false;
    }

    /// <summary>
    /// シャッフル開始
    /// </summary>
    public void StartShuffle()
    {
        // コライダーをオンにして干支をシャッフルできるようにする
        capsuleCol.enabled = true;

        // シャッフル時間を設定
        shuffleTimer = duration;

        // シャッフルの方向をランダムに取得
        int value = Random.Range(0, 2);

        // シャッフルの方向をシャッフル速度のXに設定(-1 = 左方向、1 = 右方向)
        shuffleVelosity.x = value == 0 ? shuffleVelosity.x *= -1 : shuffleVelosity.x *= 1;
    }

    /// <summary>
    /// シャッフル停止
    /// </summary>
    private void StopShuffle()
    {
        shuffleTimer = 0;

        // コライダーをオフにして干支への干渉をなくす
        capsuleCol.enabled = false;

        // 再度シャッフルボタンを押せるようにする
        uiManager.ActivateShuffleButton(true);  //コメントアウト
    }

    /// <summary>
    /// シャッフルの実処理
    /// </summary>
    /// <param name="col"></param>
    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.TryGetComponent(out Rigidbody2D rb))
        {
            // コライダー内にある干支に対して物理演算を行う

            // 干支に対するシャッフルの相対速度を計算
            Vector2 relativeVelocity = shuffleVelosity - rb.velocity;

            // 干支に力を加える(シャッフル)
            rb.AddForce(shufflePower * relativeVelocity);
        }
    }

    private void Update()
    {
        // シャッフル中のみ、shuffleTimerをカウント
        shuffleTimer -= Time.deltaTime;

        // shuffleTimerが0になり、かつコライダーがオンの場合には
        if (shuffleTimer <= 0 && capsuleCol.enabled)
        {
            // シャッフル停止
            StopShuffle();
        }
    }
}
