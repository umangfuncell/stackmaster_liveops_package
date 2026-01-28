using UnityEngine;
using System.Collections.Generic;
using FunCell.GridMerge.GamePlay;

namespace FunCell.GridMerge.UI
{
    public class PanelManager : MonoBehaviour
    {
        public static PanelManager Instance;

        [SerializeField] private List<Panel> _panels;
        private Stack<Panel> _panelStack;

        public void Init()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            //collecting panels
            _panels = new List<Panel>();
            for (int i = 0; i < transform.childCount; i++)
            {
                Panel panel = transform.GetChild(i).GetComponent<Panel>();
                if (panel != null)
                {
                    panel.gameObject.SetActive(false);
                    _panels.Add(panel);
                }
            }

            _panelStack = new Stack<Panel>();

            Open<MenuPanel>();
        }
        public void Open<T>() where T : Panel
        {
            Panel target = GetPanel<T>();
            if (_panelStack.Count > 0)
            {
                _panelStack.Pop().Close();
            }
            _panelStack.Push(target);
            target.Open();
        }
        public void Close()
        {
            if (_panelStack.Count > 0)
            {
                _panelStack.Pop().Close();
            }
        }
        private Panel GetPanel<T>() where T : Panel
        {
            Panel target = null;
            foreach (var panel in _panels)
            {
                if (panel.GetType() == typeof(T))
                {
                    target = panel;
                    break;
                }
            }
            return target;
        }
    }
}

