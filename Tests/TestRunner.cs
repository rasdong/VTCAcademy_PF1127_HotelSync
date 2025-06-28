using NUnit.Framework;
using System;
using System.Reflection;

namespace HotelManagementSystem.Tests
{
    /// <summary>
    /// Test program để chạy tất cả tests
    /// </summary>
    public class TestRunner
    {
        /// <summary>
        /// Entry point cho test runner
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public static void Main(string[] args)
        {
            // Chỉ chạy khi có argument --test-runner
            if (args.Length == 0 || args[0] != "--test-runner")
            {
                Console.WriteLine("Use --test-runner argument to run tests");
                return;
            }

            Console.WriteLine("=== Hotel Management System - Test Runner ===");
            Console.WriteLine($"Started at: {DateTime.Now}");
            Console.WriteLine();

            try
            {
                // Chạy tất cả tests trong assembly hiện tại
                var testResult = RunAllTests();
                
                Console.WriteLine();
                Console.WriteLine("=== Test Summary ===");
                Console.WriteLine($"Total Tests: {testResult.TotalTests}");
                Console.WriteLine($"Passed: {testResult.PassedTests}");
                Console.WriteLine($"Failed: {testResult.FailedTests}");
                Console.WriteLine($"Skipped: {testResult.SkippedTests}");
                Console.WriteLine();
                
                if (testResult.FailedTests > 0)
                {
                    Console.WriteLine("❌ Some tests failed!");
                    Environment.ExitCode = 1;
                }
                else
                {
                    Console.WriteLine("✅ All tests passed!");
                    Environment.ExitCode = 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error running tests: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Environment.ExitCode = 1;
            }
            
            Console.WriteLine($"Finished at: {DateTime.Now}");
            
            // Đợi người dùng nhấn phím nếu chạy từ IDE
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Chạy tất cả tests và trả về kết quả
        /// </summary>
        /// <returns>Test result summary</returns>
        private static TestResult RunAllTests()
        {
            var result = new TestResult();
            
            // Lấy tất cả test classes
            var assembly = Assembly.GetExecutingAssembly();
            var testClasses = GetTestClasses(assembly);
            
            Console.WriteLine($"Found {testClasses.Length} test classes:");
            foreach (var testClass in testClasses)
            {
                Console.WriteLine($"- {testClass.Name}");
            }
            Console.WriteLine();
            
            // Chạy tests cho từng class
            foreach (var testClass in testClasses)
            {
                RunTestsForClass(testClass, result);
            }
            
            return result;
        }

        /// <summary>
        /// Lấy tất cả test classes từ assembly
        /// </summary>
        /// <param name="assembly">Assembly to scan</param>
        /// <returns>Array of test classes</returns>
        private static Type[] GetTestClasses(Assembly assembly)
        {
            var testClasses = new System.Collections.Generic.List<Type>();
            
            foreach (var type in assembly.GetTypes())
            {
                if (type.GetCustomAttribute<TestFixtureAttribute>() != null)
                {
                    testClasses.Add(type);
                }
            }
            
            return testClasses.ToArray();
        }

        /// <summary>
        /// Chạy tests cho một class cụ thể
        /// </summary>
        /// <param name="testClass">Test class to run</param>
        /// <param name="result">Result object to update</param>
        private static void RunTestsForClass(Type testClass, TestResult result)
        {
            Console.WriteLine($"Running tests for {testClass.Name}...");
            
            try
            {
                // Tạo instance của test class
                var instance = Activator.CreateInstance(testClass);
                if (instance == null)
                {
                    Console.WriteLine($"  ❌ Could not create instance of {testClass.Name}");
                    result.FailedTests++;
                    return;
                }
                
                // Chạy OneTimeSetUp nếu có
                RunOneTimeSetUp(instance);
                
                // Lấy tất cả test methods
                var testMethods = GetTestMethods(testClass);
                
                foreach (var method in testMethods)
                {
                    RunSingleTest(instance, method, result);
                }
                
                // Chạy OneTimeTearDown nếu có
                RunOneTimeTearDown(instance);
                
                Console.WriteLine($"  Completed {testClass.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ❌ Error in {testClass.Name}: {ex.Message}");
                result.FailedTests++;
            }
            
            Console.WriteLine();
        }

        /// <summary>
        /// Lấy tất cả test methods từ một class
        /// </summary>
        /// <param name="testClass">Test class</param>
        /// <returns>Array of test methods</returns>
        private static MethodInfo[] GetTestMethods(Type testClass)
        {
            var testMethods = new System.Collections.Generic.List<MethodInfo>();
            
            foreach (var method in testClass.GetMethods())
            {
                if (method.GetCustomAttribute<TestAttribute>() != null)
                {
                    testMethods.Add(method);
                }
            }
            
            return testMethods.ToArray();
        }

        /// <summary>
        /// Chạy OneTimeSetUp method nếu có
        /// </summary>
        /// <param name="instance">Test instance</param>
        private static void RunOneTimeSetUp(object instance)
        {
            var methods = instance.GetType().GetMethods();
            foreach (var method in methods)
            {
                if (method.GetCustomAttribute<OneTimeSetUpAttribute>() != null)
                {
                    method.Invoke(instance, null);
                    break;
                }
            }
        }

        /// <summary>
        /// Chạy OneTimeTearDown method nếu có
        /// </summary>
        /// <param name="instance">Test instance</param>
        private static void RunOneTimeTearDown(object instance)
        {
            var methods = instance.GetType().GetMethods();
            foreach (var method in methods)
            {
                if (method.GetCustomAttribute<OneTimeTearDownAttribute>() != null)
                {
                    method.Invoke(instance, null);
                    break;
                }
            }
        }

        /// <summary>
        /// Chạy SetUp method nếu có
        /// </summary>
        /// <param name="instance">Test instance</param>
        private static void RunSetUp(object instance)
        {
            var methods = instance.GetType().GetMethods();
            foreach (var method in methods)
            {
                if (method.GetCustomAttribute<SetUpAttribute>() != null)
                {
                    method.Invoke(instance, null);
                    break;
                }
            }
        }

        /// <summary>
        /// Chạy TearDown method nếu có
        /// </summary>
        /// <param name="instance">Test instance</param>
        private static void RunTearDown(object instance)
        {
            var methods = instance.GetType().GetMethods();
            foreach (var method in methods)
            {
                if (method.GetCustomAttribute<TearDownAttribute>() != null)
                {
                    method.Invoke(instance, null);
                    break;
                }
            }
        }

        /// <summary>
        /// Chạy một test method cụ thể
        /// </summary>
        /// <param name="instance">Test instance</param>
        /// <param name="method">Test method</param>
        /// <param name="result">Result object to update</param>
        private static void RunSingleTest(object instance, MethodInfo method, TestResult result)
        {
            result.TotalTests++;
            
            try
            {
                Console.Write($"    {method.Name}... ");
                
                // Chạy SetUp
                RunSetUp(instance);
                
                // Chạy test method
                method.Invoke(instance, null);
                
                // Chạy TearDown
                RunTearDown(instance);
                
                Console.WriteLine("✅ PASSED");
                result.PassedTests++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FAILED: {ex.InnerException?.Message ?? ex.Message}");
                result.FailedTests++;
                
                // In stack trace nếu cần debug
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    Console.WriteLine($"      Stack trace: {ex.InnerException?.StackTrace ?? ex.StackTrace}");
                }
            }
        }
    }

    /// <summary>
    /// Lưu trữ kết quả test
    /// </summary>
    public class TestResult
    {
        public int TotalTests { get; set; }
        public int PassedTests { get; set; }
        public int FailedTests { get; set; }
        public int SkippedTests { get; set; }
    }
}
