﻿// ---------------------------------------
// Brief : プレイヤーの当たり判定
// 
// Date  : 2014/04/24
// 
// Author: Y.Watanabe
// ---------------------------------------

using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Linq;

public class PlayerCollision : MonoBehaviour
{
	private Collider[] m_colliders;

	// 衝突方向
	private bool[] m_hitDirections = new bool[4] { false, false, false, false };

	// 衝突距離
	[SerializeField, Range(0, 10)]
	private float m_hitDistance;

	private ReactiveProperty<bool> isHitRP = new ReactiveProperty<bool>();

	// 衝突しているか
	public IReactiveProperty<bool> IsHit
	{
		get { return isHitRP; }
	}

	// 衝突相手のオブジェクト
	private Transform m_hitObject;

	/// <summary> 
	/// 更新前処理
	/// </summary>
	void Start ()
	{
		var rb = GetComponentInParent<Rigidbody>();
		var pm = GetComponent<PlayerMover>();

		RaycastHit hit = new RaycastHit();

		// 上下左右のColliderを参照
		m_colliders = transform.GetChild(0).GetComponentsInChildren<Collider>();

		// RigitBodyのスリープ解除
		this.UpdateAsObservable()
			.Subscribe(_ => rb.WakeUp());

		// はさまれた判定を取る
		foreach (var item in m_colliders.Select((v, i) => new { Value = v, Index = i }))
		{
			item.Value.OnTriggerEnterAsObservable()
				.Where(col => col != transform.root.GetComponent<Collider>())
				.Subscribe(_ =>
				{
					m_hitDirections[item.Index] = true;
					Debug.Log(_.transform.name);
				});

			item.Value.OnTriggerExitAsObservable()
				.Where(col => col != transform.root.GetComponent<Collider>())
				.Subscribe(_ =>
				{
					m_hitDirections[item.Index] = false;
					Debug.Log(_.transform.name);
				});
		}

		// 左右のはさまれた判定
		this.UpdateAsObservable()
			.Where(_ => m_hitDirections[0] == true)
			.Where(_ => m_hitDirections[1] == true)
			.Subscribe(_ =>
			{
				Destroy(transform.root.gameObject);
			});

		// 上下のはさまれた判定
		this.UpdateAsObservable()
			.Where(_ => m_hitDirections[2] == true)
			.Where(_ => m_hitDirections[3] == true)
			.Subscribe(_ => Destroy(transform.root.gameObject));

		// 衝突情報と通知を飛ばす
		this.UpdateAsObservable()
			.Select(_ => Physics.Raycast(transform.position, transform.forward, out hit, m_hitDistance))
			.Subscribe(x => {
				isHitRP.SetValueAndForceNotify(x);
				m_hitObject = hit.transform;
			});

		// プレイヤーとオブジェクトが衝突した時
		IsHit
			.Where(x => x == true)
			// 衝突相手が存在するか
			.Where(_ => m_hitObject != null)
			.Subscribe(_ =>
			{
				transform.parent = m_hitObject;
			});

		// プレイヤーとオブジェクトが離れた時
		pm.IsMoving
			.Where(x => x == false)
			// 何かのオブジェクトの子になっているか
			//.Where(_ => transform.root.GetInstanceID() != transform.GetInstanceID())
			.Subscribe(_ =>
			{
				transform.parent = null;
				m_hitObject = null;
			});
	}
}
