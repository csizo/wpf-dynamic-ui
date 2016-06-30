using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Csizmazia.WpfDynamicEntity.Data;
using Csizmazia.WpfDynamicUI.DataServices;

namespace Csizmazia.WpfDynamicEntity.ApplicationHost
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {

            var codeGen = new Csizmazia.WpfDynamicUI.DataServices.CodeGen();

            codeGen.GenerateDynamicListModelType<NorthwindEntities>(new[]
                                                                 {
                                                                     typeof (Categories), typeof (Customers),
                                                                     typeof (Employees), typeof(Products), typeof (Orders), typeof (Shippers)
                                                                     , typeof (Suppliers)
                                                                 });
            codeGen.GenerateDynamicListModelType<NorthwindEntities, Categories>();


            var dynamicAssembly = codeGen.BuildTemporaryAssembly();

            //codeGen.BuildTemporaryAssembly();
            //codeGen.BuildAssembly(@"c:\temp\dynamic.dll");
            codeGen.BuildSourceCode(@"c:\temp\dynamic.cs");

            dynamicAssembly.RegisterInMefContainer();


            WpfDynamicUI.MefComposition.Container.Instance.RegisterAssembly(typeof(MyEditor).Assembly);




            base.OnStartup(e);


        }
    }

    public class MyEditor : EntityDetail<NorthwindEntities, Categories>
    {

    }
}
