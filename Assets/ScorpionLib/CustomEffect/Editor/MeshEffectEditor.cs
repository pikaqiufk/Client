using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshEffect))]
[CanEditMultipleObjects]
public class MeshEffectEditor : Editor
{
    private MeshEffect effect;
    void OnEnable()
    {
    }
    void OnSelectAtlas(Object obj)
    {
        serializedObject.Update();
        SerializedProperty sp = serializedObject.FindProperty("mAtlas");
        sp.objectReferenceValue = obj;
        serializedObject.ApplyModifiedProperties();
        NGUITools.SetDirty(serializedObject.targetObject);
        NGUISettings.atlas = obj as UIAtlas;

        if (effect != null)
        {
            effect.gameObject.renderer.material = new Material(NGUISettings.atlas.spriteMaterial);
            effect.gameObject.renderer.sharedMaterial.shader = Shader.Find("Particles/Additive");
        }
    }

    void SelectSprite(string spriteName)
    {
        serializedObject.Update();
        SerializedProperty sp = serializedObject.FindProperty("mSpriteName");
        sp.stringValue = spriteName;
        serializedObject.ApplyModifiedProperties();
        NGUITools.SetDirty(serializedObject.targetObject);
        NGUISettings.selectedSprite = spriteName;

        if (effect != null)
        {
            var atals = NGUISettings.atlas;
            var sprite = atals.GetSprite(spriteName);

            var w = (float)atals.texture.width;
            var h = (float)atals.texture.height;

            var uvOffset = new Vector2(sprite.x / w, (h - sprite.height - sprite.y) / h);
            var uvScale = new Vector2(sprite.width / w, sprite.height / h);

            effect.gameObject.renderer.sharedMaterial.mainTextureOffset = uvOffset;
            effect.gameObject.renderer.sharedMaterial.mainTextureScale = uvScale;
        }
    }

    public override void OnInspectorGUI()
    {
        effect = target as MeshEffect;

        EditorGUI.BeginChangeCheck();

        DrawDefaultInspector();

        GUILayout.BeginHorizontal();
        if (NGUIEditorTools.DrawPrefixButton("Atlas"))
            ComponentSelector.Show<UIAtlas>(OnSelectAtlas);
        SerializedProperty atlas = NGUIEditorTools.DrawProperty("", serializedObject, "mAtlas", GUILayout.MinWidth(20f));

        if (GUILayout.Button("Edit", GUILayout.Width(40f)))
        {
            if (atlas != null)
            {
                UIAtlas atl = atlas.objectReferenceValue as UIAtlas;
                NGUISettings.atlas = atl;
                NGUIEditorTools.Select(atl.gameObject);
            }
        }
        GUILayout.EndHorizontal();

        SerializedProperty sp = serializedObject.FindProperty("mSpriteName");
        NGUIEditorTools.DrawAdvancedSpriteField(atlas.objectReferenceValue as UIAtlas, sp.stringValue, SelectSprite, false);

    }

    void OnDisable()
    {
    }
}