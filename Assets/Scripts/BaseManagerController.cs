using Assets.Scripts.Enums;
using Assets.Scripts.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public abstract class BaseManagerController<T> : MonoBehaviour where T : BaseData, new()
    {
        protected InputField NameField { get; set; }
        protected InputField EffectTextField { get; set; }
        protected InputField FlavourTextField { get; set; }
        protected Dropdown EditDropdown { get; set; }

        public virtual void Start()
        {
            NameField = GameObject.Find("Canvas/NameField").GetComponent<InputField>();
            EffectTextField = GameObject.Find("Canvas/EffectTextField").GetComponent<InputField>();
            FlavourTextField = GameObject.Find("Canvas/FlavourTextField").GetComponent<InputField>();

            // Populate edit dropdown and attach delegate
            EditDropdown = GameObject.Find("Canvas/EditDropdown").GetComponent<Dropdown>();
            EditDropdown.AddOptions(GetExistingNames());
            EditDropdown.onValueChanged.AddListener(x => PopulateWithExisting(x - 1));

            // Register button
            GameObject.Find("Canvas/SubmitButton").GetComponent<Button>()
                .onClick.AddListener(() => Submit(new T()));
        }

        protected List<string> GetEnumNames(Type enumType)
        {
            if (enumType == typeof(Colour))
            {
                return new List<string>()
                {
                    "None",
                    "Red",
                    "Blue",
                    "Green",
                    "Purple",
                    "Prismatic"
                };
            }

            if (enumType == typeof(CardType))
            {
                return new List<string>()
                {
                    "None",
                    "Creature",
                    "Action",
                    "Permanent"
                };
            }

            return Enum.GetNames(enumType).ToList();
        }

        protected List<string> GetRangeText(int min, int max)
        {
            return Enumerable.Range(0, max - min + 1).Select(x => x.ToString()).ToList();
        }

        private List<string> GetExistingNames()
        {
            var existingNames = DataIO.ReadAll<T>()
                .OrderBy(x => x.Id)
                .Select(x => $"{x.Name} - {x.Id}")
                .ToList();

            existingNames.Insert(0, string.Empty);

            return existingNames;
        }

        protected virtual void Submit(T data)
        {
            // Retrieve or create id
            var selectedExisting = EditDropdown.captionText.text;

            int id;
            if (selectedExisting == string.Empty)
            {
                var existingCards = DataIO.ReadAll<T>();
                id = existingCards.Max(x => x.Id) + 1;
            }
            else
            {
                id = int.Parse(selectedExisting[^2..]);
            }

            // Retrieve the base properties
            data.Id = id;
            data.Name = NameField.text;
            data.EffectText = EffectTextField.text;
            data.FlavourText = FlavourTextField.text;

            // Write
            DataIO.Write(data);

            // Clear and prepare UI
            EditDropdown.ClearOptions();
            EditDropdown.AddOptions(GetExistingNames());
            PopulateComponents(new T());
        }

        // Populate the dropdowns and fields with the data of selected id
        private void PopulateWithExisting(int id)
        {
            if (id == -1)
            {
                PopulateComponents(new T());
                return;
            }

            var existing = DataIO.Read<T>(id);
            PopulateComponents(existing);
        }

        // Populate UI with base fields
        protected virtual void PopulateComponents(T data)
        {
            NameField.text = data.Name;
            EffectTextField.text = data.EffectText;
            FlavourTextField.text = data.FlavourText;
        }
    }
}
