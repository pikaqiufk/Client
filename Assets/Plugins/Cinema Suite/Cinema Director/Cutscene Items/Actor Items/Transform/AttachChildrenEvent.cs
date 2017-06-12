using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Detaches all children in hierarchy from this Parent.
    /// </summary>
    [CutsceneItemAttribute("Transform", "Attach Children", CutsceneItemGenre.ActorItem)]
    public class AttachChildrenEvent : CinemaActorEvent
    {
        public GameObject[] Children;
        public override void Trigger(GameObject actor)
        {
            if (actor != null && Children != null)
            {
                {
                    var __array1 = Children;
                    var __arrayLength1 = __array1.Length;
                    for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
                    {
                        var child = (GameObject)__array1[__i1];
                        {
                            child.transform.parent = actor.transform;
                        }
                    }
                }
            }
        }

        public override void Reverse(GameObject actor)
        {
        }
    }
}