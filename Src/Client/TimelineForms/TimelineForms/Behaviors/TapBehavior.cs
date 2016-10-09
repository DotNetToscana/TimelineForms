using Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace TimelineForms.Behaviors
{
    [Preserve(AllMembers = true)]
    [ContentProperty("Actions")]
    public sealed class TapBehavior : BehaviorBase<View>
    {
        public static readonly BindableProperty ActionsProperty = BindableProperty.Create("Actions", typeof(ActionCollection), typeof(TapBehavior), null);

        public ActionCollection Actions
        {
            get
            {
                var actionCollection = (ActionCollection)GetValue(ActionsProperty);
                if (actionCollection == null)
                {
                    actionCollection = new ActionCollection();
                    SetValue(ActionsProperty, actionCollection);
                }

                return actionCollection;
            }
        }

        protected override void OnAttachedTo(View bindable)
        {
            base.OnAttachedTo(bindable);
            RegisterEvent();
        }

        protected override void OnDetachingFrom(View bindable)
        {
            DeregisterEvent();
            base.OnDetachingFrom(bindable);
        }

        private void RegisterEvent()
        {
            var tgr = new TapGestureRecognizer();
            tgr.Tapped += OnEvent;

            AssociatedObject.GestureRecognizers.Add(tgr);
        }

        private void DeregisterEvent()
        {
            AssociatedObject.GestureRecognizers.Clear();
        }

        private async void OnEvent(object sender, object eventArgs)
        {
            foreach (var bindable in Actions)
            {
                bindable.BindingContext = BindingContext;
                var action = (IAction)bindable;

                await action.Execute(sender, eventArgs);
            }
        }
    }
}
