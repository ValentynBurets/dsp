using dsp.ViewModel;
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

namespace dsp
{
    /// <summary>
    /// Interaction logic for DiscreteSignalsView.xaml
    /// </summary>
    public partial class DiscreteSignalsView : Window
    {
        public DiscreteSignalsView()
        {
            InitializeComponent();
            DataContext = new DiscreteSignalsViewModel();
        }
    }
}
