using System;

using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Reflection;

namespace DaYanJiangHu
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInProcess(GAME_PROCESS)]//*
    public class DaYanJiangHu : BaseUnityPlugin
    {
        public const string GUID = "com.zhangsan.dygh.datingmod";
        public const string NAME = "DaTingMod";
        public const string VERSION = "1.0";
        private const string GAME_PROCESS = "EvolutionOfJiangHu.exe";//*

        // 窗口开关
        private bool DisplayingWindow = false;
        private bool training_consume = false;
        private static bool biguan_flag = true;
        private static bool tili_flag = true;
        private static bool shen_gong_flag = true;
        private static bool modPlayerFactionFlag = false;

        private static int x_resolution = 1920;
        private static int y_resolution = 1080;

        private static List<Item> items = new List<Item>();
        private static List<string> item_names = new List<string>();
        private static string default_item = "";
        private static ConfigEntry<string> config_item;
        private static List<Item> selectedItem = new List<Item>();
        // 定义窗口位置 x y 宽 高
        Rect windowRect = new Rect(500, 200, 500, 300);

        private string default_sex = "女";
        private bool supper_roles_flag = true;


        GameObject prefab_StarMapToolsBasePanel;//资源

        //private Vector2 scrollPosition;
        // 启动按键
        private ConfigEntry<KeyboardShortcut> ShowCounter { get; set; }

        private Sprite skill_04_;

        static public string[] showText = new string[] { };
        static public Sprite[] sprite;
        static Dropdown dropDownItem;
        static List<string> temoNames;
        static List<Sprite> sprite_list;
        static public GameObject panel;

        public Dropdown dropDown;

        [SerializeField]
        Transform UIPanel;
        [Obsolete]
        void Start()
        {
            Harmony.CreateAndPatchAll(typeof(DaYanJiangHu));
            // 允许用户自定义启动快捷键
            ShowCounter = Config.AddSetting("打开窗口快捷键", "Key", new BepInEx.Configuration.KeyboardShortcut(KeyCode.F9));
            //save_items_to_local();
            //update_item_config();
            Logger.LogDebug("测试MOD");
            UIPanel = this.GetComponent<Transform>();
            //TasksItemSlotList = base.GetComponentsInChildren<TasksItemSlot>();
            //UIPanel.gameObject.SetActive(false);
            //panel.SetActive(false);
            //showText = new string[] { "AAA", "BBB" };
            //dropDownItem = this.GetComponent<Dropdown>();
            //temoNames = new List<string>();
            //sprite_list = new List<Sprite>();
            //AddNames(showText);
            //UpdateDropDownItem(temoNames);
            //var ab = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("StarMapTools.starmaptools"));
            //prefab_StarMapToolsBasePanel = ab.LoadAsset<GameObject>("StarMapToolsBasePanel");
            //var ab = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("DaYanJiangHu.skill_04_"));
            //skill_04_ = ab.LoadAsset<Sprite>("skill_04_");
            //List<Item> items = ItemsManager.instance.getitemList();
            //foreach(Item item in items)
            //{
            //    if(item.Sprite == "skill_04_")
            //    {
            //        //item.Sprite = skill_04_
            //    }
            //}
            //add_item();
        }

        void UpdateDropDownItem(List<string> showNames)
        {
            showNames = new List<String>();
            showNames.Add("AAA");
            showNames.Add("BBB");
            dropDownItem = this.GetComponent<Dropdown>();
            //dropDownItem.options.Clear();
            Dropdown.OptionData temoData;
            for (int i = 0; i < showNames.Count; i++)
            {
                temoData = new Dropdown.OptionData();
                temoData.text = showNames[i];
                temoData.image = sprite_list[i];
                dropDownItem.options.Add(temoData);
            }
            dropDownItem.captionText.text = showNames[0];
        }


        void AddNames(string[] showText)
        {
            showText = new string[] { "AAA", "BBB" };
            Debug.LogFormat("showText: ", showText);
            temoNames = new List<String>();
            for (int i = 0; i < showText.Length; i++)
            {
                temoNames.Add(showText[i]);
            }
            sprite_list = new List<Sprite>();
            sprite = new Sprite[] { };
            for (int i = 0; i < sprite.Length; i++)
            {
                sprite_list.Add(sprite[i]);
            }
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
                // 创建一个新窗口
                // 注意：第一个参数(20210218)为窗口ID，ID尽量设置的与众不同，若与其他Mod的窗口ID相同，将会导致窗口冲突
                windowRect = GUI.Window(20210530, windowRect, DoMyWindow, "辣鸡游戏MOD");
                //panel.SetActive(true);


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
                titleStyle.normal.textColor = new Color(25f / 256f, 159 / 256f, 197 / 256f);
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
                save_items_to_file();
                add_item();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                //获取内功
                gen_neigong();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                //修改门派名字
                modifyPlayerFaction();
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
            //定义窗体可以活动的范围
            GUI.DragWindow(new Rect(0, 0, UnityEngine.Screen.width, UnityEngine.Screen.height));
        }

        private void modifyPlayerFaction()
        {
            if (GUILayout.Button("修改门派名字"))
            {
                modPlayerFactionFlag = true;
                var ab = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("DaYanJiangHu.title_bg_c")); 
                Debug.LogFormat("0 --- {0}", OpenUi._instance.FactionNamePanel);
                Debug.LogFormat("0 --- {0}", OpenUi._instance.FactionNamePanel.transform.GetChild(0));
                Debug.LogFormat("0 --- {0}", OpenUi._instance.FactionNamePanel.transform.GetChild(0).GetComponent<Image>().sprite.name);
                Debug.LogFormat("1 --- {0}", OpenUi._instance.FactionNamePanel.transform.GetChild(1));
                Debug.LogFormat("1 --- {0}", OpenUi._instance.FactionNamePanel.transform.GetChild(1).GetComponent<Image>().sprite.name);
                Debug.LogFormat("2 --- {0}", OpenUi._instance.FactionNamePanel.transform.GetChild(2).childCount);
                Debug.LogFormat("2 --- {0}", OpenUi._instance.FactionNamePanel.transform.GetChild(2).GetChild(0).GetComponent<Text>().text);
                Debug.LogFormat("3 --- {0}", OpenUi._instance.FactionNamePanel.transform.GetChild(3).childCount);
                //OpenUi._instance.FactionNamePanel.transform.GetChild(0).GetComponent<Image>().sprite = null;
                RectTransform rectTf = OpenUi._instance.FactionNamePanel.transform.GetChild(1).GetComponent<RectTransform>();
                //UnityEngine.Object.Destroy(OpenUi._instance.FactionNamePanel.transform.GetChild(1).gameObject);
                //UnityEngine.Object.Destroy(OpenUi._instance.FactionNamePanel.transform.GetChild(1).gameObject.GetComponent<Image>());
                //Component[] componments2 = OpenUi._instance.FactionNamePanel.transform.GetChild(1).GetComponents<Component>();
                //Debug.LogFormat("组件数量: {0}", componments2.Length);
                //UnityEngine.Object.Destroy(componments2[2]);
                //foreach (Component component_test in componments2)
                //{
                //    Debug.LogFormat("childCount{0}", component_test.transform.childCount);
                //    Debug.LogFormat("组件Type: {0}", component_test.GetType());
                //}
                //UnityEngine.Object.Destroy();
                //OpenUi._instance.FactionNamePanel.transform.GetChild(1).GetComponent<Image>().sprite = ab.LoadAsset<Sprite>("title_bg_c");
                //OpenUi._instance.FactionNamePanel.transform.GetChild(1).GetComponent<Image>().sprite = ab.LoadAsset<Sprite>("title_bg");
                OpenUi._instance.FactionNamePanel.transform.GetChild(1).GetComponent<Image>().sprite = ab.LoadAsset<Sprite>("title_bg_c");
                //OpenUi._instance.FactionNamePanel.transform.GetChild(1).GetComponent<Image>().enabled = true;
                //OpenUi._instance.FactionNamePanel.transform.GetChild(1).gameObject.AddComponent<RectTransform>();
                //OpenUi._instance.FactionNamePanel.transform.GetChild(1).gameObject.AddComponent<CanvasRenderer>();
                //OpenUi._instance.FactionNamePanel.transform.GetChild(1).gameObject.AddComponent<Image>();
                OpenUi._instance.FactionNamePanel.SetActive(true);
            }
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(OpenUi), "SetFactionName", new Type[] { })]
        public static bool Gongfa_SetFactionName_postfix(ref OpenUi __instance)
        {
            if (modPlayerFactionFlag)
            {
                SoundEffectManager._instance.ClickButton();
                if (__instance.FactionNamePanel.transform.GetChild(2).GetChild(1).GetComponent<Text>().text == "")
                {
                    __instance.systemobjet.GetComponent<SaveManager>().ShowRemind("请输入门派名字");
                    return false;
                }
                __instance.systemobjet.GetComponent<SaveManager>().ShowRemind("<color=#00FF07>门派已修改</color>");
                __instance.Gamedatas.PlayerFaction = __instance.FactionNamePanel.transform.GetChild(2).GetChild(1).GetComponent<Text>().text;
                __instance.FactionNamePanel.SetActive(false);
                return false;
            }
            else
            {
                return true;
            }
        }

        private void gen_neigong()
        {
            if (GUILayout.Button("随机获取内功"))
            {
                List<Item.Quality> item_quality = new List<Item.Quality>();
                item_quality.Add(Item.Quality.卖艺把式);
                item_quality.Add(Item.Quality.防身之术);
                item_quality.Add(Item.Quality.寻常武艺);
                item_quality.Add(Item.Quality.一流功夫);
                item_quality.Add(Item.Quality.高深武学);
                item_quality.Add(Item.Quality.盖世神功);
                int i = UnityEngine.Random.Range(0, item_quality.Count);
                if (shen_gong_flag)
                {
                    item_quality[i] = Item.Quality.盖世神功;
                }
                string text = "<color=#FFFFFF>";
                switch (item_quality[i])
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
                Item gongfa = NeiGongManager._instance.GetNeiGong(item_quality[i]);
                OpenUi._instance.Warehouse.GetComponent<Inventory>().StoreItem(gongfa);
                OpenUi._instance.systemobjet.GetComponent<SaveManager>().ShowRemind("获得新的" + gongfa.itemType.ToString() + "：" + text + gongfa.quality.ToString() + "·" + gongfa.Name + "</color>");
                if (gongfa.quality == Item.Quality.盖世神功 || gongfa.quality == Item.Quality.高深武学 || gongfa.quality == Item.Quality.一流功夫)
                {
                    OpenUi._instance.HomeMessage.Add("获得重要物品（" + text + gongfa.itemType.ToString() + "</color>）：" + text + gongfa.quality.ToString() + "·" + gongfa.Name + "</color>");
                }
            }
            if (GUILayout.Button("随机获取外功"))
            {
                List<Item.Quality> item_quality = new List<Item.Quality>();
                item_quality.Add(Item.Quality.卖艺把式);
                item_quality.Add(Item.Quality.防身之术);
                item_quality.Add(Item.Quality.寻常武艺);
                item_quality.Add(Item.Quality.一流功夫);
                item_quality.Add(Item.Quality.高深武学);
                item_quality.Add(Item.Quality.盖世神功);
                int i = UnityEngine.Random.Range(0, item_quality.Count);
                if (shen_gong_flag)
                {
                    item_quality[i] = Item.Quality.盖世神功;
                }
                string text = "<color=#FFFFFF>";
                switch (item_quality[i])
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
                Item gongfa = WaiGongManager._instance.GetWaiGong(item_quality[i]);
                OpenUi._instance.Warehouse.GetComponent<Inventory>().StoreItem(gongfa);
                OpenUi._instance.systemobjet.GetComponent<SaveManager>().ShowRemind("获得新的" + gongfa.itemType.ToString() + "：" + text + gongfa.quality.ToString() + "·" + gongfa.Name + "</color>");
                if (gongfa.quality == Item.Quality.盖世神功 || gongfa.quality == Item.Quality.高深武学 || gongfa.quality == Item.Quality.一流功夫)
                {
                    OpenUi._instance.HomeMessage.Add("获得重要物品（" + text + gongfa.itemType.ToString() + "</color>）：" + text + gongfa.quality.ToString() + "·" + gongfa.Name + "</color>");
                }
            }
            if (GUILayout.Button("随机获取身法"))
            {
                List<Item.Quality> item_quality = new List<Item.Quality>();
                item_quality.Add(Item.Quality.卖艺把式);
                item_quality.Add(Item.Quality.防身之术);
                item_quality.Add(Item.Quality.寻常武艺);
                item_quality.Add(Item.Quality.一流功夫);
                item_quality.Add(Item.Quality.高深武学);
                item_quality.Add(Item.Quality.盖世神功);
                int i = UnityEngine.Random.Range(0, item_quality.Count);
                if (shen_gong_flag)
                {
                    item_quality[i] = Item.Quality.盖世神功;
                }
                string text = "<color=#FFFFFF>";
                switch (item_quality[i])
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
                Item gongfa = ShenFaManager._instance.GetShenFa(item_quality[i]);
                OpenUi._instance.Warehouse.GetComponent<Inventory>().StoreItem(gongfa);
                OpenUi._instance.systemobjet.GetComponent<SaveManager>().ShowRemind("获得新的" + gongfa.itemType.ToString() + "：" + text + gongfa.quality.ToString() + "·" + gongfa.Name + "</color>");
                if (gongfa.quality == Item.Quality.盖世神功 || gongfa.quality == Item.Quality.高深武学 || gongfa.quality == Item.Quality.一流功夫)
                {
                    OpenUi._instance.HomeMessage.Add("获得重要物品（" + text + gongfa.itemType.ToString() + "</color>）：" + text + gongfa.quality.ToString() + "·" + gongfa.Name + "</color>");
                }
            }
            if (GUILayout.Button("神功模式"))
            {
                if (shen_gong_flag)
                {
                    shen_gong_flag = false;
                }
                else
                {
                    shen_gong_flag = true;
                }
            }
            GUILayout.Label(shen_gong_flag ? "已开启" : "已关闭", GUILayout.Width(60f));
        }

        private void save_items_to_local()
        {
            if (GUILayout.Button("虚空取物"))
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
                    update_item_config();
                }
            }
        }

        public void add_item()
        {
            GUILayout.Label("物品名称", GUILayout.Width(80f));
            default_item = GUILayout.TextField(default_item, GUILayout.Width(160));
            if (GUILayout.Button("虚空取物", GUILayout.Width(80f)))
            {
                Debug.LogFormat("物品数量: {0}", items.Count);
                if (items != null)
                {
                    foreach (Item item in items)
                    {
                        //Debug.LogFormat("物品ID: {0}, 物品名称: {1}, 物品等级: {2}", item.ID, item.Name, item.quality);
                        Debug.LogFormat("遍历的物品: {0}, 默认物品: {1}", item.Name, default_item);
                        if (item.Name.Equals(default_item))
                        {
                            selectedItem.Add(item);
                        }
                    }
                    if (selectedItem.Count > 0)
                    {
                        int i = UnityEngine.Random.Range(0, selectedItem.Count);
                        OpenUi._instance.Warehouse.GetComponent<Inventory>().StoreItem(selectedItem[i]);
                        OpenUi._instance.systemobjet.GetComponent<SaveManager>().ShowRemind(selectedItem[i].Name + "已通过虚空取物进入到您的仓库");
                    }
                    else
                    {
                        OpenUi._instance.systemobjet.GetComponent<SaveManager>().ShowRemind("无此物品，请进入游戏安装目录下的info.txt文件查找所需物品");
                    }
                }
            }
        }

        public void save_items_to_file()
        {
            string infoPath = "info.txt";
            items = ItemsManager.instance.getitemList();
            if (!File.Exists(infoPath))
            {
                FileStream fileStream = new FileStream(infoPath, FileMode.CreateNew);
                StreamWriter streamWriter = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
                streamWriter.WriteLine("ID 名称 级别 类型 值 功能1 功能2 功能3 五行属性1 五行属性2");
                if (items != null)
                {
                    foreach (Item item in items)
                    {
                        Debug.LogFormat("物品ID: {0}, 物品名称: {1}, 物品等级: {2}", item.ID, item.Name, item.quality);
                        streamWriter.WriteLine(Convert.ToString(item.ID) + " " + item.Name + " " + item.quality + " " + item.itemType + " " + item.Value + " " + item.Skills_0 + " " + item.Skills_1 + " " + item.Skills_2 + " " + item.Elements_0.ToString() + " " + item.Elements_1.ToString());
                    }
                }
                streamWriter.Close(); //此处有坑。。。。要注意安全
                fileStream.Close();
            }
        }

        private void update_item_config()
        {
            if (item_names.Count == 0)
            {
                item_names.Add("");
            }
            if (item_names.Count <= 1)
            {
                config_item = Config.AddSetting("物品选择", "物品名称", default_item, new ConfigDescription("物品选择1", null, new AcceptableValueList<string>(item_names.ToArray())));
            }

        }

        public void save_game()
        {
            OpenUi._instance.systemobjet.GetComponent<SaveManager>().savePanel.transform.GetChild(1).GetChild(0).GetComponent<SaveGame>().SartSaveGames();
        }

        public void reset_resolution()
        {
            x_resolution = Convert.ToInt32(GUILayout.TextField(Regex.Replace(Convert.ToString(x_resolution), @"[^\d]", ""), GUILayout.Width(60f)));
            y_resolution = Convert.ToInt32(GUILayout.TextField(Regex.Replace(Convert.ToString(y_resolution), @"[^\d]", ""), GUILayout.Width(60f)));
            if (GUILayout.Button("设置", GUILayout.Width(60f)))
            {
                x_resolution = x_resolution > 2560 ? 2560 : x_resolution;
                x_resolution = x_resolution < 1024 ? 1024 : x_resolution;
                y_resolution = y_resolution > 1440 ? 1440 : y_resolution;
                y_resolution = y_resolution < 768 ? 768 : y_resolution;
                UnityEngine.Screen.SetResolution(x_resolution, y_resolution, true);
            }
            //if (GUILayout.Button("全屏"))
            //{
            //    Debug.LogFormat("width: {0}, height: {1}", UnityEngine.Screen.width, UnityEngine.Screen.height);
            //    UnityEngine.Screen.SetResolution(y_resolution, y_resolution, true);
            //}
        }

        public void role_max_up()
        {
            if (GUILayout.Button("人数升级"))
            {
                //if (OpenUi._instance.Gamedatas.datingLev < 10)
                //{
                //    OpenUi._instance.Gamedatas.datingLev++;
                //    OpenUi._instance.Gamedatas.PeopleLimit_dt += 8;
                //    OpenUi._instance.systemobjet.GetComponent<SaveManager>().ShowRemind("弟子人数上限+8");
                //}
                OpenUi._instance.Gamedatas.datingLev++;
                OpenUi._instance.Gamedatas.PeopleLimit_dt += 8;
                OpenUi._instance.systemobjet.GetComponent<SaveManager>().ShowRemind("弟子人数上限+8");
            }
        }

        public void warehouse_up()
        {
            if (GUILayout.Button("仓库升级"))
            {
                if (OpenUi._instance.Gamedatas.CangkuLev < 10)
                {
                    OpenUi._instance.Gamedatas.CangkuLev++;
                    OpenUi._instance.Gamedatas.CangkuLimit += 60;
                    OpenUi._instance.systemobjet.GetComponent<SaveManager>().ShowRemind("仓库等级+1");
                }
            }
        }

        public void add_money()
        {
            if (GUILayout.Button("金钱+1000"))
            {

                //UIPanel.gameObject.SetActive(true);
                //List<Dropdown.OptionData> listOptions = new List<Dropdown.OptionData>();
                //Debug.LogFormat("初始化成功");
                //listOptions.Add(new Dropdown.OptionData("Option 0"));
                //listOptions.Add(new Dropdown.OptionData("Option 1"));
                //Debug.LogFormat("添加数据成功");
                //AddDropDownOptionsData(listOptions);

                OpenUi._instance.Gamedatas.res_money += 1000;
                OpenUi._instance.systemobjet.GetComponent<SaveManager>().ShowRemind("深受百姓爱戴，特赠礼: 金钱增加1000");
            }
        }

        void AddDropDownOptionsData(List<Dropdown.OptionData> listOptions)
        {
            if(dropDown == null)
            {
                dropDown = this.GetComponent<Dropdown>();
            }
            dropDown.AddOptions(listOptions);
        }


        //void AddDropDownOptionsData(string itemText)
        //{
        //    //添加一个下拉选项
        //    Dropdown.OptionData data = new Dropdown.OptionData();
        //    data.text = itemText;
        //    //data.image = "指定一个图片做背景不指定则使用默认"；
        //    dropDown.options.Add(data);
        //}

        public void add_food()
        {
            if (GUILayout.Button("粮食+1000"))
            {
                OpenUi._instance.Gamedatas.res_food += 1000;
                OpenUi._instance.systemobjet.GetComponent<SaveManager>().ShowRemind("深受百姓爱戴，特赠礼: 粮食增加1000");
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

        public void no_need_wait_biguan()
        {
            //连续闭关开关
            if (GUILayout.Button("闭关无需等待"))
            {
                biguan_flag = biguan_flag ? false : true;
            }
            GUILayout.Label(biguan_flag ? "已启用" : "已停用", GUILayout.Width(60f));
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
            GUILayout.Label(tili_flag ? "已启用" : "已停用", GUILayout.Width(60f));
        }

        //添加弟子
        public void add_roles_button(string sex)
        {
            if (GUILayout.Button("喜提妹子"))
            {
                sex = "女";
                add_roles_1(sex);
            }
            if (GUILayout.Button("天降猛男"))
            {
                sex = "男";
                add_roles_1(sex);
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
            GUILayout.Label(this.supper_roles_flag ? "已开启" : "已关闭", GUILayout.Width(60f));
            //if (GUILayout.Button("虚空取物"))
            //{

            //}

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

        public void add_roles_1(string sex)
        {
            if (!check_is_created_org())
            {
                return;
            }
            //int currentData = OpenUi._instance.dating.GetComponent<UI>().BG_gl.GetComponent<RoleSlot>().rumen.transform.childCount;
            int currentData = OpenUi._instance.Gamedatas.Number_rm;
            Roles roles = null;
            bool flag = true;
            for (int i = 0; i < currentData; i++)
            {
                int rumen_count = OpenUi._instance.dating.GetComponent<UI>().BG_gl.GetComponent<RoleSlot>().rumen.transform.GetChild(i).childCount;
                if (rumen_count < 8)
                {
                    roles = RoleManager.instance.GetRoles(OpenUi._instance.Gamedatas.PlayerFaction, OpenUi._instance.Gamedatas.res_fame, UnityEngine.Random.Range(2, 4));
                }
                else
                {
                    roles = RoleManager.instance.GetRoles(OpenUi._instance.Gamedatas.PlayerFaction, OpenUi._instance.Gamedatas.res_fame, 1);
                }

                if (roles != null )
                {
                    if (sex == null || roles.Sex.Equals(sex))
                    {
                        flag = true;
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
                        roles.Salary *= -1;
                        OpenUi._instance.Gamedatas.Roles_player.Add(roles);
                        OpenUi._instance.Gamedatas.Number_rm += 1;
                        OpenUi._instance.Gamedatas.People_dt += 1;
                        Debug.LogFormat("currentData: {0}", currentData);
                        Debug.LogFormat("People_dt: {0}", OpenUi._instance.Gamedatas.People_dt);
                        Debug.LogFormat("Roles_Wuguan: {0}", OpenUi._instance.Gamedatas.Roles_Wuguan);
                        Debug.LogFormat("Number_rm: {0}", OpenUi._instance.Gamedatas.Number_rm);
                        int pageSize = OpenUi._instance.Gamedatas.Number_rm / 8;
                        //Convert.ToInt32(Math.Truncate(pageSize));
                        Debug.LogFormat("pageSize: {0}", pageSize);
                        //OpenUi._instance.talk.GetComponent<DialogManager>().roles.Add(roles);
                        OpenUi._instance.dating.GetComponent<UI>().BG_gl.GetComponent<RoleSlot>().rumen.transform.GetChild(currentData / 8).GetComponent<roleList_R>().GetRoleList(roles);
                        OpenUi._instance.systemobjet.GetComponent<SaveManager>().ShowRemind(roles.Name + "成功移民来到了您的门派");
                        return;
                    }
                    else
                    {
                        flag = false;
                        break;
                    }
                }
            }
            if (!flag)
            {
                add_roles(sex);
            }
        }

        public void add_roles(string sex)
        {
            if (!check_is_created_org())
            {
                return;
            }
            Roles roles = RoleManager.instance.GetRoles(OpenUi._instance.Gamedatas.PlayerFaction, OpenUi._instance.Gamedatas.res_fame, UnityEngine.Random.Range(2, 4));
            if (roles != null && sex != null)
            {
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
                    roles.Salary *= -1;
                    OpenUi._instance.Gamedatas.Roles_player.Add(roles);
                    OpenUi._instance.Gamedatas.Number_rm += 1;
                    OpenUi._instance.Gamedatas.People_dt += 1;
                    int currentData = OpenUi._instance.dating.GetComponent<UI>().BG_gl.GetComponent<RoleSlot>().rumen.transform.childCount;
                    Debug.LogFormat("currentData: {0}", currentData);
                    for (int i = 0; i < currentData; i++)
                    {
                        if (OpenUi._instance.dating.GetComponent<UI>().BG_gl.GetComponent<RoleSlot>().rumen.transform.GetChild(i).childCount < 8)
                        {

                        }
                        else
                        {
                            roles = RoleManager.instance.GetRoles(OpenUi._instance.Gamedatas.PlayerFaction, OpenUi._instance.Gamedatas.res_fame, 1);
                        }
                        //    if (i > 8)
                        //{
                        //    i += 1;
                        //    currentData += 1;
                        //}
                        OpenUi._instance.dating.GetComponent<UI>().BG_gl.GetComponent<RoleSlot>().rumen.transform.GetChild(i).GetComponent<roleList_R>().GetRoleList(roles);
                    }

                    Debug.LogFormat("People_dt: {0}", OpenUi._instance.Gamedatas.People_dt);
                    Debug.LogFormat("Roles_Wuguan: {0}", OpenUi._instance.Gamedatas.Roles_Wuguan);
                    OpenUi._instance.systemobjet.GetComponent<SaveManager>().ShowRemind(roles.Name + "成功移民来到了您的门派");
                }
                else
                {
                    add_roles(sex);
                }
            }
        }

        public void add_item_bak()
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
            //roles.TrainProbability = (float)Traverse.Create(__instance).Field("Probability").GetValue();
            roles.TrainSpeed = (float)Traverse.Create(__instance).Field("trainSpeed").GetValue();
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
        [HarmonyPostfix]
        [HarmonyPatch(typeof(RoleDataOperations), "MasterRolesPractice")]
        public static void RoleDataOperations_MasterRolesPractice_Transpiler(ref RoleDataOperations __instance)
        {
            OpenUi._instance.Gamedatas.MasterRoles.PracticeTime_now = 1;
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
                //Debug.LogFormat("时间: {0}, 数据: {1}", tradeTime, code.operand);
                //var methodInfo = AccessTools.Method(typeof(UnityEngine.Random), nameof(UnityEngine.Random.Range), new System.Type[] { typeof(int), typeof(int) });
                if (code.opcode == OpCodes.Ldc_I4 && code.operand != null && code.operand == (object)1800)
                {
                    //Debug.LogFormat("code.operand == 1800 IL值: {0}", code.operand);
                    yield return new CodeInstruction(OpCodes.Ldc_R4, 0.001);
                }
                if (code.opcode == OpCodes.Ldc_I4 && code.operand != null && code.operand == (object)3000)
                {
                    //Debug.LogFormat("code.operand == 1800 IL值: {0}", code.operand);
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
        public static void GetGongFa_GongFaGet_Transpiler(ref GetGongFa __instance, ref Item __result, ref float __state)
        {
            //string tradeTime = DateTime.Now.ToString("yyyy-MM-dd HH：mm：ss", DateTimeFormatInfo.InvariantInfo);
            Debug.LogFormat("创建功法 __atk: {0}", __state);
            //List<Item.Quality> good_qualitys = new List<Item.Quality>();
            //good_qualitys.Add(Item.Quality.精品);
            //good_qualitys.Add(Item.Quality.绝品);
            //good_qualitys.Add(Item.Quality.神品);
            //good_qualitys.Add(Item.Quality.一流功夫);
            //good_qualitys.Add(Item.Quality.高深武学);
            //good_qualitys.Add(Item.Quality.盖世神功);
            Get_z._instance.Atk = __state;
            //if (good_qualitys.Contains(__result.quality))
            //{
            //    string text = "<color=#FFFFFF>";
            //    switch (__instance.gongfa.quality)
            //    {
            //        case Item.Quality.防身之术:
            //            text = "<color=#D3D938>";
            //            break;
            //        case Item.Quality.寻常武艺:
            //            text = "<color=#38D946>";
            //            break;
            //        case Item.Quality.一流功夫:
            //            text = "<color=#63F0FF>";
            //            break;
            //        case Item.Quality.高深武学:
            //            text = "<color=#FF4840>";
            //            break;
            //        case Item.Quality.盖世神功:
            //            text = "<color=#FFB243>";
            //            break;
            //    }
            //OpenUi._instance.systemobjet.GetComponent<SaveManager>().ShowRemind("获得新的" + __instance.gongfa.itemType.ToString() + "：" + text + __instance.gongfa.quality.ToString() + "·" + __instance.gongfa.Name + "</color>");
            //OpenUi._instance.HomeMessage.Add("获得重要物品（" + text + __instance.gongfa.itemType.ToString() + "</color>）：" + text + __instance.gongfa.quality.ToString() + "·" + __instance.gongfa.Name + "</color>");
            //}
        }

        //功法
        //[HarmonyPostfix]
        ////[HarmonyPatch(typeof(GetGongFa), "GongFaGet", new Type[] { typeof(string), typeof(Item.Quality), typeof(int), typeof(string), typeof(string), typeof(Item.ItemType) })]
        //[HarmonyPatch(typeof(GetGongFa), "GetInput", new Type[] { })]
        //public static void GetGongFa_GetInput_Transpiler(ref GetGongFa __instance)
        //{
        //    string tradeTime = DateTime.Now.ToString("yyyy-MM-dd HH：mm：ss", DateTimeFormatInfo.InvariantInfo);
        //    List<Item.Quality> good_qualitys = new List<Item.Quality>();
        //    good_qualitys.Add(Item.Quality.精品);
        //    good_qualitys.Add(Item.Quality.绝品);
        //    good_qualitys.Add(Item.Quality.神品);
        //    good_qualitys.Add(Item.Quality.一流功夫);
        //    good_qualitys.Add(Item.Quality.高深武学);
        //    good_qualitys.Add(Item.Quality.盖世神功);
        //    if (good_qualitys.Contains(__instance.gongfa.quality))
        //    {
        //        string text = "<color=#FFFFFF>";
        //        switch (__instance.gongfa.quality)
        //        {
        //            case Item.Quality.防身之术:
        //                text = "<color=#D3D938>";
        //                break;
        //            case Item.Quality.寻常武艺:
        //                text = "<color=#38D946>";
        //                break;
        //            case Item.Quality.一流功夫:
        //                text = "<color=#63F0FF>";
        //                break;
        //            case Item.Quality.高深武学:
        //                text = "<color=#FF4840>";
        //                break;
        //            case Item.Quality.盖世神功:
        //                text = "<color=#FFB243>";
        //                break;
        //        }
        //        //OpenUi._instance.systemobjet.GetComponent<SaveManager>().ShowRemind("获得新的" + __instance.gongfa.itemType.ToString() + "：" + text + __instance.gongfa.quality.ToString() + "·" + __instance.gongfa.Name + "</color>");
        //        OpenUi._instance.HomeMessage.Add("获得重要物品（" + text + __instance.gongfa.itemType.ToString() + "</color>）：" + text + __instance.gongfa.quality.ToString() + "·" + __instance.gongfa.Name + "</color>");
        //    }
        //}

        [HarmonyPostfix]
        //[HarmonyPatch(typeof(GetGongFa), "GongFaGet", new Type[] { typeof(string), typeof(Item.Quality), typeof(int), typeof(string), typeof(string), typeof(Item.ItemType) })]
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
                //OpenUi._instance.HomeMessage.Add("获得重要物品（" + text + __instance.gongfa.itemType.ToString() + "</color>）：" + text + __instance.gongfa.quality.ToString() + "·" + __instance.gongfa.Name + "</color>");
            }
        }

        //功法
        [HarmonyPrefix]
        //[HarmonyPatch(typeof(GetGongFa), "GongFaGet", new Type[] { typeof(string), typeof(Item.Quality), typeof(int), typeof(string), typeof(string), typeof(Item.ItemType) })]
        [HarmonyPatch(typeof(Tasks), "GetTasksRemuneration", new Type[] { })]
        public static bool Tasks_GetTasksRemuneration_pretfix(ref Tasks __instance)
        {

            return true;
        }

        //功法
        [HarmonyPostfix]
        //[HarmonyPatch(typeof(GetGongFa), "GongFaGet", new Type[] { typeof(string), typeof(Item.Quality), typeof(int), typeof(string), typeof(string), typeof(Item.ItemType) })]
        [HarmonyPatch(typeof(Tasks), "GetTasksRemuneration", new Type[] { })]
        public static void Tasks_GetTasksRemuneration_postfix(ref Tasks __instance)
        {
            __instance.state = Tasks.State.已完成;
            List<Roles> task_roles = __instance.Taskroles;
            List<string> roles_name = new List<string>();
            foreach (Roles role in task_roles)
            {
                Debug.LogFormat("任务名：{0}，任务角色: {1}", __instance.TaskName, role.Name);
                roles_name.Add(role.Name);
                //role.rolestate = Roles.RoleState.None;
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
            //__instance.Taskroles = new List<Roles>();
            //List<Tasks> TasksItemSlotList = OpenUi._instance.Gamedatas.tasksList;
            //Debug.LogFormat("测试任务完成！！！！！！！！！{0}", TasksItemSlotList.Length);
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
