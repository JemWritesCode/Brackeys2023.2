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
    public class StartNode : BaseNode
    {
        public string startID;
        private TextField idField;

        public StartNode()
        {

        }

        public StartNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
        {
            editorWindow = _editorWindow;
            graphView = _graphView;

            title = "Start";
            SetPosition(new Rect(_position, defualtNodeSize));
            nodeGuid = Guid.NewGuid().ToString();

            idField = new TextField("ID:");
            idField.RegisterValueChangedCallback(value =>
            {
                startID = value.newValue;
            });
            idField.SetValueWithoutNotify(startID);
            mainContainer.Add(idField);

            AddOutputPort("Output", Port.Capacity.Single);
            RefreshExpandedState();
            RefreshPorts();
        }

        public override void LoadValueInToField()
        {
            idField.SetValueWithoutNotify(startID);
        }
    }
}
