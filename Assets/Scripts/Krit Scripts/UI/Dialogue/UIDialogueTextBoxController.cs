using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogueTextBoxController : MonoBehaviour, DialogueNodeVisitor
{
    [SerializeField]
    protected TextMeshProUGUI m_SpeakerText;
    [SerializeField]
    protected TextMeshProUGUI m_DialogueText;

    [SerializeField]
    protected RectTransform m_ChoicesBoxTransform;
    [SerializeField]
    protected UIDialogueChoiceController m_ChoiceControllerPrefab;

    [SerializeField]
    protected DialogueChannel m_DialogueChannel;

    protected bool m_ListenToInput = false;
    protected DialogueNode m_NextNode = null;

    protected virtual void Awake()
    {
        m_DialogueChannel.OnDialogueNodeStart += OnDialogueNodeStart;
        m_DialogueChannel.OnDialogueNodeEnd += OnDialogueNodeEnd;

        gameObject.SetActive(false);
        m_ChoicesBoxTransform.gameObject.SetActive(false);
    }

    protected virtual void OnDestroy()
    {
        m_DialogueChannel.OnDialogueNodeEnd -= OnDialogueNodeEnd;
        m_DialogueChannel.OnDialogueNodeStart -= OnDialogueNodeStart;
    }

    protected virtual void Update()
    {
        if (m_ListenToInput && Input.GetButtonDown("Submit"))
        {
            m_DialogueChannel.RaiseRequestDialogueNode(m_NextNode);
        }
    }

    protected virtual void OnDialogueNodeStart(DialogueNode node)
    {
        gameObject.SetActive(true);

        m_DialogueText.text = node.DialogueLine.Text;
        m_SpeakerText.text = node.DialogueLine.Speaker.CharacterName;

        node.Accept(this);
    }

    protected virtual void OnDialogueNodeEnd(DialogueNode node)
    {
        m_NextNode = null;
        m_ListenToInput = false;
        m_DialogueText.text = "";
        m_SpeakerText.text = "";

        foreach (Transform child in m_ChoicesBoxTransform)
        {
            Destroy(child.gameObject);
        }

        gameObject.SetActive(false);
        m_ChoicesBoxTransform.gameObject.SetActive(false);
    }

    public virtual void Visit(BasicDialogueNode node)
    {
        m_ListenToInput = true;
        m_NextNode = node.NextNode;
    }

    public virtual void Visit(ChoiceDialogueNode node)
    {
        m_ChoicesBoxTransform.gameObject.SetActive(true);

        foreach (DialogueChoice choice in node.Choices)
        {
            UIDialogueChoiceController newChoice = Instantiate(m_ChoiceControllerPrefab, m_ChoicesBoxTransform);
            newChoice.Choice = choice;
        }
    }

//private void OnEnable()
//{
//    m_DialogueChannel.OnDialogueNodeStart += OnDialogueNodeStart;
//    m_DialogueChannel.OnDialogueNodeEnd += OnDialogueNodeEnd;
//}

//private void OnDisable()
//{
//    if (m_DialogueChannel != null)
//    {
//        m_DialogueChannel.OnDialogueNodeStart -= OnDialogueNodeStart;
//        m_DialogueChannel.OnDialogueNodeEnd -= OnDialogueNodeEnd;
//    }
//}
}