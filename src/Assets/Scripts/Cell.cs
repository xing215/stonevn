using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Cell : MonoBehaviour
{
    public int index;                         // Vị trí ô trong bàn cờ
    public int stoneCount;                    // Số quân hiện tại trong ô
    public TextMeshProUGUI countText;         // Text để hiển thị số quân
    public Button button;                     // Button để bắt sự kiện click
    public Image cellImage;                   // Hình ảnh của ô
    public static Color HIGHLIGHT_COLOR = new Color(174f / 255f, 176f / 255f, 236f / 255f); // AEB0EC
    public static Color HIGHLIGHT_COLOR2 = new Color(0f / 255f, 0f / 255f, 0f / 255f);
    public static Color DEFAULT_COLOR = new Color(200f / 255f, 209f / 255f, 238f / 255f); // C8D1EE
    public static Color DEFAULT_OUTLINE_COLOR = new Color(1f, 1f, 1f, 1f); // Màu outline mặc định
    public static Color SELECTED_OUTLINE_COLOR = new Color(1f, 0.5f, 0f, 1f); // Màu outline khi được chọn
    private Outline outline; // Tham chiếu đến Outline để tạo hiệu ứng viền
    public AudioClip clickSound; // Âm thanh khi nhấn vào ô
    private AudioSource audioSource; // Nguồn phát âm thanh

    private void Start()
    {
        outline = GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = false; // Tắt outline ban đầu
            outline.effectColor = DEFAULT_OUTLINE_COLOR; // Đặt màu mặc định
        }

        if (index == 0 || index == 6) // Quan trái, phải
            SetCount(10);
        else
        {
            SetCount(5);
            button.onClick.AddListener(OnClick);
        }

        // Gán AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void SetCount(int count)
    {
        stoneCount = count;
        countText.text = count.ToString();
    }
    public IEnumerator SetCount(int count, float delay)
    {
        yield return new WaitForSeconds(delay);
        SetCount(count);
    }

    public void OnClick()
    {
        if (index > 0 && index < 6) 
        {
            PlayClickSound(); // Phát âm thanh khi nhấn
            StartCoroutine(OnCellClickedCoroutine()); // Gọi Coroutine để xử lý click
        }
    }

    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound); // Phát âm thanh một lần
        }
    }

    private bool isCoroutineRunning = false;
    private IEnumerator OnCellClickedCoroutine()
    {
        if (isCoroutineRunning) yield break; // Nếu Coroutine đang chạy, không làm gì cả
        yield return isCoroutineRunning = true;

        Debug.Log("Cell clicked: " + index);
        HighlightCell(true, 0.1f); // Hiển thị outline với delay 0.1 giây
        yield return BoardManager.Instance.OnCellClicked(this); // Gọi Coroutine từ BoardManager
        HighlightCell(false, 0.5f); // Tắt outline với delay 0.5 giây

        yield return isCoroutineRunning = false; // Đặt lại trạng thái sau khi Coroutine hoàn thành
        yield return new WaitForSeconds(0.5f); // Thời gian chờ trước khi tiếp tục
    }

    public void HighlightCell(bool highlight, float delay = 0f)
    {
        if (outline != null)
        {
            StartCoroutine(HighlightWithDelay(highlight, delay));
        }
    }

    private IEnumerator HighlightWithDelay(bool highlight, float delay)
    {
        yield return new WaitForSeconds(delay); // Thêm thời gian chờ
        outline.enabled = highlight; // Bật/tắt outline
        outline.effectColor = highlight ? SELECTED_OUTLINE_COLOR : DEFAULT_OUTLINE_COLOR; // Đổi màu outline
    }

    public void ChangeColor(Color color)
    {
        // Không cần đổi màu, logic này đã được thay thế bằng outline
    }
    public void ResetColor()
    {
        // Không cần reset màu, logic này đã được thay thế bằng outline
    }
    public IEnumerator ChangeColor(Color color, float delay)
    {
        yield return new WaitForSeconds(delay);
        ChangeColor(color);
    }
    public IEnumerator ResetColor(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetColor();
    }

    public IEnumerator AnimateStone()
    {
        HighlightCell(true); // Bật hiệu ứng highlight
        // Scale up
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.2f;
        float duration = 0.2f;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Scale down
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
        HighlightCell(false); // Tắt hiệu ứng highlight
    }

}