using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UIElements;

namespace MEET_AND_TALK
{
    [CreateAssetMenu(menuName = "Dialogue/New Dialogue")]
    [System.Serializable]
    public class DialogueContainerSO : ScriptableObject
    {
        public List<NodeLinkData> NodeLinkDatas = new List<NodeLinkData>();

        public List<DialogueChoiceNodeData> DialogueChoiceNodeDatas = new List<DialogueChoiceNodeData>();
        public List<DialogueNodeData> DialogueNodeDatas = new List<DialogueNodeData>();
        public List<TimerChoiceNodeData> TimerChoiceNodeDatas = new List<TimerChoiceNodeData>();
        public List<EndNodeData> EndNodeDatas = new List<EndNodeData>();
        public List<EventNodeData> EventNodeDatas = new List<EventNodeData>();
        public List<StartNodeData> StartNodeDatas = new List<StartNodeData>();
        public List<RandomNodeData> RandomNodeDatas = new List<RandomNodeData>();
        public List<CommandNodeData> CommandNodeDatas = new List<CommandNodeData>();

        public List<BaseNodeData> AllNodes
        {
            get
            {
                List<BaseNodeData> tmp = new List<BaseNodeData>();
                tmp.AddRange(DialogueNodeDatas);
                tmp.AddRange(DialogueChoiceNodeDatas);
                tmp.AddRange(TimerChoiceNodeDatas);
                tmp.AddRange(EndNodeDatas);
                tmp.AddRange(EventNodeDatas);
                tmp.AddRange(StartNodeDatas);
                tmp.AddRange(RandomNodeDatas);
                tmp.AddRange(CommandNodeDatas);

                return tmp;
            }
        }
    }
    [System.Serializable]
    public class NodeLinkData
    {
        public string BaseNodeGuid;
        public string TargetNodeGuid;
    }

    [System.Serializable]
    public class BaseNodeData
    {
        public string NodeGuid;
        public Vector2 Position;
    }

    [System.Serializable]
    public class DialogueChoiceNodeData : BaseNodeData
    {
        public List<DialogueNodePort> DialogueNodePorts;
        public List<LanguageGeneric<AudioClip>> AudioClips;
        public DialogueCharacterSO Character;
        public List<LanguageGeneric<string>> TextType;
        public float Duration;
    }

    [System.Serializable]
    public class TimerChoiceNodeData : BaseNodeData
    {
        public List<DialogueNodePort> DialogueNodePorts;
        public List<LanguageGeneric<AudioClip>> AudioClips;
        public DialogueCharacterSO Character;
        public List<LanguageGeneric<string>> TextType;
        public float Duration;
        public float time;
    }

    [System.Serializable]
    public class RandomNodeData : BaseNodeData
    {
        public List<DialogueNodePort> DialogueNodePorts;
    }

    [System.Serializable]
    public class DialogueNodeData : BaseNodeData
    {
        public List<DialogueNodePort> DialogueNodePorts;
        public List<LanguageGeneric<AudioClip>> AudioClips;
        public DialogueCharacterSO Character;
        public List<LanguageGeneric<string>> TextType;
        public float Duration;
    }

    [System.Serializable]
    public class EndNodeData : BaseNodeData
    {
        public EndNodeType EndNodeType;
    }

    [System.Serializable]
    public class StartNodeData : BaseNodeData
    {
        public string startID;
    }


    [System.Serializable]
    public class EventNodeData : BaseNodeData
    {
        public List<EventScriptableObjectData> EventScriptableObjects;
    }
    [System.Serializable]
    public class EventScriptableObjectData
    {
        public DialogueEventSO DialogueEventSO;
    }

    [System.Serializable]
    public class CommandNodeData : BaseNodeData
    {
        public string commmand;
    }


    [System.Serializable]
    public class LanguageGeneric<T>
    {
        public LocalizationEnum languageEnum;
        public T LanguageGenericType;
    }

    [System.Serializable]
    public class DialogueNodePort
    {
        public string InputGuid;
        public string OutputGuid;
#if UNITY_EDITOR
        [HideInInspector] public Port MyPort;
#endif
        public TextField TextField;
        public List<LanguageGeneric<string>> TextLanguage = new List<LanguageGeneric<string>>();
    }

    [System.Serializable]
    public enum EndNodeType
    {
        End,
        Repeat,
        GoBack,
        ReturnToStart
    }


#if UNITY_EDITOR

    /* --------------------- */
    // Custom Property Draw
    /* --------------------- */


    [CustomEditor(typeof(DialogueContainerSO))]
    public class DialogueContainerSOEditor : Editor
    {
        bool NodeLink = false;
        bool StartNode = false;
        bool EndNode = false;
        bool DialogueNode = false;
        bool DialogueChoiceNode = false;
        bool DialogueTimerChoiceNode = false;
        bool DialogueEventNode = false;
        bool RandomNode = false;
        bool CommandNode = false;

        public override void OnInspectorGUI()
        {
            EditorUtility.SetDirty(target);
            DialogueContainerSO _target = (DialogueContainerSO)target;

            // Base Info
            EditorGUI.indentLevel = 0;
            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
            EditorGUILayout.LabelField(_target.name, EditorStyles.boldLabel);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();


            EditorGUILayout.LabelField("Node Type: ", EditorStyles.boldLabel);

            // Node List
            NodeLink = EditorGUILayout.BeginFoldoutHeaderGroup(NodeLink, $"Node Link - [{_target.NodeLinkDatas.Count}]", EditorStyles.foldoutHeader);
            if (NodeLink)
            {
                for (int i = 0; i < _target.NodeLinkDatas.Count; i++)
                {
                    EditorGUILayout.BeginVertical("Box");

                    EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                    EditorGUILayout.LabelField("Node Link Between", EditorStyles.boldLabel);
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.TextField("Base Node", _target.NodeLinkDatas[i].BaseNodeGuid);
                    EditorGUILayout.TextField("Target Node", _target.NodeLinkDatas[i].TargetNodeGuid);

                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            StartNode = EditorGUILayout.BeginFoldoutHeaderGroup(StartNode, $"Start Node - [{_target.StartNodeDatas.Count}]", EditorStyles.foldoutHeader);
            if (StartNode)
            {
                for (int i = 0; i < _target.StartNodeDatas.Count; i++)
                {
                    EditorGUILayout.BeginVertical("Box");

                    EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                    EditorGUILayout.LabelField("ID: " + _target.StartNodeDatas[i].NodeGuid, EditorStyles.boldLabel);
                    EditorGUILayout.EndVertical();

                    _target.StartNodeDatas[i].Position = EditorGUILayout.Vector2Field("Position", _target.StartNodeDatas[i].Position);
                    _target.StartNodeDatas[i].startID = EditorGUILayout.TextField("ID:",_target.StartNodeDatas[i].startID);
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EndNode = EditorGUILayout.BeginFoldoutHeaderGroup(EndNode, $"End Node - [{_target.EndNodeDatas.Count}]", EditorStyles.foldoutHeader);
            if (EndNode)
            {
                for (int i = 0; i < _target.EndNodeDatas.Count; i++)
                {
                    EditorGUILayout.BeginVertical("Box");

                    EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                    EditorGUILayout.LabelField("ID: " + _target.EndNodeDatas[i].NodeGuid, EditorStyles.boldLabel);
                    EditorGUILayout.EndVertical();

                    _target.EndNodeDatas[i].Position = EditorGUILayout.Vector2Field("Position", _target.EndNodeDatas[i].Position);
                    _target.EndNodeDatas[i].EndNodeType = (EndNodeType)EditorGUILayout.EnumPopup("End Enum", _target.EndNodeDatas[i].EndNodeType);
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            DialogueNode = EditorGUILayout.BeginFoldoutHeaderGroup(DialogueNode, $"Dialogue Node - [{_target.DialogueNodeDatas.Count}]", EditorStyles.foldoutHeader);
            if (DialogueNode)
            {
                for (int i = 0; i < _target.DialogueNodeDatas.Count; i++)
                {
                    EditorGUILayout.BeginVertical("Box");

                    EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                    EditorGUILayout.LabelField("ID: " + _target.DialogueNodeDatas[i].NodeGuid, EditorStyles.boldLabel);
                    EditorGUILayout.EndVertical();

                    _target.DialogueNodeDatas[i].Position = EditorGUILayout.Vector2Field("Position", _target.DialogueNodeDatas[i].Position);
                    _target.DialogueNodeDatas[i].Character = (DialogueCharacterSO)EditorGUILayout.ObjectField("Character", _target.DialogueNodeDatas[i].Character, typeof(DialogueCharacterSO), false);
                    _target.DialogueNodeDatas[i].Duration = EditorGUILayout.FloatField("Display Time", _target.DialogueNodeDatas[i].Duration);

                    for (int j = 0; j < _target.DialogueNodeDatas[0].TextType.Count; j++)
                    {
                        EditorGUILayout.BeginVertical("Box");
                        EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                        EditorGUILayout.LabelField(_target.DialogueNodeDatas[0].TextType[j].languageEnum.ToString(), EditorStyles.boldLabel);
                        EditorGUILayout.EndVertical();
                        _target.DialogueNodeDatas[i].AudioClips[j].LanguageGenericType = (AudioClip)EditorGUILayout.ObjectField("Audio Clips", _target.DialogueNodeDatas[i].AudioClips[j].LanguageGenericType, typeof(AudioClip), false);
                        _target.DialogueNodeDatas[i].TextType[j].LanguageGenericType = EditorGUILayout.TextField("Displayed String", _target.DialogueNodeDatas[i].TextType[j].LanguageGenericType);
                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Separator();
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            DialogueChoiceNode = EditorGUILayout.BeginFoldoutHeaderGroup(DialogueChoiceNode, $"Dialogue Choice Node - [{_target.DialogueChoiceNodeDatas.Count}]", EditorStyles.foldoutHeader);
            if (DialogueChoiceNode)
            {
                for (int i = 0; i < _target.DialogueChoiceNodeDatas.Count; i++)
                {
                    EditorGUILayout.BeginVertical("Box");

                    EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                    EditorGUILayout.LabelField("ID: " + _target.DialogueChoiceNodeDatas[i].NodeGuid, EditorStyles.boldLabel);
                    EditorGUILayout.EndVertical();

                    _target.DialogueChoiceNodeDatas[i].Position = EditorGUILayout.Vector2Field("Position", _target.DialogueChoiceNodeDatas[i].Position);
                    _target.DialogueChoiceNodeDatas[i].Character = (DialogueCharacterSO)EditorGUILayout.ObjectField("Character", _target.DialogueChoiceNodeDatas[i].Character, typeof(DialogueCharacterSO), false);
                    _target.DialogueChoiceNodeDatas[i].Duration = EditorGUILayout.FloatField("Display Time", _target.DialogueChoiceNodeDatas[i].Duration);

                    for (int j = 0; j < _target.DialogueChoiceNodeDatas[0].TextType.Count; j++)
                    {
                        EditorGUILayout.BeginVertical("Box");
                        EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                        EditorGUILayout.LabelField(_target.DialogueChoiceNodeDatas[0].TextType[j].languageEnum.ToString(), EditorStyles.boldLabel);
                        EditorGUILayout.EndVertical();
                        _target.DialogueChoiceNodeDatas[i].AudioClips[j].LanguageGenericType = (AudioClip)EditorGUILayout.ObjectField("Audio Clips", _target.DialogueChoiceNodeDatas[i].AudioClips[j].LanguageGenericType, typeof(AudioClip), false);
                        _target.DialogueChoiceNodeDatas[i].TextType[j].LanguageGenericType = EditorGUILayout.TextField("Displayed String", _target.DialogueChoiceNodeDatas[i].TextType[j].LanguageGenericType);
                        EditorGUILayout.LabelField("Options: ", EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;
                        for (int x = 0; x < _target.DialogueChoiceNodeDatas[i].DialogueNodePorts.Count; x++)
                        {
                            _target.DialogueChoiceNodeDatas[i].DialogueNodePorts[x].TextLanguage[j].LanguageGenericType = EditorGUILayout.TextField($"Option_{x + 1}", _target.DialogueChoiceNodeDatas[i].DialogueNodePorts[x].TextLanguage[j].LanguageGenericType);
                        }
                        EditorGUI.indentLevel--;
                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Separator();
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            DialogueTimerChoiceNode = EditorGUILayout.BeginFoldoutHeaderGroup(DialogueTimerChoiceNode, $"Dialogue Time Choice Node - [{_target.TimerChoiceNodeDatas.Count}]", EditorStyles.foldoutHeader);
            if (DialogueTimerChoiceNode)
            {
                for (int i = 0; i < _target.TimerChoiceNodeDatas.Count; i++)
                {
                    EditorGUILayout.BeginVertical("Box");

                    EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                    EditorGUILayout.LabelField("ID: " + _target.TimerChoiceNodeDatas[i].NodeGuid, EditorStyles.boldLabel);
                    EditorGUILayout.EndVertical();

                    _target.TimerChoiceNodeDatas[i].Position = EditorGUILayout.Vector2Field("Position", _target.TimerChoiceNodeDatas[i].Position);
                    _target.TimerChoiceNodeDatas[i].Character = (DialogueCharacterSO)EditorGUILayout.ObjectField("Character", _target.TimerChoiceNodeDatas[i].Character, typeof(DialogueCharacterSO), false);
                    _target.TimerChoiceNodeDatas[i].Duration = EditorGUILayout.FloatField("Display Time", _target.TimerChoiceNodeDatas[i].Duration);
                    _target.TimerChoiceNodeDatas[i].time = EditorGUILayout.FloatField("Time to make decision", _target.TimerChoiceNodeDatas[i].time);

                    for (int j = 0; j < _target.TimerChoiceNodeDatas[0].TextType.Count; j++)
                    {
                        EditorGUILayout.BeginVertical("Box");
                        EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                        EditorGUILayout.LabelField(_target.TimerChoiceNodeDatas[0].TextType[j].languageEnum.ToString(), EditorStyles.boldLabel);
                        EditorGUILayout.EndVertical();
                        _target.TimerChoiceNodeDatas[i].AudioClips[j].LanguageGenericType = (AudioClip)EditorGUILayout.ObjectField("Audio Clips", _target.TimerChoiceNodeDatas[i].AudioClips[j].LanguageGenericType, typeof(AudioClip), false);
                        _target.TimerChoiceNodeDatas[i].TextType[j].LanguageGenericType = EditorGUILayout.TextField("Displayed String", _target.TimerChoiceNodeDatas[i].TextType[j].LanguageGenericType);
                        EditorGUILayout.LabelField("Options: ", EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;
                        for (int x = 1; x < _target.TimerChoiceNodeDatas[i].DialogueNodePorts.Count; x++)
                        {
                            _target.TimerChoiceNodeDatas[i].DialogueNodePorts[x].TextLanguage[j].LanguageGenericType = EditorGUILayout.TextField($"Option_{x}", _target.TimerChoiceNodeDatas[i].DialogueNodePorts[x].TextLanguage[j].LanguageGenericType);
                        }
                        EditorGUI.indentLevel--;
                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Separator();
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            DialogueEventNode = EditorGUILayout.BeginFoldoutHeaderGroup(DialogueEventNode, $"Event Node - [{_target.EventNodeDatas.Count}]", EditorStyles.foldoutHeader);
            if (DialogueEventNode)
            {
                for (int i = 0; i < _target.EventNodeDatas.Count; i++)
                {
                    EditorGUILayout.BeginVertical("Box");

                    EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                    EditorGUILayout.LabelField("ID: " + _target.EventNodeDatas[i].NodeGuid, EditorStyles.boldLabel);
                    EditorGUILayout.EndVertical();

                    _target.EventNodeDatas[i].Position = EditorGUILayout.Vector2Field("Position", _target.EventNodeDatas[i].Position);
                    EditorGUILayout.LabelField("Events: ", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    for (int x = 0; x < _target.EventNodeDatas[i].EventScriptableObjects.Count; x++)
                    {
                        _target.EventNodeDatas[i].EventScriptableObjects[x].DialogueEventSO = (DialogueEventSO)EditorGUILayout.ObjectField($"Event_{x}", _target.EventNodeDatas[i].EventScriptableObjects[x].DialogueEventSO, typeof(DialogueEventSO), false);
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            RandomNode = EditorGUILayout.BeginFoldoutHeaderGroup(RandomNode, $"Random Node - [{_target.RandomNodeDatas.Count}]", EditorStyles.foldoutHeader);
            if (RandomNode)
            {
                for (int i = 0; i < _target.RandomNodeDatas.Count; i++)
                {
                    EditorGUILayout.BeginVertical("Box");

                    EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
                    EditorGUILayout.LabelField("ID: " + _target.RandomNodeDatas[i].NodeGuid, EditorStyles.boldLabel);
                    EditorGUILayout.EndVertical();

                    _target.RandomNodeDatas[i].Position = EditorGUILayout.Vector2Field("Position", _target.RandomNodeDatas[i].Position);

                    EditorGUILayout.LabelField("Ports: ", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    for (int x = 0; x < _target.RandomNodeDatas[i].DialogueNodePorts.Count; x++)
                    {
                        _target.RandomNodeDatas[i].DialogueNodePorts[x].InputGuid = EditorGUILayout.TextField($"Port {x + 1}", _target.RandomNodeDatas[i].DialogueNodePorts[x].InputGuid);
                    }
                    EditorGUI.indentLevel--;


                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Separator();
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();


            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}