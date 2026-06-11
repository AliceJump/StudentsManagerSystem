using System;
using System.Text.RegularExpressions;

namespace StudentsManagerSystem.Common
{
    public static class InputValidator
    {
        // 学号格式：数字或字母，4-20位
        public static bool ValidateStudentNo(string studentNo)
        {
            return !string.IsNullOrWhiteSpace(studentNo) && 
                   Regex.IsMatch(studentNo, @"^[A-Za-z0-9]{4,20}$");
        }

        // 身份证：18位数字或X
        public static bool ValidateIdCard(string idCard)
        {
            return !string.IsNullOrWhiteSpace(idCard) && 
                   Regex.IsMatch(idCard, @"^[0-9]{17}[0-9X]$");
        }

        // 手机号：11位数字
        public static bool ValidatePhone(string phone)
        {
            return !string.IsNullOrWhiteSpace(phone) && 
                   Regex.IsMatch(phone, @"^1[0-9]{10}$");
        }

        // 邮箱
        public static bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return true;  // 邮箱可选
            return Regex.IsMatch(email, @"^[a-zA-Z0-9._%-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        }

        // 姓名：汉字或字母，2-50位
        public static bool ValidateName(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && 
                   Regex.IsMatch(name, @"^[\u4e00-\u9fa5a-zA-Z]{2,50}$");
        }

        public static bool ValidateAcademicYear(string academicYear)
        {
            return !string.IsNullOrWhiteSpace(academicYear) &&
                   Regex.IsMatch(academicYear, @"^\d{4}$");
        }

        public static bool ValidateSemester(string semester)
        {
            return semester == "1" || semester == "2" || semester == "第一学期" || semester == "第二学期";
        }

        public static bool ValidateMoney(string value)
        {
            return decimal.TryParse(value, out var amount) && amount >= 0;
        }
    }
}
