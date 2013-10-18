using System;
using MonoTouch.ObjCRuntime;

[assembly: LinkWith ("libMatrixTestLibSDK.a", LinkTarget.Simulator | LinkTarget.ArmV7, "-framework Accelerate", ForceLoad = true)]
