using System;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.UI.Elements
{
    public class ElementSet : MonoBehaviour
    {
        [SerializeField] private Element element;
        [SerializeField] private Transform root;

        private readonly List<Element> spawnedElements = new List<Element>();

        public int Size => spawnedElements.Count;

        private void SetupElement<T>(Element element, T arg)
        {
            element.Init();
            
            if (element is ElementFor<T> elementFor)
            {
                elementFor.Setup(arg);
                elementFor.Show();
            }
        }

        public Element GetElement(int index)
        {
            if (spawnedElements.Count <= index)
            {
                Debug.LogError($"Element {index} is out of bounds");
                return null;
            }

            return spawnedElements[index];
        }

        public List<Element> CollectElements(Predicate<Element> predicate = null)
        {
            if (predicate == null)
                return new List<Element>(spawnedElements);

            var collected = new List<Element>();

            foreach (var e in spawnedElements)
            {
                if (predicate(e))
                {
                    collected.Add(e);
                }
            }

            return collected;
        }

        public void Setup<T>(T[] args)
        {
            element.Hide();

            int reusedElements = 0;

            for (int i = 0; i < spawnedElements.Count; i++)
            {
                if (args.Length <= i)
                {
                    spawnedElements[i].Hide();
                }
                else
                {
                    SetupElement(spawnedElements[i], args[i]);
                    reusedElements++;
                }
            }

            for (int i = reusedElements; i < args.Length; i++)
            {
                Element instance = Instantiate(element, root);

                SetupElement(instance, args[i]);

                spawnedElements.Add(instance);
            }
        }

        public void Setup<T>(List<T> args)
        {
            element.Hide();

            int reusedElements = 0;

            for (int i = 0; i < spawnedElements.Count; i++)
            {
                if (args.Count <= i)
                {
                    spawnedElements[i].Hide();
                }
                else
                {
                    SetupElement(spawnedElements[i], args[i]);
                    reusedElements++;
                }
            }

            for (int i = reusedElements; i < args.Count; i++)
            {
                Element instance = Instantiate(element, root);

                SetupElement(instance, args[i]);

                spawnedElements.Add(instance);
            }
        }

        private void Update()
        {
            foreach (var e in spawnedElements)
            {
                e.Process(Time.deltaTime);
            }
        }
    }
}