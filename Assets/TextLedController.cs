using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;
using UnityEngine.UI;

public class TextLedController : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_InputField inputArea;
    [SerializeField] private TMPro.TMP_InputField scrollSpeedArea;
    [SerializeField] private TMPro.TMP_InputField partyModeSpeedArea;

    [SerializeField] private TMPro.TMP_InputField animArea1;
    [SerializeField] private TMPro.TMP_InputField animArea2;
    [SerializeField] private TMPro.TMP_InputField animArea3;

    [SerializeField] private Slider powerSlider;

    void Update()
    {

    }

    public void DisplayContext()
    {
        var text = ChangeSpaces(inputArea.text.ToCharArray());
        UduinoManager.Instance.sendCommand("sss", scrollSpeedArea.text);
        UduinoManager.Instance.sendCommand("spms", partyModeSpeedArea.text);
        UduinoManager.Instance.sendCommand("lnt", text);
    }

    public void DisplayContextStatic()
    {
        var text = ChangeSpaces(inputArea.text.ToCharArray());
        UduinoManager.Instance.sendCommand("spms", partyModeSpeedArea.text);
        UduinoManager.Instance.sendCommand("lnst", text);
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
        UduinoManager.Instance.sendCommand("cp", ledPower);
    }

    public void IsPartyMode(bool state)
    {
        char st = ' ';
        if (state) st = '1';
        else st = '0';
        UduinoManager.Instance.sendCommand("spms", partyModeSpeedArea.text);
        UduinoManager.Instance.sendCommand("spm", st);
    }

    public void IsAnimMode(bool state)
    {
        char st = ' ';
        if (state) st = '1';
        else st = '0';
        var text1 = ChangeSpaces(animArea1.text.ToCharArray());
        var text2 = ChangeSpaces(animArea2.text.ToCharArray());
        var text3 = ChangeSpaces(animArea3.text.ToCharArray());
        UduinoManager.Instance.sendCommand("lat1", text1);
        UduinoManager.Instance.sendCommand("lat2", text2);
        UduinoManager.Instance.sendCommand("lat3", text3);
        UduinoManager.Instance.sendCommand("spms", partyModeSpeedArea.text);
        UduinoManager.Instance.sendCommand("sam", st);
    }


    public void IsInverse(bool state)
    {
        char st = ' ';
        if (state) st = '1';
        else st = '0';
        UduinoManager.Instance.sendCommand("sim", st);
    }

    public void SetAligment(int id)
    {
        UduinoManager.Instance.sendCommand("sa", id);
    }

    public void SetScrollAligment(int id)
    {
        UduinoManager.Instance.sendCommand("ssa", id);
        var text = ChangeSpaces(inputArea.text.ToCharArray());
        UduinoManager.Instance.sendCommand("lnt", text);
    }
}
