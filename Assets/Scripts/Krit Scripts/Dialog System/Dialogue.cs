using UnityEngine;

/// <summary>
/// [Scriptable Object] เปรียบเสมือน "หน้าปก" หรือ "จุดเริ่มต้น" ของบทสนทนาหนึ่งชุด
/// ใช้สำหรับสร้างไฟล์ Asset ในหน้าต่าง Project เพื่อเก็บข้อมูลว่าบทสนทนานี้เริ่มที่ Node ไหน
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/Narration/FirstNode Dialogue")]
public class Dialogue : ScriptableObject
{
    [SerializeField]
    private DialogueNode m_FirstNode; // เก็บข้อมูล Node แรกสุดที่จะให้ระบบเริ่มทำงาน

    /* * Property (Getter) สำหรับให้ระบบ DialogueSequencer 
     * มาดึงค่า Node แรกไปใช้งานเมื่อสั่ง StartDialogue()
     */
    public DialogueNode FirstNode => m_FirstNode;
}