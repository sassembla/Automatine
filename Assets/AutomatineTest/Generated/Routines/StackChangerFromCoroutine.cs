using System.Collections;
using Automatine;
using UnityEngine;

public partial class RoutineContexts<InitialParamType, UpdateParamType> : RoutineBase<InitialParamType, UpdateParamType> {
	public IEnumerator StackChangerFromCoroutine (InitialParamType initialParam) {
        
        // changers which belongs to this Tack's parent Auto.
        var changers = this.Changers();
        
        // choose stack changer.(or manually,, not prefer but enable.)
        foreach (var changer in changers) {
            if (changer.ChangerName() == "ChangerForTest40_3") this.StackChanger(changer);
            break;
        }
		
        yield break;
	}
}