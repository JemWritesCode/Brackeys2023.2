using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Reflection;
using System;
using UnityEditor;

namespace MEET_AND_TALK
{
    public class EventNode : BaseNode
    {
        private List<EventScriptableObjectData> eventScriptableObjectDatas = new List<EventScriptableObjectData>();

        public List<EventScriptableObjectData> EventScriptableObjectDatas { get => eventScriptableObjectDatas; set => eventScriptableObjectDatas = value; }

        public EventNode()
        {

        }

        public EventNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
        {
            editorWindow = _editorWindow;
            graphView = _graphView;

            title = "Event";
            SetPosition(new Rect(_position, defualtNodeSize));
            nodeGuid = Guid.NewGuid().ToString();

            AddInputPort("Input", Port.Capacity.Multi);
            AddOutputPort("Output", Port.Capacity.Single);

            TopButton();
        }

        public override void LoadValueInToField()
        {
            //
        }

        private void TopButton()
        {
            ToolbarMenu button = new ToolbarMenu();
            button.text = "+ Add Event";

            button.menu.AppendAction("Empty Field", new Action<DropdownMenuAction>(x => AddScriptableEvent()));
            button.menu.AppendSeparator();

            List<Type> subclasses = SubclassFinder.GetSubclasses<DialogueEventSO>();
            for (int i = 1; i < subclasses.Count; i++)
            {
                int index = i;
                button.menu.AppendAction($"New {subclasses[i].Name}", new Action<DropdownMenuAction>(x => AddNewEvent(subclasses[index])));
            }

            titleButtonContainer.Add(button);
        }

        public void AddNewEvent(Type type)
        {
            // Create New Element
            ScriptableObject newObject = ScriptableObject.CreateInstance(type);
            string path = $"Assets/Meet and Talk/Example/Events/{type.Name}_{UnityEngine.Random.Range(0, 10000)}.asset";
            AssetDatabase.CreateAsset(newObject, path);
            AssetDatabase.SaveAssets();

            // Add to list
            EventScriptableObjectData tmp = new EventScriptableObjectData();
            tmp.DialogueEventSO = (DialogueEventSO)newObject;
            AddScriptableEvent(tmp);
        }

        public void AddScriptableEvent(EventScriptableObjectData paramidaEventScriptableObjectData = null)
        {
            EventScriptableObjectData tmpDialogueEventSO = new EventScriptableObjectData();
            if (paramidaEventScriptableObjectData != null)
            {
                tmpDialogueEventSO.DialogueEventSO = paramidaEventScriptableObjectData.DialogueEventSO;
            }
            eventScriptableObjectDatas.Add(tmpDialogueEventSO);

            Box boxContainer = new Box();
            boxContainer.AddToClassList("EventBox");

            ObjectField objectField = new ObjectField()
            {
                objectType = typeof(DialogueEventSO),
                allowSceneObjects = false,
                value = null,
            };
            objectField.AddToClassList("EventSO");
            boxContainer.Add(objectField);

            //
            Box ValueBox = new Box();
            ValueBox.name = UnityEngine.Random.Range(1, 999999999).ToString();

            objectField.RegisterValueChangedCallback(value =>
            {
                tmpDialogueEventSO.DialogueEventSO = value.newValue as DialogueEventSO;
                //VisualElement elementToRemove = mainContainer.Q(ValueBox.name);
                //Debug.Log(elementToRemove.name);
                //Debug.Log(ValueBox.name);
                //mainContainer.Remove(elementToRemove);
                //GenerateFields(ValueBox, tmpDialogueEventSO);
                mainContainer.RemoveAt(mainContainer.IndexOf(ValueBox));
                eventScriptableObjectDatas.RemoveAt(eventScriptableObjectDatas.IndexOf(tmpDialogueEventSO));
                AddScriptableEvent(tmpDialogueEventSO);
                mainContainer.RemoveAt(mainContainer.IndexOf(boxContainer));
            });
            objectField.SetValueWithoutNotify(tmpDialogueEventSO.DialogueEventSO);


            Button btn = new Button()
            {
                text = "X",
            };
            btn.clicked += () =>
            {
                DeleteBox(boxContainer);
                DeleteBox(ValueBox);
                eventScriptableObjectDatas.Remove(tmpDialogueEventSO);
            };
            btn.AddToClassList("EventBtn");

            boxContainer.Add(btn);

            mainContainer.Add(boxContainer);

            //* Event Value *//
            GenerateFields(ValueBox, tmpDialogueEventSO);

            RefreshExpandedState();
        }

        private void DeleteBox(Box boxContainer)
        {
            mainContainer.Remove(boxContainer);
        }

        public void GenerateFields(VisualElement ValueBox, EventScriptableObjectData paramidaEventScriptableObjectData = null)
        {
            if (paramidaEventScriptableObjectData != null)
            {
                if (paramidaEventScriptableObjectData.DialogueEventSO != null)
                {
                    Type scriptableObjectType = paramidaEventScriptableObjectData.DialogueEventSO.GetType();
                    FieldInfo[] fields = scriptableObjectType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    for (int i = 0; i < fields.Length; i++)
                    {
                        int index = i;

                        Box boxContainer2 = new Box();
                        boxContainer2.AddToClassList("EventBox");

                        Debug.Log(fields[i].FieldType);

                        if (fields[i].FieldType == typeof(int))
                        {
                            IntegerField objectField2 = new IntegerField()
                            {
                                value = (int)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEventSO),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value));
                            objectField2.AddToClassList("EventSO");

                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEventSO);

                            boxContainer2.Add(objectField2);
                        }
                        else if (fields[i].FieldType == typeof(float))
                        {
                            FloatField objectField2 = new FloatField()
                            {
                                value = (float)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEventSO),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value));
                            objectField2.AddToClassList("EventSO");

                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEventSO);

                            boxContainer2.Add(objectField2);
                        }
                        else if (fields[i].FieldType == typeof(string))
                        {
                            TextField objectField2 = new TextField()
                            {
                                value = (string)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEventSO),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value));
                            objectField2.AddToClassList("EventSO");

                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEventSO);

                            boxContainer2.Add(objectField2);
                        }
                        else if (fields[i].FieldType == typeof(bool))
                        {
                            Toggle objectField2 = new Toggle()
                            {
                                value = (bool)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEventSO),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value));
                            objectField2.AddToClassList("EventSO");

                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEventSO);

                            boxContainer2.Add(objectField2);
                        }
                        else if (fields[i].FieldType == typeof(Vector2))
                        {
                            Vector2Field objectField2 = new Vector2Field()
                            {
                                value = (Vector2)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEventSO),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value));
                            objectField2.AddToClassList("EventSO");

                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEventSO);

                            boxContainer2.Add(objectField2);
                        }
                        else if (fields[i].FieldType == typeof(Vector3))
                        {
                            Vector3Field objectField2 = new Vector3Field()
                            {
                                value = (Vector3)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEventSO),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value));
                            objectField2.AddToClassList("EventSO");

                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEventSO);

                            boxContainer2.Add(objectField2);
                        }
                        else if (fields[i].FieldType == typeof(Vector4))
                        {
                            Vector4Field objectField2 = new Vector4Field()
                            {
                                value = (Vector4)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEventSO),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value));
                            objectField2.AddToClassList("EventSO");

                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEventSO);

                            boxContainer2.Add(objectField2);
                        }

                        else if (fields[i].FieldType.IsEnum)
                        {
                            Enum enumValue = (Enum)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEventSO);
                            EnumField objectField2 = new EnumField(enumValue)
                            {
                                value = (Enum)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEventSO),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value));
                            objectField2.AddToClassList("EventSO");

                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEventSO);

                            boxContainer2.Add(objectField2);
                        }
                        else if (fields[i].FieldType.IsSubclassOf(typeof(UnityEngine.Object)))
                        {
                            ObjectField objectField2 = new ObjectField()
                            {
                                objectType = fields[i].FieldType,
                                allowSceneObjects = false,
                                value = (UnityEngine.Object)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEventSO),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value));
                            objectField2.AddToClassList("EventSO");
                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEventSO);
                            boxContainer2.Add(objectField2);
                        }
                        else if (fields[i].FieldType == typeof(Color))
                        {
                            ColorField objectField2 = new ColorField()
                            {
                                value = (Color)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEventSO),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value));
                            objectField2.AddToClassList("EventSO");

                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEventSO);

                            boxContainer2.Add(objectField2);
                        }
                        else if (fields[i].FieldType == typeof(Gradient))
                        {
                            GradientField objectField2 = new GradientField()
                            {
                                value = (Gradient)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEventSO),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value));
                            objectField2.AddToClassList("EventSO");

                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEventSO);

                            boxContainer2.Add(objectField2);
                        }
                        else if (fields[i].FieldType == typeof(AnimationCurve))
                        {
                            CurveField objectField2 = new CurveField()
                            {
                                value = (AnimationCurve)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEventSO),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value));
                            objectField2.AddToClassList("EventSO");

                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEventSO);

                            boxContainer2.Add(objectField2);
                        }
                        else if (fields[i].FieldType == typeof(Vector2Int))
                        {
                            Vector2IntField objectField2 = new Vector2IntField()
                            {
                                value = (Vector2Int)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEventSO),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value));
                            objectField2.AddToClassList("EventSO");

                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEventSO);

                            boxContainer2.Add(objectField2);
                        }
                        else if (fields[i].FieldType == typeof(Vector3Int))
                        {
                            Vector3IntField objectField2 = new Vector3IntField()
                            {
                                value = (Vector3Int)fields[i].GetValue(paramidaEventScriptableObjectData.DialogueEventSO),
                                label = fields[i].Name
                            };

                            objectField2.RegisterValueChangedCallback(x => fields[index].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value));
                            objectField2.AddToClassList("EventSO");

                            fields[i].SetValue(paramidaEventScriptableObjectData.DialogueEventSO, objectField2.value);
                            EditorUtility.SetDirty(paramidaEventScriptableObjectData.DialogueEventSO);

                            boxContainer2.Add(objectField2);
                        }
                        else
                        {
                            Label objectField2 = new Label($"Event doesn't support {fields[i].FieldType.ToString()}");
                            objectField2.AddToClassList("EventSO");
                            boxContainer2.Add(objectField2);
                        }

                        ValueBox.Add(boxContainer2);
                        mainContainer.Add(ValueBox);
                    }
                }
            }
        }
    }
}