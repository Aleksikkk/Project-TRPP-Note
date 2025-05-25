#nullable enable

using System;
using System.Drawing;
using System.Windows.Forms;

namespace CalendarApp
{
    public partial class NoteInputForm : Form
    {
        // Гарантируем компилятору, что созданы в InitializeComponent
        private TextBox textBoxNote = null!;
        private Button buttonOK = null!;
        private Button buttonCancel = null!;

        public string NoteText => textBoxNote.Text;

        public NoteInputForm()
        {
            InitializeComponent();
        }

        public void SetNoteText(string text)
        {
            textBoxNote.Text = text;
            textBoxNote.SelectionStart = text.Length;
            textBoxNote.Focus();
        }

        private void ButtonOK_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxNote.Text))
            {
                MessageBox.Show("Текст заметки не может быть пустым.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxNote.Focus();
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void InitializeComponent()
        {
            this.textBoxNote = new TextBox();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.SuspendLayout();
            // 
            // textBoxNote
            // 
            this.textBoxNote.Location = new Point(12, 12);
            this.textBoxNote.Multiline = true;
            this.textBoxNote.Name = "textBoxNote";
            this.textBoxNote.Size = new Size(360, 150);
            this.textBoxNote.TabIndex = 0;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new Point(216, 175);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new Size(75, 27);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "ОК";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new EventHandler(this.ButtonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new Point(297, 175);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new Size(75, 27);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Отмена";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new EventHandler(this.ButtonCancel_Click);
            // 
            // NoteInputForm
            // 
            this.ClientSize = new Size(384, 214);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textBoxNote);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Name = "NoteInputForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Введите текст заметки";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
