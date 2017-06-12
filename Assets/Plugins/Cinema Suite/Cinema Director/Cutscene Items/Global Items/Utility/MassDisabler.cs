// Cinema Suite
using CinemaDirector.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Utility action for disabling a lot of GameObjects at once and then re-enabling them at the end of the action.
    /// </summary>
    [CutsceneItem("Utility", "Mass Disabler", CutsceneItemGenre.GlobalItem)]
    public class MassDisabler : CinemaGlobalAction, IRevertable
    {
        // Game Objects to be disabled temporarily.
        public List<GameObject> GameObjects = new List<GameObject>();

        // Game Object Tags to be disabled temporarily.
        public List<string> Tags = new List<string>();

        // Cache the game objects.
        private List<GameObject> tagsCache = new List<GameObject>();

        // Options for reverting in editor.
        [SerializeField]
        private RevertMode editorRevertMode = RevertMode.Revert;

        // Options for reverting during runtime.
        [SerializeField]
        private RevertMode runtimeRevertMode = RevertMode.Revert;

        /// <summary>
        /// Cache the initial state of the target GameObject's active state.
        /// </summary>
        /// <returns>The Info necessary to revert this event.</returns>
        public RevertInfo[] CacheState()
        {
            List<GameObject> gameObjects = new List<GameObject>();
            {
                var __list1 = Tags;
                var __listCount1 = __list1.Count;
                for (int __i1 = 0; __i1 < __listCount1; ++__i1)
                {
                    var tag = (string)__list1[__i1];
                    {
                        GameObject[] tagged = GameObject.FindGameObjectsWithTag(tag);
                        {
                            var __array6 = tagged;
                            var __arrayLength6 = __array6.Length;
                            for (int __i6 = 0; __i6 < __arrayLength6; ++__i6)
                            {
                                var gameObject = (GameObject)__array6[__i6];
                                {
                                    gameObjects.Add(gameObject);
                                }
                            }
                        }
                    }
                }
            }
            gameObjects.AddRange(GameObjects);

            List<RevertInfo> reverts = new List<RevertInfo>();
            {
                var __list2 = gameObjects;
                var __listCount2 = __list2.Count;
                for (int __i2 = 0; __i2 < __listCount2; ++__i2)
                {
                    var go = (GameObject)__list2[__i2];
                    {
                        if (go != null)
                        {
                            reverts.Add(new RevertInfo(this, go, "SetActive", go.activeInHierarchy));
                        }
                    }
                }
            }
            return reverts.ToArray();
        }

        /// <summary>
        /// Trigger this Action and disable all Game Objects
        /// </summary>
        public override void Trigger()
        {
            tagsCache.Clear();
            {
                var __list3 = Tags;
                var __listCount3 = __list3.Count;
                for (int __i3 = 0; __i3 < __listCount3; ++__i3)
                {
                    var tag = (string)__list3[__i3];
                    {
                        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag);
                        {
                            var __array7 = gameObjects;
                            var __arrayLength7 = __array7.Length;
                            for (int __i7 = 0; __i7 < __arrayLength7; ++__i7)
                            {
                                var gameObject = (GameObject)__array7[__i7];
                                {
                                    tagsCache.Add(gameObject);
                                }
                            }
                        }
                    }
                }
            }
            setActive(false);
        }

        /// <summary>
        /// End the action and re-enable all game objects.
        /// </summary>
        public override void End()
        {
            setActive(true);
        }

        /// <summary>
        /// Trigger the beginning of the action while playing in reverse.
        /// </summary>
        public override void ReverseTrigger()
        {
            End();
        }

        /// <summary>
        /// Trigger the end of the action while playing in reverse.
        /// </summary>
        public override void ReverseEnd()
        {
            Trigger();
        }

        /// <summary>
        /// Enable/Disable all the game objects.
        /// </summary>
        /// <param name="enabled">Enable or Disable</param>
        private void setActive(bool enabled)
        {
            {
                var __list4 = GameObjects;
                var __listCount4 = __list4.Count;
                for (int __i4 = 0; __i4 < __listCount4; ++__i4)
                {
                    var gameObject = (GameObject)__list4[__i4];
                    {
                        gameObject.SetActive(enabled);
                    }
                }
            }
            {
                var __list5 = tagsCache;
                var __listCount5 = __list5.Count;
                for (int __i5 = 0; __i5 < __listCount5; ++__i5)
                {
                    var gameObject = (GameObject)__list5[__i5];
                    {
                        gameObject.SetActive(enabled);
                    }
                }
            }
        }

        /// <summary>
        /// Option for choosing when this Event will Revert to initial state in Editor.
        /// </summary>
        public RevertMode EditorRevertMode
        {
            get { return editorRevertMode; }
            set { editorRevertMode = value; }
        }

        /// <summary>
        /// Option for choosing when this Event will Revert to initial state in Runtime.
        /// </summary>
        public RevertMode RuntimeRevertMode
        {
            get { return runtimeRevertMode; }
            set { runtimeRevertMode = value; }
        }
    }
}