using CollectionManagmentSystem.Models;
using CollectionManagmentSystem.Controls;
using CollectionManagmentSystem.Services;
using CollectionManagmentSystem.Views;

namespace CollectionManagmentSystem
{
    public partial class MainPage : ContentPage
    {
        private readonly Services.Files _fileDataService = new Services.Files();
        private readonly string _dataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Collections");
        private List<Collection> _collections;

        public MainPage()
        {
            InitializeComponent();
            Directory.CreateDirectory(_dataFolderPath);
            LoadCollections();
        }

        private void LoadCollections()
        {
            _collections = new List<Collection>();
            var collectionFiles = Directory.GetFiles(_dataFolderPath, "*.txt");

            foreach (var filePath in collectionFiles)
            {
                var collectionName = Path.GetFileNameWithoutExtension(filePath);
                _collections.Add(new Collection { Id = _collections.Count + 1, CollectionName = collectionName });
            }

            collectionListView.ItemsSource = _collections;
        }

        private async void OnAddCollection_Clicked(object sender, EventArgs e)
        {
            var collectionName = await DisplayPromptAsync("Nowa kolekcja", "Wprowadź nazwę kolekcji:");

            if (!string.IsNullOrWhiteSpace(collectionName))
            {
                var filePath = Path.Combine(_dataFolderPath, $"{collectionName}.txt");
                if (System.IO.File.Exists(filePath))
                {
                    var confirm = await DisplayAlert("Ostrzeżenie!", "Kolekcja z podaną nazwą już istnieje. Czy chcesz dodać nową?", "OK", "Anuluj");
                    if (!confirm)
                    {
                        return;
                    }
                }

                System.IO.File.Create(filePath).Dispose();

                _collections.Add(new Collection { Id = _collections.Count + 1, CollectionName = collectionName });
                collectionListView.ItemsSource = null;
                collectionListView.ItemsSource = _collections;

                descriptionLayout1.IsVisible = false;
                descriptionLayout2.IsVisible = false;
            }
        }

        public async void OnEditCollection_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Collection selectedCollection)
            {
                string newName = await DisplayPromptAsync("Edytuj nazwę kolekcji", "Wprowadź nową nazwę: ", "OK", "Anuluj", selectedCollection.CollectionName);
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    var oldFilePath = Path.Combine(_dataFolderPath, $"{selectedCollection.CollectionName}.txt");
                    var newFilePath = Path.Combine(_dataFolderPath, $"{newName}.txt");
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Move(oldFilePath, newFilePath);
                        System.IO.File.Delete(oldFilePath);
                    }
                    selectedCollection.CollectionName = newName;
                    collectionListView.ItemsSource = null;
                    collectionListView.ItemsSource = _collections;
                    LoadCollections();

                    descriptionLayout1.IsVisible = false;
                    descriptionLayout2.IsVisible = false;
                }
            }
        }

        private async void OnDeleteCollection_Clicked(object sender, EventArgs e)
        {
            if (collectionListView.SelectedItem is Collection selectedCollection)
            {
                var confirm = await DisplayAlert("Usuń kolekcję", $"Czy na pewno chcesz usunąć podaną kolekcję: {selectedCollection.CollectionName}", "OK", "Anuluj");

                if (confirm)
                {
                    var filePath = Path.Combine(_dataFolderPath, $"{selectedCollection.CollectionName}.txt");
                    System.IO.File.Delete(filePath);

                    _collections.Remove(selectedCollection);
                    collectionListView.ItemsSource = null;
                    collectionListView.ItemsSource = _collections;

                    descriptionLayout1.IsVisible = false;
                    descriptionLayout2.IsVisible = false;
                }
            }
            else
            {
                await DisplayAlert("Błąd!", "Wybierz kolekcję do usunięcia", "OK");
            }
        }

        private void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Collection selectedCollection)
            {
                descriptionLayout1.IsVisible = true;
                descriptionLayout1.BindingContext = selectedCollection;
                descriptionLayout2.IsVisible = true;
                descriptionLayout2.BindingContext = selectedCollection;
            }
            else
            {
                descriptionLayout1.IsVisible = false;
                descriptionLayout2.IsVisible = false;
            }
        }

        private async void OnViewCollection_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Collection selectedCollection)
            {
                await Navigation.PushAsync(new CollectionPage(selectedCollection));
            }
        }

        private void ShowDataPath_Clicked(object sender, EventArgs e)
        {
            dataPathLabel.IsVisible = !dataPathLabel.IsVisible;

            if (dataPathLabel.IsVisible)
            {
                dataPathLabel.Text = $"Dane znajdują się w podanej lokalizacji: {_dataFolderPath}";
            }
        }
    }
}