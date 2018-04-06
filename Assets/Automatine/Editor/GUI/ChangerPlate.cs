using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Automatine
{
    [Serializable]
    public class ChangerPlate
    {
        public static Action<OnChangerEvent> Emit;

        [SerializeField] private ChangerPlateInspector changerPlateInspector;

        [CustomEditor(typeof(ChangerPlateInspector))]
        public class ChangerPlateInspectorGUI : Editor
        {
            public override void OnInspectorGUI()
            {
                var insp = ((ChangerPlateInspector)target).changerPlate;

                var changerId = insp.changerId;
                GUILayout.Label("changerId:" + changerId);

                var changerName = insp.changerName;
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Changer Name", GUILayout.Width(100));
                    var result = GUILayout.TextField(changerName);
                    if (result != changerName)
                    {
                        insp.UpdateChangerName(result);
                    }
                }

                var changerComments = string.Join("\n", insp.comments.ToArray());
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Comment", GUILayout.Width(100));
                    var result = GUILayout.TextArea(changerComments);
                    if (result != changerComments)
                    {
                        insp.UpdateChangerComment(result);
                    }
                }

                GUILayout.Space(10);

                // branch.
                using (new GUILayout.VerticalScope(GUI.skin.box, new GUILayoutOption[0]))
                {
                    GUILayout.Label("Branch");
                    if (insp.branchBinds.Any())
                    {
                        var binds = insp.branchBinds;
                        foreach (var bind in binds.Select((val, index) => new { val, index }))
                        {
                            using (new GUILayout.HorizontalScope(GUI.skin.box, new GUILayoutOption[0]))
                            {
                                if (GUILayout.Button(string.Empty, "OL Minus", GUILayout.Width(20)))
                                {
                                    insp.DeleteBind(bind.index);
                                }
                                using (new GUILayout.VerticalScope())
                                {
                                    var kind = bind.val.bindKind;
                                    if (GUILayout.Button(kind, "LargePopup"))
                                    {
                                        ShowContextMenuOfBind(insp, bind.val);
                                    }
                                    using (new GUILayout.VerticalScope(GUI.skin.box, new GUILayoutOption[0]))
                                    {
                                        // combinations
                                        foreach (var combination in bind.val.combinations.Select((val, index) => new { val, index }))
                                        {
                                            using (new GUILayout.HorizontalScope())
                                            {
                                                if (GUILayout.Button(string.Empty, "OL Minus", GUILayout.Width(20)))
                                                {
                                                    insp.DeleteBranchConditionAt(bind.index, combination.index);
                                                }

                                                if (GUILayout.Button(combination.val.conditionType, "GV Gizmo DropDown"))
                                                {
                                                    ShowContextMenuOfType(insp, combination.val);
                                                }

                                                if (GUILayout.Button(combination.val.conditionValue, "GV Gizmo DropDown"))
                                                {
                                                    ShowContextMenuOfValue(insp, combination.val);
                                                }
                                            }
                                            GUILayout.Space(5);
                                        }

                                        // combination + button.
                                        if (GUILayout.Button(string.Empty, "OL Plus", GUILayout.Width(20)))
                                        {
                                            insp.AddCombinationToBind(bind.index);
                                        }
                                    }
                                }
                            }
                        }

                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.FlexibleSpace();

                            // bind + button.
                            if (GUILayout.Button("+ Add Condition", GUILayout.Width(120)))
                            {
                                insp.AddBind(ChangerCondKey.CONTAINS);
                            }
                        }

                        GUILayout.Space(10);

                        var useContinue = GUILayout.Toggle(insp.useContinue, "Continue Current Auto");
                        if (useContinue != insp.useContinue) insp.UpdateUseContinue(useContinue);

                        GUILayout.Space(5);

                        EditorGUI.BeginDisabledGroup(insp.useContinue);
                        {
                            // branch next auto.
                            using (new GUILayout.HorizontalScope())
                            {
                                GUILayout.Label("Next Auto");
                                if (GUILayout.Button(insp.AutoNameFromId(insp.branchAutoId), "LargePopup"))
                                {
                                    Debug.LogError("このAuto以外のNameを出すコンテキストメニュー");
                                }
                            }

                            // branch inherits.
                            using (new GUILayout.HorizontalScope())
                            {
                                GUILayout.Label("Inherit Timeline Type");
                                GUILayout.FlexibleSpace();
                                using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.Width(130)))
                                {
                                    foreach (var inheritType in insp.branchInheritTypes.Select((val, index) => new { index, val }))
                                    {
                                        using (new GUILayout.HorizontalScope())
                                        {
                                            if (GUILayout.Button(string.Empty, "OL Minus", GUILayout.Width(20)))
                                            {
                                                insp.DeleteBranchInheritAt(inheritType.index);
                                            }
                                            if (GUILayout.Button(inheritType.val, "GV Gizmo DropDown"))
                                            {
                                                Debug.LogError("Typeを選ぶコンテキストメニューだす");
                                            }
                                        }
                                        GUILayout.Space(5);
                                    }
                                    if (GUILayout.Button(string.Empty, "OL Plus", GUILayout.Width(20)))
                                    {
                                        insp.AddBranchInheritCondition();
                                    }
                                }
                            }
                        }
                        EditorGUI.EndDisabledGroup();
                    }
                    else
                    {
                        // bind + button.
                        if (GUILayout.Button("+ Add Condition", GUILayout.Width(120)))
                        {
                            insp.AddBind(ChangerCondKey.CONTAINS);
                        }
                    }
                }

                GUILayout.Space(10);

                // finally.
                using (new GUILayout.VerticalScope(GUI.skin.box, new GUILayoutOption[0]))
                {
                    GUILayout.Label("Finally");

                    var showFinally = GUILayout.Toggle(insp.useFinally, "Use Finally");
                    if (showFinally != insp.useFinally) insp.UpdateUseFinally(showFinally);
                    if (showFinally)
                    {
                        GUILayout.Space(10);

                        // finally next auto.
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label("Next Auto");
                            if (GUILayout.Button(insp.AutoNameFromId(insp.finallyAutoId), "LargePopup"))
                            {
                                Debug.LogError("このAuto以外のNameを出すコンテキストメニュー");
                            }
                        }

                        // finally inherits.
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label("Inherit Timeline Type");
                            GUILayout.FlexibleSpace();
                            using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.Width(130)))
                            {
                                foreach (var inheritType in insp.finallyInheritTypes.Select((val, index) => new { index, val }))
                                {
                                    using (new GUILayout.HorizontalScope())
                                    {
                                        if (GUILayout.Button(string.Empty, "OL Minus", GUILayout.Width(20)))
                                        {
                                            insp.DeleteFinallyInheritAt(inheritType.index);
                                        }
                                        if (GUILayout.Button(inheritType.val, "GV Gizmo DropDown"))
                                        {
                                            Debug.LogError("Typeを選ぶコンテキストメニューだす");
                                        }
                                    }
                                    GUILayout.Space(5);
                                }
                                if (GUILayout.Button(string.Empty, "OL Plus", GUILayout.Width(20)))
                                {
                                    insp.AddFinallyInheritCondition();
                                }
                            }
                        }
                    }
                }
            }

            private void ShowContextMenuOfBind(ChangerPlate parent, CombinationBinds bind)
            {
                var menu = new GenericMenu();

                var menuItems = new List<string>{
                    ChangerCondKey.CONTAINS,
                    ChangerCondKey.NOTCONTAINS,
                    ChangerCondKey.CONTAINSALL,
                    ChangerCondKey.NOTCONTAINSALL,
                };

                foreach (var combinationBind in menuItems.Select((val, index) => new { index, val }))
                {
                    var currentIndex = combinationBind.index;
                    menu.AddItem(
                        new GUIContent(combinationBind.val),
                        false,
                        () =>
                        {
                            parent.EmitUndo("Change Branch Combination");
                            bind.bindKind = menuItems[currentIndex];
                            parent.EmitSave();
                        }
                    );
                }

                menu.ShowAsContext();
            }

            private void ShowContextMenuOfType(ChangerPlate parent, ConditionTypeValuePair typeValuePair)
            {
                var conditionType = typeValuePair.conditionType;
                var menu = new GenericMenu();

                // update parent:ChangerPlate's conditions to latest.
                ChangerPlate.Emit(new OnChangerEvent(OnChangerEvent.EventType.EVENT_REFRESHCONDITIONS, parent.changerId));

                var menuItems = parent.conditions
                    .Select(conditonData => conditonData.conditionType)
                    .Where(type => type != conditionType)
                    .ToList();

                // current.
                menu.AddDisabledItem(
                    new GUIContent(conditionType)
                );

                menu.AddSeparator(string.Empty);

                // new.
                menu.AddItem(
                    new GUIContent("Add New Type"),
                    false,
                    () =>
                    {
                        ChangerPlate.Emit(new OnChangerEvent(OnChangerEvent.EventType.EVENT_ADDNEWTYPE, parent.changerId));
                    }
                );

                menu.AddSeparator(string.Empty);

                // other.
                foreach (var conditonData in menuItems.Select((val, index) => new { index, val }))
                {
                    var currentType = conditonData.val;

                    menu.AddItem(
                        new GUIContent(currentType),
                        false,
                        () =>
                        {
                            parent.EmitUndo("Change Combination Type");
                            typeValuePair.conditionType = currentType;
                            parent.EmitSave();
                        }
                    );
                }

                menu.ShowAsContext();
            }

            private void ShowContextMenuOfValue(ChangerPlate parent, ConditionTypeValuePair typeValuePair)
            {
                var conditionType = typeValuePair.conditionType;
                var conditionValue = typeValuePair.conditionValue;

                var menu = new GenericMenu();

                // update parent:ChangerPlate's conditions to latest.
                ChangerPlate.Emit(new OnChangerEvent(OnChangerEvent.EventType.EVENT_REFRESHCONDITIONS, parent.changerId));

                var menuItems = parent.conditions
                    .Where(data => data.conditionType == conditionType)
                    .FirstOrDefault();

                // current.
                if (!string.IsNullOrEmpty(conditionValue))
                {
                    menu.AddDisabledItem(
                        new GUIContent(conditionValue)
                    );
                    menu.AddSeparator(string.Empty);
                }

                // new.
                if (string.IsNullOrEmpty(conditionType))
                {
                    menu.AddItem(
                        new GUIContent("Add New Type & Value"),
                        false,
                        () =>
                        {
                            ChangerPlate.Emit(new OnChangerEvent(OnChangerEvent.EventType.EVENT_ADDNEWTYPEVALUE, parent.changerId));
                        }
                    );
                }
                else
                {
                    menu.AddItem(
                        new GUIContent("Add New Value"),
                        false,
                        () =>
                        {
                            ChangerPlate.Emit(new OnChangerEvent(OnChangerEvent.EventType.EVENT_ADDNEWVALUE, parent.changerId));
                        }
                    );
                }


                // show other conditionValues.
                if (menuItems != null)
                {
                    // remove current.
                    menuItems.conditionValues.Remove(conditionValue);

                    menu.AddSeparator(string.Empty);

                    // other.
                    foreach (var currentConditionValue in menuItems.conditionValues.Select((val, index) => new { index, val }))
                    {
                        // var currentType = conditionType;

                        var currentValue = currentConditionValue.val;
                        var currentIndex = currentConditionValue.index;

                        menu.AddItem(
                            new GUIContent(currentValue),
                            false,
                            () =>
                            {
                                parent.EmitUndo("Change Combination Value");
                                typeValuePair.conditionValue = menuItems.conditionValues[currentIndex];
                                parent.EmitSave();
                            }
                        );
                    }
                }

                menu.ShowAsContext();
            }
        }
        [SerializeField] private bool active;

        [SerializeField] public bool isExistChanger;

        [SerializeField] public string changerId;

        [SerializeField] public string changerName;

        [SerializeField] public string rootAutoId;


        [SerializeField] public List<string> comments;

        [SerializeField] public string branchName;
        [SerializeField] public List<string> branchComment;

        [SerializeField] public List<CombinationBinds> branchBinds;

        [SerializeField] public bool useContinue;

        [SerializeField] public string branchAutoId;

        [SerializeField] public List<string> branchInheritTypes;

        [SerializeField] public bool useFinally;
        [SerializeField] public string finallyAutoId;
        [SerializeField] public List<string> finallyInheritTypes;

        [NonSerializedAttribute] public List<ConditionData> conditions = new List<ConditionData>();

        public ChangerPlate(ChangerData changerData)
        {
            this.changerId = changerData.changerId;
            this.changerName = changerData.changerName;
            this.comments = changerData.comments;
            this.rootAutoId = changerData.rootAutoId;

            this.branchBinds = new List<CombinationBinds>();

            this.branchAutoId = string.Empty;
            this.branchInheritTypes = new List<string>();

            var branches = changerData.branchs;
            if (branches.Any())
            {
                var branch = branches[0];
                this.branchComment = branch.comments;

                var binds = branch.conditonBinds;
                foreach (var bind in binds)
                {
                    var bindKind = bind.combinationKind;

                    var combinationsSource = bind.combinations;
                    var combinations = new List<ConditionTypeValuePair>();
                    foreach (var combinationSource in combinationsSource)
                    {
                        combinations.Add(new ConditionTypeValuePair(combinationSource.type, combinationSource.val));
                    }

                    this.branchBinds.Add(
                        new CombinationBinds(bindKind, combinations)
                    );
                }

                if (branch.isContinue)
                {
                    this.branchAutoId = this.rootAutoId;
                    this.useContinue = true;
                }
                else this.branchAutoId = branch.nextAutoId;

                foreach (var inheritType in branch.inheritTimelineConditions)
                {
                    this.branchInheritTypes.Add(inheritType);
                }
            }

            this.finallyAutoId = string.Empty;
            this.finallyInheritTypes = new List<string>();

            if (changerData.finallyBranch.IsExists())
            {
                this.useFinally = true;

                var finallyData = changerData.finallyBranch;
                this.finallyAutoId = finallyData.finallyAutoId;

                foreach (var inheritType in finallyData.inheritTimelineConditions)
                {
                    this.finallyInheritTypes.Add(inheritType);
                }
            }

            // collect target auto ids from this changer.
            // targetAutoIds = changerData.branchs.Where(branch => !branch.isContinue).Select(branch => branch.nextAutoId).ToList();
            // if (changerData.finallyBranch.IsExists()) targetAutoIds.Add(changerData.finallyBranch.finallyAutoId);
        }

        /**
			Generate new Changer from GUI.
		*/
        public ChangerPlate(string rootAutoId)
        {
            this.changerId = AutomatineGUISettings.ID_HEADER_CHANGER + Guid.NewGuid().ToString();
            this.comments = new List<string>();
            this.changerName = AutomatineGUISettings.DEFAULT_CHANGER_NAME;
            this.rootAutoId = rootAutoId;

            this.branchBinds = new List<CombinationBinds>();

            this.branchAutoId = string.Empty;
            this.branchInheritTypes = new List<string>();

            this.finallyAutoId = string.Empty;
            this.finallyInheritTypes = new List<string>();

            AddBind(ChangerCondKey.CONTAINS);
            // this.targetAutoIds = new List<string>();   
        }

        public void AddBind(string kind)
        {
            EmitUndo("Add Condition To Changer");
            var newBind = new CombinationBinds(kind);
            var index = branchBinds.Count;
            branchBinds.Add(newBind);
            AddCombinationToBind(index);
            EmitSave();
        }

        public void DeleteBind(int bindIndex)
        {
            EmitUndo("Delete Condition From Changer");
            branchBinds.RemoveAt(bindIndex);
            if (!branchBinds.Any())
            {
                useContinue = false;
                branchAutoId = string.Empty;
                branchInheritTypes = new List<string>();
            }
            EmitSave();
        }

        public void AddCombinationToBind(int bindIndex)
        {
            EmitUndo("Add Condition Type");
            branchBinds[bindIndex].combinations.Add(new ConditionTypeValuePair(string.Empty, string.Empty));
            EmitSave();
        }

        public void AddBranchInheritCondition()
        {
            EmitUndo("Add Inherit To Changer");
            branchInheritTypes.Add(string.Empty);
            EmitSave();
        }

        public void AddFinallyInheritCondition()
        {
            EmitUndo("Add Inherit To Changer");
            finallyInheritTypes.Add(string.Empty);
            EmitSave();
        }

        public void DeleteBranchConditionAt(int bindIndex, int conditionIndex)
        {
            EmitUndo("Delete Branch From Changer");
            Debug.LogError("bindの中で削除が走ってしまってまずい。うーーん、、抜けてから実行するようなコード書かないとダメか。");
            var targetBind = branchBinds[bindIndex];
            targetBind.combinations.RemoveAt(conditionIndex);

            if (!targetBind.combinations.Any())
            {
                DeleteBind(bindIndex);
            }
            else
            {
                EmitSave();
            }
        }

        public void DeleteBranchInheritAt(int index)
        {
            EmitUndo("Delete Inherit From Changer");
            Debug.LogError("bindの中で削除が走ってしまってまずい2。うーーん、、抜けてから実行するようなコード書かないとダメか。");
            branchInheritTypes.RemoveAt(index);
            EmitSave();
        }

        public void UpdateUseContinue(bool useContinue)
        {
            EmitUndo("Continue Using Same Auto");
            this.useContinue = useContinue;
            if (useContinue)
            {
                branchAutoId = string.Empty;
                branchInheritTypes = new List<string>();
            }
            else
            {
                branchAutoId = DefaultAutoId();
                branchInheritTypes = new List<string>();
            }
            EmitSave();
        }

        public void UpdateUseFinally(bool useFinally)
        {
            EmitUndo("Use Finally Flag");
            this.useFinally = useFinally;
            if (useFinally)
            {
                finallyAutoId = DefaultAutoId();
                finallyInheritTypes = new List<string>();
            }
            else
            {
                finallyAutoId = string.Empty;
                finallyInheritTypes = new List<string>();
            }
            EmitSave();
        }

        public void DeleteFinallyInheritAt(int index)
        {
            EmitUndo("Delete Inherit From Changer");
            finallyInheritTypes.RemoveAt(index);
            EmitSave();
        }

        public void UpdateChangerName(string newName)
        {
            EmitUndo("Update Changer Name");
            changerName = newName;
            EmitSave();
        }

        public void UpdateChangerComment(string newComment)
        {
            EmitUndo("Update Changer Comment");
            comments = newComment.Split('\n').ToList();
            EmitSave();
        }


        public void EmitUndo(string undoMessage)
        {
            Emit(new OnChangerEvent(OnChangerEvent.EventType.EVENT_UNDO, changerId, undoMessage));
        }

        public void EmitSave()
        {
            Emit(new OnChangerEvent(OnChangerEvent.EventType.EVENT_SAVE, changerId));
        }

        public string DefaultAutoId()
        {
            return "dummyAutoId";
        }

        public string AutoNameFromId(string autoId)
        {
            return "got name from Id";
        }

        public void Draw_FromChangerPlate(int changerIndex, float width)
        {
            var changerRect = new Rect(0, (changerIndex * AutomatineGUISettings.CHANGER_TO_PLATE_HEIGHT), width, AutomatineGUISettings.CHANGER_TO_PLATE_HEIGHT);
            GUI.DrawTexture(changerRect, AutomatineGUISettings.changerBaseTex);

        }

        public void Draw_ToChangerPlate(int changerIndex, float width)
        {
            GUILayout.BeginArea(new Rect(0, (changerIndex * (AutomatineGUISettings.CHANGER_TO_PLATE_HEIGHT + 2)), width + AutomatineGUISettings.CHANGER_TO_PLATE_ORDERBUTTON_SIZE, AutomatineGUISettings.CHANGER_PLATE_HEIGHT));
            {
                // bg
                var changerRect = new Rect(AutomatineGUISettings.CHANGER_TO_PLATE_ORDERBUTTON_SIZE, 0, width - AutomatineGUISettings.CHANGER_TO_PLATE_ORDERBUTTON_SIZE, AutomatineGUISettings.CHANGER_TO_PLATE_HEIGHT);

                var timelineConditionTypeLabelSmall = new GUIStyle();
                timelineConditionTypeLabelSmall.normal.textColor = Color.white;
                timelineConditionTypeLabelSmall.fontSize = 12;
                timelineConditionTypeLabelSmall.alignment = TextAnchor.MiddleCenter;

                var timelineConditionTypeLabelSmall2 = new GUIStyle();
                timelineConditionTypeLabelSmall2.normal.textColor = Color.white;
                timelineConditionTypeLabelSmall2.fontSize = 12;
                timelineConditionTypeLabelSmall2.alignment = TextAnchor.MiddleLeft;

                var timelineConditionTypeLabelSmall3 = new GUIStyle();
                timelineConditionTypeLabelSmall3.normal.textColor = Color.white;
                timelineConditionTypeLabelSmall3.fontSize = 12;
                timelineConditionTypeLabelSmall3.alignment = TextAnchor.MiddleCenter;

                var changerNameRect = new Rect(
                    AutomatineGUISettings.CHANGER_TO_PLATE_ORDERBUTTON_SIZE + 1f,
                    1f,
                    AutomatineGUISettings.CHANGER_TO_PLATE_NAME_WIDTH - 2f,
                    AutomatineGUISettings.CHANGER_TO_PLATE_HEIGHT - 2f
                );

                if (active)
                {
                    // set active.
                    GUI.DrawTexture(changerRect, AutomatineGUISettings.activeObjectBaseTex);
                    var activeChangerBaseRect = new Rect(
                        AutomatineGUISettings.CHANGER_TO_PLATE_ORDERBUTTON_SIZE + 1f,
                        1f,
                        changerRect.width - 2f,
                        changerRect.height - 2f
                    );

                    // changer base tex.
                    GUI.DrawTexture(activeChangerBaseRect, AutomatineGUISettings.changerBaseTex);

                    // Order up
                    if (GUI.Button(new Rect(3, 0, AutomatineGUISettings.CHANGER_TO_PLATE_ORDERBUTTON_SIZE, AutomatineGUISettings.CHANGER_TO_PLATE_ORDERBUTTON_SIZE), string.Empty, "Grad Up Swatch"))
                    {
                        Emit(new OnChangerEvent(OnChangerEvent.EventType.EVENT_ORDERUP, changerId));
                        return;
                    }

                    // Order down
                    if (GUI.Button(new Rect(3, AutomatineGUISettings.CHANGER_TO_PLATE_HEIGHT - AutomatineGUISettings.CHANGER_TO_PLATE_ORDERBUTTON_SIZE, AutomatineGUISettings.CHANGER_TO_PLATE_ORDERBUTTON_SIZE, AutomatineGUISettings.CHANGER_TO_PLATE_ORDERBUTTON_SIZE), string.Empty, "Grad Down Swatch"))
                    {
                        Emit(new OnChangerEvent(OnChangerEvent.EventType.EVENT_ORDERDOWN, changerId));
                        return;
                    }

                }
                else
                {
                    // changer base tex.
                    GUI.DrawTexture(changerRect, AutomatineGUISettings.changerBaseTex);
                }

                // name
                GUI.DrawTexture(changerNameRect, AutomatineGUISettings.changerNameTex);
                GUI.Label(
                    changerNameRect,
                    changerName,
                    timelineConditionTypeLabelSmall
                );

                var itemOffsetX = AutomatineGUISettings.CHANGER_TO_PLATE_NAME_WIDTH + AutomatineGUISettings.CHANGER_TO_PLATE_ORDERBUTTON_SIZE;
                if (branchBinds.Any())
                {
                    if (useContinue)
                    {
                        // continue header.
                        var changerContinueRect = new Rect(
                            itemOffsetX,
                            1f,
                            AutomatineGUISettings.CHANGER_TO_PLATE_NAME_WIDTH,
                            AutomatineGUISettings.CHANGER_TO_PLATE_PARTS_HEIGHT - 2f
                        );
                        GUI.DrawTexture(changerContinueRect, AutomatineGUISettings.changerContinueTex);
                        GUI.Label(
                            changerContinueRect,
                            " continue",
                            timelineConditionTypeLabelSmall2
                        );

                        if (GUI.Button(new Rect(itemOffsetX + AutomatineGUISettings.CHANGER_TO_PLATE_NAME_WIDTH - 30f, 1f, 30f, 15f), "c"))
                        {
                            Debug.LogError("コンディションに該当する範囲を光らせる");
                        }

                        // continue item bg.
                        var changerContinueItemRect = new Rect(
                            itemOffsetX,
                            AutomatineGUISettings.CHANGER_TO_PLATE_PARTS_HEIGHT,
                            AutomatineGUISettings.CHANGER_TO_PLATE_NAME_WIDTH,
                            AutomatineGUISettings.CHANGER_PLATE_HEIGHT - AutomatineGUISettings.CHANGER_TO_PLATE_PARTS_HEIGHT - 1f
                        );
                        GUI.DrawTexture(changerContinueItemRect, AutomatineGUISettings.changerItemBaseTex);
                        GUI.Label(
                            changerContinueItemRect,
                            "-",
                            timelineConditionTypeLabelSmall3
                        );

                        if (useFinally)
                        {
                            itemOffsetX = itemOffsetX + AutomatineGUISettings.CHANGER_TO_PLATE_NAME_WIDTH + AutomatineGUISettings.CHANGER_TO_PLATE_LINEWIDTH;

                            // finally header.
                            var changerFinallyRect = new Rect(
                                itemOffsetX,
                                1f,
                                AutomatineGUISettings.CHANGER_TO_PLATE_NAME_WIDTH,
                                AutomatineGUISettings.CHANGER_TO_PLATE_PARTS_HEIGHT - 2f
                            );
                            GUI.DrawTexture(changerFinallyRect, AutomatineGUISettings.changerFinallyTex);
                            GUI.Label(
                                changerFinallyRect,
                                " finally",
                                timelineConditionTypeLabelSmall2
                            );

                            if (GUI.Button(new Rect(itemOffsetX + AutomatineGUISettings.CHANGER_TO_PLATE_NAME_WIDTH - 60f, 1f, 30f, 15f), "c"))
                            {

                                Debug.LogError("finallyのcond");
                            }

                            if (GUI.Button(new Rect(itemOffsetX + AutomatineGUISettings.CHANGER_TO_PLATE_NAME_WIDTH - 30f, 1f, 30f, 15f), "i"))
                            {
                                Debug.LogError("finallyのinherit");
                            }

                            // finally item bg.
                            var changerFinallyItemRect = new Rect(
                                itemOffsetX,
                                AutomatineGUISettings.CHANGER_TO_PLATE_PARTS_HEIGHT,
                                AutomatineGUISettings.CHANGER_TO_PLATE_NAME_WIDTH,
                                AutomatineGUISettings.CHANGER_PLATE_HEIGHT - AutomatineGUISettings.CHANGER_TO_PLATE_PARTS_HEIGHT - 1f
                            );
                            GUI.DrawTexture(changerFinallyItemRect, AutomatineGUISettings.changerItemBaseTex);
                            if (
                                GUI.Button(
                                    changerFinallyItemRect,
                                    GetAutoNameById(finallyAutoId),
                                    timelineConditionTypeLabelSmall2
                                )
                            )
                            {
                                ShowContextMenuToOtherAuto(finallyAutoId);
                            }
                        }
                    }
                    else
                    {
                        var changerBranchRect = new Rect(
                            itemOffsetX,
                            1f,
                            AutomatineGUISettings.CHANGER_TO_PLATE_NAME_WIDTH,
                            AutomatineGUISettings.CHANGER_TO_PLATE_PARTS_HEIGHT - 2f
                        );
                        GUI.DrawTexture(changerBranchRect, AutomatineGUISettings.changerBranchTex);
                        GUI.Label(
                            changerBranchRect,
                            " branch",
                            timelineConditionTypeLabelSmall2
                        );

                        if (GUI.Button(new Rect(itemOffsetX + AutomatineGUISettings.CHANGER_TO_PLATE_NAME_WIDTH - 60f, 1f, 30f, 15f), "c"))
                        {
                            Debug.LogError("コンディションに該当する範囲を光らせる");
                        }

                        if (GUI.Button(new Rect(itemOffsetX + AutomatineGUISettings.CHANGER_TO_PLATE_NAME_WIDTH - 30f, 1f, 30f, 15f), "i"))
                        {
                            Debug.LogError("コンディションに該当する範囲を光らせる");
                        }

                        // branch item bg.
                        var changerBranchItemRect = new Rect(
                            itemOffsetX,
                            AutomatineGUISettings.CHANGER_TO_PLATE_PARTS_HEIGHT,
                            AutomatineGUISettings.CHANGER_TO_PLATE_NAME_WIDTH,
                            AutomatineGUISettings.CHANGER_PLATE_HEIGHT - AutomatineGUISettings.CHANGER_TO_PLATE_PARTS_HEIGHT - 1f
                        );
                        GUI.DrawTexture(changerBranchItemRect, AutomatineGUISettings.changerItemBaseTex);
                        if (
                            GUI.Button(
                                changerBranchItemRect,
                                GetAutoNameById(branchAutoId),
                                timelineConditionTypeLabelSmall2
                            )
                        )
                        {
                            ShowContextMenuToOtherAuto(branchAutoId);
                        }


                        if (useFinally)
                        {
                            itemOffsetX = itemOffsetX + AutomatineGUISettings.CHANGER_TO_PLATE_NAME_WIDTH + AutomatineGUISettings.CHANGER_TO_PLATE_LINEWIDTH;

                            var changerFinallyRect = new Rect(
                                itemOffsetX,
                                1f,
                                AutomatineGUISettings.CHANGER_TO_PLATE_NAME_WIDTH,
                                AutomatineGUISettings.CHANGER_TO_PLATE_PARTS_HEIGHT - 2f
                            );
                            GUI.DrawTexture(changerFinallyRect, AutomatineGUISettings.changerFinallyTex);
                            GUI.Label(
                                changerFinallyRect,
                                " finally",
                                timelineConditionTypeLabelSmall2
                            );


                            if (GUI.Button(new Rect(itemOffsetX + AutomatineGUISettings.CHANGER_TO_PLATE_NAME_WIDTH - 60f, 1f, 30f, 15f), "c"))
                            {
                                Debug.LogError("finallyのcond");
                            }

                            if (GUI.Button(new Rect(itemOffsetX + AutomatineGUISettings.CHANGER_TO_PLATE_NAME_WIDTH - 30f, 1f, 30f, 15f), "i"))
                            {
                                Debug.LogError("finallyのinherit");
                            }

                            // finally item bg.
                            var changerFinallyItemRect = new Rect(
                                itemOffsetX,
                                AutomatineGUISettings.CHANGER_TO_PLATE_PARTS_HEIGHT,
                                AutomatineGUISettings.CHANGER_TO_PLATE_NAME_WIDTH,
                                AutomatineGUISettings.CHANGER_PLATE_HEIGHT - AutomatineGUISettings.CHANGER_TO_PLATE_PARTS_HEIGHT - 1f
                            );
                            GUI.DrawTexture(changerFinallyItemRect, AutomatineGUISettings.changerItemBaseTex);
                            if (
                                GUI.Button(
                                    changerFinallyItemRect,
                                    GetAutoNameById(finallyAutoId),
                                    timelineConditionTypeLabelSmall2
                                )
                            )
                            {
                                ShowContextMenuToOtherAuto(finallyAutoId);
                            }
                        }
                    }
                }
                else
                {
                    if (useFinally)
                    {
                        var changerFinallyRect = new Rect(
                            itemOffsetX,
                            1f,
                            AutomatineGUISettings.CHANGER_TO_PLATE_NAME_WIDTH,
                            AutomatineGUISettings.CHANGER_TO_PLATE_PARTS_HEIGHT - 2f
                        );
                        GUI.DrawTexture(changerFinallyRect, AutomatineGUISettings.changerFinallyTex);
                        GUI.Label(
                            changerFinallyRect,
                            " finally",
                            timelineConditionTypeLabelSmall2
                        );

                        if (GUI.Button(new Rect(itemOffsetX + AutomatineGUISettings.CHANGER_TO_PLATE_NAME_WIDTH - 60f, 1f, 30f, 15f), "c"))
                        {
                            Debug.LogError("コンディションに該当する範囲を光らせる");
                        }

                        if (GUI.Button(new Rect(itemOffsetX + AutomatineGUISettings.CHANGER_TO_PLATE_NAME_WIDTH - 30f, 1f, 30f, 15f), "i"))
                        {
                            Debug.LogError("inheritに該当する範囲を光らせる");
                        }

                        // finally item bg.
                        var changerFinallyItemRect = new Rect(
                            itemOffsetX,
                            AutomatineGUISettings.CHANGER_TO_PLATE_PARTS_HEIGHT,
                            AutomatineGUISettings.CHANGER_TO_PLATE_NAME_WIDTH,
                            AutomatineGUISettings.CHANGER_PLATE_HEIGHT - AutomatineGUISettings.CHANGER_TO_PLATE_PARTS_HEIGHT - 1f
                        );
                        GUI.DrawTexture(changerFinallyItemRect, AutomatineGUISettings.changerItemBaseTex);
                        if (
                            GUI.Button(
                                changerFinallyItemRect,
                                GetAutoNameById(finallyAutoId),
                                timelineConditionTypeLabelSmall2
                            )
                        )
                        {
                            ShowContextMenuToOtherAuto(finallyAutoId);
                        }
                    }
                }

                switch (Event.current.type)
                {
                    case EventType.MouseUp:
                        {

                            // is right clicked
                            if (Event.current.button == 1)
                            {
                                ShowContextMenuOnChanger();
                                Event.current.Use();
                                break;
                            }

                            if (changerRect.Contains(Event.current.mousePosition))
                            {
                                Emit(new OnChangerEvent(OnChangerEvent.EventType.EVENT_SELECTED, changerId));
                            }
                            break;
                        }
                }
            }
            GUILayout.EndArea();
        }

        private void ShowContextMenuOnChanger()
        {
            var menu = new GenericMenu();

            var menuItems = new Dictionary<string, OnChangerEvent.EventType>{
                {"Add New Changer", OnChangerEvent.EventType.EVENT_ADD},
                {"Delete This Changer", OnChangerEvent.EventType.EVENT_DELETE},
            };

            foreach (var key in menuItems.Keys)
            {
                var eventType = menuItems[key];
                menu.AddItem(
                    new GUIContent(key),
                    false,
                    () =>
                    {
                        Emit(new OnChangerEvent(eventType, this.changerId));
                    }
                );
            }
            menu.ShowAsContext();
        }

        private void ShowContextMenuToOtherAuto(string nextAutoId)
        {
            var menu = new GenericMenu();

            var menuItems = new Dictionary<string, OnChangerEvent.EventType>{
                {"Open Next Auto:" + AutoNameFromId(nextAutoId), OnChangerEvent.EventType.EVENT_CHANGE_AUTO},
            };

            foreach (var key in menuItems.Keys)
            {
                var eventType = menuItems[key];
                menu.AddItem(
                    new GUIContent(key),
                    false,
                    () =>
                    {
                        Emit(new OnChangerEvent(eventType, this.changerId, nextAutoId));
                    }
                );
            }

            menu.ShowAsContext();
        }

        private string GetAutoNameById(string autoId)
        {
            return " " + AutoNameFromId(autoId);
        }

        public void SetActive()
        {
            active = true;

            ApplyDataToInspector();
            Selection.activeObject = changerPlateInspector;
        }

        public void SetDeactive()
        {
            active = false;
        }

        public bool IsActive()
        {
            return active;
        }


        public void ApplyDataToInspector()
        {
            if (changerPlateInspector == null) changerPlateInspector = ScriptableObject.CreateInstance("ChangerPlateInspector") as ChangerPlateInspector;

            changerPlateInspector.changerPlate = this;
        }
    }
    [Serializable]
    public class ConditionTypeValuePair
    {
        [SerializeField] public string conditionType;
        [SerializeField] public string conditionValue;

        public ConditionTypeValuePair(string conditionType, string conditionValue)
        {
            this.conditionType = conditionType;
            this.conditionValue = conditionValue;
        }
    }

    [Serializable]
    public class CombinationBinds
    {
        [SerializeField] public string bindKind;
        [SerializeField] public List<ConditionTypeValuePair> combinations;

        public CombinationBinds(string bindKind)
        {
            this.bindKind = bindKind;
            this.combinations = new List<ConditionTypeValuePair>();
        }

        public CombinationBinds(string bindKind, List<ConditionTypeValuePair> combinations)
        {
            this.bindKind = bindKind;
            this.combinations = combinations;
        }


    }
}