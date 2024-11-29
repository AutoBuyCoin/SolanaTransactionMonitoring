using Spectre.Console;
using System.Collections.Concurrent;

public class SpectreConsoleManager
{
    private Layout rootLayout;
    private ConcurrentDictionary<string, Layout> sections = new();
    private bool isRunning = false;
    private CancellationTokenSource cts = new();

    public SpectreConsoleManager()
    {
        Console.Clear();
        rootLayout = new Layout("Root");
    }

    public void AddTopSection(string title)
    {
        var topLayout = new Layout(title)
            .Size(5); // 固定高度5行

        rootLayout = new Layout("Root")
            .SplitRows(
                topLayout,
                new Layout("Content")
            );

        sections[title] = topLayout;
        sections["Content"] = rootLayout["Content"];
    }

    public void AddWindows(params string[] titles)
    {
        int count = titles.Length;

        // 计算行列数
        int columns = 2;  // 固定2列
        int rows = (count + columns - 1) / columns;  // 向上取整得到行数

        // 创建主布局
        var mainLayout = new Layout("Root")
            .SplitRows(
                new Layout("Content"),
                new Layout("Logs").Size(10)  // 底部固定10行高度
            );

        // 创建内容区域的网格布局
        var contentLayout = mainLayout["Content"];
        var rowLayouts = new List<Layout>();

        // 创建行布局
        for (int row = 0; row < rows; row++)
        {
            var rowLayout = new Layout($"Row{row}")
                .Size(100 / rows - (row == rows - 1 ? 20 : 0));

            rowLayouts.Add(rowLayout);
        }

        // 将所有行添加到内容区域
        contentLayout.SplitRows(rowLayouts.ToArray());

        // 添加各个窗口
        for (int i = 0; i < count; i++)
        {
            var title = titles[i];
            int row = i / columns;
            int col = i % columns;

            // 创建面板
            var panel = new Panel("")
            {
                Header = new PanelHeader(title),
                Padding = new Padding(1, 1, 1, 1),
                Border = BoxBorder.Heavy
            };

            // 获取当前行的布局
            var currentRow = rowLayouts[row];

            // 如果是该行的第一个窗口，需要先分列
            if (col == 0)
            {
                currentRow.SplitColumns(
                    new Layout($"Left_{row}"),   // 使用唯一的名称
                    new Layout($"Right_{row}")   // 使用唯一的名称
                );
            }

            // 更新对应位置的布局
            string layoutName = col == 0 ? $"Left_{row}" : $"Right_{row}";
            currentRow[layoutName].Update(panel);
            sections[title] = currentRow[layoutName];
        }

        // 添加底部日志区域
        var logsPanel = new Panel("")
        {
            Header = new PanelHeader("Logs"),
            Padding = new Padding(1, 1, 1, 1),
            Border = BoxBorder.Heavy
        };

        mainLayout["Logs"].Update(logsPanel);
        sections["Logs"] = mainLayout["Logs"];

        // 保存根布局
        rootLayout = mainLayout;
    }
    public void AddContent(string sectionTitle, string content)
    {
        if (sections.TryGetValue(sectionTitle, out var layout))
        {
            var panel = new Panel(content)
            {
                Header = new PanelHeader(sectionTitle),
                Padding = new Padding(1, 1, 1, 1)
            };
            layout.Update(panel);
        }
    }

    private void StartRefreshLoop()
    {
        isRunning = true;
        Task.Run(async () =>
        {
            while (!cts.Token.IsCancellationRequested)
            {
                Console.Clear();
                AnsiConsole.Write(rootLayout);
                await Task.Delay(100); // 刷新频率
            }
        }, cts.Token);
    }

    public void Stop()
    {
        cts.Cancel();
        isRunning = false;
    }
}