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
                RowCount = 3,
                ColumnCount = 1
            };
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
            gridResults.Columns.AddRange(new[]
            {
                 new DataGridViewTextBoxColumn { Name = "Rank", HeaderText = "#", Width = 40,   AutoSizeMode = DataGridViewAutoSizeColumnMode.None },
                new DataGridViewTextBoxColumn { Name = "DocumentId", HeaderText = "Document ID", Width = 80 ,AutoSizeMode = DataGridViewAutoSizeColumnMode.None},
                new DataGridViewTextBoxColumn { Name = "Sentence", HeaderText = "Sentence",AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill},
                new DataGridViewTextBoxColumn { Name = "Score", HeaderText = "Score", Width = 70 ,AutoSizeMode = DataGridViewAutoSizeColumnMode.None}
            });

            lblStatus = new Label { Dock = DockStyle.Bottom, Height = 20 };

            mainPanel.Controls.Add(controlPanel, 0, 0);
            mainPanel.Controls.Add(searchPanel, 0, 1);
            mainPanel.Controls.Add(gridResults, 0, 2);
            Controls.Add(mainPanel);
            Controls.Add(lblStatus);
            gridResults.CellDoubleClick += GridResults_CellDoubleClick;
        }


        private void GridResults_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != 1) return;

            var docId = gridResults.Rows[e.RowIndex].Cells["DocumentId"].Value.ToString();
            ShowDocumentKeySentences(docId);
        }
        private void ShowDocumentKeySentences(string docId)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                UpdateStatus($"Analyzing document {docId}...");

                var keySentences = analyzer.GetDocumentKeySentences(docId);
                gridResults.Rows.Clear();

                int rank = 1;
                foreach (var sentence in keySentences)
                {
                    gridResults.Rows.Add(
                        rank++,
                        sentence.DocumentId,
                        sentence.Sentence,
                        sentence.Score.ToString("F4")
                    );
                }
                UpdateStatus($"Found {keySentences.Count} key sentences in document {docId}");
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
                    gridResults.Rows.Add(
                        rank++,
                        sentence.DocumentId,
                        $"P{sentence.ParagraphIndex + 1}S{sentence.SentenceIndex + 1}: {sentence.Sentence}",
                        sentence.Score.ToString("F4"));
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

                var results = analyzer.FindSimilarSentences(txtQuery.Text);
                gridResults.Rows.Clear();

                int rank = 1;
                foreach (var result in results)
                {
                    gridResults.Rows.Add(
                        rank++,
                        result.DocumentId,
                        result.Sentence,
                        result.Score.ToString("F4"));
                }

                UpdateStatus($"Found {results.Count} similar sentences");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Search error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        public TfIdfAnalyzer()
        {
            documentVectors = new Dictionary<string, Dictionary<string, double>>();
            idf = new Dictionary<string, double>();
            documentSentences = new Dictionary<string, List<string>>();
            InitializeStopWords();
            sentencePositions = new Dictionary<string, Dictionary<int, string>>();
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
                .Take(topN)
                .ToList();
        }
        private List<List<string>> GetParagraphs(List<string> sentences)
        {
            var paragraphs = new List<List<string>>();
            var currentParagraph = new List<string>();

            foreach (var sentence in sentences)
            {
                if (sentence.Length > 0)
                {
                    currentParagraph.Add(sentence);

                    // Assume paragraph break on sentences ending with specific punctuation
                    if (sentence.EndsWith(".") || sentence.EndsWith("!") || sentence.EndsWith("?"))
                    {
                        if (currentParagraph.Count > 0)
                        {
                            paragraphs.Add(new List<string>(currentParagraph));
                            currentParagraph.Clear();
                        }
                    }
                }
            }

            if (currentParagraph.Count > 0)
            {
                paragraphs.Add(currentParagraph);
            }

            return paragraphs;
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
            var preprocessedDocs = documents.Select((doc, index) => new
            {
                Id = $"doc_{index}",
                Content = PreprocessText(doc),
                Sentences = SplitIntoSentences(doc)
            }).ToList();

            // Calculate IDF
            var wordDocFreq = new Dictionary<string, int>();
            foreach (var doc in preprocessedDocs)
            {
                var uniqueWords = new HashSet<string>(doc.Content);
                foreach (var word in uniqueWords)
                {
                    if (!wordDocFreq.ContainsKey(word))
                        wordDocFreq[word] = 0;
                    wordDocFreq[word]++;
                }

                documentSentences[doc.Id] = doc.Sentences;
            }

            // Calculate IDF scores
            int totalDocs = preprocessedDocs.Count;
            foreach (var word in wordDocFreq.Keys)
            {
                idf[word] = Math.Log((double)totalDocs / wordDocFreq[word]);
            }

            // Calculate TF-IDF vectors
            foreach (var doc in preprocessedDocs)
            {
                var wordFreq = doc.Content
                    .GroupBy(w => w)
                    .ToDictionary(g => g.Key, g => g.Count());

                var vector = new Dictionary<string, double>();
                foreach (var word in wordFreq.Keys)
                {
                    if (idf.ContainsKey(word))
                    {
                        vector[word] = wordFreq[word] * idf[word];
                    }
                }
                documentVectors[doc.Id] = vector;
            }
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
                .Take(topN)
                .ToList();
        }

        private Dictionary<string, double> CreateQueryVector(string text)
        {
            var words = PreprocessText(text);
            var wordFreq = words
                .GroupBy(w => w)
                .ToDictionary(g => g.Key, g => g.Count());

            var vector = new Dictionary<string, double>();
            foreach (var word in wordFreq.Keys)
            {
                if (idf.ContainsKey(word))
                {
                    vector[word] = wordFreq[word] * idf[word];  // TF * IDF
                }
            }
            return vector;
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
        public List<SimilarSentence> GetDocumentKeySentences(string docId)
        {
            var sentences = documentSentences[docId];
            var sentenceScores = new List<SimilarSentence>();
            var docVector = documentVectors[docId];
            int totalSentences = sentences.Count;

            for (int i = 0; i < sentences.Count; i++)
            {
                var sentence = sentences[i];
                var sentenceVector = CreateQueryVector(sentence);
                var baseScore = CalculateImportanceScore(sentenceVector, docVector);

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

            return sentenceScores
                .OrderByDescending(x => x.Score)
                .ToList();
        }
    }
}
