using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIManager : MonoBehaviour
{
    public static GUIManager Instance; // Singleton instance

    private static BoardManager boardManager = null; // Tham chiếu đến BoardManager
    // public GameObject gameOverPanel; // Panel hiển thị khi trò chơi kết thúc
    public TextMeshProUGUI turnText; // Text để hiển thị lượt chơi
    public TextMeshProUGUI playerScoreText; // Text để hiển thị điểm số của người chơi
    public TextMeshProUGUI computerScoreText; // Text để hiển thị điểm số của máy
    public TextMeshProUGUI noticeText; // Text để hiển thị thông báo

    public GameObject StartMenuPanel; //MainMenu a.k.a Start Menu
    // private Button restartButton; // Nút để khởi động lại trò chơi
    private int stoneLeft; // Số quân còn lại đang được rải. Có thể sử dụng để hiển thị ô quân.
    //   // Tham chiếu đến nút Mute và các hình ảnh
    public Button muteButton; // Nút Mute
    public Sprite muteSprite; // Hình ảnh khi Mute
    public Sprite unmuteSprite; // Hình ảnh khi Unmute

    public AudioClip winMusic; // Nhạc khi người chơi thắng
    public AudioClip loseMusic; // Nhạc khi người chơi thua
    private AudioSource audioSource; // Nguồn phát âm thanh

    private bool isMuted = false; // Trạng thái mute
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        boardManager = BoardManager.Instance; // Lấy tham chiếu đến BoardManager        
        gameOverPanel.SetActive(false); // Ẩn panel game over
        Update();
        HideNotice();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void Update()
    {
        if (boardManager == null) boardManager = BoardManager.Instance; // Lấy tham chiếu đến BoardManager nếu chưa có
        turnText.text = (boardManager.playerTurnLeft > 0) ? "Lượt của bạn" : "Lượt của AI";
        playerScoreText.text = boardManager.playerScore.ToString();
        computerScoreText.text = boardManager.computerScore.ToString();
    }

    public void UpdateStoneLeft(int count)
    {
        stoneLeft = count;
        // Cập nhật giao diện nếu cần thiết
        // Ví dụ: hiển thị số quân còn lại trên giao diện
        // Task này HH làm nhé
    }

    public GameObject gameOverPanel; // Panel hiển thị khi trò chơi kết thúc
    public TextMeshProUGUI gameOverText; // Text thông báo kết quả
    public Button restartButton; // Nút để khởi động lại trò chơi
    public Button mainMenuButton; // Optional: Button to return to main menu
    public Button returnMenu; // Nút để quay về menu


    public void ShowNotice(string message, float duration = 2f)
    {
        noticeText.text = message;
        noticeText.gameObject.SetActive(true);
        StartCoroutine(HideNotice(duration));
    }
    public void HideNotice()
    {
        noticeText.gameObject.SetActive(false);
    }
    public IEnumerator HideNotice(float duration)
    {
        yield return new WaitForSeconds(duration);
        HideNotice();
    }

    public bool ShowChoice(string message, string btn1, string btn2)
    {
        // Hiển thị hộp thoại câu hỏi (message) và 2 nút (btn1, btn2) trên giao diện
        // Trả về true nếu người chơi chọn btn1, false nếu chọn btn2
        // (Cần triển khai giao diện Unity để xử lý logic này)
        return true; // Placeholder
    }

    public void ShowGameOverPanel(int playerScore, int computerScore)
    {
        // Hiển thị Game Over Panel
        gameOverPanel.SetActive(true);

        // Đặt nội dung text dựa trên kết quả
        if (playerScore > computerScore)
        {
            gameOverText.text = "Bạn đã thắng!";
            PlayGameOverMusic(winMusic); // Phát nhạc thắng
        }
        else if (playerScore < computerScore)
        {
            gameOverText.text = "Bạn đã thua!";
            PlayGameOverMusic(loseMusic); // Phát nhạc thua
        }
        else
        {
            gameOverText.text = "Bạn đã hòa!";
        }

        // Add listener cho nút restart
        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(RestartGame);

        // Add listener cho nút quay về menu
        if (returnMenu != null)
        {
            returnMenu.onClick.RemoveAllListeners();
            returnMenu.onClick.AddListener(ReturnToMainMenu);
        }
    }

    private void PlayGameOverMusic(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            BackgroundMusicManager.Instance.StopMusic(); // Dừng nhạc nền
            audioSource.Stop(); // Dừng bất kỳ âm thanh nào đang phát
            audioSource.clip = clip;
            audioSource.Play(); // Phát nhạc kết thúc
        }
    }

    private void RestartGame()
    {
        // Reset trạng thái trò chơi
        gameOverPanel.SetActive(false);
        audioSource.Stop(); // Dừng nhạc kết thúc nếu đang phát
        BackgroundMusicManager.Instance.PlayMusic(); // Phát lại nhạc nền
        boardManager.RestartGame(); // Reset logic trò chơi trong BoardManager
    }

    private void ReturnToMainMenu()
    {
        gameOverPanel.SetActive(false);
        StartMenuPanel.SetActive(true);
        if (Background != null)
            Background.SetActive(true);
        if (GameBackground != null)
            GameBackground.SetActive(false);
        audioSource.Stop(); // Dừng nhạc kết thúc nếu đang phát
        BackgroundMusicManager.Instance.StopMusic(); // Dừng nhạc nền
    }
  public GameObject GameBackground; // Tham chiếu đến GameObject GameBackground
    
    public void HideStartMenuPanel()
    {
        StartMenuPanel.SetActive(false);      // Ẩn menu chính khi bấm start
    
        if (Background != null)
        {
            Background.SetActive(false);     // Ẩn background của Main Menu
        }
    
        if (GameBackground != null)
        {
            GameBackground.SetActive(true);  // Hiển thị background mới
        }

        BackgroundMusicManager.Instance.PlayMusic(); // Bắt đầu phát nhạc nền
    }

    public GameObject Background; // Tham chiếu đến GameObject Background
    public void ShowBackground(bool show)
    {
        if (Background != null)
        {
            Background.SetActive(show); // Hiện hoặc ẩn background
        }
        else
        {
            Debug.LogWarning("Background GameObject is not assigned in the inspector.");
        }
    }

  
    public void ToggleMute()
    {
        // Bật/tắt tiếng
        BackgroundMusicManager.Instance.ToggleMute();

        // Đổi trạng thái mute
        isMuted = !isMuted;

        // Đổi hình ảnh của nút
        if (muteButton != null)
        {
            Image buttonImage = muteButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.sprite = isMuted ? muteSprite : unmuteSprite;
            }
        }
    }

}
