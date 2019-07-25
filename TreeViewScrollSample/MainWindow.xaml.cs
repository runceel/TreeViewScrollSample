using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace TreeViewScrollSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IEnumerable<Person> People { get; } = Enumerable.Range(1, 100)
            .Select(x => new Person
            {
                Name = $"Tanaka {x}",
                Children = Enumerable.Range(1, 10)
                    .Select(y => new Person
                    {
                        Name = $"Kimura {x}-{y}",
                    })
                    .ToArray(),
            })
            .ToArray();

        public MainWindow()
        {
            InitializeComponent();
            treeView.ItemsSource = People;
        }

        private async void ScrollButton_Click(object sender, RoutedEventArgs e)
        {
            // ContainersGenerated になるまで待つ
            Task waitUntilContainersGenerated(TreeViewItem container)
            {
                var tcs = new TaskCompletionSource<object>(container);
                void statusChanged(object _, EventArgs __)
                {
                    if (container.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                    {
                        tcs.SetResult(null);
                        container.ItemContainerGenerator.StatusChanged -= statusChanged;
                        return;
                    }
                }

                container.ItemContainerGenerator.StatusChanged += statusChanged;
                return tcs.Task;
            }

            // 一番最後の最後の要素にスクロールする予定
            var parent = People.Last();
            var target = parent.Children.Last();

            // ツリービュー直下の最後の要素のTreeViewItemを取得
            var container = (TreeViewItem)treeView.ItemContainerGenerator.ContainerFromItem(parent);
            // 開く
            container.IsExpanded = true;
            // 子の ItemContainerGenerator のステータスが Generated になるまで待つ
            if (container.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
            {
                await waitUntilContainersGenerated(container);
            }

            // スクロール先を取得してスクロール
            var targetContainer = (TreeViewItem)container.ItemContainerGenerator.ContainerFromItem(target);
            targetContainer.BringIntoView();
        }
    }
}
