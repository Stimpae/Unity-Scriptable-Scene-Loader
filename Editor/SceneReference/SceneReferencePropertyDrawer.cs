using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Pastime.SceneLoader.Editor {
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferenceDrawer : PropertyDrawer {
        private enum SceneBuildStatus {
            VALID,
            MISSING,
            DISABLED
        }

        private SceneBuildStatus m_sceneBuildStatus = SceneBuildStatus.VALID;
     
        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            var sceneAssetProp = property.FindPropertyRelative("sceneAsset");
            var scenePathProp = property.FindPropertyRelative("scenePath");

            var propertyContainer = new VisualElement();
            
            var objectField = new ObjectField(property.displayName) {
                objectType = typeof(SceneAsset),
                value = sceneAssetProp.objectReferenceValue,
                bindingPath = sceneAssetProp.propertyPath,
                allowSceneObjects = false,
                style = { flexGrow = 1 }
            };
            
            objectField.AddToClassList("unity-base-field__aligned");
            objectField.Q<Label>().AddToClassList("unity-property-field__label");
            objectField.Q<VisualElement>(className: "unity-base-field__input").AddToClassList("unity-property-field__input");
            propertyContainer.Add(objectField);

            var settingsButton = CreateSettingsButton(scenePathProp);
            propertyContainer.Add(settingsButton);

            var propertyFieldInput = objectField.Q<VisualElement>(className: "unity-base-field__input");
            propertyContainer.schedule.Execute(() => {
                    HandleSceneBuildStatus(scenePathProp.stringValue, propertyFieldInput, settingsButton);
                    settingsButton.tooltip = GetStatusText(m_sceneBuildStatus);
            }).Every(300);
            
            
            return propertyContainer;
        }
        
        private Button CreateSettingsButton(SerializedProperty scenePathProp) {
            var button = new Button {
                iconImage = EditorGUIUtility.IconContent("d__Popup@2x").image as Texture2D,
                text = "Open Scene Settings",
                style = { marginLeft = 3, marginRight = 0 }
            };

            button.clicked += () => {
                var menu = new GenericDropdownMenu();
                menu.contentContainer.Clear();
                menu.AddItem("Open Build Player Window", false,
                    () => { EditorWindow.GetWindow(Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor")); });

                if (!IsSceneInBuildSettings(scenePathProp.stringValue)) {
                    menu.AddItem("Add to Build Settings", false,
                        () => { AddSceneToBuildSettings(scenePathProp.stringValue); });
                }
                else if (!IsSceneEnabledInBuildSettings(scenePathProp.stringValue)) {
                    menu.AddItem("Enable in Build Settings", false,
                        () => { EnableSceneInBuildSettings(scenePathProp.stringValue); });
                }

                menu.DropDown(button.worldBound, button, false);
            };

            return button;
        }

        private void AddSceneToBuildSettings(string path) {
            var scenes = EditorBuildSettings.scenes.ToList();
            if (scenes.All(scene => scene.path != path)) {
                scenes.Add(new EditorBuildSettingsScene(path, true));
                EditorBuildSettings.scenes = scenes.ToArray();
            }
        }

        private void EnableSceneInBuildSettings(string path) {
            var scenes = EditorBuildSettings.scenes;
            foreach (var scene in scenes) {
                if (scene.path == path) {
                    scene.enabled = true;
                    EditorBuildSettings.scenes = scenes;
                    break;
                }
            }
        }

        private void HandleSceneBuildStatus(string path, VisualElement sceneField, VisualElement settingsButton) {
            bool isValid = IsSceneInBuildSettings(path);
            bool isEnabled = IsSceneEnabledInBuildSettings(path);

            if (!isValid) SetSceneFieldStatus(sceneField, new Color(0.6f, 0, 0), SceneBuildStatus.MISSING, DisplayStyle.Flex, settingsButton);
            else if (!isEnabled) SetSceneFieldStatus(sceneField, new Color(0.8f, 0.4f, 0), SceneBuildStatus.DISABLED, DisplayStyle.Flex, settingsButton);
            else SetSceneFieldStatus(sceneField, new StyleColor(new Color32(40, 40, 40, 255)), SceneBuildStatus.VALID, DisplayStyle.None, settingsButton);
        }

        private void SetSceneFieldStatus(VisualElement sceneField, StyleColor color, SceneBuildStatus status,
            DisplayStyle buttonDisplay, VisualElement settingsButton) {
            sceneField.style.backgroundColor = color;
            m_sceneBuildStatus = status;
            settingsButton.style.display = buttonDisplay;
        }

        private string GetStatusText(SceneBuildStatus buildStatus) {
            return buildStatus switch {
                SceneBuildStatus.VALID => "Scene is in build settings",
                SceneBuildStatus.MISSING => "Scene is missing from build settings",
                SceneBuildStatus.DISABLED => "Scene is in build settings but disabled",
                _ => "Unknown status"
            };
        }

        private bool IsSceneInBuildSettings(string path) {
            return EditorBuildSettings.scenes.Any(scene => scene.path == path);
        }

        private bool IsSceneEnabledInBuildSettings(string path) {
            return EditorBuildSettings.scenes.Any(scene => scene.path == path && scene.enabled);
        }
    }
}