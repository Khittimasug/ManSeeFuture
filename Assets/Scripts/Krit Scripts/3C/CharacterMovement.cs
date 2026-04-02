using UnityEngine;

/// <summary>
/// [Component] ควบคุมการเคลื่อนที่ของตัวละคร (Character Movement)
/// โดยคำนวณทิศทางให้สัมพันธ์กับตำแหน่งของกล้อง (Camera Relative Movement)
/// </summary>
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    private float m_MovementSpeed = 1.0f; // ความเร็วในการเคลื่อนที่

    [Header("References")]
    [SerializeField]
    private Transform m_CameraTransform;   // อ้างอิงตำแหน่งกล้องเพื่อใช้คำนวณทิศทาง Forward

    private CharacterController m_CharacterController;

    private void Start()
    {
        // ดึง Component สำหรับควบคุมการเคลื่อนที่พื้นฐานของ Unity มาใช้งาน
        m_CharacterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        /* --- 1. คำนวณทิศทางสัมพันธ์กับกล้อง (Camera Direction) --- */

        // หาเวกเตอร์ที่พุ่งจากกล้องมายังตัวละคร เพื่อใช้เป็นทิศทาง "ด้านหน้า"
        Vector3 forward = (transform.position - m_CameraTransform.position).normalized;
        forward.y = 0; // ตัดค่าแนวตั้งออก เพื่อไม่ให้ตัวละครเดินจมดินหรือเหินขึ้นฟ้าเมื่อเงยหน้ากล้อง

        // ใช้ Cross Product หาเวกเตอร์ "ด้านขวา" ที่ตั้งฉากกับทิศ Forward และทิศ Up
        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

        /* --- 2. รับค่า Input จากผู้เล่น --- */

        float moveAxisX = m_MovementSpeed * Input.GetAxis("Horizontal"); // ปุ่ม A, D หรือลูกศรซ้าย-ขวา
        float moveAxisY = m_MovementSpeed * Input.GetAxis("Vertical");   // ปุ่ม W, S หรือลูกศรบน-ล่าง

        /* --- 3. ผสมทิศทางและสั่งเคลื่อนที่ --- */

        // รวมแรงเคลื่อนที่จากทั้งแนวหน้า-หลัง และ ซ้าย-ขวา
        Vector3 movement = (forward * moveAxisY) + (right * moveAxisX);

        // สั่งให้ CharacterController เคลื่อนที่ไปตามแรงที่คำนวณได้
        // หมายเหตุ: หากต้องการให้แรงโน้มถ่วงทำงาน ต้องเพิ่มแรงในแนวแกน Y ต่อไป
        m_CharacterController.Move(movement * Time.deltaTime);
    }
}