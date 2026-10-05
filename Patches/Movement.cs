using GorillaLocomotion.Climbing;
using HarmonyLib;
using UnityEngine;

namespace ForeverPreds.Patches
{
    [HarmonyPatch(typeof(GorillaLocomotion.GTPlayer), "LateUpdate")]
    internal class ExamplePatch
    {
        public static void Prefix(GorillaLocomotion.GTPlayer __instance)
        {
            if (lvT == null || rvT == null)
                CreateVelocityTrackers();

            if (pullLT == null || pullRT == null)
                CreatePullTrackers();

            bool gripHeld = ControllerInputPoller.instance.rightGrab;

            if (gripHeld && Plugin.instance.GripDisable)
                ControllerInputPoller.instance.rightControllerGripFloat = Plugin.instance.GetJoystickDown() ? 1f : 0f;

            if (!(gripHeld && Plugin.instance.GripDisable))
                VelocityLongArms(Plugin.instance.prediction);

            if (gripHeld && Plugin.instance.pullEnabled)
                PullHands(Plugin.instance.pullPower);
        }

        public static GameObject lvT = null;
        public static GameObject rvT = null;
        public static void CreateVelocityTrackers()
        {
            lvT = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(lvT.GetComponent<BoxCollider>());
            Object.Destroy(lvT.GetComponent<Rigidbody>());
            lvT.GetComponent<Renderer>().enabled = false;
            lvT.AddComponent<GorillaVelocityTracker>();

            rvT = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(rvT.GetComponent<BoxCollider>());
            Object.Destroy(rvT.GetComponent<Rigidbody>());
            rvT.GetComponent<Renderer>().enabled = false;
            rvT.AddComponent<GorillaVelocityTracker>();
        }

        public static void DestroyVelocityTrackers()
        {
            Debug.Log(lvT);
            Debug.Log(rvT);
        }

        public static GameObject pullLT = null;
        public static GameObject pullRT = null;
        public static void CreatePullTrackers()
        {
            pullLT = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(pullLT.GetComponent<BoxCollider>());
            Object.Destroy(pullLT.GetComponent<Rigidbody>());
            pullLT.GetComponent<Renderer>().enabled = false;
            pullLT.AddComponent<GorillaVelocityTracker>();

            pullRT = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(pullRT.GetComponent<BoxCollider>());
            Object.Destroy(pullRT.GetComponent<Rigidbody>());
            pullRT.GetComponent<Renderer>().enabled = false;
            pullRT.AddComponent<GorillaVelocityTracker>();

            pullLT.transform.position = GorillaTagger.Instance.leftHandTransform.position;
            pullRT.transform.position = GorillaTagger.Instance.rightHandTransform.position;
        }

        public static void PullHands(float power)
        {
            pullLT.transform.position = GorillaTagger.Instance.leftHandTransform.position;
            pullRT.transform.position = GorillaTagger.Instance.rightHandTransform.position;

            GorillaLocomotion.GTPlayer.Instance.LeftHand.controllerTransform.position += pullLT.GetComponent<GorillaVelocityTracker>().GetAverageVelocity(true, 0) * (power / 100f);
            GorillaLocomotion.GTPlayer.Instance.RightHand.controllerTransform.position += pullRT.GetComponent<GorillaVelocityTracker>().GetAverageVelocity(true, 0) * (power / 100f);
        }

        public static void VelocityLongArms(float power)
        {
            lvT.transform.position = GorillaTagger.Instance.headCollider.transform.position - GorillaTagger.Instance.leftHandTransform.position;
            rvT.transform.position = GorillaTagger.Instance.headCollider.transform.position - GorillaTagger.Instance.rightHandTransform.position;
            GorillaLocomotion.GTPlayer.Instance.LeftHand.controllerTransform.position -= lvT.GetComponent<GorillaVelocityTracker>().GetAverageVelocity(true, 0) * (power / 333f);
            GorillaLocomotion.GTPlayer.Instance.RightHand.controllerTransform.position -= rvT.GetComponent<GorillaVelocityTracker>().GetAverageVelocity(true, 0) * (power / 333f);
        }
    }
}
