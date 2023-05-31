using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using DewDrop.Utilities;

namespace DewDrop
{
    public class Input
    {
        public static Input Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Controls if events and key monitoring is enabled.
        /// </summary>
        public static bool RecieveInput
        {
            get => Instance._recieveInput;
            set => Instance._recieveInput = value;
        }
        
        private Dictionary<Keyboard.Key, bool> _keyStatuses;
        private bool _recieveInput = true;
        private ControllerType _controllerType = ControllerType.Keyboard;
        
        public event EventHandler<Keyboard.Key> OnKeyPressed;
        public event EventHandler<Keyboard.Key> OnKeyReleased;
        
        public event EventHandler<DButtons> OnButtonPressed;
        public event EventHandler<DButtons> OnButtonReleased;

        public bool this[Keyboard.Key key]
        {
            get => _keyStatuses[key];
            set => _keyStatuses[key] = value;
        }

        public Dictionary<Keyboard.Key, DButtons> SFMLtoDButtonsMap = new()
        {
            {Keyboard.Key.E, DButtons.Select},
            {Keyboard.Key.Escape, DButtons.Back},
        };
        
        //https://en.sfml-dev.org/forums/index.php?topic=16748.msg120279#msg120279
        public Dictionary<uint, DButtons> ControllertoDButtonsMap = new()
        {
            {0, DButtons.Select},
            {1, DButtons.Back},
        };
        
        public Input()
        {
            if (Instance != null)
            {
                throw new Exception("Input already exists!");
            }
            
            Debug.Log("hey");
            _keyStatuses = new Dictionary<Keyboard.Key, bool>();
            // add keys
            foreach (Keyboard.Key key in Enum.GetValues(typeof (Keyboard.Key)))
                // for some reason, semicolons throw a duplicate key exception
                // this is to prevent that
                if (!_keyStatuses.ContainsKey(key)) 
                    _keyStatuses.Add(key, false);

            Instance = this;
        }
        
        /// <summary>
        /// Hooks into the window and allows for input to be read.
        /// </summary>
        /// <param name="window">The window to read input to.</param>
        public void AttachToWindow(Window window)
        {
            window.SetKeyRepeatEnabled(false);
            window.KeyPressed += WindowOnKeyPressed;
            window.KeyReleased += WindowOnKeyReleased;
            window.JoystickConnected += WindowOnJoystickConnected;
            window.JoystickDisconnected += WindowOnJoystickDisconnected;
        }

        private void WindowOnJoystickDisconnected(object? sender, JoystickConnectEventArgs e)
        {
            _controllerType = ControllerType.Keyboard;
            Debug.Log("disconnected");
        }

        private void WindowOnJoystickConnected(object? sender, JoystickConnectEventArgs e)
        {
            Joystick.Identification identification = Joystick.GetIdentification(e.JoystickId);
            Debug.Log(identification.Name);
            if (identification.Name.Contains("XBOX"))
            {
                _controllerType = ControllerType.Xbox360;
            }
        }

        /// <summary>
        /// Detaches from the window and stops reading input.
        /// </summary>
        /// <param name="window">The window to detach from.</param>
        public void DetachFromWindow(Window window)
        {
            window.KeyPressed -= WindowOnKeyPressed;
            window.KeyReleased -= WindowOnKeyReleased;
            window.JoystickConnected -= WindowOnJoystickConnected;
            window.JoystickDisconnected -= WindowOnJoystickDisconnected; 
        }
        
        private void WindowOnKeyReleased(object? sender, KeyEventArgs e)
        {
            if (!_recieveInput)
                return;
            OnKeyReleased?.Invoke(this, e.Code);
            _keyStatuses[e.Code] = false;
            
            if (SFMLtoDButtonsMap.ContainsKey(e.Code))
            {
                OnButtonReleased?.Invoke(this, SFMLtoDButtonsMap[e.Code]);
            }
        }

        private void WindowOnKeyPressed(object? sender, KeyEventArgs e)
        {
            if (!_recieveInput) 
                return;
            OnKeyPressed?.Invoke(this, e.Code);
            _keyStatuses[e.Code] = true;
            
            if (SFMLtoDButtonsMap.ContainsKey(e.Code))
            {
                OnButtonPressed?.Invoke(this, SFMLtoDButtonsMap[e.Code]);
            }
            
        }
    }
}