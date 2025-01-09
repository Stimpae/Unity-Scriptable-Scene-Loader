using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Thundertale.SceneLoader.Editor {
    /// <summary>
    /// Provides utility methods for creating and styling editor UI elements.
    /// </summary>
    public static class EditorStyleUtils {
        /// <summary>
        /// Creates a styled header element for a collection.
        /// </summary>
        /// <param name="header">The text to display in the header.</param>
        /// <returns>A <see cref="VisualElement"/> representing the header.</returns>
        public static VisualElement CreateCollectionHeader(string header) {
            var headerElement = new VisualElement();
            headerElement.style.paddingTop = 3;
            headerElement.style.paddingBottom = 6;
            headerElement.style.paddingLeft = 7;
            headerElement.style.paddingRight = 5;
            headerElement.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            headerElement.style.borderTopWidth = 1;
            headerElement.style.borderLeftWidth = 1;
            headerElement.style.borderRightWidth = 1;
            headerElement.style.borderBottomWidth = 1;
            headerElement.style.borderTopColor = new StyleColor(new Color32(26, 26, 26, 255));
            headerElement.style.borderLeftColor = new StyleColor(new Color32(26, 26, 26, 255));
            headerElement.style.borderRightColor = new StyleColor(new Color32(26, 26, 26, 255));
            headerElement.style.borderBottomColor = new StyleColor(new Color32(26, 26, 26, 255));
            headerElement.style.borderTopLeftRadius = 3;
            headerElement.style.borderTopRightRadius = 3;
            headerElement.style.borderBottomLeftRadius = 3;
            headerElement.style.borderBottomRightRadius = 3;
            headerElement.style.translate = new Translate(0, 4f, 0);
            headerElement.Add(new Label(header));
            return headerElement;
        }

        /// <summary>
        /// Creates a styled splitter element.
        /// </summary>
        /// <param name="padding">The padding around the splitter.</param>
        /// <param name="marginCorrection">The margin correction for the splitter.</param>
        /// <returns>A <see cref="VisualElement"/> representing the splitter.</returns>
        public static VisualElement CreateSplitter(float padding, float marginCorrection = 21) {
            var splitter = new VisualElement();
            splitter.style.height = 2;
            splitter.style.borderTopWidth = 1;
            splitter.style.borderTopColor = new StyleColor(new Color32(26, 26, 26, 255));
            splitter.style.flexShrink = 0;
            splitter.style.flexGrow = 0;
            splitter.style.marginLeft = -marginCorrection;
            splitter.style.marginRight = -marginCorrection;
            splitter.style.marginTop = padding;
            splitter.style.marginBottom = padding;
            splitter.name = "Splitter";
            return splitter;
        }
        
        public static ListView CreateListView(string bindingPath, string header) {
            var listView = new ListView {
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                showBoundCollectionSize = false,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                reorderMode = ListViewReorderMode.Animated,
                reorderable = true,
                showBorder = true,
                showAddRemoveFooter = true,
                bindingPath = bindingPath,
                makeHeader = () => CreateCollectionHeader(header),
            };
            
            var scrollView = listView.Q<ScrollView>();
            scrollView.style.borderTopLeftRadius = 0;
            scrollView.style.borderTopRightRadius = 0;
            scrollView.style.paddingBottom = 0;
            scrollView.style.paddingTop = 0;
            
            return listView;
        }

        /// <summary>
        /// Creates a multi-column list view with a custom header.
        /// </summary>
        /// <param name="bindingPath">The binding path for the list view.</param>
        /// <param name="header">The text to display in the header.</param>
        /// <returns>A <see cref="MultiColumnListView"/> configured with the specified binding path and header.</returns>
        public static MultiColumnListView CreateMultiListView(string bindingPath, string header) {
            var multiColumnListView = new MultiColumnListView {
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                showBoundCollectionSize = false,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                reorderMode = ListViewReorderMode.Animated,
                reorderable = true,
                showBorder = true,
                showAddRemoveFooter = true,
                bindingPath = bindingPath,
                makeHeader = () => CreateCollectionHeader(header),
            };

            var scrollView = multiColumnListView.Q<ScrollView>();
            scrollView.style.borderTopLeftRadius = 0;
            scrollView.style.borderTopRightRadius = 0;
            scrollView.style.paddingBottom = 0;
            scrollView.style.paddingTop = 0;

            return multiColumnListView;
        }

        /// <summary>
        /// Adds a column to a multi-column list view.
        /// </summary>
        /// <param name="listView">The list view to add the column to.</param>
        /// <param name="bindingPath">The binding path for the column.</param>
        /// <param name="title">The title of the column.</param>
        /// <returns>The updated <see cref="MultiColumnListView"/> with the new column added.</returns>
        public static MultiColumnListView AddColumn(this MultiColumnListView listView, string bindingPath, string title) {
            var column = new Column {
                name = title + "Column",
                title = title,
                bindingPath = bindingPath,
                resizable = true,
                stretchable = true,
            };
            listView.columns.Add(column);
            return listView;
        }

        /// <summary>
        /// Adds a column to a multi-column list view.
        /// </summary>
        /// <param name="listView">The list view to add the column to.</param>
        /// <param name="bindingPath">The binding path for the column.</param>
        /// <param name="title">The title of the column.</param>
        /// <param name="minWidth"></param>
        /// <param name="maxWidth"></param>
        /// <returns>The updated <see cref="MultiColumnListView"/> with the new column added.</returns>
        public static MultiColumnListView AddColumn(this MultiColumnListView listView, string bindingPath, string title, float minWidth, float maxWidth) {
            var column = new Column {
                name = title + "Column",
                title = title,
                bindingPath = bindingPath,
                resizable = true,
                stretchable = true,
                minWidth = new Length(minWidth, LengthUnit.Pixel),
                maxWidth = new Length(maxWidth, LengthUnit.Pixel),
            };
            listView.columns.Add(column);
            return listView;
        }
        
        public static VisualElement CreateHelpBox(string text, HelpBoxMessageType messageType, Color32 backgroundColor, float padding) {
            var helpBox = new HelpBox(text, messageType);
            helpBox.style.backgroundColor = new StyleColor(backgroundColor);
            helpBox.style.paddingTop = padding;
            helpBox.style.paddingBottom = padding;
            helpBox.style.paddingLeft = padding;
            helpBox.style.paddingRight = padding;
            return helpBox;
        }
        
        /// <summary>
        /// Creates a space block element with specified height and width.
        /// </summary>
        /// <param name="height">The height of the space block.</param>
        /// <param name="width">The width of the space block.</param>
        /// <returns>A <see cref="VisualElement"/> representing the space block.</returns>
        public static VisualElement CreateSpaceBlock(float height, float width) {
            var spaceBlock = new VisualElement();
            spaceBlock.style.height = height;
            spaceBlock.style.width = width;
            spaceBlock.style.alignSelf = Align.Center;
            spaceBlock.style.flexShrink = 0;
            return spaceBlock;
        }
    }
}