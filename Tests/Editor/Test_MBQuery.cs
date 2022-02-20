using NUnit.Framework;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

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
            yield return null;
        }

        [Test]
        public void Test_AutoRefresh_True()
        {

        }

        [Test]
        public void Test_AutoRefresh_False()
        {

        }

        [Test]
        public void Test_Dirty()
        {

        }

        [Test]
        public void Test_ByName_Custom_Type()
        {

        }

        [Test]
        public void Test_ByName_Custom_Types()
        {

        }

        [Test]
        public void Test_ByName()
        {
            // Arrange.
            MBQuery query = new MBQuery();

            // Act.
            MonoBehaviour[] results = query.ByName("LookupOne").Values();

            // Assert.
            Assert.AreEqual(1, results.Length, "Expected the component the game object to hold one component but it didn't.");
        }

        [Test]
        public void Test_ByType()
        {
            // Arrange.
            MBQuery query = new MBQuery();

            // Act.
            MonoBehaviour[] results = query.ByType(typeof(TestComponent)).Values();
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
