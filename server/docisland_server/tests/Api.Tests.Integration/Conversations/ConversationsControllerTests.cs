using System.Net;
using System.Text;
using Api.Dtos;
using Domain.Conversations;
using Domain.Roles;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;

namespace Api.Tests.Integration.Conversations;
using File = Domain.Files.File;

public class ConversationsControllerTests: BaseIntegrationTest, IAsyncLifetime
{
    private readonly File _newFile;
    private readonly Conversation _newConversation;
    private readonly User _mainUser;
    private readonly Role _userRole = RolesData.UserRole;
    private const string TestPassword = "TestPass123!";
    
    public ConversationsControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser();
        _newFile = FilesData.NewFile(_mainUser.Id);
        _newConversation = ConversationsData.NewConversation(_mainUser.Id, _newFile.Id);
    }

    [Fact]
    public async Task ShouldCreateConversation()
    {
        // Arrange
        const string content = "";
        const string fileName = "empty.txt";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");

        using var form = new MultipartFormDataContent();
        form.Add(fileContent, "file", fileName);

        // Act
        var response = await Client.PostAsync("conversations/add", form);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var responseConversation = await response.ToResponseModel<ConversationDto>();

        var createdConversationId = new ConversationId(responseConversation.Id);

        var dbConversation = await Context.Conversations.FirstOrDefaultAsync(x => x.Id == createdConversationId);
        dbConversation.Should().NotBeNull();
    }

    
    [Fact]
    public async Task ShouldNotCreateConversationBecauseFileTypeNotSupported()
    {
        // Arrange
        const string content = "";
        const string fileName = "empty.xml";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml");

        using var form = new MultipartFormDataContent();
        form.Add(fileContent, "file", fileName);

        // Act
        var response = await Client.PostAsync("conversations/add", form);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
    }

    
    [Fact]
    public async Task ShouldDeleteConversation()
    {
        // Arrange
        var conversationId = _newConversation.Id;

        // Act
        var response = await Client.DeleteAsync($"conversations/delete/{conversationId.Value}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var dbConversation = await Context.Conversations.FirstOrDefaultAsync(x => x.Id == conversationId);
        dbConversation.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldNotDeleteConversationBecauseNotFound()
    {
        // Arrange
        var conversationId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"conversations/delete/{conversationId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    
    public async Task InitializeAsync()
    {
        await RoleManager.CreateAsync(_userRole);
        await UserManager.CreateAsync(_mainUser, TestPassword);
        await Context.Files.AddAsync(_newFile);
        Context.Conversations.Add(_newConversation);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Conversations.RemoveRange(Context.Conversations);
        Context.Files.RemoveRange(Context.Files);
        Context.Users.RemoveRange(Context.Users);
        Context.Roles.RemoveRange(Context.Roles);
        await SaveChangesAsync();
    }
}