using System;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.UI.Elements
{
    public class ElementSet : MonoBehaviour
    {
        [SerializeField] protected Element element;
        [SerializeField] protected Transform content;

        protected readonly List<Element> spawnedElements = new List<Element>();

        public int Size => spawnedElements.Count;
        
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

        public void Clear()
        {
            for (int i = 0; i < spawnedElements.Count; i++)
            {
                spawnedElements[i].Hide();
            }
        }

        public virtual void Setup<TE, T>(IEnumerable<T> objects) where TE : Element
        {
            element.Hide();
            Clear();
            int i = 0;
            
            foreach (var o in objects)
            {
                Element e;
                if (i < spawnedElements.Count)
                {
                    e = spawnedElements[i];
                }
                else
                {
                    e = Instantiate(element, content);
                    e.Init();
                    spawnedElements.Add(e);
                }
                
                if (e is ElementFor<T> elementFor)
                {
                    elementFor.Setup(o);
                }
                
                e.Show();
                i++;
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