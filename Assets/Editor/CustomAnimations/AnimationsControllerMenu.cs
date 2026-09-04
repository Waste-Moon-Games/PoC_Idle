using UI.Common.Components;
using UnityEditor;
using UnityEngine;

namespace CustomAnimations
{
    public static class AnimationsControllerMenu
    {
        [MenuItem("GameObject/Custom Animations/Animations Controller", false, 10)]
        public static void CreateAnimationController()
        {
            GameObject controller = new("[ACTION_BUTTONS_ANIMATION_SERVICE]");

            controller.AddComponent<ActionButtonsAnimationsService>();

            Undo.RegisterCreatedObjectUndo(controller, "Create Animations Controller");

            Selection.activeGameObject = controller;
        }
    }
}