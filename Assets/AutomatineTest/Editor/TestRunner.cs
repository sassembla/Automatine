using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;

// Test on compile

public partial class Test {
	private PlayerContext dummyContext;
	private Dictionary<string, PlayerContext> dummyContexts;

	
	public void RunTests () {
		var tests = new List<Action>();

		// if (false) {
		// 	Debug.LogWarning("skip tests");
		// 	return;
		// }
		
		// update conditions for test.
		var testCollection = new List<Type> {
			typeof(AutoConditions.Act), 
			typeof(AutoConditions.Attk), 
			typeof(AutoConditions.Move), 
			typeof(AutoConditions.Canc),
			typeof(AutoConditions.Hit),
			typeof(AutoConditions.Anim),
		};
		
		
		testCollection.AddRange(CollectedConditions.conditions.ToList());
		
		CollectedConditions.conditions = testCollection.ToArray();
		
		// tests
		{
			tests.Add(this._0_0_RunRoutine);
			tests.Add(this._0_1_RunRoutine_NTimes);
			tests.Add(this._0_2_RunRoutine_2Timelines);
			tests.Add(this._0_3_RunRoutine_2Timelines_NTimes);
			tests.Add(this._0_4_RunRoutine_2Timelines_2ndTimelineFirst);
			tests.Add(this._0_5_RunRoutine_2Timelines_2ndTimelineFirst_MultiTack);
			tests.Add(this._0_6_RunRoutine_2Timelines_2ndTimelineFirst_Many_MultiTack);
			
			tests.Add(this._1_0_GerenateAutoThenFalldown);
			tests.Add(this._1_1_RunContainsSpecificConditionInTimelines);
			tests.Add(this._1_2_RunContainsAllSpecificConditionInTimelines);
			tests.Add(this._1_3_RunNotContainsSpecificConditionInTimeline);
			tests.Add(this._1_4_RunNotContainsAllSpecificConditionInTimelines);
			
			tests.Add(this._2_0_AtEnd);
			tests.Add(this._2_1_AtEndWith1Start);
			tests.Add(this._2_2_ConditionContinuesIn10Frame);
			tests.Add(this._2_3_ConditionContinuesIn10FrameBetween2Tack);
			tests.Add(this._2_4_ConditionContinuesIn15FrameBetween3Tack);

			tests.Add(this._3_0_BreakTack);
			tests.Add(this._3_1_BreakTimeline);
			tests.Add(this._3_2_BreakAuto);
			
			tests.Add(this._4_0_ContinuationWithBreakAllTack);
			tests.Add(this._4_1_ContinuationWithBreakAllTimeline);
			tests.Add(this._4_2_ContinuationWithBreakOneTimelineThenKeepOtherTimeline);
			tests.Add(this._4_3_ContinuationWithBreakAuto);
			tests.Add(this._4_4_ContinuationWith1FrameConsume);
			tests.Add(this._4_5_ContinuationWithBreakTackIn1Frame);
			tests.Add(this._4_6_ContinuationWithAllTimelineConsumed);
			tests.Add(this._4_7_ContinuationWithAllTackConsumed);

			tests.Add(this._5_0_CancelThisRoutineByBreakTack);
			tests.Add(this._5_1_CancelThisRoutineByBreakTimeline);
			tests.Add(this._5_2_CancelThisRoutineByBreakAuto);
			tests.Add(this._5_3_CancelAnotherRoutineByBreakTack);
			tests.Add(this._5_4_CancelTimelineByBreakTimeline);
			tests.Add(this._5_5_CancelAllTimelinesByBreakTimeline);
			tests.Add(this._5_6_CancelAutoByBreakAuto);
			tests.Add(this._5_7_CancelAnotherRoutineByBreakAutoAndBreakTack);

			tests.Add(this._6_0_ConditionWithSkipTackByBreackTack);
			tests.Add(this._6_1_ConditionWithConsumeAllTimelineByBreackTimeline);
			tests.Add(this._6_2_ConditionWithConsumeOneTimelineByBreackTimeline);
			tests.Add(this._6_3_ConditionShouldEmptyByBreakAllTack);
			tests.Add(this._6_4_ConditionShouldKeepAfterAutoConsumed);
			tests.Add(this._6_5_ConditionShouldFalldownAfterAutoConsumed);
			tests.Add(this._6_6_ConditionWithBreakTimelineSandwich);
			tests.Add(this._6_7_ConditionWithBreakTackSandwich);
			tests.Add(this._6_8_ConditionWithBreakTacksSandwich);
			tests.Add(this._6_9_ConditionWithBreakTacksOveredSandwich);
			tests.Add(this._6_10_BreakAllTacksAndBreakTimelineIsNotSameConditions);
			tests.Add(this._6_11_AutoConditionWithSkipTackByBreackTack);
			tests.Add(this._6_12_AutoConditionWithConsumeAllTimelineByBreackTimeline);
			tests.Add(this._6_13_AutoConditionWithConsumeOneTimelineByBreackTimeline);
			tests.Add(this._6_14_AutoConditionShouldEmptyByBreakAllTack);
			tests.Add(this._6_15_AutoConditionShouldKeepAfterAutoConsumed);
			tests.Add(this._6_16_AutoConditionShouldKeepAfterChangeAuto);
			tests.Add(this._6_17_AutoConditionDoesNotKeepAfterChangeAutoByNewWithoutKeepConditions);
				
			tests.Add(this._7_0_InheritTimeline);
			tests.Add(this._7_1_InheritTimelines);
			tests.Add(this._7_2_InheritBrokeTimeline);
			tests.Add(this._7_3_InheritBrokeTimelines);
			tests.Add(this._7_4_InheritTimelineFromBrokeAuto);
			tests.Add(this._7_5_InheritTimelinesFromBrokeAuto);
			tests.Add(this._7_6_InheritTimelineThenRunTack);
			tests.Add(this._7_7_InheritTimelineThenRunTacks);
			tests.Add(this._7_8_InheritTimelineMultiTimes);
			tests.Add(this._7_9_InheritTimelineMultiTimes2);

			tests.Add(this._8_0_ConditionWithInheritTimeline);
			tests.Add(this._8_1_ConditionWithInheritTimelines);
			
			tests.Add(this._9_0_BreakTackWithInherit);

			tests.Add(this._10_0_RoutineCanRetrieveParentAutoId);
			tests.Add(this._10_1_AssumeSpan);
			tests.Add(this._10_2_AssumeSpanWithInfinity);
			tests.Add(this._10_3_AssumeSpanWithInfinityAndLimited);
			tests.Add(this._10_4_AssumeSpanWithInherit);
			tests.Add(this._10_5_AssumeSpanInProgress);
			tests.Add(this._10_6_AssumeSpanWithInfinityInProgress);
			tests.Add(this._10_7_AssumeSpanWithInfinityAndLimitedInProgress);
			tests.Add(this._10_8_AssumeSpanWithInheritInProgress);
			tests.Add(this._10_9_RoutineRetriveRestFrameOfTack);
			tests.Add(this._10_10_RoutineRetrieveCondition);
			tests.Add(this._10_11_RoutineRetrieveConditions);

			tests.Add(this._11_0_AutoWillNeverIgniteWithSameFrame);
			tests.Add(this._11_1_AutoRunningWithCatchUpFrame);
			tests.Add(this._11_2_AutoRunningWithCatchUpFrame2);
			tests.Add(this._11_3_AutoReturnsUnUpdatedConditions);
			tests.Add(this._11_7_AutoWillChangeByAutoDotBreakTimelinesThenAutoIsBroken);
			tests.Add(this._11_8_AutoWillChangeByAutoDotBreakTimelinesThenAutoIsBroken2);
			tests.Add(this._11_9_AutoWillChangeByAutoDotBreakTimelinesThenAutoIsAlive);
			
			tests.Add(this._12_0_TimelineConditionCollectionError);
			tests.Add(this._12_1_TimelineExportError);
			
			tests.Add(this._13_0_DirectionChangeAuto);
			tests.Add(this._13_1_DirectionChangeAutoTwice);
			tests.Add(this._13_2_DirectionChangeCondition);
			tests.Add(this._13_3_DirectionInheritTimeline);
		}

		// with spaces.
		{
			tests.Add(this._14_0_AutoWithSpace_AssumeSpan);
			tests.Add(this._14_1_AutoWithSpace_AssumeSpan_MoreSpan);
			tests.Add(this._14_2_AutoWithSpace_AssumeSpan_MoreSpan_IgnoreStartFrame);

			tests.Add(this._15_0_AutoWithSpace_Conditions);
			tests.Add(this._15_1_AutoWithSpace_Conditions_NextIs100Frame);
			tests.Add(this._15_2_AutoWithSpace_Conditions_KeepCondition);
			tests.Add(this._15_3_AutoWithSpace_Conditions_KeepCondition_Later);
		}
		
		// with empty typed tack
		{
			tests.Add(this._16_0_TackWithoutType);
			tests.Add(this._16_1_EmptyFrameType);
			tests.Add(this._16_2_EmptyFrameTypeAfterUpdate);
			tests.Add(this._16_3_EmptyFrameThenConditions);
		}
		
		// write file tests.
		{
			tests.Add(this._20_0_AutoGeneratorDescriptorDirectory);
			tests.Add(this._20_1_AutoGeneratorDescriptorFromJson);
			tests.Add(this._20_2_AutoGeneratorDescriptorOfUntypedTack);
			tests.Add(this._20_3_AutoGeneratorDescriptorOfEmptyTimeline);

			tests.Add(this._21_0_RoutineDescriptorDirectory);
			tests.Add(this._21_1_RoutineDescriptorFromJson);
			
			tests.Add(this._22_0_ConditionDescriptorDirectory);
			tests.Add(this._22_1_ConditionDescriptorFromJson);

			tests.Add(this._23_0_CollectedConditionsDescriptorDirectory);
			tests.Add(this._23_1_CollectedConditionsDescriptorFromJson);
			
			tests.Add(this._24_0_ChangerDescriptorDirectory);
			tests.Add(this._24_1_ChangerDescriptorFromJson);
			tests.Add(this._24_2_ChangerDescriptorFromJsonWithFinally);
			tests.Add(this._24_3_ChangerDescriptorFromJsonWithBranchesAndFinally);
		}
		

		// use written code tests.
		{
			tests.Add(this._30_0_ChangerWithResultOfTest24_TestRoute0);
			tests.Add(this._30_1_ChangerWithResultOfTest24_TestRoute1);
			tests.Add(this._30_2_ChangerWithResultOfTest24_TestRoute2);
			tests.Add(this._30_3_ChangerWithResultOfTest24_TestRoute3);
		}
		
		// Changers with Auto
		{
			tests.Add(this._40_0_Changers);
			tests.Add(this._40_1_RootChangers);
			tests.Add(this._40_2_StackChangers);
			tests.Add(this._40_3_CheckStackedChangers);
			tests.Add(this._40_4_CheckStackedChangersThenUse);
			tests.Add(this._40_5_EffectiveChangers);
			tests.Add(this._40_6_EffectiveChangersWithFrameProgress);
			tests.Add(this._40_7_StackChangerFromCoroutine);
			tests.Add(this._40_8_StackChangerFromCoroutineThenEmitChanger);
			tests.Add(this._40_9_StackChangerFromCoroutineThenEmitEffectiveChanger);
			tests.Add(this._40_10_StackedChangerWillClear);
			tests.Add(this._40_11_StackChangerFromCoroutineThenEmitEffectiveChangerWithProgress);
			tests.Add(this._40_12_StackChangerFromCoroutineThenEmitEffectiveChangerWithoutKeepingConditionsWithProgress);
			tests.Add(this._40_13_StackChangerFromCoroutineThenEmitEffectiveChangerKeepingConditionsWithProgress);
			tests.Add(this._40_14_StackChangerFromCoroutineThenEmitEffectiveChangersWithProgress);
			tests.Add(this._40_15_StackChangerFromCoroutineThenEmitEffectiveChangersWithoutKeepingConditionsWithProgress);
			tests.Add(this._40_16_StackChangerFromCoroutineThenEmitEffectiveChangersKeepingConditionsWithProgress);
		}
		
		// reset auto frame.
		{
			tests.Add(this._41_0_ResetFrame);
			tests.Add(this._41_1_ResetFrameWithSameFrame);
			tests.Add(this._41_2_ResetFrameBeforeUpdateThenUpdate);
			tests.Add(this._41_3_ResetFrameWithInheritTimeline);
			tests.Add(this._41_4_ResetFrameWithInheritTimelineInheritBeforeUpdate);
			tests.Add(this._41_5_ResetFrameWithInheritTimelineInheritAfterUpdate);
		}
		
		// generate auto from auto.
		{
			tests.Add(this._42_0_AutoFromAuto);
			tests.Add(this._42_1_AutoFromAutoWithCoroutine);
			tests.Add(this._42_2_AutoFromAutoWithChanger);
			tests.Add(this._42_3_AutoFromAutoAsOtherType);
			tests.Add(this._42_4_AutoFromJson);
			tests.Add(this._42_5_AutoFromJsonWithCoroutine);
			tests.Add(this._42_6_AutoFromAutoDataWithKeepingConditions);
			tests.Add(this._42_7_AutoFromAutoDataWithoutKeepingConditions);
		}
		
		// generate auto from editor json.
		{
			tests.Add(this._50_0_AutoFromEditorWithoutChangers);
		}
		
		// continue frame x N
		{
			tests.Add(this._60_0_AutoWithLength10By10Update);
			tests.Add(this._60_1_AutoWithLength10By5Update);
			tests.Add(this._60_2_AutoWithLength10By4Update);
			tests.Add(this._60_3_AutoWithLength10By4UpdateWithRun);
			tests.Add(this._60_4_AutoWithLength10By4Update_2);
			tests.Add(this._60_5_AutoWithLength10By4UpdateWithRun_2);
		}

		Debug.Log("test start date:" + DateTime.Now);
		
		long total = 0;
		var testCount = 100.0;
		for (int i = 0; i < testCount; i++) {
			System.Diagnostics.Stopwatch sr = new System.Diagnostics.Stopwatch();
			sr.Start();
			foreach (var test in tests) {
				Setup();
				test();
				Teardown();
			}
			sr.Stop();
			
			var e = sr.ElapsedTicks;
			total = total + e;
		}
		Debug.Log("avg:" + (total / testCount));
		
		// 13089823 vs
		//  4566898 (10)
		//  4526870
		//  4364020
		
		// 2nd gen,
		//  6038920
		//  5893450
		//  4639419 (100)
		
		// 3rd gen.(100)
		//  5034202.7(100)
		//  5176867.7(100)
		//  4983600.6(100)
		//  4606215.7(100)
		//  4672827.3(100)
		
		//  4862117.0(100)
		//  5046780.3(100)
		
		// 4th gen.(100)
		//  5477045.3
		//  4836286.5
		//  4958056.9
		//  4453225.7
		
		// 5th gen.(100)(unnecessary logs removed.)
		//   231777.4
		//   261241.8
		//   291270
		//   396080.3
		//   211299.8
		//   235186.2
		//   238572.4
		
	}


	public void Setup () {
		// Debug.LogError("do");
		dummyContext = new PlayerContext("dummyId");
		dummyContexts = new Dictionary<string, PlayerContext>{
			{dummyContext.playerId, dummyContext}
		};
	}

	public void Teardown () {
		dummyContext = null;
		dummyContexts = new Dictionary<string, PlayerContext>();
		// Debug.LogError("done");
	}
}