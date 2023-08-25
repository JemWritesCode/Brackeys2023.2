#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

using UnityEditor;
using UnityEditor.Callbacks;

namespace MEET_AND_TALK
{
    [ExecuteInEditMode]
    public class DialogueEditorWindow : EditorWindow
    {
        private DialogueContainerSO currentDialogueContainer;
        private DialogueGraphView graphView;
        private DialogueSaveAndLoad saveAndLoad;

        private LocalizationEnum languageEnum = LocalizationEnum.English;
        private Label nameOfDialogueContainer;
        private ToolbarMenu toolbarMenu;
        private ToolbarMenu toolbarTheme;

        public LocalizationEnum LanguageEnum { get => languageEnum; set => languageEnum = value; }

        [OnOpenAsset(1)]
        public static bool ShowWindow(int _instanceId, int line)
        {
            UnityEngine.Object item = EditorUtility.InstanceIDToObject(_instanceId);

            if (item is DialogueContainerSO)
            {
                DialogueEditorWindow window = (DialogueEditorWindow)GetWindow(typeof(DialogueEditorWindow));
                window.titleContent = new GUIContent("Dialogue Editor", EditorGUIUtility.FindTexture("d_Favorite Icon"));
                window.currentDialogueContainer = item as DialogueContainerSO;
                window.minSize = new Vector2(500, 250);
                window.Load();
            }

            return false;
        }

        private void OnEnable()
        {
            ContructGraphView();
            GenerateToolbar();
            Load();
        }
        private void OnDisable()
        {
            rootVisualElement.Remove(graphView);
        }

        private void ContructGraphView()
        {
            graphView = new DialogueGraphView(this);
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);

            saveAndLoad = new DialogueSaveAndLoad(graphView);
        }
        private void GenerateToolbar()
        {
            StyleSheet styleSheet = Resources.Load<StyleSheet>("Themes/DarkTheme");
            rootVisualElement.styleSheets.Add(styleSheet);

            Toolbar toolbar = new Toolbar();

            ToolbarButton saveBtn = new ToolbarButton()
            {
                text = "Save"
            };
            saveBtn.clicked += () =>
            {
                Save();
            };
            toolbar.Add(saveBtn);

            ToolbarSpacer sep_1 = new ToolbarSpacer();
            toolbar.Add(sep_1);

            ToolbarButton loadBtn = new ToolbarButton()
            {
                text = "Load"
            };
            loadBtn.clicked += () =>
            {
                bool confirmed = EditorUtility.DisplayDialog("Load the Dialogue Save?", "Are you sure you want to load the dialogue saving?\nThis will delete all unsaved dialogue changes", "Confirm", "Cancel");

                if (confirmed)
                {
                    Load();
                }
            };
            toolbar.Add(loadBtn);

            ToolbarSpacer sep_2  = new ToolbarSpacer();
            toolbar.Add(sep_2);

            toolbarMenu = new ToolbarMenu();
            foreach (LocalizationEnum language in (LocalizationEnum[])Enum.GetValues(typeof(LocalizationEnum)))
            {
                toolbarMenu.menu.AppendAction(language.ToString(), new Action<DropdownMenuAction>(x => Language(language, toolbarMenu)));
            }
            toolbar.Add(toolbarMenu);

            toolbarTheme = new ToolbarMenu();
            foreach (MeetAndTalkTheme theme in (MeetAndTalkTheme[])Enum.GetValues(typeof(MeetAndTalkTheme)))
            {
                toolbarTheme.menu.AppendAction(theme.ToString(), new Action<DropdownMenuAction>(x => ChangeTheme(theme, toolbarTheme)));
            }
            toolbar.Add(toolbarTheme);



            nameOfDialogueContainer = new Label("");
            toolbar.Add(nameOfDialogueContainer);
            nameOfDialogueContainer.AddToClassList("nameOfDialogueContainer");

            rootVisualElement.Add(toolbar);
        }
        private void Load()
        {
            if (currentDialogueContainer != null)
            {
                Language(LocalizationEnum.English, toolbarMenu);
                ChangeTheme(Resources.Load<MeetAndTalkSettings>("MeetAndTalkSettings").Theme, toolbarTheme);
                nameOfDialogueContainer.text = "" + currentDialogueContainer.name;
                saveAndLoad.Load(currentDialogueContainer);
            }
        }
        private void Save()
        {
            if (currentDialogueContainer != null)
            {
                saveAndLoad.Save(currentDialogueContainer);
            }
            Debug.Log("Save");
        }
        private void Language(LocalizationEnum _language, ToolbarMenu _toolbarMenu)
        {
            toolbarMenu.text = "Language: " + _language.ToString() + "";
            languageEnum = _language;
            graphView.LanguageReload();
        }
        private void ChangeTheme(MeetAndTalkTheme _theme, ToolbarMenu _toolbarMenu)
        {
            toolbarTheme.text = "Theme: " + _theme.ToString() + "";
            Resources.Load<MeetAndTalkSettings>("MeetAndTalkSettings").Theme = _theme;

            rootVisualElement.styleSheets.Remove(rootVisualElement.styleSheets[rootVisualElement.styleSheets.count - 1]);
            rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>($"Themes/{_theme.ToString()}Theme"));

            graphView.UpdateTheme(_theme.ToString());
        }
    }
#endif
}