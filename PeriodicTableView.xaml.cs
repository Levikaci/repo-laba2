using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;

namespace ChemistryApp2;

public partial class PeriodicTableView : Window
{
    private static readonly SolidColorBrush BrushNonmetal = new(Color.FromRgb(233, 30, 99));
    private static readonly SolidColorBrush BrushMetal = new(Color.FromRgb(33, 150, 243));
    private static readonly SolidColorBrush BrushMetalloid = new(Color.FromRgb(255, 193, 7));
    private static readonly SolidColorBrush BrushNobleGas = new(Color.FromRgb(156, 39, 176));
    private static readonly SolidColorBrush BrushHalogen = new(Color.FromRgb(0, 188, 212));
    private static readonly SolidColorBrush BrushLanthanide = new(Color.FromRgb(255, 87, 34));
    private static readonly SolidColorBrush BrushActinide = new(Color.FromRgb(121, 85, 72));

    private const double CellWidth = 62;
    private const double CellHeight = 78;

    public PeriodicTableView()
    {
        InitializeComponent();
        Loaded += PeriodicTableView_Loaded;
    }

    private async void PeriodicTableView_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            PeriodicGrid.ColumnDefinitions.Clear();
            PeriodicGrid.RowDefinitions.Clear();

   
            for (int i = 0; i < 18; i++)
                PeriodicGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(CellWidth) });

            for (int i = 0; i < 9; i++)
                PeriodicGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(CellHeight) });

            var elements = await App.Db.ChemicalElements.OrderBy(x => x.AtomicNumber).ToListAsync();

            foreach (var el in elements)
            {
                var tile = CreateElementTile(el);
                var (row, col) = GetGridPosition(el);

                if (row >= 0 && col >= 0 && col < 18)
                {
                    Grid.SetRow(tile, row);
                    Grid.SetColumn(tile, col);
                    PeriodicGrid.Children.Add(tile);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private (int row, int col) GetGridPosition(ChemicalElement el)
    {
        int period = el.Period;
        int group = el.GroupNumber;

        if (el.Category == "Лантаноид" && el.AtomicNumber >= 58 && el.AtomicNumber <= 71)
            return (7, el.AtomicNumber - 55); 

        if (el.Category == "Актиноид" && el.AtomicNumber >= 90 && el.AtomicNumber <= 103)
            return (8, el.AtomicNumber - 87); 

        if (period < 1 || period > 7 || group < 1 || group > 18)
            return (-1, -1);


        if (period == 1)
        {
            if (group == 1) return (0, 0);      
            if (group == 18) return (0, 17);  
            return (-1, -1);
        }

        if (period == 2 || period == 3)
        {
            if (group <= 2) return (period - 1, group - 1);
            if (group >= 13) return (period - 1, group - 1);
            return (-1, -1);
        }

        return (period - 1, group - 1);
    }

    private Border CreateElementTile(ChemicalElement el)
    {
        var brush = GetCategoryBrush(el.Category);
        var border = new Border
        {
            Background = brush,
            CornerRadius = new CornerRadius(5),
            Margin = new Thickness(1),
            Padding = new Thickness(2),
            Cursor = Cursors.Hand,
            Tag = el,
            BorderThickness = new Thickness(1),
            BorderBrush = Brushes.White,
            SnapsToDevicePixels = true
        };

        border.MouseEnter += (s, e) => { ((Border)s).BorderThickness = new Thickness(2); ShowElementInfo(el); };
        border.MouseLeave += (s, e) => ((Border)s).BorderThickness = new Thickness(1);
        border.MouseLeftButtonUp += (s, e) => ShowElementDetails(el);

        var panel = new StackPanel { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
        panel.Children.Add(new TextBlock { Text = el.AtomicNumber.ToString(), FontSize = 9, HorizontalAlignment = HorizontalAlignment.Right, Foreground = Brushes.White, Opacity = 0.85 });
        panel.Children.Add(new TextBlock { Text = el.Symbol, FontSize = 19, FontWeight = FontWeights.ExtraBold, HorizontalAlignment = HorizontalAlignment.Center, Foreground = Brushes.White, Margin = new Thickness(0, 1, 0, 2) });
        panel.Children.Add(new TextBlock { Text = el.NameRu, FontSize = 8, HorizontalAlignment = HorizontalAlignment.Center, Foreground = Brushes.White, TextWrapping = TextWrapping.Wrap, TextAlignment = TextAlignment.Center, MaxWidth = 58, Opacity = 0.9 });

        border.Child = panel;
        return border;
    }

    private SolidColorBrush GetCategoryBrush(string category) => category switch
    {
        "Металл" or "Щелочной металл" or "Щелочно-земельный металл" or "Переходный металл" or "Постпереходный металл" => BrushMetal,
        "Металлоид" => BrushMetalloid,
        "Неметалл" => BrushNonmetal,
        "Галоген" => BrushHalogen,
        "Инертный газ" => BrushNobleGas,
        "Лантаноид" => BrushLanthanide,
        "Актиноид" => BrushActinide,
        _ => BrushNonmetal
    };

    private void ShowElementInfo(ChemicalElement el)
    {
        InfoTitle.Text = $"{el.AtomicNumber}. {el.NameRu} ({el.Symbol})";
        InfoDetails.Text = $"Группа: {el.GroupNumber} | Период: {el.Period}\n" +
                          $"Масса: {el.AtomicMass?.ToString("F3") ?? "—"} а.е.м.\n" +
                          $"Категория: {el.Category}\n\n{el.Description ?? "Описание отсутствует"}";
    }

    private void ShowElementDetails(ChemicalElement el)
    {
        MessageBox.Show($"{el.AtomicNumber}. {el.NameRu} ({el.Symbol})\n\n" +
                       $"Масса: {el.AtomicMass?.ToString("F3") ?? "—"} а.е.м.\n" +
                       $"Группа: {el.GroupNumber}, Период: {el.Period}\n" +
                       $"Категория: {el.Category}\n\n{el.Description ?? "Нет описания"}",
                       "Элемент", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}