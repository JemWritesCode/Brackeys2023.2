using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;

namespace MEET_AND_TALK
{
#if UNITY_EDITOR

    public class DialogueSaveAndLoad
    {
        private List<Edge> edges => graphView.edges.ToList();
        private List<BaseNode> nodes => graphView.nodes.ToList().Where(node => node is BaseNode).Cast<BaseNode>().ToList();

        public DialogueGraphView graphView;

        public DialogueSaveAndLoad(DialogueGraphView _graphView)
        {
            graphView = _graphView;
        }

        public void Save(DialogueContainerSO _dialogueContainerSO)
        {
            SaveEdges(_dialogueContainerSO);
            SaveNodes(_dialogueContainerSO);

            EditorUtility.SetDirty(_dialogueContainerSO);
            AssetDatabase.SaveAssets();
        }
        public void Load(DialogueContainerSO _dialogueContainerSO)
        {
            ClearGraph();
            GenerateNodes(_dialogueContainerSO);
            ConnectNodes(_dialogueContainerSO);
        }

        #region Save

        public void SaveEdges(DialogueContainerSO _dialogueContainerSO)
        {
            _dialogueContainerSO.NodeLinkDatas.Clear();

            Edge[] connectedEdges = edges.Where(edge => edge.input.node != null).ToArray();
            for (int i = 0; i < connectedEdges.Count(); i++)
            {
                BaseNode outputNode = (BaseNode)connectedEdges[i].output.node;
                BaseNode inputNode = connectedEdges[i].input.node as BaseNode;

                _dialogueContainerSO.NodeLinkDatas.Add(new NodeLinkData
                {
                    BaseNodeGuid = outputNode.nodeGuid,
                    TargetNodeGuid = inputNode.nodeGuid
                });
            }
        }

        private void SaveNodes(DialogueContainerSO _dialogueContainerSO)
        {
            _dialogueContainerSO.DialogueChoiceNodeDatas.Clear();
            _dialogueContainerSO.DialogueNodeDatas.Clear();
            _dialogueContainerSO.TimerChoiceNodeDatas.Clear();
            _dialogueContainerSO.EventNodeDatas.Clear();
            _dialogueContainerSO.EndNodeDatas.Clear();
            _dialogueContainerSO.StartNodeDatas.Clear();
            _dialogueContainerSO.RandomNodeDatas.Clear();
            _dialogueContainerSO.CommandNodeDatas.Clear();

            nodes.ForEach(node =>
            {
                switch (node)
                {
                    case DialogueChoiceNode dialogueChoiceNode:
                        _dialogueContainerSO.DialogueChoiceNodeDatas.Add(SaveNodeData(dialogueChoiceNode));
                        break;
                    case DialogueNode dialogueNode:
                        _dialogueContainerSO.DialogueNodeDatas.Add(SaveNodeData(dialogueNode));
                        break;
                    case TimerChoiceNode timerChoiceNode:
                        _dialogueContainerSO.TimerChoiceNodeDatas.Add(SaveNodeData(timerChoiceNode));
                        break;
                    case StartNode startNode:
                        _dialogueContainerSO.StartNodeDatas.Add(SaveNodeData(startNode));
                        break;
                    case EndNode endNode:
                        _dialogueContainerSO.EndNodeDatas.Add(SaveNodeData(endNode));
                        break;
                    case EventNode eventNode:
                        _dialogueContainerSO.EventNodeDatas.Add(SaveNodeData(eventNode));
                        break;
                    case RandomNote randomNode:
                        _dialogueContainerSO.RandomNodeDatas.Add(SaveNodeData(randomNode));
                        break;
                    case CommandNode commandNode:
                        _dialogueContainerSO.CommandNodeDatas.Add(SaveNodeData(commandNode));
                        break;
                    default:
                        break;
                }
            });
        }

        private DialogueChoiceNodeData SaveNodeData(DialogueChoiceNode _node)
        {
            DialogueChoiceNodeData dialogueNodeData = new DialogueChoiceNodeData
            {
                NodeGuid = _node.nodeGuid,
                Position = _node.GetPosition().position,
                TextType = _node.Texts,
                Character = _node.Character,
                AudioClips = _node.AudioClip,
                DialogueNodePorts = _node.dialogueNodePorts,
                Duration = _node.DurationShow
            };
            foreach (DialogueNodePort nodePort in dialogueNodeData.DialogueNodePorts)
            {
                nodePort.OutputGuid = string.Empty;
                nodePort.InputGuid = string.Empty;
                foreach (Edge edge in edges)
                {
                    if (edge.output == nodePort.MyPort)
                    {
                        nodePort.OutputGuid = (edge.output.node as BaseNode).nodeGuid;
                        nodePort.InputGuid = (edge.input.node as BaseNode).nodeGuid;
                    }
                }
            }


            return dialogueNodeData;
        }

        private RandomNodeData SaveNodeData(RandomNote _node)
        {
            RandomNodeData dialogueNodeData = new RandomNodeData
            {
                NodeGuid = _node.nodeGuid,
                Position = _node.GetPosition().position,
                DialogueNodePorts = _node.dialogueNodePorts,
            };
            foreach (DialogueNodePort nodePort in dialogueNodeData.DialogueNodePorts)
            {
                nodePort.OutputGuid = string.Empty;
                nodePort.InputGuid = string.Empty;
                foreach (Edge edge in edges)
                {
                    if (edge.output == nodePort.MyPort)
                    {
                        nodePort.OutputGuid = (edge.output.node as BaseNode).nodeGuid;
                        nodePort.InputGuid = (edge.input.node as BaseNode).nodeGuid;
                    }
                }
            }


            return dialogueNodeData;
        }

        private TimerChoiceNodeData SaveNodeData(TimerChoiceNode _node)
        {
            TimerChoiceNodeData dialogueNodeData = new TimerChoiceNodeData
            {
                NodeGuid = _node.nodeGuid,
                Position = _node.GetPosition().position,
                TextType = _node.Texts,
                Character = _node.Character,
                AudioClips = _node.AudioClip,
                time = _node.ChoiceTime,
                DialogueNodePorts = _node.dialogueNodePorts,
                Duration = _node.DurationShow
            };
            foreach (DialogueNodePort nodePort in dialogueNodeData.DialogueNodePorts)
            {
                nodePort.OutputGuid = string.Empty;
                nodePort.InputGuid = string.Empty;
                foreach (Edge edge in edges)
                {
                    if (edge.output == nodePort.MyPort)
                    {
                        nodePort.OutputGuid = (edge.output.node as BaseNode).nodeGuid;
                        nodePort.InputGuid = (edge.input.node as BaseNode).nodeGuid;
                    }
                }
            }


            return dialogueNodeData;
        }
        private StartNodeData SaveNodeData(StartNode _node)
        {
            StartNodeData nodeData = new StartNodeData
            {
                NodeGuid = _node.nodeGuid,
                Position = _node.GetPosition().position,
                startID = _node.startID
            };
            return nodeData;
        }
        private EndNodeData SaveNodeData(EndNode _node)
        {
            EndNodeData nodeData = new EndNodeData
            {
                NodeGuid = _node.nodeGuid,
                Position = _node.GetPosition().position,
                EndNodeType = _node.EndNodeType
            };
            return nodeData;
        }
        private EventNodeData SaveNodeData(EventNode _node)
        {
            EventNodeData nodeData = new EventNodeData
            {
                NodeGuid = _node.nodeGuid,
                Position = _node.GetPosition().position,
                EventScriptableObjects = _node.EventScriptableObjectDatas
            };
            return nodeData;
        }
        private DialogueNodeData SaveNodeData(DialogueNode _node)
        {
            DialogueNodeData dialogueNodeData = new DialogueNodeData
            {
                NodeGuid = _node.nodeGuid,
                Position = _node.GetPosition().position,
                TextType = _node.Texts,
                Character = _node.Character,
                AudioClips = _node.AudioClip,
                DialogueNodePorts = _node.dialogueNodePorts,
                Duration = _node.DurationShow
            };

            return dialogueNodeData;
        }
        private CommandNodeData SaveNodeData(CommandNode _node)
        {
            Debug.Log(_node.command);
            CommandNodeData dialogueNodeData = new CommandNodeData
            {
                NodeGuid = _node.nodeGuid,
                Position = _node.GetPosition().position,
                
                commmand = _node.command
            };

            return dialogueNodeData;
        }
        #endregion

        #region Load

        private void ClearGraph()
        {
            edges.ForEach(edge => graphView.RemoveElement(edge));

            foreach (BaseNode node in nodes)
            {
                graphView.RemoveElement(node);
            }
        }

        private void GenerateNodes(DialogueContainerSO _dialogueContainer)
        {
            /* Start Node */
            foreach (StartNodeData node in _dialogueContainer.StartNodeDatas)
            {
                StartNode tempNode = graphView.CreateStartNode(node.Position);
                tempNode.nodeGuid = node.NodeGuid;
                tempNode.startID = node.startID;

                tempNode.LoadValueInToField();
                graphView.AddElement(tempNode);
            }

            /* End Node */
            foreach (EndNodeData node in _dialogueContainer.EndNodeDatas)
            {
                EndNode tempNode = graphView.CreateEndNode(node.Position);
                tempNode.nodeGuid = node.NodeGuid;
                tempNode.EndNodeType = node.EndNodeType;

                tempNode.LoadValueInToField();
                graphView.AddElement(tempNode);
            }

            /* Event Node */
            foreach (EventNodeData node in _dialogueContainer.EventNodeDatas)
            {
                EventNode tempNode = graphView.CreateEventNode(node.Position);
                tempNode.nodeGuid = node.NodeGuid;

                foreach (EventScriptableObjectData item in node.EventScriptableObjects)
                {
                    tempNode.AddScriptableEvent(item);
                    //tempNode.GenerateFields(item);
                }

                tempNode.LoadValueInToField();
                graphView.AddElement(tempNode);
            }

            /* Dialogue Choice Node */
            foreach (DialogueChoiceNodeData node in _dialogueContainer.DialogueChoiceNodeDatas)
            {
                DialogueChoiceNode tempNode = graphView.CreateDialogueChoiceNode(node.Position);
                tempNode.nodeGuid = node.NodeGuid;


                foreach (LanguageGeneric<string> languageGeneric in node.TextType)
                {
                    tempNode.Texts.Find(language => language.languageEnum == languageGeneric.languageEnum).LanguageGenericType = languageGeneric.LanguageGenericType;
                }
                foreach (LanguageGeneric<AudioClip> languageGeneric in node.AudioClips)
                {
                    tempNode.AudioClip.Find(language => language.languageEnum == languageGeneric.languageEnum).LanguageGenericType = languageGeneric.LanguageGenericType;
                }

                foreach (DialogueNodePort nodePort in node.DialogueNodePorts)
                {
                    tempNode.AddChoicePort(tempNode, nodePort);
                }
                tempNode.Character = node.Character;
                tempNode.DurationShow = node.Duration;

                tempNode.LoadValueInToField();
                graphView.AddElement(tempNode);
            }

            /* Random Note */
            foreach (RandomNodeData node in _dialogueContainer.RandomNodeDatas)
            {
                RandomNote tempNode = graphView.CreateRandomNode(node.Position);
                tempNode.nodeGuid = node.NodeGuid;

                foreach (DialogueNodePort nodePort in node.DialogueNodePorts)
                {
                    tempNode.AddChoicePort(tempNode, nodePort);
                }

                tempNode.LoadValueInToField();
                graphView.AddElement(tempNode);
            }

            /* Command Note */
            foreach (CommandNodeData node in _dialogueContainer.CommandNodeDatas)
            {
                CommandNode tempNode = graphView.CreateCommandNode(node.Position);
                tempNode.nodeGuid = node.NodeGuid;
                tempNode.command = node.commmand;


                tempNode.LoadValueInToField();
                graphView.AddElement(tempNode);
            }

            /* Timer Choice Node */
            foreach (TimerChoiceNodeData node in _dialogueContainer.TimerChoiceNodeDatas)
            {
                TimerChoiceNode tempNode = graphView.CreateTimerChoiceNode(node.Position);
                tempNode.nodeGuid = node.NodeGuid;

                foreach (LanguageGeneric<string> languageGeneric in node.TextType)
                {
                    tempNode.Texts.Find(language => language.languageEnum == languageGeneric.languageEnum).LanguageGenericType = languageGeneric.LanguageGenericType;
                }
                foreach (LanguageGeneric<AudioClip> languageGeneric in node.AudioClips)
                {
                    tempNode.AudioClip.Find(language => language.languageEnum == languageGeneric.languageEnum).LanguageGenericType = languageGeneric.LanguageGenericType;
                }
                tempNode.Character = node.Character;
                tempNode.DurationShow = node.Duration;
                tempNode.ChoiceTime = node.time;

                foreach (DialogueNodePort nodePort in node.DialogueNodePorts)
                {
                    tempNode.AddChoicePort(tempNode, nodePort);
                }

                tempNode.LoadValueInToField();
                graphView.AddElement(tempNode);
            }

            /* Dialogue Node */
            foreach (DialogueNodeData node in _dialogueContainer.DialogueNodeDatas)
            {
                DialogueNode tempNode = graphView.CreateDialogueNode(node.Position);
                tempNode.nodeGuid = node.NodeGuid;

                foreach (LanguageGeneric<string> languageGeneric in node.TextType)
                {
                    tempNode.Texts.Find(language => language.languageEnum == languageGeneric.languageEnum).LanguageGenericType = languageGeneric.LanguageGenericType;
                }

                foreach (LanguageGeneric<AudioClip> languageGeneric in node.AudioClips)
                {
                    tempNode.AudioClip.Find(language => language.languageEnum == languageGeneric.languageEnum).LanguageGenericType = languageGeneric.LanguageGenericType;
                }

                tempNode.Character = node.Character;
                tempNode.DurationShow = node.Duration;

                tempNode.LoadValueInToField();
                graphView.AddElement(tempNode);
            }
        }

        private void ConnectNodes(DialogueContainerSO _dialogueContainer)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                List<NodeLinkData> connections = _dialogueContainer.NodeLinkDatas.Where(edge => edge.BaseNodeGuid == nodes[i].nodeGuid).ToList();

                for (int j = 0; j < connections.Count; j++)
                {
                    string targetNodeGuid = connections[j].TargetNodeGuid;
                    BaseNode targetNode = nodes.First(node => node.nodeGuid == targetNodeGuid);

                    if ((nodes[i] is DialogueChoiceNode) == false)
                    {
                        LinkNodesTogether(nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);
                    }
                }
            }

            List<DialogueChoiceNode> dialogueNodes = nodes.FindAll(node => node is DialogueChoiceNode).Cast<DialogueChoiceNode>().ToList();

            foreach (DialogueChoiceNode dialogueNode in dialogueNodes)
            {
                foreach (DialogueNodePort nodePort in dialogueNode.dialogueNodePorts)
                {
                    if (nodePort.InputGuid != string.Empty)
                    {
                        BaseNode targetNode = nodes.First(Node => Node.nodeGuid == nodePort.InputGuid);
                        LinkNodesTogether(nodePort.MyPort, (Port)targetNode.inputContainer[0]);
                    }
                }
            }
        }

        private void LinkNodesTogether(Port _outputPort, Port _inputPort)
        {
            Edge tempEdge = new Edge()
            {
                output = _outputPort,
                input = _inputPort
            };
            tempEdge.input.Connect(tempEdge);
            tempEdge.output.Connect(tempEdge);
            graphView.Add(tempEdge);
        }

        #endregion
    }

#endif
}
