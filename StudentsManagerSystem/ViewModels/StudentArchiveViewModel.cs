using System.Collections.ObjectModel;
using StudentsManagerSystem.Common;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.ViewModels
{
    /// <summary>
    /// 学生档案视图模型。
    /// </summary>
    public sealed class StudentArchiveViewModel : ViewModelBase
    {
        private string searchText = string.Empty;
        private Student? selectedStudent;

        public StudentArchiveViewModel()
        {
            Students = new ObservableCollection<Student>();
            FamilyInfos = new ObservableCollection<FamilyInfo>();
            Rewards = new ObservableCollection<RewardRecord>();
            Punishments = new ObservableCollection<PunishmentRecord>();
            HealthRecords = new ObservableCollection<HealthRecord>();

            RefreshCommand = new AsyncRelayCommand(LoadAsync);
        }

        public ObservableCollection<Student> Students { get; }

        public ObservableCollection<FamilyInfo> FamilyInfos { get; }

        public ObservableCollection<RewardRecord> Rewards { get; }

        public ObservableCollection<PunishmentRecord> Punishments { get; }

        public ObservableCollection<HealthRecord> HealthRecords { get; }

        public Student? SelectedStudent
        {
            get => selectedStudent;
            set => SetProperty(ref selectedStudent, value);
        }

        public string SearchText
        {
            get => searchText;
            set => SetProperty(ref searchText, value);
        }

        public AsyncRelayCommand RefreshCommand { get; }

        private Task LoadAsync()
        {
            return Task.CompletedTask;
        }
    }
}