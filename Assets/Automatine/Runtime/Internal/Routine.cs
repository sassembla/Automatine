using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_PLAYER || UNITY_EDITOR // for Unity environment.
#else
using System.Reflection;
#endif

namespace Automatine
{
    public class Routine<InitialParamType, UpdateParamType>
    {
        // context which is actually executed.
        public RoutineBase<InitialParamType, UpdateParamType> routineContext;
        private readonly IEnumerator next;

        private int currentCount;

        public bool consumed = false;


        public Routine(RoutineBase<InitialParamType, UpdateParamType> routineContextSource, InitialParamType defaultParam)
        {
            if (routineContextSource.rContext == null)
            {

#if UNITY_PLAYER || UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS // for Unity environment.
                {
                    var methodInfos = routineContextSource.GetType().GetMethods();
                    for (var i = 0; i < methodInfos.Length; i++)
                    {
                        var methodInfo = methodInfos[i];
                        if (routineContextSource.routineName == methodInfo.Name)
                        {
                            var del = Delegate.CreateDelegate(typeof(Func<InitialParamType, IEnumerator>), routineContextSource, methodInfo);
                            routineContextSource.rContext = (Func<InitialParamType, IEnumerator>)del;
                            break;
                        }
                    }
                }
#else // for not Unity environment.
				{
					// get method from name.
					var methodInfo = typeof(RoutineContexts<InitialParamType, UpdateParamType>).GetTypeInfo().GetDeclaredMethod(routineContextSource.routineName);
					if (methodInfo != null)  {
						var del = methodInfo.CreateDelegate(typeof(Func<InitialParamType, IEnumerator>), routineContextSource);
						routineContextSource.rContext = (Func<InitialParamType, IEnumerator>)del;
					}
				}
#endif

                if (routineContextSource.rContext == null) throw new Exception("no method info found error:" + routineContextSource.routineName);
            }

            this.routineContext = routineContextSource;
            this.next = routineContext.rContext(defaultParam);
        }

        public void _SetRoutineParents(Auto<InitialParamType, UpdateParamType> parentAuto, Timeline<InitialParamType, UpdateParamType> parentTimeline, Tack<InitialParamType, UpdateParamType> parentTack)
        {
            this.routineContext._UpdateParents(parentAuto, parentTimeline, parentTack, this);
        }

        public void _RunRoutine(int currentFrame, UpdateParamType updateParam)
        {
            if (consumed) return;

            currentCount++;

            // execute & continue
            routineContext._UpdateParams(currentFrame, currentCount, updateParam);
            var continuation = next.MoveNext();
            if (!continuation)
            {
                consumed = true;
                return;
            }
        }
    }

    /**
		instance for executable routine.
		replace rContext to specific other context for execute that via same interface.
	*/
    public class RoutineBase<InitialParamType, UpdateParamType>
    {
        /**
			context for this routine.
			replace with the specific partial-routineContext.
		*/
        public Func<InitialParamType, IEnumerator> rContext;

        public readonly string routineName;


        private Auto<InitialParamType, UpdateParamType> parentAuto;
        private Timeline<InitialParamType, UpdateParamType> parentTimeline;
        private Tack<InitialParamType, UpdateParamType> parentTack;
        private Routine<InitialParamType, UpdateParamType> parentRoutine;

        public void _UpdateParents(
            Auto<InitialParamType, UpdateParamType> parentAuto,
            Timeline<InitialParamType, UpdateParamType> parentTimeline,
            Tack<InitialParamType, UpdateParamType> parentTack,
            Routine<InitialParamType, UpdateParamType> parentRoutine
        )
        {
            this.parentAuto = parentAuto;
            this.parentTimeline = parentTimeline;
            this.parentTack = parentTack;
            this.parentRoutine = parentRoutine;
        }

        public RoutineBase()
        {
            this.routineName = "not set by string constructor.";
        }

        public RoutineBase(string routineName)
        {
            this.routineName = routineName;
        }

        /*
			update parameters
		*/
        public int frame;
        public int loadCount;
        public UpdateParamType updateParam;
        public void _UpdateParams(int frame, int count, UpdateParamType updateParam)
        {
            this.frame = frame;
            this.loadCount = count;
            this.updateParam = updateParam;
        }


        /*
			public interfaces
		*/

        public void BreakAuto()
        {
            parentAuto.BreakAuto();
        }

        public void BreakTimeline()
        {
            parentTimeline._BreakTimeline();
        }

        public void BreakTack()
        {
            parentRoutine.consumed = true;
            parentTack._BreakTack(frame);
        }

        public string ParentsInfo()
        {
            var infos = "autoInfo:" + parentAuto.autoInfo + " timelineInfo:" + parentTimeline.timelineInfo + " tackInfo:" + parentTack.tackInfo;
            return infos;
        }

        public int RestCount()
        {
            if (parentTack.span == AutomatineDefinitions.Tack.LIMIT_UNLIMITED) return AutomatineDefinitions.Tack.LIMIT_UNLIMITED;
            return parentTack.span - loadCount;
        }

        public bool IsFinalCount()
        {
            return (RestCount() == 0);
        }

        public int ConditionValue()
        {
            return parentTack.conditionValue;
        }

        public int ConditionType()
        {
            return parentTack.conditionType;
        }

        public int[] Conditions()
        {
            return parentAuto.Conditions();
        }

        public string ParentAutoId()
        {
            return parentAuto.autoId;
        }

        public List<IAutoChanger> Changers()
        {
            return parentAuto.Changers();
        }

        public void StackChanger(IAutoChanger changer)
        {
            parentAuto.StackChanger(changer);
        }
    }
}

public partial class RoutineContexts<InitialParamType, UpdateParamType> : Automatine.RoutineBase<InitialParamType, UpdateParamType>
{
    public RoutineContexts() : base() { }
    public RoutineContexts(string routineName) : base(routineName) { }
}
