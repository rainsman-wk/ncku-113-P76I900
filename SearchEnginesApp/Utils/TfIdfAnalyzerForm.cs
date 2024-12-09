using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SearchEnginesApp.Utils.TfIdfAnalyzer;

namespace SearchEnginesApp.Utils
{
    public partial class TfIdfAnalyzerForm : Form
    {
        private TfIdfAnalyzer analyzer;
        private List<string> documents;
        private DataGridView gridResults;
        private TextBox txtQuery;
        private Button btnSearch;
        private Button btnKeySentences;
        private Label lblStatus;
        private Button btnCompare;

        public TfIdfAnalyzerForm(List<string> document)
        {
            InitializeForm();
            SetupEventHandlers();
            documents = document;

            if (documents.Count == 0)
            {
                MessageBox.Show("No abstracts found in database.");
                return;
            }

            analyzer = new TfIdfAnalyzer();
            analyzer.ProcessDocuments(documents);
            btnSearch.Enabled = true;
            btnKeySentences.Enabled = true;
            UpdateStatus($"Loaded {documents.Count} documents");
        }
        private void InitializeForm()
        {
            Text = "TF-IDF Document Analyzer";
            Size = new Size(800, 600);

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 4,
                ColumnCount = 1
            };
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var controlPanel = new Panel { Dock = DockStyle.Fill };
            btnKeySentences = new Button
            {
                Text = "Show Key Sentences",
                Width = 120,
                Location = new Point(10, 10),
                Enabled = false
            };
            controlPanel.Controls.Add(btnKeySentences);

            var searchPanel = new Panel { Dock = DockStyle.Fill };
            txtQuery = new TextBox
            {
                Multiline = true,
                Height = 60,
                Width = 600,
                Location = new Point(10, 10)
            };
            btnSearch = new Button
            {
                Text = "Search",
                Width = 100,
                Location = new Point(620, 25),
                Enabled = false
            };
            searchPanel.Controls.AddRange(new Control[] { txtQuery, btnSearch });

            gridResults = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true
            };
            gridResults.Columns.Clear();
            gridResults.Columns.AddRange(new[] 
            {
                new DataGridViewTextBoxColumn { Name = "Rank", HeaderText = "#", Width = 40, AutoSizeMode = DataGridViewAutoSizeColumnMode.None },
                new DataGridViewTextBoxColumn { Name = "DocumentId", HeaderText = "Doc ID", Width = 60, AutoSizeMode = DataGridViewAutoSizeColumnMode.None },
                new DataGridViewTextBoxColumn { Name = "Position", HeaderText = "Position", Width = 80, AutoSizeMode = DataGridViewAutoSizeColumnMode.None },
                new DataGridViewTextBoxColumn { Name = "Sentence", HeaderText = "Sentence", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill },
                new DataGridViewTextBoxColumn { Name = "Scores", HeaderText = "Scores", Width = 200, AutoSizeMode = DataGridViewAutoSizeColumnMode.None }
            });

            lblStatus = new Label { Dock = DockStyle.Bottom, Height = 20 };

            var algorithmPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 40
            };

            var radioGroup = new Panel
            {
                Width = 200,
                Height = 30,
                Location = new Point(10, 5)
            };

            var classicRadio = new RadioButton
            {
                Text = "Classic",
                Location = new Point(0, 5),
                Checked = true,
                AutoSize = true
            };

            var bm25Radio = new RadioButton
            {
                Text = "BM25",
                Location = new Point(80, 5),
                AutoSize = true
            };

            mainPanel.Controls.Add(algorithmPanel, 0, 0);
            mainPanel.Controls.Add(controlPanel, 0, 1);
            mainPanel.Controls.Add(searchPanel, 0, 2);
            mainPanel.Controls.Add(gridResults, 0, 3);

            analyzer = new TfIdfAnalyzer();

            classicRadio.CheckedChanged += (s, e) => {
                if (classicRadio.Checked && analyzer != null)
                {
                    analyzer.SetTfIdfStrategy(false);
                    UpdateStatus("Using Classic TF-IDF");
                }
            };

            bm25Radio.CheckedChanged += (s, e) => {
                if (bm25Radio.Checked && analyzer != null)
                {
                    analyzer.SetTfIdfStrategy(true);
                    UpdateStatus("Using BM25 TF-IDF");
                }
            };
            radioGroup.Controls.AddRange(new Control[] { classicRadio, bm25Radio });
            algorithmPanel.Controls.Add(radioGroup);

            Controls.Add(mainPanel);
            Controls.Add(lblStatus);
            gridResults.CellDoubleClick += GridResults_CellDoubleClick;

            btnCompare = new Button
            {
                Text = "Compare Algorithms",
                Width = 120,
                Location = new Point(140, 10)
            };
            controlPanel.Controls.Add(btnCompare);

            btnCompare.Click += (s, e) => CompareAlgorithms();
        }
        private void CompareAlgorithms()
        {
            try
            {
                 Cursor = Cursors.WaitCursor;
        gridResults.Rows.Clear();

        if (string.IsNullOrWhiteSpace(txtQuery.Text))
        {
            analyzer.SetTfIdfStrategy(false);
            var classicResults = analyzer.GetKeyParagraphSentences(10)
                .Select((r, i) => new { Result = r, ClassicRank = i + 1 });

            analyzer.SetTfIdfStrategy(true);
            var bm25Results = analyzer.GetKeyParagraphSentences(10)
                .Select((r, i) => new { Result = r, BM25Rank = i + 1 });

            foreach (var classic in classicResults)
            {
                var bm25 = bm25Results.FirstOrDefault(b => b.Result.Sentence == classic.Result.Sentence);
                string rankDisplay = $"C:{classic.ClassicRank,2} B:{bm25?.BM25Rank ?? 0,2}";
                string position = $"P{classic.Result.ParagraphIndex + 1}S{classic.Result.SentenceIndex + 1}";

                gridResults.Rows.Add(
                    rankDisplay,
                    classic.Result.DocumentId,
                    position,
                    classic.Result.Sentence,
                    $"Classic: {classic.Result.Score,6:F1}\n  BM25: {(bm25?.Result.Score ?? 0),6:F1}"
                );
            }

            UpdateStatus("Compared key sentences using both algorithms");
        }
        else
        {
            analyzer.SetTfIdfStrategy(false);
            var classicResults = analyzer.FindSimilarSentences(txtQuery.Text, 10)
                .Select((r, i) => new { Result = r, ClassicRank = i + 1 });

            analyzer.SetTfIdfStrategy(true);
            var bm25Results = analyzer.FindSimilarSentences(txtQuery.Text, 10)
                .Select((r, i) => new { Result = r, BM25Rank = i + 1 });

            foreach (var classic in classicResults)
            {
                var bm25 = bm25Results.FirstOrDefault(b => b.Result.Sentence == classic.Result.Sentence);
                string rankDisplay = $"C:{classic.ClassicRank,2} B:{bm25?.BM25Rank ?? 0,2}";

                var docParagraphs = analyzer.GetParagraphs(analyzer.GetDocumentSentences(classic.Result.DocumentId));
                int paraIndex = 0, sentIndex = 0;
                
                for (int i = 0; i < docParagraphs.Count; i++)
                {
                    var sentenceIndex = docParagraphs[i].FindIndex(s => s == classic.Result.Sentence);
                    if (sentenceIndex != -1)
                    {
                        paraIndex = i;
                        sentIndex = sentenceIndex;
                        break;
                    }
                }

                string position = $"P{paraIndex + 1}S{sentIndex + 1}";

                gridResults.Rows.Add(
                    rankDisplay,
                    classic.Result.DocumentId,
                    position,
                    classic.Result.Sentence,
                    $"Classic: {classic.Result.Score,6:F1}\n  BM25: {(bm25?.Result.Score ?? 0),6:F1}"
                );
            }

            UpdateStatus($"Compared search results for query: {txtQuery.Text}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Comparison error: {ex.Message}");
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void GridResults_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == 1 || e.ColumnIndex == 2)
            {
                var docId = gridResults.Rows[e.RowIndex].Cells["DocumentId"].Value.ToString();
                ShowDocumentKeySentences(docId);
            }
        }
        private void ShowDocumentKeySentences(string docId)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                UpdateStatus($"Analyzing document {docId}...");

                var keySentences = analyzer.GetDocumentKeySentences(docId)
                    .Select(s => s as RankedSentence)
                    .Where(s => s != null)
                    .ToList();

                gridResults.Rows.Clear();
                int rank = 1;

                foreach (var sentence in keySentences)
                {
                    string position = $"P{sentence.ParagraphIndex + 1}S{sentence.SentenceIndex + 1}";
                    gridResults.Rows.Add(
                        rank++,
                        sentence.DocumentId,
                        position,
                        sentence.Sentence,
                        sentence.Score.ToString("F1")
                    );
                }

                UpdateStatus($"Found {keySentences.Count} key sentences");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
        private void SetupEventHandlers()
        {
            btnSearch.Click += (s, e) => SearchDocuments();
            btnKeySentences.Click += (s, e) => ShowKeyDocumentSentences();
        }

        private void ShowKeyDocumentSentences()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                UpdateStatus("Finding key sentences...");
                gridResults.Rows.Clear();

                var keyParagraphSentences = analyzer.GetKeyParagraphSentences();
                int rank = 1;

                foreach (var sentence in keyParagraphSentences)
                {
                    string position = $"P{sentence.ParagraphIndex + 1}S{sentence.SentenceIndex + 1}";
                    gridResults.Rows.Add(
                        rank++,
                        sentence.DocumentId,
                        position,
                        sentence.Sentence,
                        sentence.Score.ToString("F1")
                    );
                }

                UpdateStatus($"Found {keyParagraphSentences.Count} key sentences");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void SearchDocuments()
        {
            if (string.IsNullOrWhiteSpace(txtQuery.Text))
            {
                MessageBox.Show("Please enter a search query.");
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                UpdateStatus("Searching...");
                gridResults.Rows.Clear();

                var paragraphs = new List<List<string>>();
                var results = analyzer.FindSimilarSentences(txtQuery.Text);

                int rank = 1;
                foreach (var result in results)
                {
                    // 獲取句子在段落中的位置
                    var docParagraphs = analyzer.GetParagraphs(analyzer.GetDocumentSentences(result.DocumentId));
                    int paraIndex = 0, sentIndex = 0;
                    bool found = false;

                    for (int i = 0; i < docParagraphs.Count && !found; i++)
                    {
                        var sentenceIndex = docParagraphs[i].FindIndex(s => s == result.Sentence);
                        if (sentenceIndex != -1)
                        {
                            paraIndex = i;
                            sentIndex = sentenceIndex;
                            found = true;
                        }
                    }

                    string position = found ? $"P{paraIndex + 1}S{sentIndex + 1}" : "-";

                    gridResults.Rows.Add(
                        rank++,
                        result.DocumentId,
                        position,
                        result.Sentence,
                        result.Score.ToString("F1")
                    );
                }

                UpdateStatus($"Found {results.Count} similar sentences");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Search error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void UpdateStatus(string message)
        {
            lblStatus.Text = message;
        }
    }

    public class TfIdfAnalyzer
    {
        private Dictionary<string, Dictionary<int, string>> sentencePositions;
        private double headWeight = 1.0;
        private double tailWeight = 1.0;
        private double headWindowPercent = 0.2;
        private double tailWindowPercent = 0.2;
        private Dictionary<string, Dictionary<string, double>> documentVectors;
        private Dictionary<string, double> idf;
        private HashSet<string> stopWords;
        private Dictionary<string, List<string>> documentSentences;
        private ITfIdfStrategy tfidfStrategy;
        private Dictionary<string, int> wordDocFreq;
        private Dictionary<string, List<List<string>>> documentParagraphs;

        public TfIdfAnalyzer()
        {
            documentVectors = new Dictionary<string, Dictionary<string, double>>();
            idf = new Dictionary<string, double>();
            documentSentences = new Dictionary<string, List<string>>();
            wordDocFreq = new Dictionary<string, int>();  // 初始化
            InitializeStopWords();
            sentencePositions = new Dictionary<string, Dictionary<int, string>>();
            tfidfStrategy = new ClassicTfIdf();  // 設定默認策略
            documentParagraphs = new Dictionary<string, List<List<string>>>();
        }
        public List<string> GetDocumentSentences(string docId)
        {
            return documentSentences.ContainsKey(docId) ? documentSentences[docId] : new List<string>();
        }
        public void SetTfIdfStrategy(bool useBM25)
        {
            tfidfStrategy = useBM25
                ? new BM25TfIdf(documentSentences.Values.ToList())
                : (ITfIdfStrategy)new ClassicTfIdf();
        }
        public List<RankedSentence> GetKeyParagraphSentences(int topN = 100)
        {
            var allSentences = new List<RankedSentence>();

            foreach (var docId in documentSentences.Keys)
            {
                var sentences = documentSentences[docId];
                var docVector = documentVectors[docId];
                var paragraphs = GetParagraphs(sentences);

                foreach (var paragraph in paragraphs)
                {
                    var paragraphSentences = new List<RankedSentence>();

                    for (int i = 0; i < paragraph.Count; i++)
                    {
                        var sentence = paragraph[i];
                        var sentenceVector = CreateQueryVector(sentence);
                        var baseScore = CalculateImportanceScore(sentenceVector, docVector);

                        // Apply position weights within paragraph
                        var positionWeight = 1.0;
                        var relativePosition = (double)i / paragraph.Count;

                        if (relativePosition <= headWindowPercent)
                            positionWeight = headWeight;
                        else if (relativePosition >= (1 - tailWindowPercent))
                            positionWeight = tailWeight;

                        paragraphSentences.Add(new RankedSentence
                        {
                            DocumentId = docId,
                            Sentence = sentence,
                            Score = baseScore * positionWeight,
                            ParagraphIndex = paragraphs.IndexOf(paragraph),
                            SentenceIndex = i
                        });
                    }

                    // Get top sentence from each paragraph
                    if (paragraphSentences.Any())
                    {
                        allSentences.Add(paragraphSentences.OrderByDescending(s => s.Score).First());
                    }
                }
            }

            return allSentences
                .OrderByDescending(s => s.Score)
                //.Take(topN)
                .ToList();
        }
        private List<List<string>> GetParagraphsInternal(List<string> sentences)
        {
            var paragraphs = new List<List<string>>();
            var currentParagraph = new List<string>();

            foreach (var sentence in sentences)
            {
                if (sentence.Length > 0)
                {
                    currentParagraph.Add(sentence);
                    if (sentence.EndsWith(".") || sentence.EndsWith("!") || sentence.EndsWith("?"))
                    {
                        paragraphs.Add(new List<string>(currentParagraph));
                        currentParagraph = new List<string>();
                    }
                }
            }

            if (currentParagraph.Count > 0)
            {
                paragraphs.Add(currentParagraph);
            }

            return paragraphs;
        }
        public List<List<string>> GetParagraphs(List<string> sentences)
        {
            return GetParagraphsInternal(sentences);
        }

        public class RankedSentence : SimilarSentence
        {
            public int ParagraphIndex { get; set; }
            public int SentenceIndex { get; set; }
        }

        private void InitializeStopWords()
        {
            stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "a", "an", "and", "are", "as", "at", "be", "by", "for", "from",
            "has", "he", "in", "is", "it", "its", "of", "on", "that", "the",
            "to", "was", "were", "will", "with"
        };
        }

        public void ProcessDocuments(List<string> documents)
        {
            documentVectors.Clear();
            wordDocFreq.Clear();
            documentSentences.Clear();
            documentParagraphs.Clear();

            var preprocessedDocs = documents.Select((doc, index) => new
            {
                Id = $"doc_{index}",
                Content = PreprocessText(doc),
                Sentences = SplitIntoSentences(doc)
            }).ToList();

            foreach (var doc in preprocessedDocs)
            {
                documentSentences[doc.Id] = doc.Sentences;
                documentParagraphs[doc.Id] = GetParagraphsInternal(doc.Sentences);

                var uniqueWords = new HashSet<string>(doc.Content);
                foreach (var word in uniqueWords)
                {
                    if (!wordDocFreq.ContainsKey(word))
                        wordDocFreq[word] = 0;
                    wordDocFreq[word]++;
                }

                var wordFreq = doc.Content
                    .GroupBy(w => w)
                    .ToDictionary(g => g.Key, g => g.Count());

                documentVectors[doc.Id] = wordFreq.Keys
                    .ToDictionary(
                        word => word,
                        word => wordFreq[word] * Math.Log((double)preprocessedDocs.Count / wordDocFreq[word])
                    );
            }

            SetTfIdfStrategy(false);
        }

        private List<string> PreprocessText(string text)
        {
            return Regex.Replace(text.ToLower(), @"[^a-z0-9\s]", " ")
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(word => !stopWords.Contains(word))
                .ToList();
        }

        private List<string> SplitIntoSentences(string text)
        {
            return Regex.Split(text.Trim(), @"(?<=[.!?])\s+")
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
        }

        public List<SimilarSentence> FindSimilarSentences(string queryText, int topN = 5)
        {
            var queryVector = CreateQueryVector(queryText);
            var results = new List<SimilarSentence>();

            foreach (var docId in documentSentences.Keys)
            {
                foreach (var sentence in documentSentences[docId])
                {
                    var sentenceVector = CreateQueryVector(sentence);
                    var similarity = CalculateCosineSimilarity(queryVector, sentenceVector);

                    results.Add(new SimilarSentence
                    {
                        Sentence = sentence,
                        Score = similarity,
                        DocumentId = docId
                    });
                }
            }

            return results
                .Where(r => r.Score > 0)
                .OrderByDescending(r => r.Score)
                //.Take(topN)
                .ToList();
        }

        private Dictionary<string, double> CreateQueryVector(string text)
        {
            var words = PreprocessText(text);
            return tfidfStrategy.CalculateVector(words, wordDocFreq, documentSentences.Count);
        }

        private double CalculateCosineSimilarity(Dictionary<string, double> vec1,
            Dictionary<string, double> vec2)
        {
            var intersection = vec1.Keys.Intersect(vec2.Keys);

            var dotProduct = intersection.Sum(word => vec1[word] * vec2[word]);
            var norm1 = Math.Sqrt(vec1.Values.Sum(v => v * v));
            var norm2 = Math.Sqrt(vec2.Values.Sum(v => v * v));

            return norm1 > 0 && norm2 > 0 ? dotProduct / (norm1 * norm2) : 0;
        }

        public class SimilarSentence
        {
            public string Sentence { get; set; }
            public double Score { get; set; }
            public string DocumentId { get; set; }
        }
        public List<SimilarSentence> GetKeySentences(int topN = 10)
        {
            var sentenceScores = new List<SimilarSentence>();

            foreach (var docId in documentSentences.Keys)
            {
                var sentences = documentSentences[docId];
                int totalSentences = sentences.Count;

                for (int i = 0; i < sentences.Count; i++)
                {
                    var sentence = sentences[i];
                    var sentenceVector = CreateQueryVector(sentence);
                    var docVector = documentVectors[docId];
                    var baseScore = CalculateImportanceScore(sentenceVector, docVector);

                    // Apply position weights
                    double positionWeight = 1.0;
                    if (i <= totalSentences * headWindowPercent)
                        positionWeight = headWeight;
                    else if (i >= totalSentences * (1 - tailWindowPercent))
                        positionWeight = tailWeight;

                    sentenceScores.Add(new SimilarSentence
                    {
                        DocumentId = docId,
                        Sentence = sentence,
                        Score = baseScore * positionWeight
                    });
                }
            }

            return sentenceScores
                .OrderByDescending(x => x.Score)
                .Take(topN)
                .ToList();
        }

        public void SetWeightParameters(double headWeight, double tailWeight, double headWindowPercent, double tailWindowPercent)
        {
            this.headWeight = headWeight;
            this.tailWeight = tailWeight;
            this.headWindowPercent = headWindowPercent;
            this.tailWindowPercent = tailWindowPercent;
        }
        private double CalculateImportanceScore(Dictionary<string, double> sentenceVector, Dictionary<string, double> docVector)
        {
            // Calculate base TF-IDF score
            double tfidfScore = 0;
            foreach (var word in sentenceVector.Keys)
            {
                if (docVector.ContainsKey(word))
                {
                    tfidfScore += sentenceVector[word] * docVector[word];
                }
            }

            double lengthNormalization = 1.0 / Math.Sqrt(sentenceVector.Count);
            return tfidfScore * lengthNormalization;
        }
        public List<RankedSentence> GetDocumentKeySentences(string docId)
        {
            if (!documentSentences.ContainsKey(docId))
                return new List<RankedSentence>();

            var sentences = documentSentences[docId];
            var paragraphs = GetParagraphs(sentences);
            var sentenceScores = new List<RankedSentence>();
            var docVector = documentVectors[docId];

            for (int paraIndex = 0; paraIndex < paragraphs.Count; paraIndex++)
            {
                var paragraph = paragraphs[paraIndex];

                for (int sentIndex = 0; sentIndex < paragraph.Count; sentIndex++)
                {
                    var sentence = paragraph[sentIndex];
                    var sentenceVector = CreateQueryVector(sentence);
                    var baseScore = CalculateImportanceScore(sentenceVector, docVector);

                    double positionWeight = 1.0;
                    if (sentIndex <= paragraph.Count * headWindowPercent)
                        positionWeight = headWeight;
                    else if (sentIndex >= paragraph.Count * (1 - tailWindowPercent))
                        positionWeight = tailWeight;

                    sentenceScores.Add(new RankedSentence
                    {
                        DocumentId = docId,
                        Sentence = sentence,
                        Score = baseScore * positionWeight,
                        ParagraphIndex = paraIndex,
                        SentenceIndex = sentIndex
                    });
                }
            }

            return sentenceScores
                .OrderByDescending(x => x.Score)
                .ToList();
        }
    }

    public interface ITfIdfStrategy
    {
        Dictionary<string, double> CalculateVector(List<string> terms, Dictionary<string, int> wordDocFreq, int totalDocs);
    }

    public class ClassicTfIdf : ITfIdfStrategy
    {
        public Dictionary<string, double> CalculateVector(List<string> terms, Dictionary<string, int> wordDocFreq, int totalDocs)
        {
            var wordFreq = terms.GroupBy(w => w)
                               .ToDictionary(g => g.Key, g => g.Count());

            return wordFreq.Keys
                .Where(word => wordDocFreq.ContainsKey(word))
                .ToDictionary(
                    word => word,
                    word => wordFreq[word] * Math.Log((double)totalDocs / wordDocFreq[word])
                );
        }
    }

    public class BM25TfIdf : ITfIdfStrategy
    {
        private const double k1 = 1.2;
        private const double b = 0.75;
        private double avgDocLength;

        public BM25TfIdf(List<List<string>> documents)
        {
            avgDocLength = documents.Average(d => d.Count);
        }

        public Dictionary<string, double> CalculateVector(List<string> terms, Dictionary<string, int> wordDocFreq, int totalDocs)
        {
            var wordFreq = terms.GroupBy(w => w)
                               .ToDictionary(g => g.Key, g => g.Count());

            return wordFreq.Keys
                .Where(word => wordDocFreq.ContainsKey(word))
                .ToDictionary(
                    word => word,
                    word =>
                    {
                        double tf = ((k1 + 1) * wordFreq[word]) /
                                   (k1 * (1 - b + b * terms.Count / avgDocLength) + wordFreq[word]);
                        double idf = Math.Log((totalDocs - wordDocFreq[word] + 0.5) /
                                            (wordDocFreq[word] + 0.5));
                        return tf * idf;
                    }
                );
        }
    }
}
