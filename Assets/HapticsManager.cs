// Ignore Spelling: Haptics haptic

using Autohand;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;


namespace Europa
{
    public enum HandEnum
    {
        LeftHand,
        RightHand,
        BothHands,
    }

    public class HapticsManager : Singleton<HapticsManager>
    {
        [SerializeField] private HandEnum hand;
        [SerializeField] private float duration;
        [SerializeField] private float amp;

        [SerializeField] private InputActionProperty leftHapticAction;
        [SerializeField] private InputActionProperty rightKapticAction;

        [SerializeField] UnityEngine.XR.InputDevice leftController;
        [SerializeField] UnityEngine.XR.InputDevice rightController;

        private void Start()
        {
            leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        }

        public void PlayHaptic(HandEnum _hand, float _hapticDuration = 1f, float _hapticAmp = 0.5f)
        {
            
            switch (_hand)
            {
                case HandEnum.LeftHand:
                    
                    leftController.SendHapticImpulse(0, 0.5f, 1.0f); // channel, amplitude, duration
                    break;

                case HandEnum.RightHand:
                    
                    rightController.SendHapticImpulse(0, 0.5f, 1.0f); // channel, amplitude, duration
                    break;

                default:
                    
                    leftController.SendHapticImpulse(0, 0.5f, 1.0f); // channel, amplitude, duration
                    rightController.SendHapticImpulse(0, 0.5f, 1.0f); // channel, amplitude, duration
                    break;

            }
        }
        [ContextMenu("PlayHaptics")]
        public void TestHaptics()
        {
            print("PlayHaptics1");
            PlayHaptic(HandEnum.BothHands, duration, amp);
        }


    }
}
