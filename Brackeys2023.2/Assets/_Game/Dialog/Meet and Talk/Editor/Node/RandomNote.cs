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

    public class RandomNote : BaseNode
    {
        public List<DialogueNodePort> dialogueNodePorts = new List<DialogueNodePort>();

        public RandomNote()
        {

        }

        public RandomNote(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
        {
            editorWindow = _editorWindow;
            graphView = _graphView;

            title = "Random";
            SetPosition(new Rect(_position, defualtNodeSize));
            nodeGuid = Guid.NewGuid().ToString();

            AddInputPort("Input ", Port.Capacity.Multi);


            Button button = new Button()
            {
                text = "+ Add Option"
            };
            button.clicked += () =>
            {
                AddChoicePort(this);
            };

            titleButtonContainer.Add(button);
        }

        public void ReloadLanguage()
        {
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
