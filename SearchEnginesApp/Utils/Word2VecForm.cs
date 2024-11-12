using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using MathNet.Numerics.LinearAlgebra;
using static MathNet.Numerics.LinearAlgebra.Double.DenseVector;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Collections.Concurrent;
using System.IO;
using System.Numerics;
using MathVector = MathNet.Numerics.LinearAlgebra.Vector<double>;
using VectorBuilder = MathNet.Numerics.LinearAlgebra.Double.DenseVector;
using System.Text;


namespace SearchEnginesApp.Utils
{
    public partial class Word2VecForm : Form
    {
        private Button btnVisualize;
        private Button btnToggleMode;
        private Button btnLoadStopWords;
        private Button btnShowStopWords;
        private Button btnSelectWords;
        private Label lblStatus;
        private Label lblWordCount;
        private Label lblFilter;
        private TextBox txtWordSelection;
        private TextBox txtFilter;
        private ListBox lstTokens;
        private PictureBox pictureBox;
        private ComboBox cmbFilterType;
        private CheckBox chkCaseSensitive;
        private TrackBar trackBarRotate;
        private CheckBox chkCbow;
        private Button btnRetrainCBOW;
        private Label lblTrainingStatus;

        private WordEmbedding wordEmbedding;

        private bool isDragging = false;
        private bool is3DMode = false;
        private float rotationAngle = 0;
        private float zoomFactor = 1.0f;
        private string databasePath;
        private int totalTokenCount = 0;

        private Point lastMousePos;
        private List<string> availableWords;
        private List<string> originalAvailableWords;

        private static readonly HashSet<string> StopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // 英文停用字
            "a", "an", "and", "are", "as", "at", "be", "by", "for", "from",
            "has", "he", "in", "is", "it", "its", "of", "on", "that", "the",
            "to", "was", "were", "will", "with", "the", "this", "but", "they",
            "have", "had", "what", "when", "where", "who", "which", "why", "how",
            "we","or","these","not","then","also","been","no", "our", "can", "both",
            "more", "there","those","into","one","two","three","four","five","six","seven","eight","nine","ten",
            
            // 數字和單個字母
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "0",
            "a", "b", "c", "d", "e", "f", "g", "h", "i", "j",
            "k", "l", "m", "n", "o", "p", "q", "r", "s", "t",
            "u", "v", "w", "x", "y", "z",
            
            // 特殊字符
            ",", ".", "!", "?", ";", ":", "'", "\"", "(", ")", "[", "]",
            "{", "}", "/", "\\", "|", "<", ">", "+", "-", "*", "=", "_"
        };


        public Word2VecForm(List<string> words, string dbPath = "")
        {
            try
            {
                databasePath = dbPath;
                zoomFactor = 1.0f;
                rotationAngle = 0;
                is3DMode = false;

                InitializeCBOWControls();
                InitializeFormSettings();
                InitializeBaseControls();
                InitializeFilterControls();
                InitializeWordSelectionControls(); // 新增初始化文字選擇控件
                AddContextMenu();

                InitializeData(words);

                // 初始化時就按頻率排序
                var cmbSort = Controls.Find("cmbSortType", true).FirstOrDefault() as ComboBox;
                if (cmbSort != null)
                {
                    cmbSort.SelectedIndex = 0; // "Frequency (High to Low)"
                    UpdateListBoxWithFrequency(totalTokenCount);
                }

                // 更新視窗標題加入資料庫資訊
                UpdateFormTitle();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing form: {ex.Message}",
                    "Initialization Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void InitializeCBOWControls()
        {
            // CBOW CheckBox
            chkCbow = new CheckBox
            {
                Text = "Enable CBOW",
                Location = new Point(460, 70),
                Checked = true,
                Width = 100
            };
            chkCbow.CheckedChanged += ChkCbow_CheckedChanged;

            // Retrain Button
            btnRetrainCBOW = new Button
            {
                Text = "Retrain Model",
                Location = new Point(800, 40),
                Width = 100,
                Height = 30
            };
            btnRetrainCBOW.Click += BtnRetrainCBOW_Click;

            // Training Status Label
            lblTrainingStatus = new Label
            {
                Location = new Point(910, 50),
                Width = 200,
                Height = 40,
                Text = ""
            };

            Controls.Add(chkCbow);
            Controls.Add(btnRetrainCBOW);
            Controls.Add(lblTrainingStatus);
        }


        private void UpdateFormTitle()
        {
            string baseTitle = $"Word2Vector Visualization";
            if (!string.IsNullOrEmpty(databasePath))
            {
                string dbName = Path.GetFileName(databasePath);
                baseTitle += $" - Database: {dbName}";
            }

            int totalTokens = wordEmbedding?.GetTotalTokenCount() ?? 0;
            int uniqueWords = wordEmbedding?.GetWordFrequencies().Count ?? 0;

            this.Text = $"{baseTitle} - {totalTokens:N0} Total Tokens, {uniqueWords:N0} Unique Words";
        }
        private void InitializeWordSelectionControls()
        {
            // 新增文字選擇框
            txtWordSelection = new TextBox
            {
                Location = new Point(320, 10),
                Size = new Size(400, 25),
                ForeColor = Color.Gray,
                Text = "Enter words separated by commas..."
            };

            // 添加獲得焦點和失去焦點的事件處理
            txtWordSelection.GotFocus += (s, e) =>
            {
                if (txtWordSelection.Text == "Enter words separated by commas...")
                {
                    txtWordSelection.Text = "";
                    txtWordSelection.ForeColor = Color.Black;
                }
            };

            txtWordSelection.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtWordSelection.Text))
                {
                    txtWordSelection.Text = "Enter words separated by commas...";
                    txtWordSelection.ForeColor = Color.Gray;
                }
            };

            btnSelectWords = new Button
            {
                Text = "Select Words",
                Location = new Point(730, 8),
                Size = new Size(100, 25)
            };
            btnSelectWords.Click += BtnSelectWords_Click;

            this.Controls.AddRange(new Control[] { txtWordSelection, btnSelectWords });
        }

        private async void InitializeData(List<string> words)
        {
            try
            {
                if (words == null)
                {
                    words = new List<string>();
                }

                lblTrainingStatus.Text = "Initializing model...";
                btnRetrainCBOW.Enabled = false;

                // 使用 Task.Run 在背景執行緒初始化
                await Task.Run(() =>
                {
                    try
                    {
                        wordEmbedding = new WordEmbedding(words, chkCbow.Checked);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error creating WordEmbedding: {ex.Message}",
                            "Initialization Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                });

                if (wordEmbedding != null)
                {
                    var frequencies = wordEmbedding.GetWordFrequencies();
                    availableWords = frequencies.Select(wf => wf.Word).ToList();
                    originalAvailableWords = new List<string>(availableWords);

                    is3DMode = false;
                    rotationAngle = 0;

                    if (!this.IsDisposed && this.IsHandleCreated)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            try
                            {
                                UpdateListBoxWithFrequency();
                                UpdateVisualization();
                                lblTrainingStatus.Text = "Model initialized successfully";
                                btnRetrainCBOW.Enabled = true;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error updating UI: {ex.Message}",
                                    "Update Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                            }
                        });
                    }
                }
                else
                {
                    MessageBox.Show("Failed to initialize the Word Embedding model.",
                        "Initialization Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in InitializeData: {ex.Message}",
                    "Initialization Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnRetrainCBOW.Enabled = true;
                if (string.IsNullOrEmpty(lblTrainingStatus.Text))
                {
                    lblTrainingStatus.Text = "Initialization completed";
                }
            }
        }
        private async void InitializeWordEmbeddingWithProgress(List<string> words)
        {
            lblTrainingStatus.Text = "Initializing model...";
            btnRetrainCBOW.Enabled = false;

            await Task.Run(() =>
            {
                wordEmbedding = new WordEmbedding(words, chkCbow.Checked);
            });

            lblTrainingStatus.Text = "Model initialized";
            btnRetrainCBOW.Enabled = true;
        }

        private async void ChkCbow_CheckedChanged(object sender, EventArgs e)
        {
            if (!btnRetrainCBOW.Enabled) return;

            var result = MessageBox.Show(
                "Changing CBOW setting requires retraining the model. Do you want to proceed?",
                "Retrain Model",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                await RetrainModel();
            }
            else
            {
                chkCbow.Checked = !chkCbow.Checked;
            }
        }

        private async void BtnRetrainCBOW_Click(object sender, EventArgs e)
        {
            await RetrainModel();
        }

        private async Task RetrainModel()
        {
            try
            {
                btnRetrainCBOW.Enabled = false;
                chkCbow.Enabled = false;
                lblTrainingStatus.Text = "Retraining model...";

                await Task.Run(() =>
                {
                    try
                    {
                        var words = wordEmbedding?.GetOriginalTokens() ?? new List<string>();
                        wordEmbedding = new WordEmbedding(words, chkCbow.Checked);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error in model retraining: {ex.Message}",
                            "Training Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                });

                if (wordEmbedding != null)
                {
                    UpdateVisualization();
                    UpdateSimilarWordsList();
                    lblTrainingStatus.Text = "Model retrained successfully";
                }
                else
                {
                    lblTrainingStatus.Text = "Retraining failed";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retraining model: {ex.Message}",
                    "Training Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                lblTrainingStatus.Text = "Training failed";
            }
            finally
            {
                btnRetrainCBOW.Enabled = true;
                chkCbow.Enabled = true;
            }
        }
        private bool IsWordEmbeddingInitialized()
        {
            if (wordEmbedding == null)
            {
                MessageBox.Show("Word Embedding model is not initialized.",
                    "Initialization Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        private void UpdateVisualization()
        {
            var selectedWords = GetSelectedWords();
            if (selectedWords.Any())
            {
                var vectors = selectedWords
                    .Select(word => (wordEmbedding.GetWordVector(word, is3DMode), word))
                    .ToList();

                if (is3DMode)
                    Draw3DVectors(vectors);
                else
                    Draw2DVectors(vectors);
            }
        }
        private void UpdateSimilarWordsList()
        {
            try
            {
                string mainWord = GetSelectedWord(); // 獲取第一個選中的詞
                if (string.IsNullOrEmpty(mainWord)) return;

                // 獲取相似詞並更新顯示
                var similarWords = wordEmbedding.FindMostSimilarWords(mainWord, 5, is3DMode);
                DisplaySimilarWords(similarWords);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating similar words: {ex.Message}", "Update Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string GetSelectedWord()
        {
            if (txtWordSelection.Text != "Enter words separated by commas..." &&
                !string.IsNullOrWhiteSpace(txtWordSelection.Text))
            {
                // 返回第一個選中的詞
                var words = txtWordSelection.Text.Split(',')
                    .Select(w => w.Trim())
                    .Where(w => !string.IsNullOrEmpty(w))
                    .ToList();
                return words.FirstOrDefault();
            }
            return null;
        }
        private List<string> GetSelectedWords()
        {
            if (txtWordSelection.Text == "Enter words separated by commas..." ||
                   string.IsNullOrWhiteSpace(txtWordSelection.Text))
                return new List<string>();

            return txtWordSelection.Text
                .Split(',')
                .Select(w => w.Trim())
                .Where(w => !string.IsNullOrEmpty(w))
                .ToList();
        }

        // 添加方法來顯示相似詞
        private void DisplaySimilarWords(List<(string Word, double Similarity)> similarWords)
        {
            var similarWordsListBox = Controls.Find("lstSimilarWords", true).FirstOrDefault() as ListBox;
            if (similarWordsListBox != null)
            {
                similarWordsListBox.Items.Clear();
                foreach (var (word, similarity) in similarWords)
                {
                    similarWordsListBox.Items.Add($"{word} ({similarity:F4})");
                }
            }
        }

        private void AnalyzeWordRelationships(List<string> words)
        {
            try
            {
                // 創建新表單來顯示結果
                var resultForm = new Form
                {
                    Text = "詞彙關係矩陣",
                    Width = 800,
                    Height = 600,
                    StartPosition = FormStartPosition.CenterParent
                };

                // 創建 DataGridView
                var grid = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    AllowUserToOrderColumns = false,
                    ReadOnly = true,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                    SelectionMode = DataGridViewSelectionMode.CellSelect,
                    ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
                };

                // 設置表格樣式
                grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                grid.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                // 添加複製按鈕
                var btnCopy = new Button
                {
                    Text = "複製到剪貼簿",
                    Dock = DockStyle.Bottom,
                    Height = 30
                };
                btnCopy.Click += (s, e) => CopyDataGridViewToClipboard(grid);

                // 創建容器面板
                var panel = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RowCount = 2,
                    ColumnCount = 1
                };
                panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
                panel.Controls.Add(grid, 0, 0);
                panel.Controls.Add(btnCopy, 0, 1);

                // 初始化列
                grid.Columns.Add("Words", "詞彙");
                foreach (var word in words)
                {
                    grid.Columns.Add(word, word);
                }

                // 填充數據
                foreach (var word1 in words)
                {
                    var row = new DataGridViewRow();
                    row.CreateCells(grid);

                    // 設置行標題
                    row.Cells[0].Value = word1;

                    // 填充相似度值
                    for (int i = 0; i < words.Count; i++)
                    {
                        var word2 = words[i];
                        if (word1 == word2)
                        {
                            row.Cells[i + 1].Value = "1.0000";
                            row.Cells[i + 1].Style.BackColor = Color.LightGray;
                        }
                        else
                        {
                            double similarity = wordEmbedding.CalculateCosineSimilarity(word1, word2, is3DMode);
                            row.Cells[i + 1].Value = similarity.ToString("F4");

                            // 根據相似度設置顏色
                            row.Cells[i + 1].Style.BackColor = GetColorForSimilarity(similarity);
                        }
                    }
                    grid.Rows.Add(row);
                }

                // 如果啟用了CBOW分析，添加相似詞標籤
                if (chkCbow.Checked)
                {
                    AddSimilarWordsPanel(resultForm, words);
                }

                resultForm.Controls.Add(panel);
                resultForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"分析詞彙關係時發生錯誤：{ex.Message}", "錯誤",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Color GetColorForSimilarity(double similarity)
        {
            // 根據相似度返回不同的顏色
            if (similarity >= 0.8) return Color.FromArgb(255, 200, 200); // 深紅
            if (similarity >= 0.6) return Color.FromArgb(255, 220, 220); // 中紅
            if (similarity >= 0.4) return Color.FromArgb(255, 240, 240); // 淺紅
            if (similarity >= 0.2) return Color.FromArgb(240, 240, 255); // 淺藍
            return Color.FromArgb(220, 220, 255); // 中藍
        }

        private void CopyDataGridViewToClipboard(DataGridView grid)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                // 添加標題行
                for (int i = 0; i < grid.Columns.Count; i++)
                {
                    sb.Append(grid.Columns[i].HeaderText);
                    sb.Append(i == grid.Columns.Count - 1 ? "\n" : "\t");
                }

                // 添加數據行
                foreach (DataGridViewRow row in grid.Rows)
                {
                    for (int i = 0; i < row.Cells.Count; i++)
                    {
                        sb.Append(row.Cells[i].Value?.ToString() ?? "");
                        sb.Append(i == row.Cells.Count - 1 ? "\n" : "\t");
                    }
                }

                Clipboard.SetText(sb.ToString());
                MessageBox.Show("已複製到剪貼簿", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"複製到剪貼簿時發生錯誤：{ex.Message}", "錯誤",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddSimilarWordsPanel(Form form, List<string> words)
        {
            var similarWordsPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 200,
                BorderStyle = BorderStyle.FixedSingle
            };

            var similarWordsLabel = new Label
            {
                Text = "相似詞分析",
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(Font.FontFamily, 10, FontStyle.Bold)
            };

            var similarWordsTextBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Vertical
            };

            StringBuilder sb = new StringBuilder();
            foreach (var word in words)
            {
                sb.AppendLine($"\n{word} 的相似詞：");
                var similarWords = wordEmbedding.FindMostSimilarWords(word, 5, is3DMode);
                foreach (var (similar, score) in similarWords)
                {
                    sb.AppendLine($"  {similar}: {score:F4}");
                }
            }
            similarWordsTextBox.Text = sb.ToString();

            similarWordsPanel.Controls.Add(similarWordsTextBox);
            similarWordsPanel.Controls.Add(similarWordsLabel);
            form.Width += similarWordsPanel.Width;
            form.Controls.Add(similarWordsPanel);
        }

        private void UpdateListBoxWithFrequency(int totalTokenCount = 0)
        {
            try
            {
                if (lstTokens.InvokeRequired)
                {
                    lstTokens.Invoke(new Action(() => UpdateListBoxWithFrequency()));
                    return;
                }

                // 確保 wordEmbedding 不為 null
                if (wordEmbedding == null)
                {
                    lstTokens.DataSource = null;
                    lblWordCount.Text = "No data available";
                    Console.WriteLine("WordEmbedding is null in UpdateListBoxWithFrequency");
                    return;
                }

                List<WordFrequency> wordFrequencies;
                try
                {
                    wordFrequencies = wordEmbedding.GetWordFrequencies();
                    if (wordFrequencies == null || !wordFrequencies.Any())
                    {
                        lstTokens.DataSource = null;
                        lblWordCount.Text = "No frequency data available";
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting word frequencies: {ex.Message}");
                    MessageBox.Show($"Error getting word frequencies: {ex.Message}",
                        "Data Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                var cmbSort = Controls.Find("cmbSortType", true).FirstOrDefault() as ComboBox;
                if (cmbSort == null)
                {
                    Console.WriteLine("Sort ComboBox not found");
                    return;
                }

                try
                {
                    var sortedFrequencies = SortWordFrequencies(wordFrequencies, cmbSort.SelectedItem?.ToString() ?? "Frequency (High to Low)");

                    // 更新 ListBox
                    lstTokens.DataSource = null;
                    lstTokens.DataSource = sortedFrequencies;

                    // 獲取統計資訊
                    int processedTokens = wordEmbedding.GetProcessedTokenCount();
                    int uniqueWords = sortedFrequencies.Count;
                    double averageLength = sortedFrequencies.Average(wf => wf.Length);

                    // 更新統計顯示
                    UpdateStatisticsDisplay(totalTokenCount, processedTokens, uniqueWords, averageLength);

                    // 更新表單標題
                    UpdateFormTitle();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating ListBox: {ex.Message}");
                    MessageBox.Show($"Error updating word list: {ex.Message}",
                        "Update Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical error in UpdateListBoxWithFrequency: {ex.Message}");
                MessageBox.Show($"Critical error updating display: {ex.Message}",
                    "Critical Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void InitializeFormSettings()
        {
            this.Text = "Word2Vector Visualization";
            this.Width = 1200;
            this.Height = 800;
            this.AutoScaleMode = AutoScaleMode.Font;
            this.DoubleBuffered = true;

            // 添加視窗大小改變事件
            this.Resize += Word2VecForm_Resize;
            this.MinimumSize = new Size(800, 600); // 設置最小視窗大小
        }
        private void Word2VecForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                try
                {
                    ResizeControls();
                    if (pictureBox?.Image != null)
                    {
                        UpdateVisualization();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in resize event: {ex.Message}");
                }
            }
        }
        private void ResizeControls()
        {
            try
            {
                if (!this.IsHandleCreated || this.IsDisposed)
                    return;

                // 基本尺寸設定
                int leftPanelWidth = 300;  // ListBox 的固定寬度
                int topPanelHeight = 120;  // 上方控件區域的高度
                int padding = 10;          // 控件之間的間距

                // ListBox 調整 (需要檢查控件是否已初始化)
                if (lstTokens != null && !lstTokens.IsDisposed)
                {
                    lstTokens.Size = new Size(
                        leftPanelWidth,
                        this.ClientSize.Height - topPanelHeight - padding
                    );
                }

                // PictureBox 調整
                if (pictureBox != null && !pictureBox.IsDisposed)
                {
                    pictureBox.Location = new Point(leftPanelWidth + padding * 2, topPanelHeight);
                    pictureBox.Size = new Size(
                        this.ClientSize.Width - leftPanelWidth - padding * 3,
                        this.ClientSize.Height - topPanelHeight - padding
                    );
                }

                // 標籤調整
                if (lblWordCount != null && !lblWordCount.IsDisposed)
                {
                    lblWordCount.Location = new Point(
                        leftPanelWidth + padding * 20,
                        lblWordCount.Location.Y
                    );
                }

                // TrackBar 調整
                if (trackBarRotate != null && !trackBarRotate.IsDisposed)
                {
                    trackBarRotate.Size = new Size(
                        Math.Min((this.ClientSize.Width - leftPanelWidth - padding * 4) / 3, 200),
                        trackBarRotate.Height
                    );
                }

                // 輸入框和按鈕調整
                if (txtWordSelection != null && !txtWordSelection.IsDisposed)
                {
                    int textBoxMaxWidth = Math.Min(
                        (this.ClientSize.Width - leftPanelWidth - padding * 6) / 2,
                        400  // 最大寬度限制
                    );

                    txtWordSelection.Size = new Size(textBoxMaxWidth, txtWordSelection.Height);
                }

                if (btnSelectWords != null && !btnSelectWords.IsDisposed && txtWordSelection != null)
                {
                    btnSelectWords.Location = new Point(
                        txtWordSelection.Right + padding,
                        btnSelectWords.Location.Y
                    );
                }

                // 強制重繪
                this.Invalidate();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error resizing controls: {ex.Message}");
                // 在開發時可以加入更詳細的錯誤記錄
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ResizeControls();
        }

        private void InitializeBaseControls()
        {
            // Initialize controls
            lblStatus = new Label
            {
                Text = "Current Mode: 2D",
                Location = new Point(10, 10),
                AutoSize = true
            };

            btnToggleMode = new Button
            {
                Text = "Switch to 3D",
                Location = new Point(10, 40),
                Size = new Size(100, 30)
            };
            btnToggleMode.Click += BtnToggleMode_Click;

            btnVisualize = new Button
            {
                Text = "Visualize Words",
                Location = new Point(120, 40),
                Size = new Size(100, 30)
            };
            btnVisualize.Click += btnVisualize_Click;

            // Rotation control (initially hidden)
            trackBarRotate = new TrackBar
            {
                Location = new Point(320, 40),
                Size = new Size(200, 45),
                Minimum = 0,
                Maximum = 360,
                Value = 0,
                TickFrequency = 45,
                Visible = false
            };
            trackBarRotate.ValueChanged += (s, e) =>
            {
                rotationAngle = trackBarRotate.Value;
                UpdateVisualization();
            };

            lstTokens = new ListBox
            {
                Location = new Point(10, 90),
                Size = new Size(300, 650),
                SelectionMode = SelectionMode.MultiExtended,
            };

            pictureBox = new PictureBox
            {
                Location = new Point(320, 120),
                SizeMode = PictureBoxSizeMode.Normal,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            // 添加滑鼠事件
            pictureBox.MouseDown += PictureBox_MouseDown;
            pictureBox.MouseMove += PictureBox_MouseMove;
            pictureBox.MouseUp += PictureBox_MouseUp;
            pictureBox.MouseWheel += PictureBox_MouseWheel;
            pictureBox.Cursor = Cursors.SizeAll;
            this.MouseWheel += new MouseEventHandler(PictureBox_MouseWheel);

            // 添加停用字相關控件
            btnLoadStopWords = new Button
            {
                Text = "Load Stop Words",
                Location = new Point(530, 40),
                Size = new Size(120, 30)
            };
            btnLoadStopWords.Click += BtnLoadStopWords_Click;

            btnShowStopWords = new Button
            {
                Text = "Show Stop Words",
                Location = new Point(660, 40),
                Size = new Size(120, 30)
            };
            btnShowStopWords.Click += BtnShowStopWords_Click;

            lblWordCount = new Label
            {
                Location = new Point(790, 45),
                AutoSize = true
            };

            this.Controls.AddRange(new Control[] {
        lblStatus,
        btnToggleMode,
        btnVisualize,
        btnLoadStopWords,
                btnShowStopWords,
        trackBarRotate,
        lstTokens,
        lblWordCount,
        pictureBox
    });
            ResizeControls();
        }

        private void InitializeFilterControls()
        {
            // 過濾標籤
            lblFilter = new Label
            {
                Text = "Filter:",
                Location = new Point(10, 90),
                AutoSize = true
            };

            // 過濾類型下拉框
            cmbFilterType = new ComboBox
            {
                Location = new Point(60, 87),
                Size = new Size(100, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFilterType.Items.AddRange(new string[] { "Contains", "Starts With", "Ends With", "Exact Match" });
            cmbFilterType.SelectedIndex = 0;
            cmbFilterType.SelectedIndexChanged += FilterWords;

            var cmbSortType = new ComboBox
            {
                Name = "cmbSortType",
                Location = new Point(170, 87),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbSortType.Items.AddRange(new string[] {
            "Frequency (High to Low)",
            "Frequency (Low to High)",
            "Alphabetical A-Z",
            "Alphabetical Z-A",
            "Length (Long to Short)",
            "Length (Short to Long)"
        });
            cmbSortType.SelectedIndex = 0;  // 預設選擇頻率由高到低
            cmbSortType.SelectedIndexChanged += (s, e) => UpdateListBoxWithFrequency();

            // 過濾文字框
            txtFilter = new TextBox
            {
                Location = new Point(330, 87),
                Size = new Size(120, 25)
            };
            txtFilter.Text = "Enter text...";
            txtFilter.ForeColor = Color.Gray;
            txtFilter.GotFocus += (s, e) =>
            {
                if (txtFilter.Text == "Enter text...")
                {
                    txtFilter.Text = "";
                    txtFilter.ForeColor = Color.Black;
                }
            };
            txtFilter.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtFilter.Text))
                {
                    txtFilter.Text = "Enter text...";
                    txtFilter.ForeColor = Color.Gray;
                }
            };
            txtFilter.TextChanged += (s, e) =>
            {
                if (txtFilter.Text != "Enter text...")
                {
                    FilterWords();
                }
            };

            // 區分大小寫選項
            chkCaseSensitive = new CheckBox
            {
                Text = "Case Sensitive",
                Location = new Point(460, 90),
                AutoSize = true
            };
            chkCaseSensitive.CheckedChanged += FilterWords;





            // 調整 ListBox 位置
            lstTokens.Location = new Point(10, 120);
            lstTokens.Size = new Size(300, 620);

            // 添加控件到表單
            this.Controls.AddRange(new Control[] {
        lblFilter,
        cmbFilterType,
        cmbSortType,
        txtFilter,
        chkCaseSensitive,
        chkCbow
    });

            // 更新標籤位置和大小以容納更多資訊
            lblWordCount.Location = new Point(320, 90);
            lblWordCount.AutoSize = true;
            lblWordCount.Font = new Font(lblWordCount.Font.FontFamily, 9);

            // 初始化時統計並顯示頻率
            UpdateListBoxWithFrequency(totalTokenCount);
        }

        private Dictionary<string, int> CalculateWordFrequencies(List<string> words)
        {
            return words
                .GroupBy(w => w, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    g => g.Key,
                    g => g.Count(),
                    StringComparer.OrdinalIgnoreCase
                );
        }
        private void UpdateStatisticsDisplay(int totalTokens, int processedTokens, int uniqueWords, double averageLength)
        {
            if (lblWordCount.InvokeRequired)
            {
                lblWordCount.Invoke(new Action(() =>
                    UpdateStatisticsDisplay(totalTokens, processedTokens, uniqueWords, averageLength)));
                return;
            }

            try
            {
                int stopWordsCount = StopWords != null ? StopWords.Count : 0;
                lblWordCount.Text = string.Format(
                    "Total Tokens: {0:N0} | " +
                    "Processed: {1:N0} | " +
                    "Unique: {2:N0} | " +
                    "Avg Length: {3:F1} | " +
                    "Stop Words: {4:N0}",
                    totalTokens,
                    processedTokens,
                    uniqueWords,
                    averageLength,
                    stopWordsCount
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating statistics display: {ex.Message}");
                lblWordCount.Text = "Error displaying statistics";
            }
        }
        private List<WordFrequency> SortWordFrequencies(List<WordFrequency> frequencies, string sortType)
        {
            if (frequencies == null)
            {
                return new List<WordFrequency>();
            }

            try
            {
                switch (sortType)
                {
                    case "Alphabetical A-Z":
                        return frequencies.OrderBy(wf => wf.Word).ToList();

                    case "Alphabetical Z-A":
                        return frequencies.OrderByDescending(wf => wf.Word).ToList();

                    case "Frequency (High to Low)":
                        return frequencies
                            .OrderByDescending(wf => wf.Frequency)
                            .ThenBy(wf => wf.Word)
                            .ToList();

                    case "Frequency (Low to High)":
                        return frequencies
                            .OrderBy(wf => wf.Frequency)
                            .ThenBy(wf => wf.Word)
                            .ToList();

                    case "Length (Long to Short)":
                        return frequencies
                            .OrderByDescending(wf => wf.Length)
                            .ThenBy(wf => wf.Word)
                            .ToList();

                    case "Length (Short to Long)":
                        return frequencies
                            .OrderBy(wf => wf.Length)
                            .ThenBy(wf => wf.Word)
                            .ToList();

                    default:
                        return frequencies
                            .OrderByDescending(wf => wf.Frequency)
                            .ThenBy(wf => wf.Word)
                            .ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sorting frequencies: {ex.Message}");
                return frequencies.ToList(); // 返回未排序的列表
            }
        }

        private void UpdateListBoxData(bool initialize = false)
        {
            var sortedWords = originalAvailableWords.OrderBy(w => w).ToList();
            lstTokens.DataSource = null;
            lstTokens.DataSource = sortedWords;

            if (initialize)
            {
                UpdateWordCount(sortedWords.Count);
            }
        }
        private void FilterWords(object sender = null, EventArgs e = null)
        {
            try
            {
                if (!IsValidFilterText())
                {
                    ResetToDefaultView();
                    return;
                }

                var filterText = txtFilter.Text;
                var comparison = chkCaseSensitive.Checked ?
                    StringComparison.Ordinal :
                    StringComparison.OrdinalIgnoreCase;

                var filteredWords = FilterWordsByCriteria(filterText, comparison).ToList();

                // 使用新的頻率更新方法
                var frequencies = CalculateWordFrequencies(filteredWords);
                var wordFrequencies = frequencies.Select(kvp => new WordFrequency
                {
                    Word = kvp.Key,
                    Frequency = kvp.Value,
                    Length = kvp.Key.Length
                }).ToList();

                lstTokens.DataSource = null;
                lstTokens.DataSource = wordFrequencies;
                UpdateWordCount(wordFrequencies.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering words: {ex.Message}", "Filter Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsValidFilterText()
        {
            return txtFilter.Text != "Enter text..." && !string.IsNullOrEmpty(txtFilter.Text);
        }


        private IEnumerable<string> FilterWordsByCriteria(string filterText, StringComparison comparison)
        {
            return originalAvailableWords.Where(word =>
            {
                switch (cmbFilterType.SelectedItem.ToString())
                {
                    case "Contains":
                        return word.IndexOf(filterText, comparison) >= 0;
                    case "Starts With":
                        return word.StartsWith(filterText, comparison);
                    case "Ends With":
                        return word.EndsWith(filterText, comparison);
                    case "Exact Match":
                        return word.Equals(filterText, comparison);
                    default:
                        return true;
                }
            });
        }

        private void ResetToDefaultView()
        {
            var sortButton = this.Controls.OfType<Button>().FirstOrDefault(b => b.Text.StartsWith("Sort"));
            var sortedWords = originalAvailableWords;

            if (sortButton != null && sortButton.Text == "Sort Z-A")
            {
                sortedWords = originalAvailableWords.OrderByDescending(w => w).ToList();
            }
            else
            {
                sortedWords = originalAvailableWords.OrderBy(w => w).ToList();
            }

            lstTokens.DataSource = null;
            lstTokens.DataSource = sortedWords;
            UpdateWordCount(sortedWords.Count);
        }

        private void HandleVisualizationError(Exception ex)
        {
            MessageBox.Show($"Error during visualization: {ex.Message}",
                "Visualization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private StringComparison GetComparisonType()
        {
            return chkCaseSensitive.Checked ?
                StringComparison.Ordinal :
                StringComparison.OrdinalIgnoreCase;
        }

        // 修改清除過濾器方法
        private void ClearFilter()
        {
            txtFilter.Text = "Enter text...";
            txtFilter.ForeColor = Color.Gray;
            cmbFilterType.SelectedIndex = 0;
            chkCaseSensitive.Checked = false;

            // 維持當前的排序順序
            var sortButton = this.Controls.OfType<Button>().FirstOrDefault(b => b.Text.StartsWith("Sort"));
            var sortedWords = originalAvailableWords;
            if (sortButton != null && sortButton.Text == "Sort Z-A")
            {
                sortedWords = originalAvailableWords.OrderByDescending(w => w).ToList();
            }
            else
            {
                sortedWords = originalAvailableWords.OrderBy(w => w).ToList();
            }

            lstTokens.DataSource = sortedWords;
            UpdateWordCount(sortedWords.Count);
        }

        private void UpdateWordCount(int filteredCount)
        {
            try
            {
                if (lblWordCount.InvokeRequired)
                {
                    lblWordCount.Invoke(new Action(() => UpdateWordCount(filteredCount)));
                    return;
                }

                // 計算各種數量
                int totalWords = originalAvailableWords?.Count ?? 0;
                int currentWords = filteredCount;
                int filteredOutWords = Math.Max(0, totalWords - currentWords);
                int stopWordsCount = StopWords?.Count ?? 0;

                // 建立顯示文字
                string displayText = string.Format(
                    "Words: {0} (Total: {1}, Filtered: {2}, Stop Words: {3})",
                    currentWords,
                    totalWords,
                    filteredOutWords,
                    stopWordsCount
                );

                // 更新標籤文字
                if (!lblWordCount.IsDisposed)
                {
                    lblWordCount.Text = displayText;
                }
            }
            catch (Exception ex)
            {
                // 錯誤處理（可以選擇記錄到日誌）
                Console.WriteLine($"Error updating word count: {ex.Message}");
            }
        }

        private List<string> FilterStopWords(List<string> words)
        {
            return words.Where(w => !string.IsNullOrWhiteSpace(w) &&
                                   !StopWords.Contains(w.ToLower()) &&
                                   w.Length > 1)  // 過濾單個字符
                        .ToList();
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && is3DMode)
            {
                isDragging = true;
                lastMousePos = e.Location;
                pictureBox.Capture = true;
            }
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && is3DMode)
            {
                int deltaX = e.X - lastMousePos.X;

                rotationAngle = (rotationAngle + deltaX) % 360;
                if (rotationAngle < 0) rotationAngle += 360;

                trackBarRotate.Value = (int)rotationAngle;

                UpdateVisualization();

                lastMousePos = e.Location;
            }
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
            pictureBox.Capture = false;
        }

        private void BtnLoadStopWords_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var newStopWords = File.ReadAllLines(openFileDialog.FileName)
                                             .Where(line => !string.IsNullOrWhiteSpace(line))
                                             .Select(line => line.Trim().ToLower());

                        foreach (var word in newStopWords)
                        {
                            StopWords.Add(word);
                        }

                        // 重新過濾詞列表
                        var originalWords = ((List<string>)lstTokens.DataSource);
                        var filteredWords = FilterStopWords(originalWords);
                        lstTokens.DataSource = filteredWords;
                        lblWordCount.Text = $"Words: {filteredWords.Count} (Original: {originalWords.Count})";

                        MessageBox.Show($"Loaded {newStopWords.Count()} stop words.",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading stop words: {ex.Message}",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnShowStopWords_Click(object sender, EventArgs e)
        {
            var stopWordsForm = new Form()
            {
                Text = "Stop Words",
                Width = 400,
                Height = 600,
                StartPosition = FormStartPosition.CenterParent
            };

            var textBox = new TextBox()
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Text = string.Join(Environment.NewLine, StopWords.OrderBy(w => w))
            };

            stopWordsForm.Controls.Add(textBox);
            stopWordsForm.ShowDialog();
        }

        private void ExportStopWords()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.DefaultExt = "txt";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllLines(saveFileDialog.FileName,
                            StopWords.OrderBy(w => w));
                        MessageBox.Show("Stop words exported successfully.",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error exporting stop words: {ex.Message}",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // 添加右鍵選單
        private void AddContextMenu()
        {
            var contextMenu = new ContextMenuStrip();

            // 添加複製功能
            var copyMenuItem = new ToolStripMenuItem("Copy Selected Words", null, (s, e) => CopySelectedWords());
            copyMenuItem.ShortcutKeys = Keys.Control | Keys.C;
            contextMenu.Items.Add(copyMenuItem);

            // 添加分隔線
            contextMenu.Items.Add(new ToolStripSeparator());

            // 原有的選單項目
            contextMenu.Items.Add("Add to Stop Words", null, (s, e) =>
            {
                var selectedItems = lstTokens.SelectedItems.Cast<object>();
                foreach (var item in selectedItems)
                {
                    string word;
                    if (item is WordFrequency wordFreq)
                    {
                        word = wordFreq.Word;
                    }
                    else
                    {
                        word = item.ToString();
                    }
                    StopWords.Add(word.ToLower());
                }

                originalAvailableWords = originalAvailableWords
                    .Where(w => !StopWords.Contains(w.ToLower()))
                    .ToList();

                FilterWords();
            });

            contextMenu.Items.Add("Clear Filter", null, (s, e) => ClearFilter());
            contextMenu.Items.Add("Export Stop Words", null, (s, e) => ExportStopWords());

            lstTokens.ContextMenuStrip = contextMenu;

            // 添加KeyDown事件處理
            lstTokens.KeyDown += ListBox_KeyDown;
        }
        private void ListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                CopySelectedWords();
                e.Handled = true;
            }
        }

        private void CopySelectedWords()
        {
            try
            {
                if (lstTokens.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Please select at least one word to copy.", "No Selection",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 收集選中的文字
                var selectedWords = lstTokens.SelectedItems.Cast<object>()
                    .Select(item =>
                    {
                        if (item is WordFrequency wordFreq)
                        {
                            return wordFreq.Word;
                        }
                        return item.ToString();
                    })
                    .ToList();

                // 使用逗號連接文字
                string copiedText = string.Join(", ", selectedWords);

                // 複製到剪貼簿
                Clipboard.SetText(copiedText);

                // 可選：顯示提示訊息
                lblStatus.Text = $"Copied {selectedWords.Count} word{(selectedWords.Count > 1 ? "s" : "")} to clipboard";

                // 啟動一個計時器來清除狀態訊息
                var timer = new Timer();
                timer.Interval = 3000; // 3秒後清除訊息
                timer.Tick += (s, e) =>
                {
                    lblStatus.Text = $"Current Mode: {(is3DMode ? "3D" : "2D")}";
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error copying words: {ex.Message}", "Copy Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnToggleMode_Click(object sender, EventArgs e)
        {
            is3DMode = !is3DMode;
            btnToggleMode.Text = is3DMode ? "Switch to 2D" : "Switch to 3D";
            lblStatus.Text = $"Current Mode: {(is3DMode ? "3D" : "2D")}";
            trackBarRotate.Visible = is3DMode;
            UpdateVisualization();
        }

        private void btnVisualize_Click(object sender, EventArgs e)
        {
            UpdateVisualization();

            // 分析選中詞的關係
            var selectedWords = GetSelectedWords();
            if (selectedWords.Count > 1)
            {
                AnalyzeWordRelationships(selectedWords);
            }
        }

        private void BtnSelectWords_Click(object sender, EventArgs e)
        {
            try
            {
                // 清除當前選擇
                lstTokens.ClearSelected();

                // 獲取並處理輸入的文字
                var inputWords = txtWordSelection.Text
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(w => w.Trim())
                    .Where(w => !string.IsNullOrEmpty(w))
                    .ToList();

                if (!inputWords.Any())
                {
                    MessageBox.Show("Please enter at least one word.", "Input Required",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 尋找並選擇匹配的項目
                for (int i = 0; i < lstTokens.Items.Count; i++)
                {
                    var item = lstTokens.Items[i];
                    string word;
                    if (item is WordFrequency wordFreq)
                    {
                        word = wordFreq.Word;
                    }
                    else
                    {
                        word = item.ToString();
                    }

                    if (inputWords.Any(w => w.Equals(word, StringComparison.OrdinalIgnoreCase)))
                    {
                        lstTokens.SetSelected(i, true);
                    }
                }

                // 檢查是否有找到任何匹配
                if (lstTokens.SelectedIndices.Count == 0)
                {
                    MessageBox.Show("No matching words found in the list.", "No Matches",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    UpdateVisualization();

                    // 分析選中詞的關係
                    var selectedWords = GetSelectedWords();
                    if (selectedWords.Count > 1)
                    {
                        AnalyzeWordRelationships(selectedWords);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting words: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void PictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            // 同時支援2D和3D模式的縮放
            float zoomStep = e.Delta > 0 ? 1.1f : 0.9f;
            zoomFactor *= zoomStep;

            // 限制縮放範圍
            zoomFactor = Math.Max(0.1f, Math.Min(5.0f, zoomFactor));

            UpdateVisualization();
        }

        private bool ValidateSelection()
        {
            if (lstTokens.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one word.");
                return false;
            }
            return true;
        }

        private List<(MathVector vector, string word)> GetSelectedVectors()
        {
            var vectors = new List<(MathVector vector, string word)>();
            foreach (var item in lstTokens.SelectedItems)
            {
                try
                {
                    string word;
                    if (item is WordFrequency wordFreq)
                    {
                        word = wordFreq.Word;
                    }
                    else
                    {
                        word = item.ToString();
                    }
                    vectors.Add((wordEmbedding.GetWordVector(word, is3DMode), word));
                }
                catch
                {
                    continue;
                }
            }
            return vectors;
        }

        private void RenderVectors(List<(MathVector vector, string word)> vectors)
        {
            if (is3DMode)
                Draw3DVectors(vectors);
            else
                Draw2DVectors(vectors);
        }
        private void Draw2DVectors(List<(MathVector vector, string word)> vectorsWithWords)
        {
            try
            {
                using (var bitmap = new Bitmap(pictureBox.Width, pictureBox.Height))
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.Clear(Color.White);

                    const int padding = 40;
                    var plotWidth = bitmap.Width - 2 * padding;
                    var plotHeight = bitmap.Height - 2 * padding;

                    var vectors = vectorsWithWords.Select(x => x.vector).ToList();

                    // 計算範圍
                    double minX = vectors.Min(v => v[0]);
                    double maxX = vectors.Max(v => v[0]);
                    double minY = vectors.Min(v => v[1]);
                    double maxY = vectors.Max(v => v[1]);

                    // 添加邊距
                    double rangeX = (maxX - minX) * 1.1;
                    double rangeY = (maxY - minY) * 1.1;
                    minX -= rangeX * 0.05;
                    maxX += rangeX * 0.05;
                    minY -= rangeY * 0.05;
                    maxY += rangeY * 0.05;

                    // 繪製座標軸
                    DrawAxes(graphics, padding, bitmap.Width, bitmap.Height);

                    // 使用Dictionary來存儲詞的顏色
                    var wordColors = GetWordColors(vectorsWithWords.Select(v => v.word).ToList());

                    // 繪製每個詞向量
                    foreach (var (vector, word) in vectorsWithWords)
                    {
                        // 計算歸一化座標
                        double normalizedX = SafeNormalize(vector[0], minX, maxX);
                        double normalizedY = SafeNormalize(vector[1], minY, maxY);

                        // 轉換到屏幕座標
                        float screenX = (float)(normalizedX * plotWidth + padding);
                        float screenY = (float)((1 - normalizedY) * plotHeight + padding);

                        // 獲取詞的顏色
                        Color wordColor = wordColors[word];

                        // 繪製向量點和標籤
                        Draw2DWordVector(graphics, word, screenX, screenY, wordColor);
                    }

                    pictureBox.Image?.Dispose();
                    pictureBox.Image = (Bitmap)bitmap.Clone();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error drawing 2D vectors: {ex.Message}");
            }
        }
        private void Draw3DVectors(List<(MathVector vector, string word)> vectorsWithWords)
        {
            try
            {
                using (var bitmap = new Bitmap(pictureBox.Width, pictureBox.Height))
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.Clear(Color.White);

                    var centerX = bitmap.Width / 2;
                    var centerY = bitmap.Height / 2;
                    var scale = (Math.Min(bitmap.Width, bitmap.Height) / 4.0f) * zoomFactor;

                    // 繪製3D網格
                    DrawGrid(graphics, centerX, centerY, scale, 5);

                    // 獲取詞的顏色映射
                    var wordColors = GetWordColors(vectorsWithWords.Select(v => v.word).ToList());

                    // 計算並排序3D點
                    var points3D = vectorsWithWords
                        .Select(vw =>
                        {
                            double[] normalized = NormalizeVector(vw.vector);
                            return (
                                x: normalized[0] * scale,
                                y: normalized[1] * scale,
                                z: normalized[2] * scale,
                                word: vw.word
                            );
                        })
                        .OrderByDescending(p =>
                            -p.x * Math.Sin(rotationAngle * Math.PI / 180) +
                            p.z * Math.Cos(rotationAngle * Math.PI / 180))
                        .ToList();

                    // 繪製每個點
                    foreach (var point in points3D)
                    {
                        var screenPoint = Rotate3DPoint(point.x, point.y, point.z, rotationAngle);
                        float screenX = centerX + screenPoint.X;
                        float screenY = centerY - screenPoint.Y;

                        // 獲取詞的顏色並調整深度
                        Color baseColor = wordColors[point.word];
                        Color depthAdjustedColor = AdjustColorByDepth(baseColor, point.z);

                        // 繪製3D向量點和標籤
                        Draw3DWordVector(graphics, point.word, screenX, screenY, depthAdjustedColor, point.z);
                    }

                    // 添加控制提示
                    DrawControlHints(graphics, bitmap.Width, bitmap.Height);

                    pictureBox.Image?.Dispose();
                    pictureBox.Image = (Bitmap)bitmap.Clone();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error drawing 3D vectors: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Draw2DWordVector(Graphics g, string word, float x, float y, Color color)
        {
            const int pointSize = 6;
            using (var brush = new SolidBrush(color))
            using (var font = new Font("Arial", 8))
            {
                // 繪製點
                g.FillEllipse(brush, x - pointSize / 2, y - pointSize / 2, pointSize, pointSize);

                // 使用文字路徑來確保更好的可讀性
                using (var path = new GraphicsPath())
                {
                    path.AddString(word,
                        font.FontFamily,
                        (int)font.Style,
                        font.Size,
                        new PointF(x + pointSize, y - pointSize),
                        StringFormat.GenericDefault);

                    // 繪製白色背景確保文字可見
                    using (var pen = new Pen(Color.White, 3))
                    {
                        g.DrawPath(pen, path);
                    }
                    g.FillPath(brush, path);
                }
            }
        }

        private void Draw3DWordVector(Graphics g, string word, float x, float y, Color color, double depth)
        {
            const int pointSize = 6;
            float depthScale = (float)(1.0 - depth * 0.0005); // 根據深度調整大小

            using (var brush = new SolidBrush(color))
            using (var font = new Font("Arial", 8 * depthScale))
            {
                // 繪製點
                float adjustedSize = pointSize * depthScale;
                g.FillEllipse(brush,
                    x - adjustedSize / 2,
                    y - adjustedSize / 2,
                    adjustedSize,
                    adjustedSize);

                // 使用文字路徑
                using (var path = new GraphicsPath())
                {
                    path.AddString(word,
                        font.FontFamily,
                        (int)font.Style,
                        font.Size,
                        new PointF(x + adjustedSize, y - adjustedSize),
                        StringFormat.GenericDefault);

                    // 繪製白色背景
                    using (var pen = new Pen(Color.White, 3))
                    {
                        g.DrawPath(pen, path);
                    }
                    g.FillPath(brush, path);
                }
            }
        }
        private Dictionary<string, Color> GetWordColors(List<string> words)
        {
            var colors = new Dictionary<string, Color>();
            var selectedWords = GetSelectedWords();

            foreach (var word in words)
            {
                if (selectedWords.Contains(word))
                {
                    colors[word] = Color.Red; // 選中的詞
                }
                else
                {
                    // 檢查是否是相似詞
                    var mainWord = selectedWords.FirstOrDefault();
                    if (!string.IsNullOrEmpty(mainWord))
                    {
                        var similarWords = wordEmbedding.FindMostSimilarWords(mainWord, 5, is3DMode)
                            .Select(x => x.Word)
                            .ToList();

                        colors[word] = similarWords.Contains(word) ? Color.Blue : Color.Black;
                    }
                    else
                    {
                        colors[word] = Color.Black;
                    }
                }
            }

            return colors;
        }

        private double[] NormalizeVector(MathVector vector)
        {
            double[] result = new double[vector.Count];
            for (int i = 0; i < vector.Count; i++)
            {
                result[i] = vector[i] / 10.0; // 調整比例
            }
            return result;
        }

        private void DrawControlHints(Graphics g, int width, int height)
        {
            using (var font = new Font("Arial", 8))
            {
                string[] hints = {
                $"Rotation: {rotationAngle:F0}°",
                "Left click and drag to rotate",
                $"Zoom: {zoomFactor:F2}x"
            };

                float y = height - 60;
                foreach (var hint in hints)
                {
                    g.DrawString(hint, font, Brushes.Gray, 10, y);
                    y += 20;
                }
            }
        }
        private Color AdjustColorByDepth(Color baseColor, double z)
        {
            try
            {
                // 將z值標準化到0-1範圍
                // z值越大（越遠）顏色越淡
                double depth = (z + 5.0) / 10.0; // 假設z的範圍是-5到5
                depth = Math.Max(0.3, Math.Min(1.0, depth)); // 限制最小亮度為0.3

                // 調整RGB分量
                int r = (int)(baseColor.R * depth);
                int g = (int)(baseColor.G * depth);
                int b = (int)(baseColor.B * depth);

                return Color.FromArgb(baseColor.A, r, g, b);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AdjustColorByDepth: {ex.Message}");
                return baseColor; // 發生錯誤時返回原始顏色
            }
        }

        private void DrawAxes(Graphics g, int padding, int width, int height)
        {
            try
            {
                // 定義座標軸的樣式
                using (var axisPen = new Pen(Color.Black, 1))
                using (var gridPen = new Pen(Color.LightGray, 1) { DashStyle = DashStyle.Dot })
                using (var font = new Font("Arial", 8))
                {
                    // 計算座標系的範圍
                    int plotWidth = width - 2 * padding;
                    int plotHeight = height - 2 * padding;

                    // 原點位置
                    float originX = padding;
                    float originY = height - padding;

                    // 繪製X軸
                    g.DrawLine(axisPen,
                        originX, originY,
                        width - padding, originY);

                    // 繪製Y軸
                    g.DrawLine(axisPen,
                        originX, height - padding,
                        originX, padding);

                    // 繪製網格線和刻度
                    int gridCount = 10; // 網格線數量
                    float xStep = plotWidth / (float)gridCount;
                    float yStep = plotHeight / (float)gridCount;

                    // 繪製垂直網格線和X軸刻度
                    for (int i = 0; i <= gridCount; i++)
                    {
                        float x = originX + i * xStep;

                        // 網格線
                        g.DrawLine(gridPen,
                            x, padding,
                            x, height - padding);

                        // 刻度值
                        float value = (i - gridCount / 2) / (float)(gridCount / 4);
                        string label = value.ToString("F1");
                        var size = g.MeasureString(label, font);

                        g.DrawString(label, font, Brushes.Black,
                            x - size.Width / 2,
                            originY + 5);
                    }

                    // 繪製水平網格線和Y軸刻度
                    for (int i = 0; i <= gridCount; i++)
                    {
                        float y = padding + i * yStep;

                        // 網格線
                        g.DrawLine(gridPen,
                            padding, y,
                            width - padding, y);

                        // 刻度值
                        float value = ((gridCount - i) - gridCount / 2) / (float)(gridCount / 4);
                        string label = value.ToString("F1");
                        var size = g.MeasureString(label, font);

                        g.DrawString(label, font, Brushes.Black,
                            originX - size.Width - 5,
                            y - size.Height / 2);
                    }

                    // 添加軸標籤
                    using (var boldFont = new Font("Arial", 10, FontStyle.Bold))
                    {
                        // X軸標籤
                        g.DrawString("X", boldFont, Brushes.Black,
                            width - padding - 20,
                            originY + 20);

                        // Y軸標籤
                        g.DrawString("Y", boldFont, Brushes.Black,
                            originX - 20,
                            padding - 20);
                    }

                    // 原點標籤
                    g.DrawString("O", font, Brushes.Black,
                        originX - 15,
                        originY + 5);

                    // 可選：添加刻度單位或其他註釋
                    using (var smallFont = new Font("Arial", 7))
                    {
                        g.DrawString("×1.0", smallFont, Brushes.Gray,
                            width - padding - 5,
                            originY + 5);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error drawing axes: {ex.Message}");
            }
        }

        // 添加一個輔助方法來繪製3D座標軸
        private void Draw3DAxes(Graphics g, float centerX, float centerY, float scale)
        {
            try
            {
                using (var xPen = new Pen(Color.Red, 2))
                using (var yPen = new Pen(Color.Green, 2))
                using (var zPen = new Pen(Color.Blue, 2))
                using (var font = new Font("Arial", 10, FontStyle.Bold))
                {
                    // X軸
                    var xEnd = Rotate3DPoint(scale, 0, 0, rotationAngle);
                    g.DrawLine(xPen,
                        centerX, centerY,
                        centerX + xEnd.X, centerY - xEnd.Y);
                    g.DrawString("X", font, Brushes.Red,
                        centerX + xEnd.X + 5,
                        centerY - xEnd.Y - 5);

                    // Y軸
                    var yEnd = Rotate3DPoint(0, scale, 0, rotationAngle);
                    g.DrawLine(yPen,
                        centerX, centerY,
                        centerX + yEnd.X, centerY - yEnd.Y);
                    g.DrawString("Y", font, Brushes.Green,
                        centerX + yEnd.X + 5,
                        centerY - yEnd.Y - 5);

                    // Z軸
                    var zEnd = Rotate3DPoint(0, 0, scale, rotationAngle);
                    g.DrawLine(zPen,
                        centerX, centerY,
                        centerX + zEnd.X, centerY - zEnd.Y);
                    g.DrawString("Z", font, Brushes.Blue,
                        centerX + zEnd.X + 5,
                        centerY - zEnd.Y - 5);

                    // 添加刻度
                    DrawAxisTicks(g, centerX, centerY, scale);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error drawing 3D axes: {ex.Message}");
            }
        }

        private void DrawAxisTicks(Graphics g, float centerX, float centerY, float scale)
        {
            try
            {
                using (var font = new Font("Arial", 8))
                {
                    // 每個軸繪製5個刻度
                    for (int i = 1; i <= 5; i++)
                    {
                        float tickLength = scale / 5 * i;
                        float tickValue = i / 5.0f;

                        // X軸刻度
                        var xTick = Rotate3DPoint(tickLength, 0, 0, rotationAngle);
                        g.DrawString(tickValue.ToString("F1"), font, Brushes.Red,
                            centerX + xTick.X + 2,
                            centerY - xTick.Y + 2);

                        // Y軸刻度
                        var yTick = Rotate3DPoint(0, tickLength, 0, rotationAngle);
                        g.DrawString(tickValue.ToString("F1"), font, Brushes.Green,
                            centerX + yTick.X + 2,
                            centerY - yTick.Y + 2);

                        // Z軸刻度
                        var zTick = Rotate3DPoint(0, 0, tickLength, rotationAngle);
                        g.DrawString(tickValue.ToString("F1"), font, Brushes.Blue,
                            centerX + zTick.X + 2,
                            centerY - zTick.Y + 2);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error drawing axis ticks: {ex.Message}");
            }
        }

        // 安全的歸一化方法
        private double SafeNormalize(double value, double min, double max)
        {
            if (Math.Abs(max - min) < double.Epsilon)
                return 0.5;
            return Math.Max(0, Math.Min(1, (value - min) / (max - min)));
        }
        private void DrawGrid(Graphics g, float centerX, float centerY, float scale, int divisions)
        {
            try
            {
                // Grid colors
                var xGridPen = new Pen(Color.FromArgb(50, Color.Red), 1);
                var yGridPen = new Pen(Color.FromArgb(50, Color.Green), 1);
                var zGridPen = new Pen(Color.FromArgb(50, Color.Blue), 1);
                var xAxisPen = new Pen(Color.Red, 2);
                var yAxisPen = new Pen(Color.Green, 2);
                var zAxisPen = new Pen(Color.Blue, 2);

                float step = scale / divisions;

                // Draw grid lines
                for (int i = -divisions; i <= divisions; i++)
                {
                    float pos = i * step;

                    // X-Y plane
                    DrawRotated3DLine(g, centerX, centerY,
                        pos, -scale / 2, 0,  // From bottom
                        pos, scale / 2, 0,   // To top
                        xGridPen, rotationAngle);

                    DrawRotated3DLine(g, centerX, centerY,
                        -scale / 2, pos, 0,  // From left
                        scale / 2, pos, 0,   // To right
                        yGridPen, rotationAngle);

                    // X-Z plane
                    DrawRotated3DLine(g, centerX, centerY,
                        pos, 0, -scale / 2,
                        pos, 0, scale / 2,
                        xGridPen, rotationAngle);

                    DrawRotated3DLine(g, centerX, centerY,
                        -scale / 2, 0, pos,
                        scale / 2, 0, pos,
                        zGridPen, rotationAngle);

                    // Y-Z plane
                    DrawRotated3DLine(g, centerX, centerY,
                        0, pos, -scale / 2,
                        0, pos, scale / 2,
                        yGridPen, rotationAngle);

                    DrawRotated3DLine(g, centerX, centerY,
                        0, -scale / 2, pos,
                        0, scale / 2, pos,
                        zGridPen, rotationAngle);
                }

                // Draw main axes
                DrawRotated3DLine(g, centerX, centerY,
                    -scale / 2, 0, 0,
                    scale / 2, 0, 0,
                    xAxisPen, rotationAngle); // X axis

                DrawRotated3DLine(g, centerX, centerY,
                    0, -scale / 2, 0,
                    0, scale / 2, 0,
                    yAxisPen, rotationAngle); // Y axis

                DrawRotated3DLine(g, centerX, centerY,
                    0, 0, -scale / 2,
                    0, 0, scale / 2,
                    zAxisPen, rotationAngle); // Z axis

                // Draw axis labels
                using (var font = new Font("Arial", 8, FontStyle.Bold))
                {
                    var xEnd = Rotate3DPoint(scale / 2 + 20, 0, 0, rotationAngle);
                    g.DrawString("X", font, Brushes.Red,
                        centerX + xEnd.X, centerY - xEnd.Y);

                    var yEnd = Rotate3DPoint(0, scale / 2 + 20, 0, rotationAngle);
                    g.DrawString("Y", font, Brushes.Green,
                        centerX + yEnd.X, centerY - yEnd.Y);

                    var zEnd = Rotate3DPoint(0, 0, scale / 2 + 20, rotationAngle);
                    g.DrawString("Z", font, Brushes.Blue,
                        centerX + zEnd.X, centerY - zEnd.Y);
                }

                // Draw scale markers
                if (divisions <= 10) // 只在分割數不太大時顯示刻度
                {
                    using (var font = new Font("Arial", 6))
                    {
                        for (int i = -divisions; i <= divisions; i++)
                        {
                            if (i == 0) continue; // Skip origin

                            float pos = i * step;
                            var value = (i / (float)divisions).ToString("F1");

                            // X axis markers
                            var xPoint = Rotate3DPoint(pos, 0, 0, rotationAngle);
                            g.DrawString(value, font, Brushes.Red,
                                centerX + xPoint.X - 10, centerY + 5);

                            // Y axis markers
                            var yPoint = Rotate3DPoint(0, pos, 0, rotationAngle);
                            g.DrawString(value, font, Brushes.Green,
                                centerX - 20, centerY - yPoint.Y);

                            // Z axis markers
                            var zPoint = Rotate3DPoint(0, 0, pos, rotationAngle);
                            g.DrawString(value, font, Brushes.Blue,
                                centerX + zPoint.X + 5, centerY - zPoint.Y);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 錯誤處理但繼續執行，避免影響整個視圖
                Console.WriteLine($"Error in DrawGrid: {ex.Message}");
            }
        }

        private void DrawRotated3DLine(Graphics g, float centerX, float centerY,
            double x1, double y1, double z1, double x2, double y2, double z2,
            Pen pen, float angle)
        {
            try
            {
                var start = Rotate3DPoint(x1, y1, z1, angle);
                var end = Rotate3DPoint(x2, y2, z2, angle);

                g.DrawLine(pen,
                    centerX + start.X,
                    centerY - start.Y,
                    centerX + end.X,
                    centerY - end.Y);
            }
            catch (Exception ex)
            {
                // 錯誤處理但繼續執行
                Console.WriteLine($"Error in DrawRotated3DLine: {ex.Message}");
            }
        }
        private PointF Rotate3DPoint(double x, double y, double z, float angle)
        {
            try
            {
                // 轉換角度為弧度
                double radians = angle * Math.PI / 180.0;
                double cos = Math.Cos(radians);
                double sin = Math.Sin(radians);

                // 應用旋轉矩陣（繞Y軸旋轉）
                double rotatedX = x * cos + z * sin;
                double rotatedY = y;
                double rotatedZ = -x * sin + z * cos;

                // 添加簡單的透視效果
                double perspective = 1.0 + rotatedZ * 0.001;

                // 返回最終的2D點
                return new PointF(
                    (float)(rotatedX * perspective),
                    (float)(rotatedY * perspective)
                );
            }
            catch (Exception ex)
            {
                // 錯誤處理
                Console.WriteLine($"Error in Rotate3DPoint: {ex.Message}");
                return new PointF(0, 0); // 返回原點作為後備選項
            }
        }

        // 可選：添加更複雜的旋轉功能
        private Matrix4x4 CreateRotationMatrix(float angleX, float angleY, float angleZ)
        {
            // 轉換為弧度
            float radiansX = angleX * (float)Math.PI / 180.0f;
            float radiansY = angleY * (float)Math.PI / 180.0f;
            float radiansZ = angleZ * (float)Math.PI / 180.0f;

            // 建立各軸的旋轉矩陣
            Matrix4x4 rotationX = Matrix4x4.CreateRotationX(radiansX);
            Matrix4x4 rotationY = Matrix4x4.CreateRotationY(radiansY);
            Matrix4x4 rotationZ = Matrix4x4.CreateRotationZ(radiansZ);

            // 組合旋轉矩陣
            return rotationX * rotationY * rotationZ;
        }

        // 可選：添加向量旋轉方法
        private Vector3 RotateVector(Vector3 vector, float angleX, float angleY, float angleZ)
        {
            Matrix4x4 rotation = CreateRotationMatrix(angleX, angleY, angleZ);
            return Vector3.Transform(vector, rotation);
        }

        // 可選：添加透視投影方法
        private PointF Project3DTo2D(Vector3 point3D, float fov, float aspectRatio, float nearPlane, float farPlane)
        {
            // 建立透視投影矩陣
            Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView(
                fov * (float)Math.PI / 180.0f,
                aspectRatio,
                nearPlane,
                farPlane
            );

            // 應用投影
            Vector3 projected = Vector3.Transform(point3D, projection);

            // 轉換到螢幕座標
            return new PointF(
                projected.X * pictureBox.Width / 2 + pictureBox.Width / 2,
                -projected.Y * pictureBox.Height / 2 + pictureBox.Height / 2
            );
        }
    }

    public class WordEmbedding
    {
        private readonly ConcurrentDictionary<string, MathVector> wordVectors2D;
        private readonly ConcurrentDictionary<string, MathVector> wordVectors3D;
        private readonly ConcurrentDictionary<string, WordFrequency> wordFrequencies;
        private readonly Random random;
        private int totalTokenCount;
        private readonly bool useCBOW;
        private readonly int windowSize;
        private readonly double learningRate;
        private readonly List<string> originalTokens;
        public int GetTotalTokenCount() => totalTokenCount;
        public int GetProcessedTokenCount() => wordFrequencies.Values.Sum(wf => wf.Frequency);
        private static readonly HashSet<string> StopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // 英文停用字
            "a", "an", "and", "are", "as", "at", "be", "by", "for", "from",
            "has", "he", "in", "is", "it", "its", "of", "on", "that", "the",
            "to", "was", "were", "will", "with", "the", "this", "but", "they",
            "have", "had", "what", "when", "where", "who", "which", "why", "how",
            "we","or","these","not","then","also","been","no", "our", "can", "both",
            "more", "there","those","into","one","two","three","four","five","six","seven","eight","nine","ten",
            
            // 數字和單個字母
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "0",
            "a", "b", "c", "d", "e", "f", "g", "h", "i", "j",
            "k", "l", "m", "n", "o", "p", "q", "r", "s", "t",
            "u", "v", "w", "x", "y", "z",
            
            // 特殊字符
            ",", ".", "!", "?", ";", ":", "'", "\"", "(", ")", "[", "]",
            "{", "}", "/", "\\", "|", "<", ">", "+", "-", "*", "=", "_"
        };

        public WordEmbedding(List<string> words, bool useCBOW = false, int windowSize = 2, double learningRate = 0.025)
        {
            this.useCBOW = useCBOW;
            this.windowSize = windowSize;
            this.learningRate = learningRate;
            this.originalTokens = words ?? new List<string>();

            random = new Random();
            wordVectors2D = new ConcurrentDictionary<string, MathNet.Numerics.LinearAlgebra.Vector<double>>();
            wordVectors3D = new ConcurrentDictionary<string, MathNet.Numerics.LinearAlgebra.Vector<double>>();
            wordFrequencies = new ConcurrentDictionary<string, WordFrequency>();

            totalTokenCount = words?.Count ?? 0;

            InitializeWordFrequencies(words);
            var filteredWords = wordFrequencies.Keys.ToList();
            InitializeVectors(filteredWords);
            if (useCBOW)
            {
                TrainCBOW();
            }
        }
        public bool LoadWordVectors(string text)
        {
            try
            {
                // 清空現有的向量
                wordVectors2D.Clear();
                wordVectors3D.Clear();

                // 分行處理文本
                var lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    ProcessLine(line.Trim());
                }

                // 驗證是否成功載入向量
                if (wordVectors2D.Count == 0)
                {
                    Console.WriteLine("沒有成功載入任何詞向量");
                    return false;
                }

                // 更新總token數
                totalTokenCount = wordVectors2D.Count;

                Console.WriteLine($"成功載入 {wordVectors2D.Count} 個詞向量");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"載入詞向量時發生錯誤: {ex.Message}");
                return false;
            }
        }
        public bool SaveWordVectors(string filePath)
        {
            try
            {
                using (var writer = new StreamWriter(filePath))
                {
                    foreach (var kvp in wordVectors2D)
                    {
                        var word = kvp.Key;
                        var vector2D = kvp.Value;
                        var vector3D = wordVectors3D[word];

                        // 寫入格式：詞 2D向量值 3D向量值
                        writer.WriteLine($"{word} {string.Join(" ", vector2D.ToArray())} {string.Join(" ", vector3D.ToArray())}");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存詞向量時發生錯誤: {ex.Message}");
                return false;
            }
        }
        private void ProcessLine(string line)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(line)) return;

                var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 3) // 至少需要一個詞和兩個維度的向量值
                {
                    Console.WriteLine($"跳過無效行: {line}");
                    return;
                }

                string word = parts[0].ToLower(); // 保持一致的小寫處理

                try
                {
                    // 解析2D向量
                    var values2D = new double[2];
                    for (int i = 0; i < 2; i++)
                    {
                        if (!double.TryParse(parts[i + 1], out values2D[i]))
                        {
                            Console.WriteLine($"無法解析2D向量值: {parts[i + 1]}，詞: {word}");
                            return;
                        }
                    }

                    // 解析3D向量（如果有）
                    var values3D = new double[3];
                    for (int i = 0; i < Math.Min(3, parts.Length - 1); i++)
                    {
                        if (!double.TryParse(parts[i + 1], out values3D[i]))
                        {
                            Console.WriteLine($"無法解析3D向量值: {parts[i + 1]}，詞: {word}");
                            return;
                        }
                    }

                    // 將向量添加到相應的集合中
                    var vector2D = MathVector.Build.DenseOfArray(values2D);
                    var vector3D = MathVector.Build.DenseOfArray(values3D);

                    wordVectors2D.AddOrUpdate(word, vector2D, (k, v) => vector2D);
                    wordVectors3D.AddOrUpdate(word, vector3D, (k, v) => vector3D);

                    // 更新詞頻信息
                    wordFrequencies.AddOrUpdate(word,
                        new WordFrequency { Word = word, Frequency = 1, Length = word.Length },
                        (k, v) => { v.Frequency++; return v; });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"處理向量值時發生錯誤: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"處理行時發生錯誤: {line}\n錯誤: {ex.Message}");
            }
        }

        public List<string> GetOriginalTokens()
        {
            return new List<string>(originalTokens);
        }

        private void TrainCBOW()
        {
            // 移除停用詞後的tokens
            var validTokens = originalTokens
                .Where(w => !string.IsNullOrWhiteSpace(w) && !StopWords.Contains(w.ToLower()))
                .Select(w => w.ToLower())
                .ToList();

            const int epochs = 5;

            for (int epoch = 0; epoch < epochs; epoch++)
            {
                for (int i = 0; i < validTokens.Count; i++)
                {
                    // 獲取目標詞和上下文
                    var targetWord = validTokens[i];
                    var contextWords = GetContextWords(validTokens, i, windowSize);

                    if (!contextWords.Any()) continue;

                    // 2D training
                    TrainCBOWForDimension(targetWord, contextWords, false);

                    // 3D training
                    TrainCBOWForDimension(targetWord, contextWords, true);
                }
            }
        }
        private void TrainCBOWForDimension(string targetWord, List<string> contextWords, bool is3D)
        {
            var vectors = is3D ? wordVectors3D : wordVectors2D;
            var dimensions = is3D ? 3 : 2;

            // 計算上下文向量平均值
            var contextVectorSum = MathVector.Build.Dense(dimensions);
            foreach (var contextWord in contextWords)
            {
                if (vectors.TryGetValue(contextWord, out var vector))
                {
                    contextVectorSum += vector;
                }
            }
            var contextVectorAvg = contextVectorSum / contextWords.Count;

            // 更新目標詞向量
            if (vectors.TryGetValue(targetWord, out var targetVector))
            {
                var error = targetVector - contextVectorAvg;
                var updatedVector = targetVector - (error * learningRate);
                vectors.TryUpdate(targetWord, updatedVector, targetVector);
            }
        }
        private List<string> GetContextWords(List<string> tokens, int position, int windowSize)
        {
            var contextWords = new List<string>();

            // 收集前windowSize個詞
            for (int i = Math.Max(0, position - windowSize); i < position; i++)
            {
                contextWords.Add(tokens[i]);
            }

            // 收集後windowSize個詞
            for (int i = position + 1; i <= Math.Min(tokens.Count - 1, position + windowSize); i++)
            {
                contextWords.Add(tokens[i]);
            }

            return contextWords;
        }
        public double CalculateCosineSimilarity(string word1, string word2, bool is3D = false)
        {
            try
            {
                var vectors = is3D ? wordVectors3D : wordVectors2D;

                // 獲取兩個詞的向量（轉為小寫以保持一致性）
                if (!vectors.TryGetValue(word1.ToLower(), out var vector1) ||
                    !vectors.TryGetValue(word2.ToLower(), out var vector2))
                {
                    Console.WriteLine($"找不到詞向量: {word1} 或 {word2}");
                    return 0;
                }

                // 檢查向量維度
                if (vector1.Count != vector2.Count)
                {
                    throw new ArgumentException($"向量維度不匹配: {word1}={vector1.Count}, {word2}={vector2.Count}");
                }

                // 使用 MathNet.Numerics 的向量運算計算餘弦相似度
                double dotProduct = vector1.DotProduct(vector2);
                double norm1 = vector1.L2Norm();
                double norm2 = vector2.L2Norm();

                // 避免除以零
                if (norm1 == 0 || norm2 == 0)
                {
                    Console.WriteLine($"向量範數為零: {word1}={norm1}, {word2}={norm2}");
                    return 0;
                }

                // 計算餘弦相似度
                double similarity = dotProduct / (norm1 * norm2);

                // 確保結果在 -1 到 1 之間
                similarity = Math.Max(-1, Math.Min(1, similarity));

                return similarity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"計算餘弦相似度時發生錯誤: {ex.Message}");
                Console.WriteLine($"Word1: {word1}, Word2: {word2}, Is3D: {is3D}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return 0;
            }
        }
        private void InitializeWordFrequencies(List<string> words)
        {
            // 先計算詞頻
            var frequencies = words
                .Where(w => !string.IsNullOrWhiteSpace(w) && !StopWords.Contains(w.ToLower()))
                .GroupBy(w => w.ToLower())
                .ToDictionary(
                    g => g.Key,
                    g => new WordFrequency
                    {
                        Word = g.First(),  // 保留原始大小寫
                        Frequency = g.Count(),
                        Length = g.Key.Length
                    }
                );

            // 存入 ConcurrentDictionary
            foreach (var kvp in frequencies)
            {
                wordFrequencies.TryAdd(kvp.Key, kvp.Value);
            }
        }
        public List<(string Word, double Similarity)> FindMostSimilarWords(string word, int topN = 5, bool is3D = false)
        {
            var targetVector = GetWordVector(word, is3D);
            var vectors = is3D ? wordVectors3D : wordVectors2D;

            return vectors
                .AsParallel()
                .Where(kvp => kvp.Key != word.ToLower())
                .Select(kvp => (
                    Word: wordFrequencies[kvp.Key].Word,
                    Similarity: kvp.Value.DotProduct(targetVector) / (kvp.Value.L2Norm() * targetVector.L2Norm())
                ))
                .OrderByDescending(x => x.Similarity)
                .Take(topN)
                .ToList();
        }
        public List<WordFrequency> GetWordFrequencies()
        {
            return wordFrequencies.Values.ToList();
        }

        public List<string> GetAvailableWords()
        {
            return wordFrequencies.Values.Select(wf => wf.Word).ToList();
        }

        private void InitializeVectors(List<string> words)
        {
            Parallel.ForEach(words, word =>
            {
                if (!string.IsNullOrWhiteSpace(word))
                {
                    InitializeWordVectors(word);
                }
            });
        }

        private void InitializeWordVectors(string word)
        {
            try
            {
                var values2D = new[] { GetRandomValue(), GetRandomValue() };
                var values3D = new[] { GetRandomValue(), GetRandomValue(), GetRandomValue() };

                wordVectors2D.TryAdd(word, MathVector.Build.DenseOfArray(values2D));
                wordVectors3D.TryAdd(word, MathVector.Build.DenseOfArray(values3D));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing vectors for word '{word}': {ex.Message}");
            }
        }

        private double GetRandomValue()
        {
            const double maxRange = 5.0;
            return (random.NextDouble() * 2 - 1) * maxRange;
        }
        public MathVector GetWordVector(string word, bool is3D = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(word))
                    throw new ArgumentException("Word cannot be null or empty");

                var vectors = is3D ? wordVectors3D : wordVectors2D;
                if (vectors.TryGetValue(word.ToLower(), out var vector))
                    return vector;

                throw new ArgumentException($"Word '{word}' not found in the model");
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error getting vector for word '{word}': {ex.Message}", ex);
            }
        }
    }

    public class WordFrequency
    {
        public string Word { get; set; }
        public int Frequency { get; set; }
        public int Length { get; set; }

        public override string ToString()
        {
            return $"{Word} ({Frequency} times, {Length} chars)";
        }
    }


}