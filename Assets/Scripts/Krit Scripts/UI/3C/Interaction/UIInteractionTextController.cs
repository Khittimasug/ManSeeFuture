using UnityEngine;
using TMPro;

/// <summary>
/// [UI Component] ควบคุมการแสดงผลข้อความแจ้งเตือนบนหน้าจอ (เช่น "Press [Space] to Interact")
/// โดยจะแสดงผลเฉพาะเมื่อผู้เล่นอยู่ในระยะที่สามารถโต้ตอบกับวัตถุได้เท่านั้น
/// </summary>
public class UIInteractionTextController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField]
    private TextMeshProUGUI m_Text; // ตัวอักษร UI (TextMeshPro) ที่ต้องการซ่อน/แสดง

    [Header("Monitoring")]
    [SerializeField]
    private InteractionInstigator m_WatchedInteractionInstigator; // อ้างอิงตัวตรวจจับการโต้ตอบที่อยู่กับผู้เล่น

    void Update()
    {
        /* * [Logic การแสดงผล]
         * ข้อความจะปรากฏ (Enabled) ก็ต่อเมื่อ:
         * 1. สคริปต์ตรวจจับของผู้เล่น "เปิดใช้งานอยู่" (ไม่ได้ถูกสั่งปิดโดย FlowState)
         * 2. มีวัตถุอย่างน้อย 1 ชิ้นอยู่ในระยะที่กดถึง (HasNearbyInteractables)
         */
        m_Text.enabled = m_WatchedInteractionInstigator.enabled &&
                         m_WatchedInteractionInstigator.HasNearbyInteractables();
    }
}