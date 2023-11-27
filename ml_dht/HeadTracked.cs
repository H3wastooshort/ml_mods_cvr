using ABI.CCK.Components;
using ABI_RC.Core.Player;
using RootMotion.FinalIK;
using System.Reflection;
using UnityEngine;
using ViveSR.anipal.Lip;
using MelonLoader;
namespace ml_dht
{
    [DisallowMultipleComponent]
    class HeadTracked : MonoBehaviour
    {
        static FieldInfo ms_emotePlaying = typeof(PlayerSetup).GetField("_emotePlaying", BindingFlags.NonPublic | BindingFlags.Instance);

        bool m_enabled = false;
        bool m_headTracking = true;
        bool m_blinking = true;
        bool m_eyeTracking = true;
        float m_smoothing = 0.5f;
        bool m_mirrored = false;
        bool m_faceOverride = true;

        public bool rel_rotation = false;

        public MelonLoader.MelonLogger.Instance log;

        CVRAvatar m_avatarDescriptor = null;
        LookAtIK m_lookIK = null;
        Transform m_headBone = null;

        Vector3 m_headPosition;
        Quaternion m_headRotation;
        Vector2 m_gazeDirection;
        float m_blinkProgress = 0f;
        Vector2 m_mouthShapes;
        float m_eyebrowsProgress = 0f;

        Quaternion m_bindRotation;
        Quaternion m_lastHeadRotation;


        VRIK m_vrIK = null;

        Vector3 prevHeadPos = Vector3.zero;
        Quaternion prevHeadRot = Quaternion.identity;
        bool prevHeadValid = false;

        public void dump_data(MelonLogger.Instance logger)
        {
            logger.Msg(m_enabled ? "Mod Enabled" : "Mod Disabled");
            logger.Msg(m_headTracking ? "Head Enabled" : "Head Disabled");
            logger.Msg(string.Format("Head Pos: X{0} Y{1} Z{2}", m_headPosition.x, m_headPosition.y, m_headPosition.z));
            logger.Msg(string.Format("Head Rot: X{0} Y{1} Z{2} W{3}", m_headRotation.x, m_headRotation.y, m_headRotation.z, m_headRotation.w));
        }

        internal void OnIKPreUpdate()
        {

            if (m_vrIK != null && m_headTracking && m_enabled) {
                //prevHeadPos = m_vrIK.solver.spine.headTarget.transform.localPosition;


                prevHeadRot = m_vrIK.solver.spine.headTarget.transform.localRotation;
                prevHeadValid = true;

                //m_vrIK.solver.spine.headTarget.transform.localPosition = m_headPosition;
                m_vrIK.solver.spine.headTarget.transform.localRotation = m_headRotation.normalized;


                if (rel_rotation)
                {
                    Transform l_camera = PlayerSetup.Instance.GetActiveCamera().transform;
                    Vector3 normrot = m_vrIK.solver.spine.headTarget.transform.rotation.normalized.eulerAngles;
                    Vector3 camrot = l_camera.rotation.normalized.eulerAngles;
                    normrot.y += camrot.y;
                    m_vrIK.solver.spine.headTarget.transform.eulerAngles=normrot;
                }
            }
        }

        internal void OnIKPostUpdate()
        {
            if (m_vrIK != null && prevHeadValid)
            {
                //m_vrIK.solver.spine.headTarget.transform.localPosition = prevHeadPos;
                m_vrIK.solver.spine.headTarget.transform.localRotation = prevHeadRot;
                prevHeadValid = false;
            }
        }

        // Unity events
        void Start()
        {
            Settings.EnabledChange += this.SetEnabled;
            Settings.HeadTrackingChange += this.SetHeadTracking;
            Settings.EyeTrackingChange += this.SetEyeTracking;
            Settings.BlinkingChange += this.SetBlinking;
            Settings.SmoothingChange += this.SetSmoothing;
            Settings.MirroredChange += this.SetMirrored;
            Settings.FaceOverrideChange += this.SetFaceOverride;
        }

        void OnDestroy()
        {
            Settings.EnabledChange -= this.SetEnabled;
            Settings.HeadTrackingChange -= this.SetHeadTracking;
            Settings.EyeTrackingChange -= this.SetEyeTracking;
            Settings.BlinkingChange -= this.SetBlinking;
            Settings.SmoothingChange -= this.SetSmoothing;
            Settings.MirroredChange -= this.SetMirrored;
            Settings.FaceOverrideChange -= this.SetFaceOverride;
        }

        // Tracking updates
        public void UpdateTrackingData(ref TrackingData p_data)
        {
            m_headPosition.Set(p_data.m_headPositionX * (m_mirrored ? -1f : 1f), p_data.m_headPositionY, p_data.m_headPositionZ);
            m_headRotation.Set(p_data.m_headRotationX, p_data.m_headRotationY * (m_mirrored ? -1f : 1f), p_data.m_headRotationZ * (m_mirrored ? -1f : 1f), p_data.m_headRotationW);
            m_gazeDirection.Set(m_mirrored ? (1f - p_data.m_gazeX) : p_data.m_gazeX, p_data.m_gazeY);
            m_blinkProgress = p_data.m_blink;
            m_mouthShapes.Set(p_data.m_mouthOpen, p_data.m_mouthShape);
            m_eyebrowsProgress = p_data.m_brows;
        }

        void OnLookIKPostUpdate()
        {
            if(m_enabled && m_headTracking && (m_headBone != null))
            {
                m_lastHeadRotation = Quaternion.Slerp(m_lastHeadRotation, m_avatarDescriptor.transform.rotation * (m_headRotation * m_bindRotation), m_smoothing);

                if(!(bool)ms_emotePlaying.GetValue(PlayerSetup.Instance))
                    m_headBone.rotation = m_lastHeadRotation;
            }
        }

        // Game events
        /*internal void OnEyeControllerUpdate(CVREyeController p_component)
        {
            if(m_enabled)
            {
                
                // Gaze
                if(m_eyeTracking)
                {
                    Transform l_camera = PlayerSetup.Instance.GetActiveCamera().transform;

                    p_component.manualViewTarget = true;
                    p_component.targetViewPosition = l_camera.position + l_camera.rotation * new Vector3((m_gazeDirection.x - 0.5f) * 2f, (m_gazeDirection.y - 0.5f) * 2f, 1f);
                }

                // Blink
                if(m_blinking)
                {
                    p_component.manualBlinking = true;
                    p_component.blinkProgress = m_blinkProgress;
                }
            }
        }*/

        internal void OnFaceTrackingUpdate(CVRFaceTracking p_component)
        {
            if (m_enabled && m_faceOverride)
            {
                if(m_avatarDescriptor != null)
                    m_avatarDescriptor.useVisemeLipsync = false;

                float l_weight = Mathf.Clamp01(Mathf.InverseLerp(0.25f, 1f, Mathf.Abs(m_mouthShapes.y))) * 100f;

                p_component.BlendShapeValues[(int)LipShape_v2.Jaw_Open] = m_mouthShapes.x * 100f;
                p_component.BlendShapeValues[(int)LipShape_v2.Mouth_Pout] = ((m_mouthShapes.y > 0f) ? l_weight : 0f);
                p_component.BlendShapeValues[(int)LipShape_v2.Mouth_Smile_Left] = ((m_mouthShapes.y < 0f) ? l_weight : 0f);
                p_component.BlendShapeValues[(int)LipShape_v2.Mouth_Smile_Right] = ((m_mouthShapes.y < 0f) ? l_weight : 0f);
                p_component.LipSyncWasUpdated = true;
                //p_component.UpdateLipShapes();
            }
        }

        internal void OnSetupAvatar()
        {

            m_vrIK = PlayerSetup.Instance._animator.GetComponent<VRIK>();
            m_vrIK.solver.OnPreUpdate += this.OnIKPreUpdate;
            m_vrIK.solver.OnPostUpdate+= this.OnIKPostUpdate;

            m_avatarDescriptor = PlayerSetup.Instance._avatar.GetComponent<CVRAvatar>();
            m_headBone = PlayerSetup.Instance._animator.GetBoneTransform(HumanBodyBones.Head);
            m_lookIK = PlayerSetup.Instance._avatar.GetComponent<LookAtIK>();

            if(m_headBone != null)
                m_bindRotation = (m_avatarDescriptor.transform.GetMatrix().inverse * m_headBone.GetMatrix()).rotation;

            if(m_lookIK != null)
                m_lookIK.solver.OnPostUpdate += this.OnLookIKPostUpdate;

        }
        internal void OnAvatarClear()
        {
            m_vrIK = null;
            m_avatarDescriptor = null;
            m_lookIK = null;
            m_headBone = null;
            m_lastHeadRotation = Quaternion.identity;
            m_bindRotation = Quaternion.identity;
        }

        // Settings
        internal void SetEnabled(bool p_state)
        {
            log.Msg(m_enabled ? "Was Enabled" : "Was Disabled");

            if (m_enabled != p_state)
            {
                m_enabled = p_state;
                if(m_enabled && m_headTracking)
                    m_lastHeadRotation = ((m_headBone != null) ? m_headBone.rotation : m_bindRotation);
            }

            log.Msg(m_enabled ? "Now Enabled" : "Now Disabled");
        }
        internal void SetHeadTracking(bool p_state)
        {
            if(m_headTracking != p_state)
            {
                m_headTracking = p_state;
                if(m_enabled && m_headTracking)
                    m_lastHeadRotation = ((m_headBone != null) ? m_headBone.rotation : m_bindRotation);
            }
        }
        internal void SetEyeTracking(bool p_state)
        {
            m_eyeTracking = p_state;
        }
        internal void SetBlinking(bool p_state)
        {
            m_blinking = p_state;
        }
        internal void SetSmoothing(float p_value)
        {
            m_smoothing = 1f - Mathf.Clamp(p_value, 0f, 0.99f);
        }
        internal void SetMirrored(bool p_state)
        {
            m_mirrored = p_state;
        }
        internal void SetFaceOverride(bool p_state)
        {
            m_faceOverride = p_state;
        }
    }
}
