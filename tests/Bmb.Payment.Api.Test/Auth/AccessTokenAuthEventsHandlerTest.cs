using Bmb.Payment.Api.Auth;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;

namespace Bmb.Payment.Api.Test.Auth;

[TestSubject(typeof(AccessTokenAuthEventsHandler))]
public class AccessTokenAuthEventsHandlerTest
{

    [Fact]
    public async Task MessageReceivedHandler_ShouldSetToken_WhenAccessTokenHeaderIsPresent()
    {
        // Arrange
        var handler = AccessTokenAuthEventsHandler.Instance;
        var context = new DefaultHttpContext();
        context.Request.Headers["AccessToken"] = "Bearer test_token";
        var messageReceivedContext = new MessageReceivedContext(
            context,
            new AuthenticationScheme("Bearer", null, typeof(JwtBearerHandler)),
            new JwtBearerOptions()
        );

        // Act
        await handler.MessageReceivedHandler(messageReceivedContext);

        // Assert
        messageReceivedContext.Token.Should().Be("test_token");
    }

    [Fact]
    public async Task MessageReceivedHandler_ShouldNotSetToken_WhenAccessTokenHeaderIsNotPresent()
    {
        // Arrange
        var handler = AccessTokenAuthEventsHandler.Instance;
        var context = new DefaultHttpContext();
        var messageReceivedContext = new MessageReceivedContext(
            context,
            new AuthenticationScheme("Bearer", null, typeof(JwtBearerHandler)),
            new JwtBearerOptions()
        );

        // Act
        await handler.MessageReceivedHandler(messageReceivedContext);

        // Assert
        messageReceivedContext.Token.Should().BeNull();
    }

    [Fact]
    public async Task MessageReceivedHandler_ShouldSetToken_WhenAccessTokenHeaderWithoutBearerPrefix()
    {
        // Arrange
        var handler = AccessTokenAuthEventsHandler.Instance;
        var context = new DefaultHttpContext();
        context.Request.Headers["AccessToken"] = "test_token";
        var messageReceivedContext = new MessageReceivedContext(
            context,
            new AuthenticationScheme("Bearer", null, typeof(JwtBearerHandler)),
            new JwtBearerOptions()
        );

        // Act
        await handler.MessageReceivedHandler(messageReceivedContext);

        // Assert
        messageReceivedContext.Token.Should().Be("test_token");
    }
}