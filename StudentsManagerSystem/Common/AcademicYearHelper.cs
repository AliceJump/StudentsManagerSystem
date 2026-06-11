using System;

namespace StudentsManagerSystem.Common
{
    public static class AcademicYearHelper
    {
        public static string NormalizeStartYear(string academicYear)
        {
            if (string.IsNullOrWhiteSpace(academicYear))
            {
                return string.Empty;
            }

            academicYear = academicYear.Trim();
            return academicYear.Length >= 4 ? academicYear[..4] : academicYear;
        }

        public static string FormatDisplay(string academicYear)
        {
            var startYearText = NormalizeStartYear(academicYear);
            if (!int.TryParse(startYearText, out var startYear))
            {
                return academicYear;
            }

            return $"{startYear}-{startYear + 1}";
        }
    }
}
