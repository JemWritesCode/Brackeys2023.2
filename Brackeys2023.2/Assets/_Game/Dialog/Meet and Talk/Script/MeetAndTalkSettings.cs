#define MEET_AND_TALK

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace MEET_AND_TALK
{
    public class MeetAndTalkSettings : ScriptableObject
    {
        public const string k_CampaingPath = "Assets/Meet and Talk/Resources/MeetAndTalkSettings.asset";

        [SerializeField] public GameObject DialoguePrefab;
        [SerializeField] public MeetAndTalkTheme Theme;

        private static MeetAndTalkSettings _instance;
        public static MeetAndTalkSettings Instance
        {
            get { return _instance; }
        }

#if UNITY_EDITOR
        internal static MeetAndTalkSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<MeetAndTalkSettings>(k_CampaingPath);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<MeetAndTalkSettings>();


                AssetDatabase.CreateAsset(settings, k_CampaingPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }
        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
#endif
    }

#if UNITY_EDITOR
    static class MeetAndTalkSettingsIMGUIRegister
    {
        [SettingsProvider]
        public static SettingsProvider CampaignManagerProvider()
        {
            var provider = new SettingsProvider("Project/Meet and Talk", SettingsScope.Project)
            {
                label = "Meet and Talk",
                guiHandler = (searchContext) =>
                {
                    EditorGUILayout.HelpBox("Coming Soon", MessageType.Info, true);
                    var settings = MeetAndTalkSettings.GetSerializedSettings();
                    EditorGUILayout.PropertyField(settings.FindProperty("Theme"), new GUIContent("Editor Theme"));
                    EditorGUILayout.PropertyField(settings.FindProperty("DialoguePrefab"), new GUIContent("Dialogue Prefab"));
                //EditorGUILayout.PropertyField(settings.FindProperty("NodeTheme"), new GUIContent("Localization String"));
            }
            };

            return provider;
        }
    }

    class MeetAndTalkSettingsProvider : SettingsProvider
    {
        private SerializedObject m_CustomSettings;

        class Styles
        {
            //public static GUIContent chapter = new GUIContent("chapter");
            //public static GUIContent localization = new GUIContent("localization");
            //public static GUIContent selectedLang = new GUIContent("selectedLang");
        }

        const string k_CampaingPath = "Assets/Meet and Talk/Resources/MeetAndTalkSettings.asset";
        public MeetAndTalkSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope) { }

        public static bool IsSettingsAvailable()
        {
            return File.Exists(k_CampaingPath);
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            m_CustomSettings = MeetAndTalkSettings.GetSerializedSettings();
        }

        public override void OnGUI(string searchContext)
        {
            //EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("chapter"), Styles.chapter);
            //EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("localization"), Styles.localization);
            //EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("selectedLang"), Styles.selectedLang);
        }
    }


#endif

    public enum MeetAndTalkTheme
    {
        Dark = 0, PureDark = 1,
        //Light = 2, PureLight = 3
    }
}