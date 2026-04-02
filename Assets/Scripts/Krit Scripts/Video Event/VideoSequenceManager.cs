using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;

/// <summary>
/// [Data Structure] สำหรับเก็บข้อมูลวิดีโอแต่ละช่วง
/// ใช้กำหนดพฤติกรรมว่าวิดีโอจบแล้วจะให้ทำอะไรต่อ
/// </summary>
[System.Serializable]
public class VideoSegment
{
    public string segmentName;      // ชื่อเรียกใน Inspector (ช่วยให้จัดลำดับง่าย)
    public VideoClip clip;          // ไฟล์ไฟล์วิดีโอ
    public bool hasChoice;          // ตัวกำหนด: True = หยุดรอเลือกทางเลือก / False = เล่นตัวถัดไปทันที

    [Tooltip("เหตุการณ์ที่จะเกิดขึ้นเมื่อจบวิดีโอ (เช่น เปลี่ยนเพลง, ปลดล็อค Achievement)")]
    public UnityEvent onSegmentEnd;
}

public class VideoSequenceManager : MonoBehaviour
{
    [Header("Components Settings")]
    public VideoPlayer videoPlayer; // ตัวควบคุมการเล่นไฟล์วิดีโอ
    public GameObject choicePanel;  // UI หน้าจอทางเลือก (Canvas/Panel)

    [Header("Video Playlist")]
    public List<VideoSegment> playlist; // รายการเนื้อเรื่องวิดีโอทั้งหมด

    private int currentIndex = 0;   // ตัวชี้ตำแหน่งวิดีโอปัจจุบันใน List

    void Start()
    {
        // ลงทะเบียน Event: เมื่อไฟล์วิดีโอเล่นจนถึงจุดจบ (Loop Point) 
        // ให้ไปเรียกฟังก์ชัน OnVideoFinished เพื่อตัดสินใจว่าจะทำอะไรต่อ
        videoPlayer.loopPointReached += OnVideoFinished;

        choicePanel.SetActive(false); // เริ่มเกมให้ซ่อนหน้าต่างทางเลือกไว้ก่อน
        PlaySegment(0);               // เริ่มเล่นวิดีโอแรกสุด
    }

    void Update()
    {
        /* * [LOGIC: การตรวจหาทางเลือกล่วงหน้า]
         * เนื่องจาก LoopPointReached บางครั้งทำงานช้า หรือทำให้ภาพกระพริบก่อนหยุด
         * เราจึงใช้ Update ช่วยเช็คว่า "ถ้าใกล้จะจบแล้ว (เหลือ 0.1 วินาที) และมีทางเลือกให้เลือก" 
         * ให้ชิงสั่ง Pause วิดีโอไว้ที่เฟรมสุดท้ายทันที
         */
        if (videoPlayer.isPlaying && playlist[currentIndex].hasChoice && videoPlayer.time > 0.5f)
        {
            double timeBeforeEnd = videoPlayer.length - videoPlayer.time;

            if (timeBeforeEnd <= 0.1f) // ระยะประชิดก่อนจบวิดีโอ
            {
                videoPlayer.Pause(); // หยุดวิดีโอค้างไว้ (ให้ผู้เล่นเห็นภาพสุดท้าย)

                if (!choicePanel.activeSelf)
                {
                    // หมายเหตุ: choicePanel จะถูกเปิดผ่าน Dialogue System 
                    // หรือจะเปิดตรงนี้ก็ได้หากไม่ได้ใช้ระบบ Dialogue แยก
                    playlist[currentIndex].onSegmentEnd?.Invoke(); // รันคำสั่งพิเศษที่ตั้งไว้ใน Inspector
                }
            }
        }
    }

    /// <summary>
    /// ฟังก์ชันหัวใจหลัก: ใช้เปลี่ยนวิดีโอที่จะเล่น
    /// มีการจัดการเรื่องการรีเซ็ตค่า Time และเตรียมความพร้อมไฟล์ (Prepare)
    /// </summary>
    public void PlaySegment(int index)
    {
        if (index >= 0 && index < playlist.Count)
        {
            currentIndex = index;

            choicePanel.SetActive(false); // ปิดหน้าต่างทางเลือกทันทีที่เริ่มคลิปใหม่

            videoPlayer.Stop(); // ต้องสั่ง Stop เพื่อรีเซ็ตค่า Time/Buffer เดิมให้เกลี้ยง
            videoPlayer.clip = playlist[index].clip;

            /*
             * [.Prepare()] ช่วยลดอาการกระตุกและป้องกันปัญหา Video หยุดตั้งแต่เริ่ม
             * เป็นการบอกให้ Engine โหลดข้อมูล Length และ Metadata ของคลิปใหม่ให้พร้อมก่อนกด Play
             */
            videoPlayer.Prepare();
            videoPlayer.Play();

            Debug.Log($"[VideoManager] Playing: {playlist[index].segmentName}");
        }
        else
        {
            Debug.LogWarning("[VideoManager] Index out of range: ไม่พบวิดีโอลำดับนี้ใน Playlist");
        }
    }

    /// <summary>
    /// ทำหน้าที่ตัดสินใจหลังจากวิดีโอ "จบสนิท" (สำหรับวิดีโอที่ไม่มีทางเลือก)
    /// </summary>
    void OnVideoFinished(VideoPlayer vp)
    {
        VideoSegment currentSegment = playlist[currentIndex];

        // ถ้าไม่มีทางเลือก (hasChoice = false) ให้ไหลไปเล่นวิดีโอถัดไปใน List ทันที
        if (!currentSegment.hasChoice)
        {
            currentSegment.onSegmentEnd?.Invoke();
            PlayNext();
        }
    }

    /// <summary>
    /// สั่งเล่นวิดีโอตัวถัดไปตามลำดับใน Playlist
    /// </summary>
    public void PlayNext()
    {
        PlaySegment(currentIndex + 1);
    }

    /// <summary>
    /// ฟังก์ชันรับคำสั่งจากการคลิกปุ่ม Choice บน UI
    /// </summary>
    /// <param name="choice">ข้อมูล Choice ที่เก็บ Index ของวิดีโอปลายทางไว้</param>
    public void OnChoiceSelected(DialogueChoice choice)
    {
        // ดึงค่า VideoIndex จากปุ่มที่กด แล้วส่งไปให้ PlaySegment ทำงาน
        PlaySegment(choice.VideoIndex);
        choicePanel.SetActive(false);
    }
}