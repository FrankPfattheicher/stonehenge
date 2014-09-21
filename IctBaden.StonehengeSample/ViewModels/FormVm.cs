using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using IctBaden.Stonehenge;

namespace IctBaden.StonehengeSample.ViewModels
{
    public class FormVm : ActiveViewModel
    {
        private static int nid = 1;
        private string name;
        private Timer timer;

        public string Id { get; private set; }
        public string Clock { get; private set; }

        [Bindable(true, BindingDirection.OneWay)]
        public string Prompt { get { return "Enter name:"; } }
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                Debug.WriteLine("[{0}] Name={1}", Id, name);
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

        public bool CanSayHello
        { get { return !string.IsNullOrEmpty(Name); } }


        [Bindable(true, BindingDirection.OneWay)]
        public List<string> OptionValues { get; set; }
        public List<string> SelectedOptions { get; set; }
        public string SelectedOption { get; set; }

        public CheckedItem AutoUpdate { get; set; }

        public List<CheckedItem> BitValues { get; set; }

        public string ByteValue
        {
            get
            {
                var value = BitValues.Where(bitValue => bitValue.Checked).Aggregate(0, (current, bitValue) => current | bitValue.Value);
                return string.Format("0x{0:X2}", value);
            }
        }

        public DateTime TimeStamp { get; set; }

        public FormVm(AppSession session)
            : base(session)
        {
            nid++;
            Id = "Form (Instance #" + nid + ")";
            Name = "Frank";

            OptionValues = new List<string> { "One", "Two", "Tree", "Four" };
            SelectedOptions = new List<string> { "Tree" };
            SelectedOption = "Two";
            AutoUpdate = new CheckedItem { Title = "clock display" };

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
        public void OnBitsChanged()
        {
        }

        [ActionMethod]
        public void OnNameChanged(object name)
        {
        }

        [ActionMethod]
        public void OnAutoUpdateChanged()
        {
            if (AutoUpdate.Checked && (timer == null))
            {
                timer = new Timer(ClockTick, this, 1000, 1000);
            }
            else if (!AutoUpdate.Checked && (timer != null))
            {
                timer.Dispose();
                timer = null;
            }
        }

    }
}
