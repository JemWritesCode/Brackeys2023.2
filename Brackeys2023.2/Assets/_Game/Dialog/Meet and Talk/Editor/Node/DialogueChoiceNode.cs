using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Linq;

namespace MEET_AND_TALK
{
    public class DialogueChoiceNode : BaseNode
    {
        private List<LanguageGeneric<string>> texts = new List<LanguageGeneric<string>>();
        private List<LanguageGeneric<AudioClip>> audioClip = new List<LanguageGeneric<AudioClip>>();
        private DialogueCharacterSO character = new DialogueCharacterSO();
        private float durationShow = 5;

        public List<DialogueNodePort> dialogueNodePorts = new List<DialogueNodePort>();

        public List<LanguageGeneric<string>> Texts { get => texts; set => texts = value; }
        public List<LanguageGeneric<AudioClip>> AudioClip { get => audioClip; set => audioClip = value; }
        public DialogueCharacterSO Character { get => character; set => character = value; }
        public float DurationShow { get => durationShow; set => durationShow = value; }

        private TextField texts_Field;
        private ObjectField audioClips_Field;
        private FloatField duration_Field;
        private ObjectField character_Field;

        public DialogueChoiceNode()
        {

        }

        public DialogueChoiceNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
        {
            editorWindow = _editorWindow;
            graphView = _graphView;

            title = "Choice";
            SetPosition(new Rect(_position, defualtNodeSize));
            nodeGuid = Guid.NewGuid().ToString();

            AddInputPort("Input ", Port.Capacity.Multi);

            foreach (LocalizationEnum language in (LocalizationEnum[])Enum.GetValues(typeof(LocalizationEnum)))
            {
                texts.Add(new LanguageGeneric<string>
                {
                    languageEnum = language,
                    LanguageGenericType = ""
                });
                AudioClip.Add(new LanguageGeneric<AudioClip>
                {
                    languageEnum = language,
                    LanguageGenericType = null
                });
            }
            /* AUDIO CLIPS */
            audioClips_Field = new ObjectField()
            {
                objectType = typeof(AudioClip),
                allowSceneObjects = false,
                value = audioClip.Find(audioClips => audioClips.languageEnum == editorWindow.LanguageEnum).LanguageGenericType,
            };
            audioClips_Field.RegisterValueChangedCallback(value =>
            {
                audioClip.Find(audioClips => audioClips.languageEnum == editorWindow.LanguageEnum).LanguageGenericType = value.newValue as AudioClip;
            });
            audioClips_Field.SetValueWithoutNotify(audioClip.Find(audioClips => audioClips.languageEnum == editorWindow.LanguageEnum).LanguageGenericType);
            mainContainer.Add(audioClips_Field);

            /* Character CLIPS */
            Label label_character = new Label("Character SO");
            label_character.AddToClassList("label_name");
            label_character.AddToClassList("Label");
            mainContainer.Add(label_character);
            character_Field = new ObjectField()
            {
                objectType = typeof(DialogueCharacterSO),
                allowSceneObjects = false,
            };
            character_Field.RegisterValueChangedCallback(value =>
            {
                character = value.newValue as DialogueCharacterSO;
            });
            character_Field.SetValueWithoutNotify(character);
            mainContainer.Add(character_Field);


            /* TEXT BOX */
            Label label_texts = new Label("Displayed Text");
            label_texts.AddToClassList("label_texts");
            label_texts.AddToClassList("Label");
            mainContainer.Add(label_texts);

            texts_Field = new TextField("");
            texts_Field.RegisterValueChangedCallback(value =>
            {
                texts.Find(text => text.languageEnum == editorWindow.LanguageEnum).LanguageGenericType = value.newValue;
            });
            texts_Field.SetValueWithoutNotify(texts.Find(text => text.languageEnum == editorWindow.LanguageEnum).LanguageGenericType);
            texts_Field.multiline = true;

            texts_Field.AddToClassList("TextBox");
            mainContainer.Add(texts_Field);

            /* DIALOGUE DURATION */
            Label label_duration = new Label("Time to Display Options");
            label_duration.AddToClassList("label_duration");
            label_duration.AddToClassList("Label");
            mainContainer.Add(label_duration);

            duration_Field = new FloatField("");
            duration_Field.RegisterValueChangedCallback(value =>
            {
                durationShow = value.newValue;
            });
            duration_Field.SetValueWithoutNotify(durationShow);

            duration_Field.AddToClassList("TextDuration");
            mainContainer.Add(duration_Field);

            Button button = new Button()
            {
                text = "+ Add Choice Option"
            };
            button.clicked += () =>
            {
                AddChoicePort(this);
            };

            titleButtonContainer.Add(button);
        }

        public void ReloadLanguage()
        {
            texts_Field.RegisterValueChangedCallback(value =>
            {
                texts.Find(text => text.languageEnum == editorWindow.LanguageEnum).LanguageGenericType = value.newValue;
            });
            texts_Field.SetValueWithoutNotify(texts.Find(text => text.languageEnum == editorWindow.LanguageEnum).LanguageGenericType);

            audioClips_Field.RegisterValueChangedCallback(value =>
            {
                audioClip.Find(audioClips => audioClips.languageEnum == editorWindow.LanguageEnum).LanguageGenericType = value.newValue as AudioClip;
            });
            audioClips_Field.SetValueWithoutNotify(audioClip.Find(audioClips => audioClips.languageEnum == editorWindow.LanguageEnum).LanguageGenericType);

            foreach (DialogueNodePort nodePort in dialogueNodePorts)
            {
                nodePort.TextField.RegisterValueChangedCallback(value =>
                {
                    nodePort.TextLanguage.Find(language => language.languageEnum == editorWindow.LanguageEnum).LanguageGenericType = value.newValue;
                });
                nodePort.TextField.SetValueWithoutNotify(nodePort.TextLanguage.Find(language => language.languageEnum == editorWindow.LanguageEnum).LanguageGenericType);
            }
        }

        public override void LoadValueInToField()
        {
            texts_Field.SetValueWithoutNotify(texts.Find(language => language.languageEnum == editorWindow.LanguageEnum).LanguageGenericType);
            character_Field.SetValueWithoutNotify(character);
            audioClips_Field.SetValueWithoutNotify(audioClip.Find(language => language.languageEnum == editorWindow.LanguageEnum).LanguageGenericType);
            duration_Field.SetValueWithoutNotify(durationShow);
        }

        public Port AddChoicePort(BaseNode _basenote, DialogueNodePort _dialogueNodePort = null)
        {
            Port port = GetPortInstance(Direction.Output);


            string outputPortName = "";
            int outputPortCount = _basenote.outputContainer.Query("connector").ToList().Count();
            if (outputPortCount < 9) { outputPortName = $"Choice 0{outputPortCount + 1}"; }
            else { outputPortName = $"Choice {outputPortCount + 1}"; }

            DialogueNodePort dialogueNodePort = new DialogueNodePort();

            foreach (LocalizationEnum language in (LocalizationEnum[])Enum.GetValues(typeof(LocalizationEnum)))
            {
                dialogueNodePort.TextLanguage.Add(new LanguageGeneric<string>()
                {
                    languageEnum = language,
                    LanguageGenericType = outputPortName
                });
            }

            if (_dialogueNodePort != null)
            {
                dialogueNodePort.InputGuid = _dialogueNodePort.InputGuid;
                dialogueNodePort.OutputGuid = _dialogueNodePort.OutputGuid;

                foreach (LanguageGeneric<string> languageGeneric in _dialogueNodePort.TextLanguage)
                {
                    dialogueNodePort.TextLanguage.Find(language => language.languageEnum == languageGeneric.languageEnum).LanguageGenericType = languageGeneric.LanguageGenericType;
                }
            }

            dialogueNodePort.TextField = new TextField();
            dialogueNodePort.TextField.RegisterValueChangedCallback(value =>
            {
                dialogueNodePort.TextLanguage.Find(language => language.languageEnum == editorWindow.LanguageEnum).LanguageGenericType = value.newValue;
            });
            dialogueNodePort.TextField.SetValueWithoutNotify(dialogueNodePort.TextLanguage.Find(language => language.languageEnum == editorWindow.LanguageEnum).LanguageGenericType);

            texts_Field.multiline = true;
            dialogueNodePort.TextField.AddToClassList("ChoiceLabel");
            port.contentContainer.Add(dialogueNodePort.TextField);

            Button deleteButton = new Button(() => DeleteButton(_basenote, port))
            {
                text = "X"
            };
            port.contentContainer.Add(deleteButton);

#if UNITY_EDITOR
            dialogueNodePort.MyPort = port;
#endif
            port.portName = "";

            dialogueNodePorts.Add(dialogueNodePort);

            _basenote.outputContainer.Add(port);

            _basenote.RefreshPorts();
            _basenote.RefreshExpandedState();

            return port;
        }

        private void DeleteButton(BaseNode _node, Port _port)
        {
#if UNITY_EDITOR
            DialogueNodePort tmp = dialogueNodePorts.Find(port => port.MyPort == _port);
            dialogueNodePorts.Remove(tmp);
#endif

            IEnumerable<Edge> portEdge = graphView.edges.ToList().Where(edge => edge.output == _port);

            if (portEdge.Any())
            {
                Edge edge = portEdge.First();
                edge.input.Disconnect(edge);
                edge.output.Disconnect(edge);
                graphView.RemoveElement(edge);
            }

            _node.outputContainer.Remove(_port);

            _node.RefreshPorts();
            _node.RefreshExpandedState();
        }
    }
}