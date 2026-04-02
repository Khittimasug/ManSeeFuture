using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// [Scriptable Object] ตัวเก็บข้อมูล "หนึ่งบรรทัด" ของบทสนทนา
/// เปรียบเสมือนเนื้อหาใน 1 หน้าของกล่องข้อความที่จะแสดงผลบนจอ
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/Narration/Line")]
public class NarrationLine : ScriptableObject
{
    [SerializeField]
    private NarrationCharacter m_Speaker; // ตัวละครที่เป็นคนพูดประโยคนี้ (ดึงข้อมูลจากไฟล์ Character)

    [TextArea(3, 10)] // ช่วยให้พิมพ์ข้อความยาวๆ ใน Inspector ได้ง่ายขึ้น
    [SerializeField]
    private string m_Text;           // ข้อความบทพูดที่จะไปปรากฏใน Dialogue Text

    [SerializeField]
    private UnityEvent m_EventTrigger; // เหตุการณ์ที่จะให้เกิดขึ้น "ทันที" เมื่อประโยคนี้เริ่มขึ้น

    // Getter (Public Properties): เพื่อให้ระบบ UI นำข้อมูลไปแสดงผล
    public NarrationCharacter Speaker => m_Speaker;
    public string Text => m_Text;

    /// <summary>
    /// ฟังก์ชันสำหรับเรียกใช้ Event ที่ตั้งค่าไว้ใน Inspector
    /// เช่น สั่งให้ตัวละครเล่น Animation ท่าทางดีใจในประโยคนี้
    /// </summary>
    public void TriggerEvent()
    {
        m_EventTrigger?.Invoke();
    }
}