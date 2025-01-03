
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SerializableArray))]
public class NewBehaviourScript : PropertyDrawer
{
    private Vector2Int _size;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.PrefixLabel(position, label);

        float line_height = EditorGUIUtility.singleLineHeight;
        float line_indent = 30f;

        SerializedProperty property_size = property.FindPropertyRelative("size");
        _size = property_size.vector2IntValue;
        Rect size_position = new Rect(position.x + line_indent, position.y + line_height, position.width - line_indent, line_height);
        EditorGUI.PropertyField(size_position, property_size);

        SerializedProperty property_array = property.FindPropertyRelative("array");
        property_array.arraySize = _size.x;
        float grid_width = (position.width - line_indent) / _size.y;
        for (int i = 0; i < _size.x; i++)
        {
            SerializedProperty property_row = property_array.GetArrayElementAtIndex(i).FindPropertyRelative("row");
            property_row.arraySize = _size.y;
            Rect array_position = new Rect(position.x + line_indent, position.y + (2+i) * line_height, grid_width, line_height);
            for (int j = 0; j < _size.y; j++)
            {
                EditorGUI.PropertyField(array_position, property_row.GetArrayElementAtIndex(j), GUIContent.none);
                array_position.x += grid_width;
            }
        }
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * (_size.y + 3);
    }
}
