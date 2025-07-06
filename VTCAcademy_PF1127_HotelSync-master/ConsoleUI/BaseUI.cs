using System;
using System.Runtime.InteropServices;

namespace HotelManagementSystem
{
    public abstract class BaseUI
    {
        protected int width;
        protected int height;
        protected int x;
        protected int y;
        protected string? currentUsername;
        protected string? currentRole;
        protected int? currentUserId;

        public BaseUI(string? username, string? role, int? userId)
        {
            currentUsername = username;
            currentRole = role;
            currentUserId = userId;
        }

        public void DrawHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.SetCursorPosition((Console.WindowWidth - title.Length - 4) / 2, 1);
            Console.Write("═══ " + title + " ═══");

            string[] hotelSyncArt = new[]
            {
                "  ██╗  ██╗ ██████╗ ████████╗███████╗██╗     ███████╗██╗   ██╗███╗   ██╗ ██████╗ ",
                "  ██║  ██║██╔═══██╗╚══██╔══╝██╔════╝██║     ██╔════╝╚██╗ ██╔╝████╗  ██║██╔════╝ ",
                "  ███████║██║   ██║   ██║   █████╗  ██║     ███████╗ ╚████╔╝ ██╔██╗ ██║██║     ",
                "  ██╔══██║██║   ██║   ██║   ██╔══╝  ██║     ╚════██║  ╚██╔╝  ██║╚██╗██║██║     ",
                "  ██║  ██║╚██████╔╝   ██║   ███████╗███████╗███████║   ██║   ██║ ╚████║╚██████╗",
                "  ╚═╝  ╚═╝ ╚═════╝    ╚═╝   ╚══════╝╚══════╝╚══════╝   ╚═╝   ╚═╝  ╚═══╝ ╚═════╝ "
            };

            int artWidth = hotelSyncArt[0].Length;
            int startX = (Console.WindowWidth - artWidth) / 2;
            int startY = 3;

            for (int i = 0; i < hotelSyncArt.Length; i++)
            {
                Console.SetCursorPosition(startX, startY + i);
                Console.Write(hotelSyncArt[i]);
            }

            Console.ResetColor();
        }

        public void SetupBox(int boxWidth, int boxHeight)
        {
            width = Math.Min(boxWidth, Console.WindowWidth - 2);
            height = boxHeight;
            x = Math.Max(0, (Console.WindowWidth - width) / 2);
            y = Math.Max(0, (Console.WindowHeight - height) / 2);

            DrawBox();
        }

        private void DrawBox()
        {
            if (Console.WindowWidth < width || Console.WindowHeight < height)
            {
                Console.Clear();
                Console.WriteLine("Lỗi: Cửa sổ console quá nhỏ! Vui lòng mở rộng cửa sổ.");
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey();
                Console.Clear();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.SetCursorPosition(x, y);
            Console.Write("┌" + new string('─', width) + "┐");
            for (int i = 1; i < height - 1; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write("│" + new string(' ', width) + "│");
            }
            Console.SetCursorPosition(x, y + height - 1);
            Console.Write("└" + new string('─', width) + "┘");
            Console.ResetColor();
        }

        protected string? ReadInputWithEscape(int inputX, int inputY)
        {
            Console.SetCursorPosition(inputX, inputY);
            string input = string.Empty;
            int maxLength = 20;
            Console.Write(new string(' ', maxLength));
            Console.SetCursorPosition(inputX, inputY);

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    if (keyInfo.Key == ConsoleKey.Escape)
                        return null;
                    else if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine();
                        return input;
                    }
                    else if (keyInfo.Key == ConsoleKey.Backspace && input.Length > 0)
                    {
                        input = input[..^1];
                        Console.SetCursorPosition(inputX + input.Length, inputY);
                        Console.Write(' ');
                        Console.SetCursorPosition(inputX + input.Length, inputY);
                    }
                    else if (keyInfo.KeyChar != '\0' && input.Length < maxLength - 1)
                    {
                        input += keyInfo.KeyChar;
                        Console.Write(keyInfo.KeyChar);
                    }
                }
            }
        }

        protected string ReadPassword()
        {
            string password = string.Empty;
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    return string.Empty;
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password[..^1];
                    Console.Write("\b \b");
                }
                else if (key.Key != ConsoleKey.Backspace)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            }
            return password;
        }

        public void ShowSuccessMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(x + 2, y + height - 2);
            Console.Write(message);
            Console.ResetColor();
        }

        public void ShowErrorMessage(string message)
        {
            // Ghi log lỗi UI
            SimpleLogger.LogError(currentUsername ?? "Unknown", $"UI Error: {message}");
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(x + 2, y + height - 2);
            Console.Write(message);
            Console.ResetColor();
        }

        public void ShowInfoMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(x + 2, y + height - 2);
            Console.Write(message);
            Console.ResetColor();
        }
    }
}
