using CollectionManagmentSystem.Views;
using CollectionManagmentSystem.Models;
using CollectionManagmentSystem.Controls;
using CollectionManagmentSystem.Services;

namespace CollectionManagmentSystem.Views;

public partial class CollectionPage : ContentPage
{
    private List<CollectionItem> _collectionItems;
    private Collection _collection;
    private readonly string _dataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Collections");
    public CollectionPage(Collection collection)
    {
        InitializeComponent();
        BindingContext = collection;
        _collection = collection;
        LoadCollectionItems();
    }

    private async void OnSummary_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CollectionSummaryPage(_collection));
    }

    private async void LoadCollectionItems()
    {
        var dataService = new Files();
        _collectionItems = await dataService.LoadCollectionItems(_collection.CollectionName);
        collectionView.ItemsSource = _collectionItems;
       
    }

    private void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is CollectionItem selectedItem)
        {
            editButtonLayout.IsVisible = true;
        }
        else
        {
            editButtonLayout.IsVisible = false;
        }
    }

    private async void OnAdd_Clicked(object sender, EventArgs e)
    {
        string itemName = await DisplayPromptAsync("Nowy przedmiot", "Wprowad� nazw� nowego przedmiotu:");
        if (itemName == null)
        {
            return;
        }

        if (_collectionItems.Any(item => item.Name == itemName))
        {
            var confirm = await DisplayAlert("Ostrze�enie!", "Przedmiot z podan� nazw� ju� istnieje. Czy chcesz doda� nowy?", "OK", "Anuluj");
            if (!confirm)
            {
                return;
            }
        }

        double price;
        while (true)
        {
            string priceText = await DisplayPromptAsync("Nowy przedmiot", "Podaj cen� przedmiotu:");
            if (priceText == null)
            {
                return;
            }

            if (double.TryParse(priceText, out price))
            {
                break;
            }

            await DisplayAlert("B��d!", "Nieprawid�owa cena. Wprowad� prawid�ow� warto��.", "OK");
        }

        int rating;
        while (true)
        {
            string ratingText = await DisplayPromptAsync("Nowy przedmiot", "Wprowad� ocen� przedmiotu:");
            if (ratingText == null)
            {
                return;
            }

            if (int.TryParse(ratingText, out rating))
            {
                break;
            }
                
            await DisplayAlert("B��d!", "Nieprawid�owa ocena.  Wprowad� prawid�ow� warto��.", "OK");
        }

        string status = await GetStatusPrompt();

        if (status == null)
        {
            return;
        }

        int newId = _collectionItems.Count + 1;

        CollectionItem newItem = new CollectionItem
        {
            ProductId = newId,
            Name = itemName,
            Price = price,
            Rating = rating,
            Status = status
        }; 

        _collectionItems.Add(newItem);
        var dataService = new Files();
        dataService.SaveCollectionItems(_collection.CollectionName, _collectionItems);

        LoadCollectionItems();

        editButtonLayout.IsVisible = false;
    }

    private async void OnEdit_Clicked(object sender, EventArgs e)
    {
        if (collectionView.SelectedItem is CollectionItem selectedItem)
        {

            string selectedProperty = await DisplayActionSheet("Wybierz w�a�ciwo�� do edycji:", null, null, "Nazwa", "Cena", "Ocena", "Stan");
            if (selectedProperty == null || selectedProperty == "Cancel")
                return;

            string newValue;
            switch (selectedProperty)
            {
                case "Nazwa":
                    newValue = await DisplayPromptAsync("Edytuj nazw� przedmiotu", "Wprowad� now� nazw� przedmiotu:", "OK", "Anuluj", selectedItem.Name);
                    if (newValue == null)
                    {
                        return;
                    }

                    selectedItem.Name = newValue;
                    break;
                case "Cena":
                    newValue = await DisplayPromptAsync("Edytuj cen�", "Wprowad� now� cen� przedmiotu:", "OK", "Anuluj", selectedItem.Price.ToString());
                    if (newValue == null)
                    {
                        return;
                    }

                    double newPrice;
                    if (!double.TryParse(newValue, out newPrice))
                    {
                        await DisplayAlert("B��d!", "Wyst�pi� b��d podczas przetwarzania ceny przedmiotu. Wprowad� prawid�ow� liczb�.", "OK");
                        return;
                    }

                    selectedItem.Price = newPrice;
                    break;
                case "Ocena":
                    newValue = await DisplayPromptAsync("Edytuj ocen�", "Wprowad� now� ocen�:", "OK", "Anuluj", selectedItem.Rating.ToString());
                    if (newValue == null)
                    {
                        return;
                    }

                    int newRating;
                    if (!int.TryParse(newValue, out newRating))
                    {
                        await DisplayAlert("B��d!", "Wyst�pi� b��d podczas przetwarzania oceny przedmiotu. Wprowad� prawid�ow� liczb�.", "OK");
                        return;
                    }
                    selectedItem.Rating = newRating;
                    break;
                case "Stan":
                    newValue = await GetStatusPrompt();
                    if (newValue == null)
                    {
                        return;
                    }

                    selectedItem.Status = newValue;
                    break;
            }

            var dataService = new Files();
            dataService.SaveCollectionItems(_collection.CollectionName, _collectionItems);

            LoadCollectionItems();

            editButtonLayout.IsVisible = false;
        }
    }

    private async void OnDelete_Clicked(object sender, EventArgs e)
    {
        if (collectionView.SelectedItem is CollectionItem selectedItem)
        {
            var confirm = await DisplayAlert("Usu� przedmiot", $"Czy na pewno chcesz usun�� podany przedmiot: {selectedItem.Name}", "OK", "Anuluj");

            if (confirm)
            {
                _collectionItems.Remove(selectedItem);
                var dataService = new Files();
                dataService.SaveCollectionItems(_collection.CollectionName, _collectionItems);
                LoadCollectionItems();
                editButtonLayout.IsVisible = false;
            }
        }
    }

    private async Task<string> GetStatusPrompt()
    {
        string[] statusOptions = { "Nowy", "U�ywany", "Na sprzeda�", "Sprzedany" };
        string selectedStatus = await DisplayActionSheet("Wybierz stan produktu: ", null, null, statusOptions);

        if (selectedStatus == null)
        {
            return null;
        }
        return selectedStatus;
    }
}