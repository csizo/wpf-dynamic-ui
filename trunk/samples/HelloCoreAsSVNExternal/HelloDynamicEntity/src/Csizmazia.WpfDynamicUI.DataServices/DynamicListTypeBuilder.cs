using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using Csizmazia.Discovering;

namespace Csizmazia.WpfDynamicUI.DataServices
{
    public class DynamicListTypeBuilder
    {
        /// <summary>
        /// types for filter property is generated
        /// </summary>
        private static readonly IEnumerable<Type> FilterPropertyTypes = new[]
                                                                            {
                                                                                typeof (string), typeof (int),
                                                                                typeof (int?), typeof (bool),
                                                                                typeof (bool?), typeof (byte),
                                                                                typeof (byte?), typeof (DateTime),
                                                                                typeof (DateTime?)
                                                                            };

        private readonly List<Type> ProcessedTypes = new List<Type>();
        private readonly CodeGen _codeGen;

        public DynamicListTypeBuilder(CodeGen codeGen)
        {
            _codeGen = codeGen;
        }


        public void BuildDynamicListModel<TContext, TEntity>()
            where TContext : DbContext, new()
            where TEntity : class
        {
            Type entityType = typeof (TEntity);


            BuildDynamicListModel<TContext>(entityType);
        }

        public void BuildDynamicListModel<TContext>(Type entityType)
            where TContext : DbContext, new()
        {
            if (ProcessedTypes.Contains(entityType))
            {
                //logger warn skipping already added entity
                //return DynamicTypes[entityType];
                return;
            }


            GenerateDynamicListModel(_codeGen.CodeNamespace, typeof (TContext), entityType);

            ProcessedTypes.Add(entityType);
        }


        internal static void GenerateDynamicListModel(CodeNamespace codeNamespace, Type contextType, Type entityType)
        {
            //Create the Dynamic class
            CodeTypeDeclaration genClass = codeNamespace.DefineClass(entityType, "EntityListModel");

            //add base type declaration
            Type baseType = typeof (EntityListModel<,>).MakeGenericType(contextType, entityType);
            genClass.BaseTypes.Add(new CodeTypeReference(baseType));


            genClass.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Codegen Region"));

            GenerateQueryProviderMember(genClass, entityType);

            GenerateEntryStatisEntryPoint(genClass, entityType);

            GenerateFilterPropertyMembers(genClass, entityType);

            GenerateApplyFilterMethod(genClass, entityType);

            genClass.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, string.Empty));
        }


        private static void GenerateEntryStatisEntryPoint(CodeTypeDeclaration genClass, Type entityType)
        {
            //public static [Entities name](StartupModel para){}
            var codeMemberMethod = new CodeMemberMethod
                                       {
                                           ReturnType = new CodeTypeReference(typeof (void)),
                                           Name = entityType.Name,
                                           Attributes = MemberAttributes.Public | MemberAttributes.Static,
                                       };

            codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof (StartupModel), "para"));

            codeMemberMethod.Statements.Add(
                new CodeSnippetExpression(
                    string.Format(
                        "WpfDynamicUI.BusinessModel.BusinessApplication.Instance.OpenModel(()=>new {0}EntityListModel())",
                        entityType.Name)));


            genClass.Members.Add(codeMemberMethod);
        }

        private static void GenerateQueryProviderMember(CodeTypeDeclaration genClass, Type entityType)
        {
            //public static [Entities name](StartupModel para){}
            var codeMemberMethod = new CodeMemberMethod
                                       {
                                           ReturnType = new CodeTypeReference(typeof (void)),
                                           Name = "ProvideQuery",
                                           Attributes = MemberAttributes.Family | MemberAttributes.Override,
                                       };


            codeMemberMethod.Statements.Add(
                new CodeSnippetExpression(
                    string.Format(
                        "_query = new Csizmazia.WpfDynamicUI.Collections.PagedQueryable<{0}>(Repository.SelectAll<{0}>().OrderBy(e=>e.{1}))",
                        entityType.FullName, entityType.GetInstanceProperties().FirstOrDefault().Name)));


            genClass.Members.Add(codeMemberMethod);
        }

        private static void GenerateFilterPropertyMembers(CodeTypeDeclaration genClass, Type entityType)
        {
            foreach (
                PropertyInfo pi in
                    entityType.GetInstanceProperties().Where(pi => FilterPropertyTypes.Contains(pi.PropertyType)))
            {
                string fieldName = string.Format("_Filter{0}", pi.Name);
                string propertyName = string.Format("Filter{0}", pi.Name);
                bool isValueType = pi.PropertyType.IsValueType;

                bool isNullable = (pi.PropertyType.IsGenericType &&
                                   pi.PropertyType.GetGenericTypeDefinition() == typeof (Nullable<>));

                string fieldType = isValueType && !isNullable
                                       ? string.Format("System.Nullable<{0}>", pi.PropertyType)
                                       : pi.PropertyType.FullName;

                #region field

                //generate field 
                var clsMember = new CodeMemberField
                                    {
                                        Name = string.Format(fieldName),
                                        Attributes = MemberAttributes.Private,
                                        Type = new CodeTypeReference(fieldType)
                                    };

                genClass.Members.Add(clsMember);

                #endregion

                #region Property

                //generate property getter and setter
                var property = new CodeMemberProperty
                                   {
                                       Name = string.Format(propertyName),
                                       Type = new CodeTypeReference(fieldType),
                                       Attributes = MemberAttributes.Public
                                   };

                //Add [Display] Attribute
                property.DefineDisplayAttribute(null, true);

                var codeFieldReferenceExpression = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
                                                                                    fieldName);
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

                genClass.Members.Add(property);

                #endregion
            }
        }

        private static void GenerateApplyFilterMethod(CodeTypeDeclaration genClass, Type entityType)
        {
            var method = new CodeMemberMethod
                             {
                                 ReturnType = new CodeTypeReference(typeof (void)),
                                 Name = "ApplyFilter",
                                 Attributes = MemberAttributes.Override | MemberAttributes.Family,
                             };


            var codeThisReferenceExpression = new CodeThisReferenceExpression();

            //check for QueryNull
            //if(Query==null) return;
            method.Statements.Add(
                new CodeConditionStatement(
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(codeThisReferenceExpression, "Query"),
                        CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(null)),
                    new CodeMethodReturnStatement()));


            //Expression<Func<T,bool>> condition;
            method.Statements.Add(
                new CodeVariableDeclarationStatement(
                    string.Format("System.Linq.Expressions.Expression<System.Func<{0},bool>>", entityType.FullName),
                    "condition"));

            //condition = c=>true;
            method.Statements.Add(new CodeSnippetExpression("condition = c=> true"));

            var conditionVariableReference = new CodeVariableReferenceExpression("condition");

            foreach (
                PropertyInfo pi in
                    entityType.GetInstanceProperties().Where(pi => FilterPropertyTypes.Contains(pi.PropertyType)))
            {
                string propertyName = string.Format("Filter{0}", pi.Name);
                bool isValueType = pi.PropertyType.IsValueType;

                var codePropertyReferenceExpression =
                    new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), propertyName);

                if (pi.PropertyType == typeof (string))
                {
                    var stament =
                        new CodeSnippetExpression(
                            string.Format(
                                "if (!string.IsNullOrEmpty({1})){{ condition = condition.And(c=>c.{0}.Contains({1})); }};",
                                pi.Name, propertyName));

                    method.Statements.Add(stament);
                }
                else
                {
                    method.Statements.Add(
                        new CodeConditionStatement(
                            new CodeBinaryOperatorExpression(codePropertyReferenceExpression,
                                                             CodeBinaryOperatorType.IdentityInequality,
                                                             new CodePrimitiveExpression(null)),
                            new CodeSnippetStatement(string.Format("condition=condition.And(c=>c.{0} == {1});", pi.Name,
                                                                   propertyName))
                            ));
                }
            }

            //assign filter expression to _query.Filter
            var queryFieldReference = new CodeFieldReferenceExpression(new CodeBaseReferenceExpression(),
                                                                       "_query.Filter");
            method.Statements.Add(new CodeAssignStatement(queryFieldReference, conditionVariableReference));


            genClass.Members.Add(method);
        }
    }
}