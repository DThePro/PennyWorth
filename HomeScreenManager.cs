using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UIElements;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HomeScreenManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI welcomeUsername;
    [SerializeField]
    GameObject AddExpensePanel, ViewExpensesPanel;
    [SerializeField]
    Animator animationPanel;

    [SerializeField]
    Dropdown type;
    [SerializeField]
    TextField addType;
    [SerializeField]
    UnityEngine.UI.Button addTypeButton;
    [SerializeField]
    TextField message;
    [SerializeField]
    TextField amount;
    [SerializeField]
    TextField dateDay, dateMonth, dateYear;
    [SerializeField]
    TextField timeHour, timeMinute;
    [SerializeField]
    Dropdown timeAmPm;

    bool addTypeBool = false;
    float totalTime, timeElapsed = 0;
    bool statusActive = false, statusInfinite = false;

    // Start is called before the first frame update
    void Start()
    {
        if (UserPreferences.Instance != null) welcomeUsername.text = UserPreferences.Instance._username;
    }

    // Update is called once per frame
    void Update()
    {
        if (statusActive && !statusInfinite)
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed > totalTime)
            {
                timeElapsed = 0;
                // DeactivateStatus();
            }
        }
    }

    public void LogOut()
    {
        SceneManager.LoadScene(0);
    }

    public void AddExpense()
    {
        ViewExpensesPanel.transform.SetParent(animationPanel.transform);
        animationPanel.SetBool("Disappear", true);
        AddExpensePanel.GetComponent<Animator>().SetBool("AddExpense", true);
    }

    public void ViewExpenses()
    {
        AddExpensePanel.transform.SetParent(animationPanel.transform);
        animationPanel.SetBool("Disappear", true);
        ViewExpensesPanel.GetComponent<Animator>().SetBool("ViewExpenses", true);
    }

    public void AddExpenseFinal()
    {
        string _type = addTypeBool ? type.options[type.value].text : addType.text;

        string _message = message.text;

        float _amount;
        if (float.TryParse(amount.text, out _amount)) _amount = _amount;
        else;   // Error handling

        string __day, __month, __year;
        int _day, _month, _year;
        int.TryParse(dateDay.text, out _day);
        int.TryParse(dateDay.text, out _month);
        int.TryParse(dateDay.text, out _year);
        if (dateMonth.text == "1" ||
            dateMonth.text == "3" ||
            dateMonth.text == "5" ||
            dateMonth.text == "7" ||
            dateMonth.text == "8" ||
            dateMonth.text == "10" ||
            dateMonth.text == "12" ||
            dateMonth.text == "01" ||
            dateMonth.text == "03" ||
            dateMonth.text == "05" ||
            dateMonth.text == "07" ||
            dateMonth.text == "08")
        {
            if (_day >= 1 && _day <= 31)
            {
                __day = _day.ToString();
                __month = _month.ToString();
                __year = _year.ToString();
            }
        }
        else if (dateMonth.text == "4" ||
                dateMonth.text == "6" ||
                dateMonth.text == "9" ||
                dateMonth.text == "11" ||
                dateMonth.text == "04" ||
                dateMonth.text == "06" ||
                dateMonth.text == "09")
        {
            if (_day >= 1 && _day <= 30)
            {
                __day = _day.ToString();
                __month = _month.ToString();
                __year = _year.ToString();
            }
        }
        else if (dateMonth.text == "2" || dateMonth.text == "02")
        {
            if (_year % 400 == 0 || (_year % 400 != 0 && _year % 4 == 0))
            {
                if (_day >= 1 && _day <= 29)
                {
                    __day = _day.ToString();
                    __month = _month.ToString();
                    __year = _year.ToString();
                }
            }
            else
            {
                if (_day >= 1 && _day <= 28)
                {
                    __day = _day.ToString();
                    __month = _month.ToString();
                    __year = _year.ToString();
                }
            }
        }

        string __hour, __minute;
        int _hour, _minute;
        int.TryParse(timeHour.text, out _hour);
        int.TryParse(timeMinute.text, out _minute);
        if (_hour >= 1 && _hour <= 12 && _minute >= 0 && _minute <= 60)
        {
            if (timeAmPm.options[timeAmPm.value].text == "AM")
            {
                __hour = _hour.ToString();
            }
            else
            {
                __hour = (_hour + 12).ToString();
                if (__hour == "24") __hour = "00";
            }
            __minute = _minute.ToString();
        }

        StartCoroutine(AddExpenseFinalCoroutine(_type, _message, _amount.ToString(), _day.ToString(), _month.ToString(), _year.ToString(), _hour.ToString(), _minute.ToString()));
    }

    IEnumerator AddExpenseFinalCoroutine(string _type, string _message, string _amount, string _day, string _month, string _year, string _hour, string _minute)
    {
        totalTime = 0f;

        WWWForm form = new();
        form.AddField("name", UserPreferences.Instance._username);
        form.AddField("type", _type);
        form.AddField("message", _message);
        form.AddField("amount", _amount);
        form.AddField("date", (_year + "-" + _month + "-" + _day));
        form.AddField("time", (_hour + ":" + _minute + ":00"));

        WWW www = new WWW("https://sqlhandler.000webhostapp.com/expense.php", form);
        yield return www;

        if (www.text == "0")
        {
            Debug.Log("User created.");
            // ActivateStatus(Color.white, "Signed up successfully!", 3f);
            // ActivateLogin();
        }
        else
        {
            Debug.Log("User creation failed. " + www.text);
            // ActivateStatus(Color.red, www.text, 3f);
        }
    }

    public void SwitchToAddType()
    {
        type.transform.parent.gameObject.SetActive(false);
        addType.parent.SetEnabled(true);
        addTypeBool = true;
    }
}
