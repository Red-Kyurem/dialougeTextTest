using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class dialouge_TEST : MonoBehaviour
{
    public float textSpeed = 0.1f;
    float defaultTextSpeed;
    public string textName = "????";
    public Queue<string> sentences;

    public TMP_Text dialogTextBox;
    public TMP_Text nameTextBox;

    public TextAsset dialougeFile;

    bool isItalic = false;
    bool isBolded = false;
    bool isUnderlined = false;
    bool isStrikeThrough = false;
    public bool isSentenceFilledIn = false;

    // Start is called before the first frame update
    void Start()
    {
        defaultTextSpeed = textSpeed;
        CreateNewTextLines();
        PrepareNextSentence();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12) && isSentenceFilledIn) { PrepareNextSentence(); }
        else if (Input.GetKeyDown(KeyCode.F12) && !isSentenceFilledIn) { textSpeed = 0; }
    }


    IEnumerator PrintSentence(string sentence)
    {
        // prevents the player from advancing to the next sentence
        isSentenceFilledIn = false;

        // skips the sentence if it has "//" at the beginning
        if (sentence.Substring(0, 2) == "//")
        {
            // skip the line with "//" at the beginning
            sentence = "";
            PrepareNextSentence();
        }
        // when the sentence is not skipped
        else
        {
            for (int charCount = 0; sentence.ToCharArray().Length > charCount; charCount++)
            {
                // skips the '\' character
                // the character directly after '\' will be displayed in the textbox 
                if (sentence[charCount] == '\\')
                {
                    // skip the '\' character from displaying in the textbox
                    charCount++;
                    // prints the character after the '\' to the textbox
                    dialogTextBox.text += sentence[charCount];
                }

                // starts/stops italicizing text
                else if (sentence[charCount] == '*')
                {
                    // starts italicizing text
                    if (!isItalic) { isItalic = true; dialogTextBox.text += "<i>"; }
                    // stops italicizing text
                    else { isItalic = false; dialogTextBox.text += "</i>"; }
                }

                // starts/stops bolding text
                else if (sentence[charCount] == '@')
                {
                    // starts bolding text
                    if (!isBolded) { isBolded = true; dialogTextBox.text += "<b>"; }
                    // stops bolding text
                    else { isBolded = false; dialogTextBox.text += "</b>"; }
                }

                // starts/stops underlining text
                else if (sentence[charCount] == '_')
                {
                    // starts underlining text
                    if (!isUnderlined) { isUnderlined = true; dialogTextBox.text += "<u>"; }
                    // stops underlining text
                    else { isUnderlined = false; dialogTextBox.text += "</u>"; }
                }

                // starts/stops striking through text
                else if (sentence[charCount] == '~')
                {
                    // starts striking through text
                    if (!isStrikeThrough) { isStrikeThrough = true; dialogTextBox.text += "<s>"; }
                    // stops striking through text
                    else { isStrikeThrough = false; dialogTextBox.text += "</s>"; }
                }

                // changes the speed of the text
                else if ((sentence.Length - charCount) >= 7 && sentence.Substring(charCount, 7) == "<speed=")
                {
                    // skip the '<speed=' part of the sentence from displaying in the textbox
                    charCount += 7;

                    // sets the new text speed
                    int substringLength = FindAngleBracketEnd(sentence, charCount);
                    textSpeed = float.Parse(sentence.Substring(charCount, substringLength));

                    // skip the number and '>' part of the sentence
                    charCount += substringLength;
                }

                // pauses the text for an amount of time
                else if ((sentence.Length - charCount) >= 7 && sentence.Substring(charCount, 7) == "<pause=")
                {
                    // skip the '<pause=' part of the sentence from displaying in the textbox
                    charCount += 7;

                    // pauses the text for an amount of time
                    int substringLength = FindAngleBracketEnd(sentence, charCount);
                    yield return new WaitForSeconds(float.Parse(sentence.Substring(charCount, substringLength)) - textSpeed);

                    // skip the number and '>' part of the sentence
                    charCount += substringLength;
                }

                // sets a new name for nameTextBox 
                else if ((sentence.Length - charCount) >= 6 && sentence.Substring(charCount, 6) == "<name=")
                {
                    // skip the '<name=' part of the sentence from displaying in the textbox
                    charCount += 6;

                    // sets the new name in nameTextBox
                    int substringLength = FindAngleBracketEnd(sentence, charCount);
                    nameTextBox.text = sentence.Substring(charCount, substringLength);

                    // skip the name and '>' part of the sentence
                    charCount += substringLength;
                }
                
                // prints the character to the textbox
                else
                {
                    // prints the character to the textbox
                    dialogTextBox.text += sentence[charCount];
                    // wait an amount of time 
                    yield return new WaitForSeconds(textSpeed);
                }
            }
            // allows the player to advance to the next sentence
            isSentenceFilledIn = true;
        } 
    }

    // creates new sentences to be used 
    public void CreateNewTextLines()
    {
        sentences = new Queue<string>();
        string[] dialougeFileSentences = dialougeFile.ToString().Split('\n');
        for (int i = 0;  i < dialougeFileSentences.Length; i++)
        {
            sentences.Enqueue(dialougeFileSentences[i]);
        }
    }

    // prepares for the next sentence
    public void PrepareNextSentence()
    {
        // resets sentence properties
        ResetSentence();
        // sets the sentence to the first one in the queue and removes it from the queue
        string sentence = sentences.Dequeue();

        // starts printing the sentence to the textbox
        StartCoroutine(PrintSentence(sentence));
    }

    public void ResetSentence()
    {
        // empties the textbox
        dialogTextBox.text = "";

        isItalic = false;
        isBolded = false;
        isUnderlined = false;
        isStrikeThrough = false;
        textSpeed = defaultTextSpeed;
    }

    // finds the end of the angle bracket (>) and returns the number of characters it checked
    public int FindAngleBracketEnd(string sentence, int charCount)
    {
        int i = 0;
        for (; sentence[charCount+i] != '>'; i++)
        {
            // ...do nothing
        }
        return i;
    }
}
