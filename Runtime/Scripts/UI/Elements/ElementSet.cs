using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.UI
{
    public class ElementSet : MonoBehaviour
    {
        [SerializeField] private Element element;
        [SerializeField] private Transform root;

        public int Size => spawnedElements.Count;

        private readonly List<Element> spawnedElements = new List<Element>();

        private void SetupElement<T>(Element element, T arg)
        {
            element.Init();

            var elementFor = element as ElementFor<T>;

            if (elementFor)
            {
                elementFor.Setup(arg);
                elementFor.Show();
            }
            else
            {
                elementFor.Hide();
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