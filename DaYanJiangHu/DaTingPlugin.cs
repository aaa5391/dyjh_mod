using System;

using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Globalization;
using System.Text.RegularExpressions;

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
        private bool training_consume = false;
        private static bool biguan_flag = false;
        private static bool tili_flag = false;

        private static int x_resolution = 1920;
        private static int y_resolution = 1080;

        private static List<Item> items = new List<Item>();
        private static List<string> item_names = new List<string>();
        private static string default_item = "";
        private static ConfigEntry<string> config_item;

        private string default_sex = "女";
        private bool supper_roles_flag = true;

        //private Vector2 scrollPosition;
        // 启动按键
        private ConfigEntry<KeyboardShortcut> ShowCounter { get; set; }

        [Obsolete]
        void Start()
        {
            Harmony.CreateAndPatchAll(typeof(Plugin));
            // 允许用户自定义启动快捷键
            ShowCounter = Config.AddSetting("打开窗口快捷键", "Key", new BepInEx.Configuration.KeyboardShortcut(KeyCode.F9));
            //save_items_to_local();
            Logger.LogDebug("测试MOD");
        }

        void Update()
        {
            //modify_resolution();
            //modify_dating_people();
            //modify_warehouse_capacity();
            //modify_money();


            // 监听脚本按键按下
            if (ShowCounter.Value.IsDown())
            {
                DisplayingWindow = !DisplayingWindow;
                if (DisplayingWindow)
                {
                    Debug.Log("打开窗口");
                }
                else
                {
                    Debug.Log("关闭窗口");
                }
            }
        }

        // GUI函数
        private void OnGUI()
        {
            if (this.DisplayingWindow)
            {
                // 定义窗口位置 x y 宽 高
                Rect windowRect = new Rect(500, 200, 500, 300);
                // 创建一个新窗口
                // 注意：第一个参数(20210218)为窗口ID，ID尽量设置的与众不同，若与其他Mod的窗口ID相同，将会导致窗口冲突
                windowRect = GUI.Window(20210530, windowRect, DoMyWindow, "辣鸡游戏MOD");
            }
        }

        public void DoMyWindow(int winId)
        {
            GUILayout.BeginArea(new Rect(10, 20, 490, 250));
            // 这里的大括号是可选的，我个人为了代码的阅读性,习惯性的进行了添加
            // 建议大家也使用大括号这样包裹起来，让代码看起来不那么的乱
            {
                GUIStyle titleStyle = new GUIStyle();
                titleStyle.fontSize = 24;
                GUILayout.Label("护肝神器！", titleStyle);
                GUILayout.BeginHorizontal();
                reset_resolution();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                //人数
                role_max_up();
                //仓库
                warehouse_up();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                //金钱
                add_money();
                //粮食
                add_food();
                //年龄-10
                reducer_age();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                //培养消耗开关
                if (GUILayout.Button("培养不耗贡献"))
                {
                }
                //迅速培养开关
                if (GUILayout.Button("培养下月完成"))
                {
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                //闭关无需等待
                no_need_wait_biguan();
                //体力消耗开关
                no_consume_strength();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                //添加弟子
                add_roles_button(default_sex);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                //添加物品到背包
                //save_items_to_local();
                if (items == null || items.Count == 0)
                {
                    add_item();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
            //定义窗体可以活动的范围
            GUI.DragWindow(new Rect(0, 0, UnityEngine.Screen.width, UnityEngine.Screen.height));
        }

        private void save_items_to_local()
        {
            items = ItemsManager.instance.getitemList();
            if (items != null)
            {
                foreach (Item item in items)
                {
                    Debug.LogFormat("物品ID: {0}, 物品名称: {1}, 物品等级: {2}", item.ID, item.Name, item.quality);
                    item_names.Add(item.Name);
                    //OpenUi._instance.Warehouse.GetComponent<Inventory>().StoreItem(item);
                }
                update_item_config(item_names);
            }
        }

        private void update_item_config(List<string> item_names)
        {
            if (item_names.Count == 0)
            {
                item_names.Add("");
            }
            config_item = Config.AddSetting("物品选择", "物品名称", default_item, new ConfigDescription("物品选择1", null, new AcceptableValueList<string>(item_names.ToArray())));
        }

        public void save_game()
        {
            OpenUi._instance.systemobjet.GetComponent<SaveManager>().savePanel.transform.GetChild(1).GetChild(0).GetComponent<SaveGame>().SartSaveGames();
        }

        public void reset_resolution()
        {
            x_resolution = Convert.ToInt32(GUILayout.TextField(Regex.Replace(Convert.ToString(x_resolution), @"[^\d]", ""), GUILayout.Width(60f)));
            y_resolution = Convert.ToInt32(GUILayout.TextField(Regex.Replace(Convert.ToString(y_resolution), @"[^\d]", ""), GUILayout.Width(60f)));
            if (GUILayout.Button("窗口"))
            {
                x_resolution = x_resolution > 2560 ? 2560 : x_resolution;
                x_resolution = x_resolution < 1024 ? 1024 : x_resolution;
                y_resolution = y_resolution > 1440 ? 1440 : y_resolution;
                y_resolution = y_resolution < 768 ? 768 : y_resolution;
                UnityEngine.Screen.SetResolution(x_resolution, y_resolution, false);
            }
            if (GUILayout.Button("全屏"))
            {
                Debug.LogFormat("width: {0}, height: {1}", UnityEngine.Screen.width, UnityEngine.Screen.height);
                UnityEngine.Screen.SetResolution(y_resolution, y_resolution, true);
            }
        }

        public void role_max_up()
        {
            if (GUILayout.Button("人数升级"))
            {
                OpenUi._instance.Gamedatas.PeopleLimit_dt += 8;
            }
        }

        public void warehouse_up()
        {
            if (GUILayout.Button("仓库升级"))
            {
                if (OpenUi._instance.Gamedatas.CangkuLev < 10)
                {
                    OpenUi._instance.Gamedatas.CangkuLev++;
                    OpenUi._instance.Gamedatas.CangkuLimit += 260;
                }
            }
        }

        public void add_money()
        {
            if (GUILayout.Button("金钱+1000"))
            {
                OpenUi._instance.Gamedatas.res_money += 1000;
            }
        }

        public void add_food()
        {
            if (GUILayout.Button("粮食+1000"))
            {
                OpenUi._instance.Gamedatas.res_food += 1000;
            }
        }

        public void reducer_age()
        {
            if (GUILayout.Button("返老还童"))
            {
                if (OpenUi._instance.Gamedatas.MasterRoles.Old > 20)
                {
                    OpenUi._instance.Gamedatas.MasterRoles.Old -= 10;
                }
            }
        }

        public void no_need_wait_biguan()
        {
            //连续闭关开关
            if (GUILayout.Button("闭关无需等待"))
            {
                if (biguan_flag == true)
                {
                    biguan_flag = false;
                }
                else
                {
                    biguan_flag = true;
                }
            }
            GUIStyle textStyle = new GUIStyle();
            if (biguan_flag == true)
            {
                textStyle.normal.textColor = new Color(152f, 195f, 121f);
                GUILayout.Label("已启用", textStyle);
            }
            else
            {
                textStyle.normal.textColor = new Color(210f, 71f, 16f);
                GUILayout.Label("已关闭", textStyle);
            }
        }

        public void no_consume_strength()
        {
            if (GUILayout.Button("探索不消耗体力"))
            {
                if (tili_flag == true)
                {
                    tili_flag = false;
                }
                else
                {
                    tili_flag = true;
                }
            }
            GUIStyle tili_textStyle = new GUIStyle();
            if (tili_flag == true)
            {
                tili_textStyle.normal.textColor = new Color(152f, 195f, 121f);
                GUILayout.Label("已启用", tili_textStyle);
                GUILayout.FlexibleSpace();
            }
            else
            {
                tili_textStyle.normal.textColor = new Color(210f, 71f, 16f);
                GUILayout.Label("已关闭", tili_textStyle);
                GUILayout.FlexibleSpace();
            }
        }

        //添加弟子
        public void add_roles_button(string sex)
        {
            if (GUILayout.Button("喜提妹子"))
            {
                add_roles(sex);
            }
            if (GUILayout.Button("天降猛男"))
            {
                add_roles(sex);
            }
            if (GUILayout.Button("雷劫洗礼"))
            {
                if (this.supper_roles_flag)
                {
                    this.supper_roles_flag = false;
                }
                else
                {
                    this.supper_roles_flag = true;
                }
            }
            GUILayout.Label(this.supper_roles_flag ? "已开启" : "已关闭");
            //if (GUILayout.Button("虚空取物"))
            //{

            //}

        }

        public void add_roles(string sex)
        {
            Roles roles = RoleManager.instance.GetRoles(OpenUi._instance.Gamedatas.PlayerFaction, OpenUi._instance.Gamedatas.res_fame, 1);
            if (roles != null && sex != null)
            {
                Debug.LogFormat("性别: {0}", roles.Sex);
                if (sex == null || roles.Sex.Equals(sex))
                {
                    if (supper_roles_flag)
                    {
                        //力量
                        roles.Potential_Power = Roles.Potential.无双;
                        //内力
                        roles.Potential_Inter = Roles.Potential.无双;
                        //体魄
                        roles.Potential_Body = Roles.Potential.无双;
                        //洞察
                        roles.Potential_Insight = Roles.Potential.无双;
                        //灵敏
                        roles.Potential_Agile = Roles.Potential.无双;
                        //韧性
                        roles.Potential_Toughness = Roles.Potential.无双;
                        //悟性
                        roles.Potential_Under = Roles.Potential.无双;
                        //刀
                        roles.Potential_knife = Roles.Potential.无双;
                        //剑
                        roles.Potential_Sword = Roles.Potential.无双;
                    }
                    OpenUi._instance.Gamedatas.Roles_player.Add(roles);
                }
                else
                {
                    add_roles(sex);
                }
            }
        }

        public void add_item()
        {

            if (default_item == null || default_item != config_item.Value)
            {
                Debug.Log("下拉列表2发生了变化,新的值为：" + config_item.Value);
                default_item = config_item.Value;
                List<Item> selectedItem = new List<Item>();
                foreach (Item item in items)
                {
                    if (item.Name.Equals(config_item.Value))
                    {
                        selectedItem.Add(item);
                    }
                }
                int i = UnityEngine.Random.Range(0, selectedItem.Count);
                OpenUi._instance.Warehouse.GetComponent<Inventory>().StoreItem(selectedItem[i]);

            }

        }

        //授业不消耗贡献，时间1天
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TrainManager), "ClickstartTrain_sy")]
        public static void TrainManager_ClickstartTrain_yj_Prefix(ref TrainManager __instance)
        {
            __instance.Roles.ContributionNum += (float)Traverse.Create(__instance).Field("sy").GetValue();
        }
        [HarmonyPrefix, HarmonyPatch(typeof(TrainManager), "startTrain_sy")]
        public static bool TrainManager_startTrain_sy_Prefix(ref TrainManager __instance)
        {
            string item = OpenUi._instance.NextHomeMesMessage(OpenUi._instance.Gamedatas.MasterRoles, "开始对弟子<color=#6486FF>" + __instance.Roles.Name + "</color>,单独进行<color=#D16EFF>授业解惑</color>。");
            OpenUi._instance.HomeMessage.Add(item);
            __instance.Roles.ContributionNum -= (float)Traverse.Create(__instance).Field("sy").GetValue();
            __instance.Roles.TrainTime = 1;
            __instance.Roles.rolestate = Roles.RoleState.授业解惑;
            OpenUi._instance.Gamedatas.MasterRoles.rolestate = Roles.RoleState.培养;
            OpenUi._instance.Gamedatas.MasterRoles.TrainTime = 1;
            OpenUi._instance.systemobjet.GetComponent<SaveManager>().ShowRemind("掌门开始单独对该弟子进行解惑");
            __instance.Roles = null;
            __instance.gongfaPanel.SetActive(false);
            __instance.Options.text = "武技";
            Traverse.Create(__instance).Method("ClearGongfa");
            return false;
        }

        //传授不消耗贡献，时间1天
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TrainManager), "OpenGongfa")]
        public static void TrainManager_OpenGongfa_Prefix(ref TrainManager __instance)
        {
            __instance.Roles.ContributionNum += (float)Traverse.Create(__instance).Field("cs").GetValue();
        }
        [HarmonyPrefix, HarmonyPatch(typeof(HeChengSlot), "startTrain_cs")]
        public static bool HeChengSlot_startTrain_cs_Prefix(ref HeChengSlot __instance)
        {
            OpenUi._instance.TrainPanel.GetComponent<TrainManager>().OpenGongfa();
            Roles roles = OpenUi._instance.TrainPanel.GetComponent<TrainManager>().Roles;
            roles.ContributionNum -= 100f;
            roles.TrainSkill = __instance.transform.GetChild(0).GetComponent<ItemsUi>().Item;
            roles.TrainTime = 1;
            roles.TrainProbability = (float)Traverse.Create(__instance).Field("Probability").GetValue();
            roles.rolestate = Roles.RoleState.传授功法;
            OpenUi._instance.Gamedatas.MasterRoles.rolestate = Roles.RoleState.培养;
            OpenUi._instance.Gamedatas.MasterRoles.TrainTime = 1;
            OpenUi._instance.systemobjet.GetComponent<SaveManager>().ShowRemind("掌门开始单独对该弟子进行传授");
            OpenUi._instance.TrainPanel.GetComponent<TrainManager>().Roles = null;
            string item = OpenUi._instance.NextHomeMesMessage(OpenUi._instance.Gamedatas.MasterRoles, "开始对弟子<color=#6486FF>" + roles.Name + "</color>,单独进行<color=#D16EFF>功法传授</color>。");
            OpenUi._instance.HomeMessage.Add(item);
            return false;
        }

        //易经不消耗贡献，时间1天
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TrainManager), "ClickstartTrain_yj")]
        public static void TrainManager_Modify_ContributionNum_Prefix(ref TrainManager __instance)
        {
            __instance.Roles.ContributionNum += (float)Traverse.Create(__instance).Field("yj").GetValue();
        }

        [HarmonyPrefix, HarmonyPatch(typeof(TrainManager), "startTrain_yj")]
        public static bool TrainManager_startTrain_yj_Prefix(ref TrainManager __instance)
        {
            string item = OpenUi._instance.NextHomeMesMessage(OpenUi._instance.Gamedatas.MasterRoles, "开始对弟子<color=#6486FF>" + __instance.Roles.Name + "</color>,进行<color=#D16EFF>易经洗髓</color>。");
            OpenUi._instance.HomeMessage.Add(item);
            __instance.Roles.ContributionNum -= (float)Traverse.Create(__instance).Field("yj").GetValue();
            __instance.Roles.TrainTime = 1;
            __instance.Roles.rolestate = Roles.RoleState.易经洗髓;
            OpenUi._instance.Gamedatas.MasterRoles.rolestate = Roles.RoleState.培养;
            OpenUi._instance.Gamedatas.MasterRoles.TrainTime = 1;
            OpenUi._instance.systemobjet.GetComponent<SaveManager>().ShowRemind("掌门开始对该弟子进行易经洗髓");
            __instance.Roles = null;
            __instance.gongfaPanel.SetActive(false);
            __instance.Options.text = "武技";
            Traverse.Create(__instance).Method("ClearGongfa");
            return false;
        }

        //灌顶不消耗贡献，时间1天
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TrainManager), "ClickstartTrain_gd")]
        public static void TrainManager_ClickstartTrain_gd_Prefix(ref TrainManager __instance)
        {
            __instance.Roles.ContributionNum += (float)Traverse.Create(__instance).Field("gd").GetValue();
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(TrainManager), "startTrain_gd")]
        public static IEnumerable<CodeInstruction> TrainManager_startTrain_gd_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();
            foreach (CodeInstruction code in codes)
            {
                if (code.opcode == OpCodes.Ldc_I4_3 && code.operand != null)
                {
                    yield return new CodeInstruction(OpCodes.Ldc_I4_1);
                }
                yield return code;
            }
            //codes[26].opcode = OpCodes.Ldc_I4_1;
            //codes[40].opcode = OpCodes.Ldc_I4_1;
            //return codes.AsEnumerable();
        }

        //[HarmonyPrefix, HarmonyPatch(typeof(TrainManager), "startTrain_gd")]
        //public static bool TrainManager_startTrain_gd_Prefix(ref TrainManager __instance)
        //{
        //    string item = OpenUi._instance.NextHomeMesMessage(OpenUi._instance.Gamedatas.MasterRoles, "开始对弟子<color=#6486FF>" + __instance.Roles.Name + "</color>,进行<color=#D16EFF>醍醐灌顶</color>。");
        //    OpenUi._instance.HomeMessage.Add(item);
        //    __instance.Roles.ContributionNum -= (float)Traverse.Create(__instance).Field("gd").GetValue();
        //    __instance.Roles.TrainTime = 1;
        //    __instance.Roles.rolestate = Roles.RoleState.醍醐灌顶;
        //    OpenUi._instance.Gamedatas.MasterRoles.rolestate = Roles.RoleState.培养;
        //    OpenUi._instance.Gamedatas.MasterRoles.TrainTime = 1;
        //    OpenUi._instance.systemobjet.GetComponent<SaveManager>().ShowRemind("掌门开始对该弟子进行灌顶");
        //    __instance.Roles = null;
        //    __instance.gongfaPanel.SetActive(false);
        //    __instance.Options.text = "武技";
        //    Traverse.Create(__instance).Method("ClearGongfa");
        //    return false;
        //}


        [HarmonyPrefix]
        [HarmonyPatch(typeof(NewGame_choose), "GetMasterRole")]
        public static bool NewGame_choose_MasterRole_Prefix(ref NewGame_choose __instance)
        {
            //Traverse.Create(__instance).Method("SetTalent", __instance.MasterTalent_0, Roles.MasterTalent.运交华盖);
            //Traverse.Create(__instance).Method("SetTalent", __instance.MasterTalent_1, Roles.MasterTalent.剑仙临凡);
            //Traverse.Create(__instance).Method("SetTalent", __instance.MasterTalent_2, Roles.MasterTalent.刀祖降世);
            //Traverse.Create(__instance).Method("SetTalent", __instance.MasterTalent_3, Roles.MasterTalent.一闻千悟);
            //Traverse.Create(__instance).Method("SetTalent", __instance.MasterTalent_4, Roles.MasterTalent.融会贯通);
            //Traverse.Create(__instance).Method("SetTalent", __instance.MasterTalent_5, Roles.MasterTalent.师名远播);
            bool sex = (bool)Traverse.Create(__instance).Field("sex").GetValue();
            string Toufa = (string)Traverse.Create(__instance).Field("Toufa").GetValue();
            string Houfa = (string)Traverse.Create(__instance).Field("Houfa").GetValue();
            string Meimao = (string)Traverse.Create(__instance).Field("Meimao").GetValue();
            string Huzi = (string)Traverse.Create(__instance).Field("Huzi").GetValue();
            string Eyes = (string)Traverse.Create(__instance).Field("Eyes").GetValue();
            string Bizi = (string)Traverse.Create(__instance).Field("Bizi").GetValue();
            string Zui = (string)Traverse.Create(__instance).Field("Zui").GetValue();
            string Yifu = (string)Traverse.Create(__instance).Field("Yifu").GetValue();
            string Tezheng = (string)Traverse.Create(__instance).Field("Tezheng").GetValue();

            __instance.roles = RoleManager.instance.SetMasterRole(__instance.roleName.text, sex, Toufa, Houfa, Meimao, Huzi, Eyes,
               Bizi, Zui, Yifu, Tezheng);

            //特性1
            __instance.roles.MasterTalent_0 = Roles.MasterTalent.运交华盖;
            __instance.MasterTalent_0.sprite = (Resources.Load("Sprites/Roles/MaterTalent_good_5", typeof(Sprite)) as Sprite);
            //特性2
            __instance.roles.MasterTalent_1 = Roles.MasterTalent.剑仙临凡;
            __instance.MasterTalent_1.sprite = (Resources.Load("Sprites/Roles/MaterTalent_good_0", typeof(Sprite)) as Sprite);
            //特性3
            __instance.roles.MasterTalent_2 = Roles.MasterTalent.刀祖降世;
            __instance.MasterTalent_2.sprite = (Resources.Load("Sprites/Roles/MaterTalent_good_1", typeof(Sprite)) as Sprite);
            //特性4
            __instance.roles.MasterTalent_3 = Roles.MasterTalent.一闻千悟;
            __instance.MasterTalent_3.sprite = (Resources.Load("Sprites/Roles/MaterTalent_good_3", typeof(Sprite)) as Sprite);
            //特性5
            __instance.roles.MasterTalent_4 = Roles.MasterTalent.才智无双;
            __instance.MasterTalent_4.sprite = (Resources.Load("Sprites/Roles/MaterTalent_good_4", typeof(Sprite)) as Sprite);
            //特性6
            __instance.roles.MasterTalent_5 = Roles.MasterTalent.师名远播;
            __instance.MasterTalent_5.sprite = (Resources.Load("Sprites/Roles/MaterTalent_good_2", typeof(Sprite)) as Sprite);

            //力量
            __instance.roles.Potential_Power = Roles.Potential.无双;
            __instance.Potential_liliang.sprite = (Resources.Load("Sprites/Roles/potential_5", typeof(Sprite)) as Sprite);
            //内力
            __instance.roles.Potential_Inter = Roles.Potential.无双;
            __instance.Potential_neili.sprite = (Resources.Load("Sprites/Roles/potential_5", typeof(Sprite)) as Sprite);
            //体魄
            __instance.roles.Potential_Body = Roles.Potential.无双;
            __instance.Potential_tipo.sprite = (Resources.Load("Sprites/Roles/potential_5", typeof(Sprite)) as Sprite);
            //洞察
            __instance.roles.Potential_Insight = Roles.Potential.无双;
            __instance.Potential_dongcha.sprite = (Resources.Load("Sprites/Roles/potential_5", typeof(Sprite)) as Sprite);
            //灵敏
            __instance.roles.Potential_Agile = Roles.Potential.无双;
            __instance.Potential_lingmin.sprite = (Resources.Load("Sprites/Roles/potential_5", typeof(Sprite)) as Sprite);
            //韧性
            __instance.roles.Potential_Toughness = Roles.Potential.无双;
            __instance.Potential_renxing.sprite = (Resources.Load("Sprites/Roles/potential_5", typeof(Sprite)) as Sprite);
            //悟性
            __instance.roles.Potential_Under = Roles.Potential.无双;
            __instance.Potential_wuxing.sprite = (Resources.Load("Sprites/Roles/potential_5", typeof(Sprite)) as Sprite);
            //刀
            __instance.roles.Potential_knife = Roles.Potential.无双;
            __instance.Potential_dao.sprite = (Resources.Load("Sprites/Roles/potential_5", typeof(Sprite)) as Sprite);
            //剑
            __instance.roles.Potential_Sword = Roles.Potential.无双;
            __instance.Potential_jian.sprite = (Resources.Load("Sprites/Roles/potential_5", typeof(Sprite)) as Sprite);

            __instance.liliang.text = ((int)__instance.roles.Power).ToString();
            Debug.LogFormat("力量： {0}", (__instance.roles.Power).ToString());
            __instance.neili.text = ((int)__instance.roles.InternalForce).ToString();
            __instance.tipo.text = ((int)__instance.roles.Body).ToString();
            __instance.dongcha.text = ((int)__instance.roles.Insight).ToString();
            __instance.lingmin.text = ((int)__instance.roles.Agile).ToString();
            __instance.renxing.text = ((int)__instance.roles.Toughness).ToString();
            __instance.wuxing.text = ((int)__instance.roles.Understanding).ToString();
            __instance.dao.text = ((int)__instance.roles.Knifeplay).ToString();
            __instance.jian.text = ((int)__instance.roles.Swordplay).ToString();

            return false;

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Openreward), "PageDown")]
        public static bool Openreward_PageDown_prefix(ref Openreward __instance)
        {
            SoundEffectManager._instance.ClickButton();
            Debug.Log(__instance.listObject);
            //__instance.listObject.transform.childCount = 12
            if (__instance.p == OpenUi._instance.Gamedatas.CangkuLev * 2)
            {
                OpenUi._instance.systemobjet.GetComponent<SaveManager>().ShowRemind("没有空间了");
                return false;
            }
            int p = __instance.p;
            __instance.p = p + 1;

            if (__instance.p > __instance.listObject.transform.childCount)
            {
                p = __instance.p;
                __instance.p = p - 1;
            }
            Debug.LogFormat("页数: {0}", p);
            Debug.LogFormat("__instance.listObject.transform.childCount: {0}", __instance.listObject.transform.childCount);
            __instance.PageText.text = __instance.p.ToString();
            __instance.listObject.transform.GetChild(__instance.p - 2).gameObject.SetActive(false);
            __instance.listObject.transform.GetChild(__instance.p - 1).gameObject.SetActive(true);
            return false;
        }


        //连续闭关
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(RoleDataOperations), "MasterRolesPractice")]
        public static IEnumerable<CodeInstruction> RoleDataOperations_MasterRolesPractice_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            if (biguan_flag == true)
            {
                codes[485].opcode = OpCodes.Ldc_I4_0;
            }
            return codes.AsEnumerable();
        }

        //[HarmonyTranspiler]
        //[HarmonyPatch(typeof(NEXT), "StarHouShan_ts")]
        //public static IEnumerable<CodeInstruction> EventTalkManager_StarHouShan_ts_Transpiler(IEnumerable<CodeInstruction> instructions)
        //{
        //    var codes = instructions.ToList();
        //    Debug.Log(codes[17].operand);
        //    Debug.Log("后山探索:体力");
        //    Debug.Log(codes[20].operand);
        //    Debug.Log(codes[23].operand);
        //    if (tili_flag == true)
        //    {
        //        codes[18].operand = 0;
        //        codes[19].operand = 0;
        //    }
        //    return codes.AsEnumerable();
        //}

        [HarmonyPrefix]
        [HarmonyPatch(typeof(NEXT), "StarHouShan_ts")]
        public static void EventTalkManager_StarHouShan_ts_Prefix(ref NEXT __instance)
        {
            if (tili_flag == true)
            {
                for (int i = 0; i < OpenUi._instance.Roles_mu.Count; i++)
                {
                    if (OpenUi._instance.Roles_mu[i].Strength > 0f)
                    {
                        Debug.LogFormat("{0} 体力剩余: {1}", OpenUi._instance.Roles_mu[i].Name, OpenUi._instance.Roles_mu[i].Strength);
                        if (OpenUi._instance.Roles_mu[i].Strength <= 30)
                        {
                            OpenUi._instance.Roles_mu[i].Strength += 30f;
                            OpenUi._instance.Roles_mu[i].Strength_limit += 30f;
                        }
                    }
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(RolesManager), "ShowRoles", new Type[] { typeof(int) })]
        public static void RolesManager_ShowRoles_Postfix(ref RolesManager __instance)
        {
            Debug.Log("测试分页");
            //int p = (int)traverse.create(__instance).field("p").getvalue();
            //__instance.rolesobject.transform.getchild(p).getcomponent<rolelist>().getrolelist_choose(openui._instance.gamedatas.masterroles);
        }

        //功法
        [HarmonyTranspiler]
        //[HarmonyPatch(typeof(GetGongFa), "GongFaGet", new Type[] { typeof(string), typeof(Item.Quality), typeof(int), typeof(string), typeof(string), typeof(Item.ItemType) })]
        [HarmonyPatch(typeof(GetGongFa), "GongFaGet", new Type[] { })]
        public static IEnumerable<CodeInstruction> GetGongFa_GongFaGet_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();
            string tradeTime = DateTime.Now.ToString("yyyy-MM-dd HH：mm：ss", DateTimeFormatInfo.InvariantInfo);
            foreach (CodeInstruction code in codes)
            {
                Debug.LogFormat("时间: {0}, 数据: {1}", tradeTime, code.operand);
                //var methodInfo = AccessTools.Method(typeof(UnityEngine.Random), nameof(UnityEngine.Random.Range), new System.Type[] { typeof(int), typeof(int) });
                if (code.opcode == OpCodes.Ldc_I4 && code.operand != null && code.operand == (object)1800)
                {
                    Debug.LogFormat("code.operand == 1800 IL值: {0}", code.operand);
                    yield return new CodeInstruction(OpCodes.Ldc_R4, 0.001);
                }
                if (code.opcode == OpCodes.Ldc_I4 && code.operand != null && code.operand == (object)3000)
                {
                    Debug.LogFormat("code.operand == 1800 IL值: {0}", code.operand);
                    yield return new CodeInstruction(OpCodes.Ldc_R4, 0.002);
                }
                yield return code;
            }
        }

        //功法
        [HarmonyPrefix]
        //[HarmonyPatch(typeof(GetGongFa), "GongFaGet", new Type[] { typeof(string), typeof(Item.Quality), typeof(int), typeof(string), typeof(string), typeof(Item.ItemType) })]
        [HarmonyPatch(typeof(GetGongFa), "GongFaGet", new Type[] { })]
        public static void t(ref GetGongFa __instance, ref float __state)
        {
            __state = Get_z._instance.Atk;
            Get_z._instance.Atk = 300000;
        }

        //功法
        [HarmonyPostfix]
        //[HarmonyPatch(typeof(GetGongFa), "GongFaGet", new Type[] { typeof(string), typeof(Item.Quality), typeof(int), typeof(string), typeof(string), typeof(Item.ItemType) })]
        [HarmonyPatch(typeof(GetGongFa), "GongFaGet", new Type[] { })]
        public static void GetGongFa_GetInput_Transpiler(ref GetGongFa __instance, ref Item __result, ref float __state)
        {
            string tradeTime = DateTime.Now.ToString("yyyy-MM-dd HH：mm：ss", DateTimeFormatInfo.InvariantInfo);
            Debug.LogFormat("创建功法: 时间{0}, 名称: {1}, __atk: {2}", tradeTime, __result.Name, __state);
            Get_z._instance.Atk = __state;

        }
    }
}
