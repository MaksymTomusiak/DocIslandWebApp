using System.Net;
using System.Net.Http.Json;
using Api.Dtos;
using Application.Common.Interfaces.Services.LLM;
using Domain.Conversations;
using Domain.Messages;
using Domain.Roles;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tests.Common;
using Tests.Data;
using File = Domain.Files.File;

namespace Api.Tests.Integration.Messages;

public class MessagesControllerTests: BaseIntegrationTest, IAsyncLifetime
{
    private readonly File _newFile;
    private readonly Message _newMessage;
    private readonly Conversation _newConversation;
    private readonly User _mainUser;
    private readonly Role _userRole = RolesData.UserRole;
    private const string TestPassword = "TestPass123!";
    private readonly Mock<ILlmService> _llmServiceMock;
    
    public MessagesControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser();
        _newFile = FilesData.NewFile(_mainUser.Id);
        _newConversation = ConversationsData.NewConversation(_mainUser.Id, _newFile.Id);
        _newMessage = MessagesData.NewMessage(_newConversation.Id);
        var token = TestsExtensions.GenerateMockJwt(_mainUser.Id);
        SetCustomAuthorizationHeader(token);
        _llmServiceMock = factory.LlmServiceMock;
    }

    [Fact]
    public async Task ShouldCreateMessage()
    {
        // Arrange
        var request = new MessageCreateDto(_newConversation.Id.Value, _newMessage.Content);
        
        // Act
        var response = await Client.PostAsJsonAsync("messages/add", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var responseMessage = await response.ToResponseModel<MessageDto>();

        var createdMessageId = new MessageId(responseMessage.Id);
        
        var dbMessage = await Context.Messages.FirstOrDefaultAsync(x => x.Id == createdMessageId);

        dbMessage.Should().NotBeNull();
        dbMessage?.Content.Should().NotBeNull();
        dbMessage?.ConversationId.Value.Should().Be(request.ConversationId);
        _llmServiceMock.Verify(
            x => x.AskQuestionAsync(
                It.Is<Guid>(id => id == _newFile.Id.Value),
                It.Is<string>(content => content == _newMessage.Content),
                It.IsAny<CancellationToken>()),
            Times.Once());
    }
    
    [Fact]
    public async Task ShouldNotCreateMessageBecauseConversationNotFound()
    {
        // Arrange
        var request = new MessageCreateDto(Guid.NewGuid(), _newMessage.Content);

        // Act
        var response = await Client.PostAsJsonAsync("messages/add", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        _llmServiceMock.Verify(
            x => x.AskQuestionAsync(
                It.Is<Guid>(id => id == _newFile.Id.Value),
                It.Is<string>(content => content == _newMessage.Content),
                It.IsAny<CancellationToken>()),
            Times.Never());
    }
    
    [Fact]
    public async Task ShouldDeleteMessage()
    {
        // Arrange
        var messageId = _newMessage.Id;

        // Act
        var response = await Client.DeleteAsync($"messages/delete/{messageId.Value}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var dbMessage = await Context.Messages.FirstOrDefaultAsync(x => x.Id == messageId);
        dbMessage.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldNotDeleteMessageBecauseNotFound()
    {
        // Arrange
        var messageId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"messages/delete/{messageId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    
    public async Task InitializeAsync()
    {
        await RoleManager.CreateAsync(_userRole);
        await UserManager.CreateAsync(_mainUser, TestPassword);
        await Context.Files.AddAsync(_newFile);
        Context.Conversations.Add(_newConversation);
        Context.Messages.Add(_newMessage);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Messages.RemoveRange(Context.Messages);
        Context.Conversations.RemoveRange(Context.Conversations);
        Context.Files.RemoveRange(Context.Files);
        Context.Users.RemoveRange(Context.Users);
        Context.Roles.RemoveRange(Context.Roles);
        await SaveChangesAsync();
    }
}