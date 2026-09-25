using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIBehavior : MonoBehaviour
{
    private UIDocument _document;
    private Button _startButton;
    private List<Button> _menuButtons = new List<Button>();

    private AudioSource _audioSource;
    public AudioClip AtriumUI1;
    public AudioClip AtriumUI2;

    private VisualElement _settingsContainer;
    private Button _settingsButton;
    private Button _closeButton;
    private Label _upgradeLabelA;
    private Label _upgradeLabelB;
    private Button _upgradeButtonA;
    private Button _upgradeButtonB;
    private Label _waveCount;
    private Label _baseHealth;


    [SerializeField] private int upgradeLevelA = 0;
    [SerializeField] private int upgradeLevelB = 0;
    [SerializeField] private int wave = 0;
    [SerializeField] private int baseHealthAmount = 100;


    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _document = GetComponent<UIDocument>();

        _closeButton = _document.rootVisualElement.Q<Button>("CloseButton");
        _settingsButton = _document.rootVisualElement.Q<Button>("SettingsButton");
        _settingsContainer = _document.rootVisualElement.Q<VisualElement>("SettingsContainer");

        _upgradeLabelA = _document.rootVisualElement.Q<Label>("UpgradeLabelA");
        _upgradeLabelB = _document.rootVisualElement.Q<Label>("UpgradeLabelB");
        _upgradeButtonA = _document.rootVisualElement.Q<Button>("UpgradeA");
        _upgradeButtonB = _document.rootVisualElement.Q<Button>("UpgradeB");
        _waveCount = _document.rootVisualElement.Q<Label>("WaveCount");
        _baseHealth = _document.rootVisualElement.Q<Label>("HealthAmount");

        _closeButton.RegisterCallback<ClickEvent>(OnCloseButtonClick);
        _settingsButton.RegisterCallback<ClickEvent>(OnSettingsButtonClick);
        _settingsContainer.AddToClassList("SettingsHide");
        _upgradeButtonA.RegisterCallback<ClickEvent>(OnUpgradeAClicked);
        _upgradeButtonB.RegisterCallback<ClickEvent>(OnUpgradeBClicked);


        _startButton = _document.rootVisualElement.Q<Button>("StartButton");
        _startButton.RegisterCallback<ClickEvent>(OnStartButtonClick);

        UpdateUpgradeLabelA();
        UpdateUpgradeLabelB();
    }

    // Keyboard controls
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Return))
        {
        //    OnStartButtonClick(null);
        }

        //if (Input.GetKeyDown(KeyCode.S))
        {
        //    OnSettingsButtonClick(null);
        }

        //if (Input.GetKeyDown(KeyCode.Escape))
        {
        //    OnCloseButtonClick(null);
        }

        //if (Input.GetKeyDown(KeyCode.E))
        {
        //    OnUpgradeAClicked(null);
        }

        //if (Input.GetKeyDown(KeyCode.F))
        {
        //    OnUpgradeBClicked(null);
        }

        _waveCount.text = "" + wave;

        _baseHealth.text = "" + baseHealthAmount;

    }

    private void OnDisable()
    {
        _startButton.UnregisterCallback<ClickEvent>(OnStartButtonClick);
        _settingsButton.UnregisterCallback<ClickEvent>(OnSettingsButtonClick);
        _closeButton.UnregisterCallback<ClickEvent>(OnCloseButtonClick);
        _upgradeButtonA.UnregisterCallback<ClickEvent>(OnUpgradeAClicked);
        _upgradeButtonB.UnregisterCallback<ClickEvent>(OnUpgradeBClicked);
    }


    private void OnUpgradeAClicked(ClickEvent evnt)
    {
        if (upgradeLevelA < 5)
        {
            upgradeLevelA++;
            UpdateUpgradeLabelA();
            ValidClick();

            if (upgradeLevelA == 5)
            {
                _upgradeButtonA.AddToClassList("upgradeButtonInv");
            }
        }
        else
        {
            InvalidClick();
        }
    }

    private void OnUpgradeBClicked(ClickEvent evnt)
    {
        if (upgradeLevelB < 5)
        {
            upgradeLevelB++;
            UpdateUpgradeLabelB();
            ValidClick();

            if (upgradeLevelB == 5)
            {
                _upgradeButtonB.AddToClassList("upgradeButtonInv");
            }
        }
        else
        {
            InvalidClick();
        }
    }

    private void UpdateUpgradeLabelA()
    {
        _upgradeLabelA.text = "upgrade" + upgradeLevelA;
    }

    private void UpdateUpgradeLabelB()
    {
        _upgradeLabelB.text = "upgrade" + upgradeLevelB;
    }


    private void OnStartButtonClick(ClickEvent evnt)
    {
        ValidClick();
    }

    private void OnSettingsButtonClick(ClickEvent evnt)
    {
        ValidClick();
        if (_settingsContainer.ClassListContains("SettingsHide"))
            _settingsContainer.RemoveFromClassList("SettingsHide");
        else
            _settingsContainer.AddToClassList("SettingsHide");
    }

    private void OnCloseButtonClick(ClickEvent evnt)
    {
        Debug.Log("Close");
    }


    // UI Sound Effects
    private void ValidClick()
    {
        _audioSource.PlayOneShot(AtriumUI1);
    }

    private void InvalidClick()
    {
        _audioSource.PlayOneShot(AtriumUI2);
    }

}
