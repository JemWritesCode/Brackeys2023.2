using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MEET_AND_TALK
{
    public class DialogueManager : DialogueGetData
    {
        [HideInInspector] public static DialogueManager Instance;
        public LocalizationManager localizationManager;

        [HideInInspector] public DialogueUIManager dialogueUIManager;
        public AudioSource audioSource;

        public UnityEvent StartDialogueEvent;
        public UnityEvent EndDialogueEvent;

        private BaseNodeData currentDialogueNodeData;
        private BaseNodeData lastDialogueNodeData;

        private TimerChoiceNodeData _nodeTimerInvoke;
        private DialogueNodeData _nodeDialogueInvoke;
        private DialogueChoiceNodeData _nodeChoiceInvoke;

        float Timer;

        private void Awake()
        {
            Instance = this;
            dialogueUIManager= DialogueUIManager.Instance;
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            Timer -= Time.deltaTime;
            if (Timer > 0) dialogueUIManager.TimerSlider.value = Timer;
        }

        public void SetupDialogue(DialogueContainerSO dialogue)
        {
            dialogueContainer = dialogue;
        }

        public void StartDialogue(DialogueContainerSO dialogue)
        {
            dialogueUIManager = DialogueUIManager.Instance;
            dialogueContainer = dialogue;

            if (dialogueContainer.StartNodeDatas.Count == 1) CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[0]));
            else { CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[Random.Range(0, dialogueContainer.StartNodeDatas.Count)])); }

            dialogueUIManager.dialogueCanvas .SetActive(true);
            StartDialogueEvent.Invoke();
        }

        public void StartDialogue(string ID)
        {
            dialogueUIManager = DialogueUIManager.Instance;

            // Try Get Start with ID
            bool withID = false;
            for(int i = 0; i < dialogueContainer.StartNodeDatas.Count; i++)
            {
                if(dialogueContainer.StartNodeDatas[i].startID == ID)
                {
                    CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[i]));
                    withID = true;
                }
            }
            if (!withID)
            {
                if (dialogueContainer.StartNodeDatas.Count == 1) CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[0]));
                else { CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[Random.Range(0, dialogueContainer.StartNodeDatas.Count)])); }
            }

            dialogueUIManager.dialogueCanvas.SetActive(true);
            StartDialogueEvent.Invoke();
        }

        public void StartDialogue()
        {
            dialogueUIManager= DialogueUIManager.Instance;

            if (dialogueContainer.StartNodeDatas.Count == 1) CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[0]));
            else { CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[Random.Range(0, dialogueContainer.StartNodeDatas.Count)])); }

            dialogueUIManager.dialogueCanvas.SetActive(true);
            StartDialogueEvent.Invoke();
        }

        public void CheckNodeType(BaseNodeData _baseNodeData)
        {
            switch (_baseNodeData)
            {
                case StartNodeData nodeData:
                    RunNode(nodeData);
                    break;
                case DialogueNodeData nodeData:
                    RunNode(nodeData);
                    break;
                case DialogueChoiceNodeData nodeData:
                    RunNode(nodeData);
                    break;
                case TimerChoiceNodeData nodeData:
                    RunNode(nodeData);
                    break;
                case EventNodeData nodeData:
                    RunNode(nodeData);
                    break;
                case EndNodeData nodeData:
                    RunNode(nodeData);
                    break;
                case RandomNodeData nodeData:
                    RunNode(nodeData);
                    break;
                default:
                    break;
            }
        }


        private void RunNode(StartNodeData _nodeData)
        {
            CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[0]));
        }
        private void RunNode(RandomNodeData _nodeData)
        {
            CheckNodeType(GetNodeByGuid(_nodeData.DialogueNodePorts[Random.Range(0, _nodeData.DialogueNodePorts.Count)].InputGuid));
        }
        private void RunNode(DialogueNodeData _nodeData)
        {
            lastDialogueNodeData = currentDialogueNodeData;
            currentDialogueNodeData = _nodeData;

            if (_nodeData.Character != null) dialogueUIManager.ResetText($"<color={_nodeData.Character.HexColor()}>{_nodeData.Character.characterName.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}: </color>");
            else dialogueUIManager.ResetText("");

            dialogueUIManager.fullText = $"{_nodeData.TextType.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}";

            MakeButtons(new List<DialogueNodePort>());

            if(_nodeData.AudioClips.Find(clip => clip.languageEnum == localizationManager.SelectedLang()).LanguageGenericType != null) audioSource.PlayOneShot(_nodeData.AudioClips.Find(clip => clip.languageEnum == localizationManager.SelectedLang()).LanguageGenericType);

            _nodeDialogueInvoke = _nodeData;

            IEnumerator tmp() { yield return new WaitForSeconds(_nodeData.Duration); DialogueNode_NextNode(); }
            if(_nodeData.Duration != 0) StartCoroutine(tmp());
        }
        private void RunNode(DialogueChoiceNodeData _nodeData)
        {
            lastDialogueNodeData = currentDialogueNodeData;
            currentDialogueNodeData = _nodeData;

            if (_nodeData.Character != null) dialogueUIManager.ResetText($"<color={_nodeData.Character.HexColor()}>{_nodeData.Character.characterName.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}: </color>");
            else dialogueUIManager.ResetText("");
            dialogueUIManager.fullText = $"{_nodeData.TextType.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}";

            MakeButtons(new List<DialogueNodePort>());

            _nodeChoiceInvoke = _nodeData;

            IEnumerator tmp() { yield return new WaitForSeconds(_nodeData.Duration); ChoiceNode_GenerateChoice(); }
            StartCoroutine(tmp());

            if (_nodeData.AudioClips.Find(clip => clip.languageEnum == localizationManager.SelectedLang()).LanguageGenericType != null) audioSource.PlayOneShot(_nodeData.AudioClips.Find(clip => clip.languageEnum == localizationManager.SelectedLang()).LanguageGenericType);
        }
        private void RunNode(EventNodeData _nodeData)
        {
            foreach (var item in _nodeData.EventScriptableObjects)
            {
                if (item.DialogueEventSO != null)
                {
                    item.DialogueEventSO.RunEvent();
                }
            }
            CheckNodeType(GetNextNode(_nodeData));
        }
        private void RunNode(EndNodeData _nodeData)
        {
            switch (_nodeData.EndNodeType)
            {
                case EndNodeType.End:
                    dialogueUIManager.dialogueCanvas.SetActive(false);
                    EndDialogueEvent.Invoke();
                    break;
                case EndNodeType.Repeat:
                    CheckNodeType(GetNodeByGuid(currentDialogueNodeData.NodeGuid));
                    break;
                case EndNodeType.GoBack:
                    CheckNodeType(GetNodeByGuid(lastDialogueNodeData.NodeGuid));
                    break;
                case EndNodeType.ReturnToStart:
                    CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[Random.Range(0,dialogueContainer.StartNodeDatas.Count)]));
                    break;
                default:
                    break;
            }
        }
        private void RunNode(TimerChoiceNodeData _nodeData)
        {
            lastDialogueNodeData = currentDialogueNodeData;
            currentDialogueNodeData = _nodeData;

            if (_nodeData.Character != null) dialogueUIManager.ResetText($"<color={_nodeData.Character.HexColor()}>{_nodeData.Character.characterName.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}: </color>");
            else dialogueUIManager.ResetText("");
            dialogueUIManager.fullText = $"{_nodeData.TextType.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType}";

            MakeButtons(new List<DialogueNodePort>());

            _nodeTimerInvoke = _nodeData;

            IEnumerator tmp() { yield return new WaitForSecondsRealtime(_nodeData.Duration); TimerNode_GenerateChoice(); }
            StartCoroutine(tmp());

            if (_nodeData.AudioClips.Find(clip => clip.languageEnum == localizationManager.SelectedLang()).LanguageGenericType != null) audioSource.PlayOneShot(_nodeData.AudioClips.Find(clip => clip.languageEnum == localizationManager.SelectedLang()).LanguageGenericType);

        }

        private void MakeButtons(List<DialogueNodePort> _nodePorts)
        {
            List<string> texts = new List<string>();
            List<UnityAction> unityActions = new List<UnityAction>();

            foreach (DialogueNodePort nodePort in _nodePorts)
            {
                texts.Add(nodePort.TextLanguage.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType);
                UnityAction tempAction = null;
                tempAction += () =>
                {
                    CheckNodeType(GetNodeByGuid(nodePort.InputGuid));
                };
                unityActions.Add(tempAction);
            }

            dialogueUIManager.SetButtons(texts, unityActions, false);
        }
        private void MakeTimerButtons(List<DialogueNodePort> _nodePorts, float ShowDuration, float timer)
        {
            List<string> texts = new List<string>();
            List<UnityAction> unityActions = new List<UnityAction>();

            IEnumerator tmp() { yield return new WaitForSeconds(timer); TimerNode_NextNode(); }
            StartCoroutine(tmp());

            foreach (DialogueNodePort nodePort in _nodePorts)
            {
                if (nodePort != _nodePorts[0])
                {
                    texts.Add(nodePort.TextLanguage.Find(text => text.languageEnum == localizationManager.SelectedLang()).LanguageGenericType);
                    UnityAction tempAction = null;
                    tempAction += () =>
                    {
                        StopAllCoroutines();
                        CheckNodeType(GetNodeByGuid(nodePort.InputGuid));
                    };
                    unityActions.Add(tempAction);
                }
            }

            dialogueUIManager.SetButtons(texts, unityActions, true);
            dialogueUIManager.TimerSlider.maxValue = timer; Timer = timer;
        }

        void DialogueNode_NextNode() { CheckNodeType(GetNextNode(_nodeDialogueInvoke)); }
        void ChoiceNode_GenerateChoice() { MakeButtons(_nodeChoiceInvoke.DialogueNodePorts); }
        void TimerNode_GenerateChoice() { MakeTimerButtons(_nodeTimerInvoke.DialogueNodePorts, _nodeTimerInvoke.Duration, _nodeTimerInvoke.time); }
        void TimerNode_NextNode() { CheckNodeType(GetNextNode(_nodeTimerInvoke)); }

        public void SkipDialogue()
        {
            StopAllCoroutines();

            switch (currentDialogueNodeData)
            {
                case DialogueNodeData nodeData:
                    DialogueNode_NextNode();
                    break;
                case DialogueChoiceNodeData nodeData:
                    ChoiceNode_GenerateChoice();
                    break;
                case TimerChoiceNodeData nodeData:
                    TimerNode_GenerateChoice();
                    break;
                default:
                    break;
            }
        }
    }
}
