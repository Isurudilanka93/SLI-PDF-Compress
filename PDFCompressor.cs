using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace PDFCompressor
{
    public class CompressorForm : Form
    {
        private ProgressBar progressBar;
        private Label statusLabel;
        private Button cancelButton;
        private bool cancelled = false;
        private string gsPath = @"C:\Users\Helpdesk.Landscaping\Downloads\CommonFiles\Ghostscript\bin\gswin64c.exe";

        public CompressorForm(string[] files, string outputFolder)
        {
            Text = "PDF Compression";
            Width = 400;
            Height = 150;

            progressBar = new ProgressBar()
            {
                Minimum = 0,
                Maximum = files.Length,
                Width = 350,
                Height = 25,
                Left = 20,
                Top = 20
            };

            statusLabel = new Label()
            {
                AutoSize = true,
                Left = 20,
                Top = 60
            };

            cancelButton = new Button()
            {
                Text = "Cancel",
                Left = 150,
                Top = 90
            };
            cancelButton.Click += (s, e) => { cancelled = true; Close(); };

            Controls.Add(progressBar);
            Controls.Add(statusLabel);
            Controls.Add(cancelButton);

            Shown += (s, e) => CompressFiles(files, outputFolder);
        }

        private void CompressFiles(string[] files, string outputFolder)
        {
            int i = 0;
            foreach (var file in files)
            {
                if (cancelled) break;
                i++;
                statusLabel.Text = "Compressing: " + Path.GetFileName(file);
                Refresh();

                string outFile = Path.Combine(
                    outputFolder,
                    Path.GetFileNameWithoutExtension(file) + "_compressed.pdf");

                // Rewritten without string interpolation
                string args = string.Format(
                    "-sDEVICE=pdfwrite -dCompatibilityLevel=1.4 -dPDFSETTINGS=/ebook " +
                    "-dNOPAUSE -dQUIET -dBATCH -sOutputFile=\"{0}\" \"{1}\"",
                    outFile, file);

                ProcessStartInfo psi = new ProcessStartInfo()
                {
                    FileName = gsPath,
                    Arguments = args,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                using (var proc = Process.Start(psi))
                {
                    proc.WaitForExit();
                }

                if (File.Exists(outFile))
                {
                    Process.Start(outFile);
                }

                progressBar.Value = i;
                Refresh();
            }

            Close();

            if (cancelled)
                MessageBox.Show("Compression cancelled by user.", "Cancelled");
            else
                MessageBox.Show("All files compressed successfully!", "Done");
        }

        [STAThread]
        public static void Main()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                Multiselect = true
            };

            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    Application.EnableVisualStyles();
                    Application.Run(new CompressorForm(ofd.FileNames, fbd.SelectedPath));
                }
            }
        }
    }
}
