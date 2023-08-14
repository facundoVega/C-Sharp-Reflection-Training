using System;
using System.Reflection;
using System.IO;
using System.ComponentModel.Design;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        const string InformationAttributeTypeName = "UTILITYFUNCTIONS.INFORMATIONATTRIBUTE";

        static void Main(string[] args)
        {
            const string TargetAssemblyFileName = "UtilityFunctions.dll";
            const string TargetNameSpace = "UtilityFunctions";

            Assembly assembly = Assembly.LoadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TargetAssemblyFileName));

            List<Type> classes = assembly.GetTypes()
                .Where(t => t.Namespace == TargetNameSpace && HasInformationAttribute(t)).ToList();
            
            Console.WriteLine("Please press the number key asssociated with " + "the class you wish to test");

            DisplayProgramElementList(classes);

            Type typeChoice = ReturnProgramElementReferenceFromList(classes);

            object classInstance = Activator.CreateInstance(typeChoice, null);

            Console.Clear();

            WriteHeadingToScreen($"Class: '{ typeChoice}'");

            DisplayElementDescription(ReturnInformatioCustomAttributeDescription(typeChoice));


            WritePromptToScreen("Please enter the number associated with the method you wish to test");

            List<MethodInfo> methods = typeChoice.GetMethods().Where(m => HasInformationAttribute(m)).ToList();

            DisplayProgramElementList(methods);

            MethodInfo methodChoice = ReturnProgramElementReferenceFromList(methods);

            if ( methodChoice != null)
            {
                Console.Clear();
                WriteHeadingToScreen($"Class: '{typeChoice}' - Method: '{ methodChoice.Name }'");
                DisplayElementDescription(ReturnInformatioCustomAttributeDescription(methodChoice));

                ParameterInfo[] parameters = methodChoice.GetParameters();

                object result = GetResult(classInstance, methodChoice, parameters);

                WriteResultToScreen(result);
            }
        }

        private static string ReturnInformatioCustomAttributeDescription(MemberInfo mi)
        {
            const string InformationAttributeDescriptionPropertyName = "Description";

            foreach(var attrib in mi.GetCustomAttributes())
            {
                Type typeAttrib = attrib.GetType();

                if(typeAttrib.ToString().ToUpper() == InformationAttributeTypeName)
                {
                    PropertyInfo propertyInfo = typeAttrib.GetProperty(InformationAttributeDescriptionPropertyName);

                    if (propertyInfo is not null)
                    {
                        object s = propertyInfo.GetValue(attrib, null);

                        if (s != null)
                            return s.ToString();
                    }
                }
            }

            return null;
        }

        private static void DisplayElementDescription(string elementDescription)
        {
            if (elementDescription != null)
            {
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(elementDescription);
                Console.ResetColor();
                Console.WriteLine();
            }
        }

        private static void WriteResultToScreen(object result)
        {
            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Result: {result}");
            Console.ResetColor();
            Console.WriteLine();

        }

        private static bool HasInformationAttribute(MemberInfo mi)
        {
            foreach(var attrib in mi.GetCustomAttributes())
            {
                Type typeAttr = attrib.GetType();

                if (typeAttr.ToString().ToUpper() == InformationAttributeTypeName)
                {
                    return true;
                }
            }

            return false;
        }
        private static object[] ReturnParameterValueInputAsObjectArray(ParameterInfo[] parameters)
        {
            object[] paramValues = new object[parameters.Length];
            int itemCount = 0;

            foreach (ParameterInfo parameterInfo in parameters)
            {
                WritePromptToScreen($"Please enter a value for he parameter named, '{ parameterInfo.Name }' ");

                if(parameterInfo.ParameterType ==  typeof(string))
                {
                    string inputString = Console.ReadLine();
                    paramValues[itemCount] = inputString;
                }
                else if (parameterInfo.ParameterType == typeof(int))
                {
                    int inputInt = Int32.Parse(Console.ReadLine());
                    paramValues[itemCount] = inputInt;
                }
                else if (parameterInfo.ParameterType == typeof(double))
                {
                    double inputDouble = Double.Parse(Console.ReadLine());
                    paramValues[itemCount] = inputDouble;
                }

                itemCount++;
            }

            return paramValues;
        }

        private static object GetResult (Object classInstance, MethodInfo methodInfo, ParameterInfo[] parameters )
        {
            object result = null;

            if(parameters.Length == 0)
            {
                result = methodInfo.Invoke(classInstance, null);
            }
            else
            {
                var paramValueArray = ReturnParameterValueInputAsObjectArray(parameters);
                result = methodInfo.Invoke(classInstance, paramValueArray);
            }
            return result;
        }

        public static T ReturnProgramElementReferenceFromList<T>(List<T> list)
        {
            ConsoleKey consoleKey = Console.ReadKey().Key;

            switch (consoleKey)
            {
                case ConsoleKey.D1:
                    return list[0];
                case ConsoleKey.D2:
                    return list[1];
                case ConsoleKey.D3:
                    return list[2];
                case ConsoleKey.D4:
                    return list[3];
            }

            return default;
        }

        public static void DisplayProgramElementList<T>(List<T> list)
        {
            int count = 0;

            foreach (var item in list)
            {
                count++;
                Console.WriteLine($"{count}. {item}");
            }
        }

        private static void WriteHeadingToScreen(string heading)
        {
            Console.WriteLine(heading);
            Console.WriteLine(new string('-', heading.Length));
            Console.WriteLine();
        }

        private static void WritePromptToScreen(string promptText)
        {
            Console.WriteLine(promptText);
        }
    }
}