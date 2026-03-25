using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class DialogueChoice
{
    [SerializeField]
    private string m_ChoicePreview;
    [SerializeField]
    private DialogueNode m_ChoiceNode;

    // ส่วนนี้: เพื่อระบุว่าคลิก Choice นี้แล้วให้ไปเล่นวิดีโอตัวไหนใน VideoSequenceManager
    [SerializeField]
    private int m_VideoIndex;

    public string ChoicePreview => m_ChoicePreview;
    public DialogueNode ChoiceNode => m_ChoiceNode;
    public int VideoIndex => m_VideoIndex; // Getter สำหรับดึงค่าไปใช้
}


[CreateAssetMenu(menuName = "Scriptable Objects/Narration/Dialogue/Node/Choice")]
public class ChoiceDialogueNode : DialogueNode
{
    [SerializeField]
    private DialogueChoice[] m_Choices;
    public DialogueChoice[] Choices => m_Choices;


    public override bool CanBeFollowedByNode(DialogueNode node)
    {
        return m_Choices.Any(x => x.ChoiceNode == node);
    }

    public override void Accept(DialogueNodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}