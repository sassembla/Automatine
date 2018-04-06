
using System;

namespace Automatine
{
    /**
		interface definition for type constraints. 
	*/
    public interface IAutoChanger
    {

        string ChangerName();

        Func<Auto<InitialParamType, UpdateParamType>, int, InitialParamType, Auto<InitialParamType, UpdateParamType>> Changer<InitialParamType, UpdateParamType>();

        bool IsEffective<InitialParamType, UpdateParamType>(Auto<InitialParamType, UpdateParamType> baseAuto);
    }
}