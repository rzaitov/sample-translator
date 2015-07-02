using System;
using System.Collections.Generic;
using System.Linq;

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Translator.Parser
{
	class ProjectParserListener : XcodeProjectBaseListener
	{
		Stack<Tuple<ParserRuleContext, IPBXElement>> stackOfObjects = new Stack<Tuple<ParserRuleContext, IPBXElement>> ();
		XcodeProject xcodeProject = new XcodeProject ();
		List<string> currentArray;

		public XcodeProject ProjectModel {
			get {
				return xcodeProject;
			}
		}

		public IPBXElement CurrentObject {
			get {
				return stackOfObjects.Where (c => c.Item2 != null).Select (c => c.Item2).FirstOrDefault ();
			}
			set {
				var topTuple = stackOfObjects.Pop ();
				stackOfObjects.Push (new Tuple<ParserRuleContext, IPBXElement> (topTuple.Item1, value));
			}
		}

		public override void EnterObject (XcodeProjectParser.ObjectContext context)
		{
			base.EnterObject (context);
			stackOfObjects.Push (new Tuple<ParserRuleContext, IPBXElement> (context, null));
		}

		public override void EnterArchive_version (XcodeProjectParser.Archive_versionContext context)
		{
			base.EnterArchive_version (context);
			xcodeProject.ArchiveVersion = GetTokenValue<Double> (context);
		}

		public override void EnterObject_version (XcodeProjectParser.Object_versionContext context)
		{
			base.EnterObject_version (context);
			xcodeProject.ObjectsVersion = GetTokenValue<Double> (context);
		}

		public override void EnterRoot_object (XcodeProjectParser.Root_objectContext context)
		{
			base.EnterRoot_object (context);
			xcodeProject.RootObject = GetTokenValue<string> (context);
		}

		public override void EnterIsa (XcodeProjectParser.IsaContext context)
		{
			base.EnterIsa (context);
			var isa = GetTokenValue<string> (context);
			CurrentObject = isa.CreatePBXObject ();
		}

		public override void EnterFile_ref (XcodeProjectParser.File_refContext context)
		{
			base.EnterFile_ref (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterLast_known_file_type (XcodeProjectParser.Last_known_file_typeContext context)
		{
			base.EnterLast_known_file_type (context);
			Extensions.SetPropertyByDescription<PBXFileType> (CurrentObject, context.GetText (), GetTokenValue<string> (context).GetEnumMemberByDescription<PBXFileType> ());
		}

		public override void EnterFile_encoding (XcodeProjectParser.File_encodingContext context)
		{
			base.EnterFile_encoding (context);
			Extensions.SetPropertyByDescription<PBXFileEncoding> (CurrentObject, context.GetText (), GetTokenValue<string> (context).GetEnumMemberByDescription<PBXFileEncoding> ());
		}

		public override void EnterProduct_name (XcodeProjectParser.Product_nameContext context)
		{
			base.EnterProduct_name (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterProduct_reference (XcodeProjectParser.Product_referenceContext context)
		{
			base.EnterProduct_reference (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterTarget (XcodeProjectParser.TargetContext context)
		{
			base.EnterTarget (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterTarget_proxy (XcodeProjectParser.Target_PROXYContext context)
		{
			base.EnterTarget_proxy (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterContainer_portal (XcodeProjectParser.Container_portalContext context)
		{
			base.EnterContainer_portal (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterProxy_type (XcodeProjectParser.Proxy_typeContext context)
		{
			base.EnterProxy_type (context);
			Extensions.SetPropertyByDescription<int> (CurrentObject, context.GetText (), GetTokenValue<int> (context));
		}

		public override void EnterRemote_info (XcodeProjectParser.Remote_infoContext context)
		{
			base.EnterRemote_info (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterRemote_global_id_string (XcodeProjectParser.Remote_global_id_stringContext context)
		{
			base.EnterRemote_global_id_string (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterProduct_type (XcodeProjectParser.Product_typeContext context)
		{
			base.EnterProduct_type (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterPath (XcodeProjectParser.PathContext context)
		{
			base.EnterPath (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterSource_tree (XcodeProjectParser.Source_treeContext context)
		{
			base.EnterSource_tree (context);
			Extensions.SetPropertyByDescription<PBXSourceTree> (CurrentObject, context.GetText (), GetTokenValue<string> (context).GetEnumMemberByDescription<PBXSourceTree> ());
		}

		public override void EnterBuild_action_mask (XcodeProjectParser.Build_action_maskContext context)
		{
			base.EnterBuild_action_mask (context);
			Extensions.SetPropertyByDescription<int> (CurrentObject, context.GetText (), GetTokenValue<int> (context));
		}

		public override void EnterRun_only_for_deployment_postprocessing (XcodeProjectParser.Run_only_for_deployment_postprocessingContext context)
		{
			base.EnterRun_only_for_deployment_postprocessing (context);
			Extensions.SetPropertyByDescription<int> (CurrentObject, context.GetText (), GetTokenValue<int> (context));
		}

		public override void EnterName (XcodeProjectParser.NameContext context)
		{
			base.EnterName (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterBuild_configuration_list (XcodeProjectParser.Build_configuration_listContext context)
		{
			base.EnterBuild_configuration_list (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterCompatibility_version (XcodeProjectParser.Compatibility_versionContext context)
		{
			base.EnterCompatibility_version (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterDevelopment_region (XcodeProjectParser.Development_regionContext context)
		{
			base.EnterDevelopment_region (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterHas_scanned_for_encodings (XcodeProjectParser.Has_scanned_for_encodingsContext context)
		{
			base.EnterHas_scanned_for_encodings (context);
			Extensions.SetPropertyByDescription<int> (CurrentObject, context.GetText (), GetTokenValue<int> (context));
		}

		public override void EnterMain_group (XcodeProjectParser.Main_groupContext context)
		{
			base.EnterMain_group (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterProject_dir_path (XcodeProjectParser.Project_dir_pathContext context)
		{
			base.EnterProject_dir_path (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterProject_roots (XcodeProjectParser.Project_rootsContext context)
		{
			base.EnterProject_roots (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterClang_enable_objc_arc (XcodeProjectParser.Clang_enable_objc_arcContext context)
		{
			base.EnterClang_enable_objc_arc (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterCode_sign_identity (XcodeProjectParser.Code_sign_identityContext context)
		{
			base.EnterCode_sign_identity (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterGcc_warn_about_initializer_not_fully_bracketed (XcodeProjectParser.Gcc_warn_about_initializer_not_fully_bracketedContext context)
		{
			base.EnterGcc_warn_about_initializer_not_fully_bracketed (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterGcc_warn_about_return_type (XcodeProjectParser.Gcc_warn_about_return_typeContext context)
		{
			base.EnterGcc_warn_about_return_type (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterGcc_warn_about_missing_field_initializers (XcodeProjectParser.Gcc_warn_about_missing_field_initializersContext context)
		{
			base.EnterGcc_warn_about_missing_field_initializers (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterGcc_warn_shadow (XcodeProjectParser.Gcc_warn_shadowContext context)
		{
			base.EnterGcc_warn_shadow (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterGcc_warn_sign_compare (XcodeProjectParser.Gcc_warn_sign_compareContext context)
		{
			base.EnterGcc_warn_sign_compare (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterGcc_warn_undeclared_selector (XcodeProjectParser.Gcc_warn_undeclared_selectorContext context)
		{
			base.EnterGcc_warn_undeclared_selector (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterGcc_warn_unused_function (XcodeProjectParser.Gcc_warn_unused_functionContext context)
		{
			base.EnterGcc_warn_unused_function (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterGcc_warn_unused_label (XcodeProjectParser.Gcc_warn_unused_labelContext context)
		{
			base.EnterGcc_warn_unused_label (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterGcc_warn_unused_variable (XcodeProjectParser.Gcc_warn_unused_variableContext context)
		{
			base.EnterGcc_warn_unused_variable (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterIphoneos_deployment_target (XcodeProjectParser.Iphoneos_deployment_targetContext context)
		{
			base.EnterIphoneos_deployment_target (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterProvisioning_profile (XcodeProjectParser.Provisioning_profileContext context)
		{
			base.EnterProvisioning_profile (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterRun_clang_static_analyzer (XcodeProjectParser.Run_clang_static_analyzerContext context)
		{
			base.EnterRun_clang_static_analyzer (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterDefault_configuration_is_visible (XcodeProjectParser.Default_configuration_is_visibleContext context)
		{
			base.EnterDefault_configuration_is_visible (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterDefault_configuration_name (XcodeProjectParser.Default_configuration_nameContext context)
		{
			base.EnterDefault_configuration_name (context);
			Extensions.SetPropertyByDescription<string> (CurrentObject, context.GetText (), GetTokenValue<string> (context));
		}

		public override void EnterArray (XcodeProjectParser.ArrayContext context)
		{
			base.EnterArray (context);
			currentArray = new List<string> ();
		}

		public override void EnterArray_item (XcodeProjectParser.Array_itemContext context)
		{
			base.EnterArray_item (context);
			currentArray.Add (context.GetChild (0).GetText ());
		}

		public override void ExitArray (XcodeProjectParser.ArrayContext context)
		{
			base.ExitArray (context);
			Extensions.SetPropertyByDescription<IList<string>> (CurrentObject,
				GetTokenValue<string> (context, false), new List<string> (currentArray));

			if (currentArray != null) {
				currentArray.Clear ();
				currentArray = null;
			}
		}

		public override void ExitObject (XcodeProjectParser.ObjectContext context)
		{
			base.ExitObject (context);

			var currentElement = stackOfObjects.Pop ();
			if (currentElement.Item2 != null) {
				currentElement.Item2.ID = GetTokenValue<string> (context, false);
				xcodeProject.Objects.Add (currentElement.Item2);
				Console.WriteLine ("{0} {1}", currentElement.Item2.ObjectType.ToString (), currentElement.Item2.ID);
			}
		}

		T GetTokenValue<T> (RuleContext context, bool getRightPartOfAssignment = true)
		{
			var assignmentValue = GetPartOfAssignment (context, getRightPartOfAssignment);
			var tokenText = assignmentValue.GetText ();
			return tokenText.ConvertString<T> ();
		}

		IParseTree GetPartOfAssignment (RuleContext context, bool right)
		{
			var assignmentToken = context.Parent.Parent;
			var rightSideOfAssignemnt = assignmentToken.GetChild (right ? 2 : 0);
			return rightSideOfAssignemnt.GetChild (0);
		}
	}
}
