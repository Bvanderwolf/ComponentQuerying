using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace BWolf.MonoBehaviourQuerying.Tests.Editor
{
    public class Test_ComponentQuery
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
        public void Test_Values_None()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();

            // Act.
            Component[] results = query.Values();

            // Assert.
            Assert.IsEmpty(results, "Expected no values to be returned but there were.");
        }

        [Test]
        public void Test_Value_None()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();

            // Act.
            Component result = query.Value();

            // Assert.
            Assert.IsNull(result, "Expected no value to be returned but there was.");
        }

        [Test]
        public void Test_Values_Generic_None()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();

            // Act.
            TestComponent[] results = query.Values<TestComponent>();

            // Assert.
            Assert.IsEmpty(results, "Expected no values to be returned but there were.");
        }

        [Test]
        public void Test_Value_Generic_None()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();

            // Act.
            TestComponent result = query.Value<TestComponent>();

            // Assert.
            Assert.IsNull(result, "Expected no value to be returned but there was.");
        }

        [Test]
        public void Test_AutoRefresh_True()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery(true);
            Component[] initial = query.OnType(typeof(TestComponent)).Values();

            // Act.
            TestComponent created = new GameObject("Test_GameObject").AddComponent<TestComponent>();
            Component[] seconds = query.Values();

            // Assert.
            Assert.AreNotEqual(initial.Length, seconds.Length);

            // Cleanup.
            GameObject.DestroyImmediate(created);
        }

        [Test]
        public void Test_AutoRefresh_False()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();
            Component[] initial = query.OnType(typeof(TestComponent)).Values();

            // Act.
            TestComponent created = new GameObject("Test_GameObject").AddComponent<TestComponent>();
            Component[] seconds = query.Values();

            // Assert.
            Assert.AreEqual(initial.Length, seconds.Length);

            // Cleanup.
            GameObject.DestroyImmediate(created);
        }

        [Test]
        public void Test_Dirty()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();
            Component[] initial = query.OnType(typeof(TestComponent)).Values();

            // Act.
            TestComponent created = new GameObject("Test_GameObject").AddComponent<TestComponent>();
            Component[] seconds = query.Dirty().Values();

            // Assert.
            Assert.AreNotEqual(initial.Length, seconds.Length);

            // Cleanup.
            GameObject.DestroyImmediate(created);
        }

        [Test]
        public void Test_Clear()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery().OnTag("Player");

            // Act.
            Component result = query.Clear().Value();

            // Assert.
            Assert.IsNull(result, "Expected the result after the clear to be null but it wasn't.");
        }


        [Test]
        public void Test_OnChildren_All()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();
            Component parent = query.OnName("Test_OnChildren").Value();

            // Act.
            Component[] results = query.Clear().OnChildren(parent).Values();

            // Assert.
            int expected = 18;
            Assert.AreEqual(expected, results.Length);
        }

        [Test]
        public void Test_OnChildren_Type()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();
            Component parent = query.OnName("Test_OnChildren").Value();

            // Act.
            Component[] results = query.Clear().OnChildren(parent, typeof(TestComponent), typeof(CustomComponent)).Values();

            // Assert.
            int expected = 10;
            Assert.AreEqual(expected, results.Length);
        }

        [Test]
        public void Test_OnChildren_Generic()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();
            Component parent = query.OnName("Test_OnChildren").Value();

            // Act.
            Component[] results = query.Clear().OnChildren<CustomComponent>(parent).Values();

            // Assert.
            int expected = 4;
            Assert.AreEqual(expected, results.Length);
        }

        [Test]
        public void Test_OnParent_All()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();
            Component child = query.OnName("Test_OnParent").Value();

            // Act.
            Component[] results = query.Clear().OnParent(child).Values();

            // Assert.
            int expected = 5;
            Assert.AreEqual(expected, results.Length);
        }

        [Test]
        public void Test_OnParent_Type()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();
            Component child = query.OnName("Test_OnParent").Value();

            // Act.
            Component[] results = query.Clear().OnParent(child, typeof(TestComponent)).Values();

            // Assert.
            int expected = 2;
            Assert.AreEqual(expected, results.Length);
        }

        [Test]
        public void Test_OnParent_Generic()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();
            Component child = query.OnName("Test_OnParent").Value();

            // Act.
            Component[] results = query.Clear().OnParent<TestComponent>(child).Values();

            // Assert.
            int expected = 2;
            Assert.AreEqual(expected, results.Length);
        }

        [Test]
        public void Test_OnGameObject_All()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();
            Component given = query.OnName("Test_OnGameObject").Value();

            // Act.
            Component[] results = query.Clear().OnGameObject(given.gameObject).Values();

            // Assert.
            int expected = 3;
            Assert.AreEqual(expected, results.Length);
        }

        [Test]
        public void Test_OnGameObject_Type()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();
            Component given = query.OnName("Test_OnGameObject").Value();

            // Act.
            Component[] results = query.Clear().OnGameObject(given.gameObject, typeof(TestComponent), typeof(CustomComponent)).Values();

            // Assert.
            int expected = 2;
            Assert.AreEqual(expected, results.Length);
        }

        [Test]
        public void Test_OnGameObject_Generic()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();
            Component given = query.OnName("Test_OnGameObject").Value();

            // Act.
            Component[] results = query.Clear().OnGameObject<TestComponent>(given.gameObject).Values();

            // Assert.
            int expected = 1;
            Assert.AreEqual(expected, results.Length);
        }

        [Test]
        public void Test_Use_Null()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();

            // Act.
            IComponentQuery usable = null;
            TestDelegate action = () => query.Use(usable);

            // Assert.
            Assert.Catch<ArgumentNullException>(action, "Expected the usage of a null query to cause an exception but it didn't.");
        }

        [Test]
        public void Test_Use()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();
            ComponentQuery usable = new ComponentQuery().OnTag("Player");

            // Act.
            Component result = query.Use(usable).Value();

            // Assert.
            Assert.NotNull(result, "Expected the usable query to be used and be successful in finding a component in the scene.");
        }

        [Test]
        public void Test_Use_Multiple_Null()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();

            // Act.
            IEnumerable<IComponentQuery> usables = null;
            TestDelegate action = () => query.Use(usables);

            // Assert.
            Assert.Catch<ArgumentNullException>(action, "Expected the usage of a null enumerable to cause an exception but it didn't.");
        }

        [Test]
        public void Test_Use_Multiple()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();
            ComponentQuery[] usables = new ComponentQuery[]
            {
                new ComponentQuery().OnTag("Player"),
                new ComponentQuery().OnName("Test_OnName"),
            };

            // Act.
            Component[] results = query.Use(usables).Values();

            // Assert.
            int expected = 5; // Transform x2 + TestComponent x2 + AudioSource x1
            Assert.AreEqual(expected, results.Length, "Expected the usable queries to be used and be successful in finding a components in the scene.");
        }


        [Test]
        public void Test_OnName_Custom_Type()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();

            // Act.
            Component[] results = query.OnName("Test_OnName_Custom_Type", typeof(TestComponent)).Values();

            // Assert.
            Assert.AreEqual(2, results.Length, "Expected only the two test components to be found they weren't.");
        }

        [Test]
        public void Test_OnName_Custom_Types()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();

            // Act.
            Type[] componentTypes = new Type[] { typeof(TestComponent), typeof(CustomComponent) };
            Component[] results = query.OnName("Test_OnName_Custom_Types", componentTypes).Values();

            // Assert.
            Assert.AreEqual(2, results.Length, "Expected both component types to be found but they weren't.");
        }

        [Test]
        public void Test_OnName_All()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();

            // Act.
            Component[] results = query.OnName("Test_OnName").Values();

            // Assert.
            Assert.AreEqual(2, results.Length, "Expected the game object to hold two components but it didn't.");
        }

        [Test]
        public void Test_OnName_Type_None()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();

            // Act.
            Component[] results = query.OnName("Test_OnName_None", typeof(CustomComponent)).Values();

            // Assert.
            Assert.IsEmpty(results, "Expected the component the game object to not hold the component but it did.");
        }

        [Test]
        public void Test_OnName_Generic()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();

            // Act.
            Component result = query.OnName<TestComponent>("Test_OnName").Value();

            // Assert.
            Assert.NotNull(result, "Expected the test component to be found on the game object but it wasn't.");
        }

        [Test]
        public void Test_OnTag_Type()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();

            // Act.
            Component[] results = query.OnTag("Player", typeof(TestComponent)).Values();

            // Assert.
            Assert.AreEqual(1, results.Length, "Expected the test component to be found on the tagged game object but it wasn't.");
        }

        [Test]
        public void Test_OnTag_All()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();

            // Act.
            Component[] results = query.OnTag("Player").Values();

            // Assert.
            int expected = 3;
            Assert.AreEqual(expected, results.Length, "Expected the test component to be found on the tagged game object but it wasn't.");
        }

        [Test]
        public void Test_OnTag_Generic()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();

            // Act.
            Component result = query.OnTag<AudioSource>("Player").Value();

            // Assert.
            Assert.NotNull(result, "Expected the audio source to be found on the tagger object but it wasn't.");
        }

        [Test]
        public void Test_OnType()
        {
            // Arrange.
            ComponentQuery query = new ComponentQuery();

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
