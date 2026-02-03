using EduPortal.Data;
using EduPortal.Models;
using System.Data;

namespace EduPortal
{
    public partial class MainForm : Form
    {
        private DatabaseManager _dbManager;
        private DataGridView dataGridViewCourses;
        private DataGridView dataGridViewStudents;
        private DataGridView dataGridViewTeachers;
        private TextBox textBoxCourseName, textBoxDuration, textBoxCredits, textBoxSemester, textBoxDescription;
        private TextBox textBoxStudentFirstName, textBoxStudentLastName, textBoxStudentEmail, textBoxStudentGroup;
        private TextBox textBoxTeacherFirstName, textBoxTeacherLastName, textBoxTeacherSubject, textBoxTeacherEmail;
        private TextBox textBoxSearch;
        private ComboBox comboBoxTeacher;
        private Button buttonAddCourse, buttonEditCourse, buttonDeleteCourse, buttonSearchCourse;
        private Button buttonAddStudent, buttonEditStudent, buttonDeleteStudent;
        private Button buttonAddTeacher, buttonEditTeacher, buttonDeleteTeacher;

        public MainForm()
        {
            InitializeComponent();
            _dbManager = new DatabaseManager();
            
            // Проверяем подключение к базе данных
            if (!_dbManager.TestConnection())
            {
                MessageBox.Show("Не удалось подключиться к базе данных PostgreSQL.\n\nПроверьте:\n1. Запущен ли сервер PostgreSQL\n2. Создана ли база данных EduPortal\n3. Правильность строки подключения в DatabaseManager", 
                    "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                LoadAllData();
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Образовательный портал";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1000, 600);

            // Создание TabControl
            var tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            this.Controls.Add(tabControl);

            // Создаем вкладки
            CreateCoursesTab(tabControl);
            CreateStudentsTab(tabControl);
            CreateTeachersTab(tabControl);
        }

        private void CreateCoursesTab(TabControl tabControl)
        {
            TabPage coursesTab = new TabPage("Курсы");
            
            // DataGridView для курсов
            dataGridViewCourses = new DataGridView();
            dataGridViewCourses.Location = new Point(10, 10);
            dataGridViewCourses.Size = new Size(750, 300);
            dataGridViewCourses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCourses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewCourses.MultiSelect = false;
            dataGridViewCourses.ReadOnly = true;
            dataGridViewCourses.SelectionChanged += DataGridViewCourses_SelectionChanged;
            
            // Поля для ввода данных курса
            var lblCourseName = new Label() { Text = "Название курса:", Location = new Point(10, 330), Size = new Size(100, 20) };
            textBoxCourseName = new TextBox() { Location = new Point(120, 330), Size = new Size(200, 20) };
            
            var lblDuration = new Label() { Text = "Длительность:", Location = new Point(10, 360), Size = new Size(100, 20) };
            textBoxDuration = new TextBox() { Location = new Point(120, 360), Size = new Size(100, 20) };
            
            var lblCredits = new Label() { Text = "Кредиты:", Location = new Point(10, 390), Size = new Size(100, 20) };
            textBoxCredits = new TextBox() { Location = new Point(120, 390), Size = new Size(100, 20) };
            
            var lblSemester = new Label() { Text = "Семестр:", Location = new Point(10, 420), Size = new Size(100, 20) };
            textBoxSemester = new TextBox() { Location = new Point(120, 420), Size = new Size(100, 20) };
            
            var lblTeacher = new Label() { Text = "Преподаватель:", Location = new Point(350, 330), Size = new Size(100, 20) };
            comboBoxTeacher = new ComboBox() { Location = new Point(460, 330), Size = new Size(200, 20), DropDownStyle = ComboBoxStyle.DropDownList };
            
            var lblDescription = new Label() { Text = "Описание:", Location = new Point(350, 360), Size = new Size(100, 20) };
            textBoxDescription = new TextBox() { Location = new Point(460, 360), Size = new Size(200, 60), Multiline = true };
            
            // Кнопки управления курсами
            buttonAddCourse = new Button() { Text = "Добавить", Location = new Point(10, 460), Size = new Size(80, 30) };
            buttonAddCourse.Click += ButtonAddCourse_Click;
            
            buttonEditCourse = new Button() { Text = "Редактировать", Location = new Point(100, 460), Size = new Size(100, 30) };
            buttonEditCourse.Click += ButtonEditCourse_Click;
            
            buttonDeleteCourse = new Button() { Text = "Удалить", Location = new Point(210, 460), Size = new Size(80, 30) };
            buttonDeleteCourse.Click += ButtonDeleteCourse_Click;
            
            // Поиск курсов
            var lblSearch = new Label() { Text = "Поиск:", Location = new Point(350, 460), Size = new Size(50, 20) };
            textBoxSearch = new TextBox() { Location = new Point(410, 460), Size = new Size(150, 20) };
            buttonSearchCourse = new Button() { Text = "Найти", Location = new Point(570, 458), Size = new Size(60, 25) };
            buttonSearchCourse.Click += ButtonSearchCourse_Click;
            
            // Добавляем элементы на вкладку
            coursesTab.Controls.AddRange(new Control[] {
                dataGridViewCourses, lblCourseName, textBoxCourseName, lblDuration, textBoxDuration,
                lblCredits, textBoxCredits, lblSemester, textBoxSemester, lblTeacher, comboBoxTeacher,
                lblDescription, textBoxDescription, buttonAddCourse, buttonEditCourse, buttonDeleteCourse,
                lblSearch, textBoxSearch, buttonSearchCourse
            });
            
            tabControl.TabPages.Add(coursesTab);
        }

        private void CreateStudentsTab(TabControl tabControl)
        {
            TabPage studentsTab = new TabPage("Студенты");
            
            // DataGridView для студентов
            dataGridViewStudents = new DataGridView();
            dataGridViewStudents.Location = new Point(10, 10);
            dataGridViewStudents.Size = new Size(750, 300);
            dataGridViewStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewStudents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewStudents.MultiSelect = false;
            dataGridViewStudents.ReadOnly = true;
            dataGridViewStudents.SelectionChanged += DataGridViewStudents_SelectionChanged;
            
            // Поля для ввода данных студента
            var lblFirstName = new Label() { Text = "Имя:", Location = new Point(10, 330), Size = new Size(80, 20) };
            textBoxStudentFirstName = new TextBox() { Location = new Point(100, 330), Size = new Size(150, 20) };
            
            var lblLastName = new Label() { Text = "Фамилия:", Location = new Point(10, 360), Size = new Size(80, 20) };
            textBoxStudentLastName = new TextBox() { Location = new Point(100, 360), Size = new Size(150, 20) };
            
            var lblEmail = new Label() { Text = "Email:", Location = new Point(10, 390), Size = new Size(80, 20) };
            textBoxStudentEmail = new TextBox() { Location = new Point(100, 390), Size = new Size(200, 20) };
            
            var lblGroup = new Label() { Text = "Группа:", Location = new Point(10, 420), Size = new Size(80, 20) };
            textBoxStudentGroup = new TextBox() { Location = new Point(100, 420), Size = new Size(100, 20) };
            
            // Кнопки управления студентами
            buttonAddStudent = new Button() { Text = "Добавить", Location = new Point(10, 460), Size = new Size(80, 30) };
            buttonAddStudent.Click += ButtonAddStudent_Click;
            
            buttonEditStudent = new Button() { Text = "Редактировать", Location = new Point(100, 460), Size = new Size(100, 30) };
            buttonEditStudent.Click += ButtonEditStudent_Click;
            
            buttonDeleteStudent = new Button() { Text = "Удалить", Location = new Point(210, 460), Size = new Size(80, 30) };
            buttonDeleteStudent.Click += ButtonDeleteStudent_Click;
            
            // Добавляем элементы на вкладку
            studentsTab.Controls.AddRange(new Control[] {
                dataGridViewStudents, lblFirstName, textBoxStudentFirstName, lblLastName, textBoxStudentLastName,
                lblEmail, textBoxStudentEmail, lblGroup, textBoxStudentGroup,
                buttonAddStudent, buttonEditStudent, buttonDeleteStudent
            });
            
            tabControl.TabPages.Add(studentsTab);
        }

        private void CreateTeachersTab(TabControl tabControl)
        {
            TabPage teachersTab = new TabPage("Преподаватели");
            
            // DataGridView для преподавателей
            dataGridViewTeachers = new DataGridView();
            dataGridViewTeachers.Location = new Point(10, 10);
            dataGridViewTeachers.Size = new Size(750, 300);
            dataGridViewTeachers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewTeachers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewTeachers.MultiSelect = false;
            dataGridViewTeachers.ReadOnly = true;
            dataGridViewTeachers.SelectionChanged += DataGridViewTeachers_SelectionChanged;
            
            // Поля для ввода данных преподавателя
            var lblFirstName = new Label() { Text = "Имя:", Location = new Point(10, 330), Size = new Size(80, 20) };
            textBoxTeacherFirstName = new TextBox() { Location = new Point(100, 330), Size = new Size(150, 20) };
            
            var lblLastName = new Label() { Text = "Фамилия:", Location = new Point(10, 360), Size = new Size(80, 20) };
            textBoxTeacherLastName = new TextBox() { Location = new Point(100, 360), Size = new Size(150, 20) };
            
            var lblSubject = new Label() { Text = "Предмет:", Location = new Point(10, 390), Size = new Size(80, 20) };
            textBoxTeacherSubject = new TextBox() { Location = new Point(100, 390), Size = new Size(200, 20) };
            
            var lblEmail = new Label() { Text = "Email:", Location = new Point(10, 420), Size = new Size(80, 20) };
            textBoxTeacherEmail = new TextBox() { Location = new Point(100, 420), Size = new Size(200, 20) };
            
            // Кнопки управления преподавателями
            buttonAddTeacher = new Button() { Text = "Добавить", Location = new Point(10, 460), Size = new Size(80, 30) };
            buttonAddTeacher.Click += ButtonAddTeacher_Click;
            
            buttonEditTeacher = new Button() { Text = "Редактировать", Location = new Point(100, 460), Size = new Size(100, 30) };
            buttonEditTeacher.Click += ButtonEditTeacher_Click;
            
            buttonDeleteTeacher = new Button() { Text = "Удалить", Location = new Point(210, 460), Size = new Size(80, 30) };
            buttonDeleteTeacher.Click += ButtonDeleteTeacher_Click;
            
            // Добавляем элементы на вкладку
            teachersTab.Controls.AddRange(new Control[] {
                dataGridViewTeachers, lblFirstName, textBoxTeacherFirstName, lblLastName, textBoxTeacherLastName,
                lblSubject, textBoxTeacherSubject, lblEmail, textBoxTeacherEmail,
                buttonAddTeacher, buttonEditTeacher, buttonDeleteTeacher
            });
            
            tabControl.TabPages.Add(teachersTab);
        } 
       // Методы загрузки данных
        private void LoadAllData()
        {
            LoadCourses();
            LoadStudents();
            LoadTeachers();
            LoadTeachersComboBox();
        }

        private void LoadCourses()
        {
            try
            {
                var courses = _dbManager.GetAllCourses();
                dataGridViewCourses.DataSource = courses;
                
                if (dataGridViewCourses.Columns.Count > 0)
                {
                    dataGridViewCourses.Columns["CourseId"].HeaderText = "ID";
                    dataGridViewCourses.Columns["CourseName"].HeaderText = "Название курса";
                    dataGridViewCourses.Columns["Duration"].HeaderText = "Длительность";
                    dataGridViewCourses.Columns["Credits"].HeaderText = "Кредиты";
                    dataGridViewCourses.Columns["Semester"].HeaderText = "Семестр";
                    dataGridViewCourses.Columns["Description"].HeaderText = "Описание";
                    dataGridViewCourses.Columns["TeacherName"].HeaderText = "Преподаватель";
                    if (dataGridViewCourses.Columns["TeacherId"] != null)
                        dataGridViewCourses.Columns["TeacherId"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки курсов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadStudents()
        {
            try
            {
                var students = _dbManager.GetAllStudents();
                dataGridViewStudents.DataSource = students;
                
                if (dataGridViewStudents.Columns.Count > 0)
                {
                    dataGridViewStudents.Columns["StudentId"].HeaderText = "ID";
                    dataGridViewStudents.Columns["FirstName"].HeaderText = "Имя";
                    dataGridViewStudents.Columns["LastName"].HeaderText = "Фамилия";
                    dataGridViewStudents.Columns["Email"].HeaderText = "Email";
                    dataGridViewStudents.Columns["GroupName"].HeaderText = "Группа";
                    dataGridViewStudents.Columns["EnrollmentDate"].HeaderText = "Дата поступления";
                    if (dataGridViewStudents.Columns["CourseId"] != null)
                        dataGridViewStudents.Columns["CourseId"].Visible = false;
                    if (dataGridViewStudents.Columns["FullName"] != null)
                        dataGridViewStudents.Columns["FullName"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки студентов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTeachers()
        {
            try
            {
                var teachers = _dbManager.GetAllTeachers();
                dataGridViewTeachers.DataSource = teachers;
                
                if (dataGridViewTeachers.Columns.Count > 0)
                {
                    dataGridViewTeachers.Columns["TeacherId"].HeaderText = "ID";
                    dataGridViewTeachers.Columns["FirstName"].HeaderText = "Имя";
                    dataGridViewTeachers.Columns["LastName"].HeaderText = "Фамилия";
                    dataGridViewTeachers.Columns["Subject"].HeaderText = "Предмет";
                    dataGridViewTeachers.Columns["Email"].HeaderText = "Email";
                    if (dataGridViewTeachers.Columns["Phone"] != null)
                        dataGridViewTeachers.Columns["Phone"].HeaderText = "Телефон";
                    if (dataGridViewTeachers.Columns["FullName"] != null)
                        dataGridViewTeachers.Columns["FullName"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки преподавателей: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTeachersComboBox()
        {
            try
            {
                var teachers = _dbManager.GetAllTeachers();
                comboBoxTeacher.DataSource = teachers;
                comboBoxTeacher.DisplayMember = "FullName";
                comboBoxTeacher.ValueMember = "TeacherId";
                comboBoxTeacher.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки преподавателей: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчики событий для курсов
        private void DataGridViewCourses_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewCourses.SelectedRows.Count > 0)
            {
                var row = dataGridViewCourses.SelectedRows[0];
                textBoxCourseName.Text = row.Cells["CourseName"].Value?.ToString() ?? "";
                textBoxDuration.Text = row.Cells["Duration"].Value?.ToString() ?? "";
                textBoxCredits.Text = row.Cells["Credits"].Value?.ToString() ?? "";
                textBoxSemester.Text = row.Cells["Semester"].Value?.ToString() ?? "";
                textBoxDescription.Text = row.Cells["Description"].Value?.ToString() ?? "";
                
                if (row.Cells["TeacherId"].Value != null)
                {
                    comboBoxTeacher.SelectedValue = row.Cells["TeacherId"].Value;
                }
            }
        }

        private void ButtonAddCourse_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBoxCourseName.Text))
                {
                    MessageBox.Show("Введите название курса", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (comboBoxTeacher.SelectedValue == null)
                {
                    MessageBox.Show("Выберите преподавателя", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var course = new Course
                {
                    CourseName = textBoxCourseName.Text.Trim(),
                    Duration = textBoxDuration.Text.Trim(),
                    Credits = string.IsNullOrWhiteSpace(textBoxCredits.Text) ? 3 : int.Parse(textBoxCredits.Text),
                    Semester = string.IsNullOrWhiteSpace(textBoxSemester.Text) ? 1 : int.Parse(textBoxSemester.Text),
                    TeacherId = (int)comboBoxTeacher.SelectedValue,
                    Description = textBoxDescription.Text.Trim()
                };

                _dbManager.AddCourse(course);
                MessageBox.Show("Курс успешно добавлен", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                ClearCourseFields();
                LoadCourses();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления курса: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonEditCourse_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewCourses.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Выберите курс для редактирования", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(textBoxCourseName.Text))
                {
                    MessageBox.Show("Введите название курса", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedRow = dataGridViewCourses.SelectedRows[0];
                int courseId = (int)selectedRow.Cells["CourseId"].Value;

                var course = new Course
                {
                    CourseId = courseId,
                    CourseName = textBoxCourseName.Text.Trim(),
                    Duration = textBoxDuration.Text.Trim(),
                    Credits = string.IsNullOrWhiteSpace(textBoxCredits.Text) ? 3 : int.Parse(textBoxCredits.Text),
                    Semester = string.IsNullOrWhiteSpace(textBoxSemester.Text) ? 1 : int.Parse(textBoxSemester.Text),
                    TeacherId = (int)comboBoxTeacher.SelectedValue,
                    Description = textBoxDescription.Text.Trim()
                };

                _dbManager.UpdateCourse(course);
                MessageBox.Show("Курс успешно обновлен", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                ClearCourseFields();
                LoadCourses();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления курса: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonDeleteCourse_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewCourses.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Выберите курс для удаления", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show("Вы уверены, что хотите удалить выбранный курс?", 
                    "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    var selectedRow = dataGridViewCourses.SelectedRows[0];
                    int courseId = (int)selectedRow.Cells["CourseId"].Value;

                    _dbManager.DeleteCourse(courseId);
                    MessageBox.Show("Курс успешно удален", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    ClearCourseFields();
                    LoadCourses();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления курса: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonSearchCourse_Click(object sender, EventArgs e)
        {
            try
            {
                string searchTerm = textBoxSearch.Text.Trim();
                if (string.IsNullOrEmpty(searchTerm))
                {
                    LoadCourses();
                    return;
                }

                var courses = _dbManager.SearchCourses(searchTerm);
                dataGridViewCourses.DataSource = courses;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearCourseFields()
        {
            textBoxCourseName.Clear();
            textBoxDuration.Clear();
            textBoxCredits.Clear();
            textBoxSemester.Clear();
            textBoxDescription.Clear();
            comboBoxTeacher.SelectedIndex = -1;
        }

        // Обработчики событий для студентов
        private void DataGridViewStudents_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewStudents.SelectedRows.Count > 0)
            {
                var row = dataGridViewStudents.SelectedRows[0];
                textBoxStudentFirstName.Text = row.Cells["FirstName"].Value?.ToString() ?? "";
                textBoxStudentLastName.Text = row.Cells["LastName"].Value?.ToString() ?? "";
                textBoxStudentEmail.Text = row.Cells["Email"].Value?.ToString() ?? "";
                textBoxStudentGroup.Text = row.Cells["GroupName"].Value?.ToString() ?? "";
            }
        }

        private void ButtonAddStudent_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBoxStudentFirstName.Text) || 
                    string.IsNullOrWhiteSpace(textBoxStudentLastName.Text))
                {
                    MessageBox.Show("Введите имя и фамилию студента", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var student = new Student
                {
                    FirstName = textBoxStudentFirstName.Text.Trim(),
                    LastName = textBoxStudentLastName.Text.Trim(),
                    Email = textBoxStudentEmail.Text.Trim(),
                    GroupName = textBoxStudentGroup.Text.Trim(),
                    EnrollmentDate = DateTime.Now
                };

                _dbManager.AddStudent(student);
                MessageBox.Show("Студент успешно добавлен", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                ClearStudentFields();
                LoadStudents();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления студента: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonEditStudent_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewStudents.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Выберите студента для редактирования", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedRow = dataGridViewStudents.SelectedRows[0];
                int studentId = (int)selectedRow.Cells["StudentId"].Value;

                var student = new Student
                {
                    StudentId = studentId,
                    FirstName = textBoxStudentFirstName.Text.Trim(),
                    LastName = textBoxStudentLastName.Text.Trim(),
                    Email = textBoxStudentEmail.Text.Trim(),
                    GroupName = textBoxStudentGroup.Text.Trim()
                };

                _dbManager.UpdateStudent(student);
                MessageBox.Show("Студент успешно обновлен", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                ClearStudentFields();
                LoadStudents();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления студента: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonDeleteStudent_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewStudents.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Выберите студента для удаления", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show("Вы уверены, что хотите удалить выбранного студента?", 
                    "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    var selectedRow = dataGridViewStudents.SelectedRows[0];
                    int studentId = (int)selectedRow.Cells["StudentId"].Value;

                    _dbManager.DeleteStudent(studentId);
                    MessageBox.Show("Студент успешно удален", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    ClearStudentFields();
                    LoadStudents();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления студента: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearStudentFields()
        {
            textBoxStudentFirstName.Clear();
            textBoxStudentLastName.Clear();
            textBoxStudentEmail.Clear();
            textBoxStudentGroup.Clear();
        }

        // Обработчики событий для преподавателей
        private void DataGridViewTeachers_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewTeachers.SelectedRows.Count > 0)
            {
                var row = dataGridViewTeachers.SelectedRows[0];
                textBoxTeacherFirstName.Text = row.Cells["FirstName"].Value?.ToString() ?? "";
                textBoxTeacherLastName.Text = row.Cells["LastName"].Value?.ToString() ?? "";
                textBoxTeacherSubject.Text = row.Cells["Subject"].Value?.ToString() ?? "";
                textBoxTeacherEmail.Text = row.Cells["Email"].Value?.ToString() ?? "";
            }
        }

        private void ButtonAddTeacher_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBoxTeacherFirstName.Text) || 
                    string.IsNullOrWhiteSpace(textBoxTeacherLastName.Text))
                {
                    MessageBox.Show("Введите имя и фамилию преподавателя", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var teacher = new Teacher
                {
                    FirstName = textBoxTeacherFirstName.Text.Trim(),
                    LastName = textBoxTeacherLastName.Text.Trim(),
                    Subject = textBoxTeacherSubject.Text.Trim(),
                    Email = textBoxTeacherEmail.Text.Trim()
                };

                _dbManager.AddTeacher(teacher);
                MessageBox.Show("Преподаватель успешно добавлен", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                ClearTeacherFields();
                LoadTeachers();
                LoadTeachersComboBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления преподавателя: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonEditTeacher_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewTeachers.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Выберите преподавателя для редактирования", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedRow = dataGridViewTeachers.SelectedRows[0];
                int teacherId = (int)selectedRow.Cells["TeacherId"].Value;

                var teacher = new Teacher
                {
                    TeacherId = teacherId,
                    FirstName = textBoxTeacherFirstName.Text.Trim(),
                    LastName = textBoxTeacherLastName.Text.Trim(),
                    Subject = textBoxTeacherSubject.Text.Trim(),
                    Email = textBoxTeacherEmail.Text.Trim()
                };

                _dbManager.UpdateTeacher(teacher);
                MessageBox.Show("Преподаватель успешно обновлен", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                ClearTeacherFields();
                LoadTeachers();
                LoadTeachersComboBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления преподавателя: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonDeleteTeacher_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewTeachers.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Выберите преподавателя для удаления", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show("Вы уверены, что хотите удалить выбранного преподавателя?", 
                    "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    var selectedRow = dataGridViewTeachers.SelectedRows[0];
                    int teacherId = (int)selectedRow.Cells["TeacherId"].Value;

                    _dbManager.DeleteTeacher(teacherId);
                    MessageBox.Show("Преподаватель успешно удален", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    ClearTeacherFields();
                    LoadTeachers();
                    LoadTeachersComboBox();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления преподавателя: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearTeacherFields()
        {
            textBoxTeacherFirstName.Clear();
            textBoxTeacherLastName.Clear();
            textBoxTeacherSubject.Clear();
            textBoxTeacherEmail.Clear();
        }
    }
}