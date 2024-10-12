using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuGame : MonoBehaviour {

    // 游戏模型
    private int[,,] sudokuBoard = new int[9, 9, 2]; // 数独棋盘
    private bool gameOver = false; // 游戏是否结束
    private int difficulty = 1; // 游戏难度：1: 简单, 2: 中等, 3: 困难
    private bool selectingDifficulty = true; // 是否在选择难度
    private int selectedRow = -1; // 当前选中的行
    private int selectedCol = -1; // 当前选中的列
    private bool waitingForInput = false; // 是否在等待键盘输入
    private string inputBuffer = ""; // 输入缓冲区

    // 系统处理器
    void Start () {
        Init(); // 初始化游戏
    }

    // 渲染视图
    void OnGUI() {
        // 选择难度界面
        if (selectingDifficulty) {
            GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
            // 获取屏幕的宽度和高度
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;
            // 设置矩形的宽度和高度
            int rectWidth = 300;
            int rectHeight = 200;
            // 计算矩形的中心位置
            int rectX = (screenWidth - rectWidth) / 2;
            int rectY = (screenHeight - rectHeight) / 2;
            GUI.Box(new Rect(rectX, rectY, rectWidth, rectHeight), "选择难度");
            if (GUI.Button(new Rect((screenWidth - 200) / 2, (screenHeight - 30) / 2 - 50, 200, 30), "简单")) {
                difficulty = 1; // 设置难度为简单
                selectingDifficulty = false; // 结束选择难度
                Init(); // 初始化游戏
            }
            if (GUI.Button(new Rect((screenWidth - 200) / 2, (screenHeight - 30) / 2, 200, 30), "中等")) {
                difficulty = 2; // 设置难度为中等
                selectingDifficulty = false; // 结束选择难度
                Init(); // 初始化游戏
            }
            if (GUI.Button(new Rect((screenWidth - 200) / 2, (screenHeight - 30) / 2 + 50, 200, 30), "困难")) {
                difficulty = 3; // 设置难度为困难
                selectingDifficulty = false; // 结束选择难度
                Init(); // 初始化游戏
            }
            GUI.EndGroup();
        } else {
            // 游戏界面
            GUI.Box(new Rect(50, 25, 630, 630), ""); // 绘制游戏背景框
            GUI.Box(new Rect(50, 25, 630, 630), ""); // 绘制游戏背景框
            GUI.Box(new Rect(50, 25, 630, 630), ""); // 绘制游戏背景框
            if (GUI.Button(new Rect(310, 670, 100, 30), "再来一次")) Init(); // 再来一次按钮
            if (GUI.Button(new Rect(50, 670, 100, 30), "返回主界面")) {
                selectingDifficulty = true; // 返回到选择难度界面
                Init(); // 初始化游戏
            }
            if (!gameOver) {
                for (int i = 0; i < 9; i++) {
                    for (int j = 0; j < 9; j++) {
                        if (sudokuBoard[i, j, 0] == 0) {
                            // 如果格子为空，绘制按钮
                            if (GUI.Button(new Rect(50 + j * 70, 25 + i * 70, 70, 70), "")) {
                                selectedRow = i;
                                selectedCol = j;
                                waitingForInput = true; // 设置等待键盘输入状态
                            }
                        } else {
                            // 如果格子有数字，绘制按钮并显示数字
                            GUI.Button(new Rect(50 + j * 70, 25 + i * 70, 70, 70), sudokuBoard[i, j, 0].ToString());
                        }
                    }
                }
                // 读取鼠标的坐标
                Vector2 mousePosition = Event.current.mousePosition;
                // 检测鼠标左键按下
                if (Input.GetMouseButtonDown(0) && !waitingForInput)
                {
                    // 当鼠标按下时设置selectedRow和selectedCol
                    int row = Mathf.FloorToInt((mousePosition.y - 25) / 70);
                    int col = Mathf.FloorToInt((mousePosition.x - 50) / 70);
                    if (sudokuBoard[row, col, 1] != 1 && row >= 0 && row < 9 && col >= 0 && col < 9) {
                        selectedRow = row;
                        selectedCol = col;
                        waitingForInput = true; // 设置等待键盘输入状态
                    }
                }

                if (waitingForInput) {
                    // 在右下角显示selectedRow和selectedCol的值
                    int screenWidth = Screen.width;
                    int screenHeight = Screen.height;
                    GUI.Box(new Rect(screenWidth - 160, screenHeight - 50, 120, 40), "Selected Row: " + (selectedRow+1) + "\nSelected Col: " + (selectedCol+1));
                    foreach (char c in Input.inputString)
                    {
                        // 判断输入是否为数字1-9
                        if (char.IsDigit(c) && c != '0')
                        {
                            // 将数字写入数据
                            int number = int.Parse(c.ToString());
                            sudokuBoard[selectedRow, selectedCol, 0] = number; // 更新数独棋盘
                            selectedRow = -1; // 重置选中的行
                            selectedCol = -1; // 重置选中的列
                            waitingForInput = false; // 重置等待键盘输入状态

                            // 检查数独是否完成
                            if (CheckSudoku()) {
                                gameOver = true;
                            }
                        }
                        // 检测 Delete 键或 Backspace 键
                        else if (c == '\b' || c == '\u007F') // '\b' 是 Backspace，'\u007F' 是 Delete
                        {
                            // 清除选中的格子
                            sudokuBoard[selectedRow, selectedCol, 0] = 0; // 清除数字
                            selectedRow = -1; // 重置选中的行
                            selectedCol = -1; // 重置选中的列
                            waitingForInput = false; // 重置等待键盘输入状态
                        }
                    }
                }
            } else {
                // 游戏结束，显示祝贺信息
                GUI.Box(new Rect(260, 50, 200, 200), "\n\n\n\n\n恭喜你!\n 你解决了数独。");
            }
        }
    }

    // 初始化游戏
    void Init() {
        sudokuBoard = new int[9, 9, 2]; // 初始化数独棋盘
        gameOver = false; // 重置游戏结束状态
        GenerateSudoku(difficulty); // 生成数独
    }

    // 生成数独
    void GenerateSudoku(int difficulty) {
        // 清空棋盘
        for (int i = 0; i < 9; i++) {
            for (int j = 0; j < 9; j++) {
                sudokuBoard[i, j, 0] = 0;
                sudokuBoard[i, j, 1] = 0;
            }
        }

        // 生成一个完整的有效数独棋盘
        FillBoard(sudokuBoard);

        // 根据难度移除数字
        int removeCount = 0;
        switch (difficulty) {
            case 1: // 简单
                removeCount = 40;
                break;
            case 2: // 中等
                removeCount = 49;
                break;
            case 3: // 困难
                removeCount = 58;
                break;
        }

        RemoveNumbers(sudokuBoard, removeCount);
    }

    // 递归填充数独棋盘
    bool FillBoard(int[,,] board) {
        for (int i = 0; i < 9; i++) {
            for (int j = 0; j < 9; j++) {
                if (board[i, j, 0] == 0) {
                    List<int> possibleNumbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                    Shuffle(possibleNumbers); // 随机打乱数字列表
                    foreach (int num in possibleNumbers) {
                        if (IsValid(board, i, j, num)) {
                            board[i, j, 0] = num; // 填入数字
                            board[i, j, 1] = 1; // 标记为初始数字
                            if (FillBoard(board)) {
                                return true; // 如果成功填充，返回true
                            }
                            board[i, j, 0] = 0; // 回溯
                            board[i, j, 1] = 0;
                        }
                    }
                    return false; // 如果没有数字可以填入，返回false
                }
            }
        }
        return true; // 所有格子都填满，返回true
    }

    // 检查数字是否有效
    bool IsValid(int[,,] board, int row, int col, int num) {
        for (int i = 0; i < 9; i++) {
            // 检查行、列和3x3子区域是否有重复数字
            if (board[row, i, 0] == num || board[i, col, 0] == num || board[row / 3 * 3 + i / 3, col / 3 * 3 + i % 3, 0] == num) {
                return false; // 如果有重复数字，返回false
            }
        }
        return true; // 没有重复数字，返回true
    }

    // 随机打乱列表
    void Shuffle<T>(List<T> list) {
        for (int i = list.Count - 1; i > 0; i--) {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    // 移除数字
    void RemoveNumbers(int[,,] board, int count) {
        while (count > 0) {
            int row = Random.Range(0, 9);
            int col = Random.Range(0, 9);
            if (board[row, col, 0] != 0) {
                board[row, col, 0] = 0; // 移除数字
                board[row, col, 1] = 0; // 标记为非初始数字
                count--;
            }
        }
    }

    // 检查数独是否完成
    bool CheckSudoku() {
        for (int i = 0; i < 9; i++) {
            for (int j = 0; j < 9; j++) {
                if (sudokuBoard[i, j, 0] == 0) {
                    return false; // 如果有空格，返回false
                }
            }
        }

        // 检查每一行、每一列和每一个3x3子区域
        for (int i = 0; i < 9; i++) {
            if (!CheckRow(i) || !CheckColumn(i) || !CheckSubGrid(i)) {
                return false;
            }
        }

        return true; // 所有格子都填满且有效，返回true
    }

    bool CheckRow(int row) {
        bool[] used = new bool[9];
        for (int col = 0; col < 9; col++) {
            int num = sudokuBoard[row, col, 0];
            if (num < 1 || num > 9 || used[num - 1]) {
                return false;
            }
            used[num - 1] = true;
        }
        return true;
    }

    bool CheckColumn(int col) {
        bool[] used = new bool[9];
        for (int row = 0; row < 9; row++) {
            int num = sudokuBoard[row, col, 0];
            if (num < 1 || num > 9 || used[num - 1]) {
                return false;
            }
            used[num - 1] = true;
        }
        return true;
    }

    bool CheckSubGrid(int subGridIndex) {
        bool[] used = new bool[9];
        int startRow = (subGridIndex / 3) * 3;
        int startCol = (subGridIndex % 3) * 3;
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                int num = sudokuBoard[startRow + i, startCol + j, 0];
                if (num < 1 || num > 9 || used[num - 1]) {
                    return false;
                }
                used[num - 1] = true;
            }
        }
        return true;
    }
}
