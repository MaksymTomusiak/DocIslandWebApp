namespace Application.Common.Interfaces.Services.Views;

public interface IViewRenderer
{
    string RenderView<TModel>(string viewName, TModel model, string email, string subject);
}