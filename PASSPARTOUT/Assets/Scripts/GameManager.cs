using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private UIManager um;
    private CharacterAnimator anim;
    private FlaskSession fSession;

    private string LLMUrl = "http://localhost:5001/generate_request";
    private List<string> testtheme;

    private string theme;



    // init singleton
    private void initiate()
    {
        if (Instance != null)
        {
            Debug.Log("GameManager is already instiated!");
            Destroy(gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }



    private void Awake()
    {
        initiate();

        testtheme = new List<string>()
        {
            "rat", "dog", "pig", "horse", "tiger", "lion",
            "orange", "lemon", "tree", "pizza", "bee", "peach", "watermelon"
        };
    }
    private void Start()
    {
        um = UIManager.Instance;

        // add button proc - new req
        um.AddButtonProc(ButtonType.START,  () => createNewRequest());
        um.AddButtonProc(ButtonType.GIVEUP, () => createNewRequest());
        um.AddButtonProc(ButtonType.DONE,   () => createNewRequest());

        // add button proc - quit
        um.AddButtonProc(ButtonType.EXIT, () => quitGame());

        // add button proc - anim
        anim = CharacterAnimator.Instance;
        um.AddButtonProc(ButtonType.ACCEPT, () => anim.MakeTransition_draw(true));
        um.AddButtonProc(ButtonType.GIVEUP, () => anim.MakeTransition_draw(false));
        um.AddButtonProc(ButtonType.DONE,   () => anim.MakeTransition_draw(false));

        fSession = FindObjectOfType<FlaskSession>();
    }



    private void createNewRequest()
    {
        // flask에 llm로 새로운 의뢰서 데이터 받아오기
        StartCoroutine(requireRequestText());
    }
    private IEnumerator requireRequestText()
    {
        RequestText rt = new RequestText("");
        Coroutine cor = StartCoroutine(FetchRequest(rt));
        yield return cor;
    }
    public IEnumerator FetchRequest(RequestText rt)
    {
        theme = testtheme[UnityEngine.Random.Range(0, testtheme.Count)];
        TextPayload plContainer = new TextPayload(theme);
        string payload = JsonUtility.ToJson(plContainer);

        using var req = new UnityWebRequest(LLMUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(payload);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.Success)
        {
            rt = JsonUtility.FromJson<RequestText>(req.downloadHandler.text);
            Debug.Log(rt.request_text);

            string[] txts = rt.request_text.Trim().Split('"');

            string qtxt = txts[7];
            Debug.Log(qtxt);
            string ctxt = "의뢰인: ";
            ctxt += txts[11];

            um.UpdateQuestText(qtxt);
            um.UpdateCNameText(ctxt);

            um.SetActiveNewReq(true);
        }
        else Debug.Log($"Error {req.responseCode}: {req.error}");
    }
    private void quitGame()
    {
        Application.Quit();
    }
}
