using Microsoft.AspNetCore.Components;
using static ELearningApp.Components.Shared.ModalDialog;

namespace ELearningApp.Services
{
    public interface IModalService
    {
        Task ShowAlertAsync(string title, string message, ModalType type = ModalType.Info);
        Task<bool> ShowConfirmAsync(string title, string message, string confirmText = "Confirm", string cancelText = "Cancel");
        void ShowAlert(string title, string message, ModalType type = ModalType.Info);
        void ShowConfirm(string title, string message, Func<Task> onConfirm, string confirmText = "Confirm", string cancelText = "Cancel");
        
        event Action<ModalInfo>? OnShowModal;
        event Action? OnHideModal;
    }

    public class ModalService : IModalService
    {
        public event Action<ModalInfo>? OnShowModal;
        public event Action? OnHideModal;

        private TaskCompletionSource<bool>? _confirmTaskSource;

        public async Task ShowAlertAsync(string title, string message, ModalType type = ModalType.Info)
        {
            var taskSource = new TaskCompletionSource<bool>();
            
            var modalInfo = new ModalInfo
            {
                Title = title,
                Message = message,
                Type = type,
                IsConfirm = false,
                OnClosed = async () => 
                {
                    await Task.CompletedTask;
                    taskSource.SetResult(true);
                }
            };

            OnShowModal?.Invoke(modalInfo);
            await taskSource.Task;
        }

        public async Task<bool> ShowConfirmAsync(string title, string message, string confirmText = "Confirm", string cancelText = "Cancel")
        {
            _confirmTaskSource = new TaskCompletionSource<bool>();
            
            var modalInfo = new ModalInfo
            {
                Title = title,
                Message = message,
                Type = ModalType.Confirm,
                IsConfirm = true,
                ConfirmText = confirmText,
                CancelText = cancelText,
                OnConfirmed = async () => 
                {
                    await Task.CompletedTask;
                    _confirmTaskSource.SetResult(true);
                },
                OnCancelled = async () => 
                {
                    await Task.CompletedTask;
                    _confirmTaskSource.SetResult(false);
                }
            };

            OnShowModal?.Invoke(modalInfo);
            return await _confirmTaskSource.Task;
        }

        public void ShowAlert(string title, string message, ModalType type = ModalType.Info)
        {
            var modalInfo = new ModalInfo
            {
                Title = title,
                Message = message,
                Type = type,
                IsConfirm = false
            };

            OnShowModal?.Invoke(modalInfo);
        }

        public void ShowConfirm(string title, string message, Func<Task> onConfirm, string confirmText = "Confirm", string cancelText = "Cancel")
        {
            var modalInfo = new ModalInfo
            {
                Title = title,
                Message = message,
                Type = ModalType.Confirm,
                IsConfirm = true,
                ConfirmText = confirmText,
                CancelText = cancelText,
                OnConfirmed = onConfirm
            };

            OnShowModal?.Invoke(modalInfo);
        }
    }

    public class ModalInfo
    {
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public ModalType Type { get; set; } = ModalType.Info;
        public bool IsConfirm { get; set; }
        public string ConfirmText { get; set; } = "Confirm";
        public string CancelText { get; set; } = "Cancel";
        public Func<Task>? OnConfirmed { get; set; }
        public Func<Task>? OnCancelled { get; set; }
        public Func<Task>? OnClosed { get; set; }
    }
}