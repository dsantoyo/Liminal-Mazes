using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Flashlight))]
public class Flashlight_Editor : Editor
{
    #region Properties

    SerializedProperty reloadKey                    = null;
    SerializedProperty toggleKey                    = null;

    SerializedProperty maxBatteries                 = null;
    SerializedProperty batteries                    = null;

    SerializedProperty autoReduce                   = null;
    SerializedProperty reduceSpeed                  = null;

    SerializedProperty autoIncrease                 = null;
    SerializedProperty increaseSpeed                = null;

    SerializedProperty toggleOnWTPR                 = null;

    SerializedProperty maxBatteryLife               = null;

    SerializedProperty onSound                      = null;
    SerializedProperty offSound                     = null;
    SerializedProperty reloadSound                  = null;

    SerializedProperty stateIcon                    = null;
    SerializedProperty lifeSlider                   = null;
    SerializedProperty lifeSliderFill               = null;
    SerializedProperty reloadText                   = null;
    SerializedProperty countText                    = null;
    SerializedProperty holder                       = null;

    SerializedProperty fullLifeColor                = null;
    SerializedProperty deadLifeColor                = null;

    SerializedProperty camera                       = null;
    SerializedProperty flashlight                   = null;

    SerializedProperty batteryLife                  = null;
    SerializedProperty usingFlashlight              = null;
    SerializedProperty outOfBattery                 = null;

    SerializedProperty followSpeed                  = null;
    SerializedProperty offset                       = null;

    #endregion

    bool foldoutState_Statistics = false;
    [SerializeField]
    int tab = 0;
    GameObject targetObject;

    const string foldoutKey = "Flashlight_Editor_FoldoutState_Statistics";
    const string tabKey = "Flashlight_Editor_TabInteger";

    private void OnEnable()
    {
        reloadKey                   = serializedObject.FindProperty("reloadKey");
        toggleKey                   = serializedObject.FindProperty("toggleKey");

        maxBatteries                = serializedObject.FindProperty("maxBatteries");
        batteries                   = serializedObject.FindProperty("batteries");

        autoReduce                  = serializedObject.FindProperty("autoReduce");
        reduceSpeed                 = serializedObject.FindProperty("reduceSpeed");

        autoIncrease                = serializedObject.FindProperty("autoIncrease");
        increaseSpeed               = serializedObject.FindProperty("increaseSpeed");

        toggleOnWTPR                = serializedObject.FindProperty("toggleOnWaitTillPercentageReached");

        maxBatteryLife              = serializedObject.FindProperty("maxBatteryLife");

        onSound                     = serializedObject.FindProperty("onSound");
        offSound                    = serializedObject.FindProperty("offSound");
        reloadSound                 = serializedObject.FindProperty("reloadSound");

        stateIcon                   = serializedObject.FindProperty("stateIcon");
        lifeSlider                  = serializedObject.FindProperty("lifeSlider");
        lifeSliderFill              = serializedObject.FindProperty("lifeSliderFill");
        reloadText                  = serializedObject.FindProperty("reloadText");
        countText                   = serializedObject.FindProperty("countText");
        holder                      = serializedObject.FindProperty("holder");

        fullLifeColor               = serializedObject.FindProperty("fullLifeColor");
        deadLifeColor               = serializedObject.FindProperty("deadLifeColor");

        camera                      = serializedObject.FindProperty("camera");
        flashlight                  = serializedObject.FindProperty("flashlight");

        batteryLife                 = serializedObject.FindProperty("batteryLife");
        usingFlashlight             = serializedObject.FindProperty("usingFlashlight");
        outOfBattery                = serializedObject.FindProperty("outOfBattery");

        followSpeed                 = serializedObject.FindProperty("followSpeed");
        offset                      = serializedObject.FindProperty("offset");

        foldoutState_Statistics     = EditorPrefs.GetBool(foldoutKey);
        tab                         = EditorPrefs.GetInt(tabKey);
        targetObject                = GameObject.Find(serializedObject.targetObject.name);
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        DrawTabButton(new GUIContent("Settings", "Press to view Settings"), 0, EditorStyles.miniButtonLeft);
        DrawTabButton(new GUIContent("References", "Press to view References"), 1, EditorStyles.miniButtonRight);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(3.5f);

        switch (tab)
        {
            case 0:
                DrawSettings();
                break;
            case 1:
                DrawReferences();
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
    private void DrawSettings()
    {
        GUILayout.Label("Private", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        foldoutState_Statistics = EditorGUILayout.Foldout(foldoutState_Statistics, (foldoutState_Statistics ? "Hide " : "Show ") + "Statistics");
        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetBool(foldoutKey, foldoutState_Statistics);
        }

        if (foldoutState_Statistics)
        {
            EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.PropertyField(batteryLife);
            EditorGUILayout.PropertyField(usingFlashlight);
            EditorGUILayout.PropertyField(outOfBattery);

            EditorGUI.EndDisabledGroup();
        }

        EditorGUILayout.Space();

        GUILayout.Label("Keys", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(reloadKey);
        EditorGUILayout.PropertyField(toggleKey);

        EditorGUILayout.Space();

        GUILayout.Label("Parameters", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(maxBatteries);
        if (maxBatteries.intValue < 0)
        {
            maxBatteries.intValue = 0;
        }
        batteries.intValue = EditorGUILayout.IntSlider("Batteries", batteries.intValue, 0, maxBatteries.intValue);

        EditorGUILayout.Space();

        autoReduce.boolValue = EditorGUILayout.Toggle("Auto Reduce", autoReduce.boolValue, EditorStyles.radioButton);
        if (autoReduce.boolValue)
        {
            EditorGUILayout.PropertyField(reduceSpeed);
            if (reduceSpeed.floatValue < 0.0f) { reduceSpeed.floatValue = 0.0f; }
        }
        autoIncrease.boolValue = EditorGUILayout.Toggle("Auto Increase", autoIncrease.boolValue, EditorStyles.radioButton);
        if (autoIncrease.boolValue)
        {
            EditorGUILayout.PropertyField(increaseSpeed);
            if (increaseSpeed.floatValue < 0.0f) { increaseSpeed.floatValue = 0.0f; }
            EditorGUILayout.PropertyField(toggleOnWTPR, new GUIContent("ToggleOn Wait Percentage"));
        }

        EditorGUILayout.Space();
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.FloatField("Min Battery Life", Flashlight.minBatteryLife);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.PropertyField(maxBatteryLife);
        if (maxBatteryLife.floatValue <= 0) { maxBatteryLife.floatValue++; }

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(followSpeed);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(offset, true);
        if (EditorGUI.EndChangeCheck())
        {
            UpdateRotation();
        }
    }
    private void DrawReferences()
    {
        GUILayout.Label("Audio", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(onSound);
        EditorGUILayout.PropertyField(offSound);
        EditorGUILayout.PropertyField(reloadSound);

        GUILayout.Label("UI", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(holder);
        EditorGUILayout.PropertyField(stateIcon);
        EditorGUILayout.PropertyField(countText);
        EditorGUILayout.PropertyField(lifeSlider);
        EditorGUILayout.PropertyField(lifeSliderFill);
        EditorGUILayout.PropertyField(reloadText);

        GUILayout.Label("Color", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(fullLifeColor);
        EditorGUILayout.PropertyField(deadLifeColor);

        GUILayout.Label("Object", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(camera);
        if (EditorGUI.EndChangeCheck())
        {
            UpdateRotation();
        }
        EditorGUILayout.PropertyField(flashlight);
    }
    private void DrawTabButton(GUIContent label, int index, GUIStyle style)
    {
        Color defaultColor = GUI.color;
        Color pressedColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, defaultColor.a / 2);

        GUI.color = tab == index ? pressedColor : defaultColor;
        bool pressed = GUILayout.Button(label, style, GUILayout.Height(25));
        GUI.color = defaultColor;
        if (pressed)
        {
            tab = index;
            EditorPrefs.SetInt(tabKey, tab);
        }
    }

    private void UpdateRotation()
    {
        if (camera.objectReferenceValue)
        {
            targetObject.transform.localRotation = (camera.objectReferenceValue as Camera).transform.localRotation * offset.quaternionValue;
            if (flashlight.objectReferenceValue)
            {
                (flashlight.objectReferenceValue as GameObject).transform.rotation = targetObject.transform.rotation;
            }
        }
    }
}