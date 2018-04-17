
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EditModeSample
{
    class MainViewModel
    {
        public ObservableCollection<ListItemViewModel> Items { get; private set; }

        public IEnumerable<ListItemViewModel> ComboBoxItems
        {
            get
            {
                // try to spend some time here to make the generation of ComboBoxes even slower
                return Items.Where(i => int.Parse(((int)(Math.Sqrt(i.Number * 1000))).ToString()) % 2 == 0);
            }
        }

        public MainViewModel()
        {
            this.Items = new ObservableCollection<ListItemViewModel>();
            for (int i = 0; i < 1000; i++)
            {
                this.Items.Add(new ListItemViewModel() { Number = i });
            }
        }
    }
}
