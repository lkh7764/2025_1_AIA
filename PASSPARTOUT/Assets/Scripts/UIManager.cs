using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public enum ButtonType
{
    NONE = 0,
    START,  EXIT,           // TITLE
    NEWRQ,                  // STATIC
    REFUSE, ACCEPT,         // REQUEST
    GIVEUP, DONE,   SHOWRQ  // DRAW 
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("TITLE")]
    public GameObject   titleUIObj;
    // buttons
    public Button       startButton;
    public Button       exitButton;

    [Header("STATIC")] 
    public GameObject   staticUIObj;
    // status
    public GameObject   statusViewerObj;
    public Text         moneyText;
    public Text         fameText;    
    // newRequest
    public GameObject   newReqObj;
    public Button       newReqButton;

    [Header("REQUEST")]
    public GameObject   requestUIObj;
    // texts
    public Text         questText;
    public Text         cnameText;
    // buttons
    public Button       refuseButton;
    public Button       acceptButton;

    [Header("DRAW")]
    public GameObject   drawUIObj;
    // color buttons
    public Transform    colorButtonParent;
    public List<Button> colorButtons;
    public int          selectedColorIndex;
    // brush size
    public Slider       brushSizeSlider;
    public Vector3      initBrushSize; // x: defuault | y: min | z: max
    // request viewer
    public Button       showReqViewButton;
    public bool         reqViewActive;
    public GameObject   reqViewObj;
    public Text         rvQuestText;
    public Text         rvCnameText;
    // buttons
    public Button       giveUpButton;
    public Button       doneButton;
    // sceneCanvse
    public RawImage     drawCanvas;
    public RawImage     sceneCanvas;
    public Texture2D    canvasTex;
    private Texture2D   emptyTex;

    private DrawCanvas  drawHandler;
    private FlaskSession fSession;



    // buttons
    private Dictionary<ButtonType, Button> buttons;
    private void initButtons()
    {
        buttons = new Dictionary<ButtonType, Button>()
        {
            {ButtonType.START,  startButton},
            {ButtonType.EXIT,   exitButton},
            {ButtonType.NEWRQ,  newReqButton},
            {ButtonType.REFUSE, refuseButton},
            {ButtonType.ACCEPT, acceptButton},
            {ButtonType.GIVEUP, giveUpButton},
            {ButtonType.DONE,   doneButton},
            {ButtonType.SHOWRQ, showReqViewButton}
        };
    }
    public void AddButtonProc(ButtonType type, Action proc)
    {
        if (!buttons.ContainsKey(type))
        {
            Debug.Log("there's no key " + type);
            return;
        }

        buttons[type].onClick.AddListener(new UnityEngine.Events.UnityAction(proc));
    }

    // button proc
    private void startButtonUIProc()
    {
        titleUIObj.SetActive(false);
        staticUIObj.SetActive(true);
    }
    private void exitButtonUIProc() { }
    private void newReqButtonUIProc()
    {
        newReqObj.SetActive(false);
        requestUIObj.SetActive(true);
    }
    private void refuseButtonUIProc()
    {
        requestUIObj.SetActive(false);
    }
    private void acceptButtonUIProc()
    {
        requestUIObj.SetActive(false);

        initDrawUI();
        drawUIObj.SetActive(true);
    }
    private void showReqButtonUIProc()
    {
        SetActiveReqView(!reqViewActive);
    }
    private void giveUpButtonUIProc()
    {
        drawUIObj.SetActive(false);
        sceneCanvas.texture = emptyTex;
    }
    private void doneButtonUIProc()
    {
        drawUIObj.SetActive(false);
        sceneCanvas.texture = emptyTex;

        // send
        Rect r = drawCanvas.rectTransform.rect;
        fSession.SendImgToCNN(canvasTex, "test", (int)r.width, (int)r.height);
    }



    // static ui proc
    public void UpdateMoneyText(int money)      { moneyText.text = money.ToString(); }
    public void UpdateFameText(int fame)        { fameText.text = fame.ToString(); }
    public void SetActiveNewReq(bool active)    { newReqObj.SetActive(true); }
    


    // request ui proc
    public void UpdateQuestText(string txt) { questText.text = txt; }
    public void UpdateCNameText(string txt) { cnameText.text = txt; }



    // draw ui proc
    private void initDrawUI()
    {
        // init color
        selectedColorIndex = -1;
        foreach (var button in colorButtons) button.interactable = true;


        // init brush size
        brushSizeSlider.value = initBrushSize.x;
        drawHandler.ChangeBrushColor(Color.white);
        drawHandler.ChangeBrushSize((int)initBrushSize.x);


        // init request viewer
        SetActiveReqView(false);
        rvQuestText.text = questText.text;
        rvCnameText.text = cnameText.text;


        // init canvas
        Rect r = drawCanvas.rectTransform.rect;
        int width = (int)r.width;
        int height = (int)r.height;
        canvasTex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        
        Color[] pxColors = new Color[width * height];
        for (int i = 0; i < pxColors.Length; ++i) pxColors[i] = Color.white;
        canvasTex.SetPixels(pxColors);

        canvasTex.Apply();

        drawCanvas.texture = canvasTex;
        sceneCanvas.texture = canvasTex;
        drawHandler.AllocateNewTexture(canvasTex);
    }
    // color button
    private void changeSelectedColor(int index)
    {
        int prev = selectedColorIndex;
        selectedColorIndex = index;

        if (prev > 0 && prev < colorButtons.Count - 1) colorButtons[prev].interactable = true;
        colorButtons[selectedColorIndex].interactable = false;

        // get component drawing and change color
        drawHandler.ChangeBrushColor(colorButtons[selectedColorIndex].GetComponent<Image>().color);
    }
    public void SetActiveReqView(bool active)
    {
        reqViewActive = active;

        if (active) showReqViewButton.Select();
        else EventSystem.current.SetSelectedGameObject(null);

        reqViewObj.SetActive(active);
    }



    // init singleton
    private void initiate()
    {
        if (Instance != null)
        {
            Debug.Log("UIManager is already instiated!");
            Destroy(gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }



    // life cycle
    private void Awake()
    {
        initiate();
        initButtons();
    }
    private void Start()
    {
        // title
        buttons[ButtonType.START].onClick.AddListener(() => startButtonUIProc());
        buttons[ButtonType.EXIT].onClick.AddListener(() => exitButtonUIProc());

        // static
        staticUIObj.SetActive(false);
        buttons[ButtonType.NEWRQ].onClick.AddListener(() => newReqButtonUIProc());

        // request
        requestUIObj.SetActive(false);
        buttons[ButtonType.REFUSE].onClick.AddListener(() => refuseButtonUIProc());
        buttons[ButtonType.ACCEPT].onClick.AddListener(() => acceptButtonUIProc());

        // draw
        drawUIObj.SetActive(false);
        colorButtons = new List<Button>(colorButtonParent.GetComponentsInChildren<Button>(true));
        for (int i = 0; i<colorButtons.Count; ++i)
        {
            int index = i; 
            colorButtons[i].onClick.AddListener(() => changeSelectedColor(index));
        }
        buttons[ButtonType.SHOWRQ].onClick.AddListener(() => showReqButtonUIProc());
        buttons[ButtonType.GIVEUP].onClick.AddListener(() => giveUpButtonUIProc());
        buttons[ButtonType.DONE].onClick.AddListener(() => doneButtonUIProc());

        brushSizeSlider.minValue = initBrushSize.y;
        brushSizeSlider.maxValue = initBrushSize.z;
        brushSizeSlider.onValueChanged.AddListener((float v) => drawHandler.ChangeBrushSize((int)v));

        Rect r = drawCanvas.rectTransform.rect;
        int width = (int)r.width;
        int height = (int)r.height;
        emptyTex = new Texture2D(width, height, TextureFormat.RGBA32, false);

        Color[] pxColors = new Color[width * height];
        for (int i = 0; i < pxColors.Length; ++i) pxColors[i] = Color.white;
        emptyTex.SetPixels(pxColors);

        emptyTex.Apply();

        drawHandler = drawCanvas.GetComponent<DrawCanvas>();
        fSession = FindObjectOfType<FlaskSession>();
    }
}
