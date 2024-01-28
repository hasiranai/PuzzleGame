using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Bomb : MonoBehaviour
{
    private Button btnBomb;

    private float radius;

    [SerializeField]
    private bool onGizmos = true;

    public void SetUpBomb(GameManager gameManager, float bombRadius)
    {
        if (TryGetComponent(out btnBomb))
        {
            btnBomb.onClick.AddListener(() => OnClickBomb(gameManager, bombRadius));
        }
        else
        {
            Debug.Log("ボムのボタンが取得できません");
        }

        // OnDrawGizmos 用
        radius = bombRadius;
    }

    /// <summary>
    /// ボムをタップした際の処理
    /// </summary>
    public void OnClickBomb(GameManager gameManager, float bombRadius)
    {
        // TODO SE

        List<Eto> eraseEtos = new List<Eto>();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, bombRadius);

        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent(out Eto eto))
            {
                eraseEtos.Add(eto);
            }
        }

        // TODO LINQ を利用した場合



        // TODO GameManager への干支の削除命令を書く


        Debug.Log("ボム実行");

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (!onGizmos)
        {
            return;
        }

        // コライダーの可視化
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    void Start()
    {
        // デバッグ用のテスト処理
        SetUpBomb(null, 1.0f);
    }
}
