using UnityEngine;

/// <summary>
/// [Scriptable Object] สำหรับสร้างข้อมูล "ตัวละคร" (Character Profile)
/// ใช้เก็บรายละเอียดพื้นฐาน เช่น ชื่อตัวละคร เพื่อนำไปแสดงผลบน UI (Speaker Text)
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/Narration/Character")]
public class NarrationCharacter : ScriptableObject
{
    [SerializeField]
    private string m_CharacterName; // ชื่อของตัวละครที่จะปรากฏในกล่องคำพูด (เช่น "Krit", "Narrator")

    /* * Property (Getter) สำหรับให้ UIDialogueTextBoxController 
     * ดึงชื่อไปแสดงผลในช่อง m_SpeakerText.text
     */
    public string CharacterName => m_CharacterName;
}