using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum CardType
{
    ReverseTurn, // Đổi chiều chơi
    SkipTurn,    // Bỏ lượt
    ExtraTurn    // Thêm lượt
}
public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;
    private static GUIManager guiManager;

    public Cell[] cells;        // Gán các Cell vào đây qua Inspector
    public int playerTurnLeft = 0;
    public int playerScore = 0; // Quản lý điểm số của người chơi
    public int computerScore = 0; // Quản lý điểm số của máy
    private bool isReverseTurn = false; // Quản lý chiều chơi
    private CardType? playerCard = null; // Người chơi chỉ có thể giữ 1 lá bài
    private int calcNextIndex(int currentIndex)
    {
        int nextIdx = (currentIndex + (isReverseTurn ? -1 : 1));
        if (nextIdx < 0)
            nextIdx += cells.Length;
        if (nextIdx > cells.Length - 1)
            nextIdx -= cells.Length;
        return nextIdx;
    }
    private int calcNextNextIndex(int currentIndex)
    {
        int nextIdx = (currentIndex + (isReverseTurn ? -2 : 2));
        if (nextIdx < 0)
            nextIdx += cells.Length;
        if (nextIdx > cells.Length - 1)
            nextIdx -= cells.Length;
        return nextIdx;
    }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        guiManager = GUIManager.Instance; // Lấy tham chiếu đến GUIManager
        UseCardButton.onClick.AddListener(OnUseCardButtonClicked); // Gán sự kiện cho nút sử dụng lá bài
        RestartGame(); // Khởi động lại trò chơi
    }

    public void RestartGame()
    {
        Screen.fullScreen = true;
        isReverseTurn = false;
        playerCard = null;
        playerScore = 0;
        computerScore = 0;
        playerTurnLeft = 1;
        guiManager.Update(); // Cập nhật GUIManager
        InitializeBoard(); // Khởi tạo lại bàn cờ
    }

    public void InitializeBoard()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].index = i; // Gán index cho từng ô
            if (i == 0 || i == 6) // Quan trái, phải
                cells[i].SetCount(10);
            else
            {
                cells[i].SetCount(5);
                ColorBlock colorBlock = cells[i].button.colors;
                colorBlock.disabledColor = colorBlock.normalColor; // Giữ nguyên màu sắc khi bị disable
                cells[i].button.colors = colorBlock;
            }
        }
        for (int i = 1; i < 6; i++)
            cells[i].button.interactable = true;
        for (int i = 7; i < 12; i++)
            cells[i].button.interactable = false;
    }

    public IEnumerator OnCellClicked(Cell clickedCell)
    {
        if (playerTurnLeft < 1) yield break; // Không cho phép click nếu không phải lượt người chơi
        StartCoroutine(PlayerMove(clickedCell.index)); // Gọi hàm PlayerMove để thực hiện nước đi
        yield return null; // Đợi cho coroutine hoàn thành
    }

    
    private IEnumerator moveFromCell(int startIndex)
    {
        int i = startIndex;
        int nextIndex = calcNextIndex(startIndex);
        int nextnextIndex = calcNextNextIndex(startIndex); // Ô tiếp theo sau ô hiện tại
        while (true)
        {
            int stones = cells[startIndex].stoneCount;
            Debug.Log($"Vừa nhặt {stones} quân từ ô {i}.");
            yield return StartCoroutine(cells[startIndex].ChangeColor(Cell.HIGHLIGHT_COLOR2, 0f));
            yield return StartCoroutine(cells[startIndex].ResetColor(0.8f));
            yield return StartCoroutine(cells[startIndex].SetCount(0, 0f)); // Giảm số quân trong ô hiện tại
            guiManager.UpdateStoneLeft(stones); // Cập nhật số quân còn lại trên GUIManager
            while (stones > 0)
            {
                yield return new WaitForSeconds(0.2f); // Thời gian chờ giữa các nước đi
                yield return i = calcNextIndex(i);
                yield return StartCoroutine(cells[i].SetCount(cells[i].stoneCount + 1, 0.01f));
                yield return StartCoroutine(cells[i].AnimateStone()); // Hiệu ứng quân cờ
                yield return stones--;
            }
            // Bốc bàiiiiiiiiiiii
            if (playerTurnLeft > 0 && stones == 0 && (i == 0 || i == 6)) // Quân cờ cuối cùng rơi vào ô quan
            {
                if (playerCard != null) // Nếu người chơi đã có lá bài
                    guiManager.ShowNotice("Bạn đã có một lá bài! Tiếp tục lượt chơi.");
                else
                {
                    // Hiển thị lựa chọn bốc lá bài hoặc đi tiếp
                    // bool drawCard = guiManager.ShowChoice("Bạn muốn bốc lá bài hay đi tiếp?");
                    bool drawCard = true;
                    // Mặc định là cho bốc bài đi, còn lựa còn gì thì UI xong tính sau.
                    if (drawCard)
                    {
                        playerCard = DrawCard(); // Bốc lá bài
                        guiManager.ShowNotice($"Bạn đã bốc được lá bài: {playerCard}");
                        yield break; // Kết thúc lượt
                    }
                    else
                        guiManager.ShowNotice("Bạn đã chọn đi tiếp!");
                }
            }
            // End bốc bàiiiiiiiiiiiiiii


            yield return new WaitForSeconds(0.2f); // Thời gian chờ giữa các nước đi
            yield return nextIndex = calcNextIndex(i);
            yield return nextnextIndex = calcNextNextIndex(i); // Ô tiếp theo sau ô hiện tại
            
            if (nextIndex == 0 || nextIndex == 6)
                yield break; // Nếu ô tiếp theo là ô trái hoặc phải thì kết thúc lượt chơi
            else if (cells[nextIndex].stoneCount > 0) // Nếu ô tiếp theo có quân
            {
                yield return startIndex = nextIndex;
                yield return i = nextIndex;
            }
            else
            {
                while (cells[nextIndex].stoneCount == 0 && cells[nextnextIndex].stoneCount > 0)
                {
                    yield return new WaitForSeconds(0.2f); // Thời gian chờ giữa các nước đi    
                    if (playerTurnLeft > 0) // Nếu là lượt người chơi
                    {
                        playerScore += cells[nextnextIndex].stoneCount; // Cộng điểm cho người chơi
                        Debug.Log("Điểm của người chơi: " + playerScore);
                    }
                    else // Nếu là lượt máy
                    {
                        computerScore += cells[nextnextIndex].stoneCount; // Cộng điểm cho máy
                        Debug.Log("Điểm của máy: " + computerScore);
                    }
                    yield return StartCoroutine(cells[nextnextIndex].SetCount(0, 0f));
                    yield return StartCoroutine(cells[nextnextIndex].AnimateStone()); // Hiệu ứng quân cờ
                    yield return nextIndex = calcNextIndex(nextnextIndex);
                    yield return nextnextIndex = calcNextNextIndex(nextnextIndex);
                    yield return new WaitForSeconds(0.2f); // Thời gian chờ giữa các nước đi
                }
                yield break;
            }
        }
    }

    private bool CheckEndGame()
    {
        bool playerNoMoves = true;
        bool aiNoMoves = true;

        // Check if player has no moves
        for (int i = 1; i <= 5; i++)
        {
            if (cells[i].stoneCount > 0)
            {
                playerNoMoves = false;
                break;
            }
        }

        // Check if AI has no moves
        for (int i = 7; i <= 11; i++)
        {
            if (cells[i].stoneCount > 0)
            {
                aiNoMoves = false;
                break;
            }
        }

        // Check if both sides have no stones
        bool noStonesLeft = (cells[0].stoneCount == 0 && cells[6].stoneCount == 0);

        return (playerNoMoves || aiNoMoves || noStonesLeft);
    }

    private IEnumerator CalculateFinalScores()
    {
        // Add remaining stones to respective scores
        for (int i = 0; i <= 5; i++)
        {
            yield return new WaitForSeconds(0.5f); // Thời gian chờ giữa các nước đi
            playerScore += cells[i].stoneCount;
            cells[i].SetCount(0);
            yield return StartCoroutine(cells[i].AnimateStone());
        }

        for (int i = 6; i <= 11; i++)
        {
            yield return new WaitForSeconds(0.5f); // Thời gian chờ giữa các nước đi
            computerScore += cells[i].stoneCount;
            cells[i].SetCount(0);
            yield return StartCoroutine(cells[i].AnimateStone());
        }

        Debug.Log("Game Over!");
        Debug.Log("Player Score: " + playerScore);
        Debug.Log("Computer Score: " + computerScore);

        guiManager.Update(); // Update the GUI to reflect final scores
    }

    public IEnumerator PlayerMove(int startIndex)
    {
        Debug.Log("Người chơi đã chọn ô: " + startIndex);
        if (cells[startIndex].stoneCount == 0)
        {
            Debug.Log("Ô được chọn không có quân!");
            guiManager.ShowNotice("Ô được chọn không có quân!");
            yield break;
        }
        
        for (int i = 1; i < 6; i++)
            cells[i].button.interactable = false;
        yield return canUseCard = false;

        yield return new WaitForSeconds(0.5f); // Thời gian chờ giữa các nước đi
        yield return StartCoroutine(moveFromCell(startIndex)); // Gọi hàm moveFromCell để thực hiện nước đi
        guiManager.Update(); // Cập nhật GUIManager sau khi người chơi đi xong

        if (CheckEndGame())
        {
            Debug.Log("Game Over!");
            guiManager.ShowNotice("Trò chơi kết thúc!\nBắt đầu tính điểm!", 5f);
            yield return StartCoroutine(CalculateFinalScores()); // Tính điểm cuối cùng nếu trò chơi kết thúc
            guiManager.ShowGameOverPanel(playerScore, computerScore); // Hiển thị kết quả trò chơi
            yield break; // Check for endgame condition
        }

        yield return new WaitForSeconds(1f); // Thời gian chờ trước khi máy đi
        
        playerTurnLeft--;
        yield return canUseCard = true;
        if (playerTurnLeft > 0)
        {
            for (int i = 1; i < 6; i++)
                yield return cells[i].button.interactable = true;
            yield break;
        }
        Debug.Log("Máy đi!");
        yield return StartCoroutine(ComputerMove()); // Máy đi sau 1 giây
    }

    private int EvaluateBoard(SimState state)
    {
        // Heuristic evaluation: AI tries to maximize its score and minimize the player's score
        int aiStones = 0, playerStones = 0;

        // Count stones in AI's cells
        for (int i = 7; i <= 11; i++)
            aiStones += state.board[i];

        // Count stones in Player's cells
        for (int i = 1; i <= 5; i++)
            playerStones += state.board[i];

        // Add weights to prioritize capturing stones and controlling the board
        int aiCapturePotential = 0, playerCapturePotential = 0;

        for (int i = 7; i <= 11; i++) // AI's cells
        {
            int nextIndex = calcNextIndex(i);
            int captureIndex = calcNextIndex(nextIndex);
            if (state.board[nextIndex] == 0 && state.board[captureIndex] > 0)
                aiCapturePotential += state.board[captureIndex];
        }

        for (int i = 1; i <= 5; i++) // Player's cells
        {
            int nextIndex = calcNextIndex(i);
            int captureIndex = calcNextIndex(nextIndex);
            if (state.board[nextIndex] == 0 && state.board[captureIndex] > 0)
                playerCapturePotential += state.board[captureIndex];
        }

        // Heuristic formula: prioritize AI's score, stones, and capture potential
        return (state.computerScore + aiStones + aiCapturePotential) - (state.playerScore + playerStones + playerCapturePotential);
    }

    private int Minimax(SimState state, int depth, bool isMaximizingPlayer, int alpha, int beta)
    {
        if (depth == 0 || CheckEndGameSim(state))
            return EvaluateBoard(state); // Use EvaluateBoard instead of EvaluateSimState

        if (isMaximizingPlayer)
        {
            int maxEval = int.MinValue;
            for (int i = 7; i <= 11; i++) // AI's cells
            {
                if (state.board[i] > 0)
                {
                    SimState simulatedState = SimulateMove(state, i); // Use SimState for simulation
                    int eval = Minimax(simulatedState, depth - 1, false, alpha, beta);
                    maxEval = Mathf.Max(maxEval, eval);
                    alpha = Mathf.Max(alpha, eval);
                    if (beta <= alpha) break; // Alpha-Beta pruning
                }
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            for (int i = 1; i <= 5; i++) // Player's cells
            {
                if (state.board[i] > 0)
                {
                    SimState simulatedState = SimulateMove(state, i); // Use SimState for simulation
                    int eval = Minimax(simulatedState, depth - 1, true, alpha, beta);
                    minEval = Mathf.Min(minEval, eval);
                    beta = Mathf.Min(beta, eval);
                    if (beta <= alpha) break; // Alpha-Beta pruning
                }
            }
            return minEval;
        }
    }

    private bool CheckEndGameSim(SimState state)
    {
        bool playerNoMoves = true;
        bool aiNoMoves = true;

        // Check if player has no moves
        for (int i = 1; i <= 5; i++)
        {
            if (state.board[i] > 0)
            {
                playerNoMoves = false;
                break;
            }
        }

        // Check if AI has no moves
        for (int i = 7; i <= 11; i++)
        {
            if (state.board[i] > 0)
            {
                aiNoMoves = false;
                break;
            }
        }

        // Check if both sides have no stones
        bool noStonesLeft = (state.board[0] == 0 && state.board[6] == 0);

        return (playerNoMoves || aiNoMoves || noStonesLeft);
    }

    private class SimState
    {
        public List<int> board; // Simplified to store only stone counts
        public int playerScore;
        public int computerScore;
        public bool isPlayerTurn;

        public SimState(Cell[] cells, int _playerScore, int _computerScore, bool _isPlayerTurn)
        {
            board = new List<int>();
            foreach (var cell in cells)
            {
                board.Add(cell.stoneCount); // Only store stone counts
            }
            playerScore = _playerScore;
            computerScore = _computerScore;
            isPlayerTurn = _isPlayerTurn;
        }
    }

    private SimState SimulateMove(SimState state, int index)
    {
        var simulatedState = new SimState(cells, state.playerScore, state.computerScore, state.isPlayerTurn); // Corrected initialization
        int i = index;
        int count = simulatedState.board[i];
        simulatedState.board[i] = 0;

        // Distribute stones
        while (count > 0)
        {
            i = calcNextIndex(i); // Use calcNextIndex to determine the next index
            simulatedState.board[i]++;
            count--;
        }

        // Handle capturing logic
        int next = calcNextIndex(i); // Use calcNextIndex for the next index
        while (simulatedState.board[next] == 0)
        {
            int captureIndex = calcNextIndex(next); // Use calcNextIndex for the capture index
            if (simulatedState.board[captureIndex] > 0)
            {
                simulatedState.playerScore += simulatedState.board[captureIndex];
                simulatedState.board[captureIndex] = 0;
            }
            else
            {
                break;
            }
            next = calcNextIndex(captureIndex); // Update next using calcNextIndex
        }

        return simulatedState;
    }

    public IEnumerator ComputerMove()
    {
        int bestMove = -1;
        int bestValue = int.MinValue;

        for (int i = 7; i <= 11; i++)
        {
            if (cells[i].stoneCount > 0)
            {
                SimState simulatedState = SimulateMove(new SimState(cells, playerScore, computerScore, false), i);
                int moveValue = Minimax(simulatedState, 3, false, int.MinValue, int.MaxValue); // Updated to match Minimax signature

                if (moveValue > bestValue)
                {
                    bestValue = moveValue;
                    bestMove = i;
                }
            }
        }

        if (bestMove == -1)
        {
            Debug.Log("Máy không còn nước đi!");
            guiManager.ShowNotice("Trò chơi kết thúc!\nBắt đầu tính điểm!");
            yield return StartCoroutine(CalculateFinalScores());
            yield break;
        }

        yield return StartCoroutine(moveFromCell(bestMove)); // Execute the best move
        guiManager.Update(); // Update GUI after AI's move

        if (CheckEndGame())
        {
            guiManager.ShowNotice("Trò chơi kết thúc!\nBắt đầu tính điểm!");
            yield return StartCoroutine(CalculateFinalScores()); // Tính điểm cuối cùng nếu trò chơi kết thúc
            guiManager.ShowGameOverPanel(playerScore, computerScore); // Show game over panel
            Debug.Log("Game Over!");
            yield break;
        }

        playerTurnLeft++;
        for (int i = 1; i < 6; i++)
            cells[i].button.interactable = true;
    }

    private CardType DrawCard()
    {
        int randomValue = Random.Range(1, 101); // Random từ 1 đến 100
        if (randomValue < 51) return CardType.ReverseTurn; // 50% Đổi chiều chơi
        else if (randomValue <= 76) return CardType.SkipTurn; // 25% Bỏ lượt
        else return CardType.ExtraTurn; // 25% Thêm lượt
    }
    public Button UseCardButton; // Nút sử dụng lá bài
    private bool canUseCard = true; // Biến kiểm tra xem có thể sử dụng lá bài hay không
    public void OnUseCardButtonClicked()
    {
        if (playerTurnLeft <= 0)
            guiManager.ShowNotice("Bạn không thể sử dụng lá bài\ntrong lượt của máy!");
        else if (canUseCard)
            StartCoroutine(OnUseCardButtonClicked_Mutex()); // Gọi coroutine để xử lý click
        else
            guiManager.ShowNotice("Bạn chỉ có thể sử dụng lá bài\ntrước khi thực hiện lượt chơi!");
    }
    private bool cardMutex = false;
    private IEnumerator OnUseCardButtonClicked_Mutex()
    {
        Debug.Log("Đã nhấn nút sử dụng lá bài!");
        if (cardMutex) yield break; // Nếu đang sử dụng lá bài, không cho phép click nữa
        yield return cardMutex = true; // Đánh dấu là đang sử dụng lá bài
        if (playerCard == null)
        {
            guiManager.ShowNotice("Bạn không có lá bài nào để sử dụng!");
            yield return cardMutex = false; // Đánh dấu là không còn sử dụng lá bài nữa
            yield break;
        }
        yield return new WaitForSeconds(0.5f); // Thời gian chờ giữa các nước đi
        switch (playerCard)
        {
            case CardType.ReverseTurn:
                yield return isReverseTurn = !isReverseTurn; // Đổi chiều chơi
                guiManager.ShowNotice("Lá bài Đổi chiều chơi đã được sử dụng!");
                break;

            case CardType.SkipTurn:
                yield return playerTurnLeft = 0; // Bỏ lượt
                guiManager.ShowNotice("Lá bài Bỏ lượt đã được sử dụng!");
                yield return StartCoroutine(ComputerMove()); // Máy đi ngay lập tức
                break;

            case CardType.ExtraTurn:
                yield return playerTurnLeft++;
                guiManager.ShowNotice("Lá bài Thêm lượt đã được sử dụng! Bạn được đi thêm một lượt.");
                break;
        }

        yield return new WaitForSeconds(0.5f); // Thời gian chờ giữa các nước đi
        yield return playerCard = null; // Xóa lá bài sau khi sử dụng
        
        yield return cardMutex = false; // Đánh dấu là không còn sử dụng lá bài nữa
    }
}