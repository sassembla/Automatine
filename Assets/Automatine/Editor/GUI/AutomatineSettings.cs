namespace Automatine
{
    public class AutomatineSettings
    {
        public const string AUTOMATINE_ASSET_NAME = "Automatine/";

        public const string AUTOMATINE_RUNTIME_PATH = "Runtime/";

        public const string AUTOMATINE_CODE_GENERATION_DEST_PATH = "Generated/";

        public const string AUTOMATINE_CODE_GENERATION_DEFINITIONS_PATH = "Definitions/";
        public const string AUTOMATINE_CODE_GENERATION_AUTOS_PATH = "Autos/";
        public const string AUTOMATINE_CODE_GENERATION_COROUTINE_PATH = "Coroutines/";

        public const string AUTOMATINE_DATA_GENERATION_PATH = "Json/";
        public const string RUNTIME_DATA_EXTENSION = ".json";

        public const string AUTOMATINE_EDITOR_PATH = "Editor/";
        public const string AUTOMATINE_DATA_PATH = "Data/";
        public const string AUTOMATINE_DATA_FILENAME = "automatine.json";

        public const string AUTOMATINE_DATA_LASTMODIFIED = "lastModified";
        public const string AUTOMATINE_DATA_AUTOS = "autos";
        public const string AUTOMATINE_DATA_CHANGERS = "changers";

        // auto, timeline, tack, routine & changer keys are defined at Assets/Automatine/Editor/Descriptor/AutomatineDescriptor.cs

        public const string UNITY_METAFILE_EXTENSION = ".meta";
        public const string DOTSTART_HIDDEN_FILE_HEADSTRING = ".";
        public const bool IGNORE_META = true;

        public const char UNITY_FOLDER_SEPARATOR = '/';
    }
}