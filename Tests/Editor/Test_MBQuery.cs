using NUnit.Framework;
using System;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace BWolf.MonoBehaviourQuerying.Tests.Editor
{
    public class Test_MBQuery
    {
        private Scene _currentScene;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _currentScene = EditorSceneManager.OpenScene("Packages/nl.bwolf.monobehaviourquerying/Tests/Editor/Scenes/MBQuery_Test_Scene.unity", OpenSceneMode.Additive);
        }

        [UnitySetUp]
        public IEnumerator TestSetup()
        {
            // Ensure a one frame pause before each test to ensure correct
            // opening of scene and destruction of test game objects.
            yield return null;
        }

        [Test]
        public void Test_AutoRefresh_True()
        {
            // Arrange.
            SceneQuery query = new SceneQuery(true);
            Component[] initial = query.OnType(typeof(TestComponent)).Values();

            // Act.
            TestComponent created = new GameObject("Test_GameObject").AddComponent<TestComponent>();
            Component[] seconds = query.Values();

            // Assert.
            Assert.AreNotEqual(initial.Length, seconds.Length,
                "Expected the created game object to be included in the query but it wasn't.");

            // Cleanup.
            GameObject.DestroyImmediate(created);
        }

        [Test]
        public void Test_AutoRefresh_False()
        {
            // Arrange.
            SceneQuery query = new SceneQuery();
            Component[] initial = query.OnType(typeof(TestComponent)).Values();

            // Act.
            TestComponent created = new GameObject("Test_GameObject").AddComponent<TestComponent>();
            Component[] seconds = query.Values();

            // Assert.
            Assert.AreEqual(initial.Length, seconds.Length,
                "Expected the created game object not to be included in the query but it was.");

            // Cleanup.
            GameObject.DestroyImmediate(created);
        }

        [Test]
        public void Test_Dirty()
        {
            // Arrange.
            SceneQuery query = new SceneQuery();
            Component[] initial = query.OnType(typeof(TestComponent)).Values();

            // Act.
            TestComponent created = new GameObject("Test_GameObject").AddComponent<TestComponent>();
            Component[] seconds = query.Dirty().Values();

            // Assert.
            Assert.AreNotEqual(initial.Length, seconds.Length,
                "Expected the created game object to be included in the query but it wasn't.");

            // Cleanup.
            GameObject.DestroyImmediate(created);
        }

        [Test]
        public void Test_ByName_Custom_Type()
        {
            // Arrange.
            SceneQuery query = new SceneQuery();

            // Act.
            Component[] results = query.OnName("Test_ByName_Custom_Type", typeof(TestComponent)).Values();

            // Assert.
            Assert.AreEqual(2, results.Length, "Expected only the two test components to be found they weren't.");
        }

        [Test]
        public void Test_ByName_Custom_Types()
        {
            // Arrange.
            SceneQuery query = new SceneQuery();

            // Act.
            Type[] componentTypes = new Type[] { typeof(TestComponent), typeof(CustomComponent) };
            Component[] results = query.OnName("Test_ByName_Custom_Types", componentTypes).Values();

            // Assert.
            Assert.AreEqual(2, results.Length, "Expected both component types to be found but they weren't.");
        }

        [Test]
        public void Test_ByName()
        {
            // Arrange.
            SceneQuery query = new SceneQuery();

            // Act.
            Component[] results = query.OnName("Test_ByName").Values();

            // Assert.
            Assert.AreEqual(2, results.Length, "Expected the game object to hold two components but it didn't.");
        }

        [Test]
        public void Test_ByName_None()
        {
            // Arrange.
            SceneQuery query = new SceneQuery();

            // Act.
            Component[] results = query.OnName("Test_ByName_None").Values();

            // Assert.
            Assert.AreEqual(1, results.Length, "Expected the component the game object to hold one component but it didn't.");
        }

        [Test]
        public void Test_ByTag()
        {
            // Arrange.
            SceneQuery query = new SceneQuery();

            // Act.
            Component[] results = query.OnTag("Player", typeof(TestComponent)).Values();

            // Assert.
            Assert.AreEqual(1, results.Length, "Expected the test component to be found on the tagged game object but it wasn't.");
        }

        [Test]
        public void Test_ByType()
        {
            // Arrange.
            SceneQuery query = new SceneQuery();

            // Act.
            Component[] results = query.OnType(true, typeof(TestComponent)).Values();
            Object[] expected = Resources.FindObjectsOfTypeAll(typeof(TestComponent));

            // Assert.
            Assert.AreEqual(expected.Length, results.Length, $"Expected {expected.Length} test components to be found but there were {results.Length}.");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            EditorSceneManager.CloseScene(_currentScene, true);
        }
    }
}
