﻿using Firebase.Database;
using Firebase.Database.Query;
using CSCResult.Models;
using CSCResult.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;


namespace CSCResult.Services.Implementations
{
    public class AdminService : IAdminService
    {
        FirebaseClient firebase = new FirebaseClient(CourseSettings.FireBaseDatabaseUrl, new FirebaseOptions
        {
            AuthTokenAsyncFactory = () => Task.FromResult(CourseSettings.FireBaseSecret)
        });

        public async Task<bool> UpdateScore(StudentCoursesModel studentCoursesModel)
        {
            if (!string.IsNullOrWhiteSpace(studentCoursesModel.Key))
            {
                try
                {
                    await firebase.Child(nameof(StudentCoursesModel)).Child(studentCoursesModel.Key).PutAsync(studentCoursesModel);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                var response = await firebase.Child(nameof(StudentCoursesModel)).PostAsync(studentCoursesModel);
                if (response.Key != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        public async Task<bool> DeleteCourse(string key)
        {
            try
            {
                await firebase.Child(nameof(StudentCoursesModel)).Child(key).DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<StudentCoursesModel>> GetAllCourses()
        {
            var matric_no = Preferences.Get("MatricNo", String.Empty);
            var admin_email = Preferences.Get("AdminEmail", String.Empty);
            return (await firebase.Child(nameof(StudentCoursesModel)).OnceAsync<StudentCoursesModel>()).Where(f => f.Object.AdminEmail == admin_email).Select(f => new StudentCoursesModel
            {
                AdminEmail = f.Object.AdminEmail,
                CourseCode = f.Object.CourseCode,
                CourseUnit = f.Object.CourseUnit,
                CourseDescription = f.Object.CourseDescription,
                Score = f.Object.Score,
                MatricNo = f.Object.MatricNo,
                Key = f.Key
            }).ToList();
        }
    }
}