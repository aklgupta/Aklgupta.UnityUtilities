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

		public override VisualElement CreatePropertyGUI(SerializedProperty property) {
			var fields = ((ShowFields)attribute).fields;
			
			if (fields.Count == 0)
				return new HelpBox ($"[{nameof(ShowFields)}] Please add at least 1 field to display data for", HelpBoxMessageType.Warning);
			
			var foldout = new Foldout {
				text = "data",
			};

			var targetObject = property.serializedObject.targetObject;
			foreach (var fieldName in fields) {
				GetFieldInfo(targetObject, fieldName, out var type, out var value);


				if (value is ErrorMessage msg) {
					var textField = CreateTextField(fieldName, msg.Message);
					SetColor(textField, Color.red);
					continue;
				}
				
				switch (Type.GetTypeCode(type)) {
					case TypeCode.Boolean:
						foldout.Add(new Toggle{
							label = fieldName,
							value = (bool)value,
							enabledSelf = false,
						});
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
						var textField = new TextField {
							label = fieldName,
							value = value.ToString(),
							enabledSelf = false,
						};
						foldout.Add(textField);
						break;
					case TypeCode.Object:
						switch (value) {
							case Object o:
								foldout.Add(new ObjectField{
									label = fieldName,
									value = o,
									enabledSelf = false,
								});
								break;
							case null:
								foldout.Add(new TextField {
									label = fieldName,
									value = "null",
									enabledSelf = false,
								});
								break;
							default:
								foldout.Add(new TextField {
									label = fieldName,
									value = value.ToString(),
									enabledSelf = false,
								});
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

		private static void SetColor(TextField textField, Color color) {
			((TextElement)(textField.textEdition)).style.color = color;
		}

		private bool run;
		private async void Poller() {
			run = true;
			for (int i = 0; i < 5 && run; i++) {
				Debug.Log(DateTime.Now.ToString());
				await Task.Delay(100);
			}
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



		public void Dispose() {
			run = false;
		}
	}
}