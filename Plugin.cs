using BepInEx;
using GorillaNetworking;
using HarmonyLib;
using UnityEngine;
using Valve.VR;

namespace ForeverPreds
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin instance;
        public float prediction = 4f;
        public bool GripDisable;
        public bool pullEnabled = true;
        public float pullPower = 4f;
        void Start()
        {
            instance = this;
            HarmonyPatches.ApplyHarmonyPatches();
        }

        private GUIStyle panelStyle;
        private GUIStyle creditStyle;
        private Texture2D panelTex;

        void EnsureStyles()
        {
            if (panelStyle != null)
                return;

            panelTex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            panelTex.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.65f));
            panelTex.Apply();
            panelTex.hideFlags = HideFlags.HideAndDontSave;

            panelStyle = new GUIStyle(GUI.skin.box);
            panelStyle.normal.background = panelTex;
            panelStyle.margin = new RectOffset(0, 0, 0, 0);
            panelStyle.padding = new RectOffset(0, 0, 0, 0);

            creditStyle = new GUIStyle(GUI.skin.label);
            creditStyle.fontSize = 11;
            creditStyle.normal.textColor = new Color(1f, 1f, 1f, 0.75f);
        }

        void OnGUI()
        {
            EnsureStyles();

            GUI.Box(new Rect(4f, 4f, 480f, 156f), GUIContent.none, panelStyle);

            GUI.Label(new Rect(14f, 8f, 52f, 28f), "Preds:");
            prediction = GUI.HorizontalSlider(new Rect(70f, 8f, 310f, 28f), prediction, 0f, 250f);
            GUI.Label(new Rect(388f, 8f, 78f, 28f), prediction.ToString("0.#"));

            GUI.Label(new Rect(14f, 42f, 52f, 28f), "Pull:");
            pullPower = GUI.HorizontalSlider(new Rect(70f, 42f, 310f, 28f), pullPower, 0f, 250f);
            GUI.Label(new Rect(388f, 42f, 78f, 28f), pullPower.ToString("0.#"));

            pullEnabled = GUI.Toggle(new Rect(14f, 80f, 196f, 24f), pullEnabled, "Pull (hold right grip)");
            GripDisable = GUI.Toggle(new Rect(214f, 80f, 252f, 24f), GripDisable, "Right Grip to Disable");

            GUI.Label(new Rect(14f, 106f, 452f, 22f), "Prediction slider by @goldentrophy (Fixed By AltA)", creditStyle);
            GUI.Label(new Rect(14f, 126f, 452f, 22f), "(Added Features by OmalleyGT)", creditStyle);
        }

        private bool IsSteam = true;
        private bool hasSteamChecked;

        public bool GetJoystickDown()
        {
            if (!hasSteamChecked)
            {
                IsSteam = Traverse.Create(PlayFabAuthenticator.instance).Field("platform").GetValue().ToString().ToLower() == "steam";
                hasSteamChecked = true;
            }

            bool rightJoystickClick = false;
            if (IsSteam)
                rightJoystickClick = SteamVR_Actions.gorillaTag_RightJoystickClick.GetState(SteamVR_Input_Sources.RightHand);
            else
                ControllerInputPoller.instance.rightControllerDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out rightJoystickClick);

            return rightJoystickClick;
        }
    }
}

