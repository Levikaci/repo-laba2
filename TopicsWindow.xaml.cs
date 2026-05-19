using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace ChemistryApp2;

public partial class TopicsWindow : Window
{
    public TopicsWindow()
    {
        InitializeComponent();
        Loaded += TopicsWindow_Loaded;
    }

    private async void TopicsWindow_Loaded(object sender, RoutedEventArgs e)
        => await LoadTopics();

    private async Task LoadTopics()
    {
        try
        {
            var topics = await App.Db.Topics.OrderBy(t => t.SortOrder).ToListAsync();
            TopicsList.ItemsSource = topics;
        }
        catch (System.Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки тем: {ex.Message}", "Ошибка",
                          MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void TopicsList_SelectionChanged(object s,
        System.Windows.Controls.SelectionChangedEventArgs e)
    {
        // Опционально: обработка выбора
    }

    private async void Refresh_Click(object s, RoutedEventArgs e)
        => await LoadTopics();

    private void Close_Click(object s, RoutedEventArgs e) => Close();
}