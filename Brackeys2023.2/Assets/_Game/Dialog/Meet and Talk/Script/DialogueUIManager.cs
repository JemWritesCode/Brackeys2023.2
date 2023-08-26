using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace MEET_AND_TALK
{
    public class DialogueUIManager : MonoBehaviour
    {
        public static DialogueUIManager Instance;

        [Header("Type Writing")]
        public bool EnableTypeWriting = false;
        public float typingSpeed = 50.0f;

        [Header("Dialogue UI")]
        public GameObject dialogueCanvas;
        public TextMeshProUGUI textBox;
        public Slider TimerSlider;

        [Header("Dynamic Dialogue UI")]
        public GameObject ButtonPrefab;
        public GameObject ButtonContainer;

        [HideInInspector] public string prefixText;
        [HideInInspector] public string fullText;
        private string currentText = "";
        private int characterIndex = 0;
        private float lastTypingTime;

        private List<Button> buttons = new List<Button>();
        private List<TextMeshProUGUI> buttonsTexts = new List<TextMeshProUGUI>();



        private void Awake()
        {
            Instance = this;
            if(EnableTypeWriting) lastTypingTime = Time.time;
        }

        private void Update()
        {
            if (characterIndex < fullText.Length && EnableTypeWriting)
            {
                if (Time.time - lastTypingTime > 1.0f / typingSpeed)
                {
                    currentText += fullText[characterIndex];
                    textBox.text = currentText;

                    characterIndex++;

                    lastTypingTime = Time.time;
                }
            }
            else
            {
                textBox.text = prefixText+fullText;
            }
        }

        public void ResetText(string prefix)
        {
            currentText = prefix;
            prefixText = prefix;
            characterIndex = 0;
        }

        public void SetButtons(List<string> _texts, List<UnityAction> _unityActions, bool showTimer)
        {
            foreach (Transform child in ButtonContainer.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            for (int i = 0; i < _texts.Count; i++)
            {
                GameObject btn = Instantiate(ButtonPrefab, ButtonContainer.transform);
                btn.transform.Find("Text").GetComponent<TMP_Text>().text = _texts[i];
                btn.gameObject.SetActive(true);
                btn.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                btn.GetComponent<Button>().onClick.AddListener(_unityActions[i]);
            }

            TimerSlider.gameObject.SetActive(showTimer);
        }

    }
}
