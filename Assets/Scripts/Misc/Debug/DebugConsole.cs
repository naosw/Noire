using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    [SerializeField] private float boxHeight = 50;
    [SerializeField] private float textboxHeight = 40;
    [SerializeField] private int textFont = 30;
    [SerializeField] private Texture2D boxTex;

    public static DebugCommand<int> DECREASE_HP;
    public static DebugCommand<(int, float)> DECREASE_HP_CONTINUOUS;
    public static DebugCommand<int> INCREASE_HP;
    public static DebugCommand KILL;
    public static DebugCommand SAVE;
    public static DebugCommand INFINITE_HP;
    public static DebugCommand INFINITE_STAMINA;

    public List<object> cmdList;
    
    private bool showConsole;
    private string input;
    private GUIStyle textStyle = new();
    private GUIStyle boxStyle = new();

    private void Awake()
    {
        DECREASE_HP = new DebugCommand<int>("dec_hp", "decreases player hp by x", "dec_hp <x>", (x) =>
        {
            GameEventsManager.Instance.PlayerEvents.TakeDamage(x, Vector3.zero);
        });
        
        DECREASE_HP_CONTINUOUS = new DebugCommand<(int, float)>("dec_hp_cont", "decreases player hp by x every s seconds", "dec_hp <x> <s>", (x) =>
        {
            StartCoroutine(dec_hp_cont(x.Item1, x.Item2));
        });
        
        INCREASE_HP = new DebugCommand<int>("inc_hp", "increases player hp by x", "inc_hp <x>", (x) =>
        {
            GameEventsManager.Instance.PlayerEvents.RegenHealth(x);
        });
        
        KILL = new DebugCommand("kill", "kills the player", "kill", () =>
        {
            GameEventsManager.Instance.PlayerEvents.TakeDamage(Int32.MaxValue, Vector3.zero);
        });
        
        SAVE = new DebugCommand("save", "saves the game", "save", () =>
        {
            DataPersistenceManager.Instance.SaveGame();
        });
        
        INFINITE_HP = new DebugCommand("inf_hp", "make the player's hp infinite", "inf_hp", () =>
        {
            Player.Instance.SetMaxHP(Int32.MaxValue);
        });
        
        INFINITE_STAMINA = new DebugCommand("inf_stamina", "make the player's stamina infinite", "inf_stamina", () =>
        {
            Player.Instance.SetMaxStamina(Single.MaxValue);
        });

        cmdList = new List<object>
        {
            DECREASE_HP,
            DECREASE_HP_CONTINUOUS,
            INCREASE_HP,
            KILL,
            SAVE,
            INFINITE_HP,
            INFINITE_STAMINA
        };
    }

    private IEnumerator dec_hp_cont(int value, float seconds)
    {
        while (true)
        {
            yield return null;
            GameEventsManager.Instance.PlayerEvents.TakeDamage(value, Vector3.zero);
            yield return new WaitForSeconds(seconds);
        }
    }

    private void Start()
    {
        GameInput.Instance.OnDebugConsoleToggle += OnToggleDebug;
        GameInput.Instance.OnDebugConsoleExecute += OnConsoleExecute;

        textStyle.fontSize = textFont;
        textStyle.normal.textColor = Color.white;

        boxStyle.normal.background = boxTex;
    }

    private void OnDisable()
    {
        GameInput.Instance.OnDebugConsoleToggle -= OnToggleDebug;
        GameInput.Instance.OnDebugConsoleExecute -= OnConsoleExecute;
    }

    private void OnToggleDebug()
    {
        showConsole = !showConsole;
        input = "";
    }

    private void OnConsoleExecute()
    {
        if (showConsole)
        {
            HandleInput();
            input = "";
        }
    }

    private void OnGUI()
    {
        if (!showConsole) return;

        float y = Screen.height - boxHeight;
        GUI.Box(new Rect(10, y, Screen.width, boxHeight),"", boxStyle);
        input = GUI.TextField(new Rect(40, y + 13f, Screen.width, textboxHeight), input, textStyle);
    }

    private void HandleInput()
    {
        string[] properties = input.Split(' ');
        
        foreach (var cmd in cmdList)
        {
            var cmdBase = cmd as DebugCommandBase;
            if (properties.Contains(cmdBase.cmdId))
            {
                if(cmd is DebugCommand)
                    (cmd as DebugCommand).Invoke();
                else if (cmd is DebugCommand<int>)
                    (cmd as DebugCommand<int>).Invoke(int.Parse(properties[1]));
                else if (cmd is DebugCommand<(int, float)>)
                    (cmd as DebugCommand<(int, float)>).Invoke((int.Parse(properties[1]), float.Parse(properties[2])));

                break;
            }
        }
    }
}