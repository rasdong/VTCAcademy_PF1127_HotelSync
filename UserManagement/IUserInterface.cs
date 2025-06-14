
using System;

namespace HotelManagementSystem
{
    public interface IUserInterface
    {
        string? ShowInitialScreen();
        (string?, string?) ShowLoginScreen();
        (string?, string?, string?, string?) ShowRegisterScreen();
        void ShowMainMenu(User currentUser, Action<string> onOptionSelected);
        void ShowInvoiceManagement();
        void ShowReportScreen();
        void ShowSuccessMessage(string message);
        void ShowErrorMessage(string message);
        void ShowInfoMessage(string message);
    }
}