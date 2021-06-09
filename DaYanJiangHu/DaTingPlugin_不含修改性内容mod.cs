using System;

using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

namespace DaYanJiangHu
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInProcess(GAME_PROCESS)]//*
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "com.zhangsan.dygh.datingmod";
        public const string NAME = "DaTingMod";
        public const string VERSION = "1.0";
        private const string GAME_PROCESS = "EvolutionOfJiangHu.exe";//*

        // 窗口开关
        private bool DisplayingWindow = false;

        private static List<Item> items = new List<Item>();

        //private Vector2 scrollPosition;
        // 启动按键
        private ConfigEntry<KeyboardShortcut> ShowCounter { get; set; }


        [SerializeField]
        Transform UIPanel;
        [Obsolete]
        void Start()
        {
            Harmony.CreateAndPatchAll(typeof(Plugin));
            // 允许用户自定义启动快捷键
            //ShowCounter = Config.AddSetting("打开窗口快捷键", "Key", new BepInEx.Configuration.KeyboardShortcut(KeyCode.F9));
            //save_items_to_local();
            //update_item_config();
        }


        void Update()
        {
            if(Input.GetKeyDown(KeyCode.F9))
            {
                save_items_to_file();
            }
        }

        public void save_items_to_file()
        {
            string infoPath = "item_info.txt";
            items = ItemsManager.instance.getitemList();
            if (!File.Exists(infoPath))
            {
                FileStream fileStream = new FileStream(infoPath, FileMode.CreateNew);
                StreamWriter streamWriter = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
                streamWriter.WriteLine("ID 名称 级别 类型 值 功能1 功能2 功能3");
                if (items != null)
                {
                    foreach (Item item in items)
                    {
                        Debug.LogFormat("物品ID: {0}, 物品名称: {1}, 物品等级: {2}", item.ID, item.Name, item.quality);
                        streamWriter.WriteLine(Convert.ToString(item.ID) + " " + item.Name + " " + item.quality + " " + item.itemType + " " + item.Value + " " + item.Skills_0 + " " + item.Skills_1 + " " + item.Skills_2);
                    }
                }
                streamWriter.Close(); //此处有坑。。。。要注意安全
                fileStream.Close();
            }
        }

        public void reducer_age()
        {
            if (GUILayout.Button("返老还童"))
            {
                if (OpenUi._instance.Gamedatas.MasterRoles.Old > 20)
                {
                    OpenUi._instance.Gamedatas.MasterRoles.Old -= 10;
                    OpenUi._instance.systemobjet.GetComponent<SaveManager>().ShowRemind("穿越成功，您回到了10年前");
                }
            }
        }

        public bool check_is_created_org()
        {
            int years_0 = OpenUi._instance.Gamedatas.years_0;
            int years_1 = OpenUi._instance.Gamedatas.years_1;
            int years_2 = OpenUi._instance.Gamedatas.years_2;
            int month = OpenUi._instance.Gamedatas.month;
            if ((years_0 + years_1 + years_2) >= 1 && month >= 3)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GetGongFa), "OutInput", new Type[] { })]
        public static void GetGongFa_OutInput_Postfix(ref GetGongFa __instance)
        {
            string tradeTime = DateTime.Now.ToString("yyyy-MM-dd HH：mm：ss", DateTimeFormatInfo.InvariantInfo);
            Item item = Slot_z._instance.Item;
            List<Item.Quality> good_qualitys = new List<Item.Quality>();
            good_qualitys.Add(Item.Quality.精品);
            good_qualitys.Add(Item.Quality.绝品);
            good_qualitys.Add(Item.Quality.神品);
            good_qualitys.Add(Item.Quality.一流功夫);
            good_qualitys.Add(Item.Quality.高深武学);
            good_qualitys.Add(Item.Quality.盖世神功);
            if (good_qualitys.Contains(item.quality))
            {
                string text = "<color=#FFFFFF>";
                switch (item.quality)
                {
                    case Item.Quality.防身之术:
                        text = "<color=#D3D938>";
                        break;
                    case Item.Quality.寻常武艺:
                        text = "<color=#38D946>";
                        break;
                    case Item.Quality.一流功夫:
                        text = "<color=#63F0FF>";
                        break;
                    case Item.Quality.高深武学:
                        text = "<color=#FF4840>";
                        break;
                    case Item.Quality.盖世神功:
                        text = "<color=#FFB243>";
                        break;
                }
                OpenUi._instance.systemobjet.GetComponent<SaveManager>().ShowRemind("获得新的" + item.itemType.ToString() + "：" + text + item.quality.ToString() + "·" + item.Name + "</color>");
            }
        }

        //功法
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Tasks), "GetTasksRemuneration", new Type[] { })]
        public static void Tasks_GetTasksRemuneration_postfix(ref Tasks __instance)
        {
            __instance.state = Tasks.State.已完成;
            List<Roles> task_roles = __instance.Taskroles;
            List<string> roles_name = new List<string>();
            foreach (Roles role in task_roles)
            {
                roles_name.Add(role.Name);
            }

            int count = roles_name.Count;
            string text = "<color=#578bbb>";
            text += string.Join(", ", roles_name);
            if (count == 1)
            {
                text += "</color> 独自完成了 <color=#47ba8b>" + __instance.TaskName + "</color> 任务";
                OpenUi._instance.systemobjet.GetComponent<SaveManager>().ShowRemind(text);
            }
            if (count > 1)
            {
                text += "</color> 共同完成了 <color=#47ba8b>" + __instance.TaskName + "</color> 任务";
                OpenUi._instance.systemobjet.GetComponent<SaveManager>().ShowRemind(text);
            }
            //此处为自动完成任务的代码
            OpenUi._instance.dating.GetComponent<UI>().BG_renwu.GetComponent<RenwuManager>().GetTasksRemuneration();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Item), "GetToolTipText", new Type[] { })]
        public static bool Item_GetToolTipText_prefix(ref Item __instance, ref string __state, ref string __result)
        {
            __state = __instance.Name;
            __instance.Name = get_color_item_name(__instance.Name, __instance.quality);
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Item), "GetToolTipText", new Type[] { })]
        public static void Item_GetToolTipText_postfix(ref Item __instance, ref string __state)
        {
            __instance.Name = __state;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Gongfa), "GetToolTipText", new Type[] { })]
        public static bool Gongfa_GetToolTipText_prefix(ref Gongfa __instance, ref string __state, ref string __result)
        {
            __state = __instance.Name;
            __instance.Name = get_color_item_name(__instance.Name, __instance.quality);
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Gongfa), "GetToolTipText", new Type[] { })]
        public static void Gongfa_GetToolTipText_postfix(ref Gongfa __instance, ref string __state)
        {
            __instance.Name = __state;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(NeiGong), "GetToolTipText", new Type[] { })]
        public static bool NeiGong_GetToolTipText_prefix(ref NeiGong __instance, ref string __state, ref string __result)
        {
            __state = __instance.Name;
            __instance.Name = get_color_item_name(__instance.Name, __instance.quality);
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(NeiGong), "GetToolTipText", new Type[] { })]
        public static void NeiGong_GetToolTipText_postfix(ref NeiGong __instance, ref string __state)
        {
            __instance.Name = __state;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WaiGong), "GetToolTipText", new Type[] { })]
        public static bool WaiGong_GetToolTipText_prefix(ref WaiGong __instance, ref string __state, ref string __result)
        {
            __state = __instance.Name;
            __instance.Name = get_color_item_name(__instance.Name, __instance.quality);
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(WaiGong), "GetToolTipText", new Type[] { })]
        public static void ShenFa_GetToolTipText_postfix(ref WaiGong __instance, ref string __state)
        {
            __instance.Name = __state;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ShenFa), "GetToolTipText", new Type[] { })]
        public static bool ShenFa_GetToolTipText_prefix(ref ShenFa __instance, ref string __state, ref string __result)
        {
            __state = __instance.Name;
            __instance.Name = get_color_item_name(__instance.Name, __instance.quality);
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ShenFa), "GetToolTipText", new Type[] { })]
        public static void ShenFa_GetToolTipText_postfix(ref ShenFa __instance, ref string __state)
        {
            __instance.Name = __state;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Plant_material), "GetToolTipText", new Type[] { })]
        public static bool Plant_material_GetToolTipText_prefix(ref Plant_material __instance, ref string __state, ref string __result)
        {
            __state = __instance.Name;
            __instance.Name = get_color_item_name(__instance.Name, __instance.quality);
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Plant_material), "GetToolTipText", new Type[] { })]
        public static void Plant_material_GetToolTipText_postfix(ref Plant_material __instance, ref string __state)
        {
            __instance.Name = __state;
        }

        public static string get_color_item_name(string Name, Item.Quality quality) 
        {
            string text2 = "<color=#FFFFFF>";
            switch (quality)
            {
                case Item.Quality.下:
                    text2 = "<color=#D3D938>";
                    break;
                case Item.Quality.中:
                    text2 = "<color=#38D946>";
                    break;
                case Item.Quality.上:
                    text2 = "<color=#FF4840>";
                    break;
                case Item.Quality.极:
                    text2 = "<color=#FFB243>";
                    break;
                case Item.Quality.粗品:
                    text2 = "<color=#D3D938>";
                    break;
                case Item.Quality.凡品:
                    text2 = "<color=#38D946>";
                    break;
                case Item.Quality.精品:
                    text2 = "<color=#63F0FF>";
                    break;
                case Item.Quality.绝品:
                    text2 = "<color=#FF4840>";
                    break;
                case Item.Quality.神品:
                    text2 = "<color=#FFB243>";
                    break;
                case Item.Quality.防身之术:
                    text2 = "<color=#D3D938>";
                    break;
                case Item.Quality.寻常武艺:
                    text2 = "<color=#38D946>";
                    break;
                case Item.Quality.一流功夫:
                    text2 = "<color=#63F0FF>";
                    break;
                case Item.Quality.高深武学:
                    text2 = "<color=#FF4840>";
                    break;
                case Item.Quality.盖世神功:
                    text2 = "<color=#FFB243>";
                    break;
            }
            Name = text2 + Name + "</color>";
            return Name;
        }
    }
}
