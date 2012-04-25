using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Phone.Common.Behaviors
{
    namespace Phone.Framework.Behaviors
    {
        public class UpdateOnTextChangedBehavior : Behavior<TextBox>
        {
            protected override void OnAttached()
            {
                base.OnAttached();

                this.AssociatedObject.TextChanged += AssociatedObject_TextChanged;
            }

            void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
            {
                var binding = this.AssociatedObject.GetBindingExpression(TextBox.TextProperty);
                if (binding != null)
                {
                    binding.UpdateSource();
                }
            }

            protected override void OnDetaching()
            {
                base.OnDetaching();

                this.AssociatedObject.TextChanged -= AssociatedObject_TextChanged;
            }
        }
    }

}
