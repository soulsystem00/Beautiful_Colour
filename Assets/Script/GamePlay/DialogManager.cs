using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSecond;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;
    public static DialogManager Instance { get; set; }
    private void Awake()
    {
        Instance = this;
    }

    Dialog dialog;
    Action onDialogFinished;
    int currentLine = 0;
    bool isTyping;
    
    public bool IsShowing { get; set; }


    [SerializeField] GameObject selectionBox;
     List<Text> texts;
    [SerializeField] Color highlightColor;
    bool selection;
    int currentSelect = 0;
    public IEnumerator ShowDialog(Dialog dialog, Action onFinished = null, bool selection = false)
    {
        yield return new WaitForEndOfFrame();
        this.selection = selection;
        OnShowDialog?.Invoke();

        IsShowing = true;
        this.dialog = dialog;
        onDialogFinished = onFinished;

        dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }
    public void HandleUpdate()
    {
        if (selectionBox.activeSelf)
        {
            SecondMenu();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Z) && !isTyping)
            {
                ++currentLine;
                if (currentLine < dialog.Lines.Count)
                {
                    StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
                }
                else
                {
                    if (selection)
                    {
                        texts = selectionBox.GetComponentsInChildren<Text>().ToList();
                        selectionBox.SetActive(true);
                    }
                    else
                    {
                        currentLine = 0;
                        currentSelect = 0;
                        IsShowing = false;
                        dialogBox.SetActive(false);
                        onDialogFinished?.Invoke();
                        OnCloseDialog?.Invoke();
                    }
                }
            }
        }
        

        
    }
    void SecondMenu()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            currentSelect = (currentSelect + 1) % texts.Count;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentSelect = (currentSelect - 1) % texts.Count;

        for (int i = 0; i < texts.Count; i++)
        {
            if (i == currentSelect)
                texts[i].color = highlightColor;
            else
                texts[i].color = Color.black;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            currentLine = 0;
            
            IsShowing = false;
            dialogBox.SetActive(false);
            selectionBox.SetActive(false);

            if (currentSelect == 0)
                onDialogFinished?.Invoke();

            OnCloseDialog?.Invoke();
            currentSelect = 0;
        }
    }
    public IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
    }
}
