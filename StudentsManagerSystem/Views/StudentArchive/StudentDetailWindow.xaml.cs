using System.Windows;

namespace StudentsManagerSystem.Views.StudentArchive
{
    public partial class StudentDetailWindow : Window
    {
        public StudentDetailWindow()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
