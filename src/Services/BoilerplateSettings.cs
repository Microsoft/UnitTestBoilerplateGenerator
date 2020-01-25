﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using Newtonsoft.Json;
using UnitTestBoilerplate.Model;
using UnitTestBoilerplate.Utilities;

namespace UnitTestBoilerplate.Services
{
    public class BoilerplateSettings : IBoilerplateSettings
    {
		private readonly IBoilerplateSettingsStore store;

		public BoilerplateSettings(IBoilerplateSettingsStore store)
	    {
			this.store = store;
		}

	    public TestFramework PreferredTestFramework
	    {
		    get
		    {
				string name = this.store.PreferredTestFrameworkName;
				return name != null ? TestFrameworks.Get(name) : null;
			}

		    set
		    {
				this.store.PreferredTestFrameworkName = value?.Name;
		    }
	    }

	    public MockFramework PreferredMockFramework
	    {
		    get
		    {
				string name = this.store.PreferredMockFrameworkName;
				return name != null ? MockFrameworks.Get(name) : null;
		    }

		    set
		    {
				this.store.PreferredMockFrameworkName = value?.Name;
		    }
	    }

	    public string FileNameTemplate
	    {
		    get
		    {
				string fileNameTemplate = this.store.FileNameTemplate;
				if (!string.IsNullOrWhiteSpace(fileNameTemplate))
				{
					return fileNameTemplate;
				}

			    return "$ClassName$Tests";
		    }

		    set
		    {
				this.store.FileNameTemplate = value;
		    }
	    }

	    public string GetTemplate(TestFramework testFramework, MockFramework mockFramework, TemplateType templateType)
	    {
			// First, check to see if we have a template in storage
		    string templateSettingKey = GetTemplateSettingsKey(testFramework, mockFramework, templateType);
			string template = this.store.GetTemplate(templateSettingKey);

		    if (template != null)
		    {
			    return template;
		    }

			// If not, give a proper default one
			return GetDefaultTemplate(testFramework, mockFramework, templateType);
	    }

		public static string GetDefaultTemplate(TestFramework testFramework, MockFramework mockFramework, TemplateType templateType)
		{
			switch (templateType)
			{
				case TemplateType.File:
					return new DefaultTemplateGenerator().Get(testFramework, mockFramework);
				case TemplateType.ExtraUsingNamespaces:
					return string.Empty;
				case TemplateType.MockFieldDeclaration:
					return mockFramework.MockFieldDeclarationCode;
				case TemplateType.MockFieldInitialization:
					return mockFramework.MockFieldInitializationCode;
				case TemplateType.MockObjectReference:
					return mockFramework.MockObjectReferenceCode;
				case TemplateType.TestMethodInvocation:
					return new DefaultTemplateGenerator().GetTestMethod(testFramework, mockFramework, invokeMethod: true);
				case TemplateType.TestMethodEmpty:
					return new DefaultTemplateGenerator().GetTestMethod(testFramework, mockFramework, invokeMethod: false);
				case TemplateType.TestMethodName:
					return DefaultTemplateGenerator.TestMethodName;
				default:
					throw new ArgumentOutOfRangeException(nameof(templateType), templateType, null);
			}
		}

	    public void SetTemplate(string testFrameworkString, string mockFrameworkString, string templateTypeString, string template)
	    {
			string settingsKey = GetTemplateSettingsKey(testFrameworkString, mockFrameworkString, templateTypeString);
			this.store.SetTemplate(settingsKey, template);
		}

	    public void RevertTemplateToDefault(TestFramework testFramework, MockFramework mockFramework)
	    {
			this.store.RevertTemplateToDefault(testFramework, mockFramework);
	    }

	    public static string GetTemplateSettingsKey(TestFramework testFramework, MockFramework mockFramework, TemplateType templateType)
	    {
		    return GetTemplateSettingsKey(testFramework.Name, mockFramework.Name, templateType.ToString());
	    }

		public static string GetTemplateSettingsKey(string testFrameworkString, string mockFrameworkString, string templateTypeString)
	    {
		    return $"{testFrameworkString}_{mockFrameworkString}_{templateTypeString}";
	    }

		public void Apply()
		{
			this.store.Apply();
		}
	}
}
