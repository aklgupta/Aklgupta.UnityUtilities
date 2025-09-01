using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;


namespace Aklgupta.Utils.PropertyDrawers {

	public class ShowFields : PropertyAttribute {
		public readonly IReadOnlyList<string> fields;
		public ShowFields(params string[] fields) => this.fields = new List<string>(fields);
	}


	[CustomPropertyDrawer(typeof(ShowFields))]
	public class ShowFieldsDrawer : PropertyDrawer, IDisposable  {

		private record ErrorMessage(string Message) {
			public string Message { get; } = Message;
		}

		private Object targetObject;
		private bool pollingRunning;
		private Dictionary<VisualElement, string> elements = new();

		public override VisualElement CreatePropertyGUI(SerializedProperty property) {
			var fields = ((ShowFields)attribute).fields;

			if (fields.Count == 0)
				return new HelpBox ($"[{nameof(ShowFields)}] Please add at least 1 field to display data for", HelpBoxMessageType.Warning);
			
			var foldout = new Foldout {
				text = "data",
			};

			targetObject = property.serializedObject.targetObject;
			foreach (var fieldName in fields) {
				GetFieldInfo(fieldName, out var type, out var value);


				if (value is ErrorMessage msg) {
					var textField = CreateTextField(fieldName, msg.Message);
					SetColor(textField, Color.red);
					elements.Add(textField, fieldName);
					continue;
				}
				
				switch (Type.GetTypeCode(type)) {
					case TypeCode.Boolean:
						var toggle = new Toggle{
							label = fieldName,
							value = (bool)value,
							enabledSelf = false,
						};
						foldout.Add(toggle);
						elements.Add(toggle, fieldName);
						break;
					case TypeCode.DateTime or TypeCode.Char or TypeCode.String or TypeCode.Decimal or TypeCode.Double or TypeCode.Single
						or TypeCode.Byte or TypeCode.SByte or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 or TypeCode.UInt16
						or TypeCode.UInt32 or TypeCode.UInt64:
					{
						var commonField = CreateTextField(fieldName, value.ToString());
						elements.Add(commonField, fieldName);
						break;
					}
					case TypeCode.Object:
						switch (value) {
							case Object o:
								var objectField = new ObjectField{
									label = fieldName,
									value = o,
									enabledSelf = false,
								};
								foldout.Add(objectField);
								elements.Add(objectField, fieldName);
								break;
							default:
								var objectText = CreateTextField(fieldName, "null");
								if (value == null)
									SetColor(objectText, Color.red);
								ResetColor(objectText);
								elements.Add(objectText, fieldName);
								break;
						}
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			Poller();
			return foldout;

			TextField CreateTextField(string fieldName, string msg) {
				var textField = new TextField {
					label = fieldName,
					value = msg,
					enabledSelf = false,
				};
				foldout.Add(textField);
				return textField;
			}
		}

		private static void SetColor(TextField textField, Color color) => ((TextElement)(textField.textEdition)).style.color = color;

		private static void ResetColor(TextField textField) => ((TextElement)(textField.textEdition)).style.color = StyleKeyword.Null;

		
		private async void Poller() {
			if (pollingRunning)
				return;
			pollingRunning = true;
			while (pollingRunning) {
				await Task.Delay(100);
			}
		}

		private void GetFieldInfo(string fieldName, out Type type, out object value) {
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

		public void Dispose() {
			pollingRunning = false;
		}
	}
}