﻿//************************************************/
//* @file  :ClearDirection.cs
//* @brief :クリア画面の演出
//* @date  :2017/05/24
//* @author:S.Katou
//************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace ShunLib
{
    public class ClearDirection : MonoBehaviour
    {
        // シングルトン
        static public ClearDirection instance;

        // 終点
        [SerializeField, Tooltip("移動する距離")]
        private float m_targetPositionX = 0.0f;

        //動かすオブジェクト
        [SerializeField, Tooltip("移動させるオブジェクト")]
        private GameObject[] m_obj = new GameObject[5];

        // オブジェクトの初期座標
        private float[] m_objPos = new float[5];

        //待ち時間
        [SerializeField, Tooltip("移動が始まるまでの時間")]
        private float[] m_intervalTime = new float[5];

        //画面を暗くする
        [SerializeField, Tooltip("画面を暗くする")]
        private Image m_black;

        //開始した時間
        private float m_startTime = 0.0f;

        //クリア判定とスタート判定
        private bool m_isGameClear = false;
        private bool m_isStarted = false;

        /// <summary>
        /// 初期化
        /// </summary>
        private void Awake()
        {
            // シングルトン
            if (instance == null) instance = this;
            else Destroy(gameObject);

            // 配列確保
            m_objPos = new float[m_obj.Length];
            // 座標記憶
            for (int i = 0; i < m_obj.Length; i++)
            {
                if (m_obj[i] != null) m_objPos[i] = m_obj[i].transform.position.x;
            }

            // 初期設定
            Initialize();
        }

        /// <summary>
        /// 初期設定
        /// </summary>
        public void Initialize()
        {
            m_startTime = Time.time;

            m_isGameClear = false;   // オーバーしていない
            m_isStarted = false;

            //念のため透明にする
            var color = m_black.color;
            color.a = 0.0f;
            m_black.color = color;

            // オブジェクトの座標設定
            //Debug.Log("座標は " + m_obj[0].transform.position);
            for (int i = 0; i < m_obj.Length; i++)
            {
                if (m_obj[i] != null)
                {
                    if (i >= 3||i==0)
                    {
                        m_obj[i].transform.DOComplete();
                        m_obj[i].transform.DOLocalMoveX(m_objPos[i], 0.1f).SetEase(Ease.Linear).Complete();
                    }
                    m_obj[i].SetActive(false);
                }
            }
            //Debug.Log("座標は " + m_obj[0].transform.position);
        }


        /// <summary>
        /// 更新処理
        /// </summary>
        void Update()
        {
            //クリアするまで更新しない
            if (!IsStarted())
            {
                return;
            }

            //画面を暗くする
            DarkenScreen();
        }


        /// <summary>
        /// クリア演出を始めるかどうか
        /// </summary>
        private bool IsStarted()
        {
            //クリアしているとき
            if (m_isGameClear)
                if (!m_isStarted)
                {
                    m_startTime = Time.time;
                    m_isStarted = true;
                }
            return m_isStarted;
        }

        /// <summary>
        /// タイム表示
        /// </summary>
        private void ShowTime()
        {

        }

        /// <summary>
        /// オブジェクトを移動させる
        /// </summary>
        private void MoveObject()
        {
            //オブジェクトが存在するならば移動させる
            if (m_obj.Length > 0)
            {
                for (int i = 0; i < m_obj.Length; i++)
                {
                    if (m_obj[i] != null)
                    {
                        m_obj[i].SetActive(true);
                        // 20ステの時は次へを出さない
                        if (((int)(YamagenLib.PlayInstructor.instance.GetLoadStage())
                            >= YamagenLib.SelectManager.instance.GetTexture().Length - 1
                            && (i == 3)) == false)
                        {
                            if (i >= 3 || i == 0)
                            m_obj[i].transform.DOLocalMoveX(m_targetPositionX, 0.1f).SetEase(Ease.Linear).SetDelay(m_intervalTime[i]);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 画面を暗くする
        /// </summary>
        private void DarkenScreen()
        {
            float timeStep = (Time.time - m_startTime);

            float alpha = timeStep / m_intervalTime[0];
            var color = m_black.color;

            if (alpha > 0.5f)
            {
                alpha = 0.5f;
            }
            color.a = alpha;
            m_black.color = color;
        }
        /// <summary>
        /// クリアしたら呼ぶ
        /// </summary>
        public void GameClear()
        {
            Debug.Log("Clear");
            m_isGameClear = true;
            //オブジェクトを移動させる
            MoveObject();
        }

        public bool isClear()
        {
            return IsStarted();
        }
    }
}
