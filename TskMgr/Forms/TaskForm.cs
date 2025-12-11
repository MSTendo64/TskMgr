using System;
using System.Drawing;
using System.Windows.Forms;

namespace TskMgr
{
    public partial class TaskForm : Form
    {
        private TextBox txtName;
        private TextBox txtDescription;
        private ComboBox cmbPriority;
        private ComboBox cmbStatus;
        private DateTimePicker dtpDeadline;
        private CheckBox chkHasDeadline;
        private Button btnOK;
        private Button btnCancel;
        private Label lblName;
        private Label lblDescription;
        private Label lblPriority;
        private Label lblStatus;
        private Label lblDeadline;

        public string TaskName => txtName.Text;
        public string TaskDescription => txtDescription.Text;
        public TaskPriority TaskPriority => (TaskPriority)cmbPriority.SelectedIndex;
        public TaskStatus TaskStatus => (TaskStatus)cmbStatus.SelectedIndex;
        public DateTime? Deadline => chkHasDeadline.Checked ? dtpDeadline.Value : (DateTime?)null;

        public TaskForm()
        {
            InitializeComponent();
            this.Text = "Добавить задачу";
        }

        public TaskForm(Task task) : this()
        {
            this.Text = "Редактировать задачу";
            txtName.Text = task.Name;
            txtDescription.Text = task.Description;
            cmbPriority.SelectedIndex = (int)task.Priority;
            cmbStatus.SelectedIndex = (int)task.Status;

            if (task.DeadLine.HasValue)
            {
                chkHasDeadline.Checked = true;
                dtpDeadline.Value = task.DeadLine.Value;
                dtpDeadline.Enabled = true;
            }
        }

        private void InitializeComponent()
        {
            this.Size = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Название
            lblName = new Label
            {
                Text = "Название:",
                Location = new Point(20, 20),
                AutoSize = true
            };

            txtName = new TextBox
            {
                Location = new Point(120, 17),
                Size = new Size(250, 20)
            };

            // Описание
            lblDescription = new Label
            {
                Text = "Описание:",
                Location = new Point(20, 60),
                AutoSize = true
            };

            txtDescription = new TextBox
            {
                Location = new Point(120, 57),
                Size = new Size(250, 60),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            // Приоритет
            lblPriority = new Label
            {
                Text = "Приоритет:",
                Location = new Point(20, 130),
                AutoSize = true
            };

            cmbPriority = new ComboBox
            {
                Location = new Point(120, 127),
                Size = new Size(150, 21),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbPriority.Items.AddRange(new object[] { "Низкий", "Нормальный", "Высокий" });
            cmbPriority.SelectedIndex = 1;

            // Статус
            lblStatus = new Label
            {
                Text = "Статус:",
                Location = new Point(20, 170),
                AutoSize = true
            };

            cmbStatus = new ComboBox
            {
                Location = new Point(120, 167),
                Size = new Size(150, 21),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStatus.Items.AddRange(new object[] { "Не начата", "В процессе", "Завершена" });
            cmbStatus.SelectedIndex = 0;

            // Дедлайн
            lblDeadline = new Label
            {
                Text = "Дедлайн:",
                Location = new Point(20, 210),
                AutoSize = true
            };

            chkHasDeadline = new CheckBox
            {
                Text = "Установить дедлайн",
                Location = new Point(120, 207),
                AutoSize = true
            };
            chkHasDeadline.CheckedChanged += (s, e) =>
                dtpDeadline.Enabled = chkHasDeadline.Checked;

            dtpDeadline = new DateTimePicker
            {
                Location = new Point(120, 230),
                Size = new Size(150, 20),
                Format = DateTimePickerFormat.Short,
                Enabled = false,
                MinDate = DateTime.Now.AddDays(-1)
            };

            // Кнопки
            btnOK = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(200, 280),
                Size = new Size(80, 30)
            };
            btnOK.Click += (s, e) => ValidateInput();

            btnCancel = new Button
            {
                Text = "Отмена",
                DialogResult = DialogResult.Cancel,
                Location = new Point(290, 280),
                Size = new Size(80, 30)
            };

            // Добавление элементов на форму
            this.Controls.AddRange(new Control[]
            {
                lblName, txtName,
                lblDescription, txtDescription,
                lblPriority, cmbPriority,
                lblStatus, cmbStatus,
                lblDeadline, chkHasDeadline, dtpDeadline,
                btnOK, btnCancel
            });

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private void ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название задачи", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }
        }
    }
}