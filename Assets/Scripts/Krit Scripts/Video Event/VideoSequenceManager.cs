using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;

// คลาสสำหรับเก็บข้อมูลวิดีโอแต่ละช่วง (ใช้ [System.Serializable] เพื่อให้แสดงผลใน Inspector ได้)
[System.Serializable]
public class VideoSegment
{
    public string segmentName;     // ชื่อเรียกช่วงวิดีโอ (เอาไว้ดูใน Inspector)
    public VideoClip clip;         // ไฟล์วิดีโอคลิป (.mp4, .mov ฯลฯ)
    public bool hasChoice;         // ติ๊กถ้าต้องการให้มีปุ่มเลือกเมื่อจบวิดีโอนี้

    [Tooltip("เหตุการณ์ที่จะเกิดขึ้นเมื่อจบวิดีโอ หรือเมื่อเลือก Choice")]
    public UnityEvent onSegmentEnd; // ใช้สำหรับเรียกคำสั่งอื่นๆ ของ Unity เช่น เปลี่ยนฉาก, ปิดเสียง ฯลฯ
}

public class VideoSequenceManager : MonoBehaviour
{
    [Header("Components Settings")]
    public VideoPlayer videoPlayer; // ตัวเล่นวิดีโอในฉาก
    public GameObject choicePanel;  // UI Panel ที่เก็บปุ่มเลือก (จะเปิดเมื่อวิดีโอจบและ hasChoice เป็น true)

    [Header("Video Playlist")]
    public List<VideoSegment> playlist; // รายการวิดีโอทั้งหมดที่เราตั้งค่าไว้ใน Inspector

    private int currentIndex = 0;   // เก็บค่าลำดับวิดีโอที่กำลังเล่นอยู่ปัจจุบัน

    void Start()
    {
        // ลงทะเบียน Event: เมื่อ VideoPlayer เล่นวิดีโอจนจบ (ถึง Loop Point) ให้ไปเรียกฟังก์ชัน OnVideoFinished
        videoPlayer.loopPointReached += OnVideoFinished;

        // เริ่มต้นด้วยการปิด Panel ทางเลือกก่อน
        choicePanel.SetActive(false);

        // เริ่มเล่นวิดีโอตัวแรก (ลำดับที่ 0)
        PlaySegment(0);
    }

    /// <summary>
    /// ฟังก์ชันสำหรับสั่งเล่นวิดีโอตามลำดับ Index ที่ระบุ
    /// </summary>
    public void PlaySegment(int index)
    {
        // ตรวจสอบก่อนว่า Index ที่ส่งมา มีอยู่ในรายการ Playlist จริงหรือไม่ (ป้องกัน Error)
        if (index >= 0 && index < playlist.Count)
        {
            currentIndex = index;
            videoPlayer.clip = playlist[index].clip; // เปลี่ยนคลิปวิดีโอ
            videoPlayer.Play();                      // สั่งเล่น
        }
        else
        {
            Debug.Log("สิ้นสุดรายการวิดีโอ หรือ Index ไม่ถูกต้อง");
        }
    }

    // ฟังก์ชันนี้จะถูกเรียกอัตโนมัติเมื่อวิดีโอเล่นจบ (callback จาก videoPlayer.loopPointReached)
    void OnVideoFinished(VideoPlayer vp)
    {
        // ดึงข้อมูลวิดีโอปัจจุบันออกมาดู
        VideoSegment currentSegment = playlist[currentIndex];

        if (currentSegment.hasChoice)
        {
            // ถ้าวิดีโอนี้ถูกตั้งค่าว่า "มีทางเลือก"
            // ตัวเครื่องจะค้างภาพวิดีโอเฟรมสุดท้ายไว้ แล้วเปิด UI Choice ขึ้นมาให้ผู้เล่นกด
            choicePanel.SetActive(true);
        }
        else
        {
            // ถ้าไม่มีทางเลือก ให้รันคำสั่งใน Unity Event (ถ้ามี)
            currentSegment.onSegmentEnd?.Invoke();

            Debug.Log("Video end");
            // แล้วข้ามไปเล่นวิดีโอถัดไปทันที
            PlayNext();
        }
    }

    /// <summary>
    /// ฟังก์ชันสั่งเล่นวิดีโอลำดับถัดไป (+1 จากลำดับปัจจุบัน)
    /// </summary>
    public void PlayNext()
    {
        PlaySegment(currentIndex + 1);
    }

    /// <summary>
    /// ฟังก์ชันสำหรับให้ "ปุ่ม Choice" เรียกใช้
    /// โดยเราจะส่งเลข Index ของวิดีโอที่เราต้องการให้ไปต่อมาที่นี่
    /// </summary>
    public void SelectChoice(int nextVideoIndex)
    {
        // ตัวอย่าง: ถ้ากดปุ่ม A แล้วอยากให้ข้ามไปเล่นวิดีโอตัวที่ 5 ก็ส่งเลข 5 มา
        PlaySegment(nextVideoIndex);
    }
}