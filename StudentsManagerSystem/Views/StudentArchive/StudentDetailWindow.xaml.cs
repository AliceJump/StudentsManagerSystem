using StudentsManagerSystem.Models;
using System.Windows;

namespace StudentsManagerSystem.Views.StudentArchive
{
    public partial class StudentDetailWindow : Window
    {
        public StudentDetailWindow(Student student)
        {
            InitializeComponent();
            DataContext = student;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
