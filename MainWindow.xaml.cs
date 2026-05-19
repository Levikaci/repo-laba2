using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace ChemistryApp2;

public partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();

    private void Window_Loaded(object sender, RoutedEventArgs e) => LoadTopics();

    private async void LoadTopics()
    {
        try
        {
            var list = await App.Db.Topics.OrderBy(t => t.SortOrder).ToListAsync();
            TopicsList.ItemsSource = list;
            if (list.Any()) TopicsList.SelectedIndex = 0;
        }
        catch (Exception ex) { ShowError(ex.Message); }
    }

    private void TopicsList_SelectionChanged(object s, SelectionChangedEventArgs e)
    {
        if (TopicsList.SelectedItem is Topic t) { TopicTitle.Text = t.Title; TopicContent.Text = t.Content; HideElement(); }
    }

    private async void Search_Click(object s, RoutedEventArgs e) => await SearchElements();
    private void Clear_Click(object s, RoutedEventArgs e) { SearchBox.Text = ""; ResultsList.ItemsSource = null; HideElement(); ErrorBorder.Visibility = Visibility.Collapsed; }

    private async Task SearchElements()
    {
        ErrorBorder.Visibility = Visibility.Collapsed;
        string q = SearchBox.Text.Trim();
        if (string.IsNullOrEmpty(q)) { ShowError("Введите запрос"); return; }

        try
        {
            var list = int.TryParse(q, out var n)
                ? await App.Db.ChemicalElements.Where(x => x.AtomicNumber == n).Take(10).ToListAsync()
                : await App.Db.ChemicalElements.Where(x => x.Symbol.ToLower().Contains(q.ToLower()) || x.NameRu.ToLower().Contains(q.ToLower())).Take(10).ToListAsync();

            if (!list.Any()) { ShowError($"Не найдено: {q}"); return; }
            ResultsList.ItemsSource = list;
            HideElement();
        }
        catch (Exception ex) { ShowError(ex.Message); }
    }

    private void ResultsList_SelectionChanged(object s, SelectionChangedEventArgs e)
    {
        if (ResultsList.SelectedItem is ChemicalElement el)
        {
            ElNum.Text = el.AtomicNumber.ToString();
            ElSym.Text = el.Symbol;
            ElName.Text = el.NameRu;
            ElMass.Text = el.AtomicMass?.ToString("F3") + " а.е.м." ?? "—";
            SeparatorEl.Visibility = Visibility.Visible;
            ElementInfo.Visibility = Visibility.Visible;
        }
    }


    private void ShowTopics_Click(object sender, RoutedEventArgs e)
    {
        var topicsWindow = new TopicsWindow { Owner = this };
        topicsWindow.ShowDialog();
        LoadTopics();
    }


    private void ShowTable_Click(object s, RoutedEventArgs e)
    {
        new PeriodicTableView().ShowDialog();
    }
    private void HideElement() { SeparatorEl.Visibility = Visibility.Collapsed; ElementInfo.Visibility = Visibility.Collapsed; }
    private void ShowError(string msg) { ErrorMsg.Text = msg; ErrorBorder.Visibility = Visibility.Visible; }
}