using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace Automatine
{
    public class AutomatineGUIWindow : EditorWindow
    {
        [SerializeField] private List<AutoComponent> autos = new List<AutoComponent>();
        [SerializeField] List<ConditionData> conditions;

        private NewAutoWindow newAutoWindow;
        private ConditionsGUIAuxWindow conditionsWindow;


        private DateTime lastLoaded = DateTime.MinValue;

        private struct ManipulateTargets
        {
            public List<string> activeObjectIds;

            public ManipulateTargets(List<string> activeObjectIds)
            {
                this.activeObjectIds = activeObjectIds;
            }
        }
        private ManipulateTargets manipulateTargets = new ManipulateTargets(new List<string>());

        private float selectedPos;
        private int selectedFrame;
        private float cursorPos;
        private float scrollPosX;
        private float scrollPosY;

        private int footerHeight = AutomatineGUISettings.CHANGER_SPACE_HEIGHT_MIN;


        private bool repaint;


        private GUIStyle coroutineBackgroundStyle;

        private GUIStyle activeFrameLabel;
        private GUIStyle activeConditionValueLabel;

        private struct ManipulateEvents
        {
            public bool keyLeft;
            public bool keyRight;
            public bool keyUp;
            public bool keyDown;
        }
        private ManipulateEvents manipulateEvents = new ManipulateEvents();

        private List<OnAutomatineEvent> automatineEventStacks = new List<OnAutomatineEvent>();
        private List<OnConditionsEvent> conditionEventStacks = new List<OnConditionsEvent>();


        private TypeGeneratorWindow typeGeneratorWindow;
        private ValueGeneratorWindow valueGeneratorWindow;

        private Timer showCoroutineTimer;
        private CoroutineWindow coroutineWindow;
        private string showingCoroutineParentTackId = string.Empty;
        private Rect coroutineWindowRect;


        [SerializeField] private int modifiedCount = 0;

        /**
			Menu item for AssetGraph.
		*/

        [MenuItem("Window/Automatine")]
        static void ShowEditor()
        {
            EditorWindow.GetWindow<AutomatineGUIWindow>(typeof(AutomatineGUIWindow));
        }

        public void OnFocus()
        {
            // update handlers. these static handlers are erase when window is full-screened and badk to normal window.
            AutoComponent.Emit = AutomatineEmit;
            TimelineTrack.Emit = AutomatineEmit;
            TackPoint.Emit = AutomatineEmit;
            ChangerPlate.Emit = ChangerEmit;
        }

        public void OnEnable()
        {
            InitializeResources();

            ReloadSavedData();

            AutomatineGUISettings.dummyTex = AssetDatabase.LoadAssetAtPath(AutomatineGUISettings.RESOURCE_DUMMY_BG, typeof(Texture2D)) as Texture2D;

            // handler for Undo/Redo
            Undo.undoRedoPerformed += () =>
            {
                Repaint();
                if (conditionsWindow != null) conditionsWindow.Repaint();
            };

            AutoComponent.Emit = AutomatineEmit;
            TimelineTrack.Emit = AutomatineEmit;
            TackPoint.Emit = AutomatineEmit;
            ChangerPlate.Emit = ChangerEmit;

            showingCoroutineParentTackId = string.Empty;

            InitializeAutosView();
        }

        private void InitializeAutosView()
        {
            this.titleContent = new GUIContent("Automatine");

            this.wantsMouseMove = true;
            this.minSize = new Vector2(600f, 300f);
        }

        private void ReloadSavedData()
        {
            /*
				load saved data.
			*/
            var basePath = FileController.PathCombine(Application.dataPath, AutomatineSettings.AUTOMATINE_ASSET_NAME, AutomatineSettings.AUTOMATINE_EDITOR_PATH, AutomatineSettings.AUTOMATINE_DATA_PATH);

            // create Data folder under Assets/Automatine
            if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);

            var dataPath = FileController.PathCombine(basePath, AutomatineSettings.AUTOMATINE_DATA_FILENAME);
            var deserialized = new AutomatineData();
            var lastModified = DateTime.Now;

            autos = new List<AutoComponent>();

            if (File.Exists(dataPath))
            {
                // load
                deserialized = LoadData(dataPath);

                var lastModifiedStr = deserialized.lastModified;
                lastModified = Convert.ToDateTime(lastModifiedStr);

                foreach (var autoData in deserialized.autoDatas)
                {
                    var autoId = autoData.autoId;
                    var loadedAutoComponent = new AutoComponent(autoData);

                    /*
						load changer datas from changersData.
					*/
                    foreach (var changersData in deserialized.changersDatas)
                    {
                        foreach (var changer in changersData.changers)
                        {
                            if (changer.rootAutoId == autoId)
                            {
                                loadedAutoComponent.AddChanger(new ChangerPlate(changer));
                                continue;
                            }

                            if (changer.branchs[0].nextAutoId == autoId)
                            {
                                loadedAutoComponent.AddRootChanger(new ChangerPlate(changer));
                                continue;
                            }

                            if (changer.finallyBranch.finallyAutoId == autoId)
                            {
                                loadedAutoComponent.AddRootChanger(new ChangerPlate(changer));
                                continue;
                            }
                        }
                    }

                    autos.Add(loadedAutoComponent);
                }

                conditions = deserialized.conditionsDatas;
            }
            else
            {
                // renew empty autos and changers.
                var firstAuto = GenerateFirstAuto();
                autos.Add(firstAuto);
                SetAutoToActive(firstAuto.autoId);
                SaveData();

                // reload.
                deserialized = LoadData(dataPath);
                conditions = deserialized.conditionsDatas;
            }


            /*
				do nothing if json does not modified after load.
			*/
            if (lastModified == lastLoaded) return;
            lastLoaded = lastModified;

            // activate saved auto.
            var lastActiveAutoId = deserialized.lastActiveAutoId;

            foreach (var auto in autos)
            {
                auto.SetDeactive();
                if (auto.autoId == lastActiveAutoId)
                {
                    auto.SetActive();

                    scrollPosX = deserialized.lastScrolledPosX;
                    scrollPosY = deserialized.lastScrolledPosY;

                    break;
                }
            }

            modifiedCount = 0;
        }

        private AutomatineData LoadData(string dataPath)
        {
            var dataStr = string.Empty;
            using (var sr = new StreamReader(dataPath))
            {
                dataStr = sr.ReadToEnd();
            }
            return JsonUtility.FromJson<AutomatineData>(dataStr);
        }

        private void SaveData()
        {
            var lastModified = DateTime.Now;
            SaveData(lastModified, autos, ModifyingAuto().autoId, scrollPosX, scrollPosY);
            modifiedCount = modifiedCount + 1;
        }

        private void SaveData(
            DateTime lastModified,
            List<AutoComponent> currentAutos,
            string lastActiveAutoId,
            float lastScrollPosX,
            float lastScrollPosY
        )
        {
            var currentAutosList = new List<AutoData>();
            var currentChangersList = new List<ChangersInAuto>();

            foreach (var auto in currentAutos)
            {
                var changerPlates = auto.ChangerPlates();

                var changerDatas = ChangerDatas(changerPlates);

                currentChangersList.Add(new ChangersInAuto(auto.autoId, changerDatas));

                var timelineList = new List<TimelineData>();
                foreach (var timeline in auto.TimelineTracks())
                {

                    var tackList = new List<TackData>();
                    foreach (var tack in timeline.TackPoints().OrderBy(tack => tack.start))
                    {
                        var tackDict = new TackData(tack.tackId, tack.info, tack.start, tack.span, tack.conditionType, tack.conditionValue, tack.routineIds);

                        tackList.Add(tackDict);
                    }

                    var timelineDict = new TimelineData(timeline.timelineId, timeline.info, tackList);

                    timelineList.Add(timelineDict);
                }

                var autoData = new AutoData(auto.autoId, auto.name, auto.info, auto.comments, timelineList);
                currentAutosList.Add(autoData);
            }

            var data = new AutomatineData(lastModified.ToString(), currentAutosList, currentChangersList, conditions, lastActiveAutoId, lastScrollPosX, lastScrollPosY);

            var dataStr = JsonUtility.ToJson(data);
            var targetDirPath = FileController.PathCombine(Application.dataPath, AutomatineSettings.AUTOMATINE_ASSET_NAME, AutomatineSettings.AUTOMATINE_EDITOR_PATH, AutomatineSettings.AUTOMATINE_DATA_PATH);

            if (!Directory.Exists(targetDirPath))
            {
                Directory.CreateDirectory(targetDirPath);
            }

            var targetFilePath = FileController.PathCombine(Application.dataPath, AutomatineSettings.AUTOMATINE_ASSET_NAME, AutomatineSettings.AUTOMATINE_EDITOR_PATH, AutomatineSettings.AUTOMATINE_DATA_PATH, AutomatineSettings.AUTOMATINE_DATA_FILENAME);
            using (var sw = new StreamWriter(targetFilePath))
            {
                sw.Write(dataStr);
            }
        }

        private List<ChangerData> ChangerDatas(List<ChangerPlate> changerPlates)
        {
            var changerDatas = new List<ChangerData>();
            foreach (var changer in changerPlates)
            {
                var binds = new List<ConditionBindData>();
                foreach (var bindSource in changer.branchBinds)
                {
                    var kind = bindSource.bindKind;
                    var combinations = bindSource.combinations;

                    var conditionTypeValueDatas = new List<ConditionTypeValueData>();
                    foreach (var combination in combinations)
                    {
                        conditionTypeValueDatas.Add(
                            new ConditionTypeValueData(combination.conditionType, combination.conditionValue)
                        );
                    }

                    binds.Add(
                        new ConditionBindData(kind, conditionTypeValueDatas)
                    );
                }

                changerDatas.Add(
                    new ChangerData(
                        changer.changerId,
                        changer.changerName,
                        changer.rootAutoId,
                        changer.comments,
                        new List<BranchData>{
                            new BranchData(
                                new List<string>(),
                                binds,
                                changer.branchAutoId,
                                changer.useContinue,
                                changer.branchInheritTypes
                            )
                        },
                        new FinallyBranchData(changer.finallyAutoId, changer.finallyInheritTypes)
                    )
                );
            }
            return changerDatas;
        }

        private void InitializeResources()
        {
            AutomatineGUISettings.tickTex = AssetDatabase.LoadAssetAtPath(AutomatineGUISettings.RESOURCE_TICK, typeof(Texture2D)) as Texture2D;

            // timeline
            AutomatineGUISettings.timelineHeaderTex = AssetDatabase.LoadAssetAtPath(AutomatineGUISettings.RESOURCE_TRACK_HEADER_BG, typeof(Texture2D)) as Texture2D;
            AutomatineGUISettings.conditionLineBgTex = AssetDatabase.LoadAssetAtPath(AutomatineGUISettings.RESOURCE_CONDITIONLINE_BG, typeof(Texture2D)) as Texture2D;
            AutomatineGUISettings.frameTex = AssetDatabase.LoadAssetAtPath(AutomatineGUISettings.RESOURCE_TRACK_FRAME_BG, typeof(Texture2D)) as Texture2D;
            AutomatineGUISettings.whitePointTex = AssetDatabase.LoadAssetAtPath(AutomatineGUISettings.RESOURCE_TACK_WHITEPOINT, typeof(Texture2D)) as Texture2D;
            AutomatineGUISettings.grayPointTex = AssetDatabase.LoadAssetAtPath(AutomatineGUISettings.RESOURCE_TACK_GRAYPOINT, typeof(Texture2D)) as Texture2D;
            AutomatineGUISettings.whitePointSingleTex = AssetDatabase.LoadAssetAtPath(AutomatineGUISettings.RESOURCE_TACK_WHITEPOINT_SINGLE, typeof(Texture2D)) as Texture2D;
            AutomatineGUISettings.grayPointSingleTex = AssetDatabase.LoadAssetAtPath(AutomatineGUISettings.RESOURCE_TACK_GRAYPOINT_SINGLE, typeof(Texture2D)) as Texture2D;

            // tack
            AutomatineGUISettings.tackInfinityTex = AssetDatabase.LoadAssetAtPath(AutomatineGUISettings.RESOURCE_TACK_INFINITY, typeof(Texture2D)) as Texture2D;

            // changer
            AutomatineGUISettings.changerBaseTex = AssetDatabase.LoadAssetAtPath(AutomatineGUISettings.RESOURCE_CHANGER_BG, typeof(Texture2D)) as Texture2D;
            AutomatineGUISettings.changerNameTex = AssetDatabase.LoadAssetAtPath(AutomatineGUISettings.RESOURCE_CHANGER_NAME, typeof(Texture2D)) as Texture2D;
            AutomatineGUISettings.changerItemBaseTex = AssetDatabase.LoadAssetAtPath(AutomatineGUISettings.RESOURCE_CHANGER_ITEM_BG, typeof(Texture2D)) as Texture2D;
            AutomatineGUISettings.changerBranchTex = AssetDatabase.LoadAssetAtPath(AutomatineGUISettings.RESOURCE_CHANGER_BRANCH, typeof(Texture2D)) as Texture2D;
            AutomatineGUISettings.changerContinueTex = AssetDatabase.LoadAssetAtPath(AutomatineGUISettings.RESOURCE_CHANGER_CONTINUE, typeof(Texture2D)) as Texture2D;
            AutomatineGUISettings.changerFinallyTex = AssetDatabase.LoadAssetAtPath(AutomatineGUISettings.RESOURCE_CHANGER_FINALLY, typeof(Texture2D)) as Texture2D;

            AutomatineGUISettings.activeObjectBaseTex = AssetDatabase.LoadAssetAtPath(AutomatineGUISettings.RESOURCE_ACTIVE_OBJECT_BASE, typeof(Texture2D)) as Texture2D;

            // coroutine
            AutomatineGUISettings.coroutineBackgroundTex = AssetDatabase.LoadAssetAtPath(AutomatineGUISettings.RESOURCE_COROUTINE_BACKGROUND, typeof(Texture2D)) as Texture2D;
            coroutineBackgroundStyle = new GUIStyle();
            coroutineBackgroundStyle.normal.background = AutomatineGUISettings.coroutineBackgroundTex;

            activeFrameLabel = new GUIStyle();
            activeFrameLabel.normal.textColor = Color.white;

            activeConditionValueLabel = new GUIStyle();
            activeConditionValueLabel.fontSize = 11;
            activeConditionValueLabel.normal.textColor = Color.white;

            AutomatineGUISettings.coroutineStyle = new GUIStyle();
            AutomatineGUISettings.coroutineStyle.normal.textColor = Color.white;
            AutomatineGUISettings.coroutineStyle.richText = true;

            AutomatineGUISettings.tickStyle = new GUIStyle();
            AutomatineGUISettings.tickStyle.normal.background = AutomatineGUISettings.tickTex;
        }


        private int drawCounter = 0;
        private void Update()
        {
            drawCounter++;

            if (drawCounter % 5 != 0) return;


            if (10000 < drawCounter) drawCounter = 0;

            var consumed = false;
            // emit events.
            if (manipulateEvents.keyLeft)
            {
                SelectPreviousTack();
                consumed = true;
            }
            if (manipulateEvents.keyRight)
            {
                SelectNextTack();
                consumed = true;
            }

            if (manipulateEvents.keyUp)
            {
                SelectAheadObject();
                consumed = true;
            }
            if (manipulateEvents.keyDown)
            {
                SelectBelowObject();
                consumed = true;
            }

            // renew.
            if (consumed) manipulateEvents = new ManipulateEvents();
        }

        private void SelectPreviousTack()
        {
            if (!HasSelectedAuto()) return;

            var auto = ModifyingAuto();

            if (manipulateTargets.activeObjectIds.Any())
            {
                if (manipulateTargets.activeObjectIds.Count == 1)
                {
                    auto.SelectPreviousTackOfTimelines(manipulateTargets.activeObjectIds[0]);
                }
                else
                {
                    Debug.Log("複数カウントがあるので、");//複数選択してある場合は一番左のものの一個左、選択も更新、表示範囲も追随
                                              // 複数のタイムラインに対しての選択
                }
            }

            if (!manipulateTargets.activeObjectIds.Any()) return;

            var currentSelectedFrame = auto.GetStartFrameById(manipulateTargets.activeObjectIds[0]);
            if (0 <= currentSelectedFrame)
            {
                FocusToFrame(currentSelectedFrame);
            }
        }

        private void SelectNextTack()
        {
            if (!HasSelectedAuto()) return;

            var auto = ModifyingAuto();
            if (manipulateTargets.activeObjectIds.Any())
            {
                if (manipulateTargets.activeObjectIds.Count == 1)
                {
                    auto.SelectNextTackOfTimelines(manipulateTargets.activeObjectIds[0]);
                }
                else
                {
                    Debug.Log("複数カウントがあるので、");//複数選択してある場合は一番左のものの一個左、選択も更新、表示範囲も追随
                                              // 複数のタイムラインに対しての選択
                }
            }

            if (!manipulateTargets.activeObjectIds.Any()) return;

            var currentSelectedFrame = auto.GetStartFrameById(manipulateTargets.activeObjectIds[0]);
            if (0 <= currentSelectedFrame)
            {
                FocusToFrame(currentSelectedFrame);
            }
        }

        private void SelectAheadObject()
        {
            if (!HasSelectedAuto()) return;

            var auto = ModifyingAuto();

            // if selecting object is top, select auto. unselect all other objects.
            if (auto.IsActiveTimelineOrContainsActiveObject(0))
            {
                var activeFrame = auto.GetStartFrameById(manipulateTargets.activeObjectIds[0]);

                AutomatineEmit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_UNSELECTED, null));

                if (0 <= activeFrame)
                {
                    // selectedFrame = activeFrame;
                    Debug.LogWarning("表示上の位置が更新されない。頭が冴えてる時にやろう。");
                }

                // set active.
                ModifyingAuto().SetActive();
                return;
            }

            if (!manipulateTargets.activeObjectIds.Any()) return;
            auto.SelectAboveObjectById(manipulateTargets.activeObjectIds[0]);

            var currentSelectedFrame = auto.GetStartFrameById(manipulateTargets.activeObjectIds[0]);
            if (0 <= currentSelectedFrame)
            {
                FocusToFrame(currentSelectedFrame);
            }
        }

        private void SelectBelowObject()
        {
            if (!HasSelectedAuto()) return;

            var auto = ModifyingAuto();

            if (manipulateTargets.activeObjectIds.Any())
            {
                auto.SelectBelowObjectById(manipulateTargets.activeObjectIds[0]);
                var currentSelectedFrame = auto.GetStartFrameById(manipulateTargets.activeObjectIds[0]);
                if (0 <= currentSelectedFrame)
                {
                    FocusToFrame(currentSelectedFrame);
                }
                return;
            }

            /*
				choose tack of first timeline under tick.
			*/
            auto.SelectTackAtFrame(selectedFrame);
        }


        /**
			draw GUI
	   	*/
        private void OnGUI()
        {
            var changed = false;

            var viewWidth = this.position.width;
            var viewHeight = this.position.height;

            DrawHeader(viewWidth, viewHeight);

            using (new GUI.ClipScope(new Rect(0, AutomatineGUISettings.HEADER_HEIGHT + AutomatineGUISettings.HEADER_SPAN, viewWidth, viewHeight - (AutomatineGUISettings.HEADER_HEIGHT + AutomatineGUISettings.HEADER_SPAN + footerHeight))))
            {
                changed = DrawAutoConponent(viewWidth, viewHeight - (AutomatineGUISettings.HEADER_HEIGHT));
            }

            DrawFooter(viewWidth, viewHeight, footerHeight);

            if (changed)
            {
                SaveData();
                Repaint();
            }
        }

        public void DrawHeader(float viewWidth, float viewHeight)
        {
            var shouldCompile = false;
            using (new EditorGUILayout.HorizontalScope(GUI.skin.box, GUILayout.Width(viewWidth), GUILayout.Height(AutomatineGUISettings.HEADER_HEIGHT)))
            {
                if (GUILayout.Button(string.Empty, "TL tab plus right", GUILayout.Width(AutomatineGUISettings.CONDITION_INSPECTOR_BOX_WIDTH / 2)))
                {
                    if (newAutoWindow == null) newAutoWindow = CreateInstance<NewAutoWindow>();
                    newAutoWindow.Emit = AutomatineEmit;
                    newAutoWindow.autoNames = autos.Select(auto => auto.name).ToList();
                    newAutoWindow.ShowAuxWindow();
                }

                GUILayout.Label("Auto");
                if (!HasSelectedAuto()) return;// automatically create new auto.

                var activeAutoName = ModifyingAuto().name;

                if (GUILayout.Button(activeAutoName, "Popup", GUILayout.Width(200)))
                {
                    Action<string> Selected = (string newAutoId) =>
                    {
                        SetAutoToActive(newAutoId);
                    };

                    var activeAutoId = ModifyingAuto().autoId;

                    var menu = new GenericMenu();
                    for (var i = 0; i < autos.Count; i++)
                    {
                        var autoId = autos[i].autoId;
                        var name = autos[i].name;
                        menu.AddItem(
                            new GUIContent(name), (activeAutoId == autoId), () => Selected(autoId)
                        );
                    }

                    menu.ShowAsContext();

                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Conditions"))
                {
                    if (conditionsWindow == null) conditionsWindow = CreateInstance<ConditionsGUIAuxWindow>();
                    conditionsWindow.Emit = ConditionsEmit;
                    conditionsWindow.conditions = conditions;
                    conditionsWindow.ShowAuxWindow();
                }


                GUILayout.FlexibleSpace();

                if (0 < modifiedCount)
                {
                    GUILayout.Label("saved.");
                }
                else
                {
                    GUILayout.Label(string.Empty);
                }

                if (GUILayout.Button("Export As Runtime Code"))
                {
                    var basePath = FileController.PathCombine(Application.dataPath, AutomatineSettings.AUTOMATINE_ASSET_NAME);

                    // create Data folder under Assets/Automatine
                    if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);

                    var dataPath = FileController.PathCombine(basePath, AutomatineSettings.AUTOMATINE_EDITOR_PATH, AutomatineSettings.AUTOMATINE_DATA_PATH, AutomatineSettings.AUTOMATINE_DATA_FILENAME);

                    var deserialized = LoadData(dataPath);

                    var generatedPath = FileController.PathCombine(Application.dataPath, AutomatineSettings.AUTOMATINE_ASSET_NAME, AutomatineSettings.AUTOMATINE_RUNTIME_PATH, AutomatineSettings.AUTOMATINE_CODE_GENERATION_DEST_PATH);

                    // condition definitions.
                    WriteConditionAndCollectedCondition(generatedPath, deserialized.conditionsDatas);

                    // autos
                    {
                        var autosPath = FileController.PathCombine(generatedPath, AutomatineSettings.AUTOMATINE_CODE_GENERATION_AUTOS_PATH);
                        FileController.RemakeDirectory(autosPath);
                        var autoDatas = deserialized.autoDatas;
                        WriteAutos(autosPath, autoDatas);
                    }

                    modifiedCount = 0;

                    shouldCompile = true;
                }

                if (GUILayout.Button("Export As Data"))
                {
                    var basePath = FileController.PathCombine(Application.dataPath, AutomatineSettings.AUTOMATINE_ASSET_NAME);

                    // create Data folder under Assets/Automatine
                    if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);

                    var dataPath = FileController.PathCombine(basePath, AutomatineSettings.AUTOMATINE_EDITOR_PATH, AutomatineSettings.AUTOMATINE_DATA_PATH, AutomatineSettings.AUTOMATINE_DATA_FILENAME);
                    if (!File.Exists(dataPath)) SaveData();

                    var deserialized = LoadData(dataPath);

                    var generatedPath = FileController.PathCombine(Application.dataPath, AutomatineSettings.AUTOMATINE_ASSET_NAME, AutomatineSettings.AUTOMATINE_RUNTIME_PATH, AutomatineSettings.AUTOMATINE_CODE_GENERATION_DEST_PATH);

                    // check diff between saved condition data vs exported condition enum.
                    var definitionPath = FileController.PathCombine(generatedPath, AutomatineSettings.AUTOMATINE_CODE_GENERATION_DEFINITIONS_PATH);
                    if (!Directory.Exists(definitionPath))
                    {
                        WriteConditionAndCollectedCondition(generatedPath, deserialized.conditionsDatas);
                        shouldCompile = true;
                    }
                    else
                    {// folder exists.
                     // check conditionType is exists or not.
                        var existConditionTypesSources = CollectedConditions.conditions;

                        var existConditionTypes = existConditionTypesSources.Select(condType => condType.ToString().Replace("AutoConditions+", string.Empty)).ToList();
                        var savedTypes = deserialized.conditionsDatas.Select(cond => cond.conditionType).ToList();

                        // check that saved conditionTypes are exists. if not, should generate code. 
                        foreach (var savedType in savedTypes)
                        {
                            if (!existConditionTypes.Contains(savedType))
                            {
                                Debug.Log("target conditionType does not output yet:" + savedType + ". compiling.");
                                shouldCompile = true;
                                continue;
                            }

                            // condition type is exists. check values.
                            var currentConditionType = ConditionGateway.ConditionTypeFromString(savedType);
                            if (currentConditionType == null)
                            {
                                Debug.LogWarning("saved type:" + currentConditionType + " is exists but failed to detect. compiling.");
                                shouldCompile = true;
                                continue;
                            }

                            var existsConditionValues = Enum.GetNames(currentConditionType);
                            var savedValues = deserialized.conditionsDatas.Where(cond => cond.conditionType == savedType).SelectMany(cond => cond.conditionValues).ToList();
                            foreach (var val in savedValues)
                            {
                                if (!existsConditionValues.Contains(val))
                                {
                                    Debug.LogWarning("saved type is not exist in definition yet:" + val + " compiling.");
                                    shouldCompile = true;
                                }
                            }
                        }
                    }


                    /*
						generate RuntimeAuto source json.
					*/
                    var runtimeAutoDataPath = FileController.PathCombine(generatedPath, AutomatineSettings.AUTOMATINE_DATA_GENERATION_PATH);
                    FileController.RemakeDirectory(runtimeAutoDataPath);

                    WriteRuntimeAutoJSON(deserialized.autoDatas, runtimeAutoDataPath);

                    modifiedCount = 0;

                    // if should compile, export all datas here.
                    if (shouldCompile)
                    {
                        // condition definitions.
                        WriteConditionAndCollectedCondition(generatedPath, deserialized.conditionsDatas);

                        // autos
                        {
                            var autosPath = FileController.PathCombine(generatedPath, AutomatineSettings.AUTOMATINE_CODE_GENERATION_AUTOS_PATH);
                            FileController.RemakeDirectory(autosPath);
                            var autoDatas = deserialized.autoDatas;
                            WriteAutos(autosPath, autoDatas);
                        }

                    }
                }
            }

            if (shouldCompile)
            {
                // compile.
                AssetDatabase.Refresh();
            }
        }

        private void WriteRuntimeAutoJSON(List<AutoData> autoDatas, string runtimeAutoDataPath)
        {
            foreach (var autoData in autoDatas)
            {
                Debug.LogWarning("changers are not yet ready.");
                var runtimeAutoData = autoData.GetRuntimeAutoData(new List<RuntimeChangerData>(), new List<RuntimeChangerData>());
                var json = JsonUtility.ToJson(runtimeAutoData);

                var path = runtimeAutoDataPath + autoData.name + AutomatineSettings.RUNTIME_DATA_EXTENSION;

                // export as AutoName.json.
                using (var sw = new StreamWriter(path))
                {
                    sw.WriteLine(json);
                }
            }
        }

        private void WriteConditionAndCollectedCondition(string generatedPath, List<ConditionData> conditionsDatas)
        {
            var definitionPath = FileController.PathCombine(generatedPath, AutomatineSettings.AUTOMATINE_CODE_GENERATION_DEFINITIONS_PATH);
            FileController.RemakeDirectory(definitionPath);

            // conditions.
            WriteConditions(definitionPath, conditionsDatas);

            // collected conditions.
            WriteCollectedConditions(definitionPath, conditionsDatas);
        }

        private void WriteConditions(string definitionPath, List<ConditionData> conditionsDatas)
        {
            foreach (var conditonData in conditionsDatas)
            {
                var conditionName = conditonData.conditionType;
                var conditionComments = new List<string>();//deserialized.comment;
                var conditionsAndComments = new List<ValueAndCommentData>();

                foreach (var conditionValue in conditonData.conditionValues)
                {
                    conditionsAndComments.Add(new ValueAndCommentData(conditionValue, "dummy comment"));
                }

                var desc = AutoDescriptor.AutoConditions("AutoConditions", conditionName, conditionComments, conditionsAndComments);

                var outputPath = FileController.PathCombine(definitionPath, conditionName + ".cs");

                // write condition file.
                using (var sw = new StreamWriter(outputPath))
                {
                    sw.Write(desc);
                }
            }
        }

        private void WriteCollectedConditions(string definitionPath, List<ConditionData> conditionsDatas)
        {
            var conditions = conditionsDatas.Select(cond => cond.conditionType).ToList();

            var desc = AutoDescriptor.CollectedConditions("CollectedConditions", conditions);

            var outputPath = FileController.PathCombine(definitionPath, "CollectedConditions" + ".cs");

            // write condition file.
            using (var sw = new StreamWriter(outputPath))
            {
                sw.Write(desc);
            }
        }

        private void WriteAutos(string autosPath, List<AutoData> autoDatas)
        {
            foreach (var autoData in autoDatas)
            {
                var desc = AutoDescriptor.Auto("<InitialParamType, UpdateParamType>", autoData);

                var outputPath = FileController.PathCombine(autosPath, autoData.name + ".cs");

                // write condition file.
                using (var sw = new StreamWriter(outputPath))
                {
                    sw.Write(desc);
                }
            }
        }

        private bool DrawAutoConponent(float viewWidth, float componentMaxHeight)
        {
            var xScrollIndex = -scrollPosX;
            var yOffsetPos = 0f;

            var timelineHeight = 0f;
            var inspectorHeight = AssumeConditionInspectorHeight(yOffsetPos);
            var timelineScopeHeight = componentMaxHeight - inspectorHeight;

            BeginWindows();
            {
                // draw component header.
                DrawConditionInspector(xScrollIndex, 0, viewWidth);

                // add already assumed inspector height.
                yOffsetPos += inspectorHeight;

                /*
					draw timelines & coroutineWindow.
				*/
                var timelineOffsetPos = scrollPosY + yOffsetPos;
                if (HasSelectedAuto())
                {
                    var activeAuto = ModifyingAuto();

                    // draw timelines.
                    timelineHeight = DrawTimelines(activeAuto, timelineOffsetPos, xScrollIndex, viewWidth);
                    timelineOffsetPos += activeAuto.TimelinesTotalHeight();

                    // draw tick
                    DrawTick();

                    // draw coroutine code window.
                    if (!string.IsNullOrEmpty(showingCoroutineParentTackId))
                    {
                        coroutineWindowRect = GUI.Window(AutomatineGUISettings.WINDOW_COROUTINEWINDOW_ID, coroutineWindowRect, (int unusedIndex) => coroutineWindow.DrawCoroutineWindow(), string.Empty, coroutineBackgroundStyle);
                        GUI.BringWindowToFront(AutomatineGUISettings.WINDOW_COROUTINEWINDOW_ID);
                    }
                }
            }
            EndWindows();

            DrawTickFrame();

            switch (Event.current.type)
            {
                // delete coroutine window when move.
                case EventType.MouseMove:
                    {
                        if (showCoroutineTimer != null)
                        {
                            showCoroutineTimer.Dispose();
                            showCoroutineTimer = null;
                        }

                        showingCoroutineParentTackId = string.Empty;
                        break;
                    }

                // mouse event handling.
                case EventType.MouseDown:
                    {
                        var touchedFrameCount = TimelineTrack.GetFrameOnTimelineFromAbsolutePosX(scrollPosX + (Event.current.mousePosition.x - AutomatineGUISettings.TIMELINE_CONDITIONBOX_SPAN));
                        if (touchedFrameCount < 0) touchedFrameCount = 0;
                        selectedPos = touchedFrameCount * AutomatineGUISettings.TACK_FRAME_WIDTH;
                        selectedFrame = touchedFrameCount;
                        repaint = true;

                        AutomatineEmit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_UNSELECTED, null));
                        break;
                    }
                case EventType.ContextClick:
                    {
                        var touchedFrameCount = TimelineTrack.GetFrameOnTimelineFromAbsolutePosX(scrollPosX + (Event.current.mousePosition.x - AutomatineGUISettings.TIMELINE_CONDITIONBOX_SPAN));
                        ShowContextMenu(touchedFrameCount);
                        break;
                    }
                case EventType.MouseUp:
                    {
                        var touchedFrameCount = TimelineTrack.GetFrameOnTimelineFromAbsolutePosX(scrollPosX + (Event.current.mousePosition.x - AutomatineGUISettings.TIMELINE_CONDITIONBOX_SPAN));
                        if (touchedFrameCount < 0) touchedFrameCount = 0;
                        selectedPos = touchedFrameCount * AutomatineGUISettings.TACK_FRAME_WIDTH;
                        selectedFrame = touchedFrameCount;
                        repaint = true;

                        // set auto to active.
                        ModifyingAuto().SetActive();
                        break;
                    }
                case EventType.MouseDrag:
                    {
                        var pos = scrollPosX + (Event.current.mousePosition.x - AutomatineGUISettings.TIMELINE_CONDITIONBOX_SPAN);
                        if (pos < 0) pos = 0;
                        selectedPos = pos - ((AutomatineGUISettings.TACK_FRAME_WIDTH / 2f) - 1f);
                        selectedFrame = TimelineTrack.GetFrameOnTimelineFromAbsolutePosX(pos);

                        FocusToFrame(selectedFrame);

                        repaint = true;
                        break;
                    }

                // scroll event handling.
                case EventType.ScrollWheel:
                    {
                        // reset coroutine timer when scroll.
                        if (showCoroutineTimer != null) showCoroutineTimer.Dispose();

                        if (0 != Event.current.delta.x)
                        {
                            scrollPosX = scrollPosX + (Event.current.delta.x * 2);
                            if (scrollPosX < 0) scrollPosX = 0;

                            repaint = true;
                        }

                        if (0 != Event.current.delta.y)
                        {
                            var scrollableHeight = (timelineHeight - timelineScopeHeight) + AutomatineGUISettings.TIMELINE_EMPTY_HEIGHT;
                            if (scrollableHeight < 0) scrollableHeight = 0;
                            var yDelta = (Event.current.delta.y);

                            scrollPosY = scrollPosY - yDelta;

                            if (0 < scrollPosY) scrollPosY = 0;

                            if (scrollableHeight <= Math.Abs(scrollPosY)) scrollPosY = -scrollableHeight;

                            repaint = true;
                        }

                        break;
                    }

                // key event handling.
                case EventType.KeyDown:
                    {
                        switch (Event.current.keyCode)
                        {
                            case KeyCode.LeftArrow:
                                {
                                    if (manipulateTargets.activeObjectIds.Count == 0)
                                    {

                                        selectedFrame = selectedFrame - 1;
                                        if (selectedFrame < 0) selectedFrame = 0;
                                        selectedPos = selectedFrame * AutomatineGUISettings.TACK_FRAME_WIDTH;
                                        repaint = true;

                                        FocusToFrame(selectedFrame);
                                    }
                                    manipulateEvents.keyLeft = true;
                                    Event.current.Use();
                                    break;
                                }
                            case KeyCode.RightArrow:
                                {
                                    if (manipulateTargets.activeObjectIds.Count == 0)
                                    {
                                        selectedFrame = selectedFrame + 1;
                                        selectedPos = selectedFrame * AutomatineGUISettings.TACK_FRAME_WIDTH;
                                        repaint = true;

                                        FocusToFrame(selectedFrame);
                                    }
                                    manipulateEvents.keyRight = true;
                                    Event.current.Use();
                                    break;
                                }
                            case KeyCode.UpArrow:
                                {
                                    manipulateEvents.keyUp = true;
                                    Event.current.Use();
                                    break;
                                }
                            case KeyCode.DownArrow:
                                {
                                    manipulateEvents.keyDown = true;
                                    Event.current.Use();
                                    break;
                                }
                        }
                        break;
                    }


            }

            // update cursor pos
            cursorPos = selectedPos - scrollPosX;

            /*
				commands
			*/
            if (Event.current.type == EventType.ValidateCommand)
            {

                switch (Event.current.commandName)
                {
                    case "Delete":
                        {
                            // Debug.Log("del!");
                            // if (!activeTimelineIds.Any() &&
                            // 	!activeTackIds.Any()) {
                            // 	Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_DELETE, activeTackIds));
                            // }
                            break;
                        }
                    case "Copy":
                        {
                            // Debug.Log("copy!");
                            // if (!string.IsNullOrEmpty(activeTimelineId) &&
                            // 	!string.IsNullOrEmpty(activeTackId)) {
                            // 	Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_COPY, activeTimelineId, activeTackId));
                            // }
                            break;
                        }
                    case "Cut":
                        {
                            // Debug.Log("cut!");
                            // if (!string.IsNullOrEmpty(activeTimelineId) &&
                            // 	!string.IsNullOrEmpty(activeTackId)) {
                            // 	Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_COPY, activeTimelineId, activeTackId));
                            // }
                            break;
                        }
                    case "Paste":
                        {
                            // Debug.Log("paste! 保持してあるTackのサイズ、名前、Routine情報を現在のTimelineに貼り付ける。newの内容で特定の状態を入れるかんじにするか");
                            // Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_PASTE, activeTimelineId, activeTackId));
                            break;
                        }
                }
            }

            if (repaint) HandleUtility.Repaint();

            UpdateAutoConditions();

            var changed = false;

            if (automatineEventStacks.Any())
            {
                foreach (var onAutomatineEvent in automatineEventStacks)
                {
                    var change = EmitAutomatineEventAfterDraw(onAutomatineEvent);
                    if (change) changed = true;
                }
                automatineEventStacks.Clear();
            }

            if (conditionEventStacks.Any())
            {
                foreach (var onConditionEvent in conditionEventStacks) EmitConditionEventAfterDraw(onConditionEvent);
                conditionEventStacks.Clear();
                changed = true;
            }

            return changed;
        }

        private void DrawFooter(float viewWidth, float viewHeight, float activeFooterHeight)
        {
            var changerStartY = viewHeight - activeFooterHeight;

            AutoComponent activeAuto;
            if (HasSelectedAuto())
            {
                activeAuto = ModifyingAuto();
            }
            else
            {
                return;
            }

            var changersRect = new Rect(0, changerStartY, viewWidth, activeFooterHeight);
            GUILayout.BeginArea(changersRect);
            {
                using (new EditorGUILayout.HorizontalScope(GUI.skin.box, GUILayout.Width(viewWidth), GUILayout.Height(activeFooterHeight)))
                {
                    // 一つのレイヤ(changer足すボタンとかその辺用)を作る

                    using (new EditorGUILayout.HorizontalScope(GUILayout.Width(viewWidth), GUILayout.Height(AutomatineGUISettings.CHANGER_HEADER_HEIGHT)))
                    {
                        // if (GUILayout.Button("======")) {
                        //	 Debug.Log("なんかせんとな、、このボタンを持って動かしたい。線の画像でいいか、、");
                        // }

                    }

                    var changerContentsHeight = activeFooterHeight - AutomatineGUISettings.CHANGER_HEADER_HEIGHT;

                    var width = viewWidth / 2;

                    // left, "from" changer.
                    GUILayout.BeginArea(new Rect(0, AutomatineGUISettings.CHANGER_HEADER_HEIGHT, width, changerContentsHeight));
                    {
                        var changerRect = new Rect(0, 0, width, changerContentsHeight);
                        GUI.DrawTexture(changerRect, AutomatineGUISettings.changerBaseTex);

                        Draw_FromChangerComponent(activeAuto, width);
                    }
                    GUILayout.EndArea();

                    // right, "to" changer.
                    GUILayout.BeginArea(new Rect(width, AutomatineGUISettings.CHANGER_HEADER_HEIGHT, width, changerContentsHeight));
                    {
                        Draw_ToChangerComponent(activeAuto, width);
                    }
                    GUILayout.EndArea();
                }
            }
            GUILayout.EndArea();

            if (changersRect.Contains(Event.current.mousePosition))
            {
                switch (Event.current.type)
                {
                    case EventType.MouseUp:
                        {
                            // is right clicked
                            if (Event.current.button == 1)
                            {
                                Debug.LogWarning("changer is not yet implemented.");
                                // ShowContextMenuForChanger();
                                break;
                            }
                            break;
                        }
                    default:
                        {
                            // Debug.Log("unhandled type:" + Event.current.type);
                            break;
                        }
                }
            }
        }

        private void ShowContextMenuForChanger()
        {
            var menu = new GenericMenu();

            switch (footerHeight)
            {
                case AutomatineGUISettings.CHANGER_SPACE_HEIGHT_MAX:
                    {
                        menu.AddItem(
                            new GUIContent("Add New Changer"),
                            false,
                            () =>
                            {
                                AddChanger(ModifyingAuto());
                            }
                        );
                        menu.AddDisabledItem(
                            new GUIContent("Open Changer Window")
                        );
                        menu.AddItem(
                            new GUIContent("Close Changer Window"),
                            false,
                            () =>
                            {
                                CloseChangerSpace();
                            }
                        );
                        break;
                    }
                case AutomatineGUISettings.CHANGER_SPACE_HEIGHT_MIN:
                    {
                        menu.AddItem(
                            new GUIContent("Open Changer Window"),
                            false,
                            () =>
                            {
                                OpenChangerSpace();
                            }
                        );
                        menu.AddDisabledItem(
                            new GUIContent("Close Changer Window")
                        );
                        break;
                    }
            }
            menu.ShowAsContext();
        }


        private void OpenChangerSpace()
        {
            footerHeight = AutomatineGUISettings.CHANGER_SPACE_HEIGHT_MAX;
        }
        private void CloseChangerSpace()
        {
            footerHeight = AutomatineGUISettings.CHANGER_SPACE_HEIGHT_MIN;
        }

        private void Draw_FromChangerComponent(AutoComponent activeAuto, float width)
        {
            // var current_FromChangers = 
            // foreach (var changer in current_FromChangers.Select((v, i) => new { v, i })) {
            //	 changer.v.Draw_FromChangerPlate(changer.i);
            // }
        }

        private void Draw_ToChangerComponent(AutoComponent activeAuto, float width)
        {
            var current_ToChangers = activeAuto.changers;

            foreach (var changer in current_ToChangers.Select((v, i) => new { v, i }))
            {
                changer.v.Draw_ToChangerPlate(changer.i, width);
            }
        }

        private void ShowContextMenu(int frame)
        {
            var menu = new GenericMenu();

            if (HasSelectedAuto())
            {
                var currentAuto = ModifyingAuto();
                var autoId = currentAuto.autoId;

                var menuItems = new Dictionary<string, OnAutomatineEvent.EventType>{
                    {"Add New Timeline", OnAutomatineEvent.EventType.EVENT_AUTO_ADDTIMELINE},
					// {"Add New Changer", OnAutomatineEvent.EventType.EVENT_AUTO_ADDCHANGER},
					{"Delete This Auto", OnAutomatineEvent.EventType.EVENT_AUTO_DELETED},
                };

                var tacks = ModifyingAuto().TackByFrame(frame);
                var coroutinesCount = 0;
                foreach (var tack in tacks) coroutinesCount += tack.routineIds.Count;

                if (0 < coroutinesCount)
                {
                    menuItems["Show " + coroutinesCount + " Coroutines At Frame " + frame] = OnAutomatineEvent.EventType.EVENT_SHOW_COROUTINEONFRAMEWINDOW;
                }

                foreach (var key in menuItems.Keys)
                {
                    var eventType = menuItems[key];
                    menu.AddItem(
                        new GUIContent(key),
                        false,
                        () =>
                        {
                            AutomatineEmit(new OnAutomatineEvent(eventType, autoId, frame));
                        }
                    );
                }
            }
            else
            {
                Debug.LogWarning("未調整");
                var menuItems = new Dictionary<string, OnAutomatineEvent.EventType>{
                    {"Add New Auto", OnAutomatineEvent.EventType.EVENT_AUTO_ADDAUTO},
                };

                foreach (var key in menuItems.Keys)
                {
                    var eventType = menuItems[key];
                    menu.AddItem(
                        new GUIContent(key),
                        false,
                        () =>
                        {
                            AutomatineEmit(new OnAutomatineEvent(eventType, string.Empty, -1));
                        }
                    );
                }
            }

            menu.ShowAsContext();
        }

        private AutoComponent GenerateFirstAuto()
        {
            var tackPoints = new List<TackPoint>();
            tackPoints.Add(new TackPoint(0, AutomatineGUISettings.DEFAULT_TACK_NAME, 0, 10));

            var timelines = new List<TimelineTrack>();
            timelines.Add(new TimelineTrack(0, AutomatineGUISettings.DEFAULT_TIMELINE_NAME, tackPoints));

            return new AutoComponent(AutomatineGUISettings.DEFAULT_AUTO_NAME, AutomatineGUISettings.DEFAULT_AUTO_INFO, timelines);
        }

        private AutoComponent NewAuto(string autoName)
        {
            var tackPoints = new List<TackPoint>();
            tackPoints.Add(new TackPoint(0, AutomatineGUISettings.DEFAULT_TACK_NAME, 0, 10));

            var timelines = new List<TimelineTrack>();
            timelines.Add(new TimelineTrack(0, AutomatineGUISettings.DEFAULT_TIMELINE_NAME, tackPoints));
            return new AutoComponent(autoName, AutomatineGUISettings.DEFAULT_AUTO_INFO, new List<TimelineTrack>());
        }

        private void UpdateAutoConditions()
        {
            if (HasSelectedAuto())
            {
                var activeAuto = ModifyingAuto();
                activeAuto.UpdateTimelinesCondition();
            }
        }

        private float AssumeConditionInspectorHeight(float yOffset)
        {
            return yOffset
                + AutomatineGUISettings.CONDITION_INSPECTOR_FRAMECOUNT_HEIGHT
                + AutomatineGUISettings.CONDITION_INSPECTOR_FRAMELINE_HEIGHT
                + AssumeConditionLineHeight();
        }


        private void DrawConditionInspector(float xScrollIndex, float yIndex, float inspectorWidth)
        {
            GUI.Window(
                AutomatineGUISettings.WINDOW_CONDITIONINSPECTOR_ID,
                new Rect(0, 0, inspectorWidth, AssumeConditionInspectorHeight(yIndex)),
                (int ignored) =>
                {
                    var height = yIndex;

                    // draw control box.
                    {
                        GUI.DrawTexture(new Rect(0, yIndex, AutomatineGUISettings.TIMELINE_CONDITIONBOX_WIDTH, AutomatineGUISettings.CONDITION_INSPECTOR_FRAMECOUNT_HEIGHT + AutomatineGUISettings.CONDITION_INSPECTOR_FRAMELINE_HEIGHT), AutomatineGUISettings.dummyTex);
                    }

                    var frameRailWidth = inspectorWidth - AutomatineGUISettings.TIMELINE_CONDITIONBOX_SPAN;
                    var assumedHeight = AssumeConditionInspectorHeight(yIndex);

                    // draw frame.
                    {
                        GUI.BeginGroup(new Rect(AutomatineGUISettings.TIMELINE_CONDITIONBOX_SPAN, height, frameRailWidth, assumedHeight));
                        {
                            var internalHeight = 0f;

                            // count & frame in header.
                            {
                                TimelineTrack.DrawFrameBG(xScrollIndex, internalHeight, frameRailWidth, AutomatineGUISettings.CONDITION_INSPECTOR_FRAMELINE_HEIGHT, true);
                                internalHeight = internalHeight + AutomatineGUISettings.CONDITION_INSPECTOR_FRAMECOUNT_HEIGHT + AutomatineGUISettings.CONDITION_INSPECTOR_FRAMELINE_HEIGHT;
                            }

                            if (HasSelectedAuto())
                            {
                                var currentAuto = ModifyingAuto();
                                var timelines = currentAuto.TimelineTracks();
                                foreach (var timeline in timelines)
                                {
                                    internalHeight = internalHeight + AutomatineGUISettings.CONDITION_INSPECTOR_CONDITIONLINE_SPAN;

                                    DrawConditionLine(0, xScrollIndex, timeline, internalHeight, (int)((-xScrollIndex + inspectorWidth) / AutomatineGUISettings.TACK_FRAME_WIDTH));
                                    internalHeight = internalHeight + AutomatineGUISettings.CONDITION_INSPECTOR_CONDITIONLINE_HEIGHT;
                                }

                                if (timelines.Any())
                                {
                                    // add footer.
                                    internalHeight = internalHeight + AutomatineGUISettings.CONDITION_INSPECTOR_CONDITIONLINE_SPAN;
                                }
                            }
                        }
                        GUI.EndGroup();
                    }
                },
                string.Empty,
                "AnimationKeyframeBackground"
            );

            // let it front.
            GUI.BringWindowToFront(AutomatineGUISettings.WINDOW_CONDITIONINSPECTOR_ID);
        }

        private float DrawTimelines(AutoComponent activeAuto, float yOffsetPos, float xScrollIndex, float viewWidth)
        {
            var timelineTotalHeight = activeAuto.DrawTimelines(activeAuto, yOffsetPos, xScrollIndex, viewWidth);
            return timelineTotalHeight;
        }

        private void DrawTick()
        {
            var tickRect = new Rect(AutomatineGUISettings.TIMELINE_CONDITIONBOX_SPAN + cursorPos + (AutomatineGUISettings.TACK_FRAME_WIDTH / 2f) - 1f, 0f, 3f, this.position.height);
            GUI.Window(AutomatineGUISettings.WINDOW_TICK_ID, tickRect, (int unusedIndex) => { }, string.Empty, AutomatineGUISettings.tickStyle);
            GUI.BringWindowToFront(AutomatineGUISettings.WINDOW_TICK_ID);
        }


        private void DrawTickFrame()
        {
            GUI.BeginGroup(new Rect(AutomatineGUISettings.TIMELINE_CONDITIONBOX_SPAN, 0f, position.width - AutomatineGUISettings.TIMELINE_CONDITIONBOX_SPAN, position.height));
            {
                // draw frame count.
                if (selectedFrame == 0)
                {
                    GUI.Label(new Rect(cursorPos + 5f, 1f, 10f, AutomatineGUISettings.CONDITION_INSPECTOR_FRAMECOUNT_HEIGHT), "0", activeFrameLabel);
                }
                else
                {
                    var span = 0;
                    var selectedFrameStr = selectedFrame.ToString();
                    if (2 < selectedFrameStr.Length) span = ((selectedFrameStr.Length - 2) * 8) / 2;
                    GUI.Label(new Rect(cursorPos + 2 - span, 1f, selectedFrameStr.Length * 10, AutomatineGUISettings.CONDITION_INSPECTOR_FRAMECOUNT_HEIGHT), selectedFrameStr, activeFrameLabel);
                }
            }
            GUI.EndGroup();
        }

        private float AssumeConditionLineHeight()
        {
            var height = 0f;

            if (HasSelectedAuto())
            {
                var currentAuto = ModifyingAuto();
                var timelines = currentAuto.TimelineTracks();
                for (var i = 0; i < timelines.Count; i++)
                {
                    height = height + AutomatineGUISettings.CONDITION_INSPECTOR_CONDITIONLINE_SPAN;
                    height = height + AutomatineGUISettings.CONDITION_INSPECTOR_CONDITIONLINE_HEIGHT;
                }

                if (timelines.Any())
                {
                    // add footer.
                    height = height + AutomatineGUISettings.CONDITION_INSPECTOR_CONDITIONLINE_SPAN;
                }
            }

            return height;
        }

        private void DrawConditionLine(float xOffset, float xScrollIndex, TimelineTrack timeline, float yOffset, int viewFrameWidth)
        {
            foreach (var tack in timeline.TackPoints())
            {
                var start = tack.start;
                var drawSpan = tack.span;

                if (tack.span == AutomatineDefinitions.Tack.LIMIT_UNLIMITED) drawSpan = viewFrameWidth;


                var startPos = xOffset + xScrollIndex + (start * AutomatineGUISettings.TACK_FRAME_WIDTH);
                var length = drawSpan * AutomatineGUISettings.TACK_FRAME_WIDTH;
                var tex = tack.GetColorTex();

                // draw background.
                if (tack.IsActive())
                {
                    var condtionLineBgRect = new Rect(startPos, yOffset - 1, length, AutomatineGUISettings.CONDITION_INSPECTOR_CONDITIONLINE_HEIGHT + 2);
                    GUI.DrawTexture(condtionLineBgRect, AutomatineGUISettings.conditionLineBgTex);
                }
                else
                {
                    var condtionLineBgRect = new Rect(startPos, yOffset + 1, length, AutomatineGUISettings.CONDITION_INSPECTOR_CONDITIONLINE_HEIGHT - 2);
                    GUI.DrawTexture(condtionLineBgRect, AutomatineGUISettings.conditionLineBgTex);
                }

                // fill color.
                var condtionLineRect = new Rect(startPos + 1, yOffset, length - 2, AutomatineGUISettings.CONDITION_INSPECTOR_CONDITIONLINE_HEIGHT);
                GUI.DrawTexture(condtionLineRect, tex);
            }

            // draw condition text if tick is on the condition.
            foreach (var tack in timeline.TackPoints())
            {
                var start = tack.start;
                var span = tack.span;
                var conditionValue = tack.conditionValue;

                if (start <= selectedFrame && (selectedFrame < start + span || span == AutomatineDefinitions.Tack.LIMIT_UNLIMITED))
                {
                    GUI.Label(new Rect(cursorPos + (AutomatineGUISettings.TACK_FRAME_WIDTH / 2f) + 3f, yOffset - 5f, conditionValue.Length * 10f, 20f), conditionValue, activeConditionValueLabel);
                }
            }
        }


        private void ShowCoroutineWindow(object activeTackIdObj)
        {
            var activeTackId = activeTackIdObj.ToString();
            AutomatineEmit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_SHOW_COROUTINEWINDOW, activeTackId));
        }

        private void AutomatineEmit(OnAutomatineEvent onTrackEvent)
        {
            var type = onTrackEvent.eventType;
            // tack events.
            switch (type)
            {
                case OnAutomatineEvent.EventType.EVENT_UNSELECTED:
                    {
                        manipulateTargets = new ManipulateTargets(new List<string>());

                        Undo.RecordObject(this, "Unselect");

                        var selectedAuto = ModifyingAuto();
                        selectedAuto.DeactivateAllObjects();
                        Repaint();
                        return;
                    }
                case OnAutomatineEvent.EventType.EVENT_OBJECT_SELECTED:
                    {
                        manipulateTargets = new ManipulateTargets(new List<string> { onTrackEvent.activeObjectId });

                        var selectedAuto = ModifyingAuto();

                        Undo.RecordObject(this, "Select");

                        // update selections.
                        selectedAuto.ActivateObjectsAndDeactivateOthers(manipulateTargets.activeObjectIds);

                        Repaint();
                        return;
                    }

                /*
					auto events.
				*/
                case OnAutomatineEvent.EventType.EVENT_AUTO_ADDAUTO:
                    {
                        var newAutoName = onTrackEvent.activeObjectId;
                        var newAuto = NewAuto(newAutoName);

                        Undo.RecordObject(this, "Add New Auto");

                        autos.Add(newAuto);

                        SetAutoToActive(newAuto.autoId);
                        return;
                    }
                case OnAutomatineEvent.EventType.EVENT_AUTO_ADDTIMELINE:
                    {
                        var activeAuto = ModifyingAuto();

                        Undo.RecordObject(this, "Add Timeline");

                        activeAuto.AddTimeline();

                        SaveData();
                        return;
                    }
                case OnAutomatineEvent.EventType.EVENT_AUTO_ADDCHANGER:
                    {
                        Undo.RecordObject(this, "Add New Changer");
                        AddChanger(ModifyingAuto());
                        SaveData();
                        return;
                    }
                case OnAutomatineEvent.EventType.EVENT_AUTO_DELETED:
                    {
                        if (HasSelectedAuto())
                        {
                            Undo.RecordObject(this, "Delete Auto");

                            var activeAuto = ModifyingAuto().autoId;
                            var deletingIndex = autos.FindIndex(auto => auto.autoId == activeAuto);
                            autos.RemoveAt(deletingIndex);

                            SaveData();
                        }
                        return;
                    }

                /*
					timeline events.
				*/
                case OnAutomatineEvent.EventType.EVENT_TIMELINE_ADDTACK:
                    {
                        automatineEventStacks.Add(onTrackEvent.Copy());
                        return;
                    }

                case OnAutomatineEvent.EventType.EVENT_TIMELINE_COPY:
                    {
                        // var targetTimelineId = onTrackEvent.timelineId;
                        // var targetTackId = onTrackEvent.tackId;

                        // var currentAuto = SelectedAuto();
                        // var currentTimelines = currentAuto.timelineTracks.Where(tl => tl.timelineId == targetTimelineId).ToList();
                        // if (!currentTimelines.Any()) return;

                        // var currentTimeline = currentTimelines[0];
                        Debug.Log("このTimelineを丸ごとコピーする");
                        return;
                    }
                case OnAutomatineEvent.EventType.EVENT_TIMELINE_CUT:
                    {
                        Debug.Log("tlのカット");

                        // tlを一時保存領域に置き、消す
                        // 一時保存領域の内容も入れ替わる感じか、、、面倒くせえ
                        return;
                    }
                case OnAutomatineEvent.EventType.EVENT_TIMELINE_PASTE:
                    {
                        Debug.Log("tlのペースト");

                        // var currentAuto = ModifyingAuto();

                        // やることは、内容の決まったTLのAdd。
                        return;
                    }
                case OnAutomatineEvent.EventType.EVENT_TIMELINE_DELETE:
                    {
                        var targetTimelineId = onTrackEvent.activeObjectId;
                        var activeAuto = ModifyingAuto();

                        Undo.RecordObject(this, "Delete Timeline");

                        activeAuto.DeleteObjectById(targetTimelineId);

                        Repaint();

                        SaveData();
                        return;
                    }

                /*
					tack events.
				*/
                case OnAutomatineEvent.EventType.EVENT_TACK_MOVING:
                    {
                        var movingTackId = onTrackEvent.activeObjectId;

                        var activeAuto = ModifyingAuto();

                        activeAuto.SetMovingTackToTimelimes(movingTackId);
                        break;
                    }
                case OnAutomatineEvent.EventType.EVENT_TACK_MOVED:
                    {

                        Undo.RecordObject(this, "Move Tack");

                        return;
                    }
                case OnAutomatineEvent.EventType.EVENT_TACK_MOVED_AFTER:
                    {
                        var targetTackId = onTrackEvent.activeObjectId;

                        var activeAuto = ModifyingAuto();
                        var activeTimelineIndex = activeAuto.GetTackContainedTimelineIndex(targetTackId);
                        if (0 <= activeTimelineIndex)
                        {
                            activeAuto.UpdateByTackMoved(activeTimelineIndex, targetTackId);

                            Repaint();
                            SaveData();
                        }
                        return;
                    }
                case OnAutomatineEvent.EventType.EVENT_TACK_DELETEAFTERUNLIMITED:
                    {
                        var unlimitedTackId = onTrackEvent.activeObjectId;
                        var targetTimeline = ModifyingAuto().TimelineByTack(unlimitedTackId);
                        var tacksAfterUnlimitedTack = targetTimeline.TacksAfterTack(unlimitedTackId);

                        foreach (var deletingTack in tacksAfterUnlimitedTack)
                        {
                            automatineEventStacks.Add(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_TACK_DELETED, deletingTack.tackId));
                        }
                        return;
                    }
                case OnAutomatineEvent.EventType.EVENT_TACK_DELETED:
                    {
                        automatineEventStacks.Add(onTrackEvent.Copy());
                        return;
                    }

                case OnAutomatineEvent.EventType.EVENT_TACK_REMOVECOROUTINE:
                    {
                        automatineEventStacks.Add(onTrackEvent.Copy());
                        return;
                    }


                case OnAutomatineEvent.EventType.EVENT_TACK_COPY:
                    {
                        // var targetTimelineId = onTrackEvent.timelineId;
                        // var targetTackId = onTrackEvent.tackId;

                        // var currentAuto = SelectedAuto();
                        // var currentTimelines = currentAuto.timelineTracks.Where(tl => tl.timelineId == targetTimelineId).ToList();
                        // if (!currentTimelines.Any()) return;

                        // var currentTimeline = currentTimelines[0];
                        // var currentTacks = currentTimeline.tackPoints.Where(tack => tack.tackId == targetTackId).ToList();
                        // if (!currentTacks.Any()) return;

                        // var currentTack = currentTacks[0];
                        Debug.Log("対象のtackのデータを取り出してノートに保存する。Autoをまたぐ可能性があるのか。なので、なんか、、やっぱJSONかなあ、、");
                        return;
                    }

                case OnAutomatineEvent.EventType.EVENT_SHOW_COROUTINEWINDOW:
                    {
                        automatineEventStacks.Add(onTrackEvent.Copy());
                        return;
                    }

                case OnAutomatineEvent.EventType.EVENT_SHOW_COROUTINEONFRAMEWINDOW:
                    {
                        automatineEventStacks.Add(onTrackEvent.Copy());
                        return;
                    }


                case OnAutomatineEvent.EventType.EVENT_REFRESHTIMELINECONDITIONS:
                    {
                        var activeAuto = ModifyingAuto();
                        // update type constraints of all Timelines.
                        activeAuto.TimelineTracks().ForEach(timelineTrack => timelineTrack.UpdateCondition());
                        break;
                    }

                case OnAutomatineEvent.EventType.EVENT_REFRESHTACKCONDITIONS:
                    {
                        var activeTackId = onTrackEvent.activeObjectId;
                        var activeAuto = ModifyingAuto();

                        // update type constraints of all Timelines.
                        activeAuto.TimelineTracks().ForEach(timelineTrack => timelineTrack.UpdateCondition());

                        var targetTymelineType = activeAuto.TimelineByTack(activeTackId).conditionTypeConstraint;
                        var unselectableTypes = new List<string>();

                        if (string.IsNullOrEmpty(targetTymelineType) || targetTymelineType == AutomatineGUISettings.TIMELINE_CONDITION_EMPTY)
                        {
                            unselectableTypes = activeAuto.TimelineTracks().Select(track => track.conditionTypeConstraint).ToList();
                        }
                        else
                        {
                            unselectableTypes = conditions
                                .Select(conditonData => conditonData.conditionType)
                                .Where(t => t != targetTymelineType)
                                .ToList();
                        }

                        var targetTack = activeAuto.TackById(activeTackId);

                        // update targetTack's conditions to latest one.
                        targetTack.conditions = conditions;
                        targetTack.unselectableTypes = unselectableTypes;
                        return;
                    }

                case OnAutomatineEvent.EventType.EVENT_ADDNEWTYPE:
                    {
                        var targetTackId = onTrackEvent.activeObjectId;

                        if (typeGeneratorWindow == null) typeGeneratorWindow = CreateInstance<TypeGeneratorWindow>();

                        typeGeneratorWindow.existTypes = conditions.Select(cond => cond.conditionType).ToList();

                        typeGeneratorWindow.enter = (string newTypeName) =>
                        {
                            AutomatineEmit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_TACK_SETNEWTYPE, targetTackId, -1, newTypeName));
                        };

                        typeGeneratorWindow.ShowAuxWindow();
                        return;
                    }
                case OnAutomatineEvent.EventType.EVENT_ADDNEWVALUE:
                    {
                        var targetTackId = onTrackEvent.activeObjectId;

                        if (valueGeneratorWindow == null) valueGeneratorWindow = CreateInstance<ValueGeneratorWindow>();

                        var tackType = ModifyingAuto().TackById(targetTackId).conditionType;
                        valueGeneratorWindow.type = tackType;
                        valueGeneratorWindow.existValues = conditions.Where(cond => cond.conditionType == tackType).SelectMany(cond => cond.conditionValues).ToList();

                        valueGeneratorWindow.enter = (string newValueName) =>
                        {
                            AutomatineEmit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_TACK_SETNEWVALUE, targetTackId, -1, newValueName));
                        };

                        valueGeneratorWindow.ShowAuxWindow();
                        return;
                    }



                case OnAutomatineEvent.EventType.EVENT_TACK_SETNEWTYPE:
                    {
                        var targetTackId = onTrackEvent.activeObjectId;
                        var newType = onTrackEvent.message;

                        var targetTack = ModifyingAuto().TackById(targetTackId);

                        Undo.RecordObject(this, "Set Tack Type");

                        conditions.Add(new ConditionData(newType, new List<string>()));
                        targetTack.conditionType = newType;
                        targetTack.conditionValue = string.Empty;

                        SaveData();
                        return;
                    }

                case OnAutomatineEvent.EventType.EVENT_TACK_SETNEWVALUE:
                    {
                        var targetTackId = onTrackEvent.activeObjectId;
                        var newValue = onTrackEvent.message;

                        var targetTack = ModifyingAuto().TackById(targetTackId);

                        var addingValueSTypeIndex = conditions.FindIndex(conditionData => conditionData.conditionType == targetTack.conditionType);
                        if (addingValueSTypeIndex < 0) break;

                        Undo.RecordObject(this, "Set Tack Value");

                        conditions[addingValueSTypeIndex].conditionValues.Add(newValue);
                        targetTack.conditionValue = newValue;

                        SaveData();
                        return;
                    }

                case OnAutomatineEvent.EventType.EVENT_TACK_SHOWCOROUTINES:
                    {
                        var targetTackId = onTrackEvent.activeObjectId;

                        // restart timer for show coroutine code.
                        if (showCoroutineTimer != null) showCoroutineTimer.Dispose();
                        showCoroutineTimer = new Timer(ShowCoroutineWindow, targetTackId, AutomatineGUISettings.SHOW_COROUTINE_WAIT, Timeout.Infinite);

                        return;
                    }

                case OnAutomatineEvent.EventType.EVENT_ADDNEWCOROUTINE:
                    {
                        var newCoroutineSlotIndex = onTrackEvent.frame;
                        var newCoroutineId = onTrackEvent.message;
                        var activeTackId = onTrackEvent.activeObjectId;

                        Undo.RecordObject(this, "Set New Coroutine");

                        // generate Coroutine file at Generated folder.
                        var generateTargetFolderPath = FileController.PathCombine(Application.dataPath, AutomatineSettings.AUTOMATINE_ASSET_NAME, AutomatineSettings.AUTOMATINE_RUNTIME_PATH, AutomatineSettings.AUTOMATINE_CODE_GENERATION_DEST_PATH, AutomatineSettings.AUTOMATINE_CODE_GENERATION_COROUTINE_PATH);
                        if (!Directory.Exists(generateTargetFolderPath)) FileController.RemakeDirectory(generateTargetFolderPath);

                        var desc = AutoDescriptor.Routine("RoutineContexts <InitialParamType, UpdateParamType> : RoutineBase<InitialParamType, UpdateParamType>", newCoroutineId, new List<string>());

                        var newCoroutinePath = FileController.PathCombine(generateTargetFolderPath, newCoroutineId + ".cs");

                        // write class file
                        using (var sw = new StreamWriter(newCoroutinePath))
                        {
                            sw.Write(desc);
                        }

                        var activeTack = ModifyingAuto().TackById(activeTackId);
                        activeTack.routineIds[newCoroutineSlotIndex] = newCoroutineId;

                        SaveData();

                        AssetDatabase.Refresh();

                        return;
                    }

                case OnAutomatineEvent.EventType.EVENT_UNDO:
                    {
                        var undoMessage = onTrackEvent.message;
                        Undo.RecordObject(this, undoMessage);
                        return;
                    }
                case OnAutomatineEvent.EventType.EVENT_SAVE:
                    {
                        SaveData();
                        return;
                    }



                default:
                    {
                        Debug.Log("no match type:" + type);
                        break;
                    }
            }
        }

        private void ChangerEmit(OnChangerEvent onChangerEvent)
        {
            var activeAuto = ModifyingAuto();
            var changers = activeAuto.changers;

            var type = onChangerEvent.eventType;
            switch (type)
            {
                case OnChangerEvent.EventType.EVENT_SELECTED:
                    {
                        var selectedChangerId = onChangerEvent.activeObjectId;

                        Undo.RecordObject(this, "Select Changer");

                        foreach (var changer in activeAuto.changers)
                        {
                            changer.SetDeactive();
                            if (changer.changerId == selectedChangerId) changer.SetActive();
                        }

                        break;
                    }

                case OnChangerEvent.EventType.EVENT_ORDERUP:
                    {
                        var activeChangerId = onChangerEvent.activeObjectId;
                        var changerIndex = changers.FindIndex(changer => changer.changerId == activeChangerId);
                        if (changerIndex <= 0) break;

                        Undo.RecordObject(this, "Change Changer Order");
                        var activeChanger = changers[changerIndex];

                        changers.RemoveAt(changerIndex);
                        changers.Insert(changerIndex - 1, activeChanger);

                        SaveData();
                        break;
                    }
                case OnChangerEvent.EventType.EVENT_ORDERDOWN:
                    {
                        var activeChangerId = onChangerEvent.activeObjectId;
                        var changerIndex = changers.FindIndex(changer => changer.changerId == activeChangerId);
                        if (changerIndex < 0) break;
                        if (changerIndex == changers.Count - 1) break;

                        Undo.RecordObject(this, "Change Changer Order");

                        var activeChanger = changers[changerIndex];

                        changers.RemoveAt(changerIndex);
                        changers.Insert(changerIndex + 1, activeChanger);

                        SaveData();
                        break;
                    }

                case OnChangerEvent.EventType.EVENT_UNDO:
                    {
                        var message = onChangerEvent.message;
                        Undo.RecordObject(this, message);

                        break;
                    }

                case OnChangerEvent.EventType.EVENT_SAVE:
                    {
                        SaveData();
                        break;
                    }

                case OnChangerEvent.EventType.EVENT_ADD:
                    {
                        AddChanger(ModifyingAuto());
                        break;
                    }

                case OnChangerEvent.EventType.EVENT_DELETE:
                    {
                        var changerId = onChangerEvent.activeObjectId;
                        DeleteChanger(ModifyingAuto(), changerId);
                        break;
                    }

                case OnChangerEvent.EventType.EVENT_CHANGE_AUTO:
                    {
                        var nextAutoId = onChangerEvent.message;
                        Debug.Log("nextAutoIdを指定して飛びたい:" + nextAutoId);
                        // SetAutoToActive(nextAutoId);
                        break;
                    }

                case OnChangerEvent.EventType.EVENT_REFRESHCONDITIONS:
                    {
                        var changerPlateId = onChangerEvent.activeObjectId;
                        var changerPlate = activeAuto.changers.Where(changer => changer.changerId == changerPlateId).FirstOrDefault();

                        // update changerPlate's conditions to latest one.
                        changerPlate.conditions = conditions;

                        break;
                    }

                default:
                    {
                        Debug.Log("unknown event:" + type);
                        break;
                    }
            }
        }


        private void ConditionsEmit(OnConditionsEvent onConditionsEvent)
        {
            var window = onConditionsEvent.window;
            // var currentConditionsData = window.conditions;

            var type = onConditionsEvent.eventType;
            switch (type)
            {
                case OnConditionsEvent.EventType.EVENT_ADDTYPE:
                    {
                        var newConditonTypeName = onConditionsEvent.typeName;

                        Undo.RecordObject(this, "Add New Condition Type");
                        conditions.Add(new ConditionData(newConditonTypeName, new List<string>()));
                        window.conditions = conditions;
                        SaveData();
                        break;
                    }

                case OnConditionsEvent.EventType.EVENT_ADDVALUE:
                    {
                        var newConditonTypeName = onConditionsEvent.typeName;
                        var newConditonValueName = onConditionsEvent.valueName;

                        var addingValueSTypeIndex = conditions.FindIndex(conditionData => conditionData.conditionType == newConditonTypeName);
                        if (addingValueSTypeIndex < 0) break;

                        Undo.RecordObject(this, "Add New Condition Value");

                        conditions[addingValueSTypeIndex].conditionValues.Add(newConditonValueName);
                        window.conditions = conditions;
                        SaveData();
                        break;
                    }

                case OnConditionsEvent.EventType.EVENT_DELETETYPE:
                    {
                        conditionEventStacks.Add(onConditionsEvent.Copy());
                        break;
                    }


                case OnConditionsEvent.EventType.EVENT_DELETEVALUE:
                    {
                        conditionEventStacks.Add(onConditionsEvent.Copy());
                        break;
                    }

                default:
                    {
                        Debug.Log("unknown event:" + type);
                        break;
                    }
            }
        }

        public void SetAutoToActive(string autoId)
        {
            Undo.RecordObject(this, "Change Active Auto");

            foreach (var auto in autos)
            {
                auto.SetDeactive();
                if (auto.autoId == autoId)
                {
                    auto.SetActive();
                }
            }

            SaveData();
        }

        private bool HasSelectedAuto()
        {
            if (autos.Any())
            {
                foreach (var auto in autos)
                {
                    if (auto.IsActive()) return true;
                }
            }
            return false;
        }

        private AutoComponent ModifyingAuto()
        {
            foreach (var auto in autos)
            {
                if (auto.IsActive()) return auto;
            }

            if (autos.Any())
            {
                SetAutoToActive(autos[0].autoId);
                return autos[0];
            }

            AutomatineEmit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_AUTO_ADDAUTO, AutomatineGUISettings.DEFAULT_AUTO_NAME));

            return ModifyingAuto();
        }


        private bool EmitAutomatineEventAfterDraw(OnAutomatineEvent onAutomatineEvent)
        {
            var type = onAutomatineEvent.eventType;
            switch (type)
            {
                case OnAutomatineEvent.EventType.EVENT_SHOW_COROUTINEWINDOW:
                    {
                        var targetTackId = onAutomatineEvent.activeObjectId;
                        var targetTack = ModifyingAuto().TackById(targetTackId);

                        // contains at least 1 coroutine. set it for display.
                        coroutineWindow = new CoroutineWindow(targetTack.routineIds);

                        coroutineWindowRect = new Rect(Event.current.mousePosition.x + AutomatineGUISettings.COROUTINEWINDOW_DEFAULT_X, AutomatineGUISettings.COROUTINEWINDOW_DEFAULT_Y, AutomatineGUISettings.COROUTINEWINDOW_DEFAULT_WIDTH, this.position.height - (AutomatineGUISettings.COROUTINEWINDOW_DEFAULT_Y + AutomatineGUISettings.HEADER_HEIGHT + AutomatineGUISettings.BOTTOM_MARGIN));

                        showingCoroutineParentTackId = targetTackId;
                        return false;
                    }
                case OnAutomatineEvent.EventType.EVENT_SHOW_COROUTINEONFRAMEWINDOW:
                    {
                        var targetFrameCount = onAutomatineEvent.frame;

                        var tacks = ModifyingAuto().TackByFrame(targetFrameCount);
                        var coroutineIds = tacks.SelectMany(tack => tack.routineIds).ToList();

                        // contains at least 1 coroutine. set it for display.
                        coroutineWindow = new CoroutineWindow(coroutineIds);

                        coroutineWindowRect = new Rect(Event.current.mousePosition.x + AutomatineGUISettings.COROUTINEWINDOW_DEFAULT_X, AutomatineGUISettings.COROUTINEWINDOW_DEFAULT_Y, AutomatineGUISettings.COROUTINEWINDOW_DEFAULT_WIDTH, this.position.height - (AutomatineGUISettings.COROUTINEWINDOW_DEFAULT_Y + AutomatineGUISettings.HEADER_HEIGHT + AutomatineGUISettings.BOTTOM_MARGIN));

                        showingCoroutineParentTackId = coroutineIds[0];
                        return false;
                    }
                case OnAutomatineEvent.EventType.EVENT_TIMELINE_ADDTACK:
                    {
                        var targetTimelineId = onAutomatineEvent.activeObjectId;
                        var targetFramePos = onAutomatineEvent.frame;

                        var currentAuto = ModifyingAuto();

                        Undo.RecordObject(this, "Add Tack");
                        currentAuto.AddNewTackToTimeline(targetTimelineId, targetFramePos);
                        return true;
                    }
                case OnAutomatineEvent.EventType.EVENT_TACK_DELETED:
                    {
                        var targetTackId = onAutomatineEvent.activeObjectId;
                        var currentAuto = ModifyingAuto();

                        Undo.RecordObject(this, "Delete Tack");

                        currentAuto.DeleteObjectById(targetTackId);
                        return true;
                    }
                case OnAutomatineEvent.EventType.EVENT_TACK_REMOVECOROUTINE:
                    {
                        var targetTackId = onAutomatineEvent.activeObjectId;
                        var removingRoutineIndex = onAutomatineEvent.frame;// dirty... this is not frame. this is index of removing routine in tack.

                        var tack = ModifyingAuto().TackById(targetTackId);

                        if (removingRoutineIndex < tack.routineIds.Count)
                        {
                            Undo.RecordObject(this, "Remove Coroutine");
                            tack.routineIds.RemoveAt(removingRoutineIndex);
                        }
                        return true;
                    }
            }
            return false;
        }


        private void EmitConditionEventAfterDraw(OnConditionsEvent onConditionsEvent)
        {
            var window = onConditionsEvent.window;
            // var currentConditionsData = window.conditions;
            // var type = onConditionsEvent.eventType;

            switch (onConditionsEvent.eventType)
            {
                case OnConditionsEvent.EventType.EVENT_DELETETYPE:
                    {
                        var deletingConditonTypeName = onConditionsEvent.typeName;
                        var deletingIndex = conditions.FindIndex(conditionData => conditionData.conditionType == deletingConditonTypeName);

                        if (deletingIndex < 0) break;

                        Undo.RecordObject(this, "Delete Condition Type");
                        conditions.RemoveAt(deletingIndex);
                        window.conditions = conditions;

                        foreach (var auto in autos) auto.DeleteConditionType(deletingConditonTypeName);

                        SaveData();
                        break;
                    }
                case OnConditionsEvent.EventType.EVENT_DELETEVALUE:
                    {
                        var deletingConditonTypeName = onConditionsEvent.typeName;
                        var deletingConditonValueName = onConditionsEvent.valueName;

                        var deletingValueSTypeIndex = conditions.FindIndex(conditionData => conditionData.conditionType == deletingConditonTypeName);
                        if (deletingValueSTypeIndex < 0) break;

                        var deletingValueIndex = conditions[deletingValueSTypeIndex].conditionValues.FindIndex(conditionValue => conditionValue == deletingConditonValueName);
                        if (deletingValueIndex < 0) break;

                        Undo.RecordObject(this, "Delete Condition Value");
                        conditions[deletingValueSTypeIndex].conditionValues.RemoveAt(deletingValueIndex);
                        window.conditions = conditions;

                        foreach (var auto in autos) auto.DeleteConditionValue(deletingConditonValueName);

                        SaveData();
                        break;
                    }
            }
        }

        private void AddChanger(AutoComponent activeAuto)
        {
            Undo.RecordObject(this, "Add New Changer");
            activeAuto.changers.Add(new ChangerPlate(activeAuto.autoId));
            SaveData();
        }

        private void DeleteChanger(AutoComponent activeAuto, string activeChangerId)
        {
            Undo.RecordObject(this, "Delete Changer");
            var changerIndex = activeAuto.changers.FindIndex(changer => changer.changerId == activeChangerId);
            if (changerIndex < 0) return;

            activeAuto.changers.RemoveAt(changerIndex);
            SaveData();
        }

        private void FocusToFrame(int focusTargetFrame)
        {
            var leftFrame = (int)Math.Round(scrollPosX / AutomatineGUISettings.TACK_FRAME_WIDTH);
            var rightFrame = (int)(((scrollPosX + (position.width - AutomatineGUISettings.TIMELINE_CONDITIONBOX_SPAN)) / AutomatineGUISettings.TACK_FRAME_WIDTH) - 1);

            // left edge of view - leftFrame - rightFrame - right edge of view

            if (focusTargetFrame < leftFrame)
            {
                scrollPosX = scrollPosX - ((leftFrame - focusTargetFrame) * AutomatineGUISettings.TACK_FRAME_WIDTH);
                return;
            }

            if (rightFrame < focusTargetFrame)
            {
                scrollPosX = scrollPosX + ((focusTargetFrame - rightFrame) * AutomatineGUISettings.TACK_FRAME_WIDTH);
                return;
            }
        }



        public static bool IsTimelineId(string activeObjectId)
        {
            if (activeObjectId.StartsWith(AutomatineGUISettings.ID_HEADER_TIMELINE)) return true;
            return false;
        }

        public static bool IsTackId(string activeObjectId)
        {
            if (activeObjectId.StartsWith(AutomatineGUISettings.ID_HEADER_TACK)) return true;
            return false;
        }

    }

}
