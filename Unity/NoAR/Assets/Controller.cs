// GENERATED AUTOMATICALLY FROM 'Assets/Controller.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Controller : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controller()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controller"",
    ""maps"": [
        {
            ""name"": ""ControlMap"",
            ""id"": ""4ba8b54d-aaad-49c2-8fec-421a3f31bcb2"",
            ""actions"": [
                {
                    ""name"": ""Rotate"",
                    ""type"": ""Value"",
                    ""id"": ""5281f5ac-f983-4820-ab63-2e9f76755549"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Rotate2"",
                    ""type"": ""Value"",
                    ""id"": ""f53b70c8-0822-4e46-a7cb-bedea200f2cd"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""4795e49b-df4e-4e71-9a70-8fd0fdef0b2f"",
                    ""path"": ""<Pointer>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ed6ce6f3-4a1f-4fba-af82-dd4fdc5e056f"",
                    ""path"": ""<Touchscreen>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c21b7f6c-391f-4f2a-bed0-d9947f7605a7"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""334f578d-b2dd-4a3e-b7ed-30905d54130a"",
                    ""path"": ""<AndroidGamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""de29111b-4f53-4918-b2c6-888eaaecc0c8"",
                    ""path"": ""<Joystick>/stick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e44401ab-7fca-4092-adfc-6f9d3c573c8b"",
                    ""path"": ""<AndroidJoystick>/stick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // ControlMap
        m_ControlMap = asset.FindActionMap("ControlMap", throwIfNotFound: true);
        m_ControlMap_Rotate = m_ControlMap.FindAction("Rotate", throwIfNotFound: true);
        m_ControlMap_Rotate2 = m_ControlMap.FindAction("Rotate2", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // ControlMap
    private readonly InputActionMap m_ControlMap;
    private IControlMapActions m_ControlMapActionsCallbackInterface;
    private readonly InputAction m_ControlMap_Rotate;
    private readonly InputAction m_ControlMap_Rotate2;
    public struct ControlMapActions
    {
        private @Controller m_Wrapper;
        public ControlMapActions(@Controller wrapper) { m_Wrapper = wrapper; }
        public InputAction @Rotate => m_Wrapper.m_ControlMap_Rotate;
        public InputAction @Rotate2 => m_Wrapper.m_ControlMap_Rotate2;
        public InputActionMap Get() { return m_Wrapper.m_ControlMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ControlMapActions set) { return set.Get(); }
        public void SetCallbacks(IControlMapActions instance)
        {
            if (m_Wrapper.m_ControlMapActionsCallbackInterface != null)
            {
                @Rotate.started -= m_Wrapper.m_ControlMapActionsCallbackInterface.OnRotate;
                @Rotate.performed -= m_Wrapper.m_ControlMapActionsCallbackInterface.OnRotate;
                @Rotate.canceled -= m_Wrapper.m_ControlMapActionsCallbackInterface.OnRotate;
                @Rotate2.started -= m_Wrapper.m_ControlMapActionsCallbackInterface.OnRotate2;
                @Rotate2.performed -= m_Wrapper.m_ControlMapActionsCallbackInterface.OnRotate2;
                @Rotate2.canceled -= m_Wrapper.m_ControlMapActionsCallbackInterface.OnRotate2;
            }
            m_Wrapper.m_ControlMapActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Rotate.started += instance.OnRotate;
                @Rotate.performed += instance.OnRotate;
                @Rotate.canceled += instance.OnRotate;
                @Rotate2.started += instance.OnRotate2;
                @Rotate2.performed += instance.OnRotate2;
                @Rotate2.canceled += instance.OnRotate2;
            }
        }
    }
    public ControlMapActions @ControlMap => new ControlMapActions(this);
    public interface IControlMapActions
    {
        void OnRotate(InputAction.CallbackContext context);
        void OnRotate2(InputAction.CallbackContext context);
    }
}
