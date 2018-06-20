// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
namespace IctBaden.Stonehenge.Sample.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;

    using Stonehenge;

    // ReSharper disable once UnusedMember.Global
    public class FormVm : ActiveViewModel
    {
        private static int _nid = 1;
        private string _name;
        private Timer _timer;

        public string Id { get; private set; }
        public string Clock { get; private set; }

        [Bindable(true, BindingDirection.OneWay)]
        public string Prompt => "Enter name:";

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                Debug.WriteLine("[{0}] Name={1}", Id, _name);
            }
        }

        [ActionMethod]
        public void SayHello(AppSession session)
        {
            MessageBox("Demo", "Hello " + Name);
        }

        [ActionMethod]
        public void LoginMarvin(AppSession session)
        {
            //User = "Marvin";
            NavigateTo("login");
        }

        public bool CanSayHello => !string.IsNullOrEmpty(Name);


        [Bindable(true, BindingDirection.OneWay)]
        public List<string> OptionValues { get; set; }
        public List<string> SelectedOptions { get; set; }
        public string SelectedOption { get; set; }

        public CheckedItem AutoUpdate { get; set; }

        public CheckedItem ImageOptionA { get; set; }
        public CheckedItem ImageOptionB { get; set; }
        public CheckedItem ImageOptionC { get; set; }
        public CheckedItem ImageOptionD { get; set; }

        public string ImageOptions
        {
            get
            {
                var options = "";
                options += ImageOptionA.Checked ? "A " : "_ ";
                options += ImageOptionB.Checked ? "B " : "_ ";
                options += ImageOptionC.Checked ? "C " : "_ ";
                options += ImageOptionD.Checked ? "D " : "_ ";
                return options;
            }
        }

        public List<CheckedItem> BitValues { get; set; }

        public string ByteValue
        {
            get
            {
                var value = BitValues.Where(bitValue => bitValue.Checked).Aggregate(0, (current, bitValue) => current | bitValue.Value);
                return $"0x{value:X2}";
            }
        }

        public DateTime TimeStamp { get; set; }

        public FormVm(AppSession session)
            : base(session)
        {
            _nid++;
            Id = "Form (Instance #" + _nid + ")";
            Name = "Frank";

            OptionValues = new List<string> { "One", "Two", "Tree", "Four" };
            SelectedOptions = new List<string> { "Tree" };
            SelectedOption = "Two";
            AutoUpdate = new CheckedItem { Title = "clock display" };

            ImageOptionA = new CheckedItem();
            ImageOptionB = new CheckedItem();
            ImageOptionC = new CheckedItem();
            ImageOptionD = new CheckedItem();

            BitValues = new List<CheckedItem>
        {
          new CheckedItem { Title = "Bit 0", Value = 0x01, Checked = true},
          new CheckedItem { Title = "Bit 1", Value = 0x02, Checked = false},
          new CheckedItem { Title = "Bit 2", Value = 0x04, Checked = true},
          new CheckedItem { Title = "Bit 3", Value = 0x08, Checked = false},
          new CheckedItem { Title = "Bit 4", Value = 0x10, Checked = false},
          new CheckedItem { Title = "Bit 5", Value = 0x20, Checked = true},
          new CheckedItem { Title = "Bit 6", Value = 0x40, Checked = false},
          new CheckedItem { Title = "Bit 7", Value = 0x80, Checked = true},
        };

            ClockTick(this);

            TimeStamp = DateTime.Now;
        }

        private void ClockTick(object state)
        {
            this["Clock"] = DateTime.Now.ToLongTimeString();
        }

        [ActionMethod]
        public void OnOptionChanged()
        {
        }

        [ActionMethod]
        public void OnImgChanged()
        {
        }

        [ActionMethod]
        public void OnBitsChanged()
        {
        }

        [ActionMethod]
        public void OnNameChanged(object name)
        {
            Debug.WriteLine(name);
        }

        [ActionMethod]
        public void OnAutoUpdateChanged()
        {
            if (AutoUpdate.Checked && (_timer == null))
            {
                _timer = new Timer(ClockTick, this, 1000, 1000);
            }
            else if (!AutoUpdate.Checked && (_timer != null))
            {
                _timer.Dispose();
                _timer = null;
            }
        }

    }
}
