using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : Singleton<GameManager>
{
    [Header("[Cat]")] public List<Cat> cats;
    public int catCount;

    [Header("[GameUI]")] public TextMeshProUGUI scoreText;
    public TextMeshPro countText;
    public TextMeshPro comboText;
    public CanvasGroup canvasGroup;
    public Image[] life;
    public GameObject countObj;
    [SerializeField] private GameObject floorObj;
    [SerializeField] private float downY;
    [SerializeField] private Image gaugeTop;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private Image flag;
    [SerializeField] private GameObject jewel;
    private float flagPosYStep = 40f;
    
    private VertexGradient gradient;

    [SerializeField] private GameObject[] backgrounds;

    public int score;
    public float currentTime;
    public bool isGameOver;
    public bool resumed;


    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI bestText;
    public TextMeshProUGUI feverTimeTitle;

    //game
    private int _maxLife = 5;
    private float _maxTime = 60;
    private GameObject _catPrefabObj;
    private Cat _catPrefabObjScript;
    private int _currentLevel;
    private int _currentLife;
    private bool _jewelCheck;

    public ConstInfo constInfo;
    private ObjectPoolManager _pool;
    private UIManager _um;
    private AudioManager _am;
    public ItemManager _im;

    private bool hasSpawnedThisRound;
    private int bestscore;
    public int comboCount;
    public int comboCountReal;

    private void Start()
    {
        _pool = ObjectPoolManager.Instance;
        _um = UIManager.Instance;
        _am = AudioManager.Instance;
        constInfo = new ConstInfo();

        Time.timeScale = 0f;
    }

    public int InitSetting()
    {
        //ui init
        bestscore = PlayerPrefs.GetInt("bestscore");
        catCount = 0;
        score = 0;
        currentTime = 0;
        flag.rectTransform.anchoredPosition = Vector3.zero;
        jewel.SetActive(false);
        Gradient();

        //game init
        scoreText.text = "점수 : " + score;
        _currentLife = _maxLife;
        gaugeTop.fillAmount = 1f;
        downY = 1f;
        countObj.SetActive(false);
        comboText.gameObject.SetActive(false);
        isGameOver = false;


        for (int i = 0; i < _maxLife; i++)
        {
            life[i].transform.localScale = Vector3.one;
        }

        floorObj.transform.position = new Vector3(0, 0, 0);
        Color c = floorObj.GetComponent<Renderer>().material.color;
        c.a = 1f;
        floorObj.GetComponent<Renderer>().material.color = c;

        //level init
        ComboReset();

        //cat init
        int selectedIndex = PlayerPrefs.GetInt("selectedCatIndex", -1);
        if (selectedIndex == -1) // 선택된 고양이 인덱스가 없어 -1이라면 return으로 빠져나옴
            return selectedIndex;
        constInfo.CatIndexInit(selectedIndex);
        Debug.Log(selectedIndex);

        cats = new List<Cat>();
        cats.Clear();
        _pool.SpawnCatSetting(selectedIndex);
        StartCoroutine(SpawnCat());

        return selectedIndex;
    }

    private void Update()
    {
        if (_um.IsPaused) return;

        Vector2 touchPos = Vector2.zero;
        bool touchDown = false;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        if (Input.GetMouseButtonDown(0))
        {
            touchDown = true;
            touchPos = Input.mousePosition;
        }
#else
    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
    {
        touchDown = true;
        touchPos = Input.GetTouch(0).position;
    }
#endif

        if (touchDown)
        {
            if (IsPointerOverUIPos(touchPos)) return;
            TryJump();
        }

        if (!isGameOver)
        {
            UpdateTimeUI();
        }
    }

    void TryJump()
    {
        if (_catPrefabObjScript is null) return;
        if (!_catPrefabObjScript.IsJumping)
            _catPrefabObjScript.Jumping();
    }

    bool IsPointerOverUIPos(Vector2 screenPos)
    {
        if (EventSystem.current == null) return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = screenPos;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }


    public IEnumerator SpawnCat()
    {
        yield return new WaitForSeconds(0.8f);
        _catPrefabObj = _pool.GetPrefabObj(_pool.catPrefabObjQueue, _pool.catPrefabObj[constInfo.CurrentCatIndex],
            _pool.catPrefabObjParent);
        _catPrefabObjScript = _catPrefabObj.GetComponent<Cat>();
        _catPrefabObjScript.IsJumping = false;
        _catPrefabObjScript.Init(() => StartCoroutine(SpawnCat()));
        _catPrefabObjScript.Swapping(constInfo.CurrentSwappingDur);
        _catPrefabObj.transform.position = new Vector3(0, _catPrefabObj.transform.position.y, 0);

        SpawnItem();
    }

    private void SpawnItem()
    {
        if (catCount == 0) return;
        if (catCount % 4 == 0 && !hasSpawnedThisRound)
        {
            hasSpawnedThisRound = true;
            int rand = Random.Range(0, 6);
            if (rand >= 3 && !_im.timeObj.activeInHierarchy)
                _im.SpawnItem(_im.timeObj);
            else if (rand <=2 && !_im.minusObj.activeInHierarchy)
                _im.SpawnItem(_im.minusObj);
            // else if (rand is 2 or 1 && !_im.pointObj.activeInHierarchy)
            //     _im.SpawnItem(_im.pointObj);
        }
        else
        {
            hasSpawnedThisRound = false;
        }
    }


    public IEnumerator DownCtrl()
    {
        yield return new WaitForSeconds(0.3f);
        foreach (Cat cat in cats)
        {
            cat.transform.DOMoveY(cat.transform.position.y - downY, 0.3f);
        }

        foreach (GameObject obj in backgrounds)
        {
            if(obj.transform.position.y<-10)
                obj.transform.position = new Vector3(0, 20, 0);
            obj.transform.DOMoveY(obj.transform.position.y - downY, 0.3f);
        }

        Color c = floorObj.GetComponent<Renderer>().material.color;
        if (c.a != 0)
            floorObj.transform.DOMoveY(floorObj.transform.position.y - downY, 0.3f).OnComplete(() =>
            {
                if (catCount > 5)
                {
                    Color c = floorObj.GetComponent<Renderer>().material.color;
                    c.a = 0;
                    floorObj.GetComponent<Renderer>().material.color = c;
                }
                
            });

        if (_im.timeObj.activeSelf)
            _im.timeObj.transform.DOMoveY(_im.timeObj.transform.position.y - downY, 0.3f);
        
        if (_im.minusObj.activeSelf)
            _im.minusObj.transform.DOMoveY(_im.minusObj.transform.position.y - downY, 0.3f);
        
        // if (_im.pointObj.activeSelf)
        //     _im.pointObj.transform.DOMoveY(_im.pointObj.transform.position.y - downY, 0.3f);

        if (comboText.gameObject.activeSelf)
            comboText.transform.DOMoveY(comboText.transform.position.y - downY, 0.3f);
        
        if (countObj.activeSelf)
            countObj.transform.DOMoveY(countObj.transform.position.y - downY, 0.3f);
    }

    private void GameOver()
    {
        isGameOver = true;
        _um.OpenOverPanel();
        
        resultText.text = score.ToString();

        var result = Mathf.Max(bestscore, score);
        PlayerPrefs.SetInt("bestscore", result);
        PlayerPrefs.Save();
        bestText.text = PlayerPrefs.GetInt("bestscore").ToString();
    }

    public void CountCat(GameObject currentCat)
    {
        cats.Add(currentCat.GetComponent<Cat>());
        catCount += 1;
        
        if (currentCat.transform.position.x > 1.1f)
        {
            countObj.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            countText.transform.localRotation = Quaternion.Euler(0f, -180f, 0f);
            countObj.transform.position = currentCat.transform.position + new Vector3(-1f, 1f, transform.position.z);
        }
        else
        {
            countObj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            countText.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            countObj.transform.position = currentCat.transform.position + new Vector3(1f, 1f, transform.position.z);
        }

        countText.text = catCount.ToString();
        countObj.SetActive(true);
        DOVirtual.DelayedCall(0.5f, () => { countObj.SetActive(false); });
    }

    //UI
    public void DecreaseLife()
    {
        if (_currentLife <= 0)
        {
            GameOver();
            return;
        }

        _currentLife--;
        UpdateLifeUI();
    }


    private void UpdateTimeUI()
    {
        if (currentTime <=0)
            currentTime = 0;
        currentTime += Time.deltaTime;

        float ratio = Mathf.Clamp01(1f - (currentTime / _maxTime));
        gaugeTop.fillAmount = ratio;

        if (ratio < 0.3f)
        {
            gaugeTop.GetComponent<Image>().color = new Color32(217, 87, 99, 255);
        }

        else
        {
            gaugeTop.GetComponent<Image>().color = new Color32(250, 241, 53, 255);
        }

        if (ratio <= 0f)
        {
            GameOver();
        }
    }
    
    public void UpdateCountUI()
    {
        if (flag.rectTransform.anchoredPosition.y < 280f)
        {
            flag.rectTransform.anchoredPosition += new Vector2(0, flagPosYStep);
        }
        
        if (flag.rectTransform.anchoredPosition.y is 280f && !_jewelCheck)
        {
            _am.OnSfxPlay(5);
            _jewelCheck = true;
            jewel.SetActive(true);
        }
        
    }

    private void UpdateLifeUI()
    {
        life[_currentLife].transform.DOScale(0f, 0.3f);
    }


    public void ComboInit(GameObject currentCat)
    {
        comboCount++;
        comboCountReal++;

        comboCount = constInfo.LevelStep(comboCount);
        constInfo.LevelInit(comboCount);
        
        score += 100+ 100 * (comboCountReal / 10);
        scoreText.text = "점수 : " + score;

        switch (comboCountReal / 10)
        {
            case 0:
                comboText.color = new Color32(255, 96, 106, 255);
                comboText.enableVertexGradient = false;
                break;
            case 1:
                comboText.color = new Color32(255, 153, 17, 255);
                comboText.enableVertexGradient = false;
                break;
            case 2:
                comboText.color = new Color32(255, 212, 70, 255);
                comboText.enableVertexGradient = false;
                break;
            case 3:
                comboText.color = new Color32(90, 172, 108, 255);
                comboText.enableVertexGradient = false;
                break;
            case 4:
                comboText.color = new Color32(90, 172, 255, 255);
                comboText.enableVertexGradient = false;
                break;
            case 5:
                comboText.color = new Color32(56, 75, 187, 255);
                comboText.enableVertexGradient = false;
                break;
            case 6:
                comboText.color = new Color32(134, 89, 171, 255);
                comboText.enableVertexGradient = false;
                break;
            case 7:
                comboText.color = Color.white;
                comboText.colorGradient = gradient;
                comboText.enableVertexGradient = true;
                break;
            default:
                comboText.color = Color.white;
                comboText.colorGradient = gradient;
                comboText.enableVertexGradient = true;
                break;
        }
        
        comboText.transform.position = currentCat.transform.position;
        comboText.gameObject.SetActive(true);
        DOVirtual.DelayedCall(0.5f, () =>
        {
            comboText.gameObject.SetActive(false);
            
        });
        
        
        if (comboCountReal>9 && comboCountReal % 10 == 0 && isGameOver == false)
        {
            comboText.color = Color.white;
            comboText.colorGradient = gradient;
            comboText.enableVertexGradient = true;
            
            pauseButton.gameObject.SetActive(false);
            
            comboText.text = "콤보 맥스";
            var pos = currentCat.transform.position;
            
            DOVirtual.DelayedCall(0.5f, () =>
            {
                if (isGameOver) return;
                Time.timeScale = 0f;
                Animator animator = currentCat.GetComponent<Animator>();
                animator.updateMode = AnimatorUpdateMode.UnscaledTime;
                animator.SetTrigger("Fever");
                _am.OnSfxPlay(3);

                currentCat.transform.DOMoveX(4f, 2.5f).SetUpdate(true).OnComplete(() =>
                {
                    currentCat.transform.position = new Vector3(-2.3f, currentCat.transform.position.y,
                        currentCat.transform.position.z);
                    currentCat.transform.DOMoveX(pos.x, 1f).SetUpdate(true).OnComplete(() =>
                    {
                        _am.OnSfxStop();
                        animator.SetTrigger("Land");
                    });
                    OnFeverTime();
                });
            });
        }
        else
        {
            comboText.text = $"콤보 {comboCountReal}";
        }

       
    }

    public void ComboReset()
    {
        constInfo.LevelInit(0);
        comboCount = 0;
        comboCountReal = 0;
    }

    public void Gradient()
    {
        // Gradient 생성
        gradient = new VertexGradient(
            new Color32(200, 75, 75, 255), // topLeft (밝은 빨강)
            new Color32(255, 212, 9, 255), // topRight (밝은 주황)
            new Color32(90, 170, 108, 255), // bottomLeft (하늘색)
            new Color32(90, 172, 255, 255)
        );
    }

    private void OnFeverTime()
    {
        Gradient();
        feverTimeTitle.colorGradient = gradient;
        feverTimeTitle.enableVertexGradient = true;
        _um.OpenFeverPanel();
        _am.OnBgmPlay(1);

        // 깜빡이기: CanvasGroup으로 알파 조절!
        canvasGroup.alpha = 1f;
        Tween tween = canvasGroup.DOFade(0f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true);

        DOVirtual.DelayedCall(5f, () =>
        {
            _um.CloseFeverPanel();
            Time.timeScale = 1f;
            _am.OnBgmPlay(3);
            pauseButton.gameObject.SetActive(true);
            tween.Kill(true);
        });
    }
    
}