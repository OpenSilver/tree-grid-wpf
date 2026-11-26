# TreeGrid.OpenSilver

A multi-column tree grid for [OpenSilver](https://opensilver.net), which runs WPF-style
XAML and C# in the browser. Hierarchical data binding, row and column virtualization,
sorting, Excel-style filtering, editing with validation, frozen panes, stacked headers,
grouping, footers, drag-and-drop, clipboard support and CSV export.

It is built from the same source code as
[`TreeGrid.Wpf`](https://www.nuget.org/packages/TreeGrid.Wpf), so the API and the XAML
are the same.

## Install

```
dotnet add package TreeGrid.OpenSilver
```

The package depends on `OpenSilver` and `OpenSilver.WpfCompat`.

## Quick start

The namespaces match the WPF package; only the assembly name differs:

```xml
xmlns:tg="clr-namespace:TreeGrid.Wpf;assembly=TreeGrid.OpenSilver"
xmlns:cols="clr-namespace:TreeGrid.Wpf.Columns;assembly=TreeGrid.OpenSilver"
```

The grid uses its light theme unless the app defines the `TreeGrid.*` brushes. With
[OpenSilver.Themes.Modern](https://github.com/OpenSilver/OpenSilver.Themes.Modern), it takes
the colors of the theme's palette instead, and follows its light and dark modes. To keep a
grid dark whatever the app's mode, merge the dark theme into its resources, or into the
application resources for every grid:

```xml
<ResourceDictionary Source="/TreeGrid.OpenSilver;component/Themes/FluentDark.xaml" />
```

Then declare the grid exactly as on WPF:

```xml
<tg:TreeGridControl ItemsSource="{Binding Staff}"
                    ChildPropertyName="Children"
                    AllowSorting="True"
                    AllowFiltering="True"
                    FrozenColumnCount="1">
    <tg:TreeGridControl.Columns>
        <cols:TreeGridTextColumn MappingName="FirstName" HeaderText="First Name" Width="220" />
        <cols:TreeGridNumericColumn MappingName="Salary" HeaderText="Salary" Width="110" />
        <cols:TreeGridProgressBarColumn MappingName="Completion" HeaderText="Progress" />
    </tg:TreeGridControl.Columns>
</tg:TreeGridControl>
```

## Browser differences

- The clipboard holds plain text only, and pasting reads it asynchronously: `Paste()`
  returns 0 and the outcome arrives through `PasteCompleted`. The browser may ask for
  permission the first time.
- Export is CSV only. `Export(exporter, stream)` writes synchronously, while the stream
  from OpenSilver's `SaveFileDialog` only supports `WriteAsync`, so export to a
  `MemoryStream` first and then write that to the file.
- Yes/No confirmations (bulk value change, row delete) are shown as OK/Cancel browser
  dialogs.
- Numeric columns do not filter keystrokes or pasted text while typing; values are still
  parsed and validated on commit.

## Documentation

The full guide is in the
[GitHub repository](https://github.com/gulshan-verma/WpfTreeGrid#readme), which also
has a runnable demo app.

## License

MIT
