using Terminal.Gui;
using static Terminal.Gui.View;

namespace Solana链监控指定帐号代币交易.Controllers
{
    public class ConsoleManager
    {
        private Dictionary<string, (Window window, TextView textView)> windows = new Dictionary<string, (Window, TextView)>();

        bool SaveLog = true;
        public ConsoleManager(int width = 120, int height = 60)   
        {
            // 设置控制台窗口大小
            Console.SetWindowSize(width, height);
            Console.SetBufferSize(width, height);  // 设置缓冲区大小与窗口大小相同
           
            Application.Init();
           
        }
        public void AddTopWindow(string title)
        {
            var top = Application.Top;

            // 创建顶部窗口，高度改为5行
            var win = new Window(title)
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = 5  // 改为5行高度
            };

            var text = new TextView()
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill() - 1,
                Height = Dim.Fill() - 1,
                ReadOnly = true,
                CanFocus = false,
                Enabled = false,
                AllowsTab = false,
                AllowsReturn = false
            };

            win.Add(text);
            top.Add(win);
            windows[title] = (win, text);
        }

        public void AddContent(string windowTitle, string content)
        {
            if (windows.TryGetValue(windowTitle, out var window))
            {
                Application.MainLoop.Invoke(() =>
                {
                    var textView = window.textView;

                    // 插入新内容
                    var formattedContent = $"[{DateTime.Now:HH:mm:ss}] {content}\n";
                    textView.Text += formattedContent;

                    // 自动滚动到底部
                    var lines = textView.Text.Split("\n");
                    if (lines.Length > textView.Frame.Height)
                    {
                        int newTopRow = lines.Length - textView.Frame.Height;
                        textView.TopRow = Math.Max(newTopRow, 0);
                    }

                    textView.SetNeedsDisplay();
                    window.window.SetNeedsDisplay();
                    
                  
                });
            }
        }
         
        private string ExtractAddress(string text)
        {
            var keyword = "合约地址:";
            var startIndex = text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);
            if (startIndex >= 0)
            {
                startIndex += keyword.Length;
                var endIndex = text.IndexOf(' ', startIndex);
                if (endIndex == -1) endIndex = text.Length; // 如果是行尾
                return text.Substring(startIndex, endIndex - startIndex).Trim();
            }
            return string.Empty;
        }



        public void AddWindows(params string[] titles)
        {
            var top = Application.Top;
            int count = titles.Length;

            // 计算行列数
            int columns = 2;  // 固定2列
            int rows = (count + columns - 1) / columns;  // 向上取整得到行数

            // 计算每个窗口的高度百分比，预留底部5行
            int heightPercent = (100 - 20) / rows;  // 95%的高度平均分配

            // 先添加底部窗口
            var bottomWin = new Window("Logs")
            {
                X = 0,
                Y = Pos.Percent(80),  // 在95%位置
                Width = Dim.Fill(),   // 占满宽度
                Height = Dim.Percent(20) // 5%的高度
            };

            var bottomText = new TextView()
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill() - 1,
                Height = Dim.Fill() - 1,
                ReadOnly = true,
                CanFocus = true
            };

            bottomWin.Add(bottomText);
            top.Add(bottomWin);
            windows["Logs"] = (bottomWin, bottomText);

            // 添加其他窗口
            for (int i = 0; i < count; i++)
            {
                var title = titles[i];

                int row = i / columns;
                int col = i % columns;

                var win = new Window(title)
                {
                    X = col == 0 ? 0 : Pos.Percent(50),
                    Y = Pos.Percent(row * heightPercent),
                    Width = Dim.Percent(50),
                    Height = Dim.Percent(heightPercent)
                };

                var text = new TextView()
                {
                    X = 0,
                    Y = 1,
                    Width = Dim.Fill() - 1,
                    Height = Dim.Fill() - 1,
                    ReadOnly = true,
                    CanFocus = true
                };
                // 只注册一次点击事件
                text.MouseClick += (MouseEventArgs me) =>
                {
                    if (me.MouseEvent.Flags == MouseFlags.Button1Clicked)
                    {
                        var lines = text.Text.Split("\n");
                        int clickedLineIndex = me.MouseEvent.Y + text.TopRow;

                        if (clickedLineIndex < lines.Length && clickedLineIndex >= 0)
                        {
                            var clickedLine = lines[clickedLineIndex];
                            if (clickedLine.Contains("合约地址:"))
                            {
                                var address = ExtractAddress(clickedLine.ToString());
                                if (!string.IsNullOrEmpty(address))
                                {
                                    var add = address.Replace("(点击复制)", "");
                                    Clipboard.TrySetClipboardData(address);
                                    MessageBox.Query("提示", $"已复制到剪贴板: {add}", "确定");
                                }
                            }
                        }
                    }
                };
                win.Add(text);
                top.Add(win);
                windows[title] = (win, text);
            }
        } 
        //public void AddContent(string windowTitle, string content)
        //{
        //    if (windows.TryGetValue(windowTitle, out var window))
        //    {
        //        Application.MainLoop.Invoke(() =>
        //        {
        //            var textView = window.textView;
        //            textView.Text += $"[{DateTime.Now:HH:mm:ss}] {content}\n";

        //            // 计算并设置滚动位置
        //            var lines = textView.Text.Split("\n");
        //            if (lines.Length > textView.Frame.Height)
        //            {
        //                int newTopRow = lines.Length - textView.Frame.Height + 1;
        //                if (newTopRow > 0)
        //                {
        //                    textView.TopRow = newTopRow;
        //                }
        //            }

        //            textView.SetNeedsDisplay();
        //            window.window.SetNeedsDisplay();
                   
        //        });
        //    }
        //}
        public void Run()
        {
            Application.Run();
        }
    }
}
