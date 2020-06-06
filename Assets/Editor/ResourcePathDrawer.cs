using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Plugins.Common.Editor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ResourcePathAttribute))]
    public class SkinnableResourcePathDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 36;
        }

        public override void OnGUI(Rect position, SerializedProperty property, UnityEngine.GUIContent label)
        {
            ResourcePathAttribute attr = attribute as ResourcePathAttribute;

            Rect r1 = new Rect(position.x, position.y, position.width - 100, 18);
            string resPath = EditorGUI.TextField(r1, label, property.stringValue);

            Rect r2 = new Rect(position.x + position.width - 100, position.y, 100, 18);
           
            property.stringValue = resPath;
            string realPath = resPath;
             

            Rect r3 = new Rect(position.x, position.y + 18, position.width, 18);
            var curAsset = LoadResource(realPath,attr.Type);
            var newAsset = EditorGUI.ObjectField(r3, curAsset, attr.Type, false);

            if (newAsset != null && newAsset != curAsset)
            {
                resPath = GetResourcePath(newAsset);
              
                property.stringValue = resPath;
            }
        }

        public UnityEngine.Object LoadResource(string path, Type t)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            path = path.Replace(':', '/');
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            string[] subFix;
            if (t == typeof(GameObject) || typeof(Component).IsAssignableFrom(t))
            {
                subFix = new[] { ".prefab" };
            }
            else if (t == typeof(AudioClip))
            {
                subFix = new[] { ".wav", ".mp3", ".ogg" };
            }
            else if (typeof(Texture).IsAssignableFrom(t))
            {
                subFix = new[] { ".jpg", ".png", ".tga", ".dds" };
            }
            else if (typeof(Shader).IsAssignableFrom(t))
            {
                subFix = new[] { ".shader", };
            }
            else if (typeof(Material).IsAssignableFrom(t))
            {
                subFix = new[] { ".mat", };
            }
            else if (typeof(Mesh).IsAssignableFrom(t))
            {
                subFix = new[] { ".fbx" };
            }
            else if (typeof(TextAsset).IsAssignableFrom(t))
            {
                subFix = new[] { ".txt", ".json", ".bytes" };
            }
            else
            {
                subFix = new[] { ".asset" };
            }
            for (int i = 0; i < subFix.Length; i++)
            {
                var obj = AssetDatabase.LoadMainAssetAtPath("Assets/ArtResources/" + path + subFix[i]);
                if (obj != null)
                {
                    return obj;
                }
            }
            return Resources.Load(path);
        }


        public   string GetResourcePath(  UnityEngine.Object obj)
        {
            var assetPath = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(assetPath))
            {
                var prefabObj = PrefabUtility.GetCorrespondingObjectFromSource(obj);
                assetPath = AssetDatabase.GetAssetPath(prefabObj);
                if (string.IsNullOrEmpty(assetPath))
                {
                    return null;
                }
            }
            if (assetPath.StartsWith("Assets/ArtResources/", StringComparison.CurrentCultureIgnoreCase))
            {
                // bundle 资源
                var bunldeName = AssetDatabase.GetImplicitAssetBundleName(assetPath);
                if (string.IsNullOrEmpty(bunldeName))
                {
                    return string.Empty;
                }
                var assetName = assetPath.Substring("Assets/".Length + bunldeName.Length + 1);

                var extIdx = assetName.LastIndexOf('.');
                if (extIdx != -1)
                {
                    assetName = assetName.Substring(0, extIdx);
                }
                else
                {
                    assetName = string.Empty;
                }
                if (assetName.Length == 0)
                {
                    return bunldeName.Substring("ArtResources/".Length);
                }
                else
                {
                    return bunldeName.Substring("ArtResources/".Length) + ":" + assetName;
                }

            }
            else
            {
                // resources
                var idx = assetPath.IndexOf("/Resources/");
                if (idx != -1)
                {
                    var startIdx = idx + "/Resources/".Length;
                    var extIdx = assetPath.LastIndexOf('.');
                    var resPath = assetPath.Substring(startIdx, extIdx - startIdx);
                    return resPath;
                }
            }
            return string.Empty;
        }

    }



}
