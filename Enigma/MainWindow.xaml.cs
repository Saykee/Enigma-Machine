using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Timers;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Enigma
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Creating the Rotor Combo Boxes
            char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            CbxRotor1.ItemsSource = alphabet;
            CbxRotor2.ItemsSource = alphabet;
            CbxRotor3.ItemsSource = alphabet;
            CbxRotor1.SelectedIndex = 0;
            CbxRotor2.SelectedIndex = 0;
            CbxRotor3.SelectedIndex = 0;
        }

        //Code used to scale the window to any size without distorting objects
        #region WPF Scaling

        public static readonly DependencyProperty ScaleValueProperty = DependencyProperty.Register("ScaleValue", typeof(double), typeof(MainWindow), new UIPropertyMetadata(1.0, new PropertyChangedCallback(OnScaleValueChanged), new CoerceValueCallback(OnCoerceScaleValue)));

        private static object OnCoerceScaleValue(DependencyObject o, object value)
        {
            MainWindow mainWindow = o as MainWindow;
            if (mainWindow != null)
                return mainWindow.OnCoerceScaleValue((double)value);
            else
                return value;
        }

        private static void OnScaleValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            MainWindow mainWindow = o as MainWindow;
            if (mainWindow != null)
                mainWindow.OnScaleValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceScaleValue(double value)
        {
            if (double.IsNaN(value))
                return 1.0f;

            value = Math.Max(0.1, value);
            return value;
        }

        protected virtual void OnScaleValueChanged(double oldValue, double newValue)
        {

        }

        public double ScaleValue
        {
            get
            {
                return (double)GetValue(ScaleValueProperty);
            }
            set
            {
                SetValue(ScaleValueProperty, value);
            }
        }

        private void MainGrid_SizeChanged(object sender, EventArgs e)
        {
            CalculateScale();
        }

        private void CalculateScale()
        {
            double yScale = ActualHeight / 250f;
            double xScale = ActualWidth / 200f;
            double value = Math.Min(xScale, yScale);
            ScaleValue = (double)OnCoerceScaleValue(MainWindow1, value);
        }
        #endregion WPF Scaling

        //Disables the space bar. Enigma has no way of encrypting a space
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
            else
            {
                base.OnPreviewKeyDown(e);
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Window Loads -Display Popup with instructions on how to use the application
            Instructions instructions = new Instructions();
            instructions.Show();
        }

        private void BtnEnigmaInfo_Click(object sender, RoutedEventArgs e)
        {
            //Enigma Logo Button - Checks if the window is alrady open and if so will not open another window
            bool isWindowOpen = false;

            foreach (Window w in Application.Current.Windows)
            {
                if (w is Instructions)
                {
                    isWindowOpen = true;
                    w.Activate();
                }
            }

            if (!isWindowOpen)
            {
                Instructions instructions = new Instructions();
                instructions.Show();
            }
        }

        private void TxtBxMessage_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //When the user types something, update the text block with the encoded message
            TxtBlkEMessage.Text = TxtBxMessage.Text + "123";

        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            //Save Button - Saves the encoded message to a .txt file where the user wants
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, TxtBlkEMessage.Text);

        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            //Clear Button - Clears the Text Box & Text Block
            TxtBxMessage.Text = "";
            TxtBlkEMessage.Text = "";
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            //Reset Button - Resets the Rotors
            CbxRotor1.SelectedIndex = 0;
            CbxRotor2.SelectedIndex = 0;
            CbxRotor3.SelectedIndex = 0;
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            //Settings Button - Opens the settings window for the Plugs & Rotors
            Settings settings = new Settings();
            settings.ShowDialog();
        }

        //Updates the Text Blocks next to the Rotor Combo Boxes to the choosen value
        #region Update Rotor Text Block
        private void CbxRotor1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TxtBlkRotor1.Text = CbxRotor1.SelectedValue.ToString();
            UpdateSettingsText();
        }

        private void CbxRotor2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TxtBlkRotor2.Text = CbxRotor2.SelectedValue.ToString();
            UpdateSettingsText();
        }

        private void CbxRotor3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TxtBlkRotor3.Text = CbxRotor3.SelectedValue.ToString();
            UpdateSettingsText();
        }
        #endregion Update Rotor Text Blocks

        public void UpdateSettingsText()
        {
            TxtBlkSettings.Text = $"| Rotor 1 : {TxtBlkRotor1.Text} | Rotor 2 : {TxtBlkRotor2.Text} | Rotor 3 : {TxtBlkRotor3.Text} |";
        }
    }
}
