using System.Collections;
using Automatine;

/**
	
*/
public partial class RoutineContexts<InitialParamType, UpdateParamType> : RoutineBase<InitialParamType, UpdateParamType> {
	public IEnumerator BreakAutoAt2Frame (InitialParamType initialParam) {
        int count = 0;
        
		// nothig to do.
		while (true) {
            count++;
            if (count == 2) break; 
			yield return null;
		}
        
        BreakAuto();
	}
}