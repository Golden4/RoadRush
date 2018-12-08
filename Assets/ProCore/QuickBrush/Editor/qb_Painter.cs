using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class qb_Painter : EditorWindow
{
    [MenuItem("Tools/QuickBrush/Панель инструментов")]
    public static void ShowWindow()
    {
        window = GetWindow<qb_Painter>(false, "QuickBrush");

        //window.position = new Rect(100,100,290,600);
        window.maxSize = new Vector2(320, 800);
        window.minSize = new Vector2(320, 300);
    }

    #region Variable Declarations
    //[SerializeField]
    //	private qb_ObjectContainer		container; 

    static qb_Painter window;

    static BrushMode brushMode;

    static bool brushDirection = true;		//Positive or negative - Indicates whether we are placing or erasing

    static bool placementModifier;

    static SceneView.OnSceneFunc onSceneGUIFunc;


    static Texture2D removePrefabXTexture_normal;
    static Texture2D removePrefabXTexture_hover;
    static Texture2D addPrefabTexture;
    static Texture2D addPrefabFieldTexture;
    static Texture2D selectPrefabCheckTexture_normal;
    static Texture2D selectPrefabCheckTexture_hover;
    static Texture2D prefabFieldBackgroundTexture;
    static Texture2D brushIcon_Active;
    static Texture2D brushIcon_Inactive;
    static Texture2D eraserIcon_Active;
    static Texture2D eraserIcon_Inactive;
    static Texture2D placementIcon_Active;
    #endregion


    void OnEnable()
    {
        window = this;
        onSceneGUIFunc = this.OnSceneGUI;
        SceneView.onSceneGUIDelegate += onSceneGUIFunc;

        const string guiPath = "Assets/ProCore/QuickBrush/Resources/Skin/";

        addPrefabTexture = AssetDatabase.LoadAssetAtPath(guiPath + "qb_addPrefabIcon.tga", typeof(Texture2D)) as Texture2D;
        addPrefabFieldTexture = AssetDatabase.LoadAssetAtPath(guiPath + "qb_addPrefabField.tga", typeof(Texture2D)) as Texture2D;
        removePrefabXTexture_normal = AssetDatabase.LoadAssetAtPath(guiPath + "qb_removePrefabXIcon_normal.tga", typeof(Texture2D)) as Texture2D;
        removePrefabXTexture_hover = AssetDatabase.LoadAssetAtPath(guiPath + "qb_removePrefabXIcon_hover.tga", typeof(Texture2D)) as Texture2D;

        selectPrefabCheckTexture_normal = AssetDatabase.LoadAssetAtPath(guiPath + "qb_selectPrefabCheck_normal.tga", typeof(Texture2D)) as Texture2D;
        selectPrefabCheckTexture_hover = AssetDatabase.LoadAssetAtPath(guiPath + "qb_selectPrefabCheck_hover.tga", typeof(Texture2D)) as Texture2D;
        prefabFieldBackgroundTexture = AssetDatabase.LoadAssetAtPath(guiPath + "qb_prefabFieldBackground.tga", typeof(Texture2D)) as Texture2D;

        brushIcon_Active = AssetDatabase.LoadAssetAtPath(guiPath + "qb_brushIcon_Active.tga", typeof(Texture2D)) as Texture2D;
        brushIcon_Inactive = AssetDatabase.LoadAssetAtPath(guiPath + "qb_brushIcon_Inactive.tga", typeof(Texture2D)) as Texture2D;

        eraserIcon_Active = AssetDatabase.LoadAssetAtPath(guiPath + "qb_eraserIcon_Active.tga", typeof(Texture2D)) as Texture2D;
        eraserIcon_Inactive = AssetDatabase.LoadAssetAtPath(guiPath + "qb_eraserIcon_Inactive.tga", typeof(Texture2D)) as Texture2D;

        placementIcon_Active = AssetDatabase.LoadAssetAtPath(guiPath + "qb_placementIcon_Active.tga", typeof(Texture2D)) as Texture2D;
        /////

        UpdateGroups();
        //BuildStyles();
        EnableMenu();
        ClearForm();

        //Temp Removes any instance of ObjectContainer from the scene.
        //qb_ObjectContainer tryInstance = (qb_ObjectContainer)FindObjectOfType(typeof(qb_ObjectContainer));
        //if(tryInstance != null)
        //	Object.DestroyImmediate(tryInstance.gameObject);
    }

    void OnDisable()
    {
        DisableMenu();
    }

    void ClearForm()
    {
        Repaint();
    }

    void EnableMenu()
    {
        brushDirection = true;
        brushMode = BrushMode.Off;
        brushSettingsFoldout = false;
        rotationFoldout = false;
        scaleFoldout = false;
        groupObjects = false;
        selectedPrefabIndex = -1;

        LoadSettings();
    }

    void DisableMenu()
    {
        brushDirection = true;
        brushMode = BrushMode.Off;
        brushSettingsFoldout = false;
        rotationFoldout = false;
        scaleFoldout = false;

        SaveSettings();
    }

    #region Brush Settings Vars
    [SerializeField]
    private bool brushSettingsFoldout = false;

    [SerializeField]
    private float brushRadius = 0.5f;		//size of the brush in 3d space
    [SerializeField]
    private float brushRadiusMin = 0.2f;
    [SerializeField]
    private float brushRadiusMax = 5f;
    [SerializeField]
    private float brushSpacing = 0.2f;
    [SerializeField]
    private float brushSpacingMax = 2f;
    [SerializeField]
    private float brushSpacingMin = 0.02f;
    [SerializeField]
    private float scatterRadius = 0f;
    #endregion

    #region Rotation Settings Vars
    [SerializeField]
    private bool rotationFoldout = false;

    //Alignment	
    [SerializeField]
    private bool alignToNormal = true;
    [SerializeField]
    private bool alignToStroke = true;

    //Offset
    [SerializeField]
    private Vector3 rotationRange = Vector3.zero;
    #endregion

    #region Scale Settings Vars
    [SerializeField]
    private bool scaleFoldout = false;

    //The minimum and maximum possible scale
    [SerializeField]
    private float scaleMin = 0.1f;
    [SerializeField]
    private float scaleMax = 3f;

    //The minimum and maximum current scale range setting
    [SerializeField]
    private Vector3 scaleRandMin = new Vector3(1f, 1f, 1f);
    [SerializeField]
    private Vector3 scaleRandMax = new Vector3(1f, 1f, 1f);
    [SerializeField]
    private float scaleRandMaxUniform = 1f;
    [SerializeField]
    private float scaleRandMinUniform = 1f;
    [SerializeField]
    private bool scaleUniform = true;
    #endregion

    #region Sorting Vars
    [SerializeField]
    private bool sortingFoldout = false;

    //Selection
    [SerializeField]
    private bool paintToSelection = false;

    //Groups
    static List<qb_Group> groups = new List<qb_Group>();
    static List<string> groupNames = new List<string>();
    [SerializeField]
    private bool groupObjects = false;
    static qb_Group curGroup;
    static string newGroupName = "";
    [SerializeField]
    private int groupDropdownIndex = 0;

    //Layers
    [SerializeField]
    private bool paintToLayer = false;		//Brush only acts on on chosen layer
    [SerializeField]
    private int layerIndex = 0;			//The index of that layer
    #endregion

    #region Live Vars
    //Painting
    static bool paintable = false;			//set by the mouse raycast to control whether an object can be painted on
    static qb_Stroke curStroke;
    static qb_Point cursorPoint = new qb_Point();

    //Prefab Section
    private Vector2 prefabFieldScrollPosition;
    [SerializeField]
    private qb_PrefabObject[] prefabGroup = new qb_PrefabObject[0];
    private Object newPrefab;
    private Object previousPrefab;
    private List<int> removalList;
    private int selectedPrefabIndex = -1;

    //Menu Scrolling
    static Vector2 topScroll = Vector2.zero;

    //Tool Tip
    private string curTip = string.Empty;
    private bool drawCurTip = false; //this is set false on each redraw and checked at the end of OnGUI - set by DoTipCheck if a control with a tip is currently moused over
    #endregion

    //static bool 		useGrid = false;
    //static bool 		showGrid = false;
    //static float 		gridSpacing = 1f;
    //static bool		gridFoldout = true;

    //static float 		baseSpacing = 1f;
    //static float 		spacingRandomRange = 0f;
    //static bool		spacingFoldout = true;

    void OnGUI()
    {
        UpdateGroups();
        BuildStyles();
        drawCurTip = false;

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(masterVerticalStyle, GUILayout.Width(280)); //Begin Master Vertical

        EditorGUILayout.BeginHorizontal();//Brush Toggles Section Begin

        Texture2D brushIndicatorTexture = null;

        switch (brushMode)
        {
            case BrushMode.Off:
                if (brushDirection == true)
                {
                    brushIndicatorTexture = brushIcon_Inactive;
                }

                if (brushDirection == false)
                {
                    brushIndicatorTexture = eraserIcon_Inactive;
                }
                break;

            case BrushMode.On:

                if (placementModifier)
                    brushIndicatorTexture = placementIcon_Active;

                else
                {
                    if (brushDirection == true)
                    {
                        brushIndicatorTexture = brushIcon_Active;
                    }

                    if (brushDirection == false)
                    {
                        brushIndicatorTexture = eraserIcon_Active;
                    }
                }
                break;
        }

        GUILayout.Label(brushIndicatorTexture, picButtonStyle, GUILayout.Width(32), GUILayout.Height(40));
        DoTipCheck("Brush On/Off Indicator");

        EditorGUI.BeginDisabledGroup(true);
        GUILayout.Label("Рисовать кистью:" + System.Environment.NewLine + "Поставить экземпляр:" + System.Environment.NewLine + "Стереть:", tipLabelStyle, GUILayout.Width(120), GUILayout.Height(40)); DoTipCheck("Зажамите ctrl и рисуйте");
        GUILayout.Label("ctrl+click/вести мышь" + System.Environment.NewLine + "ctrl+shift+click/вести мышь" + System.Environment.NewLine + "ctrl+x", tipLabelStyle, GUILayout.Width(146), GUILayout.Height(40)); DoTipCheck("Зажамите ctrl и рисуйте");
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal(); // Brush Toggles Section End


        #region Prefab Picker
        var newPrefabs = new List<Object>();
        removalList = new List<int>();


        EditorGUILayout.BeginHorizontal();

        if (prefabGroup.Length == 0)
        {
            EditorGUILayout.BeginVertical(GUILayout.Height(78));
            newPrefabs = PrefabDragBox((int)window.position.width - 6, addPrefabFieldTexture, "Перетащите сюда префаб"); DoTipCheck("Перетащите сюда префаб для добавления");
            EditorGUILayout.EndVertical();
        }

        else
        {
            newPrefabs = PrefabDragBox((int)window.position.width - 6, addPrefabTexture, ""); DoTipCheck("Перетащите сюда префаб для добавления");

            prefabFieldScrollPosition = EditorGUILayout.BeginScrollView(prefabFieldScrollPosition, GUILayout.Height(78));
            EditorGUILayout.BeginHorizontal();
            //Prefab Objects can be dragged or selected in this horizontal list

            for (int i = 0; i < prefabGroup.Length; i++)
            {
                EditorGUILayout.BeginHorizontal(prefabFieldStyle, GUILayout.Height(60), GUILayout.Width(70));

                prefabGroup[i].weight = GUILayout.VerticalSlider(prefabGroup[i].weight, 1f, 0.001f, prefabAmountSliderStyle, prefabAmountSliderThumbStyle, GUILayout.Height(50)); DoTipCheck("Likelyhood that this object will be placed vs the others in the list");
                EditorGUILayout.BeginVertical();

                Texture2D previewTexture = null;

#if UNITY_3_5
							previewTexture =  EditorUtility.GetAssetPreview(prefabGroup[i].prefab);
#else
                previewTexture = AssetPreview.GetAssetPreview(prefabGroup[i].prefab);
#endif

                prefabPreviewWindowStyle.normal.background = previewTexture;//previewTexture;

                GUILayout.Label("", prefabPreviewWindowStyle, GUILayout.Height(50), GUILayout.Width(50));

                var prefabButtonRect = GUILayoutUtility.GetLastRect();
                var xControlRect = new Rect(prefabButtonRect.xMax - 14, prefabButtonRect.yMin, 14, 14);

                if (GUI.Button(xControlRect, "", prefabRemoveXStyle))
                {
                    removalList.Add(i);
                    Event.current.Use();
                } DoTipCheck("'Red X' = remove prefab from list" + System.Environment.NewLine + "'Green Check' = mark to place exclusively");

                var checkControlRect = new Rect(prefabButtonRect.xMax - 14, prefabButtonRect.yMax - 14, 14, 14);

                prefabSelectCheckStyle.normal.background = selectedPrefabIndex == i ? selectPrefabCheckTexture_hover : selectPrefabCheckTexture_normal;


                if (GUI.Button(checkControlRect, "", prefabSelectCheckStyle))
                {
                    if (selectedPrefabIndex != i)
                        selectedPrefabIndex = i;

                    else
                        selectedPrefabIndex = -1;
                } //DoTipCheck("Click to mark object as selected - it will be placed exclusively");

                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.EndHorizontal();

        if (removalList.Count > 0)
        {
            foreach (int index in removalList)
            {

                if (selectedPrefabIndex > index)
                {
                    selectedPrefabIndex -= 1;
                }

                else if (selectedPrefabIndex == index)
                {
                    selectedPrefabIndex = -1;
                }

                ArrayUtility.RemoveAt(ref prefabGroup, index);
            }

            //				EditorUtility.SetDirty(prefabGroup);
        }

        if (newPrefabs.Count > 0)
        {
            foreach (Object newPrefab in newPrefabs)
            {
                ArrayUtility.Add(ref prefabGroup, new qb_PrefabObject(newPrefab, 1f));
            }
            //				added = true;
            //			    EditorUtility.SetDirty(prefabGroup);
        }

        #endregion


        topScroll = EditorGUILayout.BeginScrollView(topScroll, GUILayout.Width((int)window.position.width - 6));
        EditorGUILayout.BeginVertical(GUILayout.Width((int)window.position.width - 6));

        #region	Stroke Properties
        brushSettingsFoldout = EditorGUILayout.Foldout(brushSettingsFoldout, "Настройка кисти:"); DoTipCheck("Настройка кисти и мазка");

        if (brushSettingsFoldout == true)
        {
            EditorGUILayout.BeginVertical(menuBlockStyle, GUILayout.Width((int)window.position.width - 8));

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Радиус кисти", GUILayout.Width(100)); DoTipCheck("Размер окружности кисти");
            brushRadius = EditorGUILayout.Slider(brushRadius, brushRadiusMin, brushRadiusMax); DoTipCheck("Размер окружности кисти");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Минимум", GUILayout.Width(70)); DoTipCheck("Слайдер мин. значение");
            var tryRadiusMin = EditorGUILayout.FloatField(brushRadiusMin, floatFieldCompressedStyle); DoTipCheck("Слайдер мин. значение");
            brushRadiusMin = tryRadiusMin < brushRadiusMax ? tryRadiusMin : brushRadiusMax;

            EditorGUILayout.LabelField("Максимум", GUILayout.Width(70)); DoTipCheck("Слайдер макс. значение");
            var tryRadiusMax = EditorGUILayout.FloatField(brushRadiusMax, floatFieldCompressedStyle); DoTipCheck("Слайдер макс. значение");
            brushRadiusMax = tryRadiusMax > brushRadiusMin ? tryRadiusMax : brushRadiusMin;
            EditorGUILayout.EndHorizontal();

            //	EditorGUILayout.EndVertical();

            //	EditorGUILayout.BeginVertical(menuBlockStyle,GUILayout.Width(260));

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Плотность", GUILayout.Width(100)); DoTipCheck("Плотность разброса элементов в радиусе кисти");
            scatterRadius = EditorGUILayout.Slider(scatterRadius, 0f, 1f); DoTipCheck("Плотность разброса элементов в радиусе кисти");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(menuBlockStyle, GUILayout.Width((int)window.position.width - 8));

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Плотность мазка", GUILayout.Width(100)); DoTipCheck("Расстояние между элементами при нанесении мазка за один проход");
            brushSpacing = EditorGUILayout.Slider(brushSpacing, brushSpacingMin, brushSpacingMax); DoTipCheck("Расстояние между элементами при нанесении мазка за один проход");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Минимум", GUILayout.Width(70)); DoTipCheck("Слайдер мин. значение");
            var trySpacingMin = EditorGUILayout.FloatField(brushSpacingMin, floatFieldCompressedStyle); DoTipCheck("Слайдер мин. значение");
            brushSpacingMin = trySpacingMin < brushSpacingMax ? trySpacingMin : brushSpacingMax;

            EditorGUILayout.LabelField("Максимум", GUILayout.Width(70)); DoTipCheck("Слайдер макс. значение");
            var trySpacingMax = EditorGUILayout.FloatField(brushSpacingMax, floatFieldCompressedStyle); DoTipCheck("Слайдер макс. значение");
            brushSpacingMax = trySpacingMax > brushSpacingMin ? trySpacingMax : brushSpacingMin;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

        }
        #endregion

        EditorGUILayout.Space();

        sortingFoldout = EditorGUILayout.Foldout(sortingFoldout, "Настройки группировки:"); DoTipCheck("Настройки группировки и используемых слоев");

        if (sortingFoldout == true)
        {
            #region Layers

            EditorGUILayout.BeginVertical(menuBlockStyle, GUILayout.Width((int)window.position.width - 8));
            //A toggle determining whether to isolate painting to specific layers
            paintToLayer = EditorGUILayout.Toggle("Рисовать по слою", paintToLayer, EditorStyles.toggle); DoTipCheck("Во время рисования объекты будут наносится на все объекты слоя");
            //A dropdown where the user can check off which layers to paint to
            EditorGUI.BeginDisabledGroup(!paintToLayer);
            layerIndex = EditorGUILayout.LayerField("Выбрать слой", layerIndex); DoTipCheck("Во время рисования объекты будут наносится на все объекты данного слоя");
            EditorGUI.EndDisabledGroup();

            paintToSelection = EditorGUILayout.Toggle("Рисовать по выбранному", paintToSelection); DoTipCheck("Во время рисования объекты будут наносится на все выделенные объекты");

            EditorGUILayout.EndVertical();

            #endregion

            #region Groups
            EditorGUILayout.BeginVertical(menuBlockStyle, GUILayout.Width((int)window.position.width - 8));

            groupObjects = EditorGUILayout.Toggle("Групировать", groupObjects); DoTipCheck("Все объекты будут помещены в отдельную группу");

            EditorGUI.BeginDisabledGroup(!groupObjects);
            groupDropdownIndex = EditorGUILayout.Popup("Выбрать группу", groupDropdownIndex, groupNames.ToArray()); DoTipCheck("Выбирите группу в которую будут добавлены объекты");

            if (groups.Count != 0)
                curGroup = groups[groupDropdownIndex];

            EditorGUILayout.BeginHorizontal();
            newGroupName = EditorGUILayout.TextField("Имя новой группы", newGroupName, GUILayout.Width(210)); DoTipCheck("Введите имя новой группы для добовление в список выбора");

            EditorGUI.BeginDisabledGroup(newGroupName == "");
            if (GUILayout.Button("Добавить", GUILayout.Width(70)))
            {
                CreateGroup(newGroupName);
                newGroupName = "";
            } DoTipCheck("Создать новую группу");
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndVertical();
            #endregion
        }

        EditorGUILayout.Space();

        #region Rotation
        rotationFoldout = EditorGUILayout.Foldout(rotationFoldout, "Вращение:"); DoTipCheck("Вращение объектов при рисовании");
        if (rotationFoldout == true)
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginVertical(menuBlockStyle, GUILayout.Width((int)window.position.width - 8));
            alignToNormal = EditorGUILayout.Toggle("По поверхности", alignToNormal); DoTipCheck("Вращение объектов относительно поверхности");

            alignToStroke = EditorGUILayout.Toggle("По ходу", alignToStroke); DoTipCheck("Вращение объектов относительно хода мазка кисти");
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(menuBlockStyle, GUILayout.Width((int)window.position.width - 8));

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Смещение по X", GUILayout.Width(100)); DoTipCheck("Upper limit (in degrees) to randomly offset object rotation around the X axis");
            rotationRange.x = EditorGUILayout.Slider(rotationRange.x, 0f, 179f); DoTipCheck("Upper limit (in degrees) to randomly offset object rotation around the X axis");
            rotationRange.x = (float)System.Math.Round(rotationRange.x, 0);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Смещение по Y", GUILayout.Width(100)); DoTipCheck("Upper limit (in degrees) to randomly offset object rotation around the Y axis");
            rotationRange.y = EditorGUILayout.Slider(rotationRange.y, 0f, 179f); DoTipCheck("Upper limit (in degrees) to randomly offset object rotation around the Y axis");
            rotationRange.y = (float)System.Math.Round(rotationRange.y, 0);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Смещение по Z", GUILayout.Width(100)); DoTipCheck("Upper limit (in degrees) to randomly offset object rotation around the Z axis");
            rotationRange.z = EditorGUILayout.Slider(rotationRange.z, 0f, 179f); DoTipCheck("Upper limit (in degrees) to randomly offset object rotation around the Z axis");
            rotationRange.z = (float)System.Math.Round(rotationRange.z, 0);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
        #endregion

        EditorGUILayout.Space();

        #region Scale
        scaleFoldout = EditorGUILayout.Foldout(scaleFoldout, "Диапазон изменения размеров:", EditorStyles.foldout); DoTipCheck("Настройки масштабирования размещаемых объектов");
        if (scaleFoldout == true)
        {
            EditorGUILayout.BeginHorizontal();

            scaleUniform = EditorGUILayout.Toggle(scaleUniform, toggleButtonStyle, GUILayout.Width(15)); DoTipCheck("Равномерное масштабирование объектов в заданном диапазоне");
            GUILayout.Label("Равномерно"); DoTipCheck("Равномерное масштабирование объектов в заданном диапазоне");

            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(!scaleUniform);

            EditorGUILayout.BeginHorizontal(GUILayout.Width((int)window.position.width - 8));

            EditorGUILayout.BeginVertical(menuBlockStyle, GUILayout.Width(78));

            EditorGUILayout.LabelField(scaleRandMinUniform + " для " + scaleRandMaxUniform, GUILayout.Width(78)); DoTipCheck("Слайдер диапазона случайных значений (Min/Max)");
            EditorGUILayout.MinMaxSlider(ref scaleRandMinUniform, ref scaleRandMaxUniform, scaleMin, scaleMax); DoTipCheck("Слайдер диапазона случайных значений (Min/Max)");

            scaleRandMinUniform = (float)System.Math.Round(scaleRandMinUniform, 1);
            scaleRandMaxUniform = (float)System.Math.Round(scaleRandMaxUniform, 1);
            scaleRandMinUniform = Mathf.Clamp(scaleRandMinUniform, scaleMin, scaleMax);
            scaleRandMaxUniform = Mathf.Clamp(scaleRandMaxUniform, scaleMin, scaleMax);

            if (scaleUniform)
            {
                scaleRandMin = new Vector3(scaleRandMinUniform, scaleRandMinUniform, scaleRandMinUniform);
                scaleRandMax = new Vector3(scaleRandMaxUniform, scaleRandMaxUniform, scaleRandMaxUniform);
            }

            EditorGUILayout.EndVertical();

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.BeginVertical(menuBlockStyle, GUILayout.Width(170), GUILayout.MaxWidth(170));
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Мнимум", GUILayout.Width(108)); DoTipCheck("Слайдер: Минимальное значение");
            scaleMin = EditorGUILayout.FloatField(scaleMin, floatFieldCompressedStyle); DoTipCheck("Слайдер: Минимальное значение");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Максимум", GUILayout.Width(108)); DoTipCheck("Слайдер: Максимальное значение");
            scaleMax = EditorGUILayout.FloatField(scaleMax, floatFieldCompressedStyle); DoTipCheck("Слайдер: Максимальное значение");
            EditorGUILayout.EndHorizontal();

            scaleMin = (float)System.Math.Round(scaleMin, 1);
            scaleMax = (float)System.Math.Round(scaleMax, 1);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            //-------------------------
            EditorGUILayout.Space();
            //-------------------------

            EditorGUILayout.BeginHorizontal();

            scaleUniform = !EditorGUILayout.Toggle(!scaleUniform, toggleButtonStyle, GUILayout.Width(15)); DoTipCheck("Модель масштабируется отдельно по каждой оси");
            GUILayout.Label("Масштаб по оси"); DoTipCheck("Модель масштабируется отдельно по каждой оси");

            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(scaleUniform);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(menuBlockStyle);
            scaleRandMin.x = Mathf.Clamp(scaleRandMin.x, scaleMin, scaleMax);
            EditorGUILayout.LabelField("X " + scaleRandMin.x.ToString("0.0") + " to " + scaleRandMax.x.ToString("0.0"), GUILayout.Width(76)); DoTipCheck("X Axis Random Scaling Range Split Slideer (Min/Max)");
            EditorGUILayout.MinMaxSlider(ref scaleRandMin.x, ref scaleRandMax.x, scaleMin, scaleMax); DoTipCheck("X Axis Random Scaling Range Split Slideer (Min/Max)");

            scaleRandMin.x = (float)System.Math.Round(scaleRandMin.x, 1);
            scaleRandMax.x = (float)System.Math.Round(scaleRandMax.x, 1);
            scaleRandMin.x = Mathf.Clamp(scaleRandMin.x, scaleMin, scaleMax);
            scaleRandMax.x = Mathf.Clamp(scaleRandMax.x, scaleMin, scaleMax);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(menuBlockStyle);
            scaleRandMin.y = Mathf.Clamp(scaleRandMin.y, scaleMin, scaleMax);
            EditorGUILayout.LabelField("Y " + scaleRandMin.y.ToString("0.0") + " to " + scaleRandMax.y.ToString("0.0"), GUILayout.Width(76)); DoTipCheck("Y Axis Random Scaling Range Split Slideer (Min/Max)");
            EditorGUILayout.MinMaxSlider(ref scaleRandMin.y, ref scaleRandMax.y, scaleMin, scaleMax); DoTipCheck("Y Axis Random Scaling Range Split Slideer (Min/Max)");

            scaleRandMin.y = (float)System.Math.Round(scaleRandMin.y, 1);
            scaleRandMax.y = (float)System.Math.Round(scaleRandMax.y, 1);
            scaleRandMin.y = Mathf.Clamp(scaleRandMin.y, scaleMin, scaleMax);
            scaleRandMax.y = Mathf.Clamp(scaleRandMax.y, scaleMin, scaleMax);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(menuBlockStyle);
            scaleRandMin.z = Mathf.Clamp(scaleRandMin.z, scaleMin, scaleMax);
            EditorGUILayout.LabelField("Z " + scaleRandMin.z.ToString("0.0") + " to " + scaleRandMax.z.ToString("0.0"), GUILayout.Width(76)); DoTipCheck("Z Axis Random Scaling Range Split Slideer (Min/Max)");
            EditorGUILayout.MinMaxSlider(ref scaleRandMin.z, ref scaleRandMax.z, scaleMin, scaleMax); DoTipCheck("Z Axis Random Scaling Range Split Slideer (Min/Max)");

            scaleRandMin.z = (float)System.Math.Round(scaleRandMin.z, 1);
            scaleRandMax.z = (float)System.Math.Round(scaleRandMax.z, 1);
            scaleRandMin.z = Mathf.Clamp(scaleRandMin.z, scaleMin, scaleMax);
            scaleRandMax.z = Mathf.Clamp(scaleRandMax.z, scaleMin, scaleMax);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            EditorGUI.EndDisabledGroup();

        }

        EditorGUILayout.EndVertical(); //Overall Vertical Container End
        EditorGUILayout.EndScrollView(); // Overall Scroll View End
        #endregion
        /*
	#region Grid
		gridFoldout = EditorGUILayout.Foldout(gridFoldout,"Grid");
		if(gridFoldout == true)
		{
			EditorGUILayout.BeginVertical();

			useGrid = EditorGUILayout.Toggle("Use Grid:",useGrid);
			gridSpacing = EditorGUILayout.FloatField("",gridSpacing); //Default is 1f
			showGrid =  EditorGUILayout.Toggle("Show Grid:",useGrid);
			
			EditorGUILayout.EndVertical();
		}

	#endregion
*/
        //	EditorGUILayout.EndVertical();

        if (GUILayout.Button("Настройки по умолчанию"))
        {
            LoadDefaultSettings();
            SaveSettings();
        }

        string tipToDraw = drawCurTip ? curTip : string.Empty;

        //EditorGUILayout.BeginVertical(GUILayout.Width(280));//,GUILayout.MaxWidth(280));
        EditorGUILayout.HelpBox(tipToDraw, MessageType.Info);


        EditorGUILayout.EndVertical();//Master Vertical End

        this.Repaint();

    }

    static void DoTipCheck(string entry)
    {

        if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
        {
            window.curTip = entry;
            window.drawCurTip = true;
        }

    }

    static private void DrawBrushGizmo(RaycastHit mouseRayHit)
    {
        if (placing == false)
        {
            if (mouseRayHit.collider != null)
            {
                Handles.color = Color.white;
                Handles.DrawWireDisc(mouseRayHit.point, mouseRayHit.normal, window.brushRadius);
                Handles.color = Color.blue;
                Handles.DrawWireDisc(mouseRayHit.point, mouseRayHit.normal, window.scatterRadius * window.brushRadius);
            }
        }

        else
        {
            if (placingObject != null)
            {
                Handles.color = Color.green;
                Handles.DrawWireDisc(placingObject.transform.position, placingUpVector, Vector3.Distance(placingPlanePoint, placingObject.transform.position));
                Handles.DrawSolidDisc(placingPlanePoint, placingUpVector, 0.1f);
                Handles.DrawPolyLine(new Vector3[2] { placingObject.transform.position, placingPlanePoint });
                Handles.ArrowCap(0, placingPlanePoint + ((placingPlanePoint - placingObject.transform.position).normalized * -0.5f), Quaternion.LookRotation(placingPlanePoint - placingObject.transform.position, placingUpVector), 1f);
            }
        }

    }

    private enum BrushMode
    {
        Off,
        On
    }

    static bool placing;	//currently placing an object - ie have spawned object and not yet stopped modifying scale / rotation
    static bool painting; 	//in stroke

    public void OnSceneGUI(SceneView sceneView)
    {
        /* Rules pseudo code
        //if ALT not down
        //{
            //if mouse click when shift down -		begin PlaceMode
            //if drag while PlaceMode -				scale and rotate (calculate and execute scale and rotation on stored object)
            //if mouse up while placing -			end PlaceMode (if shift is still held down it will pop back on based on the stuff above)
            //if mouse right down while placing -	remove stored object from scene
			
            //if click when shift up -				begin StrokeMode
            //if drag while StrokeMode -			Paint stuff
            //if mouse up while StokeMode -			end StrokeMode
        //}
        */


        bool altDown = false;
        bool shiftDown = false;
        bool ctrlDown = false;
        //		bool xDown = false;

        if (Event.current.control)
        {
            ctrlDown = true;
            sceneView.Focus();
        }

        if (ctrlDown == false)
        {
            brushMode = BrushMode.Off;
            EndStroke();
            EndPlaceStroke();
            return;
        }

        else
            brushMode = BrushMode.On;

        //	if(xDown)
        //		brushDirection = !brushDirection;


        RaycastHit mouseRayHit = new RaycastHit();
        mouseRayHit = DoMouseRaycast();

        DrawBrushGizmo(mouseRayHit);
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        //Default Mode is Paint, from there we can 
        if (Event.current.alt)
        {
            //modify mode to Camera Navigation
            //if we are currently painting end stroke
            //if we are currently placing commit
            altDown = true;
        }

        if (Event.current.shift) //might want to force end 
        {
            //modify mode to Place
            shiftDown = true;
            placementModifier = true;
        }

        else
            placementModifier = false;

        if (GetKeyUp == KeyCode.X)
        {
            brushDirection = !brushDirection;
        }

        if (altDown == false)
        {
            switch (Event.current.type)
            {

                case EventType.mouseUp:
                    if (Event.current.button == 0)
                    {
                        if (brushMode == BrushMode.On)
                        {
                            if (painting == true)
                            {
                                EndStroke();
                            }

                            if (placing == true)
                            {
                                EndPlaceStroke();
                            }
                        }
                    }
                    break;

                case EventType.mouseDown:
                    if (Event.current.button == 0)
                    {
                        if (brushMode == BrushMode.On)
                        {
                            if (!shiftDown)
                            {
                                BeginStroke();

                                if (paintable)
                                {
                                    Paint(mouseRayHit);
                                    Event.current.Use();
                                }
                                Tools.current = Tool.None;
                            }

                            else //shiftDown
                            {
                                if (paintable)
                                {
                                    BeginPlaceStroke();
                                    Event.current.Use();
                                }
                                Tools.current = Tool.None;
                            }
                        }
                    }
                    break;

                case EventType.mouseDrag:
                    if (Event.current.button == 0)
                    {
                        if (brushMode == BrushMode.On)
                        {
                            if (placing == true)
                            {
                                //if(paintable)
                                //{
                                UpdatePlace(sceneView);
                                Event.current.Use();
                                Tools.current = Tool.None; //make sure needed?
                                //}
                            }

                            else if (painting == true)
                            {
                                if (paintable)
                                {
                                    Paint(mouseRayHit);
                                    Event.current.Use();
                                    Tools.current = Tool.None; //make sure needed?
                                }
                            }

                        }
                    }
                    HandleUtility.Repaint();
                    break;

                case EventType.mouseMove:
                    HandleUtility.Repaint();
                    break;

            }
        }

        CalculateCursor(mouseRayHit);

        //This repaint is important to make lines and indicators not hang around for more frames
        sceneView.Repaint();

    }

    static void BeginStroke()
    {
        curStroke = new qb_Stroke();
        painting = true;
    }

    static void EndStroke()
    {
        curStroke = null;
        painting = false;
    }

    static void UpdateStroke()
    {
        //use the calculated stored cursor position to check distance from previous point on the stroke

        //if the calculated cursor position is at or beyond the BrushSpacingDistance from the last point in the stroke
        //add a point to the stroke 

        if (curStroke.GetCurPoint() == null) //there is no cur point, we are starting the stroke 
        {
            qb_Point nuPoint = curStroke.AddPoint(cursorPoint.position, cursorPoint.upVector, cursorPoint.dirVector);
            DoBrushIterration(nuPoint);
        }

        else
        {
            float distanceFromLastPt = Vector3.Distance(cursorPoint.position, curStroke.GetCurPoint().position);
            Vector3 strokeDirection = cursorPoint.position - curStroke.GetCurPoint().position;

            if (distanceFromLastPt >= window.brushSpacing)
            {
                //Debug.DrawRay(cursorPoint.position,strokeDirection * strokeDirection.magnitude * -1f,Color.red);
                qb_Point newPoint = curStroke.AddPoint(cursorPoint.position, cursorPoint.upVector, strokeDirection.normalized);
                DoBrushIterration(newPoint);
            }
        }
    }

    static void DoBrushIterration(qb_Point newPoint) // do whatever needs to be done on the bruh itteration
    {
        //if brush is positive
        //do a paint itteration
        if (brushDirection == true)
            PlaceGeo(newPoint);

        //if brush is negative
        //do an erase itteration
        else
            EraseGeo(newPoint);

        //later, we'll need another case for a vertex color brush, probably just an additional layer rather than exclusive

    }

    static GameObject placingObject;
    static Vector3 placingUpVector;

    static void BeginPlaceStroke()
    {
        curStroke = new qb_Stroke();
        qb_Point nuPoint = curStroke.AddPoint(cursorPoint.position, cursorPoint.upVector, cursorPoint.dirVector);//cursorPoint.dirVector);

        placingObject = PlaceObject(nuPoint);//PlaceGeo(nuPoint);

        if (placingObject != null)
        {
            placing = true;
            placingUpVector = placingObject.transform.up;
        }
    }

    static void EndPlaceStroke()
    {
        curStroke = null;

        //release from placing mode
        placing = false;
    }

    static Vector3 placingPlanePoint = Vector3.zero;

    static void UpdatePlace(SceneView sceneView)
    {
        if (placingObject != null)
        {
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Vector3 mouseWorldPoint = mouseRay.GetPoint(0f);

            placingPlanePoint = GetLinePlaneIntersectionPoint(mouseWorldPoint, mouseRay.direction, placingObject.transform.position, placingUpVector); //contact;//Vector3.Project(placingObjectRay.direction,placingUpVector);

            Vector3 difVector = placingPlanePoint - placingObject.transform.position;

            Bounds placingObjectBounds = TryGetObjectBounds(placingObject);

            float modifiedScale = 1f;

            //	if(placingObjectBounds == null)
            //		modifiedScale = (difVector.magnitude * 2f);

            //	else
            //		modifiedScale = (difVector.magnitude * 2f) / ((Mathf.Max(placingObject.renderer.bounds.extents.x , placingObject.renderer.bounds.extents.x) * 2) / placingObject.transform.localScale.x);

            modifiedScale = (difVector.magnitude * 2f) / /*((Mathf.Max(placingObjectBounds.extents.x , placingObjectBounds.extents.y) * 2)*/(placingObjectBounds.size.magnitude / placingObject.transform.localScale.x);

            placingObject.transform.rotation = Quaternion.LookRotation(difVector, placingUpVector);//this has to be rotation relative to the original placement rotation along its disk in the direction of the mouse pointer
            placingObject.transform.localScale = new Vector3(modifiedScale, modifiedScale, modifiedScale);//This has to be the distance between the screen cursor's position and the screen bound position of the object's placement point
        }
    }

    static Bounds TryGetObjectBounds(GameObject topObject)
    {
        //We need to itterate through the object's hierarchy and determine if it has any kind of object with bounds
        //if yes	return cumulative bounds
        //if no		return null

        //So, we itterate through the hierarchy to find any renderers or meshes to get bounds from
        //Then we combine all bounds to get a total
        Bounds combinedBounds = new Bounds(topObject.transform.position, new Vector3(1f, 1f, 1f));

        if (topObject.GetComponent(typeof(MeshRenderer)))
            combinedBounds = topObject.GetComponent<Renderer>().bounds;

        Renderer[] renderers = topObject.GetComponentsInChildren<MeshRenderer>() as Renderer[];// as Renderer[];

        foreach (Renderer render in renderers)
        {
            //if (render != renderer) 
            combinedBounds.Encapsulate(render.bounds);
        }

        return combinedBounds;
    }

    static Vector3 GetLinePlaneIntersectionPoint(Vector3 rayOrigin, Vector3 rayDirection, Vector3 pointOnPlane, Vector3 planeNormal)
    {
        float epsilon = 0.0000001f;
        Vector3 contact = Vector3.zero;


        Vector3 ray = (rayOrigin + (rayDirection * 1000f)) - rayOrigin;
        //Vector3 ray = rayOrigin - (rayDirection * Mathf.Infinity); 

        Vector3 difVector = rayOrigin - pointOnPlane;

        float dot = Vector3.Dot(planeNormal, ray);

        if (Mathf.Abs(dot) > epsilon)
        {
            float fac = -Vector3.Dot(planeNormal, difVector) / dot;
            Vector3 fin = ray * fac;

            contact = rayOrigin + fin;
        }

        return contact;

    }

    static Vector3 GetFlattenedDirection(Vector3 worldVector, Vector3 flattenUpVector)
    {
        Vector3 flattened = Vector3.Cross(flattenUpVector, worldVector);
        Vector3 diskDirection = Vector3.Cross(flattened, flattenUpVector);

        return diskDirection;
    }

    static Object PickRandPrefab()
    {

        float totalWeight = 0f;
        for (int i = 0; i < window.prefabGroup.Length; i++)
        {
            totalWeight += window.prefabGroup[i].weight;
        }

        float randomNumber = Random.Range(0f, totalWeight);

        float weightSum = 0f;
        int chosenIndex = 0;

        for (int x = 0; x < window.prefabGroup.Length; x++)
        {
            weightSum += window.prefabGroup[x].weight;

            if (randomNumber < weightSum)//prefabGroup[x].weight)
            {
                chosenIndex = x;
                break;
            }
        }

        return window.prefabGroup[chosenIndex].prefab;

    }

    static void Paint(RaycastHit mouseRayHit) //This function is called when the stroke reaches its next step - We feed it the hit from the latest Raycast
    {
        //were only here if the cursor is over a paintable object and the mouse button is pressed
        CalculateCursor(mouseRayHit);

        UpdateStroke();
    }

    static Object objectToSpawn;
    private static void PlaceGeo(qb_Point newPoint)
    {
        //-1 : if there are no prefabs in the queue. Do not paint
        if (window.prefabGroup.Length == 0)
            return;

        //0	: declare function variables
        Vector3 spawnPosition = Vector3.zero;
        Quaternion spawnRotation = Quaternion.identity;
        //Vector3 spawnScale = new Vector3(1f,1f,1f);
        Vector3 upVector = Vector3.up;
        Vector3 forwardVector = Vector3.forward; //blank filled - this value should never end up being used

        //1 : if there is more than one prefab in the queue, pick one using the randomizer
        if (window.prefabGroup.Length > 0)
        {
            if (window.selectedPrefabIndex != -1)
            {
                if (window.prefabGroup.Length > window.selectedPrefabIndex && window.prefabGroup[window.selectedPrefabIndex] != null)
                    objectToSpawn = window.prefabGroup[window.selectedPrefabIndex].prefab;

                else
                {
                    window.selectedPrefabIndex = -1;
                    return;
                }
            }

            else
                objectToSpawn = PickRandPrefab();
        }

        else
            return;

        //2 : use the current point in the stroke to Get a random point around its upVector Axis
        Vector3 castPosition = GetRandomPointOnDisk(newPoint.upVector);//Vector3.zero;

        //3 : use the random disk point to cast down along the upVector of the stroke point
        Vector3 rayDir = -newPoint.upVector;
        //RaycastHit hit;

        qb_RaycastResult result = DoPlacementRaycast(castPosition + (rayDir * -0.02F), rayDir);

        //4 : if cast successful, get cast point and normal - if cast is unsuccessful, return...<---
        if (result.success == true)
        {
            spawnPosition = result.hit.point;

            if (window.alignToNormal == true)
            {
                upVector = result.hit.normal;
                forwardVector = GetFlattenedDirection(Vector3.forward, upVector);
            }

            forwardVector = GetFlattenedDirection(Vector3.forward, upVector);

            if (window.alignToStroke == true)
            {
                //forwardVector = curStroke.GetCurPoint().dirVector;
                forwardVector = GetFlattenedDirection(curStroke.GetCurPoint().dirVector, upVector);
            }
        }

        else
            return;

        //5 : instantiate the prefab
        GameObject newObject = null;

        newObject = PrefabUtility.InstantiatePrefab(objectToSpawn) as GameObject;
        qb_Object marker = newObject.AddComponent<qb_Object>();//.hideFlags = HideFlags.HideInInspector;
        marker.hideFlags = HideFlags.HideInInspector;
        Undo.RegisterCreatedObjectUndo(newObject, "QB Place Object");

        //6 : use settings to scale, rotate, and place the object
        spawnRotation = GetSpawnRotation(upVector, forwardVector);

        newObject.transform.position = spawnPosition;
        newObject.transform.rotation = spawnRotation;

        Vector3 randomScale;

        if (window.scaleUniform == true)
        {
            float randomScaleUni = Random.Range(window.scaleRandMinUniform, window.scaleRandMaxUniform);
            randomScale = new Vector3(randomScaleUni, randomScaleUni, randomScaleUni);
        }

        else
            randomScale = new Vector3(Random.Range(window.scaleRandMin.x, window.scaleRandMax.x), Random.Range(window.scaleRandMin.y, window.scaleRandMax.y), Random.Range(window.scaleRandMin.z, window.scaleRandMax.z));

        newObject.transform.localScale = new Vector3(randomScale.x, randomScale.y, randomScale.z);//Random.Range(scaleMin.x,scaleMax.x),Random.Range(scaleMin.y,scaleMax.y),Random.Range(scaleMin.z,scaleMax.z));//spawnScale;

        //7 : If we have a group, add the object to the group
        if (window.groupObjects == true && curGroup != null)
        {
            curGroup.AddObject(newObject);
        }

        //		qb_ObjectContainer.GetInstance().AddObject(newObject);

    }

    private static GameObject PlaceObject(qb_Point newPoint)
    {
        //-1 : if there are no prefabs in the queue. Do not place
        if (window.prefabGroup.Length == 0)
            return null;

        if (window.prefabGroup[0] == null)
            return null;

        //0	: declare function variables
        Vector3 spawnPosition = Vector3.zero;
        Quaternion spawnRotation = Quaternion.identity;
        Vector3 upVector = Vector3.up;

        //1 : if there is more than one prefab in the queue, pick one

        if (window.selectedPrefabIndex != -1)
        {
            if (window.prefabGroup.Length > window.selectedPrefabIndex && window.prefabGroup[window.selectedPrefabIndex] != null)
                objectToSpawn = window.prefabGroup[window.selectedPrefabIndex].prefab;

            else
                window.selectedPrefabIndex = -1;

        }

        else
        {
            if (window.prefabGroup.Length > 0 && window.prefabGroup[0] != null)
                objectToSpawn = window.prefabGroup[0].prefab;

            else
            {
                //window.selectedPrefabIndex = -1;
                return null;
            }
        }
        //else return null;

        //2 : use the current point in the stroke to Get a random point around its upVector Axis
        Vector3 castPosition = newPoint.position;

        //3 : use the random disk point to cast down along the upVector of the stroke point
        Vector3 rayDir = -newPoint.upVector;

        qb_RaycastResult result = DoPlacementRaycast(castPosition, rayDir);

        //4 : if cast successful, get cast point and normal - if cast is unsuccessful, return...<---
        if (result.success == true)
        {
            spawnPosition = result.hit.point;

            if (window.alignToNormal == true)
            {
                upVector = result.hit.normal;
            }

        }

        else
            return null;

        //5 : instantiate the prefab
        GameObject newObject = null;

        newObject = PrefabUtility.InstantiatePrefab(objectToSpawn) as GameObject;
        qb_Object marker = newObject.AddComponent<qb_Object>();//.hideFlags = HideFlags.HideInInspector;
        marker.hideFlags = HideFlags.HideInInspector;
        Undo.RegisterCreatedObjectUndo(newObject, "QB Place Object");

        //6 : use settings to scale, rotate, and place the object
        if (window.alignToNormal)
        {
            spawnRotation = Quaternion.LookRotation(curStroke.GetCurPoint().dirVector, upVector);
        }

        else
        {
            spawnRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        }

        newObject.transform.position = spawnPosition;
        newObject.transform.rotation = spawnRotation;

        //7 : If we have a group, add the object to the group
        if (window.groupObjects == true && curGroup != null)
        {
            curGroup.AddObject(newObject);
        }

        //		qb_ObjectContainer.GetInstance().AddObject(newObject);

        return newObject;
    }

    private static void EraseGeo(qb_Point newPoint)
    {
        //		qb_ObjectContainer objectContainer = qb_ObjectContainer.GetInstance();

        GameObject[] objects = window.GetObjects();

        List<int> removalList = new List<int>();

        for (int i = 0; i < objects.Length; i++)
        {
            if (Vector3.Distance(objects[i].transform.position, newPoint.position) < window.brushRadius)
            {
                removalList.Add(i);
            }
        }

        if (removalList.Count > 0)
            window.EraseObjects(removalList);
    }

    private static qb_RaycastResult DoPlacementRaycast(Vector3 castPosition, Vector3 rayDirection)
    {
        RaycastHit hit;
        bool success = false;

        Physics.Raycast(castPosition + (-0.1f * rayDirection), rayDirection, out hit, float.MaxValue);

        if (hit.collider != null)
        {
            success = true;

            if (window.paintToLayer == true)
            {
                if (hit.collider.gameObject.layer != window.layerIndex)
                    success = false;
            }

            if (window.paintToSelection == true)
            {

                Transform[] selectedObjects = Selection.transforms;
                bool contains = ArrayUtility.Contains(selectedObjects, hit.collider.transform);

                if (!contains)
                    success = false;
            }
        }

        qb_RaycastResult result = new qb_RaycastResult(success, hit);

        return result;
    }

    private static Vector3 GetRandomPointOnDisk(Vector3 upVector)
    {
        float angle = Random.Range(0f, 1f) * Mathf.PI;
        Vector2 direction = new Vector2((float)Mathf.Cos(angle), (float)Mathf.Sin(angle));

        Vector3 direction3D = new Vector3(direction.x, 0f, direction.y);
        Vector3 flattened = Vector3.Cross(upVector, direction3D);
        Vector3 diskDirection = Vector3.Cross(flattened, upVector);

        float distanceFromCenter = (window.scatterRadius * window.brushRadius) * Random.Range(0.0f, 1.0f);
        Vector3 randomPoint = curStroke.GetCurPoint().position + (diskDirection.normalized * distanceFromCenter);

        return randomPoint;
    }

    //private static Quaternion GetPlacementSpawnRotation()
    //{

    //}

    private static Quaternion GetSpawnRotation(Vector3 upVector, Vector3 forwardVector)
    {
        //based on rotation range about axis
        //Quaternion rotation = Quaternion.AngleAxis(baseRotationY,upVector);
        //Vector3 upVector = curStroke.GetCurPoint().upVector;
        Quaternion rotation = Quaternion.identity;
        Vector3 rotationOffset = Vector3.zero;
        //Vector3 forwardVector = Vector3.forward;

        //if(alignToStroke)
        //	forwardVector = curStroke.GetCurPoint().dirVector;

        //if(alignToNormal == true)
        //{
        if (upVector.magnitude != 0 && forwardVector.magnitude != 0)
            rotation = Quaternion.LookRotation(forwardVector, upVector);
        //}
        if (placing)
            return rotation;

        rotationOffset = new Vector3(Random.Range(-window.rotationRange.x, window.rotationRange.x), Random.Range(-window.rotationRange.y, window.rotationRange.y), Random.Range(-window.rotationRange.z, window.rotationRange.z));

        rotation = rotation * Quaternion.Euler(rotationOffset);

        return rotation;
    }

    private static RaycastHit DoMouseRaycast() //Does a Raycast from mouse position and returns the hit - the main thread uses it to draw a handle and paint
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;

        if (window.paintToLayer == true && window.layerIndex != -1)
            Physics.Raycast(ray, out hit, float.MaxValue, 1 << window.layerIndex);

        else
            Physics.Raycast(ray, out hit, float.MaxValue);


        if (hit.collider != null)
        {
            paintable = true;

            if (window.paintToSelection == true)
            {
                Transform hitObject = hit.collider.transform;
                Transform[] selectedObjects = Selection.transforms;
                bool contains = ArrayUtility.Contains(selectedObjects, hitObject);

                if (!contains)
                {
                    hit = new RaycastHit();
                    paintable = false;
                }
            }
        }

        else
            paintable = false;

        return hit;
    }

    private static void CalculateCursor(RaycastHit mouseRayHit)
    {
        Vector3 upVector = Vector3.zero;
        Vector3 forwardVector = Vector3.zero;
        Vector3 positionVector = Vector3.zero;

        if (mouseRayHit.collider != null)
        {
            upVector = mouseRayHit.normal;
            positionVector = mouseRayHit.point;
            forwardVector = GetFlattenedDirection(Vector3.forward, upVector); //placement needs a direction to work with- we have no stroke direction yet, so we use flattened forward
        }

        cursorPoint.UpdatePoint(positionVector, upVector, forwardVector);
    }

    private static void CreateGroup(string groupName)
    {
        GameObject newGroupObject = new GameObject("QB_Group_" + groupName);
        curGroup = newGroupObject.AddComponent<qb_Group>();
        curGroup.groupName = groupName;
        groups.Add(curGroup);
        groupNames.Add(groupName);
    }

    private static void UpdateGroups() //updates the groups and groupNames arrays based on what is in the scene
    {
        qb_Group[] groupsInScene = GameObject.FindObjectsOfType(typeof(qb_Group)) as qb_Group[];

        groups.Clear();
        groupNames.Clear();

        for (int i = 0; i < groupsInScene.Length; i++)
        {
            groups.Add(groupsInScene[i]);
            groupNames.Add(groupsInScene[i].groupName);
        }

    }

    //This check is called whenever the tool needs to verify that the groups it has on record still exist in the scene
    //Rather than doing the full update groups loop, this is cheaper and is only concerned with groups that might have been deleted
    private static void CheckGroupsValid()
    {
        List<int> removalList = new List<int>();

        for (int i = 0; i < groups.Count; i++)
        {
            if (groups[i] == null)
                removalList.Add(i);
        }

        for (int x = 0; x < removalList.Count; x++)
        {
            groups.RemoveAt(removalList[x]);
            window.Repaint();

        }

        if (groups.Count == 0)
        {
            window.groupObjects = false;
            window.Repaint();
        }

    }

    static List<Object> PrefabDragBox(int width, Texture2D texture, string text)
    {
        List<Object> draggedObjects = new List<Object>();

        // Draw the controls

        window.prefabAddButtonStyle.normal.background = texture;

        GUILayout.Label(text, window.prefabAddButtonStyle, GUILayout.Width(width), GUILayout.MinWidth(width), GUILayout.Height(60), GUILayout.MinHeight(60));

        Rect lastRect = GUILayoutUtility.GetLastRect();


        // Handle events
        Event evt = Event.current;
        switch (evt.type)
        {
            case EventType.DragUpdated:
                // Test against rect from last repaint
                if (lastRect.Contains(evt.mousePosition))
                {
                    // Change cursor and consume the event
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    evt.Use();
                }
                break;

            case EventType.DragPerform:
                // Test against rect from last repaint
                if (lastRect.Contains(evt.mousePosition))
                {

                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        if (draggedObject.GetType() == typeof(GameObject))	//Debug.Log(draggedObject.GetType());
                        {
                            draggedObjects.Add(draggedObject);
                        }
                    }
                    // Change cursor and consume the event and drag
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    DragAndDrop.AcceptDrag();
                    evt.Use();
                }
                break;
        }

        return draggedObjects;
    }

    void OnDestroy()
    {
        brushMode = BrushMode.Off;
        SceneView.onSceneGUIDelegate -= onSceneGUIFunc;
        //		window = null;
    }

    public KeyCode GetKeyUp { get { return Event.current.type == EventType.KeyUp ? Event.current.keyCode : KeyCode.None; } }


    [SerializeField]
    private GUIStyle prefabAmountSliderStyle;
    [SerializeField]
    private GUIStyle prefabAmountSliderThumbStyle;
    [SerializeField]
    private GUIStyle toggleButtonStyle;
    [SerializeField]
    private GUIStyle prefabPreviewWindowStyle;
    [SerializeField]
    private GUIStyle prefabSelectCheckStyle;
    [SerializeField]
    private GUIStyle prefabRemoveXStyle;
    [SerializeField]
    private GUIStyle prefabFieldStyle;
    [SerializeField]
    private GUIStyle floatFieldCompressedStyle;
    [SerializeField]
    private GUIStyle prefabAddButtonStyle;
    [SerializeField]
    private GUIStyle picButtonStyle;
    [SerializeField]
    private GUIStyle menuBlockStyle;
    [SerializeField]
    private GUIStyle masterVerticalStyle;
    [SerializeField]
    private GUIStyle tipLabelStyle;

    //	[SerializeField] private GUIStyle scrollBarStyle;
    //	[SerializeField] private GUIStyle scrollBarStyleH;

    private void SetStyleParameters()
    {
        //		window.scrollBarStyle.margin.left = 0;
        //		window.scrollBarStyle.margin.right = 0;
        //		window.scrollBarStyle.padding.left = 0;
        //		window.scrollBarStyle.padding.right = 0;

        window.masterVerticalStyle.margin.left = 0;
        window.masterVerticalStyle.margin.right = 0;
        window.masterVerticalStyle.padding.left = 0;
        window.masterVerticalStyle.padding.left = 0;

        window.prefabAmountSliderStyle.margin.top = 4;

        window.prefabAmountSliderThumbStyle.fixedHeight = 10;

        window.prefabPreviewWindowStyle.margin.top = 4;

        window.prefabRemoveXStyle.normal.background = removePrefabXTexture_normal;
        window.prefabRemoveXStyle.hover.background = removePrefabXTexture_hover;

        window.prefabSelectCheckStyle.normal.background = selectPrefabCheckTexture_normal;
        window.prefabSelectCheckStyle.hover.background = selectPrefabCheckTexture_hover;

        window.prefabAddButtonStyle.margin.top = 0;
        window.prefabAddButtonStyle.margin.bottom = 0;
        window.prefabAddButtonStyle.margin.left = 4;
        window.prefabAddButtonStyle.margin.right = 0;
        window.prefabAddButtonStyle.fixedHeight = 60;
        window.prefabAddButtonStyle.normal.background = addPrefabTexture;


        window.prefabFieldStyle.padding.left = 1;
        window.prefabFieldStyle.padding.right = 1;
        window.prefabFieldStyle.margin.top = 0;
        window.prefabFieldStyle.margin.left = 7;
        window.prefabFieldStyle.margin.right = 2;

        window.prefabFieldStyle.fixedHeight = 60;
        window.prefabFieldStyle.fixedWidth = 72;
        window.prefabFieldStyle.normal.background = prefabFieldBackgroundTexture;

        window.floatFieldCompressedStyle.fixedWidth = 50;
        window.floatFieldCompressedStyle.stretchWidth = false;

        window.picButtonStyle.padding.left = 0;
        window.picButtonStyle.padding.top = 0;
        window.picButtonStyle.padding.bottom = 0;
        window.picButtonStyle.padding.right = 0;
        window.picButtonStyle.margin.left = 4;
        window.menuBlockStyle.margin.right = 0;

        window.tipLabelStyle.fontSize = 9;
        window.tipLabelStyle.padding.top = 0;
    }

    private void BuildStyles()
    {
        window.masterVerticalStyle = new GUIStyle(EditorStyles.label);
        window.prefabAmountSliderStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).verticalSlider);
        window.prefabAmountSliderThumbStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).verticalSliderThumb);
        window.toggleButtonStyle = new GUIStyle(EditorStyles.radioButton);
        window.floatFieldCompressedStyle = new GUIStyle(EditorStyles.textField);
        window.prefabPreviewWindowStyle = new GUIStyle(EditorStyles.label);
        window.prefabRemoveXStyle = new GUIStyle(EditorStyles.label);
        window.prefabSelectCheckStyle = new GUIStyle(EditorStyles.label);
        window.prefabAddButtonStyle = new GUIStyle(EditorStyles.miniButton);
        window.prefabFieldStyle = new GUIStyle(EditorStyles.label);
        window.picButtonStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).label);
        window.menuBlockStyle = new GUIStyle(EditorStyles.textField);
        window.tipLabelStyle = new GUIStyle(EditorStyles.label);

        SetStyleParameters();
    }

    static void SaveSettings()
    {

        #region Brush Settings Vars
        EditorPrefs.SetFloat("qb_BrushRadius", window.brushRadius);

        EditorPrefs.SetFloat("qb_BrushRadiusMin", window.brushRadiusMin);

        EditorPrefs.SetFloat("qb_BrushRadiusMax", window.brushRadiusMax);

        EditorPrefs.SetFloat("qb_BrushSpacing", window.brushSpacing);

        EditorPrefs.SetFloat("qb_BrushSpacingMax", window.brushSpacingMin);

        EditorPrefs.SetFloat("qb_BrushSpacingMax", window.brushSpacingMax);

        EditorPrefs.SetFloat("qb_ScatterRadius", window.scatterRadius);
        #endregion

        #region Rotation Settings Vars
        //Alignment	
        EditorPrefs.SetBool("qb_AlignToNormal", window.alignToNormal);

        EditorPrefs.SetBool("qb_AlignToStroke", window.alignToStroke);
        //Offset
        EditorPrefs.SetFloat("qb_RotationRangeX", window.rotationRange.x);
        EditorPrefs.SetFloat("qb_RotationRangeY", window.rotationRange.y);
        EditorPrefs.SetFloat("qb_RotationRangeZ", window.rotationRange.z);
        #endregion

        #region Scale Settings Vars
        //The minimum and maximum possible scale
        EditorPrefs.SetFloat("qb_ScaleMin", window.scaleMin);

        EditorPrefs.SetFloat("qb_ScaleMax", window.scaleMax);

        //The minimum and maximum current scale range setting
        EditorPrefs.SetFloat("qb_ScaleRandMinX", window.scaleRandMin.x);
        EditorPrefs.SetFloat("qb_ScaleRandMinY", window.scaleRandMin.y);
        EditorPrefs.SetFloat("qb_ScaleRandMinZ", window.scaleRandMin.z);

        EditorPrefs.SetFloat("qb_ScaleRandMaxX", window.scaleRandMax.x);
        EditorPrefs.SetFloat("qb_ScaleRandMaxY", window.scaleRandMax.y);
        EditorPrefs.SetFloat("qb_ScaleRandMaxZ", window.scaleRandMax.z);

        EditorPrefs.SetFloat("qb_ScaleRandMinUniform", window.scaleMin);

        EditorPrefs.SetFloat("qb_ScaleRandMaxUniform", window.scaleMax);

        EditorPrefs.SetBool("qb_ScaleUniform", window.scaleUniform);
        #endregion

        #region Sorting Vars
        //Selection
        EditorPrefs.SetBool("qb_PaintToSelection", window.paintToSelection);

        //Layers
        EditorPrefs.SetBool("qb_PaintToSelection", window.paintToLayer);

        EditorPrefs.SetInt("qb_LayerIndex", window.layerIndex);
        #endregion

        string prefabGroupString = string.Empty;

        for (int i = 0; i < window.prefabGroup.Length; i++)
        {
            prefabGroupString += "/" + AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(window.prefabGroup[i].prefab));
        }

        EditorPrefs.SetString("qb_PrefabGUIDList", prefabGroupString);

    }

    static void LoadSettings()
    {

        #region Brush Settings Vars
        window.brushRadius = EditorPrefs.GetFloat("qb_BrushRadius", window.brushRadius);

        window.brushRadiusMin = EditorPrefs.GetFloat("qb_BrushRadiusMin", window.brushRadiusMin);

        window.brushRadiusMax = EditorPrefs.GetFloat("qb_BrushRadiusMax", window.brushRadiusMax);

        window.brushSpacing = EditorPrefs.GetFloat("qb_BrushSpacing", window.brushSpacing);

        window.brushSpacingMax = EditorPrefs.GetFloat("qb_BrushSpacingMax", window.brushSpacingMax);

        window.brushSpacingMin = EditorPrefs.GetFloat("qb_BrushSpacingMin", window.brushSpacingMin);

        window.scatterRadius = EditorPrefs.GetFloat("qb_ScatterRadius", window.scatterRadius);
        #endregion

        #region Rotation Settings Vars
        //Alignment	
        window.alignToNormal = EditorPrefs.GetBool("qb_AlignToNormal", window.alignToNormal);

        window.alignToStroke = EditorPrefs.GetBool("qb_AlignToStroke", window.alignToStroke);

        //Offset
        float rotationRangeX = EditorPrefs.GetFloat("qb_RotationRangeX", window.rotationRange.x);
        float rotationRangeY = EditorPrefs.GetFloat("qb_RotationRangeY", window.rotationRange.y);
        float rotationRangeZ = EditorPrefs.GetFloat("qb_RotationRangeZ", window.rotationRange.z);
        window.rotationRange = new Vector3(rotationRangeX, rotationRangeY, rotationRangeZ);
        #endregion

        #region Scale Settings Vars
        //The minimum and maximum possible scale
        window.scaleMin = EditorPrefs.GetFloat("qb_ScaleMin", window.scaleMin);

        window.scaleMax = EditorPrefs.GetFloat("qb_ScaleMax", window.scaleMax);

        //The minimum and maximum current scale range setting
        float scaleRandMinX = EditorPrefs.GetFloat("qb_ScaleRandMinX", window.scaleRandMin.x);
        float scaleRandMinY = EditorPrefs.GetFloat("qb_ScaleRandMinY", window.scaleRandMin.y);
        float scaleRandMinZ = EditorPrefs.GetFloat("qb_ScaleRandMinZ", window.scaleRandMin.z);
        window.scaleRandMin = new Vector3(scaleRandMinX, scaleRandMinY, scaleRandMinZ);

        float scaleRandMaxX = EditorPrefs.GetFloat("qb_ScaleRandMaxX", window.scaleRandMax.x);
        float scaleRandMaxY = EditorPrefs.GetFloat("qb_ScaleRandMaxY", window.scaleRandMax.y);
        float scaleRandMaxZ = EditorPrefs.GetFloat("qb_ScaleRandMaxZ", window.scaleRandMax.z);
        window.scaleRandMax = new Vector3(scaleRandMaxX, scaleRandMaxY, scaleRandMaxZ);

        window.scaleRandMaxUniform = EditorPrefs.GetFloat("qb_ScaleRandMaxUniform", window.scaleRandMaxUniform);

        window.scaleRandMinUniform = EditorPrefs.GetFloat("qb_ScaleRandMinUniform", window.scaleRandMinUniform);

        window.scaleUniform = EditorPrefs.GetBool("qb_ScaleUniform", window.scaleUniform);
        #endregion

        #region Sorting Vars
        //Selection
        window.paintToSelection = EditorPrefs.GetBool("qb_PaintToSelection", window.paintToSelection);
        //Layers
        window.paintToLayer = EditorPrefs.GetBool("qb_PaintToLayer", window.paintToLayer);

        window.layerIndex = EditorPrefs.GetInt("qb_LayerIndex", window.layerIndex);
        #endregion

        #region Repopulate the Prefab List
        string retreivedPrefabGroupString = EditorPrefs.GetString("qb_PrefabGUIDList", string.Empty);

        string[] prefabStringList = new string[0];
        List<UnityEngine.Object> newPrefabs = new List<UnityEngine.Object>();

        if (retreivedPrefabGroupString != string.Empty)
        {
            //first clear out any items that are in the prefab list now
            window.prefabGroup = new qb_PrefabObject[0];
            //then retreive and split the saved prefab guids into a list
            prefabStringList = retreivedPrefabGroupString.Split('/');//string;
        }

        foreach (string GUIDstring in prefabStringList)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(GUIDstring);
            Object item = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));

            if (item != null)
                newPrefabs.Add(item);
        }

        if (newPrefabs.Count > 0)
        {
            foreach (UnityEngine.Object newPrefab in newPrefabs)
            {
                ArrayUtility.Add(ref window.prefabGroup, new qb_PrefabObject(newPrefab, 1f));
            }
        }
        #endregion
    }

    static void LoadDefaultSettings()
    {
        #region Brush Settings Vars
        window.brushRadius = 0.5f;

        window.brushRadiusMin = 0.2f;

        window.brushRadiusMax = 5f;

        window.brushSpacing = 0.2f;

        window.brushSpacingMax = 2f;

        window.brushSpacingMin = 0.02f;

        window.scatterRadius = 0f;
        #endregion

        #region Rotation Settings Vars
        //Alignment	
        window.alignToNormal = true;

        window.alignToStroke = true;

        //Offset
        window.rotationRange = new Vector3(0f, 0f, 0f);
        #endregion

        #region Scale Settings Vars
        //The minimum and maximum possible scale
        window.scaleMin = 0.1f;

        window.scaleMax = 3f;

        //The minimum and maximum current scale range setting
        window.scaleRandMin = new Vector3(1f, 1f, 1f);

        window.scaleRandMax = new Vector3(1f, 1f, 1f);

        window.scaleRandMaxUniform = 1f;

        window.scaleRandMinUniform = 1f;

        window.scaleUniform = true;
        #endregion

        #region Sorting Vars
        //Selection
        window.paintToSelection = false;
        //Layers
        window.paintToLayer = false;

        window.layerIndex = 0;
        #endregion	}

        window.prefabGroup = new qb_PrefabObject[0];

    }

    #region Temp Erasing
    public GameObject[] sceneObjects = new GameObject[0];
    public void EraseObjects(List<int> indexList)
    {

        List<GameObject> removalList = new List<GameObject>();

        foreach (int index in indexList)
        {
            removalList.Add(sceneObjects[index]);
        }

        foreach (GameObject obj in removalList)
        {
            ArrayUtility.Remove(ref sceneObjects, obj);
            EraseObject(obj);
        }

    }

    public void EraseObject(GameObject obj)
    {

#if UNITY_4_3
        Undo.DestroyObjectImmediate(obj);
#else
			Undo.RegisterSceneUndo("Erased Object");
			//Undo.RegisterUndo(obj, "Erased Object");
			GameObject.DestroyImmediate(obj);
#endif

    }

    public void VerifyObjects()
    {
        qb_Object[] objs = Object.FindObjectsOfType(typeof(qb_Object)) as qb_Object[];
        sceneObjects = new GameObject[objs.Length];

        for (int i = 0; i < sceneObjects.Length; i++)
        {
            sceneObjects[i] = objs[i].gameObject;
        }
    }

    public GameObject[] GetObjects()
    {
        VerifyObjects();
        return sceneObjects;
    }
    #endregion

}