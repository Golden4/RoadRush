using UnityEngine;
using UnityEditor;

public class qb_Window : EditorWindow
{
    public static qb_Window window;

    #region Textures
    static Texture2D bulletPointTexture;

    protected static void LoadTextures()
    {
        string guiPath = "Assets/ProCore/QuickBrush/Resources/Skin/";
        bulletPointTexture = AssetDatabase.LoadAssetAtPath(guiPath + "qb_bullet.tga", typeof(Texture2D)) as Texture2D;
    }

    #endregion

    void OnEnable()
    {
        window = this;

        LoadTextures();
        //	BuildStyles();
    }

    protected static void MenuListItem(bool bulleted, bool centered, string text)
    {
        EditorGUILayout.BeginHorizontal();

        if (bulleted)
            GUILayout.Label(bulletPointTexture, window.bulletPointStyle);

        EditorGUILayout.LabelField(text, centered ? window.labelStyle_centered : window.labelStyle);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

    }

    protected static void MenuListItem(bool bulleted, string text)
    {
        MenuListItem(bulleted, false, text);
    }

    protected static void MenuListItem(string text)
    {
        MenuListItem(false, false, text);
    }

    [SerializeField]
    protected GUIStyle labelStyle;
    [SerializeField]
    protected GUIStyle labelStyle_bold;
    [SerializeField]
    protected GUIStyle labelStyle_centered;
    [SerializeField]
    protected GUIStyle menuBlockStyle;
    [SerializeField]
    protected GUIStyle bulletPointStyle;

    protected void BuildStyles()
    {
        labelStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).label)
        {
            alignment = TextAnchor.UpperLeft,
            wordWrap = true,
            padding = { left = 0 },
            margin = { left = 0 }
        };

        labelStyle_bold = new GUIStyle(EditorStyles.boldLabel);

        labelStyle_centered = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.UpperCenter };

        bulletPointStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).label)
        {
            margin = { right = 0, left = 0, top = 0, bottom = 0 },
            padding = { right = 0, left = 0, top = 0, bottom = 0 }
        };

        menuBlockStyle = new GUIStyle(EditorStyles.textField) { alignment = TextAnchor.UpperLeft };
    }
}
