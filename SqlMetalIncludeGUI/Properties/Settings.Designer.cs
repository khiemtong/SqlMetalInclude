﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17020
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SqlMetalIncludeGUI.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Program Files\\Microsoft SDKs\\Windows\\v6.0A\\Bin\\SqlMetal.exe")]
        public string SqlMetalExe {
            get {
                return ((string)(this["SqlMetalExe"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ConfigDirectory {
            get {
                return ((string)(this["ConfigDirectory"]));
            }
            set {
                this["ConfigDirectory"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"namespace {0}
\{
    partial class {1}
    \{
        //Uncomment to give a default constructor to {1},
        //Assumes a app setting called SqlConnectionString.
        //public {1}() :
        //    this(Properties.Settings.SqlConnectionString)
        //\{ \}
    \}
\}")]
        public string NonDesignerCSharpFileWithNamespace {
            get {
                return ((string)(this["NonDesignerCSharpFileWithNamespace"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("partial class {0}\r\n\\{\r\n    //Uncomment to give a default constructor to {0},\r\n   " +
            " //Assumes a app setting called SqlConnectionString.\r\n    //public {0}() :\r\n    " +
            "//    this(Properties.Settings.SqlConnectionString)\r\n    //\\{ \\}\r\n\\}\r\n")]
        public string NonDesignerCSharpFileWithoutNamespace {
            get {
                return ((string)(this["NonDesignerCSharpFileWithoutNamespace"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Namespace {0}\r\n    Partial Class {1}\r\n\r\n    End Class\r\nEnd Namespace")]
        public string NonDesignerVbFileWithNamespace {
            get {
                return ((string)(this["NonDesignerVbFileWithNamespace"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Partial Class {0}\r\n\r\nEnd Class")]
        public string NonDesignerVbFileWithoutNamespace {
            get {
                return ((string)(this["NonDesignerVbFileWithoutNamespace"]));
            }
        }
    }
}