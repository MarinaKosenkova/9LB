using Desktop.TaskFolder;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Desktop.Repository
{
    public class TaskRepository
    {
        public delegate void TaskItemsChangedDel();
        public static event TaskItemsChangedDel? TaskItemsChanged;

        private static readonly ApiClient apiClient = new ApiClient("http://45.144.64.179");
        private static List<TaskDictionary> guestTasks = LocalStorage.LoadTasks();

        public static async Task<List<TaskDictionary>> GetTaskReposesAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return guestTasks;
            }
            return await apiClient.GetAsync<List<TaskDictionary>>($"/api/tasks?userId={userId}");
        }

        public static async Task AddTaskDictionaryAsync(TaskDictionary dict)
        {
            try
            {
                if (dict.UserId == Guid.Empty)
                {
                    guestTasks.Add(dict);
                    LocalStorage.SaveTasks(guestTasks);
                    RefreshTaskItems();
                }
                else
                {
                    var response = await apiClient.PostAsync<TaskDictionary>("/api/tasks", dict);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException($"Error: {response.StatusCode}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"API request failed: {ex.Message}");
                guestTasks.Add(dict);
                LocalStorage.SaveTasks(guestTasks);
                RefreshTaskItems();
            }
        }

        public static async Task RemoveTaskDictionaryAsync(TaskDictionary dict)
        {
            try
            {
                if (dict.UserId == Guid.Empty)
                {
                    guestTasks.Remove(dict);
                    LocalStorage.SaveTasks(guestTasks);
                    RefreshTaskItems();
                }
                else
                {
                    var response = await apiClient.DeleteAsync($"/api/tasks/{dict.Id}");
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException($"Error: {response.StatusCode}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"API request failed: {ex.Message}");
                guestTasks.Remove(dict);
                LocalStorage.SaveTasks(guestTasks);
                RefreshTaskItems();
            }
        }

        public static async Task UpdateTaskDictionaryAsync(TaskDictionary dict)
        {
            try
            {
                if (dict.UserId == Guid.Empty)
                {
                    var task = guestTasks.FirstOrDefault(t => t.Id == dict.Id);
                    if (task != null)
                    {
                        task.Name = dict.Name;
                        task.Description = dict.Description;
                        task.Date = dict.Date;
                        task.Time = dict.Time;
                        task.IsCompleted = dict.IsCompleted;
                        task.Category = dict.Category;
                        LocalStorage.SaveTasks(guestTasks);
                        RefreshTaskItems();
                    }
                }
                else
                {
                    var response = await apiClient.PutAsync<TaskDictionary>($"/api/tasks/{dict.Id}", dict);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException($"Error: {response.StatusCode}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"API request failed: {ex.Message}");
                var task = guestTasks.FirstOrDefault(t => t.Id == dict.Id);
                if (task != null)
                {
                    task.Name = dict.Name;
                    task.Description = dict.Description;
                    task.Date = dict.Date;
                    task.Time = dict.Time;
                    task.IsCompleted = dict.IsCompleted;
                    task.Category = dict.Category;
                    LocalStorage.SaveTasks(guestTasks);
                    RefreshTaskItems();
                }
            }
        }

        public static void RefreshTaskItems()
        {
            TaskItemsChanged?.Invoke();
        }
    }
}