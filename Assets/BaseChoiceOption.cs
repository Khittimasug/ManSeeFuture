using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BaseChoiceOption
{
    [Header("Hollow Choice Settings")]
    public string choiceText; // ข้อความที่ให้ผู้เล่นเลือก (เช่น "สบายดี")
    [TextArea]
    public string replyText;  // ข้อความที่ NPC จะตอบกลับ (เช่น "เยี่ยมไปเลย")

    [Space]
    public UnityEvent onChoiceSelected; // Event เผื่อไว้ลากคำสั่งเสริมใน Inspector

    // ใช้ virtual เพื่อให้ Accumulate/Decision มาเพิ่มลอจิก (เช่น บวกแต้ม) ได้
    public virtual void ExecuteChoice()
    {
        Debug.Log($"[Player]: {choiceText}");
        Debug.Log($"[NPC Reply]: {replyText}");

        // รัน Event เพิ่มเติม (ถ้ามี)
        onChoiceSelected?.Invoke();
    }

    // ฟังก์ชันเช็คว่าตัวเลือกนี้กดได้หรือไม่
    // ใช้ virtual เพื่อให้ Decision Choice นำไปใช้เช็คเงื่อนไข Lock ได้
    public virtual bool IsInteractable()
    {
        return true; // Hollow Choice พื้นฐานจะกดได้เสมอ
    }
}