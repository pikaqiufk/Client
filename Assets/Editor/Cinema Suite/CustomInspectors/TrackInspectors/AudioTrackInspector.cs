using UnityEditor;
using UnityEngine;
using CinemaDirector;

[CustomEditor(typeof(AudioTrack))]
public class AudioTrackInspector : Editor
{
    // Properties
    private SerializedObject audioTrack;

    private GUIContent addAudio = new GUIContent("Add Audio Item", "Add a new audio clip to this track.");
    /// <summary>
    /// On inspector enable, load serialized objects
    /// </summary>
    public void OnEnable()
    {
        audioTrack = new SerializedObject(this.target);
    }

    /// <summary>
    /// Update and Draw the inspector
    /// </summary>
    public override void OnInspectorGUI()
    {
        audioTrack.Update();
        {
            var __array1 = (target as AudioTrack).AudioClips;
            var __arrayLength1 = __array1.Length;
            for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
            {
                var audio = (CinemaAudio)__array1[__i1];
                {
                    EditorGUILayout.ObjectField(audio.name, audio, typeof(CinemaAudio), true);
                }
            }
        }
        if (GUILayout.Button(addAudio))
        {
            string label = DirectorHelper.getCutsceneItemName("Audio Item", typeof(CinemaAudio));
            GameObject audioItem = new GameObject(label, new System.Type[] { typeof(CinemaAudio), typeof(AudioSource) });
            audioItem.transform.parent = (this.target as AudioTrack).transform;
        }

        audioTrack.ApplyModifiedProperties();
    }
}
