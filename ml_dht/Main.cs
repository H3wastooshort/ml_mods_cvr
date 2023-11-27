using ABI.CCK.Components;
using ABI_RC.Core.Player;
using System.Reflection;
using UnityEngine;

namespace ml_dht
{
    public class DesktopHeadTracking : MelonLoader.MelonMod
    {
        static DesktopHeadTracking ms_instance = null;

        MemoryMapReader m_mapReader = null;
        byte[] m_buffer = null;
        TrackingData m_trackingData;

        HeadTracked m_localTracked = null;

        public override void OnInitializeMelon()
        {
            if(ms_instance == null)
                ms_instance = this;

            LoggerInstance.Msg("Bad Desktop Head Tracking, by SDraw, badly fixed by H3");
            LoggerInstance.Msg("F5 -> Enable ; F6 -> Disable ; F7 -> Relative Rotation (WIP) ; F8 -> Dump Data to Console");

            Settings.Init();

            m_mapReader = new MemoryMapReader();
            m_buffer = new byte[1024];

            m_mapReader.Open("head/data");

            // Patches
            HarmonyInstance.Patch(
                typeof(PlayerSetup).GetMethod(nameof(PlayerSetup.ClearAvatar)),
                null,
                new HarmonyLib.HarmonyMethod(typeof(DesktopHeadTracking).GetMethod(nameof(OnAvatarClear_Postfix), BindingFlags.Static | BindingFlags.NonPublic))
            );
            HarmonyInstance.Patch(
                typeof(PlayerSetup).GetMethod(nameof(PlayerSetup.SetupAvatar)),
                null,
                new HarmonyLib.HarmonyMethod(typeof(DesktopHeadTracking).GetMethod(nameof(OnSetupAvatar_Postfix), BindingFlags.Static | BindingFlags.NonPublic))
            );
            /*HarmonyInstance.Patch(
                typeof(CVREyeController).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic),
                null,
                new HarmonyLib.HarmonyMethod(typeof(DesktopHeadTracking).GetMethod(nameof(OnEyeControllerUpdate_Postfix), BindingFlags.Static | BindingFlags.NonPublic))
            );*/
            /*HarmonyInstance.Patch(
                typeof(CVRFaceTracking).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic),
                null,
                new HarmonyLib.HarmonyMethod(typeof(DesktopHeadTracking).GetMethod(nameof(OnFaceTrackingUpdate_Postfix), BindingFlags.Static | BindingFlags.NonPublic))
            );*/

            MelonLoader.MelonCoroutines.Start(WaitForPlayer());
        }

        System.Collections.IEnumerator WaitForPlayer()
        {
            while(PlayerSetup.Instance == null)
                yield return null;

            m_localTracked = PlayerSetup.Instance.gameObject.AddComponent<HeadTracked>();

            m_localTracked.log = LoggerInstance;

            loadSettings();
        }

        public void loadSettings()
        {
            m_localTracked.SetEnabled(true);
            m_localTracked.SetHeadTracking(true);
            m_localTracked.SetEyeTracking(Settings.EyeTracking);
            m_localTracked.SetBlinking(Settings.Blinking);
            m_localTracked.SetMirrored(Settings.Mirrored);
            m_localTracked.SetSmoothing(Settings.Smoothing);
            m_localTracked.SetFaceOverride(Settings.FaceOverride);
        }

        public override void OnDeinitializeMelon()
        {
            if(ms_instance == this)
                ms_instance = null;

            m_mapReader?.Close();
            m_mapReader = null;
            m_buffer = null;
            m_localTracked = null;
        }

        public override void OnUpdate()
        {
            if(/*Settings.Enabled &&*/ m_mapReader.Read(ref m_buffer))
            {
                m_trackingData = TrackingData.ToObject(m_buffer);
                if(m_localTracked != null)
                    m_localTracked.UpdateTrackingData(ref m_trackingData);
            }
        }

        public override void OnLateUpdate()
        { 
            if (Input.GetKeyDown(KeyCode.F5)) m_localTracked.SetEnabled(true);
            if (Input.GetKeyDown(KeyCode.F6)) m_localTracked.SetEnabled(false);
            if (Input.GetKeyDown(KeyCode.F7))
            {
                m_localTracked.rel_rotation = !m_localTracked.rel_rotation;
                LoggerInstance.Msg(m_localTracked.rel_rotation  ? "RelRot On" :"RelRot Off");
            }
            if (Input.GetKeyDown(KeyCode.F8)) m_localTracked.dump_data(LoggerInstance);
        }

        static void OnSetupAvatar_Postfix() => ms_instance?.OnSetupAvatar();
        void OnSetupAvatar()
        {
            try
            {
                if(m_localTracked != null)
                    m_localTracked.OnSetupAvatar();
            }
            catch(System.Exception e)
            {
                MelonLoader.MelonLogger.Error(e);
            }
        }

        static void OnAvatarClear_Postfix() => ms_instance?.OnAvatarClear();
        void OnAvatarClear()
        {
            try
            {
                if(m_localTracked != null)
                    m_localTracked.OnAvatarClear();
            }
            catch(System.Exception e)
            {
                MelonLoader.MelonLogger.Error(e);
            }
        }

        /*static void OnEyeControllerUpdate_Postfix(ref CVREyeController __instance) => ms_instance?.OnEyeControllerUpdate(__instance);
        void OnEyeControllerUpdate(CVREyeController p_component)
        {
            try
            {
                if(p_component.isLocal && (m_localTracked != null))
                    m_localTracked.OnEyeControllerUpdate(p_component);
            }
            catch(System.Exception e)
            {
                MelonLoader.MelonLogger.Error(e);
            }
        }
        */
        /*static void OnFaceTrackingUpdate_Postfix(ref CVRFaceTracking __instance) => ms_instance?.OnFaceTrackingUpdate(__instance);
        void OnFaceTrackingUpdate(CVRFaceTracking p_component)
        {
            try
            {
                if(p_component.isLocal && (m_localTracked != null))
                    m_localTracked.OnFaceTrackingUpdate(p_component);
            }
            catch(System.Exception e)
            {
                MelonLoader.MelonLogger.Error(e);
            }
        }*/
    }
}