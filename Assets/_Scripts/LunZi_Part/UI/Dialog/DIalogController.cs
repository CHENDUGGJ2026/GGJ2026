using luoyu;
using MyFrame.BrainBubbles.Bubbles.Manager;
using MyFrame.EventSystem.Events;
using MyFrame.EventSystem.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace LunziSpace
{


    public class DialogController : MonoBehaviour
    {
        

        #region 序列化字段（防呆，可在Inspector赋值/代码自动查找）
        [Header("对话界面组件")]
        [SerializeField] private GameObject Name;
        [SerializeField] private GameObject Content;
        [SerializeField] private GameObject SpeakerPicture;
        [SerializeField] private GameObject NextBtn;
        [SerializeField] private GameObject PlayerPicture;
        [SerializeField] private GameObject FightBtn;
        #endregion

        #region 组件引用【核心修正：SpriteRenderer → Image（UI组件）】
        private Image _playerImage;       // 玩家立绘（UI Image）
        private Image _speakerImage;      // NPC立绘（UI Image）
        private TMP_Text _content;
        private TMP_Text _name;
        private Button _nextBtn;
        private Button _fightBtn;
        #endregion

        #region 对话核心参数
        private int curPinter = 0; // 对话指针（标记当前要显示的行号）
        private string[] rows;     // 分割后的对话文本行数组
        private TextAsset curTextFile; // 当前加载的对话文本文件
        private string curNPCid;   // 当前对话的NPC ID
        public  DialogData dialogDataBase; // 全局对话数据基类
        #endregion

        public Action FightStarAction;
        public Action FightAction;
        public Action FightOver;

        public void StartGame()
        {
            StartCoroutine(AutoStart());
        }
        IEnumerator AutoStart()
        {
            yield return new WaitForSecondsRealtime(2);
            GameManager.Instance._eventBus.Publish<FightStartEvent>(new FightStartEvent());//推送战斗开始事件
            Debug.Log("战斗开始事件推送完成");

            var scene = new BrainSceneManager(this.gameObject.transform.parent.GetComponent<RectTransform>(), Vector2Int.zero , new Vector2Int(Screen.width, Screen.height), new GameOverAdaptor(new Over()));
            FightAction?.Invoke();
            GameManager.Instance.AddUpdateListener("BrainSceneManager", scene.OnUpdate);
            scene.Start();
            yield return null;
        }

        private void Start()
        {
            InitDialogData(); // 初始化对话数据
            Init(); // 先初始化组件和事件

            gameObject.SetActive(false);
            FightBtn.SetActive(false);
            _fightBtn.onClick.AddListener(() =>
            {
                GameManager.Instance._eventBus.Publish<FightStartEvent>(new FightStartEvent());//推送战斗开始事件
                Debug.Log("战斗开始事件推送完成");

                var scene = new BrainSceneManager(this.gameObject.transform.parent.GetComponent<RectTransform>(), Vector2Int.zero, new Vector2Int(Screen.width, Screen.height), new GameOverAdaptor(new Over()));
                FightAction?.Invoke();
                GameManager.Instance.AddUpdateListener("BrainSceneManager", scene.OnUpdate);
                scene.Start();



            });
        }

        /// <summary>
        /// 初始化界面组件+事件监听（核心，确保组件获取和事件绑定）
        /// </summary>
        private void Init()
        {
            

            // 获取组件核心组件【修正：GetComponent<Image>() 替代 SpriteRenderer】
            _speakerImage = SpeakerPicture.GetComponent<Image>();
            _playerImage = PlayerPicture.GetComponent<Image>();
            _name = Name.GetComponent<TMP_Text>();
            _content = Content.GetComponent<TMP_Text>();
            _nextBtn = NextBtn.GetComponent<Button>();
            _fightBtn = FightBtn.GetComponent<Button>();

            // 组件空值校验（测试阶段关键，快速定位缺失组件）【修正：提示文字同步改为Image】
            if (_speakerImage == null) Debug.LogWarning("【DialogController】未找到SpeakerPicture的Image组件（UI专用）");
            if (_playerImage == null) Debug.LogWarning("【DialogController】未找到PlayerPicture的Image组件（UI专用）");
            if (_name == null) Debug.LogError("【DialogController】未找到SpeakerName的TMP_Text组件！");
            if (_content == null) Debug.LogError("【DialogController】未找到Content的TMP_Text组件！");
            if (_nextBtn == null) Debug.LogError("【DialogController】未找到NextBtn的Button组件！");

            // 绑定下一句按钮事件（确保只绑定一次，防止重复触发）
            _nextBtn?.onClick.RemoveAllListeners();
            _nextBtn?.onClick.AddListener(OnNextDialog);

            // 初始化界面默认状态
            _content.text = "请触发对话...";
            _name.text = "未知NPC";
            NextBtn.SetActive(false); // 初始隐藏下一页按钮
                                      // 初始化立绘状态（UI Image默认隐藏，避免空白占位）
           /* _speakerImage?.gameObject.SetActive(false);
            _playerImage?.gameObject.SetActive(false);*/
        }

        /// <summary>
        /// 初始化对话数据基类
        /// </summary>
        private void InitDialogData()
        {
            if (dialogDataBase == null)
            {
                dialogDataBase = new DialogData();
                Debug.Log("【DialogController】对话数据基类初始化完成");
            }
        }

        /// <summary>
        /// 测试用：加载NPC_001的随机普通对话
        /// </summary>
        private void TestDialog()
        {
            SetCurNPCid("NPC_001"); // 设置当前测试NPC ID
            TextAsset testText = dialogDataBase.GetRandomNormalDialog(curNPCid);
            UpdataCurText(testText); // 更新并解析对话
            UpdateNPCInfo(); // 加载NPC名字和立绘（测试阶段核心）
        }

        /// <summary>
        /// 下一句按钮点击事件（核心逻辑：指针自增+显示下一句）
        /// </summary>
        private void OnNextDialog()
        {
            Debug.Log($"【对话调试】当前指针：{curPinter}，准备执行下一句");
            curPinter++; // 指针自增，指向下一行对话
            TextParsing(rows); // 解析并显示当前指针对应的对话
        }

        /// <summary>
        /// 解析文本行，根据指针和标记执行对应逻辑
        /// 文本规则：行内用逗号分隔，字段0=行号，字段1=对话内容，字段5=标记（#=正常/Fight=战斗/Over=结束）
        /// </summary>
        private void TextParsing(string[] rows)
        {
            // 空值和长度校验
            if (rows == null || rows.Length == 0)
            {
                Debug.LogWarning("【文本解析】对话行数组为空，无法解析");
                NextBtn.SetActive(false);
                return;
            }

            // 指针越界校验（对话结束）
            if (curPinter > rows.Length - 1)
            {
                Debug.Log("【对话调试】对话已全部播放完毕");
                _content.text = "对话结束！";
                NextBtn.SetActive(false);
                curPinter = 0; // 指针重置，方便下次对话
                return;
            }

            try
            {
                string currentRow = rows[curPinter]; // 获取当前指针对应的行
                string[] cells = currentRow.Split(','); // 按逗号分割字段

                // 字段数量校验（防止文本格式错误导致数组越界）
                if (cells.Length < 6)
                {
                    Debug.LogWarning($"【文本解析】第{curPinter}行格式错误，字段不足6个，内容：{currentRow}");
                    curPinter++; // 跳过错误行
                    TextParsing(rows);
                    return;
                }

                // 解析行号（容错：防止行号不是数字）
                if (!int.TryParse(cells[0], out int lineNum))
                {
                    Debug.LogWarning($"【文本解析】第{curPinter}行行号不是数字，内容：{currentRow}");
                    curPinter++;
                    TextParsing(rows);
                    return;
                }

                string content = cells[1].Trim(); // 对话内容（去空格，防止隐形字符）
                string tag = cells[5].Trim();     // 标记位（去空格，防止空格导致匹配失败）

                // 根据标记执行对应逻辑
                switch (tag)
                {
                    case "#": // 正常对话
                        Debug.Log($"【对话调试】显示第{lineNum}行正常对话，指针：{curPinter}");
                        DialogUpdata(content);
                        NextBtn.SetActive(true); // 显示下一页按钮
                        break;

                    case "Fight": // 触发战斗
                        Debug.Log($"【对话调试】第{lineNum}行触发战斗，指针重置");
                        DialogUpdata(content);
                        curPinter = 0; // 指针重置
                        NextBtn.SetActive(false);
                        //FightBtn.SetActive(true);
                        FightStarAction?.Invoke();
                        // 后续可添加战斗事件触发代码：BattleManager.Instance.StartFight(curNPCid);
                        break;

                    case "Over": // 对话结束/结算
                        Debug.Log($"【对话调试】第{lineNum}行对话结束，进入结算");
                        DialogUpdata(content);
                        curPinter = 0; // 指针重置
                        NextBtn.SetActive(false);
                        // 后续可添加结算事件代码：UIManager.Instance.OpenSettleUI();
                        break;

                    default: // 未知标记
                        Debug.LogWarning($"【文本解析】第{lineNum}行发现未知标记：{tag}，跳过该行");
                        curPinter++;
                        TextParsing(rows);
                        break;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"【文本解析异常】指针：{curPinter}，错误信息：{e.Message}");
                curPinter++; // 异常后指针自增，防止卡死
                TextParsing(rows);
            }
        }

        /// <summary>
        /// 更新对话内容（仅负责文本，单一职责）
        /// </summary>
        private void DialogUpdata(string content = "Null")
        {
            if (_content != null)
            {
                _content.text = content;
            }
        }

        /// <summary>
        /// 更新当前对话文本并立即解析（外部调用核心方法）
        /// </summary>
        /// <param name="textAsset">要加载的对话文本</param>
        public void UpdataCurText(TextAsset textAsset)
        {
            // 空值校验
            if (textAsset == null)
            {
                Debug.LogWarning("【DialogController】传入的对话文本为空");
                _content.text = "无对话文本！";
                curTextFile = null;
                rows = null;
                NextBtn.SetActive(false);
                return;
            }

            // 更新当前文本并按行分割
            curTextFile = textAsset;
            string textFile_text = curTextFile.text;
            // 兼容所有换行符+过滤空行，保证解析纯净
            rows = textFile_text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            Debug.Log($"【DialogController】当前对话文本更新完成，共{rows.Length}行有效内容");
            curPinter = 0; // 重置指针，从第一行开始显示
            TextParsing(rows); // 立即解析并显示第一句
        }

        /// <summary>
        /// 设置当前NPC ID并更新NPC信息（名字+立绘，测试阶段关键）
        /// </summary>
        /// <param name="npcid">NPC唯一ID</param>
        public void SetCurNPCid(string npcid)
        {
            if (string.IsNullOrEmpty(npcid))
            {
                Debug.LogWarning("【DialogController】NPC ID为空");
                return;
            }

            curNPCid = npcid;
            Debug.Log($"【DialogController】当前对话NPC ID设置为：{curNPCid}");
            UpdateNPCInfo(); // 同步更新名字和立绘
        }

        /// <summary>
        /// 根据当前NPC ID更新名字和立绘（对接DialogData）【核心修正：适配UI Image组件】
        /// </summary>
        private void UpdateNPCInfo()
        {
            if (dialogDataBase == null || string.IsNullOrEmpty(curNPCid))
            {
                return;
            }

            // 获取NPC立绘并设置【修正：Image.sprite + SetActive 替代 SpriteRenderer】
            Sprite npcSprite = dialogDataBase.GetCharacterSprite(curNPCid);
            if (_speakerImage != null)
            {
                if (npcSprite != null)
                {
                    _speakerImage.sprite = npcSprite;
                    _speakerImage.SetNativeSize(); // 可选：还原Sprite原始尺寸，适配UI
                    _speakerImage.gameObject.SetActive(true); // 显示立绘
                }
                else
                {
                    Debug.LogWarning($"【DialogController】未找到NPC {curNPCid} 的立绘，隐藏立绘");
                    _speakerImage.gameObject.SetActive(false); // 隐藏立绘
                }
            }

            // 获取NPC名字并设置（从DialogData的配置中查找，需在DialogData加拓展方法）
            string npcName = dialogDataBase.GetCharacterName(curNPCid);
            if (_name != null)
            {
                _name.text = string.IsNullOrEmpty(npcName) ? "未知NPC" : npcName;
            }
        }

        /// <summary>
        /// 解析正常对话文本ID为0的行中的第三个字段，转换为int返回
        /// </summary>
        /// <returns>成功返回字段值，失败返回-1</returns>
        public int TargetValue()
        {
            // 1. 基础空值校验：文本行数组/当前文本文件为空，直接返回错误值
            if (rows == null || rows.Length == 0 || curTextFile == null)
            {
                Debug.LogWarning($"【TargetValue解析】对话行数组为空或未加载文本文件，解析失败");
                return -1;
            }

            // 2. 直接定位ID=0的行（文本第一行，数组索引0，精准匹配需求）
            string targetRow = rows[1];
            // 校验目标行是否为空（过滤空行，防止解析异常）
            if (string.IsNullOrEmpty(targetRow.Trim()))
            {
                Debug.LogWarning($"【TargetValue解析】ID为0的行（第一行）为空行，解析失败");
                return -1;
            }

            try
            {
                // 3. 按逗号分割字段，校验至少包含3个字段（保证能取到Wanted）
                string[] targetCells = targetRow.Split(',');
                if (targetCells.Length < 3)
                {
                    Debug.LogWarning($"【TargetValue解析】ID为0的行字段不足3个（需ID,Content,Wanted），内容：{targetRow}，解析失败");
                    return -1;
                }

                // 4. 提取第三个字段（索引2=Wanted）并去空格（防止隐形空格/制表符导致转换失败）
                string wantedField = targetCells[2].Trim();
                // 5. 容错处理空字段情况
                if (string.IsNullOrEmpty(wantedField))
                {
                    Debug.LogWarning($"【TargetValue解析】ID为0的行第三个字段（Wanted）为空，解析失败");
                    return -1;
                }

                // 6. 转换为int并返回，容错非数字格式
                if (int.TryParse(wantedField, out int result))
                {
                    Debug.Log($"【TargetValue解析成功】ID为0的行第三个字段（Wanted）值：{result}，原始内容：{wantedField}");
                    return result;
                }
                else
                {
                    Debug.LogWarning($"【TargetValue解析】ID为0的行第三个字段（Wanted）不是有效数字，内容：{wantedField}，解析失败");
                    return -1;
                }
            }
            catch (Exception e)
            {
                // 异常捕获，防止文本格式异常导致程序卡死
                Debug.LogError($"【TargetValue解析异常】ID为0的行内容：{targetRow}，错误信息：{e.Message}");
                return -1;
            }
        }
        /// <summary>
        /// 得条件值
        /// </summary>
        /// <returns></returns>
        public int GetConditionValue()
        {
            // 1. 基础空值校验：与TargetValue一致，保证行数据有效
            if (rows == null || rows.Length == 0 || curTextFile == null)
            {
                Debug.LogWarning($"【Condition解析】对话行数组为空或未加载文本文件，解析失败");
                return -1;
            }

            // 2. 与TargetValue读取同一行（ID=0行，数组索引0），保证行一致
            string targetRow = rows[1];
            // 校验目标行是否为空（过滤空行，防止解析异常）
            if (string.IsNullOrEmpty(targetRow.Trim()))
            {
                Debug.LogWarning($"【Condition解析】ID为0的行为空行，解析失败");
                return -1;
            }

            try
            {
                // 3. 按逗号分割字段，校验至少包含4个字段（保证能取到Condition）
                string[] targetCells = targetRow.Split(',');
                if (targetCells.Length < 4)
                {
                    Debug.LogWarning($"【Condition解析】ID为0的行字段不足4个（需ID,Content,Wanted,Condition），内容：{targetRow}，解析失败");
                    return -1;
                }

                // 4. 提取第四个字段（索引3=Condition）并去空格（防止隐形空格/制表符导致转换失败）
                string conditionField = targetCells[3].Trim();
                // 5. 容错处理空字段情况
                if (string.IsNullOrEmpty(conditionField))
                {
                    Debug.LogWarning($"【Condition解析】ID为0的行第四个字段（Condition）为空，解析失败");
                    return -1;
                }

                // 6. 转换为int并返回，容错非数字格式
                if (int.TryParse(conditionField, out int result))
                {
                    Debug.Log($"【Condition解析成功】ID为0的行第四个字段（Condition）值：{result}，原始内容：{conditionField}");
                    return result;
                }
                else
                {
                    Debug.LogWarning($"【Condition解析】ID为0的行第四个字段（Condition）不是有效数字，内容：{conditionField}，解析失败");
                    return -1;
                }
            }
            catch (Exception e)
            {
                // 异常捕获，防止文本格式异常导致程序卡死
                Debug.LogError($"【Condition解析异常】ID为0的行内容：{targetRow}，错误信息：{e.Message}");
                return -1;
            }
        }
    }

    #region Mode层
    public class DialogData
    {
        /// <summary>
        /// 当前对话文件
        /// </summary>
        private TextAsset currentFile;

        /// <summary>
        /// 总表：角色ID → （对话序号 → 对话文本）
        /// </summary>
        private Dictionary<string, Dictionary<int, TextAsset>> _allTextDic = new Dictionary<string, Dictionary<int, TextAsset>>();

        /// <summary>
        /// 角色ID到立绘的表
        /// </summary>
        private Dictionary<string, Sprite> _allSpriteDic = new Dictionary<string, Sprite>();

        // 初始化所有List，避免空引用异常
        private List<TextAsset> _failDialog = new List<TextAsset>();
        private List<TextAsset> _successDialog = new List<TextAsset>();

        /// <summary>
        /// 存储所有的角色配置信息
        /// </summary>
        private List<CharacterData> _allCharacterConfigList = new List<CharacterData>();

        /// <summary>
        /// 构造函数加载文本
        /// </summary>
        public DialogData()
        {
            ReadCharacterConfig();
            LoadAllAssets();
        }

        /// <summary>
        /// 读取角色配置(Unity内置JsonUtility解析)
        /// 路径：Resources/Config/CharacterConfig.json
        /// </summary>
        private void ReadCharacterConfig()
        {
            // 加载JSON配置文件
            TextAsset configJson = Resources.Load<TextAsset>("Config/CharacterConfig");
            if (configJson == null)
            {
                Debug.LogError("【DialogData】无法加载角色配置文件！请检查路径：Resources/Config/CharacterConfig");
                return;
            }

            try
            {
                // JsonUtility解析数组需包装类，此处直接解析为列表
                _allCharacterConfigList = JsonUtility.FromJson<CharacterDataList>(configJson.text).characterDatas;
                if (_allCharacterConfigList != null && _allCharacterConfigList.Count > 0)
                {
                    Debug.Log($"【DialogData】成功加载 {_allCharacterConfigList.Count} 个角色配置");
                }
                else
                {
                    Debug.LogWarning("【DialogData】角色配置文件解析成功，但无数据");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"【DialogData】解析角色配置JSON失败：{e.Message}\n请检查JSON格式是否符合Unity JsonUtility规范");
            }
        }

        private void LoadAllAssets()
        {
            LoadAllSpriteToAllSpriteDic();
            LoadAllNormalDialogText();
            LoadSuccessList();
            LoadFailList();
        }

        /// <summary>
        /// 按配置路径加载所有角色立绘
        /// </summary>
        private void LoadAllSpriteToAllSpriteDic()
        {
            if (_allCharacterConfigList == null || _allCharacterConfigList.Count == 0)
            {
                Debug.LogWarning("【DialogData】角色配置列表为空，跳过立绘加载");
                return;
            }

            foreach (var character in _allCharacterConfigList)
            {
                // 跳过空路径，避免无效加载
                if (string.IsNullOrEmpty(character.SpritePath))
                {
                    Debug.LogWarning($"【DialogData】角色{character.npcID}立绘路径为空，跳过");
                    continue;
                }
                // 加载立绘
                Sprite sprite = Resources.Load<Sprite>(character.SpritePath);
                if (sprite != null)
                {
                    _allSpriteDic[character.npcID] = sprite;
                    Debug.Log($"【DialogData】成功加载立绘：{character.SpritePath}");
                }
                else
                {
                    Debug.LogWarning($"【DialogData】无法加载立绘：{character.SpritePath}，请检查文件是否存在");
                }
            }
        }

        /// <summary>
        /// 加载所有NPC的普通对话文本，填充_allTextDic
        /// </summary>
        private void LoadAllNormalDialogText()
        {
            if (_allCharacterConfigList == null || _allCharacterConfigList.Count == 0)
            {
                Debug.LogWarning("【DialogData】角色配置列表为空，跳过对话文本加载");
                return;
            }

            foreach (var character in _allCharacterConfigList)
            {
                var dialogDic = new Dictionary<int, TextAsset>();
                // 按配置的对话数量循环加载
                for (int i = 1; i <= character.normalDialogCount; i++)
                {
                    string loadPath = $"{character.textPath}/{i}";
                    TextAsset dialogText = Resources.Load<TextAsset>(loadPath);
                    if (dialogText != null)
                    {
                        dialogDic[i] = dialogText;
                        Debug.Log($"【DialogData】成功加载对话：{loadPath}");
                    }
                    else
                    {
                        Debug.LogWarning($"【DialogData】无法加载对话：{loadPath}，请检查文件是否存在");
                    }
                }
                // 将当前NPC的对话库加入总表
                _allTextDic[character.npcID] = dialogDic;
            }
            Debug.Log($"【DialogData】所有普通对话加载完成，共{_allTextDic.Count}个NPC的对话库");
        }

        /// <summary>
        /// 加载成功对话文本（Resources/Dialogues/Success/1~5）
        /// </summary>
        private void LoadSuccessList()
        {
            for (int i = 0; i < 1; i++)
            {
                string loadPath = $"Dialogues/Success/{i + 1}";
                TextAsset dialog = Resources.Load<TextAsset>(loadPath);
                if (dialog != null)
                {
                    _successDialog.Add(dialog);
                    Debug.Log($"【DialogData】加载了{i + 1}号 成功对话文件");
                }
                else
                {
                    Debug.LogWarning($"【DialogData】无法加载成功对话：{loadPath}");
                }
            }
        }

        /// <summary>
        /// 加载失败对话文本（Resources/Dialogues/Fail/1~5）
        /// </summary>
        private void LoadFailList()
        {
            for (int i = 0; i < 1; i++)
            {
                string loadPath = $"Dialogues/Fail/{i + 1}";
                TextAsset dialog = Resources.Load<TextAsset>(loadPath);
                if (dialog != null)
                {
                    _failDialog.Add(dialog);
                    Debug.Log($"【DialogData】加载了{i + 1}号 失败对话文件");
                }
                else
                {
                    Debug.LogWarning($"【DialogData】无法加载失败对话：{loadPath}");
                }
            }
        }

        #region 外部公共接口（供外部脚本调用）
        /// <summary>
        /// 得到当前对话文件
        /// </summary>
        public TextAsset GetCurrentText()
        {
            return currentFile;
        }

        /// <summary>
        /// 根据角色ID获取随机普通对话（核心接口）
        /// </summary>
        /// <param name="npcID">角色唯一ID（如npc_001）</param>
        /// <returns>随机对话文本，失败返回null</returns>
        public TextAsset GetRandomNormalDialog(string npcID)
        {
            // 检查NPC是否存在对话库
            if (!_allTextDic.TryGetValue(npcID, out var dialogDic) || dialogDic.Count == 0)
            {
                Debug.LogWarning($"【DialogData】角色{npcID}无可用的普通对话");
                return null;
            }
            // 随机获取对话序号
            List<int> dialogIndices = new List<int>(dialogDic.Keys);
            int randomIndex = dialogIndices[UnityEngine.Random.Range(0, dialogIndices.Count)];
            // 赋值当前对话并返回
            currentFile = dialogDic[randomIndex];

            // 新增调试日志：打印NPCID + 随机到的对话序号（第几个文本）
            Debug.Log($"【DialogData-随机对话调试】角色{npcID}，随机到第{randomIndex}个普通对话文本");

            return currentFile;
        }

        public string GetCharacterName(string npcID)
        {
             var character = _allCharacterConfigList.Find(c => c.npcID == npcID);
             return character == null ? null : character.name;
         }
            /// <summary>
            /// 获取随机成功对话
            /// </summary>
        public TextAsset GetRandomSuccessDialog()
        {
            if (_successDialog.Count == 0)
            {
                Debug.LogWarning("【DialogData】无可用的成功对话");
                return null;
            }
            int randomIndex = UnityEngine.Random.Range(0, _successDialog.Count);
            currentFile = _successDialog[randomIndex];
            return currentFile;
        }

        /// <summary>
        /// 获取随机失败对话
        /// </summary>
        public TextAsset GetRandomFailDialog()
        {
            if (_failDialog.Count == 0)
            {
                Debug.LogWarning("【DialogData】无可用的失败对话");
                return null;
            }
            int randomIndex = UnityEngine.Random.Range(0, _failDialog.Count);
            currentFile = _failDialog[randomIndex];
            return currentFile;
        }

        /// <summary>
        /// 根据角色ID获取立绘
        /// </summary>
        /// <param name="npcID">角色唯一ID</param>
        /// <returns>角色立绘Sprite，失败返回null</returns>
        public Sprite GetCharacterSprite(string npcID)
        {
            if (_allSpriteDic.TryGetValue(npcID, out var sprite))
            {
                return sprite;
            }
            Debug.LogWarning($"【DialogData】未找到角色{npcID}的立绘");
            return null;
        }

        /// <summary>
        /// 检查角色是否存在配置
        /// </summary>
        public bool IsCharacterExist(string npcID)
        {
            return _allCharacterConfigList.Exists(c => c.npcID == npcID);
        }


        public CharacterData GetRandomCharacterData(int index)
        {
            return _allCharacterConfigList[index];
        }
        #endregion
    }

    #region JSON解析辅助类（Unity JsonUtility专用）
    /// <summary>
    /// 角色配置JSON数组包装类
    /// Unity JsonUtility不支持直接解析纯数组，需外层包装
    /// </summary>
    [Serializable]
    public class CharacterDataList
    {
        public List<CharacterData> characterDatas;
    }
    #endregion

    [Serializable]
    public class CharacterData
    {
        public string npcID;
        public string name;
        public string textPath;
        public string SpritePath;
        public int normalDialogCount;
    }

    #endregion
}
