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
using System.Windows.Shapes;

namespace MemoryGame
{
    /// <summary>
    /// Interaction logic for DifficultyWindow.xaml
    /// </summary>
    public partial class DifficultyWindow : Window
    {
        public int SelectedDifficulty { get; private set; }

        public DifficultyWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (DifficultyComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                SelectedDifficulty = int.Parse(selectedItem.Tag.ToString());
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Please select a difficulty level.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
