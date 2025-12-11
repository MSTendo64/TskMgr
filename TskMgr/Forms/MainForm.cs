using TskMgr;
using Task = TskMgr.Task;
using TaskStatus = TskMgr.TaskStatus;

public partial class MainForm : Form
{
    private TaskManager taskManager;
    private ListView listViewTasks;
    private Button btnAddTask;
    private Button btnEditTask;
    private Button btnDeleteTask;
    private Button btnRefresh;
    private Button btnMarkCompleted;
    private Button btnSearch;
    private ComboBox cmbFilterPriority;
    private ComboBox cmbFilterStatus;
    private TextBox txtSearch;
    private Label lblFilter;
    private Label lblSearch;
    private Panel panelFilters;
    private Panel panelControls;
    private Panel panelSearch;
    private ContextMenuStrip contextMenu;

    public MainForm()
    {
        taskManager = new TaskManager();
        InitializeUI();
        LoadTasks();
    }

    private void InitializeUI()
    {
        // Настройка главной формы
        this.Text = "Менеджер задач";
        this.Size = new Size(950, 650);
        this.StartPosition = FormStartPosition.CenterScreen;

        // Создание контекстного меню
        contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("Редактировать", null, (s, e) => EditSelectedTask());
        contextMenu.Items.Add("Удалить", null, (s, e) => DeleteSelectedTask());
        contextMenu.Items.Add("Отметить как выполненную", null, (s, e) => MarkAsCompleted());

        // Панель поиска
        panelSearch = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            BackColor = Color.LightSteelBlue
        };

        lblSearch = new Label
        {
            Text = "Поиск:",
            Location = new Point(10, 15),
            AutoSize = true,
            Font = new Font("Arial", 9, FontStyle.Bold)
        };

        txtSearch = new TextBox
        {
            Location = new Point(60, 12),
            Width = 250,
            PlaceholderText = "Введите ключевые слова..."
        };
        txtSearch.KeyPress += (s, e) =>
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SearchTasks();
                e.Handled = true;
            }
        };

        btnSearch = new Button
        {
            Text = "Найти",
            Location = new Point(320, 11),
            Size = new Size(80, 25),
            BackColor = Color.SteelBlue,
            ForeColor = Color.White
        };
        btnSearch.Click += (s, e) => SearchTasks();

        var btnClearSearch = new Button
        {
            Text = "Очистить",
            Location = new Point(410, 11),
            Size = new Size(80, 25)
        };
        btnClearSearch.Click += (s, e) =>
        {
            txtSearch.Text = "";
            LoadTasks();
        };

        panelSearch.Controls.AddRange(new Control[] { lblSearch, txtSearch, btnSearch, btnClearSearch });

        // Панель фильтров
        panelFilters = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            BackColor = Color.LightGray
        };

        lblFilter = new Label
        {
            Text = "Фильтры:",
            Location = new Point(5, 15),
            AutoSize = true,
            Font = new Font("Arial", 9, FontStyle.Bold)
        };

        cmbFilterPriority = new ComboBox
        {
            Location = new Point(70, 12),
            Width = 120,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbFilterPriority.Items.AddRange(new object[] { "Все приоритеты", "Низкий", "Нормальный", "Высокий" });
        cmbFilterPriority.SelectedIndex = 0;
        cmbFilterPriority.SelectedIndexChanged += (s, e) => ApplyFilters();

        cmbFilterStatus = new ComboBox
        {
            Location = new Point(200, 12),
            Width = 120,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbFilterStatus.Items.AddRange(new object[] { "Все статусы", "Не начата", "В процессе", "Завершена" });
        cmbFilterStatus.SelectedIndex = 0;
        cmbFilterStatus.SelectedIndexChanged += (s, e) => ApplyFilters();

        panelFilters.Controls.AddRange(new Control[] { lblFilter, cmbFilterPriority, cmbFilterStatus });

        // Панель управления
        panelControls = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            BackColor = Color.WhiteSmoke
        };

        btnAddTask = new Button
        {
            Text = "➕ Добавить",
            Location = new Point(10, 10),
            Size = new Size(120, 30),
            BackColor = Color.LightGreen,
            Font = new Font("Arial", 9, FontStyle.Regular)
        };
        btnAddTask.Click += (s, e) => AddTask();

        btnEditTask = new Button
        {
            Text = "✏️ Редактировать",
            Location = new Point(140, 10),
            Size = new Size(120, 30),
            Font = new Font("Arial", 9, FontStyle.Regular)
        };
        btnEditTask.Click += (s, e) => EditSelectedTask();

        btnDeleteTask = new Button
        {
            Text = "🗑️ Удалить",
            Location = new Point(270, 10),
            Size = new Size(120, 30),
            BackColor = Color.LightCoral,
            Font = new Font("Arial", 9, FontStyle.Regular)
        };
        btnDeleteTask.Click += (s, e) => DeleteSelectedTask();

        btnMarkCompleted = new Button
        {
            Text = "✅ Выполнено",
            Location = new Point(400, 10),
            Size = new Size(120, 30),
            BackColor = Color.LightBlue,
            Font = new Font("Arial", 9, FontStyle.Regular)
        };
        btnMarkCompleted.Click += (s, e) => MarkAsCompleted();

        btnRefresh = new Button
        {
            Text = "🔄 Обновить",
            Location = new Point(530, 10),
            Size = new Size(120, 30),
            Font = new Font("Arial", 9, FontStyle.Regular)
        };
        btnRefresh.Click += (s, e) =>
        {
            txtSearch.Text = "";
            LoadTasks();
        };

        panelControls.Controls.AddRange(new Control[]
        {
            btnAddTask, btnEditTask, btnDeleteTask, btnMarkCompleted, btnRefresh
        });

        // ListView для отображения задач
        listViewTasks = new ListView
        {
            Dock = DockStyle.Fill,
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            MultiSelect = false,
            ContextMenuStrip = contextMenu
        };

        // Добавление колонок
        listViewTasks.Columns.Add("ID", 50);
        listViewTasks.Columns.Add("Название", 200);
        listViewTasks.Columns.Add("Описание", 300);
        listViewTasks.Columns.Add("Приоритет", 100);
        listViewTasks.Columns.Add("Статус", 100);
        listViewTasks.Columns.Add("Дата создания", 120);
        listViewTasks.Columns.Add("Дедлайн", 120);

        // Добавление элементов на форму
        this.Controls.Add(listViewTasks);
        this.Controls.Add(panelControls);
        this.Controls.Add(panelFilters);
        this.Controls.Add(panelSearch);
    }

    private void LoadTasks()
    {
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        listViewTasks.Items.Clear();

        var storage = taskManager.taskStorage;
        if (storage.tasks == null) return;

        string searchText = txtSearch.Text.Trim().ToLower();
        bool hasSearch = !string.IsNullOrWhiteSpace(searchText);
        var searchWords = hasSearch ? searchText.Split(' ', StringSplitOptions.RemoveEmptyEntries) : null;

        foreach (var kvp in storage.tasks)
        {
            var task = kvp.Value;

            // Применение поиска
            if (hasSearch && !TaskMatchesSearch(task, searchWords))
                continue;

            // Применение фильтров по приоритету
            if (cmbFilterPriority.SelectedIndex > 0)
            {
                string priorityText = GetPriorityText(task.Priority);
                if (priorityText != cmbFilterPriority.SelectedItem.ToString())
                    continue;
            }

            // Применение фильтров по статусу
            if (cmbFilterStatus.SelectedIndex > 0)
            {
                string statusText = GetStatusText(task.Status);
                if (statusText != cmbFilterStatus.SelectedItem.ToString())
                    continue;
            }

            // Добавление задачи в список
            AddTaskToListView(kvp.Key, task);
        }

        UpdateStatusBar();
    }

    private bool TaskMatchesSearch(Task task, string[] searchWords)
    {
        if (searchWords == null || searchWords.Length == 0)
            return true;

        // Проверяем каждое ключевое слово
        foreach (var word in searchWords)
        {
            bool wordFound =
                task.Name.ToLower().Contains(word) ||
                task.Description.ToLower().Contains(word) ||
                GetPriorityText(task.Priority).ToLower().Contains(word) ||
                GetStatusText(task.Status).ToLower().Contains(word) ||
                task.CreateDate.ToString("dd.MM.yyyy").Contains(word) ||
                (task.DeadLine.HasValue && task.DeadLine.Value.ToString("dd.MM.yyyy").Contains(word));

            // Если хотя бы одно слово не найдено - задача не подходит
            if (!wordFound)
                return false;
        }

        return true;
    }

    private void SearchTasks()
    {
        ApplyFilters();

        // Если есть поисковый запрос, показываем результат
        if (!string.IsNullOrWhiteSpace(txtSearch.Text))
        {
            int count = listViewTasks.Items.Count;
            MessageBox.Show($"Найдено задач: {count}", "Результаты поиска",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void AddTaskToListView(int id, Task task)
    {
        var item = new ListViewItem(id.ToString());
        item.SubItems.Add(task.Name);
        item.SubItems.Add(task.Description);
        item.SubItems.Add(GetPriorityText(task.Priority));
        item.SubItems.Add(GetStatusText(task.Status));
        item.SubItems.Add(task.CreateDate.ToString("dd.MM.yyyy"));
        item.SubItems.Add(task.DeadLine?.ToString("dd.MM.yyyy") ?? "Нет");

        // Выделение найденных слов в тексте
        if (!string.IsNullOrWhiteSpace(txtSearch.Text))
        {
            HighlightSearchTerms(item, txtSearch.Text.ToLower());
        }

        // Раскраска строк по приоритету
        switch (task.Priority)
        {
            case TaskPriority.High:
                item.BackColor = Color.FromArgb(255, 230, 230); // Светло-красный
                item.ForeColor = Color.DarkRed;
                break;
            case TaskPriority.Normal:
                item.BackColor = Color.FromArgb(255, 255, 230); // Светло-желтый
                item.ForeColor = Color.DarkGoldenrod;
                break;
            case TaskPriority.Low:
                item.BackColor = Color.FromArgb(230, 255, 230); // Светло-зеленый
                item.ForeColor = Color.DarkGreen;
                break;
        }

        // Выделение просроченных задач
        if (task.DeadLine.HasValue && task.DeadLine.Value < DateTime.Now && task.Status != TaskStatus.Completed)
        {
            item.BackColor = Color.FromArgb(255, 200, 200); // Красный для просроченных
            item.ForeColor = Color.DarkRed;
            item.ToolTipText = "ПРОСРОЧЕНО!";
        }

        listViewTasks.Items.Add(item);
    }

    private void HighlightSearchTerms(ListViewItem item, string searchText)
    {
        var searchWords = searchText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
        {
            string originalText = subItem.Text;
            string lowerText = originalText.ToLower();

            foreach (var word in searchWords)
            {
                if (lowerText.Contains(word))
                {
                    // Можно было бы сделать подсветку, но в ListView это сложно
                    // Вместо этого добавим tooltip
                    if (string.IsNullOrEmpty(item.ToolTipText))
                        item.ToolTipText = "Найдено по запросу: " + txtSearch.Text;
                    break;
                }
            }
        }
    }

    private void UpdateStatusBar()
    {
        int totalTasks = taskManager.taskStorage.tasks?.Count ?? 0;
        int displayedTasks = listViewTasks.Items.Count;

        string statusText;
        if (!string.IsNullOrWhiteSpace(txtSearch.Text))
        {
            statusText = $"Показано: {displayedTasks} из {totalTasks} задач (поиск: '{txtSearch.Text}')";
        }
        else
        {
            statusText = $"Всего задач: {totalTasks} | Показано: {displayedTasks}";
        }

        // Обновляем заголовок формы или добавляем статусбар
        this.Text = $"Менеджер задач - {statusText}";
    }

    // Остальные методы остаются без изменений...
    private string GetPriorityText(TaskPriority priority)
    {
        return priority switch
        {
            TaskPriority.Low => "Низкий",
            TaskPriority.Normal => "Нормальный",
            TaskPriority.High => "Высокий",
            _ => priority.ToString()
        };
    }

    private string GetStatusText(TaskStatus status)
    {
        return status switch
        {
            TaskStatus.NotStart => "Не начата",
            TaskStatus.Proccessing => "В процессе",
            TaskStatus.Completed => "Завершена",
            _ => status.ToString()
        };
    }

    private void AddTask()
    {
        using (var form = new TaskForm())
        {
            if (form.ShowDialog() == DialogResult.OK)
            {
                taskManager.AddTask(
                    form.TaskName,
                    form.TaskDescription,
                    form.TaskPriority,
                    form.TaskStatus,
                    form.Deadline
                );
                LoadTasks();
            }
        }
    }

    private void EditSelectedTask()
    {
        if (listViewTasks.SelectedItems.Count == 0)
        {
            MessageBox.Show("Выберите задачу для редактирования", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var selectedItem = listViewTasks.SelectedItems[0];
        int taskId = int.Parse(selectedItem.Text);
        var task = taskManager.GetTask(id: taskId);

        if (task != null)
        {
            using (var form = new TaskForm(task))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // Обновляем задачу
                    task.Name = form.TaskName;
                    task.Description = form.TaskDescription;
                    task.Priority = form.TaskPriority;
                    task.Status = form.TaskStatus;
                    task.DeadLine = form.Deadline;

                    // Сохраняем изменения
                    var storage = taskManager.taskStorage;
                    storage.UpdateTask(taskId, task);
                    storage.Save();
                    LoadTasks();
                }
            }
        }
    }

    private void DeleteSelectedTask()
    {
        if (listViewTasks.SelectedItems.Count == 0)
        {
            MessageBox.Show("Выберите задачу для удаления", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var result = MessageBox.Show("Вы уверены, что хотите удалить выбранную задачу?",
            "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            var selectedItem = listViewTasks.SelectedItems[0];
            int taskId = int.Parse(selectedItem.Text);
            taskManager.RemoveTask(taskId);
            LoadTasks();
        }
    }

    private void MarkAsCompleted()
    {
        if (listViewTasks.SelectedItems.Count == 0)
        {
            MessageBox.Show("Выберите задачу", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var selectedItem = listViewTasks.SelectedItems[0];
        int taskId = int.Parse(selectedItem.Text);
        var task = taskManager.GetTask(id: taskId);

        if (task != null && task.Status != TaskStatus.Completed)
        {
            task.Status = TaskStatus.Completed;
            var storage = taskManager.taskStorage;
            storage.UpdateTask(taskId, task);
            storage.Save();
            LoadTasks();
        }
    }
}