using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEET_AND_TALK
{
    [CreateAssetMenu(menuName = "Dialogue/New Dialogue Character")]
    public class DialogueCharacterSO : ScriptableObject
    {
        public List<LanguageGeneric<string>> characterName;
        public Color textColor = new Color(.8f, .8f, .8f, 1);

        public string HexColor()
        {
            return $"#{ColorUtility.ToHtmlStringRGB(textColor)}";
        }

        private void OnValidate()
        {
            if (characterName.Count != System.Enum.GetNames(typeof(LocalizationEnum)).Length)
            {
                // Mniej
                if (characterName.Count < System.Enum.GetNames(typeof(LocalizationEnum)).Length)
                {
                    for (int i = characterName.Count; i < System.Enum.GetNames(typeof(LocalizationEnum)).Length; i++)
                    {
                        characterName.Add(new LanguageGeneric<string>());
                        characterName[i].languageEnum = (LocalizationEnum)i;
                        characterName[i].LanguageGenericType = "";
                    }
                }
                // Wiêcej
                if (characterName.Count > System.Enum.GetNames(typeof(LocalizationEnum)).Length)
                {
                    for (int i = 0; i < characterName.Count - System.Enum.GetNames(typeof(LocalizationEnum)).Length; i++)
                    {
                        characterName.Remove(characterName[characterName.Count - 1]);
                    }
                }
            }
        }

        public string GetName()
        {
            LocalizationManager _manager = (LocalizationManager)Resources.Load("Languages");
            if (_manager != null)
            {
                return characterName.Find(text => text.languageEnum == _manager.SelectedLang()).LanguageGenericType;
            }
            else
            {
                return "Can't find Localization Manager in scene";
            }
        }
    }
}
