using System;
using System.Collections.Generic;

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Automatine
{
    [Serializable]
    public class TimelineTrack
    {
        public static Action<OnAutomatineEvent> Emit;

        [SerializeField] private TimelineTrackInspector timelineTrackInspector;

        [CustomEditor(typeof(TimelineTrackInspector))]
        public class TimelineTrackInspectorGUI : Editor
        {
            public override void OnInspectorGUI()
            {
                var insp = ((TimelineTrackInspector)target).timelineTrack;

                var timelineId = insp.timelineId;
                GUILayout.Label("timelineId:" + timelineId);

                var info = insp.info;
                GUILayout.Label("info:" + info);

                GUILayout.Space(10);
                using (new GUILayout.HorizontalScope())
                {
                    var conditionTypeConstraint = insp.conditionTypeConstraint;
                    GUILayout.Label("Condition Type:");
                    if (GUILayout.Button(conditionTypeConstraint, "GV Gizmo DropDown"))
                    {
                        insp.ShowTypeContextMenu(conditionTypeConstraint);
                    }
                }

                GUILayout.Space(5);
                GUILayout.Label("Condition Values:");

                if (!string.IsNullOrEmpty(insp.conditionTypeConstraint))
                {
                    var tackValues = insp.tackPoints.Select(tack => tack.conditionValue).ToList().GroupBy(t => t);
                    using (new GUILayout.VerticalScope())
                    {
                        foreach (var val in tackValues)
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                GUILayout.FlexibleSpace();
                                if (!string.IsNullOrEmpty(val.Key)) GUILayout.Label(val.Key + " x " + val.Count());
                            }
                        }
                    }
                }

                GUILayout.Space(10);
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Total Tack Num:");
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(insp.tackPoints.Count.ToString());
                }
                GUILayout.Space(5);
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Last Frame:");
                    GUILayout.FlexibleSpace();

                    if (insp.tackPoints.Any())
                    {
                        var lastTack = insp.tackPoints.OrderBy(t => t.start + t.span).LastOrDefault();
                        if (lastTack != null)
                        {
                            switch (lastTack.span)
                            {
                                case AutomatineDefinitions.Tack.LIMIT_UNLIMITED:
                                    {
                                        GUILayout.Label("Unlimited");
                                        break;
                                    }
                                default:
                                    {
                                        var lastTackFramePos = lastTack.start + lastTack.span - 1;
                                        GUILayout.Label(lastTackFramePos.ToString());
                                        break;
                                    }
                            }
                        }
                    }
                    else
                    {
                        GUILayout.Label("None");
                    }

                }

            }
        }

        [SerializeField] private int index;

        [SerializeField] public bool active;
        [SerializeField] public string timelineId;

        [SerializeField] public string info;
        [SerializeField] private List<TackPoint> tackPoints = new List<TackPoint>();

        [SerializeField] public string conditionTypeConstraint;

        private Rect trackRect;
        private Texture2D timelineBaseTexture;

        private float timelineScrollX;

        private GUIStyle timelineConditionTypeLabel;
        private GUIStyle timelineConditionTypeLabelSmall;

        private List<string> movingTackIds = new List<string>();

        private int tackCount;

        public TimelineTrack()
        {
            InitializeTextResource();

            // set initial track rect.
            var defaultHeight = (AutomatineGUISettings.TIMELINE_HEIGHT);
            trackRect = new Rect(0, 0, 10, defaultHeight);
        }

        public TimelineTrack(TimelineData timelineData, int index)
        {
            InitializeTextResource();

            this.timelineId = timelineData.timelineId;
            this.index = index;
            this.info = timelineData.info;
            var loadedTackPoints = new List<TackPoint>();

            foreach (var tack in timelineData.tacks)
            {
                loadedTackPoints.Add(new TackPoint(tack, tackCount));
                tackCount = tackCount + 1;
            }

            this.tackPoints = loadedTackPoints;

            UpdateCondition();

            // set initial track rect.
            var defaultHeight = (AutomatineGUISettings.TIMELINE_HEIGHT);
            trackRect = new Rect(0, 0, 10, defaultHeight);

            ApplyTextureToTacks(index);
        }



        public TimelineTrack(int index, string info, List<TackPoint> tackPoints)
        {
            InitializeTextResource();

            this.timelineId = AutomatineGUISettings.ID_HEADER_TIMELINE + Guid.NewGuid().ToString();
            this.index = index;
            this.info = info;
            this.tackPoints = new List<TackPoint>(tackPoints);

            // set initial track rect.
            var defaultHeight = (AutomatineGUISettings.TIMELINE_HEIGHT);
            trackRect = new Rect(0, 0, 10, defaultHeight);

            ApplyTextureToTacks(index);
        }

        private void InitializeTextResource()
        {
            // 
        }

        public void DeleteConditionType(string deletingConditonTypeName)
        {
            foreach (var tack in tackPoints)
            {
                if (tack.conditionType == deletingConditonTypeName)
                {
                    tack.conditionType = string.Empty;
                    tack.conditionValue = string.Empty;
                }
            }
        }

        public void DeleteConditionValue(string deletingConditonValueName)
        {
            foreach (var tack in tackPoints)
            {
                if (tack.conditionValue == deletingConditonValueName)
                {
                    tack.conditionValue = string.Empty;
                }
            }
        }

        public TackPoint TackById(string tackId)
        {
            foreach (var tack in tackPoints)
            {
                if (tack.tackId == tackId)
                {
                    return tack;
                }
            }
            return null;
        }

        public TackPoint TackByFrame(int frame)
        {
            foreach (var tack in tackPoints)
            {
                if (tack.span == AutomatineDefinitions.Tack.LIMIT_UNLIMITED)
                {
                    if (tack.start <= frame) return tack;
                }
                if (tack.start <= frame && frame < tack.start + tack.span) return tack;
            }
            return null;
        }

        public List<TackPoint> TackPoints()
        {
            return new List<TackPoint>(tackPoints);
        }

        /*
			get texture for this timeline, then set texture to every tack.
		*/
        public void ApplyTextureToTacks(int texIndex)
        {
            timelineBaseTexture = GetTimelineTexture(texIndex);
            foreach (var tackPoint in tackPoints) tackPoint.InitializeTackTexture(timelineBaseTexture);
        }

        public static Texture2D GetTimelineTexture(int textureIndex)
        {
            var color = AutomatineGUISettings.RESOURCE_COLORS_SOURCES[textureIndex % AutomatineGUISettings.RESOURCE_COLORS_SOURCES.Count];
            var colorTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            colorTex.SetPixel(0, 0, color);
            colorTex.Apply();

            return colorTex;
        }

        public float Height()
        {
            return trackRect.height;
        }

        public int GetIndex()
        {
            return index;
        }

        public void SetActive()
        {
            active = true;

            ApplyDataToInspector();
            Selection.activeObject = timelineTrackInspector;
        }

        public void SetDeactive()
        {
            active = false;
        }

        public bool IsActive()
        {
            return active;
        }

        public bool ContainsActiveTack()
        {
            foreach (var tackPoint in tackPoints)
            {
                if (tackPoint.IsActive()) return true;
            }
            return false;
        }

        public void UpdateCondition()
        {
            foreach (var tack in tackPoints)
            {
                if (!string.IsNullOrEmpty(tack.conditionType) && !string.IsNullOrEmpty(tack.conditionValue))
                {
                    this.conditionTypeConstraint = tack.conditionType;
                    return;
                }
            }

            conditionTypeConstraint = AutomatineGUISettings.TIMELINE_CONDITION_EMPTY;
        }

        public int GetStartFrameById(string objectId)
        {
            foreach (var tackPoint in tackPoints)
            {
                if (tackPoint.tackId == objectId) return tackPoint.start;
            }
            return -1;
        }

        public void DrawTimelineTrack(float headWall, float timelineScrollX, float yOffsetPos, float width)
        {
            // 毎フレーム呼んでるけど特に問題なさげ
            this.timelineConditionTypeLabel = new GUIStyle();
            timelineConditionTypeLabel.normal.textColor = Color.white;
            timelineConditionTypeLabel.fontSize = 16;
            timelineConditionTypeLabel.alignment = TextAnchor.MiddleCenter;

            this.timelineConditionTypeLabelSmall = new GUIStyle();
            timelineConditionTypeLabelSmall.normal.textColor = Color.white;
            timelineConditionTypeLabelSmall.fontSize = 12;
            timelineConditionTypeLabelSmall.alignment = TextAnchor.MiddleCenter;

            this.timelineScrollX = timelineScrollX;

            trackRect.width = width;
            trackRect.y = yOffsetPos;

            if (trackRect.y < headWall) trackRect.y = headWall;

            if (timelineBaseTexture == null) ApplyTextureToTacks(index);

            trackRect = GUI.Window(index, trackRect, WindowEventCallback, string.Empty, "AnimationKeyframeBackground");
        }

        public float TrackHeight()
        {
            return trackRect.height;
        }


        private void WindowEventCallback(int id)
        {
            // draw bg from header to footer.
            {
                if (active)
                {
                    var headerBGActiveRect = new Rect(0f, 0f, trackRect.width, AutomatineGUISettings.TIMELINE_HEIGHT);
                    GUI.DrawTexture(headerBGActiveRect, AutomatineGUISettings.activeObjectBaseTex);

                    var headerBGRect = new Rect(1f, 1f, trackRect.width - 1f, AutomatineGUISettings.TIMELINE_HEIGHT - 2f);
                    GUI.DrawTexture(headerBGRect, AutomatineGUISettings.timelineHeaderTex);
                }
                else
                {
                    var headerBGRect = new Rect(0f, 0f, trackRect.width, AutomatineGUISettings.TIMELINE_HEIGHT);
                    GUI.DrawTexture(headerBGRect, AutomatineGUISettings.timelineHeaderTex);
                }
            }

            var timelineBodyY = AutomatineGUISettings.TIMELINE_HEADER_HEIGHT;

            // timeline condition type box.	
            var conditionBGRect = new Rect(1f, timelineBodyY, AutomatineGUISettings.TIMELINE_CONDITIONBOX_WIDTH - 1f, AutomatineGUISettings.TACK_HEIGHT - 1f);
            if (active)
            {
                var conditionBGRectInActive = new Rect(1f, timelineBodyY, AutomatineGUISettings.TIMELINE_CONDITIONBOX_WIDTH - 1f, AutomatineGUISettings.TACK_HEIGHT - 1f);
                GUI.DrawTexture(conditionBGRectInActive, timelineBaseTexture);
            }
            else
            {
                GUI.DrawTexture(conditionBGRect, timelineBaseTexture);
            }

            // draw timeline condition type
            if (!string.IsNullOrEmpty(conditionTypeConstraint))
            {
                if (conditionTypeConstraint.Length <= 8)
                {
                    GUI.Label(
                        new Rect(
                            0f,
                            AutomatineGUISettings.TIMELINE_HEADER_HEIGHT - 1f,
                            AutomatineGUISettings.TIMELINE_CONDITIONBOX_WIDTH,
                            AutomatineGUISettings.TACK_HEIGHT
                        ),
                        conditionTypeConstraint,
                        timelineConditionTypeLabel
                    );
                }
                else
                {
                    var wordLines = new List<string>();
                    var chunk = 10;
                    var count = conditionTypeConstraint.Length / chunk;
                    for (var i = 0; i < count; i++)
                    {
                        var rest = chunk;
                        if (conditionTypeConstraint.Length - (i * chunk) <= rest) rest = conditionTypeConstraint.Length - (i * chunk) - 1;
                        var adding = conditionTypeConstraint.Substring(i * chunk, rest);
                        wordLines.Add(adding);
                    }
                    GUI.Label(
                        new Rect(
                            0f,
                            AutomatineGUISettings.TIMELINE_HEADER_HEIGHT - 1f,
                            AutomatineGUISettings.TIMELINE_CONDITIONBOX_WIDTH,
                            AutomatineGUISettings.TACK_HEIGHT
                        ),
                        string.Join(" \n", wordLines.ToArray()),
                        timelineConditionTypeLabelSmall
                    );
                }
            }


            var frameRegionWidth = trackRect.width - AutomatineGUISettings.TIMELINE_CONDITIONBOX_SPAN;

            // draw frame back texture & TackPoint datas on frame.
            GUI.BeginGroup(new Rect(AutomatineGUISettings.TIMELINE_CONDITIONBOX_SPAN, timelineBodyY, trackRect.width, AutomatineGUISettings.TACK_HEIGHT));
            {
                DrawFrameRegion(timelineScrollX, 0f, frameRegionWidth, (int)((-timelineScrollX + frameRegionWidth) / AutomatineGUISettings.TACK_FRAME_WIDTH));
            }
            GUI.EndGroup();

            var useEvent = false;

            // mouse manipulation.
            switch (Event.current.type)
            {
                // dragging.
                case EventType.DragUpdated:
                    {
                        Debug.LogWarning("tl D&Dアイコン更新");
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
                case EventType.DragExited:
                    {
                        Debug.LogError("tl D&D受け、ここだとroutineの追加、なんだけどPerformedで反応しない、、？？よくわからんな、、");
                        // var pathAndRefs = new Dictionary<string, object>();
                        // for (var i = 0; i < DragAndDrop.paths.Length; i++) {
                        // 	var path = DragAndDrop.paths[i];
                        // 	var refe = DragAndDrop.objectReferences[i];
                        // // 	pathAndRefs[path] = refe;
                        // 	Debug.LogError("path:" + path + " refe:" + refe);
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

                case EventType.ContextClick:
                    {
                        ShowCommandContextMenu(timelineScrollX);
                        useEvent = true;
                        break;
                    }

                // clicked.
                case EventType.MouseUp:
                    {

                        // is right clicked
                        if (Event.current.button == 1)
                        {
                            ShowCommandContextMenu(timelineScrollX);
                            useEvent = true;
                            break;
                        }

                        Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_OBJECT_SELECTED, this.timelineId));
                        useEvent = true;
                        break;
                    }
            }

            // constraints.
            trackRect.x = 0;
            if (trackRect.y < 0) trackRect.y = 0;

            GUI.DragWindow();
            if (useEvent) Event.current.Use();
        }

        public void ShowTypeContextMenu(string conditionType)
        {
            var menu = new GenericMenu();

            if (!string.IsNullOrEmpty(conditionType))
            {

                // current.
                menu.AddDisabledItem(
                    new GUIContent(conditionType)
                );

                menu.AddSeparator(string.Empty);

                // set empty.
                menu.AddItem(
                    new GUIContent("Set Empty"),
                    false,
                    () =>
                    {
                        tackPoints[0].EmitUndo("Clear Condition Type");
                        foreach (var tackPoint in tackPoints)
                        {
                            tackPoint.conditionType = string.Empty;
                            tackPoint.conditionValue = string.Empty;
                        }
                        tackPoints[0].EmitSave();

                        Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_REFRESHTIMELINECONDITIONS, timelineId));
                    }
                );

                menu.ShowAsContext();
            }
        }

        private void ShowCommandContextMenu(float scrollX)
        {
            var targetFrame = GetFrameOnTimelineFromLocalMousePos(Event.current.mousePosition, scrollX);
            var menu = new GenericMenu();

            var menuItems = new Dictionary<string, OnAutomatineEvent.EventType>{
                {"Add New Tack", OnAutomatineEvent.EventType.EVENT_TIMELINE_ADDTACK},
				// {"Paste Tack", OnAutomatineEvent.EventType.EVENT_TACK_PASTE},
				
				// {"Copy This Timeline", OnAutomatineEvent.EventType.EVENT_TIMELINE_COPY},
				// {"Cut This Timeline", OnAutomatineEvent.EventType.EVENT_TIMELINE_CUT},
				{"Delete This Timeline", OnAutomatineEvent.EventType.EVENT_TIMELINE_DELETE},
            };

            foreach (var key in menuItems.Keys)
            {
                var eventType = menuItems[key];
                var enable = IsEnableEvent(eventType, targetFrame);
                if (enable)
                {
                    menu.AddItem(
                        new GUIContent(key),
                        false,
                        () =>
                        {
                            Emit(new OnAutomatineEvent(eventType, this.timelineId, targetFrame));
                        }
                    );
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(key));
                }
            }
            menu.ShowAsContext();
        }

        private bool IsEnableEvent(OnAutomatineEvent.EventType eventType, int frame)
        {
            switch (eventType)
            {
                // case AutomatineGUISettings.EVENT_OBJECT_SELECTED: {
                // 	return false;
                // }
                // case AutomatineGUISettings.EVENT_TACK_MOVED: {
                // 	return false;
                // }
                // case AutomatineGUISettings.EVENT_TACK_DELETE: {
                // 	return false;
                // }
                // case AutomatineGUISettings.EVENT_TACK_COPY: {
                // 	return false;
                // }

                // case AutomatineGUISettings.EVENT_OBJECT_SELECTED: {
                // 	return false;
                // }
                // case AutomatineGUISettings.EVENT_TIMELINE_SORTED: {
                // 	return false;
                // }
                case OnAutomatineEvent.EventType.EVENT_TIMELINE_ADDTACK:
                    {
                        if (frame < 0) return false;

                        foreach (var tackPoint in tackPoints)
                        {
                            if (tackPoint.ContainsFrame(frame))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                case OnAutomatineEvent.EventType.EVENT_TIMELINE_DELETE:
                    {
                        return true;
                    }
                case OnAutomatineEvent.EventType.EVENT_TIMELINE_COPY:
                    {
                        return true;
                    }


                default:
                    {
                        Debug.LogWarning("unhandled eventType IsEnableEvent:" + eventType);
                        return false;
                    }
            }
        }

        private int GetFrameOnTimelineFromLocalMousePos(Vector2 localMousePos, float scrollX)
        {
            var frameSourceX = localMousePos.x + Math.Abs(scrollX) - AutomatineGUISettings.TIMELINE_CONDITIONBOX_SPAN;
            return GetFrameOnTimelineFromAbsolutePosX(frameSourceX);
        }

        public static int GetFrameOnTimelineFromAbsolutePosX(float frameSourceX)
        {
            return (int)(frameSourceX / AutomatineGUISettings.TACK_FRAME_WIDTH);
        }

        private void DrawFrameRegion(float timelineScrollX, float timelineBodyY, float frameRegionWidth, int viewFrameWidth)
        {
            var limitRect = new Rect(0, 0, frameRegionWidth, AutomatineGUISettings.TACK_HEIGHT);

            // draw frame background.
            {
                DrawFrameBG(timelineScrollX, timelineBodyY, frameRegionWidth, AutomatineGUISettings.TACK_FRAME_HEIGHT, false);
            }

            // draw tack points & label on this track in range.
            {
                var index = 0;
                foreach (var tackPoint in tackPoints)
                {
                    var isUnderEvent = movingTackIds.Contains(tackPoint.tackId);
                    if (!movingTackIds.Any()) isUnderEvent = true;

                    // draw tackPoint on the frame.
                    tackPoint.DrawTack(limitRect, this.timelineId, timelineScrollX, timelineBodyY, isUnderEvent, viewFrameWidth);
                    index++;
                }
            }
        }

        public static void DrawFrameBG(float timelineScrollX, float timelineBodyY, float frameRegionWidth, float frameRegionHeight, bool showFrameCount)
        {
            var yOffset = timelineBodyY;

            // show 0 count.
            if (showFrameCount)
            {
                if (0 < AutomatineGUISettings.TACK_FRAME_WIDTH + timelineScrollX) GUI.Label(new Rect(timelineScrollX + 3, 0, 20, AutomatineGUISettings.CONDITION_INSPECTOR_FRAMECOUNT_HEIGHT), "0");
                yOffset = yOffset + AutomatineGUISettings.CONDITION_INSPECTOR_FRAMECOUNT_HEIGHT;
            }

            // draw 1st 1 frame.
            if (0 < AutomatineGUISettings.TACK_FRAME_WIDTH + timelineScrollX)
            {
                GUI.DrawTexture(new Rect(timelineScrollX, yOffset, AutomatineGUISettings.TACK_5FRAME_WIDTH, frameRegionHeight), AutomatineGUISettings.frameTex);
            }


            var repeatCount = (frameRegionWidth - timelineScrollX) / AutomatineGUISettings.TACK_5FRAME_WIDTH;
            for (var i = 0; i < repeatCount; i++)
            {
                var xPos = AutomatineGUISettings.TACK_FRAME_WIDTH + timelineScrollX + (i * AutomatineGUISettings.TACK_5FRAME_WIDTH);
                if (xPos + AutomatineGUISettings.TACK_5FRAME_WIDTH < 0) continue;

                if (showFrameCount)
                {
                    var frameCountStr = ((i + 1) * 5).ToString();
                    var span = 0;
                    if (2 < frameCountStr.Length) span = ((frameCountStr.Length - 2) * 8) / 2;
                    GUI.Label(new Rect(xPos + (AutomatineGUISettings.TACK_FRAME_WIDTH * 4) - span, 0, frameCountStr.Length * 10, AutomatineGUISettings.CONDITION_INSPECTOR_FRAMECOUNT_HEIGHT), frameCountStr);
                }
                var frameRect = new Rect(xPos, yOffset, AutomatineGUISettings.TACK_5FRAME_WIDTH, frameRegionHeight);
                GUI.DrawTexture(frameRect, AutomatineGUISettings.frameTex);
            }
        }

        public void SelectPreviousTackOf(string tackId)
        {
            var currentExistTacks = tackPoints.OrderBy(tack => tack.start).ToList();

            var cursoredTackIndex = currentExistTacks.FindIndex(tack => tack.tackId == tackId);

            if (cursoredTackIndex < 0) return;

            if (cursoredTackIndex == 0)
            {
                Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_OBJECT_SELECTED, this.timelineId));
                return;
            }

            var previousTack = currentExistTacks[cursoredTackIndex - 1];
            Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_OBJECT_SELECTED, previousTack.tackId));
        }

        public void SelectNextTackOf(string tackId)
        {
            var currentExistTacks = tackPoints.OrderBy(tack => tack.start).ToList();
            var currentTackIndex = currentExistTacks.FindIndex(tack => tack.tackId == tackId);

            if (0 <= currentTackIndex && currentTackIndex < currentExistTacks.Count - 1)
            {
                var nextTack = currentExistTacks[currentTackIndex + 1];
                Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_OBJECT_SELECTED, nextTack.tackId));
            }
        }

        public void SelectDefaultTackOrSelectTimeline()
        {
            if (tackPoints.Any())
            {
                var firstTackPoint = tackPoints.OrderBy(tack => tack.start).FirstOrDefault();
                Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_OBJECT_SELECTED, firstTackPoint.tackId));
                return;
            }

            Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_OBJECT_SELECTED, this.timelineId));
        }

        public void ActivateTacks(List<string> activeTackIds)
        {
            foreach (var tackPoint in tackPoints)
            {
                if (activeTackIds.Contains(tackPoint.tackId))
                {
                    tackPoint.SetActive();
                }
                else
                {
                    tackPoint.SetDeactive();
                }
            }
        }

        public void DeactivateTacks()
        {
            foreach (var tackPoint in tackPoints)
            {
                tackPoint.SetDeactive();
            }
        }

        public List<TackPoint> TacksByIds(List<string> tackIds)
        {
            var results = new List<TackPoint>();
            foreach (var tackPoint in tackPoints)
            {
                if (tackIds.Contains(tackPoint.tackId))
                {
                    results.Add(tackPoint);
                }
            }
            return results;
        }

        /**
			returns the tack which has nearlest start point.
		*/
        public TackPoint TackByStart(int startPos)
        {
            var candidates = tackPoints.OrderBy(tack => tack.start).ToList();

            if (!candidates.Any()) return null;

            var sameOrOverIndex = candidates.FindIndex(orderedTack => startPos <= orderedTack.start);

            // does not exist same or over -> unders only. return last one of ordered tacks.
            if (sameOrOverIndex < 0)
            {
                // return last one.
                return candidates.Last();
            }

            var sameOrOverTack = candidates[sameOrOverIndex];
            var underLastTackSource = candidates.Where((v, i) => i < sameOrOverIndex).ToList();

            // no unders -> return sameOrOver.
            if (!underLastTackSource.Any())
            {
                return sameOrOverTack;
            }

            var underLastTack = underLastTackSource.Last();

            // choose by difference.
            var sameOrOverStart = sameOrOverTack.start;
            var underEnd = underLastTack.start + underLastTack.span;

            var distanceBetweenOver = sameOrOverStart - startPos;
            var distanceBetweenUnder = startPos - underEnd;

            if (distanceBetweenOver <= distanceBetweenUnder)
            {
                // choose over.
                return sameOrOverTack;
            }

            // choose under. 
            return underLastTack;
        }

        public bool ContainsTackById(string tackId)
        {
            foreach (var tackPoint in tackPoints)
            {
                if (tackId == tackPoint.tackId) return true;
            }
            return false;
        }

        public List<TackPoint> TacksAfterTack(string tackId)
        {
            var targetTackIndex = tackPoints.FindIndex(tack => tack.tackId == tackId);
            if (targetTackIndex < 0) return null;

            var targetTackStart = tackPoints[targetTackIndex].start;
            var tacksAfterTack = tackPoints.Where(tack => targetTackStart < tack.start).ToList();
            return tacksAfterTack;
        }

        public void UpdateByTackMoved(string tackId)
        {
            movingTackIds.Clear();

            var movedTack = TacksByIds(new List<string> { tackId })[0];

            movedTack.ApplyDataToInspector();

            foreach (var targetTack in tackPoints)
            {
                if (targetTack.tackId == tackId) continue;


                // targetTack is unlimited tack.
                if (targetTack.span == AutomatineDefinitions.Tack.LIMIT_UNLIMITED)
                {
                    // moved tack's head is contained by target tack, update.
                    // m-tm-t
                    if (movedTack.start <= targetTack.start && targetTack.start + (targetTack.span - 1) <= movedTack.start)
                    {
                        targetTack.UpdatePos(movedTack.start + movedTack.span, targetTack.span);
                        continue;
                    }

                    // moved tack's tail is contained by target tack, update.
                    // m-tm-t
                    if (targetTack.start <= movedTack.start + (movedTack.span - 1))
                    {
                        targetTack.UpdatePos(movedTack.start + movedTack.span, targetTack.span);
                        continue;
                    }

                    // moved tack is fully contained by target tack, update.
                    // t-mm-t
                    if (targetTack.start < movedTack.start)
                    {
                        targetTack.UpdatePos(movedTack.start + movedTack.span, targetTack.span);
                        continue;
                    }

                    continue;
                }

                // movedTack is unlimited tack.
                if (movedTack.span == AutomatineDefinitions.Tack.LIMIT_UNLIMITED)
                {
                    // movedTack contained targetTack, delete.
                    if (movedTack.start <= targetTack.start)
                    {
                        Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_TACK_DELETED, targetTack.tackId));
                        continue;
                    }

                    // moved tack's head is contained by target tack, update.
                    // m-tm-t
                    if (movedTack.start <= targetTack.start + (targetTack.span - 1))
                    {
                        var resizedSpan = movedTack.start - targetTack.start;
                        targetTack.UpdatePos(targetTack.start, resizedSpan);
                        continue;
                    }

                    continue;
                }

                // not contained case.
                if (targetTack.start + (targetTack.span - 1) < movedTack.start) continue;
                if (movedTack.start + (movedTack.span - 1) < targetTack.start) continue;

                // movedTack contained targetTack, delete.
                if (movedTack.start <= targetTack.start && targetTack.start + (targetTack.span - 1) <= movedTack.start + (movedTack.span - 1))
                {
                    Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_TACK_DELETED, targetTack.tackId));
                    continue;
                }

                // moved tack's head is contained by target tack, update.
                // m-tm-t
                if (movedTack.start <= targetTack.start + (targetTack.span - 1) && targetTack.start + (targetTack.span - 1) <= movedTack.start + (movedTack.span - 1))
                {
                    var resizedSpan = movedTack.start - targetTack.start;
                    targetTack.UpdatePos(targetTack.start, resizedSpan);
                    continue;
                }

                // moved tack's head is contained by target tack's tail, update.
                // t-mt-m
                if (targetTack.start <= movedTack.start + (movedTack.span - 1) && movedTack.start <= targetTack.start)
                {
                    var newStartPos = movedTack.start + movedTack.span;
                    var resizedSpan = targetTack.span - (newStartPos - targetTack.start);
                    targetTack.UpdatePos(newStartPos, resizedSpan);
                    continue;
                }

                // moved tack is fully contained by target tack, update.
                // t-mm-t
                if (targetTack.start < movedTack.start && movedTack.start + movedTack.span < targetTack.start + targetTack.span)
                {
                    var resizedSpanPoint = movedTack.start - 1;
                    var resizedSpan = resizedSpanPoint - targetTack.start + 1;
                    targetTack.UpdatePos(targetTack.start, resizedSpan);
                    continue;
                }
            }
        }

        public void SetMovingTack(string tackId)
        {
            movingTackIds = new List<string> { tackId };
        }

        public void AddNewTackToEmptyFrame(int frame)
        {
            tackPoints.Add(new TackPoint(
                tackCount,
                AutomatineGUISettings.DEFAULT_TACK_NAME,
                frame,
                AutomatineGUISettings.DEFAULT_TACK_SPAN
            ));

            tackCount = tackCount + 1;

            ApplyTextureToTacks(index);
        }

        public void DeleteTackById(string tackId)
        {
            var deletedTackIndex = tackPoints.FindIndex(tack => tack.tackId == tackId);
            if (deletedTackIndex == -1) return;

            tackPoints.RemoveAt(deletedTackIndex);
        }

        public void ApplyDataToInspector()
        {
            if (timelineTrackInspector == null) timelineTrackInspector = ScriptableObject.CreateInstance("TimelineTrackInspector") as TimelineTrackInspector;

            UpdateCondition();
            timelineTrackInspector.timelineTrack = this;
        }
    }
}