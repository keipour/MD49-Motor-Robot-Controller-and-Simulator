﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.296
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RobX.Controller.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("127.0.0.1")]
        public string SimulatorIP {
            get {
                return ((string)(this["SimulatorIP"]));
            }
            set {
                this["SimulatorIP"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("172.20.62.204")]
        public string RealIP {
            get {
                return ((string)(this["RealIP"]));
            }
            set {
                this["RealIP"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1371")]
        public string SimulatorPort {
            get {
                return ((string)(this["SimulatorPort"]));
            }
            set {
                this["SimulatorPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1371")]
        public string RealPort {
            get {
                return ((string)(this["RealPort"]));
            }
            set {
                this["RealPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int RobotType {
            get {
                return ((int)(this["RobotType"]));
            }
            set {
                this["RobotType"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Up")]
        public global::System.Windows.Forms.Keys ForwardKey {
            get {
                return ((global::System.Windows.Forms.Keys)(this["ForwardKey"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Down")]
        public global::System.Windows.Forms.Keys BackwardKey {
            get {
                return ((global::System.Windows.Forms.Keys)(this["BackwardKey"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Right")]
        public global::System.Windows.Forms.Keys RotateClockwiseKey {
            get {
                return ((global::System.Windows.Forms.Keys)(this["RotateClockwiseKey"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Left")]
        public global::System.Windows.Forms.Keys RotateCounterClockwiseKey {
            get {
                return ((global::System.Windows.Forms.Keys)(this["RotateCounterClockwiseKey"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ShiftKey")]
        public global::System.Windows.Forms.Keys StopKey {
            get {
                return ((global::System.Windows.Forms.Keys)(this["StopKey"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("480, 561")]
        public global::System.Drawing.Size FormSize {
            get {
                return ((global::System.Drawing.Size)(this["FormSize"]));
            }
            set {
                this["FormSize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0, 0")]
        public global::System.Drawing.Point FormPosition {
            get {
                return ((global::System.Drawing.Point)(this["FormPosition"]));
            }
            set {
                this["FormPosition"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public double SimulationSpeed {
            get {
                return ((double)(this["SimulationSpeed"]));
            }
            set {
                this["SimulationSpeed"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("F10")]
        public global::System.Windows.Forms.Keys GlobalStopKey {
            get {
                return ((global::System.Windows.Forms.Keys)(this["GlobalStopKey"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("F5")]
        public global::System.Windows.Forms.Keys StartKey {
            get {
                return ((global::System.Windows.Forms.Keys)(this["StartKey"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("F12")]
        public global::System.Windows.Forms.Keys ToggleKeyboardControl {
            get {
                return ((global::System.Windows.Forms.Keys)(this["ToggleKeyboardControl"]));
            }
        }
    }
}