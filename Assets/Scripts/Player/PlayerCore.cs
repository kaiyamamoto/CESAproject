﻿// ---------------------------------------
// Brief : プレイヤーのコア部分
//
// Date  : 2017/04/27
//
// Author: Y.Watanabe
// ---------------------------------------

using UniRx;
using UnityEngine;
using System.Collections;

public class PlayerCore : MonoBehaviour
{
	[SerializeField, Range(0, 10), Tooltip("登場にかかる時間")]
	private float m_waitTime;

	/// <summary>
	/// プレイヤが操作可能な状態か
	/// </summary>
	public ReactiveProperty<bool> PlayerControllable = new ReactiveProperty<bool>(false);

	/// <summary>
	/// 死んだか
	/// </summary>
	public ReactiveProperty<bool> IsDead = new ReactiveProperty<bool>(false);

    /// <summary>
    /// 復活
    /// </summary>
    public void Resurrection() { IsDead = new ReactiveProperty<bool>(false); }

    // 無敵
    private bool m_invincible = false;

    public bool Invincible
    {
        get { return m_invincible; }
        set { m_invincible = value; }
    }

    /// <summary>
    /// 更新前処理
    /// </summary>
    void Start ()
	{
		StartCoroutine(WaitStartAnimation());

		IsDead
			.DistinctUntilChanged()
			.Where(x => x == true)
            .Where(_ => m_invincible != true)
            .Subscribe(_ =>
			{
				Debug.Log("死ゾ");
				GetComponent<BreakBlock>().Break();
			});
	}


	/// <summary>
	/// 登場演出で少し待機する
	/// </summary>
	/// <returns></returns>
	IEnumerator WaitStartAnimation ()
	{
		var enter = GetComponent<PlayerEnter>();

		// プレイヤーが登場し終えるまで待機
		while (enter.IsPlayerEnter == false)
		{
			yield return new WaitForEndOfFrame();
		}

		PlayerControllable.Value = true;
	}
}
