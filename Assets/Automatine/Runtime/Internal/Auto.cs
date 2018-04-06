using System;
using System.Collections.Generic;

namespace Automatine
{
    public class AutomatineDefinitions
    {
        public class Tack
        {
            public const int LIMIT_UNLIMITED = -1;
        }
        public class Changer
        {
            public const string CLASS_HEADER = "Changer_";
            public const string INTERFACE_NAME = "IAutoChanger";
        }
    }

    /**
		Automatine. contains timelines with tacks.
		represents collections of behaviour which is based on the frame action.
	*/
    public class Auto<InitialParamType, UpdateParamType>
    {
        public readonly string autoId;
        public readonly string autoInfo;

        private readonly List<IAutoChanger> rootChangers;
        private readonly List<IAutoChanger> changers;

        private int startFrame;

        private InitialParamType initialParam;
        private List<Timeline<InitialParamType, UpdateParamType>> timelines;

        private bool updated = false;
        private bool consumed = false;
        private int completedFrame = -1;

        private bool justConsumed = false;

        private List<IAutoChanger> stackedChangers = new List<IAutoChanger>();

        private int[] defaultConditions;

        public Auto(
            string autoInfo,
            int startFrame,
            InitialParamType initialParam,
            params Timeline<InitialParamType,
            UpdateParamType>[] timelines
        )
        {
            this.autoId = Guid.NewGuid().ToString();
            this.autoInfo = autoInfo;

            this.startFrame = startFrame;

            this.initialParam = initialParam;

            // initialize all timelines.
            this.timelines = new List<Timeline<InitialParamType, UpdateParamType>>(timelines);
            for (var i = 0; i < this.timelines.Count; i++)
            {
                var tl = this.timelines[i];
                tl._SetupTimeline(this.initialParam, this);
            }
        }


        /**
			with rootChangers and changers.
		*/
        public Auto(
            string autoInfo,
            int startFrame,
            InitialParamType initialParam,
            List<IAutoChanger> rootChangers,
            List<IAutoChanger> changers,
            params Timeline<InitialParamType, UpdateParamType>[] timelines
        ) : this(
            autoInfo,
            startFrame,
            initialParam,
            timelines
        )
        {
            this.rootChangers = rootChangers;
            this.changers = changers;
        }

        /**
			return frame for run specific distance.
			return the frame for running this Auto with specific distance.
		*/
        public int NextFrame(int distance)
        {
            /*
				if not updated yet, start frame is ready but not overed.
				next frame will be initialized frame(startFrame) + (distance - 1).
			*/
            if (!updated)
            {
                return startFrame + (distance - 1);
            }

            return completedFrame + distance;
        }

        private int _overrunCount;
        public int OverrunCount()
        {
            return _overrunCount;
        }

        public Auto<InitialParamType, UpdateParamType> ChangeTo(Auto<InitialParamType, UpdateParamType> newAuto)
        {
            newAuto.SetDefaultConditions(Conditions());
            return newAuto;
        }

        public Auto<InitialParamType, UpdateParamType> ChangeToWithoutKeepingConditions(Auto<InitialParamType, UpdateParamType> newAuto)
        {
            return newAuto;
        }

        /**
			return function what returns auto which is constructed by RuntimeAutoData.
		*/
        public static Func<int, InitialParamType, Auto<InitialParamType, UpdateParamType>> RuntimeAutoGenerator(RuntimeAutoData runtimeAutoData)
        {
            return (int startFrame, InitialParamType initialParam) =>
            {
                var autoInfo = "generated";

                var newRootChangers = new List<IAutoChanger>();
                for (var i = 0; i < runtimeAutoData.rootChangers.Count; i++)
                {
                    var rootChangerSource = runtimeAutoData.rootChangers[i];
                    newRootChangers.Add(ChangerInstanceByName(rootChangerSource.changerName));
                }

                var newChangers = new List<IAutoChanger>();
                for (var i = 0; i < runtimeAutoData.changers.Count; i++)
                {
                    var changerSource = runtimeAutoData.changers[i];
                    newChangers.Add(ChangerInstanceByName(changerSource.changerName));
                }

                var newTimelines = new List<Timeline<InitialParamType, UpdateParamType>>();

                for (var i = 0; i < runtimeAutoData.timelines.Count; i++)
                {
                    var timelineSource = runtimeAutoData.timelines[i];
                    var timelineInfo = "generated";
                    var tacksSources = timelineSource.tacks;
                    var tacks = new List<Tack<InitialParamType, UpdateParamType>>();

                    for (var j = 0; j < tacksSources.Count; j++)
                    {
                        var tackSource = tacksSources[j];
                        var tackInfo = "generated";
                        var tackStart = tackSource.start;
                        var tackSpan = tackSource.span;

                        var tackCondition = ConditionGateway.ConditionFromString(tackSource.conditionType, tackSource.conditionValue);

                        var routines = new List<Func<RoutineBase<InitialParamType, UpdateParamType>>>();
                        for (var k = 0; k < tackSource.routines.Count; k++)
                        {
                            var routine = tackSource.routines[k];
                            var routineName = routine.routineName;
                            routines.Add(
                                () =>
                                {
                                    return new RoutineContexts<InitialParamType, UpdateParamType>(routineName);
                                }
                            );
                        }

                        tacks.Add(new Tack<InitialParamType, UpdateParamType>(tackInfo, tackStart, tackSpan, tackCondition, routines.ToArray()));
                    }

                    newTimelines.Add(new Timeline<InitialParamType, UpdateParamType>(timelineInfo, tacks.ToArray()));
                }

                return new Auto<InitialParamType, UpdateParamType>(
                    autoInfo,
                    startFrame,
                    initialParam,
                    newRootChangers,
                    newChangers,
                    newTimelines.ToArray()
                );
            };
        }

        protected static IAutoChanger ChangerInstanceByName(string changerClassName)
        {
            var type = Type.GetType(changerClassName);
            if (type == null) throw new Exception("failed to get changer by type of classname:" + changerClassName);
            return Activator.CreateInstance(type) as IAutoChanger;
        }

        protected void SetDefaultConditions(int[] sourceConditions)
        {
            if (updated) throw new Exception("should not do. auto is already running.");
            defaultConditions = new int[sourceConditions.Length];
            Array.Copy(sourceConditions, defaultConditions, sourceConditions.Length);
        }

        public void ResetFrame(int newFrame)
        {
            if (newFrame < 0) throw new Exception("can not use negative frame in ResetFrame(frame).");

            // not Updated yet -> set startFrame to newFrame.
            if (!updated)
            {
                startFrame = newFrame;
            }
            // if already Updated at least once, change completedFrame. 
            else
            {
                completedFrame = newFrame - 1;
            }
        }

        public bool IsSameAuto(Auto<InitialParamType, UpdateParamType> pr)
        {
            if (pr.autoId == this.autoId) return true;
            return false;
        }

        /**
			update all timelines in this Auto.
		*/
        public void Update(int frame, UpdateParamType input)
        {
            // minus, or same frame will be ignored.
            if (frame < 0) throw new Exception("can not use negative frame in Update(frame).");
            if (frame <= completedFrame) return;

            if (consumed)
            {
                _overrunCount = frame - completedFrame;
                justConsumed = false;
                return;
            }

            ClearStackedChangers();

            // new local frame should be "completedFrame + 1".
            int localFrame = completedFrame + 1;

            // calc count distance of running in this Update.
            // if inputted frame is larger than "completedFrame + 1", distance is larger than 1.
            int distance = frame - completedFrame;

            // if the first time of Update,
            if (!updated)
            {
                if (frame == startFrame)
                {
                    localFrame = frame;
                    distance = 1;
                }

                // inputted frame is not initialized startFrame,
                // run startFrame to inputted frame.
                else
                {
                    localFrame = startFrame;
                    distance = frame - startFrame + 1;
                }
            }

            // Debug.LogError("distance:" + distance + " vs frame:" + frame);

            for (int i = 0; i < distance; i++)
            {
                if (consumed)
                {
                    _overrunCount++;
                }

                // update completedFrame before running. prepare for "break" of timeline & tack.
                completedFrame = localFrame;

                if (!consumed)
                {
                    RunTimelines(localFrame, input);
                }

                for (var j = 0; j < timelines.Count; j++)
                {
                    var tl = timelines[j];
                    if (!tl.timelineConsumed)
                    {
                        break;
                    }

                    // all timelimes are consumed.
                    if (j == timelines.Count - 1)
                    {
                        consumed = true;
                        justConsumed = true;
                    }
                }

                localFrame++;
            }
            updated = true;
        }

        private void RunTimelines(int localFrame, UpdateParamType input)
        {
            for (var i = 0; i < timelines.Count; i++)
            {
                var timeline = timelines[i];
                // ignore consumed timeline.
                if (timeline.timelineConsumed)
                {
                    continue;
                }

                timeline._ProceedTimeline(localFrame, input, this);

                /*
					checking this auto is consumed or not. 
					if this auto is already consumed, stop running other timelines.
					and these rest timelines are left. they has possibility of inheriting to other auto.
				*/
                updated = true;
                if (consumed)
                {
                    return;
                }
            }
        }

        public int CurrentFrame()
        {
            if (!updated)
            {
                return startFrame;
            }
            return completedFrame;
        }

        public int AssumedRestFrame()
        {
            var longestSpan = 0;
            for (var i = 0; i < timelines.Count; i++)
            {
                var tl = timelines[i];
                if (tl.timelineConsumed)
                {
                    continue;
                }

                var assumeTimelineSpan = tl._AssumeRestFrameOnTimeline();
                if (assumeTimelineSpan == AutomatineDefinitions.Tack.LIMIT_UNLIMITED)
                {
                    return AutomatineDefinitions.Tack.LIMIT_UNLIMITED;
                }

                if (longestSpan < assumeTimelineSpan)
                {
                    longestSpan = assumeTimelineSpan;
                }
            }
            return longestSpan;
        }

        /**
			return any conditions if this auto is
				- not consumed yet
				- just consumed

			return any conditions of timelines which are
				- timeline is not inherited
				- timeline is not broken
		*/
        public int[] Conditions()
        {
            if (consumed && !justConsumed)
            {
                return _EmptyConditions();
            }
            if (!updated)
            {
                return _ConditionsBeforeUpdate();
            }

            return _ConditionsAtCurrentFrame();
        }

        protected int[] _EmptyConditions()
        {
            // fill conditions to -1.
            int[] conditions = new int[CollectedConditions.conditions.Length];
            for (int i = 0; i < conditions.Length; i++)
            {
                conditions[i] = -1;
            }
            return conditions;
        }

        protected int[] _ConditionsAtCurrentFrame()
        {
            var conditions = _EmptyConditions();

            for (var i = 0; i < timelines.Count; i++)
            {
                var tl = timelines[i];
                if (tl.inherited)
                {
                    continue;
                }
                if (tl.broke)
                {
                    continue;
                }

                ConditionValue_Type val_type = tl._CurrentTimelineConditionValueAndType(completedFrame);
                var val = val_type.val;
                var type = val_type.type;

                // ignore empty condition.
                if (type == -1)
                {
                    continue;
                }

                if (conditions[type] != -1)
                {
                    throw new Exception("Conditions: setting is bad. same conditions are already appeared in this Auto! type:" + type + " at Auto:" + autoInfo + " autoId:" + autoId);
                }

                conditions[type] = val;
            }
            return conditions;
        }

        /*
			returns 1st frame conditions of this auto.
		*/
        protected int[] _ConditionsBeforeUpdate()
        {
            if (defaultConditions != null)
            {
                return defaultConditions;
            }

            var conditions = _EmptyConditions();

            for (var i = 0; i < timelines.Count; i++)
            {
                var tl = timelines[i];
                if (tl.inherited)
                {
                    continue;
                }
                if (tl.broke)
                {
                    continue;
                }

                ConditionValue_Type val_type = tl._TimelineConditionValueAndTypeBeforeUpdate();
                var val = val_type.val;
                var type = val_type.type;

                // ignore empty condition.
                if (type == -1)
                {
                    continue;
                }

                if (conditions[type] != -1)
                {
                    throw new Exception("Conditions: setting is bad. same conditions are already appeared in this Auto! type:" + type + " at Auto:" + autoInfo + " autoId:" + autoId);
                }

                conditions[type] = val;
            }
            return conditions;
        }

        /**
			export timelines which contains the type of specified condition.
		*/
        public List<Timeline<InitialParamType, UpdateParamType>> ExportTimelines(params Type[] conditionTypes)
        {
            var targetTypes = new List<int>();
            for (var i = 0; i < conditionTypes.Length; i++)
            {
                var cond = conditionTypes[i];
                var targetConditionIndex = ConditionGateway.IndexOfConditionType(cond);

                if (targetTypes.Contains(targetConditionIndex))
                {
                    continue;
                }

                targetTypes.Add(targetConditionIndex);
            }

            return _MatchedTimelines(targetTypes);
        }

        protected List<Timeline<InitialParamType, UpdateParamType>> _MatchedTimelines(List<int> targetTypes)
        {
            var matchedTimelines = new List<Timeline<InitialParamType, UpdateParamType>>();

            for (var i = 0; i < timelines.Count; i++)
            {
                var tl = timelines[i];
                if (tl.timelineConsumed)
                {
                    continue;
                }

                var targetTypeIndex = tl._StaticConditionOfTimeline();

                if (targetTypes.Contains(targetTypeIndex))
                {
                    matchedTimelines.Add(tl);

                    // consume slot for this type.
                    targetTypes.Remove(targetTypeIndex);
                }
            }

            return matchedTimelines;
        }



        /**
			inherit tilemlines to this Auto.
		*/
        public void InheritTimelines(List<Timeline<InitialParamType, UpdateParamType>> inheritingTimelines)
        {
            for (var i = 0; i < inheritingTimelines.Count; i++)
            {
                var t = inheritingTimelines[i];
                if (t.timelineConsumed)
                {
                    continue;
                }

                t.inherited = true;
                t._UpdateTimelineParent(this);
                timelines.Add(t);
            }
        }


        /**
			return true if this Auto is already done & should falldown to other Auto.
		*/
        public bool ShouldFalldown(int frame)
        {
            if (!updated)
            {
                return false;
            }

            if (consumed && completedFrame < frame)
            {
                return true;
            }

            return false;
        }

        public bool ShouldFalldownInNextFrame(int frame)
        {
            if (!updated)
            {
                return false;
            }

            if (consumed && completedFrame == frame)
            {
                return true;
            }

            return false;
        }

        public bool JustConsumed(int frame)
        {
            if (!updated)
            {
                return false;
            }

            if (consumed && completedFrame == frame)
            {
                return true;
            }

            return false;
        }


        public void BreakAuto()
        {
            consumed = true;
            justConsumed = true;
        }

        public void BreakTimelines(params Type[] conditionTypes)
        {
            if (!updated) throw new Exception("should Update('frame') first.");
            var conditionalTimelines = ExportTimelines(conditionTypes);

            for (var i = 0; i < conditionalTimelines.Count; i++)
            {
                var tl = conditionalTimelines[i];

                if (tl.timelineConsumed)
                {
                    continue;
                }

                if (tl.broke)
                {
                    continue;
                }

                tl._BreakTimeline();
            }

            for (var i = 0; i < timelines.Count; i++)
            {
                var tl = timelines[i];
                if (!tl.timelineConsumed)
                {
                    return;
                }
            }
            BreakAuto();
        }


        /*
			changers 
		*/
        public void StackChanger(IAutoChanger changer)
        {
            stackedChangers.Add(changer);
        }

        public List<IAutoChanger> StackedChangers()
        {
            return stackedChangers;
        }

        protected void ClearStackedChangers()
        {
            stackedChangers.Clear();
        }

        public List<IAutoChanger> StackedEffectiveChangers()
        {
            return EffectiveChangers(stackedChangers);
        }

        public List<IAutoChanger> RootChangers()
        {
            if (rootChangers == null)
            {
                return new List<IAutoChanger>();
            }
            return rootChangers;
        }

        public List<IAutoChanger> Changers()
        {
            if (changers == null)
            {
                return new List<IAutoChanger>();
            }
            {
                return changers;
            }
        }

        public List<IAutoChanger> EffectiveChangers(List<IAutoChanger> candidates)
        {
            var effectiveChangers = new List<IAutoChanger>();
            for (var i = 0; i < candidates.Count; i++)
            {
                var changer = candidates[i];
                if (changer.IsEffective<InitialParamType, UpdateParamType>(this))
                {
                    effectiveChangers.Add(changer);
                }
            }
            return effectiveChangers;
        }

        public Auto<InitialParamType, UpdateParamType> EmitChanger(int frame, InitialParamType initParam, IAutoChanger emitChanger)
        {
            var baseConditions = Conditions();
            var nextAuto = emitChanger.Changer<InitialParamType, UpdateParamType>()(this, frame, initParam);
            nextAuto.SetDefaultConditions(baseConditions);
            return nextAuto;
        }

        public Auto<InitialParamType, UpdateParamType> EmitChangerWithoutKeepingConditions(int frame, InitialParamType initParam, IAutoChanger emitChanger)
        {
            var nextAuto = emitChanger.Changer<InitialParamType, UpdateParamType>()(this, frame, initParam);
            return nextAuto;
        }

        public Auto<InitialParamType, UpdateParamType> EmitChangers(int frame, InitialParamType initParam, List<IAutoChanger> emitChangers)
        {
            var currentAutoId = this.autoId;
            var baseConditions = Conditions();

            for (var i = 0; i < emitChangers.Count; i++)
            {
                var emitChanger = emitChangers[i];
                var result = emitChanger.Changer<InitialParamType, UpdateParamType>()(this, frame, initParam);
                if (currentAutoId != result.autoId)
                {
                    result.SetDefaultConditions(baseConditions);
                    return result;
                }
            }
            return this;
        }

        public Auto<InitialParamType, UpdateParamType> EmitChangersWithoutKeepingConditions(int frame, InitialParamType initParam, List<IAutoChanger> emitChangers)
        {
            var currentAutoId = this.autoId;

            for (var i = 0; i < emitChangers.Count; i++)
            {
                var emitChanger = emitChangers[i];
                var result = emitChanger.Changer<InitialParamType, UpdateParamType>()(this, frame, initParam);
                if (currentAutoId != result.autoId)
                {
                    return result;
                }
            }
            return this;
        }

        /*
			methods for conditions(int[]).
		*/

        /**
			check if the part of the condition is containd or not containd.
		*/
        public bool ContainsCondition<T>(T condition) where T : IConvertible
        {
            var currentCondition = Conditions();
            return ConditionGateway.ContainsCondition(currentCondition, condition);
        }

        /**
			check if the part of the condition is not containd or containd.
		*/
        public bool NotContainsCondition<T>(T condition) where T : IConvertible
        {
            var currentCondition = Conditions();
            return ConditionGateway.NotContainsCondition(currentCondition, condition);
        }

        /**
			check if every part of conditions are containd or not containd.
		*/
        public bool ContainsAllConditions(params object[] conditions)
        {
            var currentCondition = Conditions();
            return ConditionGateway.ContainsAllConditions(currentCondition, conditions);
        }

        /**
			check if every part of conditions are not containd or containd.
		*/
        public bool NotContainsAllConditions(params object[] conditions)
        {
            var currentCondition = Conditions();
            return ConditionGateway.NotContainsAllConditions(currentCondition, conditions);
        }

        /**
			check if all conditions are same or not.
		*/
        public bool SameConditions(Auto<InitialParamType, UpdateParamType> targetAuto)
        {
            var currentCondition = Conditions();
            var targetConditions = targetAuto.Conditions();
            return ConditionGateway.SameConditions(currentCondition, targetConditions);
        }

        public bool SameConditions(int[] conditions)
        {
            var currentCondition = Conditions();
            return ConditionGateway.SameConditions(currentCondition, conditions);
        }


        public bool Contains<T>(T condition) where T : IConvertible
        {
            var currentCondition = Conditions();
            return currentCondition.ContainsCondition(condition);
        }

        public bool NotContains<T>(T condition) where T : IConvertible
        {
            var currentCondition = Conditions();
            return currentCondition.NotContainsCondition(condition);
        }

        public bool ContainsAll(params object[] conditions)
        {
            var currentCondition = Conditions();
            return currentCondition.ContainsAllConditions(conditions);
        }

        public bool NotContainsAll(params object[] conditions)
        {
            var currentCondition = Conditions();
            return currentCondition.NotContainsAllConditions(conditions);
        }

        public bool Same(int[] currentConditionSource, int[] targetConditionSource)
        {
            return currentConditionSource.SameConditions(targetConditionSource);
        }


        /**
			returns RuntimeAutoData of this auto.
		*/
        public RuntimeAutoData RuntimeAutoData()
        {
            var runtimeRootChangers = new List<RuntimeChangerData>();
            if (rootChangers != null)
            {
                for (var i = 0; i < rootChangers.Count; i++)
                {
                    var rootChanger = rootChangers[i];
                    runtimeRootChangers.Add(new RuntimeChangerData(rootChanger.GetType().AssemblyQualifiedName));
                }
            }

            var runtimeChangers = new List<RuntimeChangerData>();
            if (changers != null)
            {
                for (var i = 0; i < changers.Count; i++)
                {
                    var changer = changers[i];
                    runtimeChangers.Add(new RuntimeChangerData(changer.GetType().AssemblyQualifiedName));
                }
            }

            var runtimeTimelineDatas = new List<RuntimeTimelineData>();

            for (var i = 0; i < timelines.Count; i++)
            {
                var timeline = timelines[i];
                var tacks = new List<RuntimeTackData>();

                for (var j = 0; j < timeline.tacks.Count; j++)
                {
                    var tack = timeline.tacks[j];
                    var routines = new List<RuntimeRoutineData>();

                    for (var k = 0; k < tack.routines.Count; k++)
                    {
                        var routine = tack.routines[k];
                        var routineName = routine.routineContext.routineName;
                        routines.Add(new RuntimeRoutineData(routineName));
                    }

                    tacks.Add(
                        new RuntimeTackData(
                            tack.start,
                            tack.span,
                            ConditionGateway.ConditionTypeString(tack.conditionType),
                            ConditionGateway.ConditionValueString(tack.conditionType, tack.conditionValue),
                            routines
                        )
                    );
                }

                runtimeTimelineDatas.Add(
                    new RuntimeTimelineData(
                        tacks
                    )
                );
            }

            return new RuntimeAutoData(
                runtimeRootChangers,
                runtimeChangers,
                runtimeTimelineDatas
            );
        }
    }
}