using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace FreqFind.Common.Extensions
{
    public static class UIAssemblies
    {
        /// <summary>
        /// This is to be executed when View cannot be found in currently loaded assemblies
        /// </summary>
        public static Action FallBackAction { get; set; }

        public static void LoadFromTypes(params Type[] types)
        {
            var assemblies = types.Select(x => x.Assembly);
            UpdateAssemblies(assemblies);
        }

        static void UpdateAssemblies(IEnumerable<Assembly> assemblies)
        {
            foreach (var uiAssembly in assemblies)
            {
                if (assemblyViewTypes.ContainsKey(uiAssembly)) continue;

                var viewTypes = uiAssembly.GetTypes()
                    .Where(x => x.Name.Contains("View") || x.Name.Contains("Control"))
                    .Where(x => x.IsSubclassOf(frameworkElementType))
                    .ToList();

                assemblyViewTypes[uiAssembly] = viewTypes;
            }
        }

        public static FrameworkElement ResolveView(Observable viewModel, bool fromFallback = false, bool skipViewModelPart = false, bool onlyWindows = false)
        {
            var start = DateTime.Now;

            var viewModelType = viewModel.GetType();
            var currentBaseType = viewModelType;
            var resolvedType = default(Type);
            if (!resolvedTypes.TryGetValue(currentBaseType, out resolvedType))
            {
                var viewModelBaseTypes = new List<Type>();
                while (skipViewModelPart || currentBaseType.Name.Contains("ViewModel"))
                {
                    viewModelBaseTypes.Add(currentBaseType);
                    if (currentBaseType.BaseType != null)
                        currentBaseType = currentBaseType.BaseType;
                    else
                        break;
                }
                if (viewModelBaseTypes.Count == 0) return null;


                //foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
                //{
                //    Debug.WriteLine(item.FullName);
                //}
                var uiAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                    .ToList();
                if (uiAssemblies.Count != assemblyViewTypes.Keys.Count)
                    UpdateAssemblies(uiAssemblies);

                var candidates = new List<Type>();
                var viewTypeNames = viewModelBaseTypes.Select(x => x.Name.Replace("Model", "")).ToList();
                if (skipViewModelPart)
                    viewTypeNames.AddRange(viewModelBaseTypes.Where(x => !x.Name.Contains("View")).Select(x => x.Name + "View"));
                viewTypeNames.AddRange(viewModelBaseTypes.Select(x => x.Name.Replace("ViewModel", "Control")));

                foreach (var pair in assemblyViewTypes)
                {
                    foreach (var type in pair.Value)
                    {
                        if (viewTypeNames.Any(x => x == type.Name))
                            candidates.Insert(0, type);
                        else if (viewTypeNames.Any(x => x.Contains(type.Name)))
                            candidates.Add(type);
                    }
                }

                if (onlyWindows)
                {
                    var windowType = typeof(Window);
                    candidates = candidates.Where(x => x.IsSubclassOf(windowType)).ToList();
                }

                if (candidates.Count == 0)
                {
                    if (FallBackAction != null && !fromFallback)
                    {
                        FallBackAction();
                        return ResolveView(viewModel, true, skipViewModelPart, onlyWindows);
                    }
                }



                var idealCandidateTypeName = viewModelType.Name.Replace("Model", "");
                var idealCandidate = candidates.Where(x => string.Equals(x.Name, idealCandidateTypeName)).FirstOrDefault();
                resolvedType = idealCandidate ?? candidates.FirstOrDefault();
                if (resolvedType != null)
                {
                    resolvedTypes[viewModelType] = resolvedType;
                }
            }

            if (resolvedType == null)
            {
                Debug.Print("Unable to find view for " + viewModelType.Name);
                return null;
            }


            var instance = (FrameworkElement)StaticReflection.FastCreateInstance(resolvedType);
            instance.DataContext = viewModel;

            Debug.Print("Resolved " + resolvedType.Name + " for " + viewModelType.Name + " in " + (DateTime.Now - start).TotalMilliseconds + " ms");
            return instance;
        }
        static Type frameworkElementType = typeof(FrameworkElement);
        static Dictionary<Assembly, List<Type>> assemblyViewTypes = new Dictionary<Assembly, List<Type>>();
        static Dictionary<Type, Type> resolvedTypes = new Dictionary<Type, Type>();

    }
}
