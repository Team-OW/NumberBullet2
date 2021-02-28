namespace Treevel.Common.Utils
{
    public class Constants
    {
        /// <summary>
        /// PlayerPrefs で使うキー群
        /// </summary>
        public static class PlayerPrefsKeys
        {
            public const string TREE = "TREE";
            public const string BRANCH_STATE = "BranchState";
            public const string BGM_VOLUME = "BGM_VOLUME";
            public const string SE_VOLUME = "SE_VOLUME";
            public const string LANGUAGE = "LANGUAGE";
            public const string STAGE_DETAILS = "STAGE_DETAILS";
            public const string LEVEL_SELECT_CANVAS_SCALE = "LEVEL_SELECT_CANVAS_SCALE";
            public const string LEVEL_SELECT_SCROLL_POSITION = "LEVEL_SELECT_SCROLL_POSITION";
            public const char KEY_CONNECT_CHAR = '-';
            public const string FAILURE_REASONS_COUNT = "FAILURE_REASONS_COUNT";
            public const string STARTUP_DAYS = "STARTUP_DAYS";
            public const string LAST_STARTUP_DATE = "LAST_STARTUP_DATE";
            public const string DATABASE_LOGIN_ID = "DATABASE_LOGIN_ID";
        }

        /// <summary>
        /// アニメーションクリップの名前
        /// </summary>
        public static class AnimationClipName
        {
            public const string BOTTLE_GET_ATTACKED = "BottleGetAttacked";
            public const string BOTTLE_DEAD = "BottleDead";
        }

        /// <summary>
        /// タイルの名前
        /// </summary>
        public static class TileName
        {
            public const string NORMAL_TILE = "NormalTile";
            public const string GOAL_TILE = "GoalTile";
            public const string WARP_TILE = "WarpTile";
            public const string HOLY_TILE = "HolyTile";
            public const string SPIDERWEB_TILE = "SpiderwebTile";
            public const string ICE_TILE = "IceTile";
        }

        /// <summary>
        /// ボトルの名前
        /// </summary>
        public static class BottleName
        {
            public const string DYNAMIC_DUMMY_BOTTLE = "DynamicDummyBottle";
            public const string STATIC_DUMMY_BOTTLE = "StaticDummyBottle";
            public const string NORMAL_BOTTLE = "NormalBottle";
            public const string ATTACKABLE_DUMMY_BOTTLE = "AttackableDummyBottle";
            public const string ERASABLE_BOTTLE = "ErasableBottle";
        }

        /// <summary>
        /// ステージ情報
        /// </summary>
        public static class StageSize
        {
            /// <summary>
            /// ステージのタイル行数
            /// </summary>
            public const int ROW = 5;

            /// <summary>
            /// ステージのタイル列数
            /// </summary>
            public const int COLUMN = 3;

            /// <summary>
            /// タイルの合計数
            /// </summary>
            public const int TILE_NUM = ROW * COLUMN;
        }

        /// <summary>
        /// シーンの名前
        /// </summary>
        public static class SceneName
        {
            public const string MENU_SELECT_SCENE = "MenuSelectScene";
            public const string SPRING_STAGE_SELECT_SCENE = "SpringStageSelectScene";
            public const string SUMMER_STAGE_SELECT_SCENE = "SummerStageSelectScene";
            public const string AUTUMN_STAGE_SELECT_SCENE = "AutumnStageSelectScene";
            public const string WINTER_STAGE_SELECT_SCENE = "WinterStageSelectScene";
            public const string GAME_PLAY_SCENE = "GamePlayScene";
            public const string START_UP_SCENE = "StartUpScene";
        }

        /// <summary>
        /// Sorting Layer の名前
        /// </summary>
        public static class SortingLayerName
        {
            public const string TILE = "Tile";
            public const string METEORITE = "Meteorite";
            public const string BOTTLE = "Bottle";
            public const string GIMMICK = "Gimmick";
            public const string GIMMICK_WARNING = "GimmickWarning";
            public const string ROAD = "Road";
        }

        /// <summary>
        /// タグの名前
        /// </summary>
        public static class TagName
        {
            public const string TILE = "Tile";
            public const string NORMAL_BOTTLE = "NormalBottle";
            public const string DUMMY_BOTTLE = "DummyBottle";
            public const string GIMMICK = "Gimmick";
            public const string GIMMICK_WARNING = "GimmickWarning";
            public const string GRAPH_UI = "GraphUi";
            public const string TREE = "Tree";
            public const string ROAD = "Road";
            public const string STAGE = "Stage";
            public const string BRANCH = "Branch";
        }

        /// <summary>
        /// Addressable Asset System で使うアドレス、
        /// Addressables Groups Windowsの「Addressable Name」と一致する必要がある
        /// </summary>
        public static class Address
        {
            // ボトル関連
            public const string NORMAL_BOTTLE_PREFAB = "NormalBottlePrefab";
            public const string DYNAMIC_DUMMY_BOTTLE_PREFAB = "DynamicDummyBottlePrefab";
            public const string STATIC_DUMMY_BOTTLE_PREFAB = "StaticDummyBottlePrefab";
            public const string ERASABLE_BOTTLE_PREFAB = "ErasableBottlePrefab";
            public const string ATTACKABLE_DUMMY_BOTTLE_PREFAB = "AttackableDummyBottlePrefab";
            public const string DYNAMIC_DUMMY_BOTTLE_SPRITE = "dynamicDummyBottle";
            public const string STATIC_DUMMY_BOTTLE_SPRITE = "staticDummyBottle";
            public const string NORMAL_BOTTLE_SPRITE_PREFIX = "normalBottle_";

            // ボトル関連のエフェクト
            public const string SELFISH_EFFECT_PREFAB = "SelfishEffectPrefab";
            public const string LIFE_EFFECT_PREFAB = "LifeEffectPrefab";
            public const string DARK_EFFECT_PREFAB = "DarkEffectPrefab";
            public const string REVERSE_EFFECT_PREFAB = "ReverseEffectPrefab";

            // タイル関連
            public const string NORMAL_TILE_PREFAB = "NormalTilePrefab";
            public const string NORMAL_TILE_SPRITE_PREFIX = "normalTile_";
            public const string GOAL_TILE_PREFAB = "GoalTilePrefab";
            public const string WARP_TILE_PREFAB = "WarpTilePrefab";
            public const string HOLY_TILE_PREFAB = "HolyTilePrefab";
            public const string SPIDERWEB_TILE_PREFAB = "SpiderwebTilePrefab";
            public const string ICE_TILE_PREFAB = "IceTilePrefab";
            public const string GOAL_TILE_SPRITE_PREFIX = "numberTile_";

            // 銃弾関連
            public const string NORMAL_HOLE_GENERATOR_PREFAB = "NormalHoleGeneratorPrefab";
            public const string AIMING_HOLE_GENERATOR_PREFAB = "AimingHoleGeneratorPrefab";

            // ギミック
            public const string TORNADO_PREFAB = "TornadoPrefab";
            public const string TORNADO_WARNING_SPRITE = "TornadoWarning";
            public const string TURN_WARNING_LEFT_SPRITE = "turnLeft";
            public const string TURN_WARNING_RIGHT_SPRITE = "turnRight";
            public const string TURN_WARNING_UP_SPRITE = "turnUp";
            public const string TURN_WARNING_BOTTOM_SPRITE = "turnBottom";
            public const string METEORITE_PREFAB = "MeteoritePrefab";
            public const string AIMING_METEORITE_PREFAB = "AimingMeteoritePrefab";
            public const string THUNDER_PREFAB = "ThunderPrefab";
            public const string SOLAR_BEAM_PREFAB = "SolarBeamPrefab";
            public const string GUST_WIND_PREFAB = "GustWindPrefab";
            public const string FOG_PREFAB = "FogPrefab";
            public const string POWDER_PREFAB = "PowderPrefab";
            public const string SAND_PILED_UP_POWDER_PREFAB = "SandPiledUpPowderPrefab";
            public const string SAND_POWDER_BACKGROUND_SPRITE = "SandBackground";
            public const string SAND_POWDER_PARTICLE_MATERIAL = "SandParticle";
            public const string ERASABLE_PREFAB = "ErasablePrefab";
        }

        /// <summary>
        /// ゲーム画面のウィンドウサイズ
        /// </summary>
        public static class WindowSize
        {
            public const float WIDTH = 900;
            public const float HEIGHT = 1600;
        }

        /// <summary>
        /// タイルの大きさ
        /// </summary>
        public static class TileRatioToWindowWidth
        {
            public const float WIDTH_RATIO = 0.22f;
            public const float HEIGHT_RATIO = WIDTH_RATIO;
        }

        /// <summary>
        /// ボトルの大きさ
        /// </summary>
        public static class BottleRatioToWindowWidth
        {
            public const float WIDTH_RATIO = TileRatioToWindowWidth.WIDTH_RATIO * 0.95f;
            public const float HEIGHT_RATIO = WIDTH_RATIO;
        }

        /// <summary>
        /// 一季節が許容できる木の数
        /// </summary>
        public const int MAX_TREE_NUM_IN_SEASON = 1000;
    }
}
