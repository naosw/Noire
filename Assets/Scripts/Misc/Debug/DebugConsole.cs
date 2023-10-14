using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugConsole : MonoBehaviour
{
    [SerializeField] private float boxHeight = 50;
    [SerializeField] private float textboxHeight = 40;
    [SerializeField] private int textFont = 30;

    public static DebugCommand<int> DEC_HP;
    public static DebugCommand<(int, int)> DEC_HP_CONT;
    public static DebugCommand<int> INC_HP;

    public List<object> cmdList;
    
    private bool showConsole;
    private string input;
    private GUIStyle style = new GUIStyle();

    private void Awake()
    {
        DEC_HP = new DebugCommand<int>("dec_hp", "decreases player hp by x", "dec_hp <x>", (x) =>
        {
            GameEventsManager.Instance.PlayerEvents.TakeDamage(x);
        });
        
        DEC_HP_CONT = new DebugCommand<(int, int)>("dec_hp_cont", "decreases player hp by x every s seconds", "dec_hp <x> <s>", (x) =>
        {
            GameEventsManager.Instance.PlayerEvents.TakeDamage(x.Item1);
        });
        
        INC_HP = new DebugCommand<int>("inc_hp", "increases player hp by x", "inc_hp <x>", (x) =>
        {
            GameEventsManager.Instance.PlayerEvents.RegenHealth(x);
        });

        cmdList = new List<object>
        {
            DEC_HP,
            INC_HP,
        };
        
        
    }

    private void Start()
    {
        GameInput.Instance.OnDebugConsoleToggle += OnToggleDebug;
        GameInput.Instance.OnDebugConsoleExecute += OnConsoleExecute;

        style.fontSize = textFont;
        style.normal.textColor = Color.white;
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
        GUI.Box(new Rect(10, y, Screen.width, boxHeight),"");
        GUI.backgroundColor = new Color(0, 0, 0, 0);
        
        input = GUI.TextField(new Rect(10, y + 5f, Screen.width, textboxHeight), input, style);
    }

    private void HandleInput()
    {
        string[] properties = input.Split(' ');
        
        foreach (var cmd in cmdList)
        {
            var cmdBase = cmd as DebugCommandBase;
            if (input.Contains(cmdBase.cmdId))
            {
                (cmd as DebugCommand)?.Invoke();
                (cmd as DebugCommand<int>)?.Invoke(int.Parse(properties[1]));
            }
            
        }
    }
}