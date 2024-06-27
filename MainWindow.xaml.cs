using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RecipeApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<RecipeHolder> recipes = new List<RecipeHolder>();
        private RecipeHolder currentRecipe;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddRecipeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
            currentRecipe = new RecipeHolder();
        }

        private void ListRecipesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Display a list of recipes in a message box
            if (recipes.Count == 0)
            {
                MessageBox.Show("No recipes available.");
            }
            else
            {
                StringBuilder recipeList = new StringBuilder();
                foreach (var recipe in recipes.OrderBy(r => r.Name))
                {
                    recipeList.AppendLine(recipe.Name);
                }
                MessageBox.Show("Recipes:\n" + recipeList.ToString());
            }
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddIngredientButton_Click(object sender, RoutedEventArgs e)
        {
            // Get ingredient details and add to the current recipe
            if (currentRecipe != null)
            {
                double quantity;
                double.TryParse(NumIngredientsTextBox.Text, out quantity);

                string unit = UnitComboBox.SelectedItem?.ToString() ?? "teaspoon"; // Default to teaspoon

                double calories;
                double.TryParse(CaloriesTextBox.Text, out calories);

                Ingredient ingredient = new Ingredient
                {
                    Name = IngredientNameTextBox.Text,
                    Quantity = quantity,
                    UnitIndex = GetUnitIndex(unit),
                    Calories = calories
                };             

                currentRecipe.Ingredients.Add(ingredient);

                // Update the IngredientsList ItemsControl
                IngredientsList.Items.Add(new TextBlock { Text = ingredient.ToString() });

                // Clear input fields
                IngredientNameTextBox.Clear();
                NumIngredientsTextBox.Clear();
                CaloriesTextBox.Clear();
            }
        }

        private int GetUnitIndex(string unit)
        {
            string[] units = { "teaspoon", "tablespoon", "cup", "g", "kg", "ml", "l" };
            for (int i = 0; i < units.Length; i++)
            {
                if (units[i] == unit)
                {
                    return i;
                }
            }
            return 0; // Default to teaspoon
        }

        private void AddStepButton_Click(object sender, RoutedEventArgs e)
        {
            // Add a step to the current recipe
            if (currentRecipe != null)
            {
                string stepText = StepsList.SelectedItem?.ToString() ?? string.Empty;
                currentRecipe.Steps.Add(stepText);
                StepsList.SelectedItem = null;
            }
        }

        private void MakeRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            // Calculate and display total calories for the current recipe
            if (currentRecipe != null)
            {
                double totalCalories = 0;
                foreach (var ingredient in currentRecipe.Ingredients)
                {
                    totalCalories += ingredient.Calories * ingredient.Quantity;
                }
                TotalCaloriesTextBlock.Text = $"Total Calories: {totalCalories:F2}";
            }
        }

        private void ClearFields()
        {
            RecipeNameTextBox.Clear();
            NumIngredientsTextBox.Clear();
            IngredientsList.Items.Clear();
            NumStepsTextBox.Clear();
            StepsList.SelectedItem = null;
            TotalCaloriesTextBlock.Text = string.Empty;
        }
    }

    public class RecipeHolder
    {
        public string Name { get; set; }
        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        public List<string> Steps { get; set; } = new List<string>();

        public double CalculateTotalCalories()
        {
            double totalCalories = 0;
            foreach (var ingredient in Ingredients)
            {
                totalCalories += ingredient.Calories * ingredient.Quantity;
            }
            return totalCalories;
        }
    }

    public class Ingredient
    {
        public string Name { get; set; }
        public double Quantity { get; set; }
        public int UnitIndex { get; set; }
        public int FoodGroupIndex { get; set; }
        public double Calories { get; set; }

        public override string ToString()
        {
            return $"{Name} - {Quantity} {GetMeasurementUnit(UnitIndex)}";
        }

        private static string GetMeasurementUnit(int unitIndex)
        {
            string[] units = { "teaspoon", "tablespoon", "cup", "g", "kg", "ml", "l" };
            if (unitIndex < 0 || unitIndex >= units.Length)
            {
                return "Unknown unit";
            }
            return units[unitIndex];
        }
    }
}