using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// [Controller] สำหรับควบคุมปุ่มทางเลือก (Choice Button) แต่ละปุ่ม
/// ทำหน้าที่รับข้อมูลจาก DialogueNode มาแสดงผล และส่งคำสั่งเมื่อผู้เล่นคลิกเลือก
/// </summary>
public class UIDialogueChoiceController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_Choice;           // ตัวหนังสือบนปุ่มที่จะแสดงคำอธิบายทางเลือก
    [SerializeField]
    private DialogueChannel m_DialogueChannel;  // ช่องทางสื่อสารหลักของระบบ Dialogue (Scriptable Object)

    private DialogueNode m_ChoiceNextNode;      // เก็บข้อมูลว่าถ้าเลือกข้อนี้แล้ว จะไป Node ไหนต่อ (ถ้ามี)
    private int m_VideoIndex;                   // เก็บหมายเลขลำดับวิดีโอที่จะต้องเล่นเมื่อกดปุ่มนี้
    private VideoSequenceManager m_VideoManager; // ตัวอ้างอิงไปยังระบบจัดการวิดีโอในฉาก

    /// <summary>
    /// Property สำหรับรับค่าข้อมูลจากระบบ Dialogue มาเซ็ตลงปุ่ม
    /// ถูกเรียกใช้งานตอนที่ระบบสร้างปุ่ม (Instantiate) ขึ้นมาบนหน้าจอ
    /// </summary>
    public DialogueChoice Choice
    {
        set
        {
            m_Choice.text = value.ChoicePreview; // แสดงข้อความบนปุ่ม
            m_ChoiceNextNode = value.ChoiceNode; // บันทึก Node ถัดไป
            m_VideoIndex = value.VideoIndex;     // บันทึก Index วิดีโอที่จะเล่น
        }
    }

    /// <summary>
    /// ฟังก์ชันช่วยเหลือสำหรับส่ง VideoManager มาให้ปุ่ม (ใช้เผื่อกรณี Find ไม่เจอ)
    /// </summary>
    public void SetupManager(VideoSequenceManager manager)
    {
        m_VideoManager = manager;
    }

    private void Start()
    {
        // 1. ลงทะเบียนเหตุการณ์การคลิกปุ่ม: เมื่อคลิกปุ่มให้รันฟังก์ชัน OnClick
        GetComponent<Button>().onClick.AddListener(OnClick);

        // 2. ค้นหา VideoSequenceManager ที่อยู่ในฉากโดยอัตโนมัติ 
        // เพื่อให้ปุ่มรู้ว่าจะต้องส่งคำสั่งเล่นวิดีโอไปที่ไหน
        if (m_VideoManager == null)
        {
            m_VideoManager = FindFirstObjectByType<VideoSequenceManager>();
        }
    }

    /// <summary>
    /// [Core Logic] ฟังก์ชันที่จะทำงานเมื่อผู้เล่น "กดปุ่มทางเลือก"
    /// </summary>
    private void OnClick()
    {
        // --- ส่วนที่ 1: เชื่อมต่อไปยังระบบวิดีโอ ---
        // สั่งให้ Video Manager เริ่มเล่นวิดีโอตามหมายเลข (Index) ที่ถูกเซ็ตไว้ในปุ่มนี้
        if (m_VideoManager != null)
        {
            m_VideoManager.PlaySegment(m_VideoIndex);
        }

        // --- ส่วนที่ 2: เชื่อมต่อไปยังระบบ Dialogue ---
        /*
         * การส่งค่า null ไปที่ RaiseRequestDialogueNode คือการสั่ง "จบการสนทนา"
         * ซึ่งจะส่งผลให้หน้าต่าง UI (TextBox/ChoicesBox) ปิดตัวลงทันที (Deactivate)
         * เพื่อให้ผู้เล่นสามารถดูวิดีโอตัวถัดไปได้โดยไม่มี UI บดบัง
         */
        if (m_DialogueChannel != null)
        {
            m_DialogueChannel.RaiseRequestDialogueNode(null);
        }
    }
}