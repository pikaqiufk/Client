public interface IUIScrollViewOptimizedWidgetProvider
{
    void BeforeDestroy(UIWidget widget);
    int Count();
    UIWidget GetWidget(int index);
    bool HasNextWidget(int index);
    void RequestUpperWidget();
}