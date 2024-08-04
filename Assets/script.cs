using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using System.Text.RegularExpressions;


public class script : MonoBehaviour {


	public KMAudio Audio;
    public AudioClip[] SFX;
    //0-9 are 0-9
    //10-19 are 10-19
    //20 is Stage, 21 is is
    //22-29 are 20-90
    //30 is ringing
    public KMBombInfo info;
    public KMBombModule Module;
	public KMSelectable[] Buttons;
	public TextMesh StageText, StageNumberText, IsText;
    public KMBossModule bossModuleHandler;
    public Material bg;
    private string[] ignoredModules;

    private int solvables;
    static int ModuleIdCounter;
    int ModuleId;
    private int currentStage, prevStage;
    private bool canPlay;
    private bool readyToSolve;
    public Color colorB, colorR;
    private int stages;
    private int prev = 1, prevF = 1, curr = 1, currF = 1;

    private int mod(int A, int b)
    {
        if (b > 0)
        {
            if (A >= 0) while (A >= b) A -= b;
            else while (A < 0) A += b;
            return A;
        }
        else if (b < 0)
        {
            if (A < 0) while (A <= b) A -= b;
            else while (A > 0) A += b;
            return A;
        }
        else return 0;
    }

    /// <summary>
    /// Returns CURRF from CURR, PREVF and PREV
    /// </summary>
    /// <param name="cs">Operation number</param>
    /// <param name="a">CURR</param>
    /// <param name="b">PREVF</param>
    /// <param name="c">PREV</param>
    /// <returns></returns>
    int getStage(int cs, int a, int b, int c)
    {
        a = a == 0 ? 1 : a;
        b = b == 0 ? 1 : b;
        c = c == 0 ? 1 : c;
        switch (mod(cs,11))
        {
            case 0: return mod((a + mod(2*b, a)+mod(4*c, b)), 10000);
            case 1: return mod(b + mod(3 * c, b) + (a * b / c),10000);
            case 2: return (int)(Math.Log(c * (1 + b * (1 + a))) / Math.Log(2) + Math.Log(a * (1 + b * (1 + c))));
            case 3: return mod((int)Math.Abs(a * Math.Sin(b + c) / Math.Cos(b * c)), 10000);
            case 4: return (int)Math.Abs(c * Math.Cos(Math.Log(a*b)/Math.Log(2)));
            case 5: return mod(mod(mod(-a, b), mod(b,(Math.Abs(c-a)/ 2))) * (a + b + c), 10000);
            case 6: return mod((int)(Math.Pow(a*b*c,1d/3)+Math.Sqrt((a*a+b*b+c*c)/9)+(a+b+c)/3),10000);
            case 7: return mod((int)(3*a*(1-Math.Exp(-b/c))),10000);
            case 8: return mod((int)(Math.Pow(Math.Sqrt(a) + Math.Sqrt(b) + Math.Sqrt(c), Math.Exp(1))), 10000);
            case 9: return mod(3/(1/a+1/b+1/c),10000);
            case 10:
                {
                    int m = mod(b * c, 9),
                        n = mod(a * c, 9),
                        p = mod(a * b, 9);
                    m = (m == 0 ? 9 : m)+1;
                    n = (n == 0 ? 9 : n)+1;
                    p = (p == 0 ? 9 : p)+1;
                    return mod((int)(a*p*Math.Log(n)/Math.Log(m)),10000);
                }
            default: return 69; // i love c#
        }
    }

    void generateSol()
    {

    }

    bool HandlePress(int num)
    {
        if (readyToSolve)
        {
            StageNumberText.text += (char)('0' + num);
            if (StageNumberText.text.Length == 4)
            {
                if (StageNumberText.text == currF.ToString())
                {
                    Module.HandlePass();
                }
                else
                {
                    Module.HandleStrike();
                    // blah blah blah
                    bg.color = colorR;
                }
            }
        }
        else
        {
            Audio.PlaySoundAtTransform(SFX[0].name, transform);
        }
        return false;
    }

    bool HandlePress1() { return HandlePress(1); }
    bool HandlePress2() { return HandlePress(2); }
    bool HandlePress3() { return HandlePress(3); }
    bool HandlePress4() { return HandlePress(4); }
    bool HandlePress5() { return HandlePress(5); }
    bool HandlePress6() { return HandlePress(6); }
    bool HandlePress7() { return HandlePress(7); }
    bool HandlePress8() { return HandlePress(8); }
    bool HandlePress9() { return HandlePress(9); }

    bool FinalCheck()
    {
        return false;
    }

    void NewStage()
    {
        prevStage = currentStage;
        StageNumberText.text = currentStage < 10 ? "0" + currentStage.ToString() : currentStage.ToString();

        //СЮДА ЗВУК ТЕЛЕФОНА ЗАХУЯРЬ
        //Audio.PlaySoundAtTransform(SFX[30].name, transform);            //НА БЛЯТЬ ТВОЙ ЗВУК ТЕЛЕФОНА ЗАЙБАЛ
        if (canPlay) Module.HandleStrike();
        else canPlay = true;
        prev = curr;
        prevF = currF;
        curr = UnityEngine.Random.Range(1, 10000);
        char sn = info.GetSerialNumber()[mod(prevF, 6)];
        int s = sn > '9' ? sn - 'A' + 10 : sn - '0';
        currF = getStage(mod(s+mod(curr,19),11), curr, prevF, prev);
    }

    void Start () {
        StageNumberText.text = "00";
        ModuleId = ModuleIdCounter++;
        currentStage = 0;
        prevStage = 0;
        readyToSolve = false;
        bg.color = colorB;
        Audio.PlaySoundAtTransform(SFX[30].name, transform);
        if (ignoredModules == null) {
            ignoredModules = bossModuleHandler.GetIgnoredModules("Rand Says", new string[] {
        "+",
        "14",
        "42",
        "501",
        "Access Codes",
        "Amnesia",
        "A>N<D",
        "Bamboozling Time Keeper",
        "Black Arrows",
        "Brainf---",
        "Busy Beaver",
        "Button Messer",
        "Cookie Jars",
        "Cube Synchronization",
        "Divided Squares",
        "Don't Touch Anything",
        "Encrypted Hangman",
        "Encryption Bingo",
        "Floor Lights",
        "Forget Any Color",
        "Forget Enigma",
        "Forget Everything",
        "Forget Infinity",
        "Forget It Not",
        "Forget Me Later",
        "Forget Me Not",
        "Forget Perspective",
        "Forget The Colors",
        "Forget Them All",
        "Forget This",
        "Forget Us Not",
        "Four-Card Monte",
        "The Heart",
        "Hogwarts",
        "Iconic",
        "Keypad Directionality",
        "The Klaxon",
        "Kugelblitz",
        "Multitask",
        "Mystery Module",
        "OmegaForget",
        "OmegaDestroyer",
        "Organization",
        "Purgatory",
        "Rand Says",
        "RPS Judging",
        "Security Council",
        "Shoddy Chess",
        "Simon",
        "Simon Forgets",
        "Simon's Stages",
        "Souvenir",
        "SuperBoss",
        "The Swan",
        "Tallordered Keys",
        "The Time Keeper",
        "Timing is Everything",
        "The Troll",
        "Turn The Key",
        "The Twin",
        "Übermodule",
        "Ultimate Custom Night",
        "The Very Annoying Button",
        "Whiteout",
        });
        }
        if (info != null) solvables = info.GetSolvableModuleNames().Where(a => !ignoredModules.Contains(a)).ToList().Count;
        else Module.HandlePass();
        if (!(solvables > 0)) Module.HandlePass();
        stages = solvables / 3;
        stages = stages > 99 ? 99 : stages;




        Buttons[0].OnInteract += HandlePress1;
        Buttons[1].OnInteract += HandlePress2;
        Buttons[2].OnInteract += HandlePress3;
        Buttons[3].OnInteract += HandlePress4;
        Buttons[4].OnInteract += HandlePress5;
        Buttons[5].OnInteract += HandlePress6;
        Buttons[6].OnInteract += HandlePress7;
        Buttons[7].OnInteract += HandlePress8;
        Buttons[8].OnInteract += HandlePress9;
    }

    void FixedUpdate()
    {
        int solved = 0;
        if (info != null) solved = info.GetSolvedModuleNames().Where(a => !ignoredModules.Contains(a)).ToList().Count;

        if (solved == solvables)
        {
            StageNumberText.text = "";
            readyToSolve = true;
            FinalCheck();
            return;
        }
        currentStage = solved / 3;

        if (currentStage != prevStage) NewStage();      //HURRY UP IT'S THE NEXT STAGE RING RING
    }

}
