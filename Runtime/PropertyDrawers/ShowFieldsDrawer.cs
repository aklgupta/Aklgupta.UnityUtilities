using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;


namespace Aklgupta.Utils.PropertyDrawers {

	public class ShowFields : PropertyAttribute {
		public readonly IReadOnlyList<string> fields;
		public ShowFields(params string[] fields) => this.fields = new List<string>(fields);
	}

	[CustomPropertyDrawer(typeof(ShowFields))]
	public class ShowFieldsDrawer : PropertyDrawer {

		private record ErrorMessage(string msg) {
			public string msg { get; } = msg;
		}

		private static GUIStyle nullFieldStyle;

		private bool foldoutToggle = true;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			nullFieldStyle ??= new GUIStyle(GUI.skin.textField) {
				fontStyle = FontStyle.Italic,
				normal = {textColor = Color.red},
				active = {textColor = Color.red},
				hover = {textColor = Color.red},
				focused = {textColor = Color.red},
			};

			var fields = ((ShowFields)attribute).fields;
			
			if (fields.Count == 0)
				EditorGUI.HelpBox(position, "Please select at least 1 field to display data for", MessageType.Warning);

			foldoutToggle = EditorGUI.Foldout(position, foldoutToggle, "Data");
			if(!foldoutToggle)
				return;

			GUI.enabled = false;
			var targetObject = property.serializedObject.targetObject;
			foreach (var fieldName in fields) {
				GetFieldInfo(targetObject, fieldName, out var type, out var value);

				if (value is ErrorMessage msg) {
					EditorGUILayout.TextField(fieldName, msg.msg, nullFieldStyle);
					continue;
				}
				
				switch (Type.GetTypeCode(type)) {
					case TypeCode.Boolean:
						EditorGUILayout.Toggle(fieldName, (bool)value);
						break;
					case TypeCode.Byte:
					case TypeCode.Char:
					case TypeCode.DateTime:
					case TypeCode.Decimal:
					case TypeCode.Double:
					case TypeCode.Int16:
					case TypeCode.Int32:
					case TypeCode.Int64:
					case TypeCode.SByte:
					case TypeCode.Single:
					case TypeCode.String:
					case TypeCode.UInt16:
					case TypeCode.UInt32:
					case TypeCode.UInt64:
						EditorGUILayout.TextField(fieldName, value.ToString());
						break;
					case TypeCode.Object:
						switch (value) {
							case Object o:
								EditorGUILayout.ObjectField(fieldName, o, type);
								break;
							case null:
								EditorGUILayout.TextField(fieldName, "null", nullFieldStyle);
								break;
							default:
								EditorGUILayout.TextField(fieldName, value.ToString());
								break;
						}
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			GUI.enabled = true;
		}

		private static void GetFieldInfo(Object targetObject, string fieldName, out Type type, out object value) {
			var field = targetObject.GetType().GetField(
				fieldName,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance
			);
			if (field != null) {
				type = field.FieldType;
				value = field.GetValue(targetObject);
				return;
			}
			
			var prop = targetObject.GetType().GetProperty(
				fieldName,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance
			);
			if (prop != null) {
				type = prop.PropertyType;
				try {
					value = prop.GetValue(targetObject);
				}
				catch (Exception) {
					value = new ErrorMessage("Property value could not be fetched");
				}
				return;
			}

			var method = targetObject.GetType().GetMethod(
				fieldName,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance
			);
			if (method != null) {
				type = method.ReturnType;
				try {
					value = method.Invoke(targetObject, null);
				}
				catch (Exception) {
					value = new ErrorMessage("The method failed");
				}
				return;
			}
			
			value = new ErrorMessage("Passed parameter couldn't be determined");
			type = value.GetType();
		}



	}
}