using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Editor.Engine
{
    internal class InputController
    {
        private static readonly Lazy<InputController> lazy = new(() => new InputController());
        public static InputController Instance { get { return lazy.Value; } }

        private Dictionary<Keys, bool> m_keyStates = new();
        private Dictionary<MouseButtons, bool> m_ButtonState = new();

        private InputController()
        {
            foreach(Keys key in Enum.GetValues(typeof(Keys)))
            {
                if(m_keyStates.ContainsKey(key)) continue;
                m_keyStates.Add(key, false);
            }

            foreach(MouseButtons button in Enum.GetValues(typeof(MouseButtons)))
            {
                if (m_ButtonState.ContainsKey(button)) continue;
                m_ButtonState.Add(button, false);
            }
        }

        public void SetKeyDown(Keys _key)
        {
            m_keyStates[_key] = true;
        }

        public void SetKeyUp(Keys _key)
        {
            m_keyStates[_key] = false;
        }

        public bool IsKeyDown(Keys _key)
        {
            return m_keyStates[_key];
        }

        public void SetButtonDown(MouseButtons _button)
        {
            m_ButtonState[_button] = true;
        }

        public void SetButtonUp(MouseButtons _button)
        {
            m_ButtonState[_button] = false;
        }

        public bool IsButtonDown(MouseButtons _button)
        {
            return m_ButtonState[_button];
        }

        public override string ToString()
        {
            string s = "Keys Down: ";
            foreach(var key in m_keyStates)
            {
                if (key.Value == true)
                {
                    s += key.Key + " ";
                }
            }

            s += "\nMouse Buttons Down: ";
            foreach (var button in m_ButtonState)
            {
                if(button.Value == true)
                {
                    s += button.Key + " ";
                }
            }
            return s;
        }
    }
}
