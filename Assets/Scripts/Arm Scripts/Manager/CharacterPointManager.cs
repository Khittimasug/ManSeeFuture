using UnityEngine;

/// <summary>
/// ระบุเป้าหมายตัวละครที่ต้องการจะเพิ่ม/ลดแต้ม
/// </summary>
public enum CharacterTarget
{
    None,       // ไม่มีการเปลี่ยนแต้ม
    Biw,        // ให้แต้ม Biw
    Josephine   // ให้แต้ม Josephine
}

/// <summary>
/// [Manager] ทำหน้าที่เก็บและจัดการแต้มของตัวละคร
/// </summary>
public class CharacterPointManager : MonoBehaviour
{
    // ใช้รูปแบบ Singleton เพื่อให้สคริปต์อื่น (เช่น ปุ่ม Choice) เรียกใช้งานได้ง่ายๆ
    public static CharacterPointManager Instance { get; private set; }

    [Header("Character Points")]
    public int biwPoints = 0;
    public int josephinePoints = 0;

    private void Awake()
    {
        // ตั้งค่า Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ฟังก์ชันสำหรับเพิ่มหรือลดแต้ม (ถ้าลด ให้ใส่ค่า amount เป็นติดลบ)
    /// </summary>
    public void ModifyPoints(CharacterTarget target, int amount)
    {
        if (target == CharacterTarget.Biw)
        {
            biwPoints += amount;
            Debug.Log($"[Points] Biw ได้รับ {amount} แต้ม | แต้มรวม: {biwPoints}");
        }
        else if (target == CharacterTarget.Josephine)
        {
            josephinePoints += amount;
            Debug.Log($"[Points] Josephine ได้รับ {amount} แต้ม | แต้มรวม: {josephinePoints}");
        }
    }
}