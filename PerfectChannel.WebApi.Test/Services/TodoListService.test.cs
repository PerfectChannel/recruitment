using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace PerfectChannel.WebApi.Services.Tests
{
    [TestFixture]
    public class TodoListServiceTests
    {
        private ITodoListService _todoListService;

        private const string newTaskDescription = "new Task Description";
        private const string newTaskDescription2 = "new Task Description2";
        private const string NotFoundTaskId = "NotFoundTaskId";

        [SetUp]
        public void SetUp()
        {
            // Empty list for each test
            _todoListService = new TodoListService();
        }

        [Test]
        public void TodoListService_AddTask_Success()
        {
            var returned = _todoListService.AddTask(newTaskDescription);
            Assert.That(returned, Is.True);
        }

        [Test]
        public void TodoListService_AddTask_TheSameDescriptionSuccess()
        {
            // Arrange
            var returned = _todoListService.AddTask(newTaskDescription);
            Assert.That(returned, Is.True);

            // Act
            returned = _todoListService.AddTask(newTaskDescription);
            Assert.That(returned, Is.True);

            // Assert
            var list = _todoListService.GetList();
            var lists = JsonConvert.DeserializeObject<List<List<KeyValuePair<string, string>>>>(list);
            Assert.That(lists[0], Is.Not.Empty); // Pending
            Assert.That(lists[0].Count, Is.EqualTo(2)); // Pending
        }

        [Test]
        public void TodoListService_GetList_2ListsSuccess()
        {
            // Arrange
            _todoListService.AddTask(newTaskDescription);
            _todoListService.AddTask(newTaskDescription2);

            // Act
            var list = _todoListService.GetList();

            // Assert
            Assert.That(list, Is.Not.Empty);
            var lists = JsonConvert.DeserializeObject<List<List<KeyValuePair<string, string>>>> (list);
            
            Assert.That(lists[0], Is.Not.Empty); // Pending
            Assert.That(lists[1], Is.Empty); // Completed

            Assert.That(lists[0].Count, Is.EqualTo(2));
            Assert.That(lists[0][0].Value, Is.EqualTo(newTaskDescription));
            
            // Change newTaskDescription status to completed
            _todoListService.ChangeStatus(lists[0].Where(q => q.Value == newTaskDescription).Select(q => q.Key).First());
            
            // Act
            list = _todoListService.GetList();

            // Assert
            Assert.That(list, Is.Not.Empty);
            lists = JsonConvert.DeserializeObject<List<List<KeyValuePair<string, string>>>>(list);

            Assert.That(lists[0], Is.Not.Empty); // Pending
            Assert.That(lists[1], Is.Not.Empty); // Completed

            Assert.That(lists[0].Count, Is.EqualTo(1));
            Assert.That(lists[1].Count, Is.EqualTo(1));

            Assert.That(lists[0].First().Value, Is.EqualTo(newTaskDescription2));
            Assert.That(lists[1].First().Value, Is.EqualTo(newTaskDescription));
        }

        [Test]
        public void TodoListService_ChangeStatus_Success()
        {
            // Arrange
            _todoListService.AddTask(newTaskDescription);
            var list = _todoListService.GetList();
            var lists = JsonConvert.DeserializeObject<List<List<KeyValuePair<string, string>>>>(list);
            Assert.That(lists[0][0].Value, Is.EqualTo(newTaskDescription));

            // Act
            var returned = _todoListService.ChangeStatus(lists[0][0].Key);

            // Assert
            Assert.That(returned, Is.True);
            list = _todoListService.GetList();
            lists = JsonConvert.DeserializeObject<List<List<KeyValuePair<string, string>>>>(list);

            Assert.That(lists[0], Is.Empty); // Pending
            Assert.That(lists[1], Is.Not.Empty); // Completed

            Assert.That(lists[1].Count, Is.EqualTo(1));
            Assert.That(lists[1][0].Value, Is.EqualTo(newTaskDescription));
        }

        [Test]
        public void TodoListService_ChangeStatus_NotFoundKeyReturnsFalse()
        {
            // Arrange
            _todoListService.AddTask(newTaskDescription);
            var list = _todoListService.GetList();
            var lists = JsonConvert.DeserializeObject<List<List<KeyValuePair<string, string>>>>(list);
            Assert.That(lists[0][0].Value, Is.EqualTo(newTaskDescription));

            // Act
            var returned = _todoListService.ChangeStatus(NotFoundTaskId);

            // Assert
            Assert.That(returned, Is.False);
        }
    }
}
