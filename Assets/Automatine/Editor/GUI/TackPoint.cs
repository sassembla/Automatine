using UnityEngine;
using UnityEditor;

using System;
using System.Linq;
using System.Collections.Generic;

namespace Automatine {
	[Serializable] public class TackPoint {
		public static Action<OnAutomatineEvent> Emit;

		[SerializeField] private TackPointInspector tackPointInspector;

		[CustomEditor(typeof(TackPointInspector))]
		public class TackPointInspectorGUI : Editor {
			public override void OnInspectorGUI () {
				var insp = ((TackPointInspector)target).tackPoint;
				GUILayout.Label("id:" + insp.tackId);			  

				var info = insp.info;
				GUILayout.Label("info:" + info);
				
				GUILayout.Space(10);
				
				var start = insp.start;
				GUILayout.Label("start");
				using (new GUILayout.HorizontalScope(GUI.skin.box)) {
					GUILayout.FlexibleSpace();
					GUILayout.Label("" + start);
				}
				
				GUILayout.Space(5);
				
				var span = insp.span;
				GUILayout.Label("span");
				
				var isSpanUnlimited = span == AutomatineDefinitions.Tack.LIMIT_UNLIMITED;
				
				var unlimitedOrNot = GUILayout.Toggle(isSpanUnlimited, "Unlimited Span");
				if (unlimitedOrNot != (isSpanUnlimited)) {
					insp.UpdateSpanToUnlimited(unlimitedOrNot);		   
				}
				
				using (new GUILayout.HorizontalScope(GUI.skin.box)) {
					GUILayout.FlexibleSpace();
					if (isSpanUnlimited) GUILayout.Label("Unlimited"); 
					else GUILayout.Label("" + span);
				}
				
				
				if (!isSpanUnlimited) {
					GUILayout.Space(10);
					
					var end = start + span - 1;
					GUILayout.Label("end");
					using (new GUILayout.HorizontalScope(GUI.skin.box)) {
						GUILayout.FlexibleSpace();
						GUILayout.Label("" + end);
					}
				}
				
				GUILayout.Space(10);

				GUILayout.Label("Condition Type & Value");
				using (new GUILayout.HorizontalScope(GUI.skin.box)) {
					var conditionType = insp.conditionType;
					if (GUILayout.Button(conditionType, "GV Gizmo DropDown")) {
						insp.ShowContextOfType();
					}
					
					var conditionValue = insp.conditionValue;
					if (GUILayout.Button(conditionValue, "GV Gizmo DropDown")) {
						insp.ShowContextOfValue();
					}
				}
				
				GUILayout.Space(10);
				
				GUILayout.Label("Coroutines");
				
				var routineIds = insp.routineIds;
				
				using (new GUILayout.VerticalScope(GUI.skin.box)) {
					foreach (var routine in routineIds.Select((val, index) => new {index, val})) {
						using (new GUILayout.HorizontalScope()) {
							if (GUILayout.Button(string.Empty, "OL Minus", GUILayout.Width(20))) {
								insp.RemoveCoroutine(routine.index);												
							}
							if (GUILayout.Button(routine.val, "GV Gizmo DropDown")) {
								insp.ShowContextOfCoroutine(routine.index);											  
							}
							if (!string.IsNullOrEmpty(routine.val)) {
								if (GUILayout.Button("Open", "toolbarbutton", GUILayout.Width(60))) {
									var scriptName = AutomatineSettings.UNITY_FOLDER_SEPARATOR + routine.val + ".cs";
									
									var assetPathCanditates = AssetDatabase.GetAllAssetPaths().Where(path => path.Contains(scriptName)).ToList();
									if (!assetPathCanditates.Any()) {
										Debug.LogError("Coroutine:" + routine.val + ".cs is not found. maybe script name was changed or written in another named file.");
										return;
									}
									
									var asset = AssetDatabase.LoadAssetAtPath(assetPathCanditates[0], typeof(TextAsset));
									if (asset == null) {
										Debug.LogError("Coroutine: failed to open " + routine.val + ".cs.");
										return;
									}
									
									AssetDatabase.OpenAsset(asset, 0);
									
								}
							}
						}
						GUILayout.Space(5);
					}
					
					using (new GUILayout.HorizontalScope()) {
						GUILayout.FlexibleSpace();
						if (GUILayout.Button(string.Empty, "OL Plus", GUILayout.Width(20))) {
							insp.AddCoroutine();
						}
					}
				}
			}
		}
		
		

		[SerializeField] public string tackId;
		[SerializeField] public string parentTimelineId;
		[SerializeField] private int index;
		
		[SerializeField] private bool active = false;

		[SerializeField] public string info;
		[SerializeField] public int start;
		[SerializeField] public int span;
		[SerializeField] public string conditionType;
		[SerializeField] public string conditionValue;
		[SerializeField] public List<string> routineIds;

		[SerializeField] private Texture2D tackBackTransparentTex;
		[SerializeField] private Texture2D tackColorTex;
		
		[NonSerializedAttribute] public List<ConditionData> conditions = new List<ConditionData>();
		[NonSerializedAttribute] public List<string> unselectableTypes = new List<string>(); 
		
		private Vector2 distance = Vector2.zero;

		private enum TackModifyMode : int {
			NONE,
			
			GRAB_START,
			GRAB_BODY,
			GRAB_END,
			GRAB_HALF,

			DRAG_START,
			DRAG_BODY,
			DRAG_END,
		}
		private TackModifyMode mode = TackModifyMode.NONE;

		private Vector2 dragBeginPoint;

		private CoroutineGeneratorWindow coroutineGeneratorWindow;
		public TackPoint () {}
		
		public TackPoint (TackData tackData, int index) {
			this.tackId = tackData.id;
			this.index = index;
			this.start = tackData.start;
			this.span = tackData.span;
			this.conditionType = tackData.conditionType;
			this.conditionValue = tackData.conditionValue;
			this.routineIds = tackData.routineIds;
		}
		
		public TackPoint (
			int index,
			string info, 
			int start, 
			int span
		) {
			this.tackId = AutomatineGUISettings.ID_HEADER_TACK + Guid.NewGuid().ToString();
			this.index = index;
			
			this.info = info;
			this.start = start;
			this.span = span;
			
			// default condition type & value is empty.
			this.conditionType = string.Empty;
			this.conditionValue = string.Empty;
			
			// default routines are empty.
			this.routineIds = new List<string>();
		}

		public Texture2D GetColorTex () {
			return tackColorTex;
		}

		public void InitializeTackTexture (Texture2D baseTex) {
			GenerateTextureFromBaseTexture(baseTex, index);
		}
		
		public void SetActive () {
			active = true;

			ApplyDataToInspector();
			Selection.activeObject = tackPointInspector;
		}

		public void SetDeactive () {
			active = false;
		}

		public bool IsActive () {
			return active;
		}
		
		public void EmitUndo (string undoMessage) {
			Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_UNDO, tackId, -1, undoMessage));
		}
		
		public void EmitSave () {
			Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_SAVE, tackId));
		}

		public void DrawTack (Rect limitRect, string parentTimelineId, float startX, float startY, bool isUnderEvent, int viewFrameWidth) {
			this.parentTimelineId = parentTimelineId;

			var tackBGRect = DrawTackPointInRect(startX, startY, viewFrameWidth);

			var globalMousePos = Event.current.mousePosition;

			var useEvent = false;

			var localMousePos = new Vector2(globalMousePos.x - tackBGRect.x, globalMousePos.y - tackBGRect.y);
			var sizeRect = new Rect(0, 0, tackBGRect.width, tackBGRect.height);

			if (!isUnderEvent) return;

			// mouse event handling.
			switch (mode) {
				case TackModifyMode.NONE: {
					useEvent = BeginTackModify(tackBGRect, globalMousePos);
					break;
				}

				case TackModifyMode.GRAB_START:
				case TackModifyMode.GRAB_BODY:
				case TackModifyMode.GRAB_END:
				case TackModifyMode.GRAB_HALF: {
					useEvent = RecognizeTackModify(globalMousePos);
					break;
				}

				case TackModifyMode.DRAG_START:
				case TackModifyMode.DRAG_BODY:
				case TackModifyMode.DRAG_END: {
					useEvent = UpdateTackModify(limitRect, tackBGRect, globalMousePos);
					break;
				}
			}


			// optional manipulation.
			if (sizeRect.Contains(localMousePos)) {
				switch (Event.current.type) {
					// dragging.
					case EventType.DragUpdated: {
						Debug.LogWarning("D&Dアイコン更新");
						// var refs = DragAndDrop.objectReferences;

						// foreach (var refe in refs) {
						// 	if (refe.GetType() == typeof(UnityEditor.MonoScript)) {
						// 		var type = ((MonoScript)refe).GetClass();
								
						// 		var inherited = IsAcceptableScriptType(type);

						// 		if (inherited != null) {
						// 			// at least one asset is script. change interface.
						// 			DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
						// 			break;
						// 		}
						// 	}
						// }
						useEvent = true;
						break;
					}

					// drag over.
					case EventType.DragExited: {
						// Debug.LogWarning("D&D受け、ここだとroutineの追加、なんだけどPerformedで反応しない、、？？よくわからんな、、");
						// return;
						// // var pathAndRefs = new Dictionary<string, object>();
						// for (var i = 0; i < DragAndDrop.paths.Length; i++) {
						// 	var path = DragAndDrop.paths[i];
						// 	var refe = DragAndDrop.objectReferences[i];
						// // 	pathAndRefs[path] = refe;
						// 	Debug.LogWarning("path:" + path + " refe:" + refe);
						// }


						// var shouldSave = false;
						// foreach (var item in pathAndRefs) {
						// 	var path = item.Key;
						// 	var refe = (MonoScript)item.Value;
						// 	if (refe.GetType() == typeof(UnityEditor.MonoScript)) {
						// 		var type = refe.GetClass();
						// 		var inherited = IsAcceptableScriptType(type);

						// 		if (inherited != null) {
						// 			var dropPos = Event.current.mousePosition;
						// 			var scriptName = refe.name;
						// 			var scriptType = scriptName;// name = type.
						// 			var scriptPath = path;
						// 			AddNodeFromCode(scriptName, scriptType, scriptPath, inherited, Guid.NewGuid().ToString(), dropPos.x, dropPos.y);
						// 			shouldSave = true;
						// 		}
						// 	}
						// }

						// if (shouldSave) SaveGraphWithReload();
						useEvent = true;
						break;
					}

					case EventType.ContextClick: {
						ShowContextMenu();
						useEvent = true;
						break;
					}

					// clicked.
					case EventType.MouseUp: {
						// right click.
						if (Event.current.button == 1) {
							ShowContextMenu();
							useEvent = true;
							break;
						}

						Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_OBJECT_SELECTED, tackId));
						useEvent = true;
						break;
					}
				}
			}

			if (useEvent) {
				Event.current.Use();
			}
		}
		
		public void UpdateSpanToUnlimited (bool result) {
			EmitUndo("Change Tack Span");
			
			if (result) {
				span = AutomatineDefinitions.Tack.LIMIT_UNLIMITED;
				Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_TACK_DELETEAFTERUNLIMITED, tackId));
			}
			else span = 1;
			
			EmitSave();
		}
		
		public void AddCoroutine () {
			EmitUndo("Add New Coroutine");
			routineIds.Add(string.Empty);
			EmitSave();
		}
		
		public void RemoveCoroutine (int index) {
			Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_TACK_REMOVECOROUTINE, tackId, index));
		}
		
		public void ShowContextOfCoroutine (int indexOfRoutineSlot) {
			var allCoroiutineIds = System.Reflection.Assembly.LoadFrom(AutomatineGUISettings.APPLICATION_DLL_PATH)
				.GetTypes()
				.Where(t => t.Name.Contains(AutomatineGUISettings.ROUTINE_CONTEXT_CLASS_NAME) && t.BaseType != null && t.BaseType.IsGenericType)
				.Select(t => t.GetMethods())
				.SelectMany(methods => methods)
				.Where(methodInfo => methodInfo.ReturnType == typeof(System.Collections.IEnumerator))
				.Select(method => method.Name)
				.ToList();
			
				
			var menu = new GenericMenu();
			
			var currentCoroutineId = routineIds[indexOfRoutineSlot];
			var coroiutineIds = allCoroiutineIds.Where(coroutineId => coroutineId != currentCoroutineId).ToList();
			
			// show current coroutine
			menu.AddDisabledItem(
				new GUIContent(currentCoroutineId)
			);
			
			menu.AddSeparator(string.Empty);
			
			// set empty.
			menu.AddItem(
				new GUIContent("Add New Coroutine"),
				false, 
				() => {
					if (coroutineGeneratorWindow == null) coroutineGeneratorWindow = ScriptableObject.CreateInstance<CoroutineGeneratorWindow>();
					coroutineGeneratorWindow.existCoroutines = allCoroiutineIds;
					
					coroutineGeneratorWindow.enter = (string newCoroutineName) => {
						Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_ADDNEWCOROUTINE, tackId, indexOfRoutineSlot, newCoroutineName));
					};
					
					coroutineGeneratorWindow.ShowAuxWindow();
				}
			);
			
			
			// set empty.
			menu.AddItem(
				new GUIContent("Set Empty"),
				false, 
				() => {
					EmitUndo("Set Coroutine");
					routineIds[indexOfRoutineSlot] = string.Empty;
					EmitSave();
				}
			);
			
			menu.AddSeparator(string.Empty);
			
			// other.
			foreach (var coroutine in coroiutineIds.Select((val, index) => new {index, val})) {
				var newCoroutineId = coroutine.val;
				
				menu.AddItem(
					new GUIContent(newCoroutineId),
					false, 
					() => {
						EmitUndo("Set Coroutine");
						routineIds[indexOfRoutineSlot] = newCoroutineId;
						EmitSave();
					}
				);
			}
			
			menu.ShowAsContext();
		}
		
		public void ShowContextOfType () {
			var menu = new GenericMenu();
			
			// update parent:ChangerPlate's conditions to latest.
			Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_REFRESHTACKCONDITIONS, tackId));
			
			if (!string.IsNullOrEmpty(conditionType)) {
				
				// current.
				menu.AddDisabledItem(
					new GUIContent(conditionType)
				);
				
				menu.AddSeparator(string.Empty);
				
				// set empty.
				menu.AddItem(
					new GUIContent("Set Empty"),
					false, 
					() => {
						EmitUndo("Change Condition Type");
						conditionType = string.Empty;
						conditionValue = string.Empty;
						EmitSave();
						
						Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_REFRESHTACKCONDITIONS, tackId));
					}
				);
				
				menu.ShowAsContext();
				return;
			}
			
			
			/*
				condition type is empty.
			*/
			
			var selectableMenuItems = conditions
				.Select(conditonData => conditonData.conditionType)
				.ToList();
			
			// new.
			menu.AddItem(
				new GUIContent("Add New Type"),
				false, 
				() => {
					Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_REFRESHTACKCONDITIONS, tackId));
					Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_ADDNEWTYPE, tackId));
				}
			);
			
			menu.AddSeparator(string.Empty);
			
			// other.
			foreach (var conditonData in selectableMenuItems.Select((val, index) => new {index, val})) {
				var currentType = conditonData.val;
				
				if (unselectableTypes.Contains(currentType)) {
					menu.AddDisabledItem(new GUIContent(currentType));
				} else {
					menu.AddItem(
						new GUIContent(currentType),
						false, 
						() => {
							EmitUndo("Change Condition Type");
							conditionType = currentType;
							var valueCandidates = ConditionGateway.TypeValuesFromTypeConditionStr(conditionType);
							
							if (0 < valueCandidates.Length) conditionValue = valueCandidates[0];
							EmitSave();
							Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_REFRESHTACKCONDITIONS, tackId));
						}
					);
				}
			}
			
			menu.ShowAsContext();
		}
		
		public void ShowContextOfValue () {
			var menu = new GenericMenu();
			
			// update parent:ChangerPlate's conditions to latest.
			Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_REFRESHTACKCONDITIONS, tackId));
			
			var menuItems = conditions
				.Where(data => data.conditionType == conditionType)
				.FirstOrDefault();
			
			// current.
			if (!string.IsNullOrEmpty(conditionValue)) {
				menu.AddDisabledItem(
					new GUIContent(conditionValue)
				);
				menu.AddSeparator(string.Empty);
			}
			
			// new.
			if (string.IsNullOrEmpty(conditionType)) { 
				menu.AddItem(
					new GUIContent("Add New Type"),
					false, 
					() => {
						Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_ADDNEWTYPE, tackId));
					}
				);
			} else {
				menu.AddItem(
					new GUIContent("Add New Value"),
					false, 
					() => {
						Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_ADDNEWVALUE, tackId));
					}
				);
			}
			
			// set empty.
			menu.AddItem(
				new GUIContent("Set Empty"),
				false, 
				() => {
					EmitUndo("Change Condition Value");
					conditionValue = string.Empty;
					EmitSave();
				}
			);
			
			// show other conditionValues.
			if (menuItems != null) {
				// remove current.
				var copied = new List<string>(menuItems.conditionValues);
				copied.Remove(conditionValue);
				
				menu.AddSeparator(string.Empty);
				
				// other.
				foreach (var currentConditionValue in copied.Select((val, index) => new {index, val})) {
					// var currentType = conditionType;
					
					var currentValue = currentConditionValue.val;
					var currentIndex = currentConditionValue.index;
					
					menu.AddItem(
						new GUIContent(currentValue),
						false, 
						() => {
							EmitUndo("Change Condition Value");
							conditionValue = copied[currentIndex];
							EmitSave();
						}
					);
				}
			}
			
			menu.ShowAsContext();
		}

		private void ShowContextMenu () {
			var framePoint = start;
			var menu = new GenericMenu();
			Debug.LogWarning("実行できるコマンドにonOffあるならここでなんかせんとな。");

			var menuItems = new Dictionary<string, OnAutomatineEvent.EventType>{
				// {"Copy This Tack", OnAutomatineEvent.EventType.EVENT_TACK_COPY},
				// {"Cut This Tack", OnAutomatineEvent.EventType.EVENT_TACK_CUT},
				{"Delete This Tack", OnAutomatineEvent.EventType.EVENT_TACK_DELETED},
			};

			foreach (var key in menuItems.Keys) {
				var eventType = menuItems[key];
				menu.AddItem(
					new GUIContent(key),
					false, 
					() => {
						Emit(new OnAutomatineEvent(eventType, this.tackId, framePoint));
					}
				);
			}
			menu.ShowAsContext();
		}

		private void GenerateTextureFromBaseTexture (Texture2D baseTex, int index) {
			var samplingColor = baseTex.GetPixels()[0];
			var rgbVector = new Vector3(samplingColor.r, samplingColor.g, samplingColor.b);
			
			var rotatedVector = Quaternion.AngleAxis(12.5f * index, new Vector3(1.5f * index, 1.25f * index, 1.37f * index)) * rgbVector;
			
			var slidedColor = new Color(rotatedVector.x, rotatedVector.y, rotatedVector.z, 1);

			this.tackBackTransparentTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			tackBackTransparentTex.SetPixel(0, 0, new Color(slidedColor.r, slidedColor.g, slidedColor.b, 0.5f));
			tackBackTransparentTex.Apply();

			this.tackColorTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			tackColorTex.SetPixel(0, 0, new Color(slidedColor.r, slidedColor.g, slidedColor.b, 1.0f));
			tackColorTex.Apply();
		}

		private Rect DrawTackPointInRect (float startX, float startY, int viewFrameWidth) {
			var tackStartPointX = startX + (start * AutomatineGUISettings.TACK_FRAME_WIDTH);
			var end = start + span - 1;
			
			var drawSpan = span;
			if (span == AutomatineDefinitions.Tack.LIMIT_UNLIMITED) {
				drawSpan = viewFrameWidth + 1;
				end = start + viewFrameWidth + 1;
			}
			
			var tackEndPointX = startX + (end * AutomatineGUISettings.TACK_FRAME_WIDTH);

			var tackBGRect = new Rect(tackStartPointX, startY, drawSpan * AutomatineGUISettings.TACK_FRAME_WIDTH + 1f, AutomatineGUISettings.TACK_HEIGHT);
			
			switch (mode) {
				case TackModifyMode.DRAG_START: {
					tackStartPointX = startX + (start * AutomatineGUISettings.TACK_FRAME_WIDTH) + distance.x;
					tackBGRect = new Rect(tackStartPointX, startY, drawSpan * AutomatineGUISettings.TACK_FRAME_WIDTH + 1f - distance.x, AutomatineGUISettings.TACK_HEIGHT);
					break;
				}
				case TackModifyMode.DRAG_BODY: {
					tackStartPointX = startX + (start * AutomatineGUISettings.TACK_FRAME_WIDTH) + distance.x;
					tackEndPointX = startX + (end * AutomatineGUISettings.TACK_FRAME_WIDTH) + distance.x;
					tackBGRect = new Rect(tackStartPointX, startY, drawSpan * AutomatineGUISettings.TACK_FRAME_WIDTH + 1f, AutomatineGUISettings.TACK_HEIGHT);
					break;
				}
				case TackModifyMode.DRAG_END: {
					tackEndPointX = startX + (end * AutomatineGUISettings.TACK_FRAME_WIDTH) + distance.x;
					tackBGRect = new Rect(tackStartPointX, startY, drawSpan * AutomatineGUISettings.TACK_FRAME_WIDTH + distance.x + 1f, AutomatineGUISettings.TACK_HEIGHT);
					break;
				}
			}

			

			// draw tack.
			{
				// draw bg.
				var frameBGRect = new Rect(tackBGRect.x, tackBGRect.y, tackBGRect.width, AutomatineGUISettings.TACK_FRAME_HEIGHT);

				GUI.DrawTexture(frameBGRect, tackBackTransparentTex);
				
				// draw points.
				{
					// tackpoint back line.
					if (drawSpan == 1) GUI.DrawTexture(new Rect(tackBGRect.x + (AutomatineGUISettings.TACK_FRAME_WIDTH / 3) + 1, startY + (AutomatineGUISettings.TACK_FRAME_HEIGHT / 3) - 1, (AutomatineGUISettings.TACK_FRAME_WIDTH / 3) - 1, 11), tackColorTex); 
					if (1 < drawSpan) GUI.DrawTexture(new Rect(tackBGRect.x + (AutomatineGUISettings.TACK_FRAME_WIDTH / 2), startY + (AutomatineGUISettings.TACK_FRAME_HEIGHT / 3) - 1, tackEndPointX - tackBGRect.x, 11), tackColorTex);

					// frame start point.
					DrawTackPoint(start, tackBGRect.x, startY);

					// frame end point.
					if (1 < drawSpan) DrawTackPoint(end, tackEndPointX, startY);
				}
				
				// draw infinity icon.
				if (span == AutomatineDefinitions.Tack.LIMIT_UNLIMITED) {
					GUI.DrawTexture(new Rect(tackStartPointX + 20, startY + 6, AutomatineGUISettings.tackInfinityTex.width/2, AutomatineGUISettings.tackInfinityTex.height/2), AutomatineGUISettings.tackInfinityTex);
				}
				
				
				var routineComponentY = startY + AutomatineGUISettings.TACK_FRAME_HEIGHT;

				// routine component.
				{
					var height = AutomatineGUISettings.ROUTINE_HEIGHT;
					if (active) GUI.DrawTexture(new Rect(tackBGRect.x, routineComponentY, tackBGRect.width, height), AutomatineGUISettings.activeObjectBaseTex);

					GUI.DrawTexture(new Rect(tackBGRect.x + 1, routineComponentY, tackBGRect.width - 2, height - 1), tackColorTex);
					
					// condition.
					if (!string.IsNullOrEmpty(conditionType) && (!string.IsNullOrEmpty(conditionValue))) {
						GUI.Label(new Rect(tackBGRect.x + 1, routineComponentY, tackBGRect.width - 2, height - 1), conditionType + "." + conditionValue);
					}
					
					// coroutine.
					var nonEmptyRoutines = routineIds.Where(routineId => !string.IsNullOrEmpty(routineId)).ToList();
					if (nonEmptyRoutines.Any()) {
						var coroutineDetail = nonEmptyRoutines[0];
						if (1 < nonEmptyRoutines.Count) coroutineDetail = coroutineDetail + " + " + (nonEmptyRoutines.Count - 1) + " coroutines."; 
						
						coroutineNameRect = new Rect(tackBGRect.x + 1, routineComponentY + 16, GUI.skin.label.CalcSize(new GUIContent(coroutineDetail)).x, height - 1);
						GUI.Label(coroutineNameRect, coroutineDetail);
					}
				}
			}

			return tackBGRect;
		}
		
		Rect coroutineNameRect;

		private bool BeginTackModify (Rect tackBGRect, Vector2 beginPoint) {
			if (!tackBGRect.Contains(Event.current.mousePosition)) return false;
			
			switch (Event.current.type) {
				case EventType.MouseMove: {
					if (coroutineNameRect.Contains(Event.current.mousePosition)) {
						Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_TACK_SHOWCOROUTINES, this.tackId));
						Event.current.Use();
					} 
					return false;
				}
				case EventType.MouseDown: {
					var startRect = new Rect(tackBGRect.x, tackBGRect.y, AutomatineGUISettings.TACK_FRAME_WIDTH, AutomatineGUISettings.TACK_FRAME_HEIGHT);
					if (startRect.Contains(beginPoint)) {
						if (span == 1) {
							dragBeginPoint = beginPoint;
							mode = TackModifyMode.GRAB_HALF;
							return true;
						}
						dragBeginPoint = beginPoint;
						mode = TackModifyMode.GRAB_START;
						return true;
					}
					var endRect = new Rect(tackBGRect.x + tackBGRect.width - AutomatineGUISettings.TACK_FRAME_WIDTH, tackBGRect.y, AutomatineGUISettings.TACK_FRAME_WIDTH, AutomatineGUISettings.TACK_FRAME_HEIGHT);
					if (endRect.Contains(beginPoint)) {
						dragBeginPoint = beginPoint;
						mode = TackModifyMode.GRAB_END;
						return true;
					}
					if (tackBGRect.Contains(beginPoint)) {
						dragBeginPoint = beginPoint;
						mode = TackModifyMode.GRAB_BODY;
						return true;
					}
					return false;
				}
			}

			return false;
		}

		private bool RecognizeTackModify (Vector2 mousePos) {
			
			switch (Event.current.type) {
				case EventType.MouseDrag: {
					switch (mode) {
						case TackModifyMode.GRAB_START: {
							mode = TackModifyMode.DRAG_START;
							
							// set as body if unlimited.
							if (span == AutomatineDefinitions.Tack.LIMIT_UNLIMITED) mode = TackModifyMode.DRAG_BODY;
							
							Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_OBJECT_SELECTED, tackId));
							return true;
						}
						case TackModifyMode.GRAB_BODY: {
							mode = TackModifyMode.DRAG_BODY;
							Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_OBJECT_SELECTED, tackId));
							return true;
						}
						case TackModifyMode.GRAB_END: {
							mode = TackModifyMode.DRAG_END;
							Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_OBJECT_SELECTED, tackId));
							return true;
						}
						case TackModifyMode.GRAB_HALF: {
							if (mousePos.x < dragBeginPoint.x) mode = TackModifyMode.DRAG_START;
							if (dragBeginPoint.x < mousePos.x) mode = TackModifyMode.DRAG_END;
							Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_OBJECT_SELECTED, tackId));
							return true;
						}
					}

					return false;
				}
				case EventType.MouseUp: {
					mode = TackModifyMode.NONE;
					Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_OBJECT_SELECTED, tackId));
					return true;
				}
			}

			return false;
		}
		
		private bool UpdateTackModify (Rect limitRect, Rect tackBGRect, Vector2 draggingPoint) {
			if (!limitRect.Contains(draggingPoint)) {
				ExitUpdate(distance);
				return true;
			}

			// far from bandwidth, exit mode.
			if (draggingPoint.y < 0 || tackBGRect.height + AutomatineGUISettings.TIMELINE_HEADER_HEIGHT < draggingPoint.y) {
				ExitUpdate(distance);
				return true;
			}
			
			switch (Event.current.type) {
				case EventType.MouseDrag: {
					Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_TACK_MOVING, tackId));
					
					distance = draggingPoint - dragBeginPoint;
					var distanceToFrame = DistanceToFrame(distance.x);

					switch (mode) {
						case TackModifyMode.DRAG_START: {
							// limit 0 <= start
							if ((start + distanceToFrame) < 0) distance.x = -FrameToDistance(start);
							
							// unlimited -> treat as body.
							if (span == AutomatineDefinitions.Tack.LIMIT_UNLIMITED) break; 
							
							// limit start <= end
							if (span <= (distanceToFrame + 1)) distance.x = FrameToDistance(span - 1);
							break;
						}
						case TackModifyMode.DRAG_BODY: {
							// limit 0 <= start
							if ((start + distanceToFrame) < 0) distance.x = -FrameToDistance(start);
							break;
						}
						case TackModifyMode.DRAG_END: {
							// limit start <= end
							if ((span + distanceToFrame) <= 1) distance.x = -FrameToDistance(span - 1);
							break;
						}
					}

					return true;
				}
				case EventType.MouseUp: {
					ExitUpdate(distance);
					return true;
				}
			}

			return false;
		}

		private void ExitUpdate (Vector2 currentDistance) {
			var distanceToFrame = DistanceToFrame(currentDistance.x);

			Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_TACK_MOVED, tackId));

			switch (mode) {
				case TackModifyMode.DRAG_START: {
					start = start + distanceToFrame;
					span = span - distanceToFrame;
					break;
				}
				case TackModifyMode.DRAG_BODY: {
					start = start + distanceToFrame;
					break;
				}
				case TackModifyMode.DRAG_END: {
					span = span + distanceToFrame;
					break;
				}
			}

			if (start < 0) start = 0;

			Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_TACK_MOVED_AFTER, tackId));

			mode = TackModifyMode.NONE;

			distance = Vector2.zero;
		}

		private int DistanceToFrame (float distX) {
			var distanceToFrame = (int)(distX/AutomatineGUISettings.TACK_FRAME_WIDTH);
			var distanceDelta = distX % AutomatineGUISettings.TACK_FRAME_WIDTH;
			
			// adjust behaviour by frame width.
			if (AutomatineGUISettings.BEHAVE_FRAME_MOVE_RATIO <= distanceDelta) distanceToFrame = distanceToFrame + 1;
			if (distanceDelta <= -AutomatineGUISettings.BEHAVE_FRAME_MOVE_RATIO) distanceToFrame = distanceToFrame - 1;
			
			return distanceToFrame;
		}

		private float FrameToDistance (int frame) {
			return AutomatineGUISettings.TACK_FRAME_WIDTH * frame;
		}

		private void DrawTackPoint (int frame, float pointX, float pointY) {
			if (span == 1) {
				if (frame % 5 == 0 && 0 < frame) {
					GUI.DrawTexture(new Rect(pointX + 2, pointY + (AutomatineGUISettings.TACK_FRAME_HEIGHT / 3) - 2, AutomatineGUISettings.TACK_POINT_SIZE, AutomatineGUISettings.TACK_POINT_SIZE), AutomatineGUISettings.grayPointSingleTex);
				} else {
					GUI.DrawTexture(new Rect(pointX + 2, pointY + (AutomatineGUISettings.TACK_FRAME_HEIGHT / 3) - 2, AutomatineGUISettings.TACK_POINT_SIZE, AutomatineGUISettings.TACK_POINT_SIZE), AutomatineGUISettings.whitePointSingleTex);
				}	
				return;
			}

			if (frame % 5 == 0 && 0 < frame) {
				GUI.DrawTexture(new Rect(pointX + 2, pointY + (AutomatineGUISettings.TACK_FRAME_HEIGHT / 3) - 2, AutomatineGUISettings.TACK_POINT_SIZE, AutomatineGUISettings.TACK_POINT_SIZE), AutomatineGUISettings.grayPointTex);
			} else {
				GUI.DrawTexture(new Rect(pointX + 2, pointY + (AutomatineGUISettings.TACK_FRAME_HEIGHT / 3) - 2, AutomatineGUISettings.TACK_POINT_SIZE, AutomatineGUISettings.TACK_POINT_SIZE), AutomatineGUISettings.whitePointTex);
			}
		}

		public bool ContainsFrame (int frame) {
			switch (span) {
				case AutomatineDefinitions.Tack.LIMIT_UNLIMITED: {
					if (start <= frame) return true;
					break;
				}
				default: {
					if (start <= frame && frame <= start + span - 1) return true;
					break;
				}
			}
			
			return false;
		}

		public void UpdatePos (int start, int span) {
			this.start = start;
			this.span = span;
			ApplyDataToInspector();
		}
		
		public void ApplyDataToInspector () {
			if (tackPointInspector == null) tackPointInspector = ScriptableObject.CreateInstance("TackPointInspector") as TackPointInspector;
			
			tackPointInspector.tackPoint = this;
		}
	}
}