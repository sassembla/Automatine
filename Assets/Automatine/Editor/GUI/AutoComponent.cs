using UnityEngine;
using UnityEditor;

using System;
using System.Linq;
using System.Collections.Generic;

namespace Automatine
{
    [Serializable]
    public class AutoComponent
    {
        public static Action<OnAutomatineEvent> Emit;

        [SerializeField] private AutoComponentInspector autoComponentInspector;

        [CustomEditor(typeof(AutoComponentInspector))]
        public class AutoComponentInspectorGUI : Editor
        {
            public override void OnInspectorGUI()
            {
                var insp = ((AutoComponentInspector)target).autoComponent;

                var autoName = insp.name;
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Auto Name");
                    var result = GUILayout.TextField(autoName);
                    if (result != autoName)
                    {
                        Debug.LogWarning("からっぽ、メソッド化できない文字、同じ名前が既にある(これ以外)、その他、、");

                        if (string.IsNullOrEmpty(result))
                        {
                            Debug.LogError("empty is not arrowed.");
                        }

                        // {
                        // 	Debug.LogError("same name is exists.");
                        // }

                        insp.EmitUndo("Change Auto Name");
                        insp.name = result;
                        insp.EmitSave();
                    }
                }
            }
        }

        [SerializeField] private bool active;
        [SerializeField] private List<TimelineTrack> timelineTracks;

        [SerializeField] public string autoId;

        [SerializeField] public string name;
        [SerializeField] public string info;
        [SerializeField] public List<string> comments = new List<string>();

        [SerializeField] public List<ChangerPlate> rootChangers = new List<ChangerPlate>();
        [SerializeField] public List<ChangerPlate> changers = new List<ChangerPlate>();

        private int timelineCount;


        public AutoComponent() { }

        public AutoComponent(string name, string info, List<TimelineTrack> timelineTracks)
        {
            this.active = false;

            this.autoId = AutomatineGUISettings.ID_HEADER_AUTO + Guid.NewGuid().ToString();

            this.name = name;
            this.info = info;
            this.comments = new List<string>();

            this.timelineTracks = new List<TimelineTrack>(timelineTracks);
        }

        public AutoComponent(AutoData autoData)
        {
            this.active = false;

            this.autoId = autoData.autoId;

            this.name = autoData.name;
            this.info = autoData.info;
            this.comments = autoData.comments;

            var loadedTimelines = new List<TimelineTrack>();
            foreach (var timeline in autoData.timelines)
            {
                loadedTimelines.Add(new TimelineTrack(timeline, timelineCount));
                timelineCount = timelineCount + 1;
            }
            this.timelineTracks = loadedTimelines;
        }

        public void EmitUndo(string message)
        {
            Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_UNDO, this.autoId, -1, message));
        }

        public void EmitSave()
        {
            Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_SAVE, this.autoId, -1));
        }

        public bool IsActive()
        {
            return active;
        }

        public void SetActive()
        {
            active = true;

            ApplyDataToInspector();
            Selection.activeObject = autoComponentInspector;
        }

        public void ShowInspector()
        {
            Debug.LogError("autoのinspectorをセットする。");
        }

        public void SetDeactive()
        {
            active = false;
        }

        public List<TimelineTrack> TimelineTracks()
        {
            return new List<TimelineTrack>(timelineTracks);
        }

        public void UpdateByTackMoved(int timelineIndx, string tackId)
        {
            timelineTracks[timelineIndx].UpdateByTackMoved(tackId);
        }


        public void DeleteConditionType(string deletingConditonTypeName)
        {
            foreach (var timeline in timelineTracks)
            {
                timeline.DeleteConditionType(deletingConditonTypeName);
            }
        }

        public void DeleteConditionValue(string deletingConditonValueName)
        {
            foreach (var timeline in timelineTracks)
            {
                timeline.DeleteConditionValue(deletingConditonValueName);
            }
        }



        public void AddRootChanger(ChangerPlate rootChanger)
        {
            rootChangers.Add(rootChanger);
        }

        public void AddChanger(ChangerPlate changer)
        {
            changers.Add(changer);
        }


        /*
			method for changers.
		*/
        public List<ChangerPlate> ChangerPlates()
        {
            return changers;
        }


        /*
			method for timelines.
		*/
        public float DrawTimelines(AutoComponent auto, float yOffsetPos, float xScrollIndex, float trackWidth)
        {
            var yIndex = yOffsetPos;

            var totalTimelineHeight = 0f;

            for (var windowIndex = 0; windowIndex < timelineTracks.Count; windowIndex++)
            {
                var timelineTrack = timelineTracks[windowIndex];

                timelineTrack.DrawTimelineTrack(yOffsetPos, xScrollIndex, yIndex, trackWidth);
                var trackHeight = timelineTrack.TrackHeight();

                // set next y index.
                yIndex = yIndex + trackHeight + AutomatineGUISettings.TIMELINE_SPAN;
                totalTimelineHeight += trackHeight + AutomatineGUISettings.TIMELINE_SPAN;
            }

            return totalTimelineHeight;
        }

        public float TimelinesTotalHeight()
        {
            var totalHeight = 0f;
            foreach (var timelineTrack in timelineTracks)
            {
                totalHeight += timelineTrack.Height();
            }
            return totalHeight;
        }

        public List<TimelineTrack> TimelinesByIds(List<string> timelineIds)
        {
            var results = new List<TimelineTrack>();
            foreach (var timelineTrack in timelineTracks)
            {
                if (timelineIds.Contains(timelineTrack.timelineId))
                {
                    results.Add(timelineTrack);
                }
            }
            return results;
        }

        public TimelineTrack TimelineByTack(string tackId)
        {
            foreach (var timeline in timelineTracks)
            {
                if (timeline.ContainsTackById(tackId)) return timeline;
            }
            return null;
        }

        public TackPoint TackById(string tackId)
        {
            foreach (var timelineTrack in timelineTracks)
            {
                var tack = timelineTrack.TackById(tackId);
                if (tack != null) return tack;
            }
            return null;
        }

        public List<TackPoint> TackByFrame(int frame)
        {
            var tacks = new List<TackPoint>();
            foreach (var timelineTrack in timelineTracks)
            {
                var tack = timelineTrack.TackByFrame(frame);
                if (tack != null) tacks.Add(tack);
            }
            return tacks;
        }

        public void SelectAboveObjectById(string currentActiveObjectId)
        {
            if (AutomatineGUIWindow.IsTimelineId(currentActiveObjectId))
            {
                var candidateTimelines = timelineTracks.OrderBy(timeline => timeline.GetIndex()).ToList();
                var currentTimelineIndex = candidateTimelines.FindIndex(timeline => timeline.timelineId == currentActiveObjectId);

                if (0 < currentTimelineIndex)
                {
                    var targetTimeline = timelineTracks[currentTimelineIndex - 1];
                    Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_OBJECT_SELECTED, targetTimeline.timelineId));
                    return;
                }

                return;
            }

            if (AutomatineGUIWindow.IsTackId(currentActiveObjectId))
            {
                /*
					select another timeline's same position tack.
				*/
                var currentActiveTack = TackById(currentActiveObjectId);

                if (currentActiveTack == null) return;

                var currentActiveTackStart = currentActiveTack.start;
                var currentTimelineId = currentActiveTack.parentTimelineId;

                var aboveTimeline = AboveTimeline(currentTimelineId);
                if (aboveTimeline != null)
                {
                    var nextActiveTack = aboveTimeline.TackByStart(currentActiveTackStart);

                    if (nextActiveTack != null)
                    {
                        Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_OBJECT_SELECTED, nextActiveTack.tackId));
                    }
                    else
                    {
                        // no tack found, select timeline itself.
                        Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_OBJECT_SELECTED, aboveTimeline.timelineId));
                    }
                }
                return;
            }
        }

        public void SelectBelowObjectById(string currentActiveObjectId)
        {
            if (AutomatineGUIWindow.IsTimelineId(currentActiveObjectId))
            {
                var cursoredTimelineIndex = timelineTracks.FindIndex(timeline => timeline.timelineId == currentActiveObjectId);
                if (cursoredTimelineIndex < timelineTracks.Count - 1)
                {
                    var targetTimelineIndex = cursoredTimelineIndex + 1;
                    var targetTimeline = timelineTracks[targetTimelineIndex];
                    Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_OBJECT_SELECTED, targetTimeline.timelineId));
                }
                return;
            }

            if (AutomatineGUIWindow.IsTackId(currentActiveObjectId))
            {
                /*
					select another timeline's same position tack.
				*/
                var currentActiveTack = TackById(currentActiveObjectId);

                if (currentActiveTack == null) return;

                var currentActiveTackStart = currentActiveTack.start;
                var currentTimelineId = currentActiveTack.parentTimelineId;

                var belowTimeline = BelowTimeline(currentTimelineId);
                if (belowTimeline != null)
                {
                    var nextActiveTack = belowTimeline.TackByStart(currentActiveTackStart);
                    if (nextActiveTack != null)
                    {
                        Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_OBJECT_SELECTED, nextActiveTack.tackId));
                    }
                    else
                    {
                        // no tack found, select timeline itself.
                        Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_OBJECT_SELECTED, belowTimeline.timelineId));
                    }
                }
                return;
            }
        }

        private TimelineTrack AboveTimeline(string baseTimelineId)
        {
            var baseIndex = timelineTracks.FindIndex(timeline => timeline.timelineId == baseTimelineId);
            if (0 < baseIndex) return timelineTracks[baseIndex - 1];
            return null;
        }

        private TimelineTrack BelowTimeline(string baseTimelineId)
        {
            var baseIndex = timelineTracks.FindIndex(timeline => timeline.timelineId == baseTimelineId);
            if (baseIndex < timelineTracks.Count - 1) return timelineTracks[baseIndex + 1];
            return null;
        }

        public void SelectPreviousTackOfTimelines(string currentActiveObjectId)
        {
            /*
				if current active id is tack, select previous one.
				and if active tack is the head of timeline, select timeline itself.
			*/
            if (AutomatineGUIWindow.IsTackId(currentActiveObjectId))
            {
                foreach (var timelineTrack in timelineTracks)
                {
                    timelineTrack.SelectPreviousTackOf(currentActiveObjectId);
                }
            }
        }

        public void SelectNextTackOfTimelines(string currentActiveObjectId)
        {
            // if current active id is timeline, select first tack of that.
            if (AutomatineGUIWindow.IsTimelineId(currentActiveObjectId))
            {
                foreach (var timelineTrack in timelineTracks)
                {
                    if (timelineTrack.timelineId == currentActiveObjectId)
                    {
                        timelineTrack.SelectDefaultTackOrSelectTimeline();
                    }
                }
                return;
            }

            // if current active id is tack, select next one.
            if (AutomatineGUIWindow.IsTackId(currentActiveObjectId))
            {
                foreach (var timelineTrack in timelineTracks)
                {
                    timelineTrack.SelectNextTackOf(currentActiveObjectId);
                }
            }
        }

        public bool IsActiveTimelineOrContainsActiveObject(int index)
        {
            if (index < timelineTracks.Count)
            {
                var currentTimeline = timelineTracks[index];
                if (currentTimeline.IsActive()) return true;
                return currentTimeline.ContainsActiveTack();
            }
            return false;
        }

        public int GetStartFrameById(string objectId)
        {
            if (AutomatineGUIWindow.IsTimelineId(objectId))
            {
                return -1;
            }

            if (AutomatineGUIWindow.IsTackId(objectId))
            {
                var targetContainedTimelineIndex = GetTackContainedTimelineIndex(objectId);
                if (0 <= targetContainedTimelineIndex)
                {
                    var foundStartFrame = timelineTracks[targetContainedTimelineIndex].GetStartFrameById(objectId);
                    if (0 <= foundStartFrame) return foundStartFrame;
                }
            }

            return -1;
        }

        public void SelectTackAtFrame(int frameCount)
        {
            if (timelineTracks.Any())
            {
                var firstTimelineTrack = timelineTracks[0];
                var nextActiveTack = firstTimelineTrack.TackByStart(frameCount);
                if (nextActiveTack != null)
                {
                    Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_OBJECT_SELECTED, nextActiveTack.tackId));
                }
                else
                {
                    // no tack found, select timeline itself.
                    Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_OBJECT_SELECTED, firstTimelineTrack.timelineId));
                }
            }
        }

        public void DeactivateAllObjects()
        {
            foreach (var timelineTrack in timelineTracks)
            {
                timelineTrack.SetDeactive();
                timelineTrack.DeactivateTacks();
            }
        }

        public void SetMovingTackToTimelimes(string tackId)
        {
            foreach (var timelineTrack in timelineTracks)
            {
                if (timelineTrack.ContainsTackById(tackId))
                {
                    timelineTrack.SetMovingTack(tackId);
                }
            }
        }

        /**
			set active to active objects, and set deactive to all other objects.
			affect to records of Undo/Redo.
		*/
        public void ActivateObjectsAndDeactivateOthers(List<string> activeObjectIds)
        {
            foreach (var timelineTrack in timelineTracks)
            {
                if (activeObjectIds.Contains(timelineTrack.timelineId)) timelineTrack.SetActive();
                else timelineTrack.SetDeactive();

                timelineTrack.ActivateTacks(activeObjectIds);
            }
        }

        public int GetTackContainedTimelineIndex(string tackId)
        {
            return timelineTracks.FindIndex(timelineTrack => timelineTrack.ContainsTackById(tackId));
        }

        public void AddNewTackToTimeline(string timelineId, int frame)
        {
            var targetTimeline = TimelinesByIds(new List<string> { timelineId })[0];
            targetTimeline.AddNewTackToEmptyFrame(frame);
        }

        public void DeleteObjectById(string deletedObjectId)
        {
            if (AutomatineGUIWindow.IsTackId(deletedObjectId))
            {
                foreach (var timelineTrack in timelineTracks)
                {
                    timelineTrack.DeleteTackById(deletedObjectId);
                }
                return;
            }

            timelineTracks.Remove(timelineTracks.Where(tl => tl.timelineId == deletedObjectId).FirstOrDefault());
        }

        public void AddTimeline()
        {
            timelineTracks.Add(new TimelineTrack(timelineCount, "New Timeline", new List<TackPoint>()));
            timelineCount = timelineCount + 1;
        }

        public void UpdateTimelinesCondition()
        {
            foreach (var timelineTrack in timelineTracks)
            {
                timelineTrack.UpdateCondition();
            }
        }

        public void ApplyDataToInspector()
        {
            if (autoComponentInspector == null) autoComponentInspector = ScriptableObject.CreateInstance("AutoComponentInspector") as AutoComponentInspector;

            autoComponentInspector.autoComponent = this;
        }
    }
}