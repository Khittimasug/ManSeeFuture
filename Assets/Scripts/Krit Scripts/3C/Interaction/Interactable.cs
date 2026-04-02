using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// [Component] สำหรับวัตถุที่ผู้เล่นสามารถตอบโต้ได้ (Interactable Object)
/// ใช้ติดไว้กับ Object ในฉาก เช่น ประตู, สวิตช์ไฟ, หรือ NPC
/// </summary>
public class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField]
    private UnityEvent m_OnInteraction; // เหตุการณ์ที่จะให้เกิดขึ้นเมื่อผู้เล่นกดปุ่มโต้ตอบ

    /// <summary>
    /// ฟังก์ชันหลักที่ระบบควบคุมตัวละคร (เช่น Raycast หรือ Trigger) จะเป็นคนเรียกใช้
    /// </summary>
    public void DoInteraction()
    {
        // สั่งให้ทุกฟังก์ชันที่ลากใส่ไว้ใน Unity Event ทำงานทันที
        if (m_OnInteraction != null)
        {
            m_OnInteraction.Invoke();

            // ตัวอย่างการใช้งานใน Inspector:
            // - ลาก DialogueChannel ใส่ -> เลือก RaiseRequestDialogue(myDialogue)
            // - ลาก Animator ของประตูใส่ -> เลือก Play("Open")
            // - ลาก GameObject ของไอเทมใส่ -> เลือก SetActive(false)
        }
    }
}