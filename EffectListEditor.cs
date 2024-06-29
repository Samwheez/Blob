using BlobBuddies.Runtime;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;

namespace BlobBuddies.Editor
{
    [CustomPropertyDrawer(typeof(EffectList))]
    public class EffectListEditor : PropertyDrawer
    {
        private const float FOLDOUT_HEIGHT = 16f;

        private SerializedProperty list;

        
        // Effects must be added to this in order for this to work.  5 bucks to whoever finds a way around this.
        private void PopulateAddMenu(GenericMenu contextMenu)
        {
            AddEffectToAddMenu(contextMenu, "Damage", 0);
            AddEffectToAddMenu(contextMenu, "Spawn Targeted Projectile", 1);
            AddEffectToAddMenu(contextMenu, "Dash To Target", 2);
            AddEffectToAddMenu(contextMenu, "Push Target Away From Origin", 3);
        }
        private void CreateEffect(object effectID)
        {
            switch ((ushort)effectID)
            {
                case 0:
                    AddEffect(new DamageEffect());
                    break;
                case 1:
                    AddEffect(new SpawnTargetedProjectileEffect());
                    break;
                case 2:
                    AddEffect(new DashToTargetEffect());
                    break;
                case 3:
                    AddEffect(new PushTargetAwayFromOriginEffect());
                    break;
            }
        }




        private void AddEffect(Effect effect)
        {
            list.serializedObject.Update();
            list.InsertArrayElementAtIndex(list.arraySize);
            list.GetArrayElementAtIndex(list.arraySize - 1).managedReferenceValue = effect;
            list.serializedObject.ApplyModifiedProperties();
        }

        private void AddEffectToAddMenu(GenericMenu contextMenu, string menuPath, ushort effectID)
        {
            contextMenu.AddItem(new GUIContent(menuPath), false, CreateEffect, effectID);
        }



        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (list == null)
            {
                list = property.FindPropertyRelative("effects");
            }
            
            float height = FOLDOUT_HEIGHT;
            if (property.isExpanded)
            {
                for (int i = 0; i < list.arraySize; i++)
                {
                    height += EditorGUI.GetPropertyHeight(list.GetArrayElementAtIndex(i));
                }
            }
            return height;
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            //DrawAddAndRemoveButtons();

            EditorGUI.BeginProperty(position, label, property);

            Rect foldoutRect = new Rect(position.x, position.y, position.width, FOLDOUT_HEIGHT);

            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);


            //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            if (property.isExpanded)
            {

                EditorGUI.indentLevel++;

                float addY = FOLDOUT_HEIGHT;
                for (int i = 0; i < list.arraySize; i++)
                {
                    SerializedProperty element = list.GetArrayElementAtIndex(i);
                    Rect rect = new Rect(0, position.y + addY, position.width, EditorGUI.GetPropertyHeight(element));
                    addY += rect.height;

                    EditorGUI.PropertyField(rect, element, new GUIContent("Effect " + i), true);
                }
                
                

                EditorGUI.indentLevel--;

                DrawAddAndRemoveButtons();

                EditorGUILayout.Space(10);

            }






            EditorGUI.EndProperty();
        }


        private void DrawAddAndRemoveButtons()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Add Effect"))
            {
                GenericMenu contextMenu = new GenericMenu();
                PopulateAddMenu(contextMenu);
                contextMenu.ShowAsContext();
            }

            if (GUILayout.Button("Remove Effect"))
            {
                GenericMenu contextMenu = new GenericMenu();
                PopulateRemoveMenu(contextMenu);
                contextMenu.ShowAsContext();
            }

            GUILayout.EndHorizontal();

        }

        private void PopulateRemoveMenu(GenericMenu contextMenu)
        {
            for (int i = 0; i < list.arraySize; i++)
            {
                AddEffectToRemoveMenu(contextMenu, "Effect " + i, i);
            }
        }

        private void AddEffectToRemoveMenu(GenericMenu contextMenu, string menuPath, int index)
        {
            contextMenu.AddItem(new GUIContent(menuPath), false, RemoveEffect, index);
        }

        private void RemoveEffect(object index)
        {
            list.serializedObject.Update();
            int i = (int)index;
            Debug.Log(i);
            list.DeleteArrayElementAtIndex(i);
            list.serializedObject.ApplyModifiedProperties();
        }
    }
}
