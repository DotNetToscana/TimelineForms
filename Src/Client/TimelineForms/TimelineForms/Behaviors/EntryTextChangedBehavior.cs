using Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TimelineForms.Behaviors
{
    /// <summary>
    /// Updates text while Entry text changes
    /// </summary>
    public class EntryTextChangedBehavior : BehaviorBase<Entry>
    {
        public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(EntryTextChangedBehavior), null, propertyChanged: OnTextChanged);

        private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var behavior = (EntryTextChangedBehavior)bindable;
            if (behavior.AssociatedObject == null)
                return;

            behavior.AssociatedObject.Text = Convert.ToString(newValue);
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            this.AssociatedObject.TextChanged += this.OnEntryTextChanged;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            this.AssociatedObject.TextChanged -= this.OnEntryTextChanged;
            base.OnDetachingFrom(bindable);
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            this.Text = e.NewTextValue;
        }
    }
}
