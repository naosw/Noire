using UnityEngine;
using Ink.Runtime;
using TMPro;
using UnityEngine.Events;

public class DialogueUI : UI
{
	public static DialogueUI Instance { get; private set; }
	
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private GameObject choicesHolder;
    [SerializeField] private ButtonUI buttonPrefab;
    
    private Story story;
    private const int MAX_CHOICES = 3;

    private void Awake()
    {
	    Instance = this;
	    Hide();
    }

    public void StartDialogue(Story newStory)
    {
	    GameEventsManager.Instance.GameStateEvents.UIToggle(true);
	    Show();
        story = newStory;
        RefreshView();
    }
    
    // This is the main function called every time the story changes. It does a few things:
	// Destroys all the old content and choices.
	// Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
	private void RefreshView () {
		RemoveChildren();
		
		while (story.canContinue) {
			displayText.text = story.Continue().Trim();
		}

		// Display all the choices, if there are any! [HARD UPPER-LIMIT: 3 choices]
		if(story.currentChoices.Count > MAX_CHOICES)
			Debug.LogError("Too many choices for this dialogue!");
		
		if(story.currentChoices.Count > 0)
		{
			foreach (Choice choice in story.currentChoices) {
				CreateChoiceView(choice.text.Trim(), () => OnClickChoiceButton(choice));
			}
		}
		else
		{
			CreateChoiceView("End conversation", EndDialogue);
		}
	}

	private void EndDialogue()
	{
		Hide();
		GameEventsManager.Instance.GameStateEvents.UIToggle(false);
	}

	// creates a choice view on button index i
	private void CreateChoiceView(string choiceText, UnityAction call)
	{
		ButtonUI choiceButton = Instantiate (buttonPrefab, choicesHolder.transform);
		choiceButton.SetText(choiceText);
		choiceButton.AddListener(call);
	}

	// When we click the choice button, tell the story to choose that choice!
	void OnClickChoiceButton (Choice choice) {
		story.ChooseChoiceIndex (choice.index);
		RefreshView();
	}
	
	void RemoveChildren () {
		int childCount = choicesHolder.transform.childCount;
		for (int i = childCount - 1; i >= 0; --i) {
			Destroy (choicesHolder.transform.GetChild (i).gameObject);
		}
	}
}