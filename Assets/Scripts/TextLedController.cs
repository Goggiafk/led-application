using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Android;

public class TextLedController : MonoBehaviour
{
    [SerializeField] private FlexibleColorPicker fcp;
    [SerializeField] private TMP_InputField inputArea;
    [SerializeField] private Slider scrollSpeedArea;
    [SerializeField] private TMP_InputField deviceName;

    [SerializeField] private TextMeshProUGUI pairedStatus;
    [SerializeField] private TextMeshProUGUI connectionStatus;


    [SerializeField] private Slider powerSlider;

    [SerializeField] private GameObject startingOptions;
    [SerializeField] private GameObject[] appWindows;
    [SerializeField] private GameObject[] appButtons;
    [SerializeField] private GameObject[] bluetoothElements;
    [SerializeField] private GameObject battery;
    [SerializeField] private GameObject[] batteryLevels;

    private bool isMenuActive;
    private bool IsConnected;
    private bool isTimer;
    private bool hasSecondPassed;
    private bool sendBatteryStatus = true;
    public static string dataRecived = "";
    public string rgbArray = "";
    public int editId = -1;
    // Start is called before the first frame update

    private void Awake()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation)
  || !Permission.HasUserAuthorizedPermission(Permission.FineLocation)
  || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_SCAN")
  || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_ADVERTISE")
  || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_CONNECT"))
            Permission.RequestUserPermissions(new string[] {
    Permission.CoarseLocation,
    Permission.FineLocation,
    "android.permission.BLUETOOTH_SCAN",
    "android.permission.BLUETOOTH_ADVERTISE",
    "android.permission.BLUETOOTH_CONNECT"
  });
    }

    void Start()
    {
        LoadBattery();
        if (PlayerPrefs.HasKey("device"))
            deviceName.text = PlayerPrefs.GetString("device");
        if (PlayerPrefs.HasKey("badgeText"))
            inputArea.text = PlayerPrefs.GetString("badgeText");
        if (PlayerPrefs.HasKey("scrollValue"))
            scrollSpeedArea.value = PlayerPrefs.GetFloat("scrollValue");
        if (!PlayerPrefs.HasKey("savesQuantity"))
            PlayerPrefs.SetInt("savesQuantity", 0);
        SetAllElementsActive(false);
        isMenuActive = false;
        IsConnected = false;
        BluetoothService.CreateBluetoothObject();
    }

    void Update()
    {
        if (IsConnected)
        {
            try
            {
                string datain = BluetoothService.ReadFromBluetooth();
                if (datain.Length > 1)
                {
                    dataRecived = datain;
                    //print(dataRecived);
                    if (dataRecived == "medium" || dataRecived == "low" || dataRecived == "high" || dataRecived == "CHG") SetBatteryLevel(dataRecived);
                }

                if (isMenuActive == false)
                {
                    PlayerPrefs.SetString("device", deviceName.text.ToString());
                    SetAllElementsActive(true);
                    isMenuActive = true;
                }
            }
            catch (Exception e)
            {
                IsConnected = false;
            }
        }
        else if (isMenuActive == true)
        {
            SetAllElementsActive(false);
            isMenuActive = false;
        }

        connectionStatus.text = "Connection Status: " + IsConnected;

        if (isTimer && hasSecondPassed && IsConnected)
        {
            hasSecondPassed = false;
            StartCoroutine(Timer(1, () => {
                BluetoothService.WritetoBluetooth("lnt`" + DateTime.Now.ToString() + "`");
                SetColorOfText();
                hasSecondPassed = true;
            }));
        }
    }

    public void SetElementActive(int i)
    {
        for (int j = 0; j < appWindows.Length - 1; j++)
        {
            appWindows[j].SetActive(false);
        }
        appWindows[i].SetActive(true);
    }

    public void SetAllElementsActive(bool state)
    {
        startingOptions.SetActive(!state);
        for (int i = 0; i < appWindows.Length - 1; i++)
        {
            if (!state)
                appWindows[i].SetActive(state);
            appButtons[i].SetActive(state);
        }
        appWindows[4].SetActive(true);
        battery.SetActive(state);
        for (int i = 0; i < bluetoothElements.Length; i++)
        {
            bluetoothElements[i].SetActive(state);
        }
    }

    private void SetBatteryLevel(string level)
    {
        for (int i = 0; i < batteryLevels.Length; i++)
        {
            batteryLevels[i].SetActive(false);
        }
        switch (level)
        {
            case "low":
                batteryLevels[0].SetActive(true);
                break;
            case "medium":
                batteryLevels[1].SetActive(true);
                break;
            case "high":
                batteryLevels[2].SetActive(true);
                break;
            case "CHG":
                batteryLevels[3].SetActive(true);
                break;
        }


    }

    public void FillBadge()
    {
        var colorFCP = GameManager.Instance.drawScript.fcp.color;
        foreach (var pixel in GameManager.Instance.drawScript.matrixOfPixels)
        {
            pixel.GetComponent<PixelScript>().PaintPixel(colorFCP);
        }
        BluetoothService.WritetoBluetooth("fc`" + (colorFCP.r * 255).ToString() + "`" + (colorFCP.g * 255).ToString() + "`" + (colorFCP.b * 255).ToString() + "`");
    }

    public void SaveDraw()
    {
        rgbArray = "";
        var matrix = GameManager.Instance.drawScript.matrixOfPixels;
        var matrixLength = GameManager.Instance.drawScript.matrix;
        for (int i = 0; i < matrixLength.x; i++)
        {
            for (int j = 0; j < matrixLength.y; j++)
            {
                var pixelC = matrix[i, j].GetComponent<PixelScript>();
                rgbArray += pixelC.GetPixelPosition().x + "`" + pixelC.GetPixelPosition().y + "`" + (pixelC.rgb.r * 255).ToString() + "`" + (pixelC.rgb.g * 255).ToString() + "`" + (pixelC.rgb.b * 255).ToString() + "`";
            }
        }
        if (editId == -1)
        {
            int quantity = PlayerPrefs.GetInt("savesQuantity");
            Debug.Log(quantity);
            PlayerPrefs.SetString("saveData" + quantity, rgbArray);
            GameManager.Instance.presetLEDControll.AddObject(quantity);
            quantity++;
            PlayerPrefs.SetInt("savesQuantity", quantity);
        }
        else
        {
            PlayerPrefs.SetString("saveData" + editId, rgbArray);
            GameManager.Instance.presetLEDControll.layoutList[editId].ClearMatrix();
            GameManager.Instance.presetLEDControll.layoutList[editId].DrawMatrix();
            SetElementActive(0);
            editId = -1;
        }
    }

    public void StartButton()
    {
        if (!IsConnected)
        {
            print(deviceName.text.ToString());
            IsConnected = BluetoothService.StartBluetoothConnection(deviceName.text.ToString());
            SetBatteryLevel("CHG");
            if (PlayerPrefs.HasKey("ledPower"))
                powerSlider.value = PlayerPrefs.GetFloat("ledPower");
            ValueChangeCheck();
            CleanMatrix();
        }
    }

    private void LoadBattery()
    {
        StartCoroutine(Timer(5, () => {
            if(IsConnected && sendBatteryStatus) BluetoothService.WritetoBluetooth("sbs`" + DateTime.Now.ToString() + "`");
            LoadBattery();
        }));
    }

    public void StopButton()
    {
        if (IsConnected)
        {
            IsConnected = false;
            BluetoothService.StopBluetoothConnection();
        }
    }

    public void SetColorOfText()
    {
        PlayerPrefs.SetString("badgeText", inputArea.text.ToString());
        BluetoothService.WritetoBluetooth("stc`" + fcp.color.r * 255 + "`" + fcp.color.g * 255 + "`" + fcp.color.b * 255 + "`");
    }

    public void DisplayContext()
    {
        SetColorOfText();
        if (IsConnected && (inputArea.text.ToString() != "" || inputArea.text.ToString() != null))
        {
            BluetoothService.WritetoBluetooth("lnt`" + inputArea.text.ToString() + "`");
        }
    }

    public void AddText()
    {
        SetColorOfText();
        if (IsConnected && (inputArea.text.ToString() != "" || inputArea.text.ToString() != null))
        {
            BluetoothService.WritetoBluetooth("ant`" + inputArea.text.ToString() + "`");
        }
    }

    public void StartTimer()
    {
        if (isTimer)
        {
            isTimer = false;
            hasSecondPassed = false;
        } else
        {
            isTimer = true;
            hasSecondPassed = false;
        }
    }

    private string ChangeSpaces(char[] text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            if(text[i] == ' ')
            {
                text[i] = '`';
            }
        }
        string s = new string(text);
        return s;
    }
    
    public void ValueChangeCheck()
    {
        var ledPower = powerSlider.value;
        PlayerPrefs.SetFloat("ledPower", powerSlider.value);
        BluetoothService.WritetoBluetooth("cp`" + ledPower.ToString() + "`");
        //UduinoManager.Instance.sendCommand("cp", ledPower);
    }
    public void PaintPixel(int x, int y, Color rgb)
    {
        BluetoothService.WritetoBluetooth("pp`" + x.ToString() + "`" + y.ToString() + "`" + (rgb.r * 255).ToString() + "`" + (rgb.g * 255).ToString() + "`" + (rgb.b * 255).ToString() + "`");
        //UduinoManager.Instance.sendCommand("cp", ledPower);
    }

    public void CleanMatrix()
    {
        BluetoothService.WritetoBluetooth("lnt` `");
        BluetoothService.WritetoBluetooth("cl`");
        foreach (var pixel in GameManager.Instance.drawScript.matrixOfPixels)
        {
            pixel.GetComponent<PixelScript>().PaintPixel(Color.black);
        }
        StartCoroutine(Timer(0.3f, () =>
        {
            BluetoothService.WritetoBluetooth("cl`");
        }));
    }

    public void CleanTextMatrix()
    {
        BluetoothService.WritetoBluetooth("lnt` `");
        BluetoothService.WritetoBluetooth("cl`");
        StartCoroutine(Timer(0.3f, () =>
        {
            BluetoothService.WritetoBluetooth("cl`");
        }));
    }

    public void SendCommand(string command)
    {
        BluetoothService.WritetoBluetooth(command + "`");
    }

    public void ScrollText()
    {
        SetColorOfText();
        BluetoothService.WritetoBluetooth("sss`" + scrollSpeedArea.value + "`");
        PlayerPrefs.SetFloat("scrollValue", scrollSpeedArea.value);
        if (IsConnected && (inputArea.text.ToString() != "" || inputArea.text.ToString() != null))
        {
            BluetoothService.WritetoBluetooth("st`" + inputArea.text.ToString() + "`");
        }
    }

    public void SetAligment(int id)
    {
        BluetoothService.WritetoBluetooth("sa`" + id + "`");
    }

    public void SetScrollAligment(int id)
    {
        BluetoothService.WritetoBluetooth("ssa`" + id + "`");
    }

    private IEnumerator Timer(float time, System.Action action)
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        action();
    }
    public void OpenPage(string url)
    {
        Application.OpenURL(url);
    }

    public void SendEmail()
    {
        string email = "info@cyberbadge.net";
        string subject = MyEscapeURL("Ordering Badges");
        string body = MyEscapeURL("Hello\r\nI would like to learn more about CyberBadges and potentionally order some");
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }

    string MyEscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }

    public IEnumerator LoadStuff(int id)
    {
        sendBatteryStatus = false;

        BluetoothService.WritetoBluetooth("sdl`");
        string arg = PlayerPrefs.GetString("saveData" + id);
        string[] splitArray = arg.Split(char.Parse("`"));
        int k = 0;
        var matrixLength = GameManager.Instance.drawScript.matrix;
        for (int i = 0; i < matrixLength.x; i++)
        {
            for (int j = 0; j < matrixLength.y; j++)
            {
                int x = int.Parse(splitArray[k]);
                k++;
                int y = int.Parse(splitArray[k]);
                k++;
                float r = float.Parse(splitArray[k]);
                k++;
                float g = float.Parse(splitArray[k]);
                k++;
                float b = float.Parse(splitArray[k]);
                k++;
                BluetoothService.WritetoBluetooth(x + "`" + y + "`" + r + "`" + g + "`" + b + "`");
                yield return new WaitForSeconds(0.03f);
            }
        }
        sendBatteryStatus = true;
        GameManager.Instance.presetLEDControll.isLoadingPreset = false;
        var layouts = FindObjectsOfType<LayoutScript>();
        foreach (var layout in layouts)
        {
            bool isActive = !GameManager.Instance.presetLEDControll.isLoadingPreset;
            layout.LoadButton.interactable = isActive;
            layout.DeleteButton.interactable = isActive;
            layout.EditButton.interactable = isActive;
            layout.UpButton.interactable = isActive;
            layout.DownButton.interactable = isActive;
            GameManager.Instance.bottomPanel.SetActive(isActive);
        }
    }
}
