using System;
using System.Collections.Generic;

namespace Automatine
{
    public class Tack<InitialParamType, UpdateParamType>
    {

        private bool consumed = false;
        public bool broke = false;

        public readonly string tackInfo;
        public readonly int start;
        public readonly int span;
        public readonly int conditionValue;
        public readonly int conditionType;
        private readonly Func<RoutineBase<InitialParamType, UpdateParamType>>[] routineSources;
        public List<Routine<InitialParamType, UpdateParamType>> routines;

        private int tackLoadCount = 0;
        private Timeline<InitialParamType, UpdateParamType> parentTimeline;
        private int tackConsumedFrame = -1;

        public Tack(string tackInfo, int start, int span, object conditionSource, params Func<RoutineBase<InitialParamType, UpdateParamType>>[] routineSources)
        {
            this.tackInfo = tackInfo;
            this.start = start;

            if (span < 0) span = AutomatineDefinitions.Tack.LIMIT_UNLIMITED;
            if (span == 0) throw new Exception("tack span should be greater than 0 or AutomatineDefinitions.Tack.LIMIT_UNLIMITED.");
            this.span = span;

            // ready condition value and type.
            this.conditionValue = (int)conditionSource;
            this.conditionType = ConditionGateway.GetIndexOfConditionType(conditionSource);

            this.routineSources = routineSources;
        }

        /**
			tack without condition.
		*/
        public Tack(string tackInfo, int start, int span, params Func<RoutineBase<InitialParamType, UpdateParamType>>[] routineSources)
        {
            this.tackInfo = tackInfo;
            this.start = start;

            if (span < 0) span = AutomatineDefinitions.Tack.LIMIT_UNLIMITED;
            if (span == 0) throw new Exception("tack span should be greater than 0 or AutomatineDefinitions.Tack.LIMIT_UNLIMITED.");
            this.span = span;

            // empty condition value and type.
            this.conditionValue = -1;
            this.conditionType = -1;

            this.routineSources = routineSources;
        }

        /**
			initialize all routines in this tack with initial parameter.
		*/
        public void _InitializeTack(InitialParamType initialParam)
        {
            this.routines = new List<Routine<InitialParamType, UpdateParamType>>();
            for (var i = 0; i < routineSources.Length; i++)
            {
                var rSource = routineSources[i];
                var routineContext = rSource();
                var routine = new Routine<InitialParamType, UpdateParamType>(routineContext, initialParam);
                this.routines.Add(routine);
            }
        }

        public int _AssumeRestFrameOnTack()
        {
            if (span == AutomatineDefinitions.Tack.LIMIT_UNLIMITED)
            {
                return AutomatineDefinitions.Tack.LIMIT_UNLIMITED;
            }
            return (start + (span - 1));
        }

        public void _SetupTack(Auto<InitialParamType, UpdateParamType> parentAuto, Timeline<InitialParamType, UpdateParamType> parentTimeline)
        {
            this.parentTimeline = parentTimeline;
            _UpdateTackParent(parentAuto, parentTimeline);
        }

        public void _UpdateTackParent(Auto<InitialParamType, UpdateParamType> parentAuto, Timeline<InitialParamType, UpdateParamType> parentTimeline)
        {
            this.parentTimeline = parentTimeline;
            for (var i = 0; i < this.routines.Count; i++)
            {
                var routine = this.routines[i];
                routine._SetRoutineParents(parentAuto, parentTimeline, this);
            }
        }


        /**
			run all living routines in this tack.
		*/
        public void _RunRoutines(int currentFrame, UpdateParamType input)
        {
            if (consumed) return;

            tackLoadCount++;

            for (var i = 0; i < routines.Count; i++)
            {
                var routine = routines[i];
                routine._RunRoutine(currentFrame, input);

                /*
					check if this tack is consumed from inside of routine or not consumed.
					if already consumed, skip running other routine in this tack.
				*/
                if (consumed) return;
            }

            if (span == AutomatineDefinitions.Tack.LIMIT_UNLIMITED) return;

            if (span <= tackLoadCount) _SetConsumed(currentFrame);

            return;
        }

        public int _TackConsumedFrame()
        {
            return tackConsumedFrame;
        }

        public void _BreakTack(int frame)
        {
            parentTimeline._BreakCurrentTack(frame);
        }

        public bool _IsConsumed()
        {
            return consumed;
        }

        public void _SetConsumed(int currentFrame)
        {
            consumed = true;
            tackConsumedFrame = currentFrame;
        }
    }
}