using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Reflection;
using Csizmazia.Collections;
using Csizmazia.Discovering;
using Csizmazia.WpfDynamicUI.BusinessModel;
using LinqKit;

namespace Csizmazia.WpfDynamicUI.DataServices
{
    internal static class CodeGenExtensions
    {
        private static readonly string @Namespace = typeof (CodeGen).Namespace + ".CodeGen";


        public static void AddReferences(this CodeCompileUnit codeCompileUnit)
        {
            //add references
            codeCompileUnit.ReferencedAssemblies.Add(typeof (int).Assembly.Location);
            codeCompileUnit.ReferencedAssemblies.Add(typeof (INotifyPropertyChanged).Assembly.Location); //System.dll
            codeCompileUnit.ReferencedAssemblies.Add(typeof (DbContext).Assembly.Location);
            codeCompileUnit.ReferencedAssemblies.Add(typeof (CodeGen).Assembly.Location);
            codeCompileUnit.ReferencedAssemblies.Add(typeof (BusinessApplication).Assembly.Location);
            codeCompileUnit.ReferencedAssemblies.Add(typeof (DisplayAttribute).Assembly.Location);
            codeCompileUnit.ReferencedAssemblies.Add(typeof (Expression<>).Assembly.Location);
            codeCompileUnit.ReferencedAssemblies.Add(typeof (Func<>).Assembly.Location);
            codeCompileUnit.ReferencedAssemblies.Add(typeof (Linq).Assembly.Location);
        }

        public static void AddReference(this CodeCompileUnit codeCompileUnit, Type entityType)
        {
            if (!codeCompileUnit.ReferencedAssemblies.Contains(entityType.Assembly.Location))
                codeCompileUnit.ReferencedAssemblies.Add(entityType.Assembly.Location);
        }

        public static CodeNamespace DefineNamespace(this CodeCompileUnit codeCompileUnit)
        {
            //Create the namespace
            string namespaceName = @Namespace;
            var codeNamespace = new CodeNamespace(namespaceName);

            codeNamespace.Imports.Add(new CodeNamespaceImport("LinqKit"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("System.Linq"));

            codeCompileUnit.Namespaces.Add(codeNamespace);

            return codeNamespace;
        }

        public static CodeTypeDeclaration DefineClass(this CodeNamespace codeNamespace, Type entityType,
                                                      string namePostFix)
        {
            string typeName = string.Format("{0}{1}", entityType.Name.Replace('.', '_'), namePostFix);

            var codeTypeDeclaration = new CodeTypeDeclaration(typeName)
                                          {
                                              IsClass = true
                                          };


            codeNamespace.Types.Add(codeTypeDeclaration);

            return codeTypeDeclaration;
        }

        public static CodeAttributeDeclaration DefineDisplayAttribute(this CodeMemberProperty codeMemberProperty,
                                                                      bool? autoGenerateField, bool? autoGenerateFilter)
        {
            //argument list
            var arguments = new List<CodeAttributeArgument>();


            if (autoGenerateFilter != null)
                arguments.Add(new CodeAttributeArgument("AutoGenerateFilter",
                                                        new CodePrimitiveExpression(autoGenerateFilter.Value)));
            if (autoGenerateField != null)
                arguments.Add(new CodeAttributeArgument("AutoGenerateField",
                                                        new CodePrimitiveExpression(autoGenerateField.Value)));


            var attributeDeclaration =
                new CodeAttributeDeclaration("System.ComponentModel.DataAnnotations.DisplayAttribute",
                                             arguments.ToArray());

            codeMemberProperty.CustomAttributes.Add(attributeDeclaration);

            return attributeDeclaration;
        }

        public static CodeAttributeDeclaration DefineUIHintAttribute(this CodeMemberProperty codeMemberProperty,
                                                                     string uiHint, params object[] controlParameters)
        {
            //argument list
            var arguments = new List<CodeAttributeArgument>();


            arguments.Add(new CodeAttributeArgument("uiHint", new CodePrimitiveExpression(uiHint)));
            arguments.Add(new CodeAttributeArgument("presentationLayer", new CodePrimitiveExpression(uiHint)));
            arguments.Add(new CodeAttributeArgument("controlParameters", new CodePrimitiveExpression(controlParameters)));


            var attributeDeclaration =
                new CodeAttributeDeclaration("System.ComponentModel.DataAnnotations.UIHintAttribute",
                                             arguments.ToArray());

            codeMemberProperty.CustomAttributes.Add(attributeDeclaration);

            return attributeDeclaration;
        }

        public static CodeTypeDeclaration DefineProperties(this CodeTypeDeclaration codeTypeDeclaration, Type entityType)
        {
            codeTypeDeclaration.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Properties"));


            foreach (PropertyInfo propertyInfo in entityType.GetInstanceProperties())
            {
                codeTypeDeclaration.DefineProperty(propertyInfo);
            }

            codeTypeDeclaration.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, string.Empty));
            return codeTypeDeclaration;
        }

        public static CodeTypeDeclaration DefineProperty(this CodeTypeDeclaration codeTypeDeclaration,
                                                         PropertyInfo propertyInfo)
        {
            codeTypeDeclaration.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start,
                                                                            string.Format("{0}", propertyInfo.Name)));

            CodeFieldReferenceExpression field = propertyInfo.DefineField(codeTypeDeclaration);

            CodePropertyReferenceExpression property = propertyInfo.DefineProperty(codeTypeDeclaration, field);


            codeTypeDeclaration.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, string.Empty));

            return codeTypeDeclaration;
        }

        public static CodeFieldReferenceExpression DefineField(this PropertyInfo propertyInfo,
                                                               CodeTypeDeclaration codeTypeDeclaration,
                                                               string fieldNamePrefix = "", string fieldNamePostfix = "")
        {
            string fieldName = string.Format("_{0}{1}{2}", fieldNamePrefix, propertyInfo.Name, fieldNamePostfix);

            return propertyInfo.PropertyType.DefineField(codeTypeDeclaration, fieldName);
        }

        public static CodeFieldReferenceExpression DefineField(this Type fieldType,
                                                               CodeTypeDeclaration codeTypeDeclaration, string fieldName)
        {
            //var fieldName = string.Format("_{0}{1}{2}", fieldNamePrefix, propertyInfo.Name, fieldNamePostfix);
            //var fieldType = propertyInfo.PropertyType.FullName;

            //generate field 
            var clsMember = new CodeMemberField
                                {
                                    Name = string.Format(fieldName),
                                    Attributes = MemberAttributes.Private,
                                    Type = new CodeTypeReference(fieldType)
                                };

            codeTypeDeclaration.Members.Add(clsMember);


            var codeFieldReferenceExpression = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
                                                                                fieldName);

            return codeFieldReferenceExpression;
        }

        public static CodePropertyReferenceExpression DefineProperty(this PropertyInfo propertyInfo,
                                                                     CodeTypeDeclaration codeTypeDeclaration,
                                                                     CodeFieldReferenceExpression
                                                                         codeFieldReferenceExpression,
                                                                     string propertyNamePrefix = "",
                                                                     string propertyNamePostfix = "",
                                                                     bool? autoGenerateField = null,
                                                                     bool? autoGenerateFilter = null)
        {
            string propertyName = string.Format("{0}{1}", propertyNamePrefix, propertyInfo.Name);

            return propertyInfo.PropertyType.DefineProperty(codeTypeDeclaration, codeFieldReferenceExpression,
                                                            propertyName, autoGenerateField, autoGenerateFilter);
        }

        public static CodePropertyReferenceExpression DefineProperty(this Type fieldType,
                                                                     CodeTypeDeclaration codeTypeDeclaration,
                                                                     CodeFieldReferenceExpression
                                                                         codeFieldReferenceExpression,
                                                                     string propertyName,
                                                                     bool? autoGenerateField = null,
                                                                     bool? autoGenerateFilter = null,
                                                                     Action<CodeMemberProperty> customization = null)
        {
            //var propertyName = string.Format("{0}{1}", propertyNamePrefix, propertyInfo.Name);
            //var fieldType = propertyInfo.PropertyType.FullName;

            //generate property getter and setter
            var property = new CodeMemberProperty
                               {
                                   Name = string.Format(propertyName),
                                   Type = new CodeTypeReference(fieldType),
                                   Attributes = MemberAttributes.Public
                               };

            //Add [Display] Attribute
            property.DefineDisplayAttribute(autoGenerateField, autoGenerateFilter);

            var codePropertySetValueReferenceExpression = new CodePropertySetValueReferenceExpression();

            #region getter

            property.GetStatements.Add(new CodeMethodReturnStatement(codeFieldReferenceExpression));

            #endregion

            #region setter

            //if (_PropValue == value) return;
            property.SetStatements.Add(
                new CodeConditionStatement(new CodeBinaryOperatorExpression(codeFieldReferenceExpression,
                                                                            CodeBinaryOperatorType.ValueEquality,
                                                                            codePropertySetValueReferenceExpression),
                                           new CodeMethodReturnStatement()));


            var beforeVariableDeclaration = new CodeVariableDeclarationStatement(typeof (object), "before");
            var beforeVariableReference = new CodeVariableReferenceExpression("before");

            //object before;
            property.SetStatements.Add(beforeVariableDeclaration);

            //before = _PropValue;
            property.SetStatements.Add(new CodeAssignStatement(beforeVariableReference,
                                                               codeFieldReferenceExpression));

            //_PropValue = value;
            property.SetStatements.Add(new CodeAssignStatement(codeFieldReferenceExpression,
                                                               codePropertySetValueReferenceExpression));


            var onPropertyChangedMethodReference =
                new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "OnPropertyChanged");

            //OnPropertyChanged("PropertyName",before,value)
            property.SetStatements.Add(new CodeMethodInvokeExpression(onPropertyChangedMethodReference,
                                                                      new CodePrimitiveExpression(propertyName),
                                                                      beforeVariableReference,
                                                                      codePropertySetValueReferenceExpression));

            #endregion

            //customize definition...
            if (customization != null)
                customization(property);


            var codePropertyReferenceExpression = new CodePropertyReferenceExpression(
                new CodeThisReferenceExpression(), propertyName);

            return codePropertyReferenceExpression;
        }


        public static CodePropertyReferenceExpression DefineLookupProperty(this PropertyInfo propertyInfo,
                                                                           CodeTypeDeclaration codeTypeDeclaration,
                                                                           bool? autoGenerateField = null,
                                                                           bool? autoGenerateFilter = null)
        {
            Type propertyType = typeof (TweakedObservableCollection<>).MakeGenericType(propertyInfo.PropertyType);
            string fieldName = string.Format("_For{0}List", propertyInfo.Name);
            string propertyName = string.Format("For{0}List", propertyInfo.Name);

            CodeFieldReferenceExpression field = propertyType.DefineField(codeTypeDeclaration, fieldName);

            CodePropertyReferenceExpression property = propertyType.DefineProperty(codeTypeDeclaration, field,
                                                                                   propertyName, autoGenerateField,
                                                                                   autoGenerateFilter,
                                                                                   p =>
                                                                                   p.DefineUIHintAttribute(
                                                                                       UIHints.DisplayComboBox,
                                                                                       UIHints.ItemsControlParameters.
                                                                                           SelectedItemBindingPath,
                                                                                       propertyInfo.Name));


            return property;
        }

        public static CodeMethodReferenceExpression DefineLookupPropertyInit(this PropertyInfo propertyInfo,
                                                                             CodeTypeDeclaration codeTypeDeclaration,
                                                                             CodePropertyReferenceExpression
                                                                                 lookupPropertyReference)
        {
            string methodName = "For{0}ListInit";

            //public static [Entities name](StartupModel para){}
            var codeMemberMethod = new CodeMemberMethod
                                       {
                                           ReturnType = new CodeTypeReference(typeof (void)),
                                           Name = methodName,
                                           Attributes = MemberAttributes.Family,
                                       };


            //For{0}List.Clear();
            //var query = from e in Context.SelectAll<T>() select e;
            //For{0}List.AddRange(query.ToList());

            /*
            codeMemberMethod.Statements.Add(
                new CodeSnippetExpression(
                    string.Format(
                        "_query = new Csizmazia.WpfDynamicUI.Collections.PagedQueryable<{0}>(Repository.SelectAll<{0}>().OrderBy(e=>e.{1}))",
                        entityType.FullName, entityType.GetInstanceProperties().FirstOrDefault().Name)));
            */

            codeTypeDeclaration.Members.Add(codeMemberMethod);

            var codeMethodReferenceExpression = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(),
                                                                                  methodName);

            return codeMethodReferenceExpression;
        }
    }
}