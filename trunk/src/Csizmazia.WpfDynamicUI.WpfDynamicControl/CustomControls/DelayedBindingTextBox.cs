using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.CustomControls
{
    /// <summary>
    /// Represents a TextBox whose Text Binding will get updated after a specified interval when the user stops entering text
    ///  </summary>
    /// <remarks>http://www.codeproject.com/Articles/42767/Text-Box-with-Delayed-Binding</remarks>
    public class DelayedBindingTextBox : TextBox
    {
        public static readonly DependencyProperty DelayTimeProperty =
            DependencyProperty.Register("DelayTime", typeof (int), typeof (DelayedBindingTextBox),
                                        new UIPropertyMetadata(667));

        private Timer _timer;

        /// <summary>
        /// Gets and Sets the amount of time to wait after the text has changed before updating the binding
        /// </summary>
        public int DelayTime
        {
            get { return (int) GetValue(DelayTimeProperty); }
            set { SetValue(DelayTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DelayTime.  This enables animation, styling, binding, etc...


        //override this to update the source if we get an enter or return
        protected override void OnKeyDown(KeyEventArgs e)
        {
            //we dont update the source if we accept enter
            if (AcceptsReturn)
            {
            }
                //update the binding if enter or return is pressed
            else if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                //get the binding
                BindingExpression bindingExpression = GetBindingExpression(TextProperty);

                //if the binding is valid update it
                if (BindingCanProceed(bindingExpression))
                {
                    //update the source
                    bindingExpression.UpdateSource();
                }
            }
            base.OnKeyDown(e);
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            //get the binding
            BindingExpression bindingExpression = GetBindingExpression(TextProperty);

            if (BindingCanProceed(bindingExpression))
            {
                //get rid of the timer if it exists
                if (_timer != null)
                {
                    //dispose of the timer so that it wont get called again
                    _timer.Dispose();
                }

                //recreate the timer everytime the text changes
                _timer = new Timer(o =>
                                       {
                                           //create a delegate method to do the binding update on the main thread
                                           Method updateSourceAction = delegate
                                                                           {
                                                                               //update the binding
                                                                               bindingExpression.UpdateSource();
                                                                           };

                                           //need to check if the binding is still valid, as this is a threaded timer the text box may have been unloaded etc.
                                           if (BindingCanProceed(bindingExpression))
                                           {
                                               //invoke the delegate to update the binding source on the main (ui) thread
                                               Dispatcher.Invoke(updateSourceAction, new object[] {});
                                           }
                                           //dispose of the timer so that it wont get called again
                                           _timer.Dispose();
                                       }, null, DelayTime, Timeout.Infinite);
            }

            base.OnTextChanged(e);
        }

        //makes sure a binding can proceed
        private static bool BindingCanProceed(BindingExpression bindingExpression)
        {
            Boolean canProceed = false;

            //cant update if there is no BindingExpression
            if (bindingExpression == null)
            {
            }
                //cant update if we have no data item
            else if (bindingExpression.DataItem == null)
            {
            }
                //cant update if the binding is not active
            else if (bindingExpression.Status != BindingStatus.Active)
            {
            }
                //cant update if the parent binding is null
            else if (bindingExpression.ParentBinding == null)
            {
            }
                //dont need to update if the UpdateSourceTrigger is set to update every time the property changes
            else if (bindingExpression.ParentBinding.UpdateSourceTrigger == UpdateSourceTrigger.PropertyChanged)
            {
            }
                //we can proceed
            else
            {
                canProceed = true;
            }

            return canProceed;
        }

        #region Nested type: Method

        private delegate void Method();

        #endregion
    }
}