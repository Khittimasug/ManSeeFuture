using UnityEngine;
using System.Collections.Generic;

public class HollowChoiceSystem : MonoBehaviour
{
    [Header("Dialogue Context")]
    [TextArea]
    public string questionText; // คำถามหลัก เช่น "สบายดีไหม?"

    // ใช้ [SerializeReference] สำคัญมาก! 
    // เพราะมันจะทำให้ Inspector ของ Unity สามารถใส่คลาสลูก (Accumulate, Decision) ผสมกันได้
    [SerializeReference]
    public List<BaseChoiceOption> choices = new List<BaseChoiceOption>();

    private void Start()
    {
        // ตัวอย่างการแสดงผลลง Console (เวลาทำจริง สามารถเชื่อมกับ UI Button ได้)
        DisplayQuestion();
    }

    // 1. ฟังก์ชันสำหรับสร้าง UI ตัวเลือก
    public virtual void DisplayQuestion()
    {
        Debug.Log($"NPC: {questionText}");

        for (int i = 0; i < choices.Count; i++)
        {
            if (choices[i].IsInteractable())
            {
                Debug.Log($"Choice {i}: {choices[i].choiceText}");
            }
            else
            {
                // หากถูก Lock (ในกรณีที่เป็น Decision Choice)
                Debug.Log($"Choice {i}: [LOCKED] {choices[i].choiceText}");
            }
        }
    }

    // 2. ฟังก์ชันเมื่อผู้เล่นกดเลือก (ให้ UI Button ชี้มาที่ฟังก์ชันนี้และส่ง Index มา)
    public virtual void SelectChoice(int index)
    {
        if (index < 0 || index >= choices.Count) return;

        BaseChoiceOption selectedOption = choices[index];

        if (selectedOption.IsInteractable())
        {
            selectedOption.ExecuteChoice();

            // TODO: ใส่คำสั่งเปลี่ยนหน้าต่าง UI หรือจบการสนทนาตรงนี้
        }
        else
        {
            Debug.LogWarning("ตัวเลือกนี้ถูก Lock ไม่สามารถเลือกได้!");
        }
    }
}