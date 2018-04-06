using UnityEngine;
using System.Collections.Generic;

namespace Automatine
{
    public class AutomatineGUISettings
    {
        public const float CONDITION_INSPECTOR_BOX_WIDTH = 80f;

        public const int SHOW_COROUTINE_WAIT = 500;

        public const float HEADER_HEIGHT = 30f;
        public const float HEADER_SPAN = 10f;
        public const float CONDITION_BUTTON_WIDTH = 100f;
        public const float EXPORT_BUTTON_WIDTH = 100f;

        public const float CONDITION_INSPECTOR_FRAMECOUNT_HEIGHT = 15f;
        public const float CONDITION_INSPECTOR_FRAMELINE_HEIGHT = 18f;

        public const float CONDITION_INSPECTOR_CONDITIONLINE_SPAN = 7f;
        public const float CONDITION_INSPECTOR_CONDITIONLINE_HEIGHT = 4f;

        public const float ROUTINE_HEIGHT = 32f;
        public const float TACK_5FRAME_WIDTH = 80f;
        public const float TACK_FRAME_WIDTH = TACK_5FRAME_WIDTH / 5;
        public const float TACK_FRAME_HEIGHT = 25f;
        public const float TACK_HEIGHT = TACK_FRAME_HEIGHT + ROUTINE_HEIGHT;
        public const float TACK_POINT_SIZE = 13f;

        public const float TIMELINE_HEADER_HEIGHT = 1f;
        public const float TIMELINE_HEIGHT = TACK_HEIGHT + TIMELINE_HEADER_HEIGHT;
        public const float TIMELINE_CONDITIONBOX_WIDTH = CONDITION_INSPECTOR_BOX_WIDTH;
        public const float TIMELINE_CONDITIONBOX_SPAN = TIMELINE_CONDITIONBOX_WIDTH + 5f;

        public const float TIMELINE_EMPTY_HEIGHT = 60f;


        public const int CHANGER_SPACE_HEIGHT_MAX = 100;
        public const int CHANGER_SPACE_HEIGHT_MIN = 30;

        public const float CHANGER_HEADER_HEIGHT = 30f;

        public const float CHANGER_PLATE_HEIGHT = 40f;

        public const float CHANGER_TO_PLATE_NAME_WIDTH = 120f;

        public const float CHANGER_TO_PLATE_PARTS_HEIGHT = 18f;

        public const float CHANGER_TO_PLATE_LINEWIDTH = 1f;


        public const float CHANGER_TO_PLATE_HEIGHT = 40f;

        public const float CHANGER_TO_PLATE_ORDERBUTTON_SIZE = 16f;

        public const float TIMELINE_SPAN = 10f;

        public const float COROUTINEWINDOW_DEFAULT_X = -5f;
        public const float COROUTINEWINDOW_DEFAULT_Y = 25f;
        public const float BOTTOM_MARGIN = 20f;
        public const float COROUTINEWINDOW_DEFAULT_WIDTH = 500f;

        public const int WINDOW_CONDITIONINSPECTOR_ID = -3;
        public const int WINDOW_TICK_ID = -2;
        public const int WINDOW_COROUTINEWINDOW_ID = -1;


        public const string ID_HEADER_AUTO = "A_";
        public const string ID_HEADER_TIMELINE = "TL_";
        public const string ID_HEADER_TACK = "TA_";
        public const string ID_HEADER_CHANGER = "C_";

        public const int BEHAVE_FRAME_MOVE_RATIO = 4;


        // defaults
        public const string DEFAULT_AUTO_NAME = "New_Auto";
        public const string DEFAULT_AUTO_INFO = "generated by automatine.";

        public const string DEFAULT_TIMELINE_NAME = "New Timeline";
        public const string TIMELINE_CONDITION_EMPTY = "-";

        public const string DEFAULT_TACK_NAME = "New Tack";
        public const int DEFAULT_TACK_SPAN = 1;

        public const string DEFAULT_CHANGER_NAME = "New Changer";

        public const string APPLICATION_DLL_PATH = "Library/ScriptAssemblies/Assembly-CSharp.dll";

        public const string ROUTINE_CONTEXT_CLASS_NAME = "RoutineContexts";


        // styles.
        public static GUIStyle coroutineStyle;
        public static GUIStyle tickStyle;

        public const string RESOURCE_BASEPATH = "Assets/Automatine/Editor/GUI/Res/";

        public static List<Color> RESOURCE_COLORS_SOURCES = new List<Color> {
            new Color(0.000f, 0.318f, 0.604f, 1.000f),
            new Color(0.620f, 0.137f, 0.494f, 1.000f),
            new Color(0.929f, 0.043f, 0.443f, 1.000f),
            new Color(0.929f, 0.102f, 0.231f, 1.000f),
            new Color(0.953f, 0.424f, 0.133f, 1.000f),
            new Color(1.000f, 0.812f, 0.012f, 1.000f),
            new Color(0.843f, 0.867f, 0.137f, 1.000f),
            new Color(0.306f, 0.690f, 0.286f, 1.000f),
            new Color(0.051f, 0.592f, 0.286f, 1.000f),
            new Color(0.000f, 0.620f, 0.620f, 1.000f),
            new Color(0.004f, 0.631f, 0.773f, 1.000f),
            new Color(0.039f, 0.553f, 0.804f, 1.000f),
        };

        // public static List<Texture2D> RESOURCE_COLORS;
        public const string RESOURCE_TICK = RESOURCE_BASEPATH + "tick.png";
        public const string RESOURCE_CONDITIONLINE_BG = RESOURCE_BASEPATH + "conditionLineBg.png";

        public const string RESOURCE_TRACK_HEADER_BG = RESOURCE_BASEPATH + "headerBg.png";
        public const string RESOURCE_TRACK_CONDITION_BG = RESOURCE_BASEPATH + "bg.png";
        public const string RESOURCE_TRACK_FRAME_BG = RESOURCE_BASEPATH + "5frame.png";

        public const string RESOURCE_TACK_WHITEPOINT = RESOURCE_BASEPATH + "whitePoint.png";
        public const string RESOURCE_TACK_GRAYPOINT = RESOURCE_BASEPATH + "grayPoint.png";
        public const string RESOURCE_TACK_WHITEPOINT_SINGLE = RESOURCE_BASEPATH + "whitePointSingle.png";
        public const string RESOURCE_TACK_GRAYPOINT_SINGLE = RESOURCE_BASEPATH + "grayPointSingle.png";

        public const string RESOURCE_CHANGER_BG = RESOURCE_BASEPATH + "changer_bg.png";
        public const string RESOURCE_CHANGER_NAME = RESOURCE_BASEPATH + "changer_name.png";
        public const string RESOURCE_CHANGER_ITEM_BG = RESOURCE_BASEPATH + "changer_item_bg.png";
        public const string RESOURCE_CHANGER_BRANCH = RESOURCE_BASEPATH + "changer_branch.png";
        public const string RESOURCE_CHANGER_CONTINUE = RESOURCE_BASEPATH + "changer_continue.png";
        public const string RESOURCE_CHANGER_FINALLY = RESOURCE_BASEPATH + "changer_finally.png";
        public const string RESOURCE_TACK_INFINITY = RESOURCE_BASEPATH + "tack_infinity.png";

        public const string RESOURCE_ACTIVE_OBJECT_BASE = RESOURCE_BASEPATH + "activeTack.png";

        public const string RESOURCE_COROUTINE_BACKGROUND = RESOURCE_BASEPATH + "coroutineBackground.png";

        public const string RESOURCE_DUMMY_BG = RESOURCE_BASEPATH + "dummyBg.png";

        public static Texture2D tickTex;
        public static Texture2D conditionLineBgTex;
        public static Texture2D timelineHeaderTex;
        public static Texture2D frameTex;
        public static Texture2D whitePointTex;
        public static Texture2D grayPointTex;
        public static Texture2D whitePointSingleTex;
        public static Texture2D grayPointSingleTex;

        public static Texture2D changerBaseTex;
        public static Texture2D changerNameTex;
        public static Texture2D changerItemBaseTex;
        public static Texture2D changerBranchTex;
        public static Texture2D changerContinueTex;
        public static Texture2D changerFinallyTex;

        public static Texture2D tackInfinityTex;

        public static Texture2D activeObjectBaseTex;

        public static Texture2D coroutineBackgroundTex;

        public static Texture2D dummyTex;
    }
}