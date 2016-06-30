using System;
using System.Windows;

namespace Csizmazia.WpfDynamicUI.WpfHost
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public class App : Application
    {
        public App()
        {
            AppDomain domain = AppDomain.CurrentDomain;
            AppDomainSetup setup = domain.SetupInformation;
            setup.PartialTrustVisibleAssemblies = new[]
                                                      {
                                                          "System.ComponentModel.DataAnnotations," +
                                                          "PublicKey=0024000004800000940000000602000000240000525341310004000001000100B5FC90E7027F" +
                                                          "73A8FDE8938C81DD402BA65B9201D60593E96C492651E889CC13F1415EBB53FAC1131AE0BD333C5EE602" +
                                                          "9718EA31A8AEBD0DA0072F25D87DBA6FC90FFD598ED4DA35E44C398C454307E8E33B8426143DAEC9F59683" +
                                                          "8F74750E5975C64E2189F45DEF46B2A2B1247ADC3652BF5C308055DA9"
                                                      };
        }
    }
}