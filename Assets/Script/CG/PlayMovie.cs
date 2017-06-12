#region using

using System;
using CinemaDirector;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

public class PlayMovie
{
    public static void Play(string prafab, Action endCallbak = null, bool skippable = true, bool showBoder = true)
    {
        var res = ResourceManager.PrepareResourceSync<GameObject>(prafab);

        var sceneObj = Object.Instantiate(res) as GameObject;

        var scene = sceneObj.GetComponent<Cutscene>();
        scene.IsSkippable = skippable;

        var logic = sceneObj.GetComponent<CutSceneLogic>();
        logic.Skippable = skippable;
        logic.ShowBoder = showBoder;

        InputManager.Instance.enabled = false;
        GameLogic.Instance.MainCamera.active = false;
        UIManager.Instance.UIRoot.active = false;

        scene.CutsceneFinished += ((sender, evn) =>
        {
            Object.Destroy(sceneObj);
            GameLogic.Instance.MainCamera.active = true;
            UIManager.Instance.UIRoot.active = true;
            InputManager.Instance.enabled = true;
            if (null != endCallbak)
            {
                endCallbak();
            }
        });
//         ResourceManager.PrepareResource<GameObject>(prafab, (res) =>
//         {
//             ResourceManager.ChangeShader(res.transform);
// 
//             var sceneObj = GameObject.Instantiate(res) as GameObject;
// 
//             var scene = sceneObj.GetComponent<CinemaDirector.Cutscene>();
//             scene.IsSkippable = skippable;
// 
//             var logic = sceneObj.GetComponent<CutSceneLogic>();
//             logic.Skippable = skippable;
//             logic.ShowBoder = showBoder;
// 
//             InputManager.Instance.enabled = false;
//             GameLogic.Instance.MainCamera.active = false;
//             UIManager.Instance.UIRoot.active = false;
// 
//             scene.CutsceneFinished += ((sender, evn) =>
//             {
//                 GameObject.Destroy(sceneObj);
//                 GameLogic.Instance.MainCamera.active = true;
//                 UIManager.Instance.UIRoot.active = true;
//                 InputManager.Instance.enabled = true;
//                 if (null != endCallbak)
//                 {
//                     endCallbak();
//                 }
//             });
// 
//         });
    }
}