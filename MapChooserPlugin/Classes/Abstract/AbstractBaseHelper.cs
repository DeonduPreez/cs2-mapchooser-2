using MapChooserPlugin.Interfaces;

namespace MapChooserPlugin.Classes.Abstract;

public abstract class AbstractBaseHelper
{
    protected readonly ILogHelper LogHelper;

    protected AbstractBaseHelper(ILogHelper logHelper)
    {
        LogHelper = logHelper;
    }
}