﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace YamagenLib {
    public class SelectSceneButton : MonoBehaviour {

        // 初期化
        void Awake()
        {
            GetComponent<Button>().onClick.AddListener(ChangeSelect);
        }

        // シーンをSelectにする
        public void ChangeSelect()
        {
            AudioManager.Instance.Play("SELECTCUBE");
            PlayInstructor.instance.StartCoroutine("StageUnLoad");
            SceneInstructor.instance.LoadMainScene(GameScene.Select);
        }
    }
}
