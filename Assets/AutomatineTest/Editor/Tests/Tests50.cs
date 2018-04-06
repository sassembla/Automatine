using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Automatine;
using UnityEngine;


/**
	generate auto json data from editor data.
*/
public partial class Test
{
    public void _50_0_AutoFromEditorWithoutChangers()
    {

        // contains five frame then break from frame 10.
        var dummyEditorJsonData = "{\"lastModified\": \"02/15/2016 21:50:02\",\"autoDatas\": [{\"autoId\": \"A_29db81d7-af22-4513-a36b-e975a188dbbb\",\"name\": \"Test\",\"info\": \"generated by automatine.\",\"comments\": [],\"timelines\": [{\"timelineId\": \"TL_39f2c645-bf66-46aa-9d27-2b8b35f4fbd9\",\"info\": \"New Timeline\",\"tacks\": [{\"id\": \"TA_c17ae58f-0637-46b7-a067-8866cb3c54d3\",\"info\": \"New Tack\",\"start\": 10,\"span\": 6,\"conditionType\": \"Action\",\"conditionValue\": \"Move\",\"routineIds\": [\"_FiveFrameThenBreakAuto\"]}]}]}],\"changersDatas\": [],\"lastActiveAutoId\": \"dummy\",\"lastScrolledPosX\": 0.0,\"lastScrolledPosY\": 0.0}";

        // get runtimeAutoData from editor data.
        var dummyDeserializedAutomatineData = JsonUtility.FromJson<AutomatineData>(dummyEditorJsonData);
        var runtimeAutoData = dummyDeserializedAutomatineData.autoDatas.FirstOrDefault().GetRuntimeAutoData(new List<RuntimeChangerData>(), new List<RuntimeChangerData>());

        var frame = 0;

        // get auto from runtimeAutoData.
        var autoFromJson = Auto<int, int>.RuntimeAutoGenerator(runtimeAutoData)(frame, 0);

        for (var i = 0; i < 14; i++)
        {
            autoFromJson.Update(frame, 0);
            frame++;
            if (autoFromJson.ShouldFalldown(frame)) Debug.LogError("failed to generate auto from json.");
        }
        // ~13times, frame = 14.

        // frame = 14, broke by tack's coroutine.
        autoFromJson.Update(frame, 0);

        frame = frame + 1;// 15
        if (autoFromJson.ShouldFalldown(frame)) return;


        Debug.LogError("failed to generate auto from editor data.");
    }
}
