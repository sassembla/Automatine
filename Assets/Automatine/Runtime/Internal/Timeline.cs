using System.Collections.Generic;

namespace Automatine
{
    public class Timeline<InitialParamType, UpdateParamType>
    {
        public readonly string timelineInfo;
        public readonly List<Tack<InitialParamType, UpdateParamType>> tacks;

        private readonly int staticTypeOfTimeline;

        public bool timelineConsumed = false;
        public bool inherited = false;
        public bool broke = false;



        private int timelineLoadCount;


        public Timeline(string timelineInfo, params Tack<InitialParamType, UpdateParamType>[] tacks)
        {
            this.timelineInfo = timelineInfo;
            this.tacks = new List<Tack<InitialParamType, UpdateParamType>>(tacks);
            for (var i = 0; i < this.tacks.Count; i++)
            {
                var tack = this.tacks[i];
                if (tack.conditionType != -1)
                {
                    this.staticTypeOfTimeline = tack.conditionType;
                    break;
                }
            }
        }

        /**
			initialize all tacks and
			setup timeline for first tack.
		*/
        public void _SetupTimeline(InitialParamType initialParam, Auto<InitialParamType, UpdateParamType> parentAuto)
        {
            for (var i = 0; i < this.tacks.Count; i++)
            {
                var tack = this.tacks[i];
                tack._InitializeTack(initialParam);
            }

            if (0 < tacks.Count)
            {
                this.tacks[0]._SetupTack(parentAuto, this);
            }
        }

        public void _UpdateTimelineParent(Auto<InitialParamType, UpdateParamType> parentAuto)
        {
            if (0 < tacks.Count)
            {
                this.tacks[0]._UpdateTackParent(parentAuto, this);
            }
        }

        /**
			run this timeline.
		*/
        public void _ProceedTimeline(int frame, UpdateParamType input, Auto<InitialParamType, UpdateParamType> parentAuto)
        {
            if (timelineConsumed) return;

            if (tacks[0]._IsConsumed())
            {
                // no next tack. finish this timeline.
                if (tacks.Count == 1)
                {
                    tacks.RemoveAt(0);
                    timelineConsumed = true;
                    timelineLoadCount++;
                    return;
                }


                // has next tack.
                if (timelineLoadCount == tacks[1].start)
                {
                    tacks.RemoveAt(0);

                    // setup next tack.
                    tacks[0]._SetupTack(parentAuto, this);
                }
                else
                {
                    // but not yet. next tack's start is later.
                    timelineLoadCount++;
                    return;
                }
            }

            // run tack in this timeline.
            if (tacks[0].start <= timelineLoadCount) tacks[0]._RunRoutines(frame, input);

            if (tacks[0]._IsConsumed())
            {
                // does not have next or not.
                if (tacks.Count == 1)
                {
                    timelineConsumed = true;
                    timelineLoadCount++;
                    return;
                }

                // set next tack if current tack is broken.
                if (tacks[0].broke)
                {
                    tacks.RemoveAt(0);
                }
            }
            timelineLoadCount++;
        }

        public int _AssumeRestFrameOnTimeline()
        {
            var span = 0;
            for (var i = 0; i < tacks.Count; i++)
            {
                var tack = tacks[i];
                var tackLastCount = tack._AssumeRestFrameOnTack();
                if (tackLastCount == AutomatineDefinitions.Tack.LIMIT_UNLIMITED) return AutomatineDefinitions.Tack.LIMIT_UNLIMITED;
                span = tackLastCount;
            }
            return span - timelineLoadCount;
        }

        public int _StaticConditionOfTimeline()
        {
            return staticTypeOfTimeline;
        }


        public ConditionValue_Type _TimelineConditionValueAndTypeBeforeUpdate()
        {
            if (tacks.Count == 0)
            {
                return new ConditionValue_Type(-1, -1);
            }
            if (broke)
            {
                return new ConditionValue_Type(-1, -1);
            }

            if (0 < tacks[0].start)
            {
                return new ConditionValue_Type(-1, -1);
            }

            var currentTack = tacks[0];

            var val = currentTack.conditionValue;
            var index = currentTack.conditionType;

            return new ConditionValue_Type(val, index);
        }

        /**
			return value(int) of conditon and that type(int) of the condition.
		*/
        public ConditionValue_Type _CurrentTimelineConditionValueAndType(int completedFrame)
        {
            if (tacks.Count == 0)
            {
                return new ConditionValue_Type(-1, -1);
            }
            if (broke)
            {
                return new ConditionValue_Type(-1, -1);
            }

            // timelineLoadCount is loaded count of timeline.
            // condition is based on current (timelineLoadCount - 1) condition.
            if (timelineLoadCount - 1 < tacks[0].start)
            {
                return new ConditionValue_Type(-1, -1);
            }

            var currentTack = tacks[0];

            if (currentTack._IsConsumed())
            {
                // if the target tack is just consumed, tack returns condition.
                // else, return empty condition.
                if (completedFrame != currentTack._TackConsumedFrame())
                {
                    return new ConditionValue_Type(-1, -1);
                }
            }

            var val = currentTack.conditionValue;
            var index = currentTack.conditionType;

            return new ConditionValue_Type(val, index);
        }

        public void _BreakTimeline()
        {
            timelineConsumed = true;
            broke = true;
        }

        public void _BreakCurrentTack(int frame)
        {
            if (tacks.Count == 0) return;
            if (tacks[0]._IsConsumed()) return;
            if (tacks[0].broke) return;

            tacks[0]._SetConsumed(frame);
            tacks[0].broke = true;

            // if this tack is last one, check this timeline as consumed.
            if (tacks.Count == 1)
            {
                timelineConsumed = true;
                return;
            }
        }
    }

    public struct ConditionValue_Type
    {
        public int val;
        public int type;

        public ConditionValue_Type(int val, int type)
        {
            this.val = val;
            this.type = type;
        }
    }
}